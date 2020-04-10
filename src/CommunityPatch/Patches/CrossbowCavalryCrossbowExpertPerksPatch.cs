using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch.Patches {

  public class CrossbowCavalryCrossbowExpertPerksPatch : AgentWeaponEquippedPatch {

    private static PerkObject CrossbowCavalry => PerkObject.FindFirst(perk => perk.Name.GetID() == "sHXLjzCb");

    private static PerkObject CrossbowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "T4fREm7U");

    // TODO: this apply should probably be split into one for each of these perks 
    public override void Apply(Agent agent, ref WeaponStatsData weapon) {
      if (weapon.WeaponClass != (int) WeaponClass.Crossbow
        || !HeroHasPerk(agent.Character, CrossbowCavalry)
        && !HeroHasPerk(agent.Character, CrossbowExpert))
        return;

      var updatedWeapon = weapon;
      updatedWeapon.WeaponFlags = weapon.WeaponFlags & ~((ulong) WeaponFlags.CantReloadOnHorseback);
    }

  }

}