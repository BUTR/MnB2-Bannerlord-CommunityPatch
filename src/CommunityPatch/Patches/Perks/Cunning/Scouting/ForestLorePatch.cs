using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.TerrainType;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class ForestLorePatch : PerkPatchBase<ForestLorePatch>, IPartySpeed, IDailyFoodConsumption {

    private readonly IDailyFoodConsumption _halfDailyFoodConsumption;
    private readonly IPartySpeed _modifyPartySpeed;

    public ForestLorePatch() : base("TgOwisdD") {
      _halfDailyFoodConsumption = new HalfDailyFoodConsumption(Perk);
      _modifyPartySpeed = new ModifyPartySpeed(Perk);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (mobileParty.IsInTerrainType(Forest))
        _modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed); 
    }

    public void ModifyDailyFoodConsumption(ref float dailyFoodConsumption, MobileParty mobileParty, StatExplainer dailyFoodConsumptionExplanation)
      => _halfDailyFoodConsumption.ModifyDailyFoodConsumption(ref dailyFoodConsumption, mobileParty, dailyFoodConsumptionExplanation);

  }

}