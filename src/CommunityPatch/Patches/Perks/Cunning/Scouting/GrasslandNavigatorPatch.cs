using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.TerrainType;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class GrasslandNavigatorPatch : PerkPatchBase<GrasslandNavigatorPatch>, IPartySpeed {

    private readonly IPartySpeed _modifyPartySpeed;

    public GrasslandNavigatorPatch() : base("Ekqj9IFR") 
      => _modifyPartySpeed = new ModifyPartySpeed(Perk);

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      // Party Member
      if (mobileParty.IsInTerrainType(Plain))
        _modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed); 
    }

  }

}