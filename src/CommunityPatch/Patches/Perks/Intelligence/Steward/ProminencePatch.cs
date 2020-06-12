using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class ProminencePatch : DailyInfluenceGainSubPatch<ProminencePatch> {

    public ProminencePatch() : base("71EyPbaE") { }

    public override void ModifyDailyInfluenceGain(Clan clan, ref ExplainedNumber influenceChange) {
      var perk = ActivePatch.Perk;

      if (clan?.IsUnderMercenaryService ?? true)
        return;

      var ruler = clan.Kingdom?.Ruler;
      if (ruler != null && ruler != clan.Leader && clan.Leader.GetPerkValue(perk))
        influenceChange.Add(perk.PrimaryBonus, perk.Name);
    }

  }

}