# Cities Skylines Mod Helper classes

## Overview 

The Cities Skylines Mod Helper classes are a set of classes that should make writing plugins easier by providing boiler plate code for providing plugin details, finding the installed path of your library, steam Id and to load/save settings.

## Set up the project

- Create a new C# library project
- Make sure you have Target Framework 3.5 set
- Add references to:
  - System
  - System.Configuration
- Add references to the game its managed libraries:
  - ICities.dll
  - Assembly-CSharp.dll
  - ColossalManaged.dll
  - UnityEngine.dll
  - UnityEngine.UI.dll
- Make sure the 5 references are set to `Copy Local` is `false`.
- Drag the 3 classes from this lib into the new project
  - ModBase.cs
  - ModSettings.cs
  - Log.cs
- Modify **AssemblyInfo.cs**:
```c#
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyTitle("mod name")]
[assembly: AssemblyDescription("mod description")]
[assembly: AssemblyCompany("author name")]
[assembly: AssemblyProduct("mod name")]
[assembly: AssemblyCopyright("© 2015 author name")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
```
- Optionally, if you want user configurable settings, add an **App.config** to the project:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<appSettings>
	</appSettings>
</configuration>
```
- And last, set up a post-build event (for both Debug/Release) to refresh the mod each time you compile:
```bat
rd "%appdata%\..\Local\Colossal Order\Cities_Skylines\Addons\Mods\$(TargetName)" /s/q
md "%appdata%\..\Local\Colossal Order\Cities_Skylines\Addons\Mods\$(TargetName)"
xcopy "$(TargetDir)*.*" "%appdata%\..\Local\Colossal Order\Cities_Skylines\Addons\Mods\$(TargetName)" /Y /E
```

You are now done setting up the project and can start coding your plugin.

## Example implementation

Please check out my implementation of the Autosave mod for details on how to use these 3 classes.

## Steam Workshop

Once you have developed your first working plugin, you will want to publish to the Steam Workshop via the ingame Content Manager. After publishing to the Steam Workshop, you should subscribe to your own mod. You receive your mod back but now in a new location:

`<Steam Root>\SteamApps\workshop\content\255710\<steam_workshop_id>\` 

The number `255710` is the game id. In that folder each mod will have its own folder id. Find your own mod and copy the updated files in here. You can now publish an update via the Content Manager.

*Note: I choose not to copy using post-build event to this folder automatically.*

## Api reference

### ModBase.cs

This ModBase class is an abstract class that implements IUserMod for you. If you followed all the steps above correctly, you can simply create your plugin class now and inherit from ModBase. You do not have to implement IUserMod, this class will use the provided info in the Assembly attributes to report the mod information to the game.

*Use of this base class is only working for compiled class library projects. It is also absolutely important you version your library using [`AssemblyVersionAttribute`](https://msdn.microsoft.com/en-us/library/system.reflection.assemblyversionattribute), otherwise the plugin resolving code won't work properly and might return the wrong mod location. Ie.:* `[assembly: AssemblyVersion("1.0.*")]`

##### Name
Type: `System.String`

Gets the mod name.

##### Description
Type: `System.String`

Gets the in-game mod description (includes author name).

##### ModDescription
Type: `System.String`

Gets the mod description (without author name).

##### Author
Type: `System.String`

Gets the mod author.

##### Version
Type: `System.Version`

Gets the mod version.

##### ModPath
Type: `System.String`

Gets the mod path. The path is the location where your library is located. This can either be under %AppData% (when compiling and running locally) or under the 'SteamApps' folder (if installed via the Steam workshop).

##### ModDllPath
Type: `System.String`

Gets the mod library path (DLL). This is the ModPath + the filename of your DLL.

##### SteamWorkshopId
Type: `ColossalFramework.Steamworks.PublishedFileId`

Gets the steam workshop id of the mod/plugin. In local installations, returns `ColossalFramework.Steamworks.PublishedFileId.invalid`.

### ModSettings.cs
This class is a small wrapper that loads the configuration file and provides access to the AppSettings section. The class can be used as is, but is better used as a base class for a strong typed settings class.

#### .ctor(dllPath)
Initializes ModSettings and loads the configuration file that belongs to the specified dll.

##### dllPath
Type: `System.String`

The path to the dll (the configuration file to be loaded is <dllPath>.config).

#### this[key]
Return type: `System.String`

Returns the value for the specified settings key.

##### key
Type: `System.String`

The key for the setting to retrieve.

#### Contains(key)
Return type: `System.Boolean`

Returns true if the specified key exists.

##### key
Type: `System.String`

The key for the setting to retrieve.

### Log.cs
Logs info to the ingame console. Note I use [ConditionalAttribute](https://msdn.microsoft.com/en-us/library/system.diagnostics.conditionalattribute) to disable the logging functionality in Release. The `DebugOutputPanel` can overflow and cause the game to crash at the moment. In future I will implement file based logging for Release mode and hopefully this ingame logging will be fixed at some point by the devs.
 
#### Info(text, args)
Logs an informational message.

##### text
Type: `System.String`

The error message to log.

##### args
Type: `System.Object[]`

Optional arguments to use with [String.Format()](https://msdn.microsoft.com/en-us/library/system.string.format).

#### Warning(text, args)
Logs a warning message.

##### text
Type: `System.String`

The error message to log.

##### args
Type: `System.Object[]`

Optional arguments to use with [String.Format()](https://msdn.microsoft.com/en-us/library/system.string.format).

#### Error(text, args)
Logs an error message.

##### text
Type: `System.String`

The error message to log.

##### args
Type: `System.Object[]`

Optional arguments to use with [String.Format()](https://msdn.microsoft.com/en-us/library/system.string.format).
