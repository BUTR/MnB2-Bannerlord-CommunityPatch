#if !AFTER_E1_4_3

using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Social.Leadership {

  [PatchObsolete(ApplicationVersionType.EarlyAccess, 1, 0, 6)]
  public sealed class DisciplinarianPatch : PerkPatchBase<DisciplinarianPatch> {

    public override bool Applied { get; protected set; }

    public override bool? IsApplicable(Game game) {
      if (Perk?.PrimaryRole != SkillEffect.PerkRole.Personal)
        return false;

      return base.IsApplicable(game);
    }

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

#endif