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

  public sealed class EyeForLoot : PatchBase<EyeForLoot> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = RaidingHelper.TargetMethodInfo;

    private static readonly MethodInfo PatchMethodInfo = typeof(EyeForLoot).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = RaidingHelper.Hashes;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "bRGnkt9B");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus.IsDifferentFrom(.05f)) return false;
      if (TargetMethodInfo == null) return false;
      if (RaidingHelper.NextSettlementDamage == null) return false;
      if (RaidingHelper.IsFinishCalled == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      _perk.SetPrimaryBonus(5f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Prefix(ref MapEvent __instance) {
      if (RaidingHelper.IsNotRaidingEvent(__instance)) return;
      if (RaidingHelper.IsTheRaidHitNotHappeningNow(__instance, out var damageAccumulated)) return;

      ApplyPerkExtraRewardBonusToRaidHit(__instance, damageAccumulated);
    }

    private static void ApplyPerkExtraRewardBonusToRaidHit(MapEvent __instance, float hitDamage) {
      var perk = ActivePatch._perk;
      var partyMemberHitDamage = new ExplainedNumber(hitDamage);

      foreach (var party in __instance.AttackerSide.Parties.Where(x => x.MobileParty != null))
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberHitDamage);

      var damageBonus = partyMemberHitDamage.ResultNumber - partyMemberHitDamage.BaseNumber;
      RaidingHelper.SetHitDamage(__instance, partyMemberHitDamage.ResultNumber);
      RaidingHelper.IncreaseSettlementHitPoints(__instance, damageBonus);
    }

  }

}