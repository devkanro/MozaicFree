# MozaicFree
Mozaic tools for unity games, based on [BepInEx](https://github.com/BepInEx/BepInEx)

## In-Game Shortcut
You can use those shortcut to create mozaic patch for special game

#### Alt+F11
Dump all texture from current scense, all png will be saved at `BepInEx/dump/<scense-name>`
#### Alt+F10
Dump names of all game objects from current scense, all png will be saved at `BepInEx/dump/<scense-name>/objects.txt`
#### Alt+F9
Disable all game objects which be listed in `BepInEx/objects/objects.txt`
#### Alt+F8
Enable all game objects which be disabled by `Alt+F9`

## Universal game object control
#### Auto disable game object
This plugin will auto disable all game objects which be listed in `BepInEx/objects/disabled.txt`
#### Auto enable game object
This plugin will auto enable all game objects which be listed in `BepInEx/objects/enabled.txt`
You can use the `?:` for conditional enable game objects like `/CH001/CH001?:/CH001/CH001/test`, it will enable `/CH001/CH001/test` when `/CH001/CH001` is enabled
