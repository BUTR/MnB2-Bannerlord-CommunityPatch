using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Antijank {

  public abstract class MethodBodyReader : IEnumerable<(OpCode Code, object Operand)>, ICloneable {

    public static unsafe MethodBodyReader Create(byte* baseAddress, int length)
      => new UnsafeMethodBodyReader(baseAddress, length);

    public static unsafe MethodBodyReader Create(byte[] source, int length)
      => new ArrayMethodBodyReader(source, length);

    public static unsafe MethodBodyReader Create(IReadOnlyList<byte> source, int length)
      => new CopyMethodBodyReader(source, length);

    static readonly OpCode[] OneByteOpcodes;

    static readonly OpCode[] TwoBytesOpcodes;

    static MethodBodyReader() {
      OneByteOpcodes = new OpCode[256];
      TwoBytesOpcodes = new OpCode[256];

      var fields = typeof(OpCodes).GetFields(
        BindingFlags.Public | BindingFlags.Static);

      foreach (var field in fields) {
        var opcode = (OpCode) field.GetValue(null);
        if (opcode.OpCodeType == OpCodeType.Nternal)
          continue;

        if (opcode.Size == 1)
          OneByteOpcodes[opcode.Value] = opcode;
        else
          TwoBytesOpcodes[opcode.Value & 0xff] = opcode;
      }
    }

    protected MethodBodyReader(int length)
      => Length = length;

    public int Offset { get; protected set; }

    public int Length { get; }

    private unsafe OpCode ReadCode() {
      var code = Consume<byte>();
      return code != 0xfe
        ? OneByteOpcodes[code]
        : TwoBytesOpcodes[Consume<byte>()];
    }

    protected virtual unsafe T Consume<T>() where T : unmanaged {
      var v = Read<T>();
      Offset += Unsafe.SizeOf<T>();
      return v;
    }

    protected abstract T Read<T>() where T : unmanaged;

    protected abstract T Read<T>(int offset) where T : unmanaged;

    private object ReadOperand(OpCode code) {
      switch (code.OperandType) {
        case OperandType.InlineNone:
          return null;

        case OperandType.InlineSwitch:
          var length = Consume<int>();
          var branches = new int [length];
          for (var i = 0; i < length; i++)
            branches[i] = Consume<int>();
          return branches;

        case OperandType.ShortInlineBrTarget:
          return Consume<sbyte>();

        case OperandType.ShortInlineI:
          if (code == OpCodes.Ldc_I4_S)
            return Consume<sbyte>();
          else
            return Consume<byte>();

        case OperandType.ShortInlineR:
          return Consume<float>();

        case OperandType.InlineR:
          return Consume<double>();

        case OperandType.InlineI8:
          return Consume<long>();

        case OperandType.InlineI:
        case OperandType.InlineBrTarget:
        case OperandType.InlineSig:
        case OperandType.InlineString:
        case OperandType.InlineTok:
        case OperandType.InlineType:
        case OperandType.InlineMethod:
        case OperandType.InlineField:
          return Consume<int>();

        case OperandType.ShortInlineVar:
          return Consume<byte>();

        case OperandType.InlineVar:
          return Consume<ushort>();

        default:
          throw new NotSupportedException();
      }
    }

    private unsafe (OpCode Code, object Operand) ReadInstruction() {
      var code = ReadCode();
      var operand = ReadOperand(code);
      return (code, operand);
    }

    public IEnumerator<(OpCode Code, object Operand)> GetEnumerator() {
      while (Offset < Length)
        yield return ReadInstruction();

      // reset
      Offset = 0;
    }

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    public abstract MethodBodyReader Clone();

    object ICloneable.Clone()
      => Clone();

  }

  internal sealed class UnsafeMethodBodyReader : MethodBodyReader {

    public unsafe byte* BaseAddress { get; }

    public unsafe UnsafeMethodBodyReader(byte* il, int length) : base(length)
      => BaseAddress = il;

    protected override unsafe T Consume<T>() {
      var v = Unsafe.ReadUnaligned<T>(&BaseAddress[Offset]);
      Offset += Unsafe.SizeOf<T>();
      return v;
    }

    protected override unsafe T Read<T>() {
      var v = Unsafe.ReadUnaligned<T>(&BaseAddress[Offset]);
      Offset += Unsafe.SizeOf<T>();
      return v;
    }

    protected override unsafe T Read<T>(int offset) {
      var v = Unsafe.ReadUnaligned<T>(&BaseAddress[Offset + offset]);
      Offset += Unsafe.SizeOf<T>();
      return v;
    }

    public override unsafe MethodBodyReader Clone()
      => new UnsafeMethodBodyReader(BaseAddress, Length) {Offset = Offset};

  }

  internal sealed class ArrayMethodBodyReader : MethodBodyReader {

    public unsafe byte[] Source { get; }

    public unsafe ArrayMethodBodyReader(byte[] il, int length) : base(length)
      => Source = il;

    protected override unsafe T Read<T>() {
      var v = Unsafe.ReadUnaligned<T>(ref Source[Offset]);
      return v;
    }

    protected override unsafe T Read<T>(int offset) {
      var v = Unsafe.ReadUnaligned<T>(ref Source[Offset + offset]);
      return v;
    }

    public override MethodBodyReader Clone()
      => new ArrayMethodBodyReader(Source, Length) {Offset = Offset};

  }

  internal sealed class CopyMethodBodyReader : MethodBodyReader {

    public unsafe IReadOnlyList<byte> Source { get; }

    public unsafe CopyMethodBodyReader(IReadOnlyList<byte> il, int length) : base(length)
      => Source = il;

    private byte[] _buffer = new byte[8];

    protected override unsafe T Read<T>() {
      var size = Unsafe.SizeOf<T>();
      for (var i = 0; i < size; ++i)
        _buffer[i] = Source[Offset + i];
      var v = Unsafe.ReadUnaligned<T>(ref _buffer[0]);
      return v;
    }

    protected override unsafe T Read<T>(int offset) {
      var size = Unsafe.SizeOf<T>();
      var combinedOffset = Offset + offset;
      for (var i = 0; i < size; ++i)
        _buffer[i] = Source[combinedOffset + i];

      var v = Unsafe.ReadUnaligned<T>(ref _buffer[0]);
      return v;
    }

    public override MethodBodyReader Clone()
      => new CopyMethodBodyReader(Source, Length) {Offset = Offset};

  }

}