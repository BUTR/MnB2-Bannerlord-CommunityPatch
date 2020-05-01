using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public sealed class NomadicTraditionsPerkObjectPatch : IPatch {

    public bool Applied { get; private set; }

    private PerkObject _perk;

    public bool? IsApplicable(Game game)
      => _perk?.PrimaryBonus.Equals(0.5f);

    public void Apply(Game game) {
      _perk.SetPrimaryBonus(0.3f);

      Applied = true;
    }

    public void Reset()
      => _perk = DefaultPerks.Riding.NomadicTraditions;

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

  }

}