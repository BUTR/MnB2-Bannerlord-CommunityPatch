using Patches;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public abstract class ScoutingLorePerk<TPatch> : PerkPatchBase<TPatch>, IPartySpeed, IDailyFoodConsumption where TPatch : ScoutingLorePerk<TPatch> {
    
    protected ScoutingLorePerk(string perkId) : base(perkId) { }

    public virtual void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      IPartySpeed modifyPartySpeed = new ModifyPartySpeed(Perk);
      modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed); 
    }

    public virtual void ModifyDailyFoodConsumption(ref float dailyFoodConsumption, MobileParty mobileParty, StatExplainer dailyFoodConsumptionExplanation) {
      IDailyFoodConsumption halfDailyFoodConsumption = new HalfDailyFoodConsumption(Perk);
      halfDailyFoodConsumption.ModifyDailyFoodConsumption(ref dailyFoodConsumption, mobileParty, dailyFoodConsumptionExplanation); 
    }

  }

}