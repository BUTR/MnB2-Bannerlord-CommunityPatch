using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using HarmonyLib;
using TaleWorlds.Library;

namespace CommunityPatch {

  internal static class MethodHelpers {

    private static readonly FieldInfo IlInstrOffsetField = typeof(Harmony).Assembly.GetType("HarmonyLib.ILInstruction")
      .GetField("offset", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    public static byte[] MakeCilSignatureSha256(this MethodBase mb) {
      var il = PatchProcessor.ReadMethodBody(mb);
      var buf = new byte[32];
      using var hasher = SHA256.Create();
      hasher.Initialize();

      unsafe void ProcessUnsigned(ulong v, int size) {
        fixed (byte* pBuf = buf)
          *(ulong*) pBuf = v;
        hasher.TransformBlock(buf, 0, size, null, 0);
      }

      void ProcessSigned(long v, int size)
        => ProcessUnsigned((ulong) v, size);

      void ProcessString(string s) {
        using var sigHasher = SHA256.Create();
        var callSig = sigHasher.ComputeHash(Encoding.Unicode.GetBytes(s));
        hasher.TransformBlock(callSig, 0, 32, null, 0);
      }

      unsafe void ProcessSingle(float v) {
        fixed (byte* pBuf = buf)
          *(float*) pBuf = v;
        hasher.TransformBlock(buf, 0, 4, null, 0);
      }

      unsafe void ProcessDouble(double v) {
        fixed (byte* pBuf = buf)
          *(double*) pBuf = v;
        hasher.TransformBlock(buf, 0, 8, null, 0);
      }

      void ProcessInstrRef(object r) {
        ProcessSigned((int) IlInstrOffsetField.GetValue(r), 4);
      }

      ProcessString(mb.FullDescription());

      foreach (var instr in il) {
        var opCode = instr.Key;
        var opCodeValue = instr.Key.Value;
        var operand = instr.Value;

        for (var i = 0; i < opCode.Size; ++i) {
          buf[i] = (byte) (opCodeValue >> (i * 8));
        }

        hasher.TransformBlock(buf, 0, opCode.Size, null, 0);

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

      return hasher.Hash;
    }

  }

}