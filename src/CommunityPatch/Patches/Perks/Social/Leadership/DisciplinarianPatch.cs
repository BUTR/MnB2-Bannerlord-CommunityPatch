using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches.Perks.Social.Leadership {

  public sealed class DisciplinarianPatch : IPatch {

    public bool Applied { get; private set; }

    private PerkObject _perk;

    public bool? IsApplicable(Game game)
      => _perk?.PrimaryRole == SkillEffect.PerkRole.Personal;

    public void Apply(Game game) {
      _perk.SetPrimary(SkillEffect.PerkRole.PartyLeader, 0f);

      Applied = true;
    }

    public void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "ER3ieXOb");

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

  }

}