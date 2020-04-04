using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch {

  [PublicAPI]
  public partial class CommunityPatchSubModule : MBSubModuleBase {

    public override void OnGameInitializationFinished(Game game) {
      var patchType = typeof(IPatch);
      var patches = new LinkedList<IPatch>();

      Print("BeginGameStart");

      foreach (var type in typeof(CommunityPatchSubModule).Assembly.GetTypes()) {
        if (!patchType.IsAssignableFrom(type))
          continue;
        if (patchType == type)
          continue;

        try {
          patches.AddLast((IPatch) Activator.CreateInstance(type, true));
        }
        catch (TargetInvocationException tie) {
          Error(tie.InnerException, $"Failed to create instance of patch: {type.FullName}");
        }
        catch (Exception ex) {
          Error(ex, $"Failed to create instance of patch: {type.FullName}");
        }
      }

      foreach (var patch in patches) {
        try {
          if (!patch.IsApplicable(game))
            continue;

          try {
            patch.Apply(game);
          }
          catch (Exception ex) {
            Error(ex, $"Error while applying patch: {patch.GetType().FullName}");
          }
        }
        catch (Exception ex) {
          Error(ex, $"Error while checking if patch is applicable: {patch.GetType().FullName}");
        }
      }

      base.OnGameInitializationFinished(game);
    }

  }

}