using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.Patches.AgentWeaponEquippedPatch;

namespace CommunityPatch.Patches {

  public class CrossbowCavalryCrossbowExpertPerksPatch {

    private static PerkObject CrossbowCavalry => PerkObject.FindFirst(perk => perk.Name.GetID() == "sHXLjzCb");
    private static PerkObject CrossbowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "T4fREm7U");

    public static WeaponStatsData Apply(Agent agent, WeaponStatsData weapon) {
      if (weapon.WeaponClass == (int) WeaponClass.Crossbow
        && (HeroHasPerk(agent.Character, CrossbowCavalry) || HeroHasPerk(agent.Character, CrossbowExpert))) {
        var updatedWeapon = weapon;
        updatedWeapon.WeaponFlags = weapon.WeaponFlags & ~((ulong) WeaponFlags.CantReloadOnHorseback);
        return updatedWeapon;
      }

      return weapon;
    }
  }
}