using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class BribeMasterPatch : PatchBase<BribeMasterPatch> {
    
    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultBribeCalculationModel).GetMethod(nameof(DefaultBribeCalculationModel.GetBribeToEnterLordsHall), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PatchMethodInfo = typeof(BribeMasterPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x7C, 0x2F, 0x71, 0x0A, 0x54, 0x2E, 0x4F, 0x9D,
        0x5E, 0xF6, 0x5B, 0x99, 0xC8, 0x04, 0x2C, 0xE5,
        0x68, 0x6F, 0x32, 0xAB, 0x37, 0x07, 0x40, 0x56,
        0x28, 0x32, 0xAE, 0xC6, 0xC6, 0x90, 0x48, 0x8C
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "wb1nbOiq");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref int __result) {
      var perk = ActivePatch._perk;
      if (!Hero.MainHero.GetPerkValue(perk)) return;
      __result = (int) (__result * perk.PrimaryBonus);
    }
  }
}