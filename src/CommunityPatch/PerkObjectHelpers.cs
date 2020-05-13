using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch {

  internal static class PerkObjectHelpers {

    [CanBeNull]
    public static PerkObject Load(string id) {
      try {
        // campaign scenarios
        return PerkObject.FindFirst(x => x.Name.GetID() == id);
      }
      catch {
        // custom battle & other non-campaign scenarios
        try {
          return DefaultPerks.GetAllPerks().FirstOrDefault(x => x.Name.GetID() == id);
        }
        catch {
          // oof
          return null;
        }
      }
    }

  }

}