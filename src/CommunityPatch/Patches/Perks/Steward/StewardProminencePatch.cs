using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class StewardProminencePatch : PatchBase<StewardProminencePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultClanPoliticsModel).GetMethod("CalculateInfluenceChangeInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardProminencePatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x3E, 0x0F, 0x73, 0xB9, 0xE1, 0x47, 0x46, 0xAD,
        0xAF, 0xD7, 0xFA, 0xEA, 0xE6, 0x24, 0x4D, 0x3B,
        0x61, 0x51, 0x2A, 0x5B, 0xDF, 0x09, 0x7B, 0xAA,
        0x8D, 0x61, 0x7A, 0x00, 0x50, 0xCF, 0x16, 0x98
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "71EyPbaE");

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(Clan clan, ref ExplainedNumber influenceChange) {
      var perk = ActivePatch._perk;

      if (clan?.IsUnderMercenaryService ?? true) {
        return;
      }

      var ruler = clan.Kingdom?.Ruler;
      if (ruler != null && ruler != clan.Leader && clan.Leader.GetPerkValue(perk)) {
        influenceChange.Add(perk.PrimaryBonus, perk.Name);
      }
    }

  }

}