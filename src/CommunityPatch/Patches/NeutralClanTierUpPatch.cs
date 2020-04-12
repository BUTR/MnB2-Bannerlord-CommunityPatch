using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class NeutralClanTierUpPatch : PatchBase<NeutralClanTierUpPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(HeroSpawnCampaignBehavior).GetMethod("OnClanTierIncreased", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(NeutralClanTierUpPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x25, 0xC5, 0x44, 0xCF, 0x5A, 0xEF, 0x10, 0x80,
        0xFB, 0x18, 0x33, 0x43, 0x2F, 0x85, 0x43, 0xCB,
        0x61, 0x90, 0xE7, 0xAE, 0xF9, 0x26, 0xF4, 0xF9,
        0xB9, 0x62, 0x9D, 0xD4, 0xB2, 0x4B, 0xA9, 0xC2
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