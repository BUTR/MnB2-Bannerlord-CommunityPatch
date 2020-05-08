using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using HarmonyLib;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;

namespace CommunityPatch {

  internal static class MethodHelpers {

    private static readonly FieldInfo IlInstrOffsetField = typeof(Harmony).Assembly.GetType("HarmonyLib.ILInstruction")
      .GetField("offset", Public | NonPublic | Instance | DeclaredOnly);

    private static readonly CustomAttributeBuilder AggressiveInlining =
      new CustomAttributeBuilder(typeof(MethodImplAttribute).GetConstructor(Public | Instance, null, new[] {typeof(MethodImplOptions)}, null)!, new object[] {MethodImplOptions.AggressiveInlining});

    public static byte[] MakeCilSignatureSha256(this MethodBase mb) {
#if DEBUG_METHOD_SIGNATURE
      using var hashSource = new MemoryStream(65536);
      var hashSourceStrs = new LinkedList<string>();
#endif

      var il = PatchProcessor.ReadMethodBody(mb);
      var buf = new byte[32];
      using var hasher = SHA256.Create();
      hasher.Initialize();

      unsafe void ProcessUnsigned(ulong v, int size) {
        fixed (byte* pBuf = buf)
          *(ulong*) pBuf = v;
        hasher.TransformBlock(buf, 0, size, null, 0);
#if DEBUG_METHOD_SIGNATURE
        hashSource.Write(buf, 0, size);
#endif
      }

      void ProcessSigned(long v, int size)
        => ProcessUnsigned((ulong) v, size);

      void ProcessString(string s) {
        using var sigHasher = SHA256.Create();
        var sig = sigHasher.ComputeHash(Encoding.Unicode.GetBytes(s));
        hasher.TransformBlock(sig, 0, 32, null, 0);
#if DEBUG_METHOD_SIGNATURE
        hashSourceStrs.AddLast(s);
        hashSource.Write(sig, 0, 32);
#endif
      }

      unsafe void ProcessSingle(float v) {
        fixed (byte* pBuf = buf)
          *(float*) pBuf = v;
        hasher.TransformBlock(buf, 0, 4, null, 0);
#if DEBUG_METHOD_SIGNATURE
        hashSource.Write(buf, 0, 4);
#endif
      }

      unsafe void ProcessDouble(double v) {
        fixed (byte* pBuf = buf)
          *(double*) pBuf = v;
        hasher.TransformBlock(buf, 0, 8, null, 0);
#if DEBUG_METHOD_SIGNATURE
        hashSource.Write(buf, 0, 8);
#endif
      }

      void ProcessInstrRef(object r) {
        ProcessSigned((int) IlInstrOffsetField.GetValue(r), 4);
#if DEBUG_METHOD_SIGNATURE
        hashSource.Write(buf, 0, 4);
#endif
      }

      ProcessString(mb.FullDescription());

      foreach (var instr in il) {
        var opCode = instr.Key;
        var opCodeValue = instr.Key.Value;
        var operand = instr.Value;

        for (var i = 0; i < opCode.Size; ++i)
          buf[i] = (byte) (opCodeValue >> (i * 8));

        hasher.TransformBlock(buf, 0, opCode.Size, null, 0);
#if DEBUG_METHOD_SIGNATURE
        hashSource.Write(buf, 0, opCode.Size);
#endif

        switch (operand) {
          // @formatter:off
          case null: break;
          case byte v: ProcessUnsigned(v, 1); break;
          case ushort v: ProcessUnsigned(v, 2); break;
          case char v: ProcessUnsigned(v, 2); break;
          case uint v: ProcessUnsigned(v, 4); break;
          case ulong v: ProcessUnsigned(v, 8); break;
          case sbyte v: ProcessSigned(v, 1); break;
          case short v: ProcessSigned(v, 2); break;
          case int v: ProcessSigned(v, 4); break;
          case long v: ProcessSigned(v, 8); break;
          case float v: ProcessSingle(v); break;
          case double v: ProcessDouble(v); break;
          case string s: ProcessString(s); break;
          case MethodBase omb: ProcessString(omb.FullDescription()); break;
          case LocalVariableInfo lvi: ProcessSigned(lvi.LocalIndex, 4); break;
          case FieldInfo fi: ProcessString( $"{fi.DeclaringType.FullDescription()}:{fi.Name}"); break;
          case Type t: ProcessString( t.FullDescription()); break;
          // @formatter:on
          case ParameterInfo p: {
            if (p.Member is MethodBase pmb && pmb == mb)
              ProcessSigned(mb.GetParameters().IndexOf(p), 4);
            else
              throw new NotImplementedException("param ref of other method?");

            break;
          }
          default: {
            var t = operand.GetType();

            // idk why ILInstruction is internal

            // ReSharper disable once InvertIf // branch
            if (t.FullName == "HarmonyLib.ILInstruction") {
              ProcessInstrRef(operand);
              break;
            }

            // ReSharper disable once InvertIf // switch
            if (t.FullName == "HarmonyLib.ILInstruction[]") {
              var instrRefs = ((IEnumerable) operand).Cast<object>().ToArray();
              ProcessSigned(instrRefs.Length, 4);
              foreach (var instrRef in instrRefs)
                ProcessInstrRef(instrRef);

              break;
            }

            throw new NotImplementedException(t.FullName);
          }
        }
      }

      hasher.TransformFinalBlock(buf, 0, 0);

#if DEBUG_METHOD_SIGNATURE
      {
        var hashSb = new StringBuilder(64);
        for (var i = 0; i < 32; ++i)
          hashSb.AppendFormat($"{hasher.Hash[i]:X2}");
        var hashStr = hashSb.ToString();

        var sb = new StringBuilder();
        hashSource.Position = 0;
        for (;;) {
          var b = hashSource.ReadByte();
          if (b == -1) break;

          sb.AppendFormat($"{b:X2}");
        }

        CommunityPatchSubModule.Print("=============================================");
        CommunityPatchSubModule.Print("============== Source Data: =================");
        CommunityPatchSubModule.Print(sb.ToString());
        CommunityPatchSubModule.Print("============ Source Strings: ================");
        foreach (var s in hashSourceStrs)
          CommunityPatchSubModule.Print(s);
        CommunityPatchSubModule.Print("=============================================");
        CommunityPatchSubModule.Print("Hash: " + hashStr);
        CommunityPatchSubModule.Print("=============================================");
        CommunityPatchSubModule.Print("=============================================");
      }
#endif

      return hasher.Hash;
    }

    public static Type GetThisParamType(this MethodBase method) {
      if (method.IsStatic)
        return null;

      var type = method.DeclaringType;
      if (type!.IsValueType)
        type = type.MakeByRefType();
      return type;
    }

    public static TDelegate BuildInvoker<TDelegate>(this MethodBase m) where TDelegate : Delegate {
      var td = typeof(TDelegate);
      Dynamic.AccessInternals(m);
      var dtMi = td.GetMethod("Invoke", Public | NonPublic | Static | Instance);
      var dtPs = dtMi!.GetParameters();
      var dt = Dynamic.CreateStaticClass();
      var mn = $"{m.Name}Invoker";
      var d = Sigil.Emit<TDelegate>.BuildStaticMethod(dt, mn, MethodAttributes.Public);
      var ps = m.GetParameters();
      if (m.IsStatic) {
        for (ushort i = 0; i < ps.Length; i++) {
          var p = ps[i];
          var dp = dtPs[i];
          if (p.ParameterType != dp.ParameterType)
            throw new NotImplementedException($"Unhandled parameter difference: {p.ParameterType.FullName} vs. {dp.ParameterType.FullName}");

          d.LoadArgument(i);
        }
      }
      else {
        var thisParamType = m.GetThisParamType();
        if (dtPs[0].ParameterType != thisParamType)
          throw new NotImplementedException($"Unhandled this parameter difference: {dtPs[0].ParameterType.FullName} vs. {thisParamType}");

        d.LoadArgument(0);
        for (var i = 0; i < ps.Length; i++) {
          var p = ps[i];
          var dp = dtPs[i + 1];
          if (p.ParameterType != dp.ParameterType)
            throw new NotImplementedException($"Unhandled parameter difference: {p.ParameterType.FullName} vs. {dp.ParameterType.FullName}");

          d.LoadArgument((ushort) (i + 1));
        }
      }

      switch (m) {
        case MethodInfo mi:
          d.Call(mi);
          break;
        case ConstructorInfo ci:
          d.Call(ci);
          break;
        default:
          throw new NotSupportedException(m.MemberType.ToString());
      }

      d.Return();
      var mb = d.CreateMethod();
      mb.SetCustomAttribute(AggressiveInlining);
      var dti = dt.CreateTypeInfo();
      var dmi = dti!.GetMethod(mn, DeclaredOnly | Static | Public);
      return (TDelegate) dmi!.CreateDelegate(td);
    }

  }

}