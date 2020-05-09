using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static System.Reflection.BindingFlags;
using static CommunityPatch.PatchApplicabilityHelper;

namespace CommunityPatch.Patches.Perks.Social.Leadership {

  public sealed class UltimateLeaderPatch : PatchBase<UltimateLeaderPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultPartySizeLimitModel).GetMethod("AddUltimateLeaderPerkEffect", Public | NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PrefixMethodInfo =
      typeof(UltimateLeaderPatch).GetMethod(nameof(AddUltimateLeaderPerkEffectPrefix), Public | NonPublic | Static | DeclaredOnly);

    private List<PerkObject> _ultimateLeaderProbablePerks;

    private PerkObject _ultimateLeaderPerk;

    private TextObject _leadershipPerkUltimateLeaderBonusText;

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public override void Reset() {
      Initialize();
      if (!Applied)
        return;

      CommunityPatchSubModule.Harmony.Unpatch(TargetMethodInfo, PrefixMethodInfo);
      Applied = false;
    }

    private void Initialize() {
      _leadershipPerkUltimateLeaderBonusText =
        GameTexts.FindText("str_leadership_perk_bonus");

      _ultimateLeaderProbablePerks =
        Campaign.Current.PerkList.Where(perk => perk.Name.GetID() == "FK3W0SKk").ToList();

      _ultimateLeaderPerk = GetUltimateLeadershipPerk(_ultimateLeaderProbablePerks);

      _ultimateLeaderProbablePerks.Add(_ultimateLeaderPerk);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.1.228624
        0x60, 0x53, 0x11, 0x00, 0x46, 0xA3, 0x03, 0xC3,
        0xB5, 0xAE, 0x65, 0xE8, 0xBD, 0x37, 0x57, 0xE7,
        0x01, 0x4C, 0x6C, 0x5C, 0xC3, 0x4A, 0x63, 0xED,
        0xCD, 0xB6, 0xD3, 0x40, 0xEF, 0xD7, 0x8B, 0x69
      }
    };

    public override bool? IsApplicable(Game game)
      => _ultimateLeaderPerk != null && IsTargetPatchable(TargetMethodInfo, Hashes);

    public override void Apply(Game game) {
      if (Applied) return;

      // ReSharper disable once ArgumentsStyleOther
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, prefix: new HarmonyMethod(PrefixMethodInfo));
      Applied = true;
    }

    // the same as base game except doesn't require your clan leads your faction
    // and applies whether you have either of the duplicate "Ultimate Leader" perks
    public static bool AddUltimateLeaderPerkEffectPrefix(MobileParty party, ref ExplainedNumber result) {
      var clanLeader = party.LeaderHero?.Clan?.Leader;
      if (clanLeader == null)
        return false;

      var leadershipValue = clanLeader.GetSkillValue(DefaultSkills.Leadership);
      if (leadershipValue <= 250)
        return false;

      if (!ActivePatch.HasUltimateLeadershipPerk(clanLeader))
        return false;

      result.Add(
        (leadershipValue - 250) * ActivePatch._ultimateLeaderPerk.PrimaryBonus,
        ActivePatch._leadershipPerkUltimateLeaderBonusText);

      return false;
    }

    private static PerkObject GetUltimateLeadershipPerk(IReadOnlyList<PerkObject> probablePerks) {
      try {
        // ReSharper disable once PossibleNullReferenceException
        return (PerkObject) typeof(DefaultPerks.Leadership)
          .GetProperty("UltimateLeader", Public | NonPublic | Static | DeclaredOnly)
          .GetGetMethod()
          .Invoke(null, null);
      }
      catch {
        // ignored
      }

      try {
        if (probablePerks.Count == 0)
          throw new InvalidOperationException("No probable perks while looking for Ultimate Leadership");

        return probablePerks[0];
      }
      catch {
        // ignored
      }

      throw new InvalidOperationException("can't locate UltimateLeadership perk");
    }

    private bool HasUltimateLeadershipPerk(Hero hero)
      => _ultimateLeaderProbablePerks.Any(hero.GetPerkValue);

  }

}