## What it does? 
Adds a (Generate Project Files) command to .uproject file context

![image](https://i.imgur.com/HKRrge4.jpg)
## How does it work?
The extension makes command by given format

`"{UnrealVersionSelector Path}" /projectfiles "{.uproject Path}"`

(Which is by default used by ue's shell shortcut) and executes it via shell
## How does it know about UnrealVersionSelector?
Extension tries to grab that information looking at registry

`HKEY_CLASSES_ROOT\\Unreal.ProjectFile\\shell\\rungenproj\\command`

If path remains invalid, then the user will be prompted to set it manually via 

`Tools -> Options -> GenerateProjectFilesUE`
 
### [Feel free to open issues (if any) on github](https://github.com/AgitoReiKen/vsix-ue-projectgen)
