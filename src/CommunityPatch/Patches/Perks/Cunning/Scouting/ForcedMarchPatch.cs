using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class ForcedMarchPatch : PartySpeedSubPatch<ForcedMarchPatch> {

    public ForcedMarchPatch() : base("jhZe9Mfo") { }
    
    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.03f);
      base.Apply(game);
    }

    protected override bool IsPerkConditionFulfilled(MobileParty mobileParty, float baseSpeed, ExplainedNumber finalSpeed) 
      => mobileParty.Morale > Campaign.Current.Models.PartyMoraleModel.HighMoraleValue;

  }

}