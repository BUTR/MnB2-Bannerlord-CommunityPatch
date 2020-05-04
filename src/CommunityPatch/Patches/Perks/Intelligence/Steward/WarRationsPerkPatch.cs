using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class WarRationsPatch : PerkPatchBase<WarRationsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultMobilePartyFoodConsumptionModel).GetMethod("CalculateDailyFoodConsumptionf", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(WarRationsPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    internal static bool QuartermasterIsClanWide {
      get => CommunityPatchSubModule.Options.Get<bool>(nameof(QuartermasterIsClanWide));
      set => CommunityPatchSubModule.Options.Set(nameof(QuartermasterIsClanWide), value);
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x03, 0xA6, 0xD1, 0x58, 0xEA, 0x4A, 0x70, 0xB5,
        0xFD, 0x39, 0xA9, 0x33, 0x66, 0xAB, 0x92, 0x37,
        0xD5, 0xDB, 0xE7, 0x8E, 0x30, 0xF3, 0x9C, 0x5A,
        0xFC, 0x20, 0xF8, 0x64, 0xC5, 0x19, 0x19, 0x1C
      }
    };

public WarRationsPatch() : base("sLv7MMJf") {}

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    // ReSharper disable once InconsistentNaming
    
    private static void Postfix(ref float __result, MobileParty party, StatExplainer explainer) {
      var qm = party?.LeaderHero?.Clan?.GetEffectiveQuartermaster();

      if (qm == null)
        return;

      if (
        // qm is not clan-wide
        !QuartermasterIsClanWide
        // qm is not leading party
        && party.LeaderHero != qm
        // qm is not in party
        && party.MemberRoster.All(element => element.Character?.HeroObject != qm)
      )
        return;

      var perk = ActivePatch.Perk;
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