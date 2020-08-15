using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public sealed class NomadicTraditionsPerkObjectPatch : PerkPatchBase<NomadicTraditionsPerkObjectPatch> {

    public override bool Applied { get; protected set; }

    public override bool? IsApplicable(Game game) {
      if (!Perk?.PrimaryBonus.Equals(0.5f) ?? false)
        return false;

      return base.IsApplicable(game);
    }

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