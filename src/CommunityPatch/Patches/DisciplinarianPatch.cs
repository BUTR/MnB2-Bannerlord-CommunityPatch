using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches {

  internal class DisciplinarianPatch : IPatch {

    public bool IsApplicable(Game game) {
      var disciplinarian = DefaultPerks.Leadership.LeaderOfMasses;

      return disciplinarian.PrimaryRole != SkillEffect.PerkRole.PartyLeader
        && disciplinarian.SecondaryRole != SkillEffect.PerkRole.PartyLeader;
    }

    public void Apply(Game game) {
      var disciplinarian = DefaultPerks.Leadership.LeaderOfMasses;

      // Dear TaleWorlds; Value should probably be publicly exposed, maybe by a method
      // and maybe marked [Obsolete] if you want to avoid your developers doing dirty deeds
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          disciplinarian.Name,
          disciplinarian.Description
        }
      );

      // most of the properties of skills have private setters, yet Initialize is public
      DefaultPerks.Leadership.LeaderOfMasses.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        disciplinarian.Skill,
        (int) disciplinarian.RequiredSkillValue,
        disciplinarian.AlternativePerk,
        disciplinarian.PrimaryRole, disciplinarian.PrimaryBonus,
        SkillEffect.PerkRole.PartyLeader, 0f,
        disciplinarian.IncrementType
      );
    }

  }

}