using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class WarRationsPatch : PatchBase<WarRationsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultMobilePartyFoodConsumptionModel).GetMethod("CalculateDailyFoodConsumptionf", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(WarRationsPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "sLv7MMJf");

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
        0x8B, 0x51, 0x81, 0x08, 0x94, 0x99, 0xA9, 0x74,
        0xF4, 0xAA, 0x1D, 0x76, 0x6A, 0x98, 0xE1, 0xA4,
        0x53, 0x65, 0x83, 0xF1, 0x7E, 0xC7, 0x5D, 0xD1,
        0xF2, 0x18, 0x81, 0xD0, 0x7E, 0xB2, 0x07, 0x5D
      });
    }

    // ReSharper disable once InconsistentNaming
    private static void Postfix(ref float __result, MobileParty party, StatExplainer explainer) {
      var qm = party.LeaderHero.Clan.GetEffectiveQuartermaster();

      if (qm == null)
        return;

      if (
        // qm is not clan-wide
        !CommunityPatchSubModule.Options.Get<bool>("WarRationsQmClanWide")
        // qm is not leading party
        && party.LeaderHero != qm
        // qm is not in party
        && party.MemberRoster.All(element => element.Character?.HeroObject != qm)
      )
        return;

      var perk = ActivePatch._perk;
      if (!qm.GetPerkValue(perk))
        return;

      var explainedNumber = new ExplainedNumber(__result, explainer);
      explainer?.Lines.RemoveAt(explainer.Lines.Count - 1);
      switch (perk.IncrementType) {
        case SkillEffect.EffectIncrementType.Add:
          explainedNumber.Add(perk.PrimaryBonus, perk.Name);
          break;

        case SkillEffect.EffectIncrementType.AddFactor:
          explainedNumber.AddFactor(perk.PrimaryBonus, perk.Name);
          break;
      }

      __result = explainedNumber.ResultNumber;
    }

  }

}