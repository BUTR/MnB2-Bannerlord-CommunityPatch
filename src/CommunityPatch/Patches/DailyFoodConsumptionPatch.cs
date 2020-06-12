using System.Collections.Generic;
using System.Linq;
using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace CommunityPatch.Patches {

  public class DailyFoodConsumptionPatch : PatchBase<DailyFoodConsumptionPatch> {

    private static List<IDailyFoodConsumption> _subPatches;
    
    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.0.228478
        0x03, 0xA6, 0xD1, 0x58, 0xEA, 0x4A, 0x70, 0xB5,
        0xFD, 0x39, 0xA9, 0x33, 0x66, 0xAB, 0x92, 0x37,
        0xD5, 0xDB, 0xE7, 0x8E, 0x30, 0xF3, 0x9C, 0x5A,
        0xFC, 0x20, 0xF8, 0x64, 0xC5, 0x19, 0x19, 0x1C
      },
      new byte[] {
        // e1.4.0.230377
        0xAA, 0x08, 0xEB, 0xB8, 0x40, 0x06, 0xA6, 0xA5,
        0xED, 0xC8, 0x35, 0xB4, 0x22, 0x75, 0xAB, 0x08,
        0xEB, 0xA7, 0x50, 0x52, 0x1B, 0x4E, 0x72, 0xD3,
        0xA0, 0xDF, 0x65, 0x4A, 0xE2, 0x1E, 0x4E, 0x60
      }
    };

    public override void Apply(Game game) {
      if (Applied) return;

      _subPatches = CommunityPatchSubModule.Patches
        .Where(p => p is IDailyFoodConsumption && p.IsApplicable(game) == true)
        .Cast<IDailyFoodConsumption>()
        .ToList();

      base.Apply(game);
    }

    [PatchClass(typeof(DefaultMobilePartyFoodConsumptionModel))]
    private static void CalculateDailyFoodConsumptionfPostfix(ref float __result, MobileParty party, StatExplainer explainer) {
      foreach (var subPatch in _subPatches) {
        subPatch.ModifyDailyFoodConsumption(ref __result, party, explainer);
      }
    }

  }

}