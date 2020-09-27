using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public class ForTheThrillPatch : PerkPatchBase<ForTheThrillPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(MapEvent).GetMethod("ApplyRaidResult", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPrefix = typeof(ForTheThrillPatch).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x67, 0x3C, 0x63, 0x7B, 0xD0, 0x08, 0x5F, 0x69,
        0x3F, 0x88, 0xE0, 0xC0, 0x35, 0xE9, 0x97, 0x67,
        0xBF, 0x14, 0xBB, 0xF6, 0xCA, 0x11, 0x72, 0x13,
        0x23, 0xE2, 0x25, 0x97, 0x10, 0x2C, 0xA5, 0xAA
      }
    };

    public ForTheThrillPatch() : base("WACam22Q") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(10f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfoPrefix));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    public static void Prefix(ref MapEvent __instance) {
      var attackers = __instance.AttackerSide.Parties.Where(x => x.MobileParty != null).ToArray();
      var moraleGain = CalculateMoralGain(attackers);

      foreach (var attacker in attackers)
        attacker.MobileParty.RecentEventsMorale += moraleGain;
    }

    private static float CalculateMoralGain(IEnumerable<PartyBase> attackers) {
      var moraleGain = new ExplainedNumber(4f);
      var perk = ActivePatch.Perk;

      foreach (var attacker in attackers)
#if AFTER_E1_5_1
        PerkHelper.AddPerkBonusForParty(perk, attacker.MobileParty, true, ref moraleGain);
#else
        PerkHelper.AddPerkBonusForParty(perk, attacker.MobileParty, ref moraleGain);
#endif

      return moraleGain.ResultNumber;
    }

  }

}