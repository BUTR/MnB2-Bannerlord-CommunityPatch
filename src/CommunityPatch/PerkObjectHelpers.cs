using System;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch {

  internal static class PerkObjectHelpers {

    [CanBeNull]
    public static PerkObject Load(string id)
      => Load(x => x.Name.GetID() == id);

    [CanBeNull]
    public static PerkObject Load(Func<PerkObject, bool> predicate) {
      try {
        // campaign scenarios
        return PerkObject.FindFirst(predicate);
      }
      catch {
        // custom battle & other non-campaign scenarios
        try {
          return DefaultPerks.GetAllPerks().FirstOrDefault(predicate);
        }
        catch {
          // oof
          return null;
        }
      }
    }

  }

}