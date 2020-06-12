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

  public sealed class SupremeAuthorityPatch : DailyInfluenceGainSubPatch<SupremeAuthorityPatch> {

    public SupremeAuthorityPatch() : base("SFjspNSf") { }

    public override void ModifyDailyInfluenceGain(Clan clan, ref ExplainedNumber influenceChange) {
      var perk = ActivePatch.Perk;

      var ruler = clan?.Kingdom?.Ruler;
      var leader = clan?.Leader;
      if (ruler != null && ruler == leader && ruler.GetPerkValue(perk))
        influenceChange.Add(perk.PrimaryBonus, perk.Name);
    }

  }

}