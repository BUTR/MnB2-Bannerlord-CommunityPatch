using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace CommunityPatch {

  internal static class PerkObjectExtensions {

    public static void SetPrimaryBonus(this PerkObject perk, float primaryBonus)
      => perk.SetPrimary(perk.PrimaryRole, primaryBonus);

    public static void SetPrimary(this PerkObject perk, SkillEffect.PerkRole primaryRole, float primaryBonus)
#if AFTER_E1_4_2
      => perk.Modify(primaryRole, primaryBonus, perk.PrimaryIncrementType);
#else
      => perk.Modify(primaryRole, primaryBonus, perk.IncrementType);
#endif

    public static void SetIncrementType(this PerkObject perk, SkillEffect.EffectIncrementType incrementType)
      => perk.Modify(perk.PrimaryRole, perk.PrimaryBonus, incrementType);

    public static void Modify(this PerkObject perk, float primaryBonus, SkillEffect.EffectIncrementType incrementType)
      => perk.Modify(perk.PrimaryRole, primaryBonus, incrementType);

    public static void Modify(this PerkObject perk, SkillEffect.PerkRole primaryRole, float primaryBonus, SkillEffect.EffectIncrementType incrementType) {
      // Dear TaleWorlds; Value should probably be publicly exposed, maybe by a method
      // and maybe marked [Obsolete] if you want to avoid your developers doing dirty deeds
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          perk.Name,
          perk.Description
        }
      );
      // most of the properties of skills have private setters, yet Initialize is public
      perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        perk.Skill,
        (int) perk.RequiredSkillValue,
        perk.AlternativePerk,
        primaryRole, primaryBonus,
        perk.SecondaryRole, perk.SecondaryBonus,
        incrementType
      );
    }

  }

}