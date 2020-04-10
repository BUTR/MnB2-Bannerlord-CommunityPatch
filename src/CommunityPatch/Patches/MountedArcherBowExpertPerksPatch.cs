using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.Patches.AgentWeaponEquippedPatch;

namespace CommunityPatch.Patches {

  public class MountedArcherBowExpertPerksPatch {

    private static PerkObject BowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "sHXLjzCb");
    private static PerkObject MountedArcher => PerkObject.FindFirst(perk => perk.Name.GetID() == "cKTeea27");

    public static WeaponStatsData Apply(Agent agent, WeaponStatsData weapon) {
      if (weapon.ItemUsageIndex == MBItem.GetItemUsageIndex("long_bow")
        && (HeroHasPerk(agent.Character, MountedArcher) || HeroHasPerk(agent.Character, BowExpert))) {
        var updatedWeapon = weapon;
        updatedWeapon.ItemUsageIndex = MBItem.GetItemUsageIndex("bow");
        return updatedWeapon;
      }

      return weapon;
    }
  }
}