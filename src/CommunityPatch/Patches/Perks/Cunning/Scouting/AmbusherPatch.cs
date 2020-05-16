using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {
  public sealed class AmbusherPatch : PerkPatchBase<AmbusherPatch> {
    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.0.228478
        0x50, 0xC6, 0x2F, 0x01, 0x13, 0x7F, 0xF1, 0x47,
        0x2F, 0x93, 0xA8, 0x60, 0x67, 0x5C, 0x0B, 0x92,
        0x4E, 0xF5, 0xC9, 0x22, 0x80, 0xF1, 0x08, 0xF7,
        0x48, 0x47, 0x43, 0x6C, 0x1D, 0x06, 0xA2, 0x03
      }
    };

    public AmbusherPatch() : base("I8Jy7Zb9") { }

    [PatchClass(typeof(DefaultMapVisibilityModel))]
    private static void GetHideoutSpottingDistancePostfix(ref float __result) {
      if (MobileParty.MainParty.HasPerk(ActivePatch.Perk)) {
        __result += __result * ActivePatch.Perk.PrimaryBonus;
      }
    }
  }
}
