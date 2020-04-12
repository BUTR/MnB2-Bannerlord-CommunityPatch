using System;
using CommunityPatch;
using Mono;
using Mono.Cecil;
using Mono.Cecil.Metadata;

internal static class CommunityPatchLoader {

  public static void GenerateHashes() {
    foreach (var patch in CommunityPatchSubModule.Patches) {
      foreach (var mb in patch.GetMethodsChecked()) {
        var patchName = patch.GetType().Name;
        var mbName = mb.Name;

        Console.WriteLine($"{patchName} {mbName}:");
        Console.WriteLine(mb.GetFormattedHexOfCilSignatureSha256());
      }
    }
  }

}