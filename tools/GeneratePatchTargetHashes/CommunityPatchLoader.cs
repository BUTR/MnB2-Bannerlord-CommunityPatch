using System;
using System.Linq;
using System.Reflection;
using System.Text;
using CommunityPatch;
using JetBrains.Annotations;
using static System.Reflection.BindingFlags;

internal static class CommunityPatchLoader {

  private static readonly ApplicationVersionComparer VersionComparer = new ApplicationVersionComparer();

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

  [PublicAPI]
  public static string GetFormattedHexOfCilSignatureSha256(byte[] bytes)
    => bytes?.GetFormattedCsHexArray();

  public static void GenerateHashes() {
    var gameVersion = CommunityPatchSubModule.GameVersion;
    foreach (var patch in CommunityPatchSubModule.Patches) {
      var type = patch.GetType();
      var patchName = type.Name;
      var obsolete = type.GetCustomAttribute<PatchObsoleteAttribute>();
      if (obsolete != null)
        if (VersionComparer.Compare(gameVersion, obsolete.Version) >= 0) {
          Console.WriteLine($"{patchName}: obsoleted by version {obsolete.Version}, game is {gameVersion}");
          continue;
        }
      var notBefore = type.GetCustomAttribute<PatchNotBeforeAttribute>();
      if (notBefore != null)
        if (VersionComparer.Compare(gameVersion, notBefore.Version) <= 0) {
          Console.WriteLine($"{patchName}: not before version {notBefore.Version}, game is {gameVersion}");
          continue;
        }

      try {
        foreach (var mb in patch.GetMethodsChecked()) {
          if (mb == null) {
            Console.WriteLine($"{patchName}: GetMethodsChecked invocation returned a null. Must be missing a method in this version.");
            continue;
          }
          var mbName = mb.Name;

          MemberInfo match;
          var hash = mb.MakeCilSignatureSha256();

          var hashFields = type
            .GetFields(Public | NonPublic | Static | FlattenHierarchy)
            .Where(f => f.Name.Contains("Hash") && f.FieldType == typeof(byte[][]))
            .ToList();

          foreach (var field in hashFields) {
            var hashes = (byte[][]) field.GetValue(field.IsStatic ? null : patch);
            if (!hash.MatchesAnySha256(hashes))
              continue;

            match = field;
            goto matched;
          }

          var hashStaticProps = type
            .GetProperties(Public | NonPublic | Static | FlattenHierarchy)
            .Where(f => f.Name.Contains("Hash") && f.PropertyType == typeof(byte[][]))
            .ToList();

          foreach (var prop in hashStaticProps) {
            var hashes = (byte[][]) prop.GetValue(null);
            if (!hash.MatchesAnySha256(hashes))
              continue;

            match = prop;
            goto matched;
          }

          Console.WriteLine($"{patchName} {mbName}:");
          Console.WriteLine(mb.GetFormattedHexOfCilSignatureSha256());

          continue;

          matched:
          Console.WriteLine($"{patchName} {mbName}: {match.Name} already matched.");
        }
      }
      catch (Exception ex) {
        Console.WriteLine($"{patchName}: Failed due to exception.");
        Console.WriteLine(ex.ToString());
      }
    }
  }

}