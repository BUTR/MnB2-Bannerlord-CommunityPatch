using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace CommunityPatch {

  public static class MethodHelpers {

    public static byte[] GetCilBytes(this MethodBase mb)
      => mb.GetMethodBody()?.GetILAsByteArray() ?? Array.Empty<byte>();

    public static byte[] GetSha256(this byte[] bytes) {
      using var hasher = SHA256.Create();
      return hasher.ComputeHash(bytes);
    }

    public static string GetFormattedHexArray(this byte[] bytes)
      => $"{{ 0x{string.Join(", 0x", bytes.Select(b => b.ToString("X2")))}}}";

#if DEBUG
    [PublicAPI]
    public static string GetFormattedHexOfSha256OfCil(string typeName, string methodName) {
      var type = Type.GetType(typeName);
      var method = type?.GetMethod(methodName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
      return method?.GetCilBytes().GetSha256().GetFormattedHexArray();
    }
#endif

  }

}