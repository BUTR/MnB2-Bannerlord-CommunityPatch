using Patches;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public abstract class ScoutingLorePerk<TPatch> : PerkPatchBase<TPatch>, IPartySpeed, IDailyFoodConsumption where TPatch : ScoutingLorePerk<TPatch> {
    
    protected ScoutingLorePerk(string perkId) : base(perkId) { }

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (!mobileParty.HasPerk(Perk) || !IsIncreasedPartySpeedPerkConditionFulfilled(mobileParty))
        return;
      
      IPartySpeed modifyPartySpeed = new ModifyPartySpeed(Perk);
      modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed); 
    }

    public void ModifyDailyFoodConsumption(ref float dailyFoodConsumption, MobileParty mobileParty, StatExplainer dailyFoodConsumptionExplanation) {
      if (!mobileParty.HasPerk(Perk) || !IsHalvedFoodConsumptionPerkConditionFulfilled(mobileParty))
        return;
      
      IDailyFoodConsumption halfDailyFoodConsumption = new HalfDailyFoodConsumption(Perk);
      halfDailyFoodConsumption.ModifyDailyFoodConsumption(ref dailyFoodConsumption, mobileParty, dailyFoodConsumptionExplanation); 
    }
    
    protected abstract bool IsIncreasedPartySpeedPerkConditionFulfilled(MobileParty mobileParty);
    
    protected abstract bool IsHalvedFoodConsumptionPerkConditionFulfilled(MobileParty mobileParty);

  }

}