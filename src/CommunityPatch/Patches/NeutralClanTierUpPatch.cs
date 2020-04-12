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
        0x70, 0x15, 0x09, 0xF4, 0x51, 0x1F, 0xE4, 0x86,
        0x1D, 0x10, 0x80, 0xD2, 0xC7, 0xA7, 0xA6, 0xD8,
        0x6C, 0xAA, 0x54, 0x5F, 0x5B, 0xFE, 0x49, 0x88,
        0xA8, 0xD6, 0x3B, 0xB2, 0x6B, 0x83, 0x05, 0x38
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