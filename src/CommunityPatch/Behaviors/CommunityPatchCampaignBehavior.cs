using System.Reflection;
using TaleWorlds.CampaignSystem;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Behaviors {

  public class CommunityPatchCampaignBehavior : CampaignBehaviorBase {

    private static readonly FieldInfo PartyPureSpeedLastCheckVersionField = typeof(MobileParty).GetField("_partyPureSpeedLastCheckVersion", Instance | NonPublic);

    public override void RegisterEvents()
      => CampaignEvents.PerkOpenedEvent.AddNonSerializedListener(this, OnPerkOpened);

    private void OnPerkOpened(Hero hero, PerkObject openedPerk) {
      if (hero != null && openedPerk == DefaultPerks.Riding.NomadicTraditions)
        PartyPureSpeedLastCheckVersionField.SetValue(hero.PartyBelongedTo, -1);
    }

    public override void SyncData(IDataStore dataStore) {
    }

  }

}