using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.CommunityPatchSubModule;

namespace CommunityPatch.Patches {

  public class CrossbowCavalryCrossbowExpertPerksPatch : AgentWeaponEquippedPatch {

    private static PerkObject CrossbowCavalry => PerkObject.FindFirst(perk => perk.Name.GetID() == "sHXLjzCb");

    private static PerkObject CrossbowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "T4fREm7U");

    protected override bool _IsApplicable(Game game) {
      // TODO: game version check
      var supported = new ApplicationVersion(ApplicationVersionType.EarlyAccess, 1, 0, 10);
      return VersionComparer.Compare(GameVersion, supported) <= 1;
    }

    // TODO: this apply should probably be split into one for each of these perks 
    protected override void Apply(Agent agent, ref WeaponStatsData weapon) {
      if (weapon.WeaponClass != (int) WeaponClass.Crossbow
        || !HeroHasPerk(agent.Character, CrossbowCavalry)
        && !HeroHasPerk(agent.Character, CrossbowExpert))
        return;

      weapon.WeaponFlags &= ~((ulong) WeaponFlags.CantReloadOnHorseback);
    }

  }

}