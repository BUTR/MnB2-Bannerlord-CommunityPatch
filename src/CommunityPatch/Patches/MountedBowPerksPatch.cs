using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches {

  internal class MountedBowPerksPatch : IPatch {

    public bool Applied { get; private set; }

    public static readonly List<PerkObject> _perks
      = new List<PerkObject> { DefaultPerks.Bow.MountedArcher, DefaultPerks.Riding.BowExpert };

    public bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      => game.GameType as Campaign != null
          && (game.PlayerTroop as CharacterObject)?.HeroObject != null;

    public void Apply(Game game) {

      Hero hero = (game.PlayerTroop as CharacterObject)?.HeroObject;

      if (_perks.Any(hero.GetPerkValue)) PatchWeapons();

      (game.GameType as Campaign).CampaignBehaviorManager?.AddBehavior(new OnMountedBowPerkAddedBehaviour());
      
      Applied = true;
    }

    public static void PatchWeapons() {      
      foreach (ItemObject itemObj in Campaign.Current.Items.Where(item => item.ItemType == ItemObject.ItemTypeEnum.Bow)) {
        typeof(WeaponComponentData).GetProperty("ItemUsage").SetValue(itemObj.PrimaryWeapon, "bow");
      }
    }
  }

  internal class OnMountedBowPerkAddedBehaviour : CampaignBehaviorBase {

    public override void RegisterEvents() {
      CampaignEvents.PerkOpenedEvent.AddNonSerializedListener(this, (hero, perk) => {
        if (hero.Equals(Hero.MainHero) && MountedBowPerksPatch._perks.Any(perk.Equals)) {
          MountedBowPerksPatch.PatchWeapons();
        }
      });
    }

    public override void SyncData(IDataStore dataStore) {
    }
  }

}