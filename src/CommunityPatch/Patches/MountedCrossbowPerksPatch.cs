using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches {

  internal class MountedCrossbowPerksPatch : IPatch {

    public bool Applied { get; private set; }

    public bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      => game.GameType as Campaign != null
          && (game.PlayerTroop as CharacterObject)?.HeroObject != null;

    public void Apply(Game game) {

      Hero hero = (game.PlayerTroop as CharacterObject)?.HeroObject;

      if (hero.GetPerkValue(DefaultPerks.Crossbow.CrossbowCavalry)) {
        MountedCrossbowPerksPatch.EnableReloadingOnHorse();
      }
      else if (hero.GetPerkValue(DefaultPerks.Riding.CrossbowExpert)) {
        MountedCrossbowPerksPatch.MakeUsableOnHorse();
      }

      (game.GameType as Campaign).CampaignBehaviorManager?.AddBehavior(new OnMountedCrossbowPerkAddedBehaviour());
      
      Applied = true;
    }

    public static void EnableReloadingOnHorse() {
      foreach (ItemObject itemObj in Campaign.Current.Items.Where(item => item.ItemType == ItemObject.ItemTypeEnum.Crossbow)) {
        itemObj.PrimaryWeapon.WeaponFlags &= ~WeaponFlags.CantReloadOnHorseback;
      }
    }

    public static void MakeUsableOnHorse() {      
      foreach (ItemObject itemObj in Campaign.Current.Items.Where(item => item.ItemType == ItemObject.ItemTypeEnum.Crossbow)) {
        typeof(WeaponComponentData).GetProperty("ItemUsage").SetValue(itemObj.PrimaryWeapon, "crossbow");
      }
    }
  }

  internal class OnMountedCrossbowPerkAddedBehaviour : CampaignBehaviorBase {

    public override void RegisterEvents() {
      CampaignEvents.PerkOpenedEvent.AddNonSerializedListener(this, (hero, perk) => {
        if (hero.Equals(Hero.MainHero)) {
          if (perk.Equals(DefaultPerks.Crossbow.CrossbowCavalry)) {
            MountedCrossbowPerksPatch.EnableReloadingOnHorse();
          } else if (perk.Equals(DefaultPerks.Riding.CrossbowExpert)) {
            MountedCrossbowPerksPatch.MakeUsableOnHorse();
          }
        }
      });
    }

    public override void SyncData(IDataStore dataStore) {
    }
  }

}