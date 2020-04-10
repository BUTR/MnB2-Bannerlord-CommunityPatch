using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.Patches.AgentWeaponEquippedPatch;

namespace CommunityPatch.Patches {

  public class MountedArcherBowExpertPerksPatch : AgentWeaponEquippedPatch {

    private static PerkObject BowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "sHXLjzCb");

    private static PerkObject MountedArcher => PerkObject.FindFirst(perk => perk.Name.GetID() == "cKTeea27");

    public override void Apply(Agent agent, ref WeaponStatsData weapon) {
      if (weapon.ItemUsageIndex != MBItem.GetItemUsageIndex("long_bow")
        || (!HeroHasPerk(agent.Character, MountedArcher)
          && !HeroHasPerk(agent.Character, BowExpert)))
        return;

      weapon.ItemUsageIndex = MBItem.GetItemUsageIndex("bow");
    }

  }

}