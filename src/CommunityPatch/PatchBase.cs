using JetBrains.Annotations;
using TaleWorlds.Core;

namespace CommunityPatch {

  public abstract class PatchBase<TPatch> : IPatch where TPatch : IPatch {

    [PublicAPI]
    public static TPatch ActivePatch
      => (TPatch) CommunityPatchSubModule.ActivePatches[typeof(TPatch)];

    public abstract bool IsApplicable(Game game);

    public abstract void Apply(Game game);

    public abstract bool Applied { get; protected set; }

    public abstract void Reset();

  }

}