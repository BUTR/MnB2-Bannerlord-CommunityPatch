using System.Linq;
using System.Reflection;
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

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0x92, 0xC7, 0xF7, 0xE5, 0x5E, 0xC8, 0xEB, 0xE2,
        0x6A, 0x25, 0xAD, 0x3F, 0x95, 0x79, 0xD6, 0xA0,
        0x66, 0x8B, 0x9A, 0x50, 0x37, 0xDD, 0x65, 0xAC,
        0x01, 0x3E, 0xEF, 0x02, 0x78, 0x11, 0x80, 0xD0
      });
    }

    public override void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    private static void Postfix(Clan clan, ref ExplainedNumber influenceChange) {
      if (clan?.IsUnderMercenaryService ?? true) {
        return;
      }

      var ruler = clan.Kingdom?.Ruler;
      if (ruler != null && ruler != clan.Leader && clan.Leader.GetPerkValue(DefaultPerks.Steward.Prominence)) {
        influenceChange.Add(DefaultPerks.Steward.Prominence.PrimaryBonus, DefaultPerks.Steward.Prominence.Name);
      }
    }
  }

}