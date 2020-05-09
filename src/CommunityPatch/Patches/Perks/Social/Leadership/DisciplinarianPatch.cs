using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Social.Leadership {

  public sealed class DisciplinarianPatch : PerkPatchBase<DisciplinarianPatch> {

    public override bool Applied { get; protected set; }

    public override bool? IsApplicable(Game game)
      => Perk?.PrimaryRole == SkillEffect.PerkRole.Personal;

    public override void Apply(Game game) {
      Perk.SetPrimary(SkillEffect.PerkRole.PartyLeader, 0f);

      Applied = true;
    }

    public DisciplinarianPatch() : base("ER3ieXOb") {
    }

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

  }

}