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
        // e.1.0.7
        0x92, 0xC7, 0xF7, 0xE5, 0x5E, 0xC8, 0xEB, 0xE2,
        0x6A, 0x25, 0xAD, 0x3F, 0x95, 0x79, 0xD6, 0xA0,
        0x66, 0x8B, 0x9A, 0x50, 0x37, 0xDD, 0x65, 0xAC,
        0x01, 0x3E, 0xEF, 0x02, 0x78, 0x11, 0x80, 0xD0
      },
      new byte[] {
        // e.1.0.8
        0xF5, 0x76, 0xD7, 0x2B, 0x9A, 0x8E, 0x79, 0xB8,
        0x3A, 0xAE, 0xD5, 0xCF, 0xCE, 0xD4, 0x28, 0x14,
        0xA5, 0x1B, 0xB0, 0x68, 0x47, 0xD7, 0xF0, 0xA5,
        0x4E, 0xFD, 0x48, 0x33, 0x32, 0xDF, 0x2F, 0x5E
      },
      new byte[] {
        // e.1.0.9
        0xD2, 0x55, 0x27, 0xE0, 0x42, 0x38, 0xA6, 0x32,
        0x75, 0x41, 0x34, 0xC7, 0x60, 0x3C, 0x24, 0xB6,
        0x28, 0x42, 0xC1, 0x03, 0x09, 0xA5, 0x42, 0x71,
        0x72, 0x60, 0xAD, 0x16, 0xD2, 0x19, 0xF6, 0xB7
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