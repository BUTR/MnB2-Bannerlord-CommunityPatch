using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.CommunityPatchSubModule;
using static CommunityPatch.Patches.AgentWeaponEquippedPatch;

namespace CommunityPatch.Patches {

  public class MountedArcherBowExpertPerksPatch : AgentWeaponEquippedPatch {

    private static PerkObject BowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "sHXLjzCb");

    private static PerkObject MountedArcher => PerkObject.FindFirst(perk => perk.Name.GetID() == "cKTeea27");

    protected override bool _IsApplicable(Game game) {
      // TODO: game version check
      var supported = new ApplicationVersion(ApplicationVersionType.EarlyAccess, 1, 0, 10);
      return VersionComparer.Compare(GameVersion, supported) <= 1;
    }

    // TODO: this apply should probably be split into one for each of these perks 
    protected override void Apply(Agent agent, ref WeaponStatsData weapon) {
      if (weapon.ItemUsageIndex != MBItem.GetItemUsageIndex("long_bow")
        || (!HeroHasPerk(agent.Character, MountedArcher)
          && !HeroHasPerk(agent.Character, BowExpert)))
        return;

      weapon.ItemUsageIndex = MBItem.GetItemUsageIndex("bow");
    }

  }

}