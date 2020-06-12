using System;
using System.Linq;
using Patches;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public class HalfDailyFoodConsumption : IDailyFoodConsumption {

    private PerkObject _perk;

    public HalfDailyFoodConsumption(PerkObject perk) {
      this._perk = perk;
    }

    public void ModifyDailyFoodConsumption(ref float dailyFoodConsumption, MobileParty mobileParty, StatExplainer dailyFoodConsumptionExplanation) {

      if (mobileParty == null || _perk == null || !mobileParty.HasPerk(_perk))
        return;
      
      var extra = Math.Abs(dailyFoodConsumption) / 2;
      if (extra <= 0f)
        return;

      AddExplainedBonus(ref dailyFoodConsumption, extra, dailyFoodConsumptionExplanation);
      
    }

    private void AddExplainedBonus(ref float baseValue, float extra, StatExplainer explanation) {
      var explainedNumber = new ExplainedNumber(baseValue, explanation);
      var baseLine = explanation?.Lines?.Find(explainedLine => explainedLine?.Name == "Base");
      if (baseLine != null)
        explanation.Lines.Remove(baseLine);

      explainedNumber.Add(extra, _perk.Name);
      baseValue = explainedNumber.ResultNumber;
    }

  }

}