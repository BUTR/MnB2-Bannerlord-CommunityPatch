using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public sealed class NeutralClanTierUpPatch : PatchBase<NeutralClanTierUpPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(HeroSpawnCampaignBehavior).GetMethod("OnClanTierIncreased", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(NeutralClanTierUpPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x6C, 0x46, 0xC7, 0xAA, 0x16, 0x02, 0x77, 0xF2,
        0x90, 0x37, 0xE4, 0x9B, 0x8D, 0x25, 0x46, 0x96,
        0x6C, 0xA3, 0x9D, 0xAB, 0x77, 0x3D, 0x6E, 0x28,
        0x8F, 0x5B, 0x4F, 0xB1, 0xA9, 0xFF, 0xB1, 0xAA
      }
    };

    public override void Reset() {
    }

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;
      if (CampaignData.NeutralFaction.DefaultPartyTemplate != null)
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        prefix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool Prefix(Clan clan) {
      if (clan == CampaignData.NeutralFaction)
        return false;

      return true;
    }

  }

}