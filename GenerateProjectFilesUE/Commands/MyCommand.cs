using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Microsoft.Win32;

namespace GenerateProjectFilesUE
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        //  
        /// <summary>
        /// Looks into registry for 'Windows' context menu command to generate project files
        /// </summary>
        /// <returns>
        /// "{path}" /projectfiles "%1" - if exists <br/>
        /// empty string - otherwise
        /// </returns>
        async Task<string> TryGetGenCommandhViaRegistryAsync()
        {
            try
            {
                const string root = "HKEY_CLASSES_ROOT";
                const string subkey = "Unreal.ProjectFile\\shell\\rungenproj\\command";
                const string keyName = root + "\\" + subkey;

                var regValue = Registry.GetValue(keyName, "", null);
                if (regValue != null) return regValue as string;
            }
            catch (Exception ex)
            {
                await VS.MessageBox.ShowWarningAsync(ex.Message);
            }

            return null;
        }

        /// <param name="command">"{path}" /projectfiles "%1" </param>
        /// <returns>{path} - if correct<br/>
        /// empty string - otherwise</returns>
        string GetPathFromCommand(string command)
        {
            var first = command.IndexOf('"');
            if (first != -1)
            {
                var second = command.IndexOf('"', first + 1);
                if (second != -1)
                {
                    var path = command.Substring(first + 1, second - first - 1);
                    return path;
                }
            }

            return null;
        }
        /// <summary>
        /// Make the same command ue uses, if UVS is found
        ///  
        /// </summary>
        /// <returns>"{path}" /projectfiles "%1" - if UVS is valid<br/>
        /// empty string - otherwise</returns>
        async Task<string> GetProjectGenCommandAsync()
        {
            var options = await GeneralOptions.GetLiveInstanceAsync();
            if (!String.IsNullOrEmpty(options.UnrealVersionSelectorPath) && System.IO.File.Exists(options.UnrealVersionSelectorPath))
            {
                return $"\"{options.UnrealVersionSelectorPath}\" /projectfiles \"%1\"";
            }

            return null;
        }

        public async Task TrySetSettingsAsync()
        {
            var options = await GeneralOptions.GetLiveInstanceAsync();
            if (String.IsNullOrEmpty(options.UnrealVersionSelectorPath))
            {
                var command = await TryGetGenCommandhViaRegistryAsync();
                if (!String.IsNullOrEmpty(command))
                {
                    var path = GetPathFromCommand(command);
                    if (!String.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                    {
                        options.UnrealVersionSelectorPath = path;
                        await options.SaveAsync();
                    }
                }
            }
        }

        protected override async Task InitializeCompletedAsync()
        {
            Command.Supported = false;
            // to let user see the options
            await TrySetSettingsAsync();
        }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            // to gather path automatically if user leaves the path empty
            await TrySetSettingsAsync();
            var activeItem = await VS.Solutions.GetActiveItemAsync();
            if (activeItem is PhysicalFile file)
            {
                if (file.Extension.Equals(".uproject"))
                {
                    var projectGenCommand = await GetProjectGenCommandAsync();
                    if (!String.IsNullOrEmpty(projectGenCommand))
                    {
                        var command = projectGenCommand.Replace("%1", file.Name);
                        command = "\"" + command + "\"";
                        //"\"X:\\Epic Games\\Launcher\\Engine\\Binaries\\Win64\\UnrealVersionSelector.exe\" /projectfiles \"X:\\Repositories\\Games\\Playground\\Playground.uproject\""
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized,
                            UseShellExecute = true,
                            FileName = "cmd.exe",
                            Arguments = "/C " + command
                        };
                        process.StartInfo = startInfo;

                        if (!process.Start())
                        {
                            await VS.MessageBox.ShowAsync("Can't start shell");
                        }
                    }
                    else
                    {
                        await VS.MessageBox.ShowAsync(
                            "Can't find UnrealVersionSelector / Can't make a command. Please set path to UVS in Tools -> Options -> GenerateProjectFilesUE");
                    }
                }
            }
        }

        protected override void BeforeQueryStatus(EventArgs e)
        {
            var currentItem =
                ThreadHelper.JoinableTaskFactory.Run(async () => await VS.Solutions.GetActiveItemAsync());

            Command.Enabled = false;
            Command.Visible = false;
            if (currentItem is PhysicalFile file)
            {
                if (file.Extension.Equals(".uproject"))
                {
                    Command.Enabled = true;
                    Command.Visible = true;
                }
            }
        }
    }
}