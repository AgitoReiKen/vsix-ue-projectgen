## What it does? 
- Adds a command to project's context <Generate Project Files> using UnrealVersionSelectorA context util 
![image](https://imgur.com/2Rm2dgB.jpg)
## How does it work?
The extension makes command by given format
`"{UnrealVersionSelector Path}" /projectfiles "{.uproject Path}"`
(Which is by default used by ue's shell shortcut) and executes it via shell
## How does it know about UnrealVersionSelector?
Extension tries to grab that information looking at registry
`HKEY_CLASSES_ROOT\\Unreal.ProjectFile\\shell\\rungenproj\\command`

If path remains invalid, then the user will be prompted to set it manually via `Tools -> Options -> GenerateProjectFilesUE`
