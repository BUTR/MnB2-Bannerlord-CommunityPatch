using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public class ForTheThrillPatch : PatchBase<ForTheThrillPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(MapEvent).GetMethod("ApplyRaidResult", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPrefix = typeof(ForTheThrillPatch).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x67, 0x3C, 0x63, 0x7B, 0xD0, 0x08, 0x5F, 0x69,
        0x3F, 0x88, 0xE0, 0xC0, 0x35, 0xE9, 0x97, 0x67,
        0xBF, 0x14, 0xBB, 0xF6, 0xCA, 0x11, 0x72, 0x13,
        0x23, 0xE2, 0x25, 0x97, 0x10, 0x2C, 0xA5, 0xAA
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "WACam22Q");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      _perk.SetPrimaryBonus(10f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfoPrefix));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Prefix(ref MapEvent __instance) {
      var attackers = __instance.AttackerSide.Parties.Where(x => x.MobileParty != null).ToArray();
      var moraleGain = CalculateMoralGain(attackers);

      foreach (var attacker in attackers)
        attacker.MobileParty.RecentEventsMorale += moraleGain;
    }

    private static float CalculateMoralGain(IEnumerable<PartyBase> attackers) {
      var moraleGain = new ExplainedNumber(4f);
      var perk = ActivePatch._perk;

      foreach (var attacker in attackers)
        PerkHelper.AddPerkBonusForParty(perk, attacker.MobileParty, ref moraleGain);

      return moraleGain.ResultNumber;
    }

  }

}