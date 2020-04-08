using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch {

  [PublicAPI]
  public static class ClanHelpers {
    public static Hero GetRoleHero(this Clan clan, SkillEffect.PerkRole role) {
      foreach ( var hero in clan.Heroes ) {
        var heroParty = hero.PartyBelongedTo;
        if (heroParty != null && heroParty.GetHeroPerkRole(hero) == role)
          return hero;

        foreach (var partyBase in hero.OwnedParties) {
          var party = partyBase.MobileParty;
          if (party != null && party?.GetHeroPerkRole(hero) == role)
            return hero;
        }
      }
      return null;
    }
    
    public static Hero GetClanRoleHero(this Hero hero, SkillEffect.PerkRole role)
      => GetRoleHero(hero.Clan, role);
    
    public static Hero GetEffectiveQuartermaster(this Clan clan)
      => GetRoleHero(clan, SkillEffect.PerkRole.Quartermaster);
    
    public static Hero GetEffectiveSurgeon(this Clan clan)
      => GetRoleHero(clan, SkillEffect.PerkRole.Surgeon);
    
    public static Hero GetEffectiveScout(this Clan clan)
      => GetRoleHero(clan, SkillEffect.PerkRole.Scout);
    
    public static Hero GetEffectiveEngineer(this Clan clan)
      => GetRoleHero(clan, SkillEffect.PerkRole.Engineer);

  }

}