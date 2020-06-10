using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace CommunityPatch.Patches {
  public sealed class PartySpeedPatch : PatchBase<PartySpeedPatch> {
    private List<IPartySpeedSubPatch> _subPatches;

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x58, 0xF5, 0x64, 0xA2, 0xE1, 0x17, 0x5C, 0x0C,
        0x13, 0xEE, 0xED, 0xA8, 0x5E, 0xFC, 0xDF, 0x38,
        0x7E, 0xCE, 0x1E, 0xF9, 0xF8, 0x67, 0x78, 0x3F,
        0xB9, 0x71, 0xE0, 0x02, 0x8D, 0x58, 0x40, 0x99
      }
    };

    public override void Apply(Game game) {
      if (!Applied) {
        _subPatches = CommunityPatchSubModule.Patches
          .Where(p => p is IPartySpeedSubPatch && p.IsApplicable(game) == true)
          .Cast<IPartySpeedSubPatch>()
          .ToList();
        base.Apply(game);
      }
    }

    [PatchClass(typeof(DefaultPartySpeedCalculatingModel))]
    private static void CalculateFinalSpeedPostfix(ref float __result, MobileParty mobileParty, float baseSpeed, StatExplainer explanation) {
      if (mobileParty.LeaderHero is null) {
        return;
      }
      var finalSpeed = new ExplainedNumber(__result, explanation);
      foreach (var subPatch in ActivePatch._subPatches) {
        subPatch.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed);
      }
      __result = finalSpeed.ResultNumber;
    }
  }
}
