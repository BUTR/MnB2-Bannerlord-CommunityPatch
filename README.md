![Mount & Blade II: Bannerlord Community Patch](https://staticdelivery.nexusmods.com/mods/3174/images/headers/186_1586119060.png)

# Mount & Blade II: Bannerlord Community Patch module
This is going to be a mod that just fixes up some things in Mount &amp; Blade 2: Bannerlord before the Devs &amp; QA team can get to them. They have priorities and a process.

### Supported by JetBrains Open Source Development Program

[<img align="right" loading="eager" decoding="async" referrerpolicy="no-referrer" width="200" alt="JetBrains" src="https://github.com/Tyler-IN/MnB2-Bannerlord-CommunityPatch/raw/dev/tools/jetbrains.svg?sanitize=true" />][1]

We love [JetBrains][1]!

We receive free licenses to JetBrains products via their [open source development program][2].
JetBrains Toolbox manages all of their products and keeps them up to date, and updating is seamless.

### JetBrains Rider for C# / .NET

[<img align="right" loading="lazy" decoding="async" referrerpolicy="no-referrer" width="100" alt="JetBrains Rider" title="JetBrains Rider" src="https://github.com/Tyler-IN/MnB2-Bannerlord-CommunityPatch/raw/dev/tools/rider.svg?sanitize=true" />][3]

Our C# projects are designed to work best in JetBrains Rider.
We also use ReSharper with Visual Studio, dotPeek, dotCover, dotMemory and dotTrace.

#### JetBrains CLion for C / C++

[<img align="right" loading="lazy" decoding="async" referrerpolicy="no-referrer" width="100" alt="JetBrains CLion" title="JetBrains CLion" src="https://github.com/Tyler-IN/MnB2-Bannerlord-CommunityPatch/raw/dev/tools/clion.svg?sanitize=true" />][4]

Our C++ projects are designed to work best in JetBrains CLion.
We also use ReSharper C++ with Visual Studio.

### Current Fixes

* Perks
  * Athletics
    * Extra Arrows
    * Extra Throwing Weapon
    * Peak Form
  * Leadership
    * Disciplinarian <sub><sup>Fixed in e1.0.6</sup></sub>
    * Ultimate Leader
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
    * Trampler
    * Nomadic Traditions
    * Horse Grooming
  * Bow
    * Mounted Archer
    * Large Quiver
    * Battle Equipped
  * Crossbow
    * Crossbow Cavalry
  * Throwing
    * Fully Armed
    * Concealed Carry
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
    * Improved Masonry
    * Good Materials
    * Everyday Engineer
    * Builder
    * Scavenger
    * Armorcraft
    * Wall Breaker
    * Imperial Fire
    * Resolute
  * Roguery
    * Party Raiding
    * Eye for Loot
    * For the Thrill
    * Slip into Shadows
    * Briber
    * Negotiator
    * Bribe Master
    * Escape Artist
    * Slave Trader
    * Merry men
    * Concealed Blade
  * Two Handed
    * Quick Plunder
    * Eviscerate
  * Tactics
    * Companion Cavalry
    * Tactical Superiority
    * One Step Ahead
    * Elusive
    * Bait
    * Logistics
    * Ambush Specialist
    * Phalanx
    * Hammer and Anvil
    * Trusted Commander
* Policies
  * Land Grants For Veterans
* Feats
  * Aserai Cheap Caravans <sub><sup>Fixed in e1.3.0</sup></sub>
  * Battanian Forest Agility
  * Khuzait Cavalry Agility
  * Sturgian Snow Agility
* Learning Rate explanation
* Item Comparison perk-based coloring
* Party Morale bonus being too low for >10 food variety
* Fixed crash that occurs when the Neutral clan gains a clan tier 
* Warn user that early story quests will timeout
* Fix detection of snowy terrain and apply snow movement debuff as a factor of snow density

### Current Features
* Enable and Disable the Intro Video
* Enable and Disable Menu when encountering a neutral or enemy army.
* Generate Diagnostic Report
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

This repo should be placed in the folder `Modules\CommunityPatch` under your Bannerlord installation, and it will build out-of-the-box. Just open the \*.sln in an applicable IDE (JetBrains Rider, Visual Studio, etc) and build normally.

The default location for Bannerlord is `C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord`.

If your Bannerlord or local repo are in a different place, you can change the property MnB2BannerlordDir in Directory.Build.props to your bannerlord install. When editing the file be sure to use `&amp;` in place of an `&` if the path has one. If you do move the repo, then installing the mod will require copying it to `Modules\CommunityPatch`. Take care that the folder name is correct else problems may occur.

## Credits
##### Contributors
* [🅠](https://www.nexusmods.com/users/958353) ([Discord](https://discordapp.com/users/475636674076868618))
* [Skauzor](https://www.nexusmods.com/users/3289432) ([Discord](https://discordapp.com/users/123778041934643203))
* [Zarganoth](https://www.nexusmods.com/users/6940484) ([Discord](https://discordapp.com/users/298985985843396618))
* [Tynakuh](https://www.nexusmods.com/users/51824126) ([Discord](https://discordapp.com/users/178209384852094976))
* [wonkotron](https://www.nexusmods.com/users/87193583) ([Discord](https://discordapp.com/users/171467525660344320))
* [iPherian](https://www.nexusmods.com/users/86335488)
* [miguelcjalmeida](https://github.com/miguelcjalmeida)
* [Eagle](https://github.com/JoeFwd) ([Discord](https://discordapp.com/users/242802595347955715))
* [fnzr](https://github.com/fnzr)

##### Others
* Xaphedo for providing the banner art

[1]: https://www.jetbrains.com/?from=Mount%20%26%20Blade%20II%3A%20Bannerlord%20Community%20Patch
[2]: https://www.jetbrains.com/community/opensource/?from=Mount%20%26%20Blade%20II%3A%20Bannerlord%20Community%20Patch
[3]: https://www.jetbrains.com/rider/?from=Mount%20%26%20Blade%20II%3A%20Bannerlord%20Community%20Patch
[4]: https://www.jetbrains.com/clion/?from=Mount%20%26%20Blade%20II%3A%20Bannerlord%20Community%20Patch
