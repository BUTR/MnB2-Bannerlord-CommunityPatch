using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public sealed partial class NomadicTraditionsPatch : PatchBase<NomadicTraditionsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo PureSpeedMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("CalculatePureSpeed", BindingFlags.Public | BindingFlags.Instance);
    
    private static readonly MethodInfo CavalryRatioModifierMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("GetCavalryRatioModifier", BindingFlags.NonPublic | BindingFlags.Instance);
    
    private static readonly MethodInfo MountedFootmenRatioModifierMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("GetMountedFootmenRatioModifier", BindingFlags.NonPublic | BindingFlags.Instance);
    
    private static readonly MethodInfo BaseSpeedMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("CalculateBaseSpeedForParty", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo PatchMethodInfo = typeof(NomadicTraditionsPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return PureSpeedMethodInfo;
      yield return CavalryRatioModifierMethodInfo;
      yield return MountedFootmenRatioModifierMethodInfo;
      yield return BaseSpeedMethodInfo;
    }

    public override void Reset() =>
      _perk = PerkObject.FindFirst(x => x.Name.GetID() == "PB5iowxh");

    public override bool? IsApplicable(Game game)
    {
      var patchInfo = Harmony.GetPatchInfo(PureSpeedMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      return HashesMatch();
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(PureSpeedMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    private static float GetPerkBaseRatio(float magicNumber) =>
      (magicNumber * (1 + ActivePatch._perk.PrimaryBonus)) - magicNumber;
    

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(DefaultPartySpeedCalculatingModel __instance, ref StatExplainer explanation, ref float __result, MobileParty mobileParty, ref int additionalTroopOnFootCount, ref int additionalTroopOnHorseCount) {
      var hero = mobileParty.LeaderHero;
      if (hero != null && hero.GetPerkValue(DefaultPerks.Riding.NomadicTraditions)) {
        //have to recalculate cavalry and mounted footman bonuses, so repeat the code
        var party = mobileParty.Party;
        float availableHorses = mobileParty.ItemRoster.NumberOfMounts;
        var totalTroops = mobileParty.MemberRoster.TotalManCount + additionalTroopOnFootCount + additionalTroopOnHorseCount;
        float mountedTroops = party.NumberOfMenWithHorse + additionalTroopOnHorseCount;
        float troopsOnFoot = party.NumberOfMenWithoutHorse + additionalTroopOnFootCount;
        if (mobileParty.AttachedParties.Count != 0) {
          foreach (var mobileParty2 in mobileParty.AttachedParties) {
            totalTroops += mobileParty2.MemberRoster.TotalManCount;
            mountedTroops += mobileParty2.Party.NumberOfMenWithHorse;
            troopsOnFoot += mobileParty2.Party.NumberOfMenWithoutHorse;
            availableHorses += mobileParty2.ItemRoster.NumberOfMounts;
          }
        }
        var mountedFootman = Math.Min(availableHorses, troopsOnFoot);
        var mountedFootmanRatio = totalTroops == 0 ? 0 : mountedFootman / totalTroops;
        var mountedFootmanMagicNumber = (float)MountedFootmenRatioModifierMethodInfo.Invoke(__instance, new object[] {1, 1});
        var perkMountedFootmanRatio = GetPerkBaseRatio(mountedFootmanMagicNumber) * mountedFootmanRatio;
        
        var cavalryRatio = totalTroops == 0 ? 0 : mountedTroops / totalTroops;
        var cavalryMagicNumber = (float)CavalryRatioModifierMethodInfo.Invoke(__instance, new object[] {1, 1});
        var perkCavalryRatio = GetPerkBaseRatio(cavalryMagicNumber) * cavalryRatio;

        var baseSpeed = (float)BaseSpeedMethodInfo.Invoke(__instance, new object[] {totalTroops});
        
        var explainedNumber = new ExplainedNumber(__result, explanation);
        explainedNumber.Add(baseSpeed * perkCavalryRatio, ActivePatch._perk.Name);
        explainedNumber.Add(baseSpeed * perkMountedFootmanRatio, ActivePatch._perk.Name);
        __result = explainedNumber.ResultNumber;
      }
    }
  }
}