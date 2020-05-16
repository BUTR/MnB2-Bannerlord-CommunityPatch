using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {
  public sealed class PathfinderPatch : PartySpeedSubPatch<PathfinderPatch>, IPartySpeedSubPatch {
    public PathfinderPatch() : base("d2qGHXyx") { }

    void IPartySpeedSubPatch.ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (Campaign.Current.IsDay) {
        finalSpeed.AddFactor(Perk.PrimaryBonus, Perk.Name);
      }
    }
  }
}
