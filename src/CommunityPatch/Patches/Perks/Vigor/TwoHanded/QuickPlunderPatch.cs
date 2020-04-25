using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class QuickPlunderPatch : PatchBase<QuickPlunderPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = RaidingHelper.TargetMethodInfo;

    private static readonly MethodInfo PatchMethodInfo = typeof(QuickPlunderPatch).GetMethod(nameof(Prefix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = RaidingHelper.Hashes;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "9hhpbcOI");

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
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );

      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 5f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Prefix(ref MapEvent __instance) {
      if (RaidingHelper.IsNotRaidingEvent(__instance)) return;
      if (RaidingHelper.IsTheRaidHitNotHappeningNow(__instance, out var damageAccumulated)) return;

      ApplyPerkBonusToRaidHit(__instance, damageAccumulated);
    }

    private static void ApplyPerkBonusToRaidHit(MapEvent __instance, float hitDamage) {
      var perk = ActivePatch._perk;
      var partyMemberHitDamage = new ExplainedNumber(hitDamage);

      foreach (var party in __instance.AttackerSide.Parties.Where(x => x.MobileParty != null))
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberHitDamage);

      RaidingHelper.SetHitDamage(__instance, partyMemberHitDamage.ResultNumber);
    }

  }

}