using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class SupremeAuthorityPatch : PerkPatchBase<SupremeAuthorityPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultClanPoliticsModel).GetMethod("CalculateInfluenceChangeInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(SupremeAuthorityPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0xDB, 0x69, 0x3E, 0x84, 0xBE, 0x6B, 0x4C, 0xA6,
        0x13, 0x32, 0xFA, 0xA4, 0x06, 0xF5, 0xB5, 0xA3,
        0xF1, 0x2E, 0x47, 0xB5, 0xE8, 0x8F, 0x19, 0x84,
        0xCD, 0x21, 0xF8, 0x42, 0x24, 0xD7, 0x30, 0x31
      },
      new byte[] {
        // e1.0.5
        0x0D, 0x0F, 0x98, 0xD5, 0xA8, 0xD7, 0x33, 0x9F,
        0xCB, 0xCC, 0x40, 0xFE, 0x6A, 0x83, 0xBF, 0xA8,
        0xED, 0xB5, 0x15, 0xB2, 0xDA, 0xC0, 0xC9, 0xD2,
        0x61, 0xA3, 0x53, 0x81, 0xF7, 0xC2, 0x83, 0xA1
      },
      new byte[] {
        // e1.0.4
        0x9D, 0x7D, 0x06, 0x79, 0x02, 0x3A, 0xDE, 0x53,
        0x88, 0xA4, 0x61, 0xCD, 0x09, 0x6C, 0xB5, 0x0E,
        0x02, 0x69, 0x74, 0xEE, 0x1B, 0xCD, 0xB8, 0xDB,
        0x61, 0x9F, 0xC2, 0x45, 0xF0, 0x4D, 0x06, 0x4F
      },
      new byte[] {
        // e1.4.0.228531
        0xEF, 0xAA, 0xB2, 0x3B, 0xEB, 0xE1, 0x8C, 0xB5,
        0x70, 0x16, 0xDE, 0xE3, 0x1D, 0xA5, 0xF2, 0xD6,
        0x0C, 0x1B, 0x01, 0x31, 0xB6, 0x42, 0x5C, 0x7C,
        0x1C, 0x15, 0x55, 0x9C, 0x57, 0xEF, 0x99, 0x5B
      }
    };

    public SupremeAuthorityPatch() : base("SFjspNSf") {
    }

    public override bool? IsApplicable(Game game) {
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

    private static void Postfix(Clan clan, ref ExplainedNumber influenceChange) {
      var perk = ActivePatch.Perk;

      var ruler = clan?.Kingdom?.Ruler;
      var leader = clan?.Leader;
      if (ruler != null && ruler == leader && ruler.GetPerkValue(perk))
        influenceChange.Add(perk.PrimaryBonus, perk.Name);
    }

  }

}