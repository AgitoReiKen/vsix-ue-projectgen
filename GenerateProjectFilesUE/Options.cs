using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GenerateProjectFilesUE
{
    internal class DialogPageProvider
    {
        [ComVisible(true)]
        public class General : BaseOptionPage<GeneralOptions>
        {
        }
    }

    internal class GeneralOptions : BaseOptionModel<GeneralOptions>
    {
        [Category("General")]
        [DisplayName("Path to UnrealVersionSelector")]
        [Description(
            "By default it could be found in /Epic Games/Launcher/Engine/Binaries/Win64/UnrealVersionSelector.exe")]
        [DefaultValue("")]
        public string UnrealVersionSelectorPath { get; set; } = "";
    }
}