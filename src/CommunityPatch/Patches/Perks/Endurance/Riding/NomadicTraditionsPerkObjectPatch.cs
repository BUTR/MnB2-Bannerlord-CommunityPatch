using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public sealed class NomadicTraditionsPerkObjectPatch : PerkPatchBase<NomadicTraditionsPerkObjectPatch> {

    public override bool Applied { get; protected set; }

    public override bool? IsApplicable(Game game)
      => Perk?.PrimaryBonus.Equals(0.5f);

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.3f);

      Applied = true;
    }

    public NomadicTraditionsPerkObjectPatch() : base("PB5iowxh") {
    }

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

  }

}