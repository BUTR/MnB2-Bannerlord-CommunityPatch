#if false
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches {

  public sealed class VassalReleasePatch : IPatch {

    public bool Applied { get; private set; }

    public bool? IsApplicable(Game game)
      => Campaign.Current.ConversationManager.

    public void Apply(Game game) {
      
      game
      
    }

  }

}
#endif