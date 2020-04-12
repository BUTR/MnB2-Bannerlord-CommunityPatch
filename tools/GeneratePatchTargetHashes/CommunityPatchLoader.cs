using System;
using System.Reflection;
using System.Security.Cryptography;
using CommunityPatch;
using JetBrains.Annotations;
using Mono;
using Mono.Cecil;
using Mono.Cecil.Metadata;

internal static class CommunityPatchLoader {

  [PublicAPI]
  public static string GetFormattedHexOfCilSignatureSha256(string typeName, string methodName) {
    var type = Type.GetType(typeName);
    var method = type?.GetMethod(methodName,
      BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
    return method?.GetFormattedHexOfCilSignatureSha256();
  }

  [PublicAPI]
  public static string GetFormattedHexOfCilSignatureSha256(this MethodBase mb)
    => mb?.MakeCilSignatureSha256().GetFormattedCsHexArray();

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