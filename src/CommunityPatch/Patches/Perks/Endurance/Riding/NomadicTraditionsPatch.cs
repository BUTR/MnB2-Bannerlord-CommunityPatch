using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public sealed partial class NomadicTraditionsPatch : PerkPatchBase<NomadicTraditionsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo PureSpeedMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("CalculatePureSpeed", Public | Instance);

    private static readonly MethodInfo CavalryRatioModifierMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("GetCavalryRatioModifier", NonPublic | Instance);

    private static readonly Func<DefaultPartySpeedCalculatingModel, int, int, float> GetCavalryRatioModifier = CavalryRatioModifierMethodInfo.BuildInvoker<Func<DefaultPartySpeedCalculatingModel, int, int, float>>();

    private static readonly MethodInfo MountedFootmenRatioModifierMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("GetMountedFootmenRatioModifier", NonPublic | Instance);

    private static readonly Func<DefaultPartySpeedCalculatingModel, int, int, float> GetMountedFootmenRatioModifier = MountedFootmenRatioModifierMethodInfo.BuildInvoker<Func<DefaultPartySpeedCalculatingModel, int, int, float>>();

    private static readonly MethodInfo BaseSpeedMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("CalculateBaseSpeedForParty", Instance | NonPublic);

    private static readonly Func<DefaultPartySpeedCalculatingModel, int, float> CalculateBaseSpeedForParty = BaseSpeedMethodInfo.BuildInvoker<Func<DefaultPartySpeedCalculatingModel, int, float>>();

    private static readonly MethodInfo PatchMethodInfo = typeof(NomadicTraditionsPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return PureSpeedMethodInfo;
      yield return CavalryRatioModifierMethodInfo;
      yield return MountedFootmenRatioModifierMethodInfo;
      yield return BaseSpeedMethodInfo;
    }

    public NomadicTraditionsPatch() : base("PB5iowxh") {
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(PureSpeedMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      if (!HashesMatch())
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(PureSpeedMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    private float CalculatePerkBaseRatio(float magicNumber)
      => magicNumber * (1 + Perk.PrimaryBonus) - magicNumber;

    private static void Postfix(DefaultPartySpeedCalculatingModel __instance, ref StatExplainer explanation, ref float __result, MobileParty mobileParty, ref int additionalTroopOnFootCount, ref int additionalTroopOnHorseCount) {
      var hero = mobileParty.LeaderHero;

      var nomadicTraditionsPatch = ActivePatch;
      var perk = nomadicTraditionsPatch.Perk;

      if (hero == null || !hero.GetPerkValue(perk))
        return;

      //have to recalculate cavalry and mounted footman bonuses, so repeat the code
      var party = mobileParty.Party;
      float availableHorses = mobileParty.ItemRoster.NumberOfMounts;
      var totalTroops = mobileParty.MemberRoster.TotalManCount + additionalTroopOnFootCount + additionalTroopOnHorseCount;
      float mountedTroops = party.NumberOfMenWithHorse + additionalTroopOnHorseCount;
      float troopsOnFoot = party.NumberOfMenWithoutHorse + additionalTroopOnFootCount;

      foreach (var attachedMobileParty in mobileParty.AttachedParties) {
        var attachedParty = attachedMobileParty.Party;
        mountedTroops += attachedParty.NumberOfMenWithHorse;
        troopsOnFoot += attachedParty.NumberOfMenWithoutHorse;
        totalTroops += attachedMobileParty.MemberRoster.TotalManCount;
        availableHorses += attachedMobileParty.ItemRoster.NumberOfMounts;
      }

      var mountedFootman = Math.Min(availableHorses, troopsOnFoot);
      var mountedFootmanRatio = totalTroops == 0 ? 0 : mountedFootman / totalTroops;
      var mountedFootmanMagicNumber = GetMountedFootmenRatioModifier(__instance, 1, 1);

      var perkMountedFootmanRatio = nomadicTraditionsPatch.CalculatePerkBaseRatio(mountedFootmanMagicNumber) * mountedFootmanRatio;

      var cavalryRatio = totalTroops == 0 ? 0 : mountedTroops / totalTroops;
      var cavalryMagicNumber = GetCavalryRatioModifier(__instance, 1, 1);
      var perkCavalryRatio = nomadicTraditionsPatch.CalculatePerkBaseRatio(cavalryMagicNumber) * cavalryRatio;

      var baseSpeed = CalculateBaseSpeedForParty(__instance, totalTroops);

      var explainedNumber = new ExplainedNumber(__result, explanation);
      var perkName = perk.Name;
      explainedNumber.Add(baseSpeed * perkCavalryRatio, perkName);
      explainedNumber.Add(baseSpeed * perkMountedFootmanRatio, perkName);
      __result = explainedNumber.ResultNumber;
    }

  }

}