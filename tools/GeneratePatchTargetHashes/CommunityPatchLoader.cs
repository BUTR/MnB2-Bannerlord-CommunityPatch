using System;
using System.Reflection;
using System.Text;
using CommunityPatch;
using JetBrains.Annotations;
using static System.Reflection.BindingFlags;

internal static class CommunityPatchLoader {

  public static string GetFormattedCsHexArray(this byte[] bytes) {
    var sb = new StringBuilder();

    sb.AppendLine("new byte[] {");
    sb.AppendFormat("  // ").AppendLine(CommunityPatchSubModule.GameVersion.ToString());
    sb.AppendFormat($"  0x{bytes[0]:X2}");
    for (var i = 1; i < bytes.Length; ++i) {
      if (i % 8 == 0)
        sb.AppendLine(",").Append("  ");
      else
        sb.Append(", ");
      sb.AppendFormat($"0x{bytes[i]:X2}");
    }

    sb.AppendLine();
    sb.AppendLine("}");

    return sb.ToString();
  }

  [PublicAPI]
  public static string GetFormattedHexOfCilSignatureSha256(string typeName, string methodName) {
    var type = Type.GetType(typeName);
    var method = type?.GetMethod(methodName,
      DeclaredOnly | Public | NonPublic | Static | Instance);
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