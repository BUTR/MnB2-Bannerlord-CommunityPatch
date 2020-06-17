using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class LayOfTheLandPatch : PartySpeedSubPatch<LayOfTheLandPatch> {

    public LayOfTheLandPatch() : base("P68GX3zY") { }
    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.03f);
      base.Apply(game);
    }

  }

}