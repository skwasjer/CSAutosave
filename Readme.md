# Cities Skylines Autosave - per city (configurable interval) by skwas

## Overview 

- Saved games are named: `[autosave] <cityname>`. 
- Only one savegame per city 
- Be carefull, if you have several maps that share the same city name, they will all use the same autosave file. Make sure each city has a unique name. 
- To change the save interval (15 minutes default), go to the plugin folder, and find the file Autosave.dll.config. Edit the file with a text editor and change the interval at will. 
- Note that settings do not persist accross updates (if any will come). This is by design (for now), to prevent possible future file conflicts. I may change this design later. 

## Location of settings file

`...\SteamApps\workshop\content\255710\412091857\Autosave.dll.config` 

## Warning
- Do not use together with other Autosave mods 
- Use at own risk 
- Use of mods disables achievements 
- The only visual clue you have a save is running is the default progress indicator in the top right of the screen. If you experience a lag spike during playing, check that indicator, it may be that a save is in progress. Other then that, the mod should create no lag, because I don't do any fancy file management or other tricks under the hood. It's as simple as it gets. 

## A final note
I did not intend to make yet another 'autosave' mod. The mod was more a practice for myself to learn the API and to design a base framework for future mods, and also in an attempt to showcase good plugin design. Code will soon be shared via github, because the base class design and storage of settings is quite nice in my opinion, and can prove very usefull for other modders.

## To other modders
I wrote 3 helper classes and instructions in the other readme.md for everyone to reuse in mods of their own. If you do use these classes, please provide appropriate credits and a link to this github repo.