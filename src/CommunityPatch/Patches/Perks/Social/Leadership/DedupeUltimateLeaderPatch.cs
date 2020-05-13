using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Social.Leadership {

  public sealed class DedupeUltimateLeaderPatch : PerkPatchBase<DedupeUltimateLeaderPatch> {

    public DedupeUltimateLeaderPatch() : base("FK3W0SKk") {
    }

    public override bool Applied { get; protected set; }

    public override void Reset() {
      if (!Applied)
        return;

      OtherPerk = null;

      Applied = false;
    }

    public PerkObject OtherPerk;

    public override IEnumerable<MethodBase> GetMethodsChecked()
      => Enumerable.Empty<MethodBase>();

    public override bool? IsApplicable(Game game) {
      var isApplicable = base.IsApplicable(game);
      if (isApplicable != true)
        return isApplicable;

      GetOtherPerk();

      return OtherPerk != null;
    }

    private void GetOtherPerk()
      => OtherPerk ??= PerkObject.FindFirst(x => x != Perk && x.Name.GetID() == PerkId);

    private static readonly AccessTools.FieldRef<MBReadOnlyList<PerkObject>, List<PerkObject>> MbReadOnlyListGetter
      = AccessTools.FieldRefAccess<MBReadOnlyList<PerkObject>, List<PerkObject>>("_list");

    public override void Apply(Game game) {
      // ReSharper disable once ArgumentsStyleOther
      MbReadOnlyListGetter(Campaign.Current.PerkList).Remove(OtherPerk);

      if (Applied) return;

      Applied = true;
    }

  }

}