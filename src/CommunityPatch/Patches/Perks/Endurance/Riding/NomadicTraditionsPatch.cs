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

  public sealed class NomadicTraditionsPatch : PatchBase<NomadicTraditionsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("CalculatePureSpeed", BindingFlags.Public | BindingFlags.Instance);

    private static readonly MethodInfo PatchMethodInfo = typeof(NomadicTraditionsPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.2.0.226040
        0x61, 0xD7, 0x4D, 0xF5, 0xB0, 0x0E, 0x84, 0x52,
        0xDC, 0xCB, 0x2F, 0xE7, 0xE2, 0x20, 0x38, 0x10,
        0x87, 0x01, 0xE3, 0x61, 0xF1, 0xAB, 0x89, 0x7D,
        0x9C, 0xDC, 0x50, 0x6E, 0xA6, 0x7E, 0xEB, 0xEF
      }
    };

    public override void Reset() {}

    public override bool? IsApplicable(Game game)
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(DefaultPartySpeedCalculatingModel __instance, ref StatExplainer explanation, ref float __result, MobileParty mobileParty, ref int additionalTroopOnFootCount, ref int additionalTroopOnHorseCount) {
      var hero = mobileParty.LeaderHero;
      if (hero != null && hero.GetPerkValue(DefaultPerks.Riding.NomadicTraditions)) {
        //have to recalculate cavalry and mounted footman bonuses, so repeat the code
        var party = mobileParty.Party;
        var availableHorses = mobileParty.ItemRoster.NumberOfMounts;
        var totalTroops = mobileParty.MemberRoster.TotalManCount + additionalTroopOnFootCount + additionalTroopOnHorseCount;
        var mountedTroops = party.NumberOfMenWithHorse + additionalTroopOnHorseCount;
        var troopsOnFoot = party.NumberOfMenWithoutHorse + additionalTroopOnFootCount;
        if (mobileParty.AttachedParties.Count != 0) {
          foreach (var mobileParty2 in mobileParty.AttachedParties) {
            totalTroops += mobileParty2.MemberRoster.TotalManCount;
            mountedTroops += mobileParty2.Party.NumberOfMenWithHorse;
            troopsOnFoot += mobileParty2.Party.NumberOfMenWithoutHorse;
            availableHorses += mobileParty2.ItemRoster.NumberOfMounts;
          }
        }

        int mountedFootman = Math.Min(availableHorses, troopsOnFoot);
        var mountedFootmenRatioMethod = typeof(DefaultPartySpeedCalculatingModel).GetMethod("GetMountedFootmenRatioModifier", BindingFlags.Instance | BindingFlags.NonPublic);
        var defaultMountedFootmanRatio = (float)mountedFootmenRatioMethod.Invoke(__instance, new object[] {totalTroops, mountedFootman});
        //default Mounted Footman constant is 0.3, so just apply it again
        var perkMountedFootmanRatio = defaultMountedFootmanRatio;
        
        var cavalryRatioMethod = typeof(DefaultPartySpeedCalculatingModel).GetMethod("GetCavalryRatioModifier", BindingFlags.Instance | BindingFlags.NonPublic);
        var defaultCavalryRatio = (float)cavalryRatioMethod.Invoke(__instance, new object[] {totalTroops, mountedTroops});
        //default Cavalry constant is 0.6, so do math
        var perkCavalryRatio = Convert.ToSingle(0.3 * defaultCavalryRatio / 0.6);
        
        var baseSpeedMethod = typeof(DefaultPartySpeedCalculatingModel).GetMethod("CalculateBaseSpeedForParty", BindingFlags.Instance | BindingFlags.NonPublic);
        var baseSpeed = (float)baseSpeedMethod.Invoke(__instance, new object[] { totalTroops });

        var perkBonus = (perkCavalryRatio + perkMountedFootmanRatio) * baseSpeed;
        __result += perkBonus;
        explanation?.AddLine("Nomadic Traditions", perkBonus, StatExplainer.OperationType.Add);
      }
    }
  }
}