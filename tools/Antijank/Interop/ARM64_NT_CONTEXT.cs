using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Interop {

  [PublicAPI]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [StructLayout(LayoutKind.Explicit, Size = 0x390)]
  public class ARM64_NT_CONTEXT : CONTEXT {

    [FieldOffset(0x0)]
    ulong X0;

    [FieldOffset(0x00)]
    ulong X1;

    [FieldOffset(0x08)]
    ulong X2;

    [FieldOffset(0x10)]
    ulong X3;

    [FieldOffset(0x18)]
    ulong X4;

    [FieldOffset(0x20)]
    ulong X5;

    [FieldOffset(0x28)]
    ulong X6;

    [FieldOffset(0x30)]
    ulong X7;

    [FieldOffset(0x38)]
    ulong X8;

    [FieldOffset(0x40)]
    ulong X9;

    [FieldOffset(0x48)]
    ulong X10;

    [FieldOffset(0x50)]
    ulong X11;

    [FieldOffset(0x58)]
    ulong X12;

    [FieldOffset(0x60)]
    ulong X13;

    [FieldOffset(0x68)]
    ulong X14;

    [FieldOffset(0x70)]
    ulong X15;

    [FieldOffset(0x78)]
    ulong X16;

    [FieldOffset(0x80)]
    ulong X17;

    [FieldOffset(0x88)]
    ulong X18;

    [FieldOffset(0x90)]
    ulong X19;

    [FieldOffset(0x98)]
    ulong X20;

    [FieldOffset(0xA0)]
    ulong X21;

    [FieldOffset(0xA8)]
    ulong X22;

    [FieldOffset(0xB0)]
    ulong X23;

    [FieldOffset(0xB8)]
    ulong X24;

    [FieldOffset(0xC0)]
    ulong X25;

    [FieldOffset(0xC8)]
    ulong X26;

    [FieldOffset(0xD0)]
    ulong X27;

    [FieldOffset(0xD8)]
    ulong X28;

    [FieldOffset(0xE0)]
    ulong Fp;

    [FieldOffset(0xE8)]
    ulong Lr;

    [FieldOffset(0xF0)]
    ulong Sp;

    [FieldOffset(0xF8)]
    ulong Pc;

    [FieldOffset(0x110)]
    ARM64_NT_NEON128 V0;

    [FieldOffset(0x120)]
    ARM64_NT_NEON128 V1;

    [FieldOffset(0x130)]
    ARM64_NT_NEON128 V2;

    [FieldOffset(0x140)]
    ARM64_NT_NEON128 V3;

    [FieldOffset(0x150)]
    ARM64_NT_NEON128 V4;

    [FieldOffset(0x160)]
    ARM64_NT_NEON128 V5;

    [FieldOffset(0x170)]
    ARM64_NT_NEON128 V6;

    [FieldOffset(0x180)]
    ARM64_NT_NEON128 V7;

    [FieldOffset(0x190)]
    ARM64_NT_NEON128 V8;

    [FieldOffset(0x1A0)]
    ARM64_NT_NEON128 V9;

    [FieldOffset(0x1B0)]
    ARM64_NT_NEON128 V10;

    [FieldOffset(0x1C0)]
    ARM64_NT_NEON128 V11;

    [FieldOffset(0x1D0)]
    ARM64_NT_NEON128 V12;

    [FieldOffset(0x1E0)]
    ARM64_NT_NEON128 V13;

    [FieldOffset(0x1F0)]
    ARM64_NT_NEON128 V14;

    [FieldOffset(0x200)]
    ARM64_NT_NEON128 V15;

    [FieldOffset(0x210)]
    ARM64_NT_NEON128 V16;

    [FieldOffset(0x220)]
    ARM64_NT_NEON128 V17;

    [FieldOffset(0x230)]
    ARM64_NT_NEON128 V18;

    [FieldOffset(0x240)]
    ARM64_NT_NEON128 V19;

    [FieldOffset(0x250)]
    ARM64_NT_NEON128 V20;

    [FieldOffset(0x260)]
    ARM64_NT_NEON128 V21;

    [FieldOffset(0x270)]
    ARM64_NT_NEON128 V22;

    [FieldOffset(0x280)]
    ARM64_NT_NEON128 V23;

    [FieldOffset(0x290)]
    ARM64_NT_NEON128 V24;

    [FieldOffset(0x2A0)]
    ARM64_NT_NEON128 V25;

    [FieldOffset(0x2B0)]
    ARM64_NT_NEON128 V26;

    [FieldOffset(0x2C0)]
    ARM64_NT_NEON128 V27;

    [FieldOffset(0x2D0)]
    ARM64_NT_NEON128 V28;

    [FieldOffset(0x2E0)]
    ARM64_NT_NEON128 V29;

    [FieldOffset(0x2F0)]
    ARM64_NT_NEON128 V30;

    [FieldOffset(0x300)]
    ARM64_NT_NEON128 V31;

    [FieldOffset(0x310)]
    uint Fpcr;

    [FieldOffset(0x314)]
    uint Fpsr;

    //
    // Debug registers
    //

    [FieldOffset(0x318)]
    uint Bcr0;

    [FieldOffset(0x31C)]
    uint Bcr1;

    [FieldOffset(0x320)]
    uint Bcr2;

    [FieldOffset(0x324)]
    uint Bcr3;

    [FieldOffset(0x328)]
    uint Bcr4;

    [FieldOffset(0x32C)]
    uint Bcr5;

    [FieldOffset(0x330)]
    uint Bcr6;

    [FieldOffset(0x334)]
    uint Bcr7;

    [FieldOffset(0x338)]
    ulong Bvr0;

    [FieldOffset(0x340)]
    ulong Bvr1;

    [FieldOffset(0x348)]
    ulong Bvr2;

    [FieldOffset(0x350)]
    ulong Bvr3;

    [FieldOffset(0x358)]
    ulong Bvr4;

    [FieldOffset(0x360)]
    ulong Bvr5;

    [FieldOffset(0x368)]
    ulong Bvr6;

    [FieldOffset(0x370)]
    ulong Bvr7;

    [FieldOffset(0x378)]
    uint Wcr0;

    [FieldOffset(0x37C)]
    uint Wcr1;

    [FieldOffset(0x380)]
    ulong Wvr0;

    [FieldOffset(0x388)]
    ulong Wvr1;

    /* +0x390 */

  }

}