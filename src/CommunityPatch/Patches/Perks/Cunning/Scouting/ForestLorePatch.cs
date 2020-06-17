using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.TerrainType;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class ForestLorePatch : ScoutingLorePerk<ForestLorePatch> {

    public ForestLorePatch() : base("TgOwisdD") { }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }
    
    protected override bool IsIncreasedPartySpeedPerkConditionFulfilled(MobileParty mobileParty)
      => mobileParty.IsInTerrainType(Forest);

    protected override bool IsHalvedFoodConsumptionPerkConditionFulfilled(MobileParty mobileParty)
      => true;

  }

}