![Mount & Blade II: Bannerlord Community Patch](https://staticdelivery.nexusmods.com/mods/3174/images/headers/186_1586119060.png)

# Mount & Blade II: Bannerlord Community Patch module
This is going to be a mod that just fixes up some things in Mount &amp; Blade 2: Bannerlord before the Devs &amp; QA team can get to them. They have priorities and a process.

### Current Fixes

* Perks
  * Athletics
    * Extra Arrows
    * Extra Throwing Weapon
    * Peak Form
  * Leadership
    * Disciplinarian <sub><sup>Fixed in e1.0.6</sup></sub>
  * Scouting
    * Healthy Scout
  * Riding
    * Spare Arrows
    * Spare Throwing Weapon
    * Bow Expert
    * Conroi
    * Crossbow Expert
    * Filled To Brim
    * Squires
  * Bow
    * Mounted Archer
    * Large Quiver
    * Battle Equipped
  * Crossbow
    * Crossbow Cavalry
  * Throwing
    * Fully Armed
    * Battle Ready
  * Steward
    * Agrarian
    * Assessor
    * Bannerlord
    * Enhanced Mines
    * Food Rationing
    * Logistics Expert
    * Man-At-Arms
    * Nourish Settlement
    * Prominence
    * Prosperous Reign
    * Reconstruction
    * Reeve
    * Ruler
    * Supreme Authority
    * Swords As Tribute
    * Tax Collector
    * Warmonger
    * War Rations
  * Engineering
    * Ballistics
    * Construction Expert
* Policies
  * Land Grants For Veterans
* Learning Rate explanation
* Item Comparison perk-based coloring
* Party Morale bonus being too low for >10 food variety
* Fixed crash that occurs when the Neutral clan gains a clan tier 
* Warn user that early story quests will timeout

### Current Features
* Enable and Disable the Intro Video
* Copy Diagnostic Info to the Clipboard
* Toggle recording First Chance Exception info (don't use unless troubleshooting bugs)
* Automatically capture Diagnostic Info to the Clipboard when an Unhandled Exception crash occurs


### Modders
`<DelayedSubModule>` support is being separated out to a dedicated module, the [DelayedSubModules](https://github.com/Tyler-IN/MnB2-Bannerlord-DelayedSubModules) project.

## How To Contribute

If you'd like to collaborate on the project, please contact me via discord (̑[🅠#4344](https://discordapp.com/users/475636674076868618)) on the [TW Forums - Modding](https://discord.gg/5fBVT8j) discord or the [Mount and Blade](https://discordapp.com/invite/mountandblade) discord.

## Links

NexusMods: https://www.nexusmods.com/mountandblade2bannerlord/mods/186

Bannerlord Mod.IO: https://bannerlord.mod.io/community-patch

## Building

This repo should be placed under the Modules folder in your Bannerlord installation, and it will build out-of-the-box. Just open the \*.sln in Visual Studio and build normally.

The default location for Bannerlord is `C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord`.

If your Bannerlord or local repo are in a different place, you can change the property MnB2BannerlordBinDir in Directory.Build.props to the `bin/Win64_Shipping_Client` folder under your Bannerlord install. When editing the file be sure to use `&amp;` in place of an `&` if the path has one.

## Credits
##### Contributors
* [🅠](https://www.nexusmods.com/users/958353) ([Discord](https://discordapp.com/users/475636674076868618))
* [Skauzor](https://www.nexusmods.com/users/3289432) ([Discord](https://discordapp.com/users/123778041934643203))
* [Zarganoth](https://www.nexusmods.com/users/6940484) ([Discord](https://discordapp.com/users/298985985843396618))
* [Tynakuh](https://www.nexusmods.com/users/51824126) ([Discord](https://discordapp.com/users/178209384852094976))

##### Others
* Xaphedo for providing the banner art
