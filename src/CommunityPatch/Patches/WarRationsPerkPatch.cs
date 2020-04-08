using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  class WarRationsPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultMobilePartyFoodConsumptionModel).GetMethod("CalculateDailyFoodConsumptionf", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(WarRationsPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo),
        null);
      Applied = true;
    }

    public bool IsApplicable(Game game) {
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

    private static bool Prefix(ref float __result, TextObject ____partyConsumption, MobileParty party, StatExplainer explainer = null) {
      int num1 = party.Party.NumberOfAllMembers + party.Party.NumberOfPrisoners / 2;
      float num2 = (float) (-(num1 < 1 ? 1.0 : (double) num1) / 20.0);
      
      var quartermaster= ClanHelpers.GetEffectiveQuartermaster(party.LeaderHero.Clan);
      if (quartermaster != null && party.MemberRoster.Any(element => element.Character?.HeroObject == quartermaster))
        num2 *= 0.8f;

      ExplainedNumber explainedNumber = new ExplainedNumber(0.0f, explainer, (TextObject) null);
      explainedNumber.Add(num2, ____partyConsumption);
      __result = explainedNumber.ResultNumber;

      return false;
    }

  }

}