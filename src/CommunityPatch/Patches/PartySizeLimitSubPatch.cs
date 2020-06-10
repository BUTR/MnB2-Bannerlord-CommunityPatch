using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public abstract class PartySizeLimitSubPatch<TPatch> : PerkPatchBase<TPatch>, IPartySizeLimitSubPatch where TPatch : PartySizeLimitSubPatch<TPatch> {

    protected PartySizeLimitSubPatch(string perkId) : base(perkId) { }

    protected PartySizeLimitSubPatch(string perkId, Func<PerkObject, bool> perkFinder) : base(perkId, perkFinder) { }
    
    protected PartySizeLimitSubPatch(Func<PerkObject, bool> perkFinder) : base(perkFinder) { }

    public virtual void AddPartySizeLimitBonus(ref int partySizeLimit, MobileParty party, StatExplainer explanation) {
      var perk = ActivePatch.Perk;

      var extra = 0;
      if (perk.PrimaryRole == SkillEffect.PerkRole.PartyMember) {
        extra = (int) perk.PrimaryBonus * party?.MemberRoster?
          .Count(x =>
            x.Character != null
            && x.Character.IsHero
            && x.Character.HeroObject != null
            && x.Character.HeroObject.GetPerkValue(perk)
          ) ?? 0;
      } else if (perk.PrimaryRole == SkillEffect.PerkRole.Personal && (party?.LeaderHero?.GetPerkValue(perk) ?? false)) {
        extra = (int) perk.PrimaryBonus;
      }

      if (extra <= 0)
        return;

      AddExplainedPartySizeLimitBonus(ref partySizeLimit, extra, explanation);
    }
    
    protected static void AddExplainedPartySizeLimitBonus(ref int partySizeLimit, int extra, StatExplainer explanation) {
      var explainedNumber = new ExplainedNumber(partySizeLimit, explanation);
      var baseLine = explanation?.Lines?.Find(explainedLine => explainedLine?.Name == "Base");
      if (baseLine != null)
        explanation.Lines.Remove(baseLine);

      explainedNumber.Add(extra, ActivePatch.Perk.Name);
      partySizeLimit = (int) explainedNumber.ResultNumber;
    }

  }

}