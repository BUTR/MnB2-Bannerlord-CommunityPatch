using Patches;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Social.Trade {

  public class MercenaryTraderPatch  : PartySizeLimitSubPatch<MercenaryTraderPatch> {

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(15.0f);
      base.Apply(game);
    }

    public MercenaryTraderPatch() : base("radvxImF") { }

  }

}