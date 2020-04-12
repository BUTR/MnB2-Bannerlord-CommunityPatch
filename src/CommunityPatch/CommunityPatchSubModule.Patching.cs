using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    internal static readonly Harmony Harmony = new Harmony(nameof(CommunityPatch));

    public static readonly IDictionary<Type, IPatch> ActivePatches
      = new Dictionary<Type, IPatch>();

    private static void ApplyPatches(Game game) {
      //ActivePatches.Clear();

      foreach (var patch in Patches) {
        try {
          patch.Reset();
        }
        catch (Exception ex) {
          Error(ex, $"Error while resetting patch: {patch.GetType().Name}");
        }

        try {
          if (patch.IsApplicable(game)) {
            try {
              patch.Apply(game);
            }
            catch (Exception ex) {
              Error(ex, $"Error while applying patch: {patch.GetType().Name}");
            }
          }
        }
        catch (Exception ex) {
          Error(ex, $"Error while checking if patch is applicable: {patch.GetType().Name}");
        }

        var patchApplied = patch.Applied;
        if (patchApplied)
          ActivePatches[patch.GetType()] = patch;

        ShowMessage($"{(patchApplied ? "Applied" : "Skipped")} Patch: {patch.GetType().Name}");
      }
    }

    private static LinkedList<IPatch> _patches;

    public static LinkedList<IPatch> Patches {
      get {
        if (_patches != null)
          return _patches;

        var patchInterfaceType = typeof(IPatch);
        _patches = new LinkedList<IPatch>();

        foreach (var type in typeof(CommunityPatchSubModule).Assembly.GetTypes()) {
          if (type.IsInterface || type.IsAbstract)
            continue;
          if (!patchInterfaceType.IsAssignableFrom(type))
            continue;

          try {
            var patch = (IPatch) Activator.CreateInstance(type, true);
            //var patch = (IPatch) FormatterServices.GetUninitializedObject(type);
            _patches.AddLast(patch);
          }
          catch (TargetInvocationException tie) {
            Error(tie.InnerException, $"Failed to create instance of patch: {type.FullName}");
          }
          catch (Exception ex) {
            Error(ex, $"Failed to create instance of patch: {type.FullName}");
          }
        }

        return _patches;
      }
    }

  }

}