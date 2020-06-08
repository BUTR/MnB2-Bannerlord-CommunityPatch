using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {
  public sealed class TorchCarriersPatch : PartySpeedSubPatch<TorchCarriersPatch>, IPartySpeedSubPatch {
    public TorchCarriersPatch() : base("JJm1ZmPx") { }

    void IPartySpeedSubPatch.ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (Campaign.Current.IsNight) {
        finalSpeed.AddFactor(Perk.PrimaryBonus, Perk.Name);
      }
    }
  }
}
