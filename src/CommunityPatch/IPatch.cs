using JetBrains.Annotations;
using TaleWorlds.Core;

namespace CommunityPatch {

  [PublicAPI]
  public interface IPatch {

    bool IsApplicable(Game game);

    void Apply(Game game);
    
    bool Applied { get; }

  }

}