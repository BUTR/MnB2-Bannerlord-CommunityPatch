using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Behaviors {

  public class CommunityPatchCampaignBehavior : CampaignBehaviorBase {

    private static readonly FieldInfo VersionField = typeof(MobileParty).GetField("_partyPureSpeedLastCheckVersion", BindingFlags.Instance | BindingFlags.NonPublic);

    public override void RegisterEvents()
      => CampaignEvents.PerkOpenedEvent.AddNonSerializedListener(this, OnPerkOpened);

    private void OnPerkOpened(Hero hero, PerkObject openedPerk) {
      if (hero == null || openedPerk != DefaultPerks.Riding.NomadicTraditions)
        return;

      VersionField.SetValue(hero.PartyBelongedTo, -1);
    }

    public override void SyncData(IDataStore dataStore) {
    }

  }

}