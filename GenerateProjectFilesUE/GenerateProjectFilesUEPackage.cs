global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;

namespace GenerateProjectFilesUE
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.GenerateProjectFilesUEString)]
    [ProvideUIContextRule(PackageGuids.UIContextSupportedFilesString,
        name: "Supported Files",
        expression: "UnrealProject",
        termNames: new[] { "UnrealProject" },
        termValues: new[] { "HierSingleSelectionName:.uproject$"})]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "UnreaProjectRegenerator", "General", 0, 0, true)]
    public sealed class GenerateProjectFilesUEPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }
    }
     
}