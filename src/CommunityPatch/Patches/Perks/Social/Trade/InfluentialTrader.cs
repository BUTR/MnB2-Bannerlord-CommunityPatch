using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches.Perks.Social.Trade {

  public class InfluentialTrader : DailyInfluenceGainSubPatch<InfluentialTrader> {

    public InfluentialTrader() : base("esgxog1B") { }

    public override void ModifyDailyInfluenceGain(Clan clan, ref ExplainedNumber influenceChange) {
      var perk = Perk;
      if (perk == null || !(clan?.Leader?.GetPerkValue(perk) ?? false))
        return;
      
      var influenceBonus = 0.25f;
      var caravans = clan.Leader.OwnedCaravans?.Count() ?? 0f;
      var workshops = clan.Leader.OwnedWorkshops?.Count ?? 0f;
      var extraCaravanInfluence =  influenceBonus * caravans;
      var extraWorkshopInfluence = influenceBonus * workshops;
      
      if (extraCaravanInfluence > 0)
        influenceChange.Add(extraCaravanInfluence, new TextObject(perk.Name + " (caravans)"));
      
      if (extraWorkshopInfluence > 0)
        influenceChange.Add(extraWorkshopInfluence, new TextObject(perk.Name + " (workshops)"));
    } 

  }

}