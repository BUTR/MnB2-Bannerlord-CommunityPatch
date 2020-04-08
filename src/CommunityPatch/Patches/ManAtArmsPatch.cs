using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class ManAtArmsPatch : PatchBase<ManAtArmsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySizeLimitModel).GetMethod("CalculateMobilePartyMemberSizeLimit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ManAtArmsPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "WVLzi1fa");

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0x4B, 0x26, 0xD4, 0x1E, 0xF7, 0xCF, 0x5B, 0x15,
        0xE1, 0x24, 0x74, 0x8D, 0xE9, 0x46, 0x36, 0x80,
        0x6A, 0x91, 0x65, 0x5D, 0x7A, 0x6C, 0x3F, 0x43,
        0xD2, 0x7B, 0x80, 0xA7, 0x3E, 0xF0, 0x10, 0xF6
      });
    }

    // ReSharper disable once InconsistentNaming
    private static void Postfix(ref int __result, MobileParty party, StatExplainer explainer) {
      var perk = ActivePatch._perk;
      if (!party.LeaderHero.GetPerkValue(perk))
        return;

      var explainedNumber = new ExplainedNumber(__result, explainer);
      explainedNumber.Add(perk.PrimaryBonus, perk.Name);

      __result = (int) explainedNumber.ResultNumber;
    }

  }

}