#if !AFTER_E1_4_3

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.PatchApplicabilityHelper;

namespace CommunityPatch.Patches.Perks.Social.Leadership {

  [PatchObsolete(ApplicationVersionType.EarlyAccess, 1, 4, 1)]
  public sealed class UltimateLeaderPatch : PerkPatchBase<UltimateLeaderPatch> {

    public UltimateLeaderPatch() : base("FK3W0SKk") {
    }

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultPartySizeLimitModel).GetMethod("AddUltimateLeaderPerkEffect", Public | NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PrefixMethodInfo =
      typeof(UltimateLeaderPatch).GetMethod(nameof(AddUltimateLeaderPerkEffectPrefix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public override void Reset() {
      if (!Applied)
        return;

      CommunityPatchSubModule.Harmony.Unpatch(TargetMethodInfo, PrefixMethodInfo);
      Applied = false;
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

    public override bool? IsApplicable(Game game) {
      if (!IsTargetPatchable(TargetMethodInfo, Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      DedupePerksIfNeeded();

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

      var perk = ActivePatch.Perk;

      if (!clanLeader.GetPerkValue(perk))
        return false;

      result.Add((leadershipValue - 250) * perk!.PrimaryBonus, perk.Name);

      return false;
    }

    private static readonly AccessTools.FieldRef<MBReadOnlyList<PerkObject>, List<PerkObject>> MbReadOnlyListGetter
      = AccessTools.FieldRefAccess<MBReadOnlyList<PerkObject>, List<PerkObject>>("_list");

    private void DedupePerksIfNeeded() {
      var dupePerkList = GetDuplicatePerks(Campaign.Current.PerkList);
      foreach (var dupePerk in dupePerkList) {
        // ReSharper disable once ArgumentsStyleOther
        MbReadOnlyListGetter(Campaign.Current.PerkList).Remove(dupePerk);
      }
    }

    // the description for Ultimate Leader indicates everything except the lowest skill required version are duplicates
    // ReSharper disable once ReturnTypeCanBeEnumerable.Local
    private List<PerkObject> GetDuplicatePerks(IEnumerable<PerkObject> perkList)
      => perkList.Where(perk => perk.Name.GetID() == PerkId)
        .OrderBy(perk => perk.RequiredSkillValue).Skip(1).ToList();

  }

}

#endif