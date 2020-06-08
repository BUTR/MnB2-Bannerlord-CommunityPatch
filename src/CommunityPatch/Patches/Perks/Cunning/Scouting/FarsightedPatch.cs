using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {
  public sealed class FarsightedPatch : PerkPatchBase<FarsightedPatch> {
    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.0.228478
        0x20, 0x9C, 0x71, 0x29, 0xC2, 0x4F, 0x22, 0xFB,
        0x9B, 0xAA, 0xBC, 0x55, 0x18, 0x8B, 0xA5, 0x94,
        0x36, 0x76, 0x01, 0x8A, 0x4D, 0xC5, 0x1F, 0x01,
        0x91, 0x77, 0x8D, 0xA7, 0xA3, 0x9B, 0x99, 0xC8
      }
    };

    public FarsightedPatch() : base("yqPNKKGb") { }

    [PatchClass(typeof(DefaultMapVisibilityModel))]
    private static void GetPartySpottingDifficultyPostfix(ref float __result) {
      if (MobileParty.MainParty.HasPerk(ActivePatch.Perk)) {
        __result -= __result * ActivePatch.Perk.PrimaryBonus;
      }
    }
  }
}
