using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Feats {

  public sealed class DisableTaleWorldsAgilityBonusesPatch : PatchBase<DisableTaleWorldsAgilityBonusesPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = AccessTools.Method(
      AccessTools.TypeByName("Helpers.PerkHelper"),
      "AddFeatBonusForPerson");

    private static readonly MethodInfo PatchMethodInfo = AccessTools.Method(typeof(DisableTaleWorldsAgilityBonusesPatch), nameof(Prefix));

    private static readonly byte[][] ValidHashes = {
      new byte[] {
        // e1.3.0.226834
        0x29, 0x53, 0x91, 0xD0, 0xA9, 0xC2, 0x81, 0xA5,
        0xF6, 0xD8, 0xFF, 0xEF, 0x1C, 0x68, 0xE5, 0x4B,
        0xEC, 0x96, 0x86, 0xDA, 0xD1, 0x41, 0x46, 0xBD,
        0x50, 0xED, 0x87, 0x21, 0x5B, 0x10, 0xFF, 0x81
      }
    };

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public override bool? IsApplicable(Game game) {
      if (Applied) return false;

      if (TargetMethodInfo == null) {
        CommunityPatchSubModule.Error("DisableTaleWorldsAgilityBonusesPatch:  Invalid target method");
        return false;
      }

      // This patch will probably not play nicely with others because it disables the target method in certain
      // circumstances
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (HarmonyHelpers.AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(ValidHashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        prefix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    public override void Reset()
      => Applied = false;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool Prefix(
      ref FeatObject feat,
      ref CharacterObject character,
      ref ExplainedNumber bonuses) {
      if (character == null ||
        !character.GetFeatValue(feat))
        return true;

      if (feat == DefaultFeats.Cultural.BattanianForestAgility ||
        feat == DefaultFeats.Cultural.KhuzaitCavalryAgility ||
        feat == DefaultFeats.Cultural.SturgianSnowAgility)
        return false;

      return true;
    }

  }

}