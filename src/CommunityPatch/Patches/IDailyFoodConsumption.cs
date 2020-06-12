using TaleWorlds.CampaignSystem;

namespace Patches {

  public interface IDailyFoodConsumption {

    void ModifyDailyFoodConsumption(ref float dailyFoodConsumption, MobileParty mobileParty, StatExplainer dailyFoodConsumptionExplanation);

  }

}