
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1




// Metadata version: v2.0.50727
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern System.Configuration
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 2:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern Smark.Core
{
  .ver 1:1:2:0
}
.assembly Beetle
{
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 16 65 6D 61 69 6C 3A 68 65 6E 72 79 66 61   // ...email:henryfa
                                                                                                    6E 40 6D 73 6E 2E 63 6F 6D 00 00 )                // n@msn.com..
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 10 77 77 77 2E 68 65 6E 72 79 66 61 6E 2E   // ...www.henryfan.
                                                                                                6E 65 74 00 00 )                                  // net..
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 06 42 65 65 74 6C 65 00 00 )                // ...Beetle..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void DotfuscatorAttribute::.ctor(string,
                                                    int32) = ( 01 00 12 30 30 30 3A 30 3A 32 3A 35 2E 30 2E 32   // ...000:0:2:5.0.2
                                                               35 30 30 2E 30 02 00 00 00 00 00 )                // 500.0......
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 06 42 65 65 74 6C 65 00 00 )                // ...Beetle..
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 61 36 39 30 66 34 33 64 2D 39 62 63 65   // ..$a690f43d-9bce
                                                                                                  2D 34 38 33 33 2D 61 61 30 33 2D 30 33 62 39 32   // -4833-aa03-03b92
                                                                                                  63 38 62 33 63 38 32 00 00 )                      // c8b3c82..
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 21 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ..!Copyright .. 
                                                                                                  68 65 6E 72 79 66 61 6E 20 20 32 30 31 31 2D 32   // henryfan  2011-2
                                                                                                  30 31 32 20 00 00 )                               // 012 ..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 32 2E 35 2E 39 2E 32 00 00 )             // ...2.5.9.2..
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 2:5:9:2
}
.module Beetle.dll
// MVID: {57ED0704-6A92-41B0-BC8E-3B8827F127D7}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03350000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi sealed beforefieldinit DotfuscatorAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 01 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .field private string a
  .field private int32 c
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(string a,
                               int32 c) cil managed
  {
    // 代码大小       21 (0x15)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0007:  ldarg.1
    IL_0008:  stfld      string DotfuscatorAttribute::a
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      int32 DotfuscatorAttribute::c
    IL_0014:  ret
  } // end of method DotfuscatorAttribute::.ctor

  .method public hidebysig instance string 
          a() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  1
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string DotfuscatorAttribute::a
    IL_0006:  ret
  } // end of method DotfuscatorAttribute::a

  .method public hidebysig instance int32 
          c() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  1
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 DotfuscatorAttribute::c
    IL_0006:  ret
  } // end of method DotfuscatorAttribute::c

  .property instance string A()
  {
    .get instance string DotfuscatorAttribute::a()
  } // end of property DotfuscatorAttribute::A
  .property instance int32 C()
  {
    .get instance int32 DotfuscatorAttribute::c()
  } // end of property DotfuscatorAttribute::C
} // end of class DotfuscatorAttribute

.class public abstract auto ansi beforefieldinit Beetle.Package
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field private class ae a
  .field public class Beetle.EventPacketRecievMessage ReceiveMessage
  .field private class Beetle.PacketRecieveMessagerArgs b
  .field public class Beetle.TcpChannel Channel
  .field assembly class l c
  .field family class Beetle.BufferWriter Writer
  .field family class Beetle.BufferReader Reader
  .field private class Beetle.BufferWriter d
  .field private bool e
  .field private class [mscorlib]System.Text.Encoding f
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       58 (0x3a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class ae ae::f()
    IL_0006:  stfld      class ae Beetle.Package::a
    IL_000b:  ldarg.0
    IL_000c:  ldnull
    IL_000d:  ldnull
    IL_000e:  newobj     instance void Beetle.PacketRecieveMessagerArgs::.ctor(class Beetle.TcpChannel,
                                                                               class Beetle.IMessage)
    IL_0013:  stfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_0018:  ldarg.0
    IL_0019:  ldsfld     class Beetle.ObjectPool`1<class Beetle.BufferReader> Beetle.TcpUtils::BufferReaderPool
    IL_001e:  callvirt   instance !0 class Beetle.ObjectPool`1<class Beetle.BufferReader>::Pop()
    IL_0023:  stfld      class Beetle.BufferReader Beetle.Package::Reader
    IL_0028:  ldarg.0
    IL_0029:  call       instance void [mscorlib]System.Object::.ctor()
    IL_002e:  ldarg.0
    IL_002f:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0034:  call       instance void Beetle.Package::set_Coding(class [mscorlib]System.Text.Encoding)
    IL_0039:  ret
  } // end of method Package::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class Beetle.TcpChannel channel) cil managed
  {
    // 代码大小       159 (0x9f)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  call       class ae ae::f()
    IL_0006:  stfld      class ae Beetle.Package::a
    IL_000b:  ldarg.0
    IL_000c:  ldnull
    IL_000d:  ldnull
    IL_000e:  newobj     instance void Beetle.PacketRecieveMessagerArgs::.ctor(class Beetle.TcpChannel,
                                                                               class Beetle.IMessage)
    IL_0013:  stfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_0018:  ldarg.0
    IL_0019:  ldsfld     class Beetle.ObjectPool`1<class Beetle.BufferReader> Beetle.TcpUtils::BufferReaderPool
    IL_001e:  callvirt   instance !0 class Beetle.ObjectPool`1<class Beetle.BufferReader>::Pop()
    IL_0023:  stfld      class Beetle.BufferReader Beetle.Package::Reader
    IL_0028:  ldarg.0
    IL_0029:  call       instance void [mscorlib]System.Object::.ctor()
    IL_002e:  ldarg.1
    IL_002f:  ldarg.0
    IL_0030:  stfld      class Beetle.Package Beetle.TcpChannel::h
    IL_0035:  ldarg.0
    IL_0036:  ldarg.1
    IL_0037:  stfld      class Beetle.TcpChannel Beetle.Package::Channel
    IL_003c:  ldarg.0
    IL_003d:  ldarg.1
    IL_003e:  callvirt   instance class [mscorlib]System.Text.Encoding Beetle.TcpChannel::get_Coding()
    IL_0043:  call       instance void Beetle.Package::set_Coding(class [mscorlib]System.Text.Encoding)
    IL_0048:  ldarg.0
    IL_0049:  ldarg.1
    IL_004a:  ldfld      class l Beetle.TcpChannel::m
    IL_004f:  stfld      class l Beetle.Package::c
    IL_0054:  ldarg.1
    IL_0055:  ldarg.0
    IL_0056:  ldftn      instance void Beetle.Package::ImportReceiveData(object,
                                                                         class Beetle.ChannelReceiveEventArgs)
    IL_005c:  newobj     instance void Beetle.EventDataReceive::.ctor(object,
                                                                      native int)
    IL_0061:  stfld      class Beetle.EventDataReceive Beetle.TcpChannel::DataReceive
    IL_0066:  ldarg.1
    IL_0067:  ldarg.0
    IL_0068:  dup
    IL_0069:  ldvirtftn  instance void Beetle.Package::MessageWrite(class Beetle.IMessage,
                                                                    class Beetle.BufferWriter)
    IL_006f:  newobj     instance void Beetle.EventMessageWriter::.ctor(object,
                                                                        native int)
    IL_0074:  stfld      class Beetle.EventMessageWriter Beetle.TcpChannel::MessageWriting
    IL_0079:  ldarg.1
    IL_007a:  ldarg.0
    IL_007b:  dup
    IL_007c:  ldvirtftn  instance void Beetle.Package::MessageWrited(class Beetle.IMessage,
                                                                     class Beetle.BufferWriter)
    IL_0082:  newobj     instance void Beetle.EventMessageWriter::.ctor(object,
                                                                        native int)
    IL_0087:  stfld      class Beetle.EventMessageWriter Beetle.TcpChannel::MessageWrited
    IL_008c:  ldarg.0
    IL_008d:  ldfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_0092:  ldarg.1
    IL_0093:  stfld      class Beetle.TcpChannel Beetle.PacketRecieveMessagerArgs::Channel
    IL_0098:  ldarg.0
    IL_0099:  call       instance void Beetle.Package::CreateWriterReader()
    IL_009e:  ret
  } // end of method Package::.ctor

  .method public hidebysig newslot virtual 
          instance object  WriteCast(object message) cil managed
  {
    // 代码大小       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method Package::WriteCast

  .method public hidebysig newslot virtual 
          instance object  ReadCast(object message) cil managed
  {
    // 代码大小       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method Package::ReadCast

  .method public hidebysig instance void 
          GetPools() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class l l::b()
    IL_0006:  stfld      class l Beetle.Package::c
    IL_000b:  ret
  } // end of method Package::GetPools

  .method public hidebysig specialname instance class [mscorlib]System.Text.Encoding 
          get_Coding() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Text.Encoding Beetle.Package::f
    IL_0006:  ret
  } // end of method Package::get_Coding

  .method public hidebysig specialname instance void 
          set_Coding(class [mscorlib]System.Text.Encoding 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [mscorlib]System.Text.Encoding Beetle.Package::f
    IL_0007:  ret
  } // end of method Package::set_Coding

  .method public hidebysig newslot abstract virtual 
          instance void  MessageWrite(class Beetle.IMessage msg,
                                      class Beetle.BufferWriter writer) cil managed
  {
  } // end of method Package::MessageWrite

  .method public hidebysig newslot virtual 
          instance void  MessageWrited(class Beetle.IMessage msg,
                                       class Beetle.BufferWriter writer) cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Package::MessageWrited

  .method public hidebysig newslot abstract virtual 
          instance class Beetle.IMessage 
          MessageRead(class Beetle.BufferReader reader) cil managed
  {
  } // end of method Package::MessageRead

  .method public hidebysig instance void 
          CreateWriterReader() cil managed
  {
    // 代码大小       153 (0x99)
    .maxstack  5
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance class [mscorlib]System.Text.Encoding Beetle.Package::get_Coding()
    IL_0007:  ldarg.0
    IL_0008:  ldfld      class l Beetle.Package::c
    IL_000d:  ldc.i4.0
    IL_000e:  newobj     instance void Beetle.BufferWriter::.ctor(class [mscorlib]System.Text.Encoding,
                                                                  class l,
                                                                  bool)
    IL_0013:  stfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0018:  ldarg.0
    IL_0019:  ldarg.0
    IL_001a:  call       instance class [mscorlib]System.Text.Encoding Beetle.Package::get_Coding()
    IL_001f:  ldarg.0
    IL_0020:  ldfld      class l Beetle.Package::c
    IL_0025:  ldc.i4.0
    IL_0026:  newobj     instance void Beetle.BufferWriter::.ctor(class [mscorlib]System.Text.Encoding,
                                                                  class l,
                                                                  bool)
    IL_002b:  stfld      class Beetle.BufferWriter Beetle.Package::d
    IL_0030:  ldarg.0
    IL_0031:  ldfld      class Beetle.TcpChannel Beetle.Package::Channel
    IL_0036:  brfalse.s  IL_0098

    IL_0038:  ldarg.0
    IL_0039:  ldfld      class Beetle.BufferWriter Beetle.Package::d
    IL_003e:  ldarg.0
    IL_003f:  ldfld      class Beetle.TcpChannel Beetle.Package::Channel
    IL_0044:  ldfld      bool Beetle.TcpChannel::EnabledVariant
    IL_0049:  stfld      bool Beetle.BufferWriter::EnabledVariant
    IL_004e:  ldarg.0
    IL_004f:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0054:  ldarg.0
    IL_0055:  ldfld      class Beetle.TcpChannel Beetle.Package::Channel
    IL_005a:  ldfld      bool Beetle.TcpChannel::EnabledVariant
    IL_005f:  stfld      bool Beetle.BufferWriter::EnabledVariant
    IL_0064:  ldarg.0
    IL_0065:  ldfld      class Beetle.BufferReader Beetle.Package::Reader
    IL_006a:  ldarg.0
    IL_006b:  ldfld      class Beetle.TcpChannel Beetle.Package::Channel
    IL_0070:  ldfld      bool Beetle.TcpChannel::EnabledVariant
    IL_0075:  stfld      bool Beetle.BufferReader::EnabledVariant
    IL_007a:  ldarg.0
    IL_007b:  ldfld      class ae Beetle.Package::a
    IL_0080:  ldarg.0
    IL_0081:  ldfld      class Beetle.TcpChannel Beetle.Package::Channel
    IL_0086:  ldfld      int32 Beetle.TcpChannel::d
    IL_008b:  callvirt   instance bool ae::f(int32)
    IL_0090:  brtrue.s   IL_0098

    IL_0092:  ldarg.0
    IL_0093:  call       instance void Beetle.Package::a()
    IL_0098:  ret
  } // end of method Package::CreateWriterReader

  .method public hidebysig instance void 
          MessageToArraySegment(class Beetle.IMessage msg,
                                class Beetle.ByteArraySegment data) cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.BufferWriter Beetle.Package::d
    IL_0006:  callvirt   instance void Beetle.BufferWriter::Reset()
    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.BufferWriter Beetle.Package::d
    IL_0013:  callvirt   instance void Beetle.Package::MessageWrite(class Beetle.IMessage,
                                                                    class Beetle.BufferWriter)
    IL_0018:  ldarg.2
    IL_0019:  ldarg.0
    IL_001a:  ldfld      class Beetle.BufferWriter Beetle.Package::d
    IL_001f:  callvirt   instance void Beetle.ByteArraySegment::Import(class Beetle.BufferWriter)
    IL_0024:  ret
  } // end of method Package::MessageToArraySegment

  .method assembly hidebysig instance void 
          a() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.TcpChannel Beetle.Package::Channel
    IL_0006:  callvirt   instance void Beetle.TcpChannel::Dispose()
    IL_000b:  ret
  } // end of method Package::a

  .method public hidebysig instance void 
          ImportReceiveData(object sender,
                            class Beetle.ChannelReceiveEventArgs e) cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.2
    IL_0002:  callvirt   instance class Beetle.ByteArraySegment Beetle.ChannelReceiveEventArgs::get_Data()
    IL_0007:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_000c:  ldc.i4.0
    IL_000d:  ldarg.2
    IL_000e:  callvirt   instance class Beetle.ByteArraySegment Beetle.ChannelReceiveEventArgs::get_Data()
    IL_0013:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0018:  callvirt   instance void Beetle.Package::Import(uint8[],
                                                              int32,
                                                              int32)
    IL_001d:  ret
  } // end of method Package::ImportReceiveData

  .method public hidebysig newslot abstract virtual 
          instance void  Import(uint8[] data,
                                int32 start,
                                int32 count) cil managed
  {
  } // end of method Package::Import

  .method public hidebysig static int32  ByteIndexOf(uint8[] searched,
                                                     uint8[] find) cil managed
  {
    // 代码大小       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  call       int32 Beetle.Package::ByteIndexOf(uint8[],
                                                           uint8[],
                                                           int32)
    IL_0008:  ret
  } // end of method Package::ByteIndexOf

  .method public hidebysig static int32  ByteIndexOf(uint8[] searched,
                                                     uint8[] find,
                                                     int32 start) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.0
    IL_0004:  ldlen
    IL_0005:  conv.i4
    IL_0006:  ldarg.2
    IL_0007:  sub
    IL_0008:  call       int32 Beetle.Package::ByteIndexOf(uint8[],
                                                           uint8[],
                                                           int32,
                                                           int32)
    IL_000d:  ret
  } // end of method Package::ByteIndexOf

  .method public hidebysig static int32  ByteIndexOf(uint8[] searched,
                                                     uint8[] find,
                                                     int32 start,
                                                     int32 count) cil managed
  {
    // 代码大小       148 (0x94)
    .maxstack  4
    .locals init (bool V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  ldlen
    IL_0004:  conv.i4
    IL_0005:  ldc.i4.1
    IL_0006:  sub
    IL_0007:  stloc.1
    IL_0008:  ldc.i4.0
    IL_0009:  stloc.2
    IL_000a:  ldarg.2
    IL_000b:  ldarg.3
    IL_000c:  add
    IL_000d:  ldarg.1
    IL_000e:  ldlen
    IL_000f:  conv.i4
    IL_0010:  sub
    IL_0011:  stloc.3
    IL_0012:  ldarg.2
    IL_0013:  stloc.s    V_4
    IL_0015:  br.s       IL_008d

    IL_0017:  ldc.i4.0
    IL_0018:  stloc.2
    IL_0019:  ldc.i4.1
    IL_001a:  stloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldc.i4.0
    IL_001d:  ldelem.u1
    IL_001e:  ldarg.0
    IL_001f:  ldloc.s    V_4
    IL_0021:  ldelem.u1
    IL_0022:  bne.un.s   IL_0087

    IL_0024:  ldarg.1
    IL_0025:  ldloc.1
    IL_0026:  ldelem.u1
    IL_0027:  ldarg.0
    IL_0028:  ldloc.s    V_4
    IL_002a:  ldloc.1
    IL_002b:  add
    IL_002c:  ldelem.u1
    IL_002d:  bne.un.s   IL_0087

    IL_002f:  ldloc.2
    IL_0030:  ldc.i4.1
    IL_0031:  add
    IL_0032:  stloc.2
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.s   10
    IL_0036:  ble.s      IL_0056

    IL_0038:  ldarg.1
    IL_0039:  ldloc.2
    IL_003a:  ldelem.u1
    IL_003b:  ldarg.0
    IL_003c:  ldloc.s    V_4
    IL_003e:  ldloc.2
    IL_003f:  add
    IL_0040:  ldelem.u1
    IL_0041:  bne.un.s   IL_0087

    IL_0043:  ldarg.1
    IL_0044:  ldloc.1
    IL_0045:  ldloc.2
    IL_0046:  sub
    IL_0047:  ldelem.u1
    IL_0048:  ldarg.0
    IL_0049:  ldloc.s    V_4
    IL_004b:  ldloc.1
    IL_004c:  add
    IL_004d:  ldloc.2
    IL_004e:  sub
    IL_004f:  ldelem.u1
    IL_0050:  bne.un.s   IL_0087

    IL_0052:  ldloc.2
    IL_0053:  ldc.i4.1
    IL_0054:  add
    IL_0055:  stloc.2
    IL_0056:  ldloc.2
    IL_0057:  stloc.s    V_5
    IL_0059:  br.s       IL_0072

    IL_005b:  ldarg.1
    IL_005c:  ldloc.s    V_5
    IL_005e:  ldelem.u1
    IL_005f:  ldarg.0
    IL_0060:  ldloc.s    V_4
    IL_0062:  ldloc.s    V_5
    IL_0064:  add
    IL_0065:  ldelem.u1
    IL_0066:  beq.s      IL_006c

    IL_0068:  ldc.i4.0
    IL_0069:  stloc.0
    IL_006a:  br.s       IL_007b

    IL_006c:  ldloc.s    V_5
    IL_006e:  ldc.i4.1
    IL_006f:  add
    IL_0070:  stloc.s    V_5
    IL_0072:  ldloc.s    V_5
    IL_0074:  ldarg.1
    IL_0075:  ldlen
    IL_0076:  conv.i4
    IL_0077:  ldloc.2
    IL_0078:  sub
    IL_0079:  blt.s      IL_005b

    IL_007b:  ldloc.0
    IL_007c:  brfalse.s  IL_0087

    IL_007e:  ldloc.s    V_4
    IL_0080:  ldarg.1
    IL_0081:  ldlen
    IL_0082:  conv.i4
    IL_0083:  add
    IL_0084:  ldc.i4.1
    IL_0085:  sub
    IL_0086:  ret

    IL_0087:  ldloc.s    V_4
    IL_0089:  ldc.i4.1
    IL_008a:  add
    IL_008b:  stloc.s    V_4
    IL_008d:  ldloc.s    V_4
    IL_008f:  ldloc.3
    IL_0090:  ble.s      IL_0017

    IL_0092:  ldc.i4.m1
    IL_0093:  ret
  } // end of method Package::ByteIndexOf

  .method family hidebysig newslot virtual 
          instance void  OnMessageDataReader(class Beetle.BufferWriter writer) cil managed
  {
    // 代码大小       106 (0x6a)
    .maxstack  3
    .locals init (class Beetle.IMessage V_0,
             class [mscorlib]System.Exception V_1,
             object V_2)
    IL_0000:  ldsfld     object Beetle.TcpUtils::j
    IL_0005:  dup
    IL_0006:  stloc.2
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldsfld     int64 Beetle.TcpUtils::ReceiveMessages
      IL_0011:  ldc.i4.1
      IL_0012:  conv.i8
      IL_0013:  add
      IL_0014:  stsfld     int64 Beetle.TcpUtils::ReceiveMessages
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.2
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ldnull
    IL_0023:  stloc.0
    IL_0024:  ldarg.0
    IL_0025:  ldfld      class Beetle.BufferReader Beetle.Package::Reader
    IL_002a:  ldarg.0
    IL_002b:  call       instance class [mscorlib]System.Text.Encoding Beetle.Package::get_Coding()
    IL_0030:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    .try
    {
      .try
      {
        IL_0035:  ldarg.0
        IL_0036:  ldfld      class Beetle.BufferReader Beetle.Package::Reader
        IL_003b:  ldarg.1
        IL_003c:  callvirt   instance void Beetle.BufferReader::Import(class Beetle.BufferWriter)
        IL_0041:  ldarg.0
        IL_0042:  ldarg.0
        IL_0043:  ldfld      class Beetle.BufferReader Beetle.Package::Reader
        IL_0048:  callvirt   instance class Beetle.IMessage Beetle.Package::MessageRead(class Beetle.BufferReader)
        IL_004d:  stloc.0
        IL_004e:  leave.s    IL_0053

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0050:  stloc.1
        IL_0051:  ldloc.1
        IL_0052:  throw

      }  // end handler
      IL_0053:  leave.s    IL_0061

    }  // end .try
    finally
    {
      IL_0055:  ldarg.0
      IL_0056:  ldfld      class Beetle.BufferReader Beetle.Package::Reader
      IL_005b:  callvirt   instance void Beetle.BufferReader::CleanBuffer()
      IL_0060:  endfinally
    }  // end handler
    IL_0061:  ldarg.0
    IL_0062:  ldloc.0
    IL_0063:  ldnull
    IL_0064:  call       instance void Beetle.Package::a(class Beetle.IMessage,
                                                         object)
    IL_0069:  ret
  } // end of method Package::OnMessageDataReader

  .method family hidebysig newslot virtual 
          instance void  OnReceiveMessage(class Beetle.PacketRecieveMessagerArgs e) cil managed
  {
    // 代码大小       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventPacketRecievMessage Beetle.Package::ReceiveMessage
    IL_0006:  brfalse.s  IL_0014

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class Beetle.EventPacketRecievMessage Beetle.Package::ReceiveMessage
    IL_000e:  ldarg.1
    IL_000f:  callvirt   instance void Beetle.EventPacketRecievMessage::Invoke(class Beetle.PacketRecieveMessagerArgs)
    IL_0014:  ret
  } // end of method Package::OnReceiveMessage

  .method assembly hidebysig instance void 
          a(class Beetle.IMessage A_0,
            object A_1) cil managed
  {
    // 代码大小       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_0006:  ldc.i4.0
    IL_0007:  stfld      bool Beetle.PacketRecieveMessagerArgs::CleanMessage
    IL_000c:  ldarg.0
    IL_000d:  ldfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_0012:  ldarg.0
    IL_0013:  ldarg.1
    IL_0014:  callvirt   instance object Beetle.Package::ReadCast(object)
    IL_0019:  stfld      object Beetle.PacketRecieveMessagerArgs::Message
    IL_001e:  ldarg.0
    IL_001f:  ldarg.0
    IL_0020:  ldfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_0025:  callvirt   instance void Beetle.Package::OnReceiveMessage(class Beetle.PacketRecieveMessagerArgs)
    IL_002a:  ret
  } // end of method Package::a

  .method family hidebysig newslot virtual 
          instance void  OnDisposed() cil managed
  {
    // 代码大小       133 (0x85)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0006:  brfalse.s  IL_0013

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_000e:  callvirt   instance void Beetle.BufferWriter::Dispose()
    IL_0013:  ldarg.0
    IL_0014:  ldfld      class Beetle.BufferWriter Beetle.Package::d
    IL_0019:  brfalse.s  IL_0026

    IL_001b:  ldarg.0
    IL_001c:  ldfld      class Beetle.BufferWriter Beetle.Package::d
    IL_0021:  callvirt   instance void Beetle.BufferWriter::Dispose()
    IL_0026:  ldarg.0
    IL_0027:  ldfld      class Beetle.BufferReader Beetle.Package::Reader
    IL_002c:  brfalse.s  IL_0050

    IL_002e:  ldarg.0
    IL_002f:  ldfld      class Beetle.BufferReader Beetle.Package::Reader
    IL_0034:  callvirt   instance void Beetle.BufferReader::CleanBuffer()
    IL_0039:  ldsfld     class Beetle.ObjectPool`1<class Beetle.BufferReader> Beetle.TcpUtils::BufferReaderPool
    IL_003e:  ldarg.0
    IL_003f:  ldfld      class Beetle.BufferReader Beetle.Package::Reader
    IL_0044:  callvirt   instance void class Beetle.ObjectPool`1<class Beetle.BufferReader>::Push(!0)
    IL_0049:  ldarg.0
    IL_004a:  ldnull
    IL_004b:  stfld      class Beetle.BufferReader Beetle.Package::Reader
    IL_0050:  ldarg.0
    IL_0051:  ldnull
    IL_0052:  stfld      class l Beetle.Package::c
    IL_0057:  ldarg.0
    IL_0058:  ldfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_005d:  ldnull
    IL_005e:  stfld      class Beetle.TcpChannel Beetle.PacketRecieveMessagerArgs::Channel
    IL_0063:  ldarg.0
    IL_0064:  ldfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_0069:  ldnull
    IL_006a:  stfld      object Beetle.PacketRecieveMessagerArgs::Message
    IL_006f:  ldarg.0
    IL_0070:  ldnull
    IL_0071:  stfld      class Beetle.PacketRecieveMessagerArgs Beetle.Package::b
    IL_0076:  ldarg.0
    IL_0077:  ldnull
    IL_0078:  stfld      class Beetle.EventPacketRecievMessage Beetle.Package::ReceiveMessage
    IL_007d:  ldarg.0
    IL_007e:  ldnull
    IL_007f:  stfld      class Beetle.TcpChannel Beetle.Package::Channel
    IL_0084:  ret
  } // end of method Package::OnDisposed

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       39 (0x27)
    .maxstack  2
    .locals init (class Beetle.Package V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool Beetle.Package::e
      IL_000e:  brtrue.s   IL_001d

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.1
      IL_0012:  stfld      bool Beetle.Package::e
      IL_0017:  ldarg.0
      IL_0018:  callvirt   instance void Beetle.Package::OnDisposed()
      IL_001d:  leave.s    IL_0026

    }  // end .try
    finally
    {
      IL_001f:  ldloc.0
      IL_0020:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0025:  endfinally
    }  // end handler
    IL_0026:  ret
  } // end of method Package::Dispose

  .property instance class [mscorlib]System.Text.Encoding
          Coding()
  {
    .get instance class [mscorlib]System.Text.Encoding Beetle.Package::get_Coding()
    .set instance void Beetle.Package::set_Coding(class [mscorlib]System.Text.Encoding)
  } // end of property Package::Coding
} // end of class Beetle.Package

.class public abstract auto ansi beforefieldinit Beetle.Task
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field private class [mscorlib]System.Threading.Timer a
  .field private int32 b
  .field private int32 c
  .field private class [mscorlib]System.Exception d
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private bool e
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.0
    IL_0008:  call       instance void Beetle.Task::a(bool)
    IL_000d:  ret
  } // end of method Task::.ctor

  .method private hidebysig instance void 
          b() cil managed
  {
    // 代码大小       31 (0x1f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Threading.Timer Beetle.Task::a
    IL_0006:  brfalse.s  IL_001e

    IL_0008:  ldarg.0
    IL_0009:  call       instance bool Beetle.Task::get_IsDisposed()
    IL_000e:  brtrue.s   IL_001e

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class [mscorlib]System.Threading.Timer Beetle.Task::a
    IL_0016:  ldc.i4.m1
    IL_0017:  ldc.i4.m1
    IL_0018:  callvirt   instance bool [mscorlib]System.Threading.Timer::Change(int32,
                                                                                int32)
    IL_001d:  pop
    IL_001e:  ret
  } // end of method Task::b

  .method private hidebysig instance void 
          a() cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Threading.Timer Beetle.Task::a
    IL_0006:  brfalse.s  IL_0028

    IL_0008:  ldarg.0
    IL_0009:  call       instance bool Beetle.Task::get_IsDisposed()
    IL_000e:  brtrue.s   IL_0028

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class [mscorlib]System.Threading.Timer Beetle.Task::a
    IL_0016:  ldarg.0
    IL_0017:  ldfld      int32 Beetle.Task::b
    IL_001c:  ldarg.0
    IL_001d:  ldfld      int32 Beetle.Task::c
    IL_0022:  callvirt   instance bool [mscorlib]System.Threading.Timer::Change(int32,
                                                                                int32)
    IL_0027:  pop
    IL_0028:  ret
  } // end of method Task::a

  .method public hidebysig instance void 
          Start(int32 dueTime,
                int32 period) cil managed
  {
    // 代码大小       66 (0x42)
    .maxstack  6
    .locals init (class Beetle.Task V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  call       instance bool Beetle.Task::get_IsDisposed()
      IL_000e:  brtrue.s   IL_0038

      IL_0010:  ldarg.0
      IL_0011:  ldarg.1
      IL_0012:  stfld      int32 Beetle.Task::b
      IL_0017:  ldarg.0
      IL_0018:  ldarg.2
      IL_0019:  stfld      int32 Beetle.Task::c
      IL_001e:  ldarg.0
      IL_001f:  ldarg.0
      IL_0020:  ldftn      instance void Beetle.Task::a(object)
      IL_0026:  newobj     instance void [mscorlib]System.Threading.TimerCallback::.ctor(object,
                                                                                         native int)
      IL_002b:  ldnull
      IL_002c:  ldarg.1
      IL_002d:  ldarg.2
      IL_002e:  newobj     instance void [mscorlib]System.Threading.Timer::.ctor(class [mscorlib]System.Threading.TimerCallback,
                                                                                 object,
                                                                                 int32,
                                                                                 int32)
      IL_0033:  stfld      class [mscorlib]System.Threading.Timer Beetle.Task::a
      IL_0038:  leave.s    IL_0041

    }  // end .try
    finally
    {
      IL_003a:  ldloc.0
      IL_003b:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0040:  endfinally
    }  // end handler
    IL_0041:  ret
  } // end of method Task::Start

  .method public hidebysig specialname instance class [mscorlib]System.Exception 
          get_Error() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Exception Beetle.Task::d
    IL_0006:  ret
  } // end of method Task::get_Error

  .method assembly hidebysig specialname 
          instance void  a(class [mscorlib]System.Exception A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [mscorlib]System.Exception Beetle.Task::d
    IL_0007:  ret
  } // end of method Task::a

  .method private hidebysig instance void 
          a(object A_0) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  2
    .locals init (class [mscorlib]System.Exception V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Beetle.Task::b()
    IL_0006:  ldarg.0
    IL_0007:  ldnull
    IL_0008:  call       instance void Beetle.Task::a(class [mscorlib]System.Exception)
    .try
    {
      .try
      {
        IL_000d:  ldarg.0
        IL_000e:  callvirt   instance void Beetle.Task::Execute()
        IL_0013:  leave.s    IL_001f

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0015:  stloc.0
        IL_0016:  ldarg.0
        IL_0017:  ldloc.0
        IL_0018:  call       instance void Beetle.Task::a(class [mscorlib]System.Exception)
        IL_001d:  leave.s    IL_001f

      }  // end handler
      IL_001f:  leave.s    IL_0028

    }  // end .try
    finally
    {
      IL_0021:  ldarg.0
      IL_0022:  call       instance void Beetle.Task::a()
      IL_0027:  endfinally
    }  // end handler
    IL_0028:  ret
  } // end of method Task::a

  .method family hidebysig newslot abstract virtual 
          instance void  Execute() cil managed
  {
  } // end of method Task::Execute

  .method public hidebysig specialname instance bool 
          get_IsDisposed() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.Task::e
    IL_0006:  ret
  } // end of method Task::get_IsDisposed

  .method private hidebysig specialname instance void 
          a(bool A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      bool Beetle.Task::e
    IL_0007:  ret
  } // end of method Task::a

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       52 (0x34)
    .maxstack  2
    .locals init (class Beetle.Task V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  call       instance bool Beetle.Task::get_IsDisposed()
      IL_000e:  brtrue.s   IL_0017

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.1
      IL_0012:  call       instance void Beetle.Task::a(bool)
      IL_0017:  ldarg.0
      IL_0018:  ldfld      class [mscorlib]System.Threading.Timer Beetle.Task::a
      IL_001d:  brfalse.s  IL_002a

      IL_001f:  ldarg.0
      IL_0020:  ldfld      class [mscorlib]System.Threading.Timer Beetle.Task::a
      IL_0025:  callvirt   instance void [mscorlib]System.Threading.Timer::Dispose()
      IL_002a:  leave.s    IL_0033

    }  // end .try
    finally
    {
      IL_002c:  ldloc.0
      IL_002d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0032:  endfinally
    }  // end handler
    IL_0033:  ret
  } // end of method Task::Dispose

  .property instance class [mscorlib]System.Exception
          Error()
  {
    .get instance class [mscorlib]System.Exception Beetle.Task::get_Error()
    .set instance void Beetle.Task::a(class [mscorlib]System.Exception)
  } // end of property Task::Error
  .property instance bool IsDisposed()
  {
    .get instance bool Beetle.Task::get_IsDisposed()
    .set instance void Beetle.Task::a(bool)
  } // end of property Task::IsDisposed
} // end of class Beetle.Task

.class public auto ansi beforefieldinit Beetle.ChannelDisposedQueue
       extends Beetle.Task
{
  .field private class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> a
  .field private static class Beetle.ChannelDisposedQueue b
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4     0x400
    IL_0006:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs>::.ctor(int32)
    IL_000b:  stfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
    IL_0010:  ldarg.0
    IL_0011:  call       instance void Beetle.Task::.ctor()
    IL_0016:  ret
  } // end of method ChannelDisposedQueue::.ctor

  .method public hidebysig instance void 
          Add(class Beetle.ChannelDisposedEventArgs channel) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs>::Enqueue(!0)
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method ChannelDisposedQueue::Add

  .method private hidebysig instance void 
          b() cil managed
  {
    // 代码大小       49 (0x31)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0027

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
      IL_0021:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs>::Dequeue()
      IL_0026:  pop
      IL_0027:  leave.s    IL_0030

    }  // end .try
    finally
    {
      IL_0029:  ldloc.0
      IL_002a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_002f:  endfinally
    }  // end handler
    IL_0030:  ret
  } // end of method ChannelDisposedQueue::b

  .method private hidebysig instance class Beetle.ChannelDisposedEventArgs 
          a() cil managed
  {
    // 代码大小       54 (0x36)
    .maxstack  2
    .locals init (class Beetle.ChannelDisposedEventArgs V_0,
             class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0029

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs> Beetle.ChannelDisposedQueue::a
      IL_0021:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class Beetle.ChannelDisposedEventArgs>::Peek()
      IL_0026:  stloc.0
      IL_0027:  leave.s    IL_0034

      IL_0029:  ldnull
      IL_002a:  stloc.0
      IL_002b:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_002d:  ldloc.1
      IL_002e:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ldloc.0
    IL_0035:  ret
  } // end of method ChannelDisposedQueue::a

  .method public hidebysig specialname static 
          class Beetle.ChannelDisposedQueue 
          get_DisposedQueue() cil managed
  {
    // 代码大小       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class Beetle.ChannelDisposedQueue Beetle.ChannelDisposedQueue::b
    IL_0005:  ret
  } // end of method ChannelDisposedQueue::get_DisposedQueue

  .method family hidebysig virtual instance void 
          Execute() cil managed
  {
    // 代码大小       64 (0x40)
    .maxstack  2
    .locals init (class Beetle.ChannelDisposedEventArgs V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class Beetle.ChannelDisposedEventArgs Beetle.ChannelDisposedQueue::a()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_003c

    IL_0009:  call       int32 [mscorlib]System.Environment::get_TickCount()
    IL_000e:  ldloc.0
    IL_000f:  ldfld      int32 Beetle.ChannelDisposedEventArgs::a
    IL_0014:  sub
    IL_0015:  ldloc.0
    IL_0016:  callvirt   instance int32 Beetle.ChannelDisposedEventArgs::get_Delay()
    IL_001b:  ble.s      IL_003f

    IL_001d:  ldloc.0
    IL_001e:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_0023:  callvirt   instance void Beetle.TcpChannel::b()
    IL_0028:  ldloc.0
    IL_0029:  ldnull
    IL_002a:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_002f:  ldarg.0
    IL_0030:  call       instance void Beetle.ChannelDisposedQueue::b()
    IL_0035:  ldarg.0
    IL_0036:  call       instance class Beetle.ChannelDisposedEventArgs Beetle.ChannelDisposedQueue::a()
    IL_003b:  stloc.0
    IL_003c:  ldloc.0
    IL_003d:  brtrue.s   IL_0009

    IL_003f:  ret
  } // end of method ChannelDisposedQueue::Execute

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  newobj     instance void Beetle.ChannelDisposedQueue::.ctor()
    IL_0005:  stsfld     class Beetle.ChannelDisposedQueue Beetle.ChannelDisposedQueue::b
    IL_000a:  ret
  } // end of method ChannelDisposedQueue::.cctor

  .property class Beetle.ChannelDisposedQueue
          DisposedQueue()
  {
    .get class Beetle.ChannelDisposedQueue Beetle.ChannelDisposedQueue::get_DisposedQueue()
  } // end of property ChannelDisposedQueue::DisposedQueue
} // end of class Beetle.ChannelDisposedQueue

.class public auto ansi beforefieldinit Beetle.ConfigSelection
       extends [System.Configuration]System.Configuration.ConfigurationSection
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Configuration]System.Configuration.ConfigurationSection::.ctor()
    IL_0006:  ret
  } // end of method ConfigSelection::.ctor

  .method public hidebysig specialname instance int32 
          get_Connections() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "Connections"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_Connections

  .method public hidebysig specialname instance void 
          set_Connections(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "Connections"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_Connections

  .method public hidebysig specialname instance int32 
          get_Pools() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "Pools"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_Pools

  .method public hidebysig specialname instance void 
          set_Pools(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "Pools"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_Pools

  .method public hidebysig specialname instance int32 
          get_SocketSendThreads() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SocketSendThreads"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_SocketSendThreads

  .method public hidebysig specialname instance void 
          set_SocketSendThreads(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SocketSendThreads"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_SocketSendThreads

  .method public hidebysig specialname instance int32 
          get_SocketReceiveThreads() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SocketReceiveThreads"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_SocketReceiveThreads

  .method public hidebysig specialname instance void 
          set_SocketReceiveThreads(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SocketReceiveThreads"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_SocketReceiveThreads

  .method public hidebysig specialname instance int32 
          get_SocketThreadSleep() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SocketThreadSleep"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_SocketThreadSleep

  .method public hidebysig specialname instance void 
          set_SocketThreadSleep(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SocketThreadSleep"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_SocketThreadSleep

  .method public hidebysig specialname instance int32 
          get_WorkThreads() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "WorkThreads"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_WorkThreads

  .method public hidebysig specialname instance void 
          set_WorkThreads(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "WorkThreads"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_WorkThreads

  .method public hidebysig specialname instance int32 
          get_WorkThreadSleep() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "WorkThreadSleep"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_WorkThreadSleep

  .method public hidebysig specialname instance void 
          set_WorkThreadSleep(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "WorkThreadSleep"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_WorkThreadSleep

  .method public hidebysig specialname instance int32 
          get_SendBufferSize() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SendBufferSize"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_SendBufferSize

  .method public hidebysig specialname instance void 
          set_SendBufferSize(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "SendBufferSize"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_SendBufferSize

  .method public hidebysig specialname instance int32 
          get_ReceiveBufferSize() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "ReceiveBufferSize"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_ReceiveBufferSize

  .method public hidebysig specialname instance void 
          set_ReceiveBufferSize(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "ReceiveBufferSize"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_ReceiveBufferSize

  .method public hidebysig specialname instance int32 
          get_PackageMaxSize() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "PackageMaxSize"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_PackageMaxSize

  .method public hidebysig specialname instance void 
          set_PackageMaxSize(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "PackageMaxSize"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_PackageMaxSize

  .method public hidebysig specialname instance int32 
          get_StringEncodingSize() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "StringEncodingSize"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_StringEncodingSize

  .method public hidebysig specialname instance void 
          set_StringEncodingSize(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "StringEncodingSize"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_StringEncodingSize

  .method public hidebysig specialname instance bool 
          get_Statistics() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "Statistics"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Boolean
    IL_0010:  ret
  } // end of method ConfigSelection::get_Statistics

  .method public hidebysig specialname instance void 
          set_Statistics(bool 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "Statistics"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Boolean
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_Statistics

  .method public hidebysig specialname instance int32 
          get_ChannelMaxQueueData() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "ChannelMaxQueueData"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_ChannelMaxQueueData

  .method public hidebysig specialname instance void 
          set_ChannelMaxQueueData(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "ChannelMaxQueueData"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_ChannelMaxQueueData

  .method public hidebysig specialname instance bool 
          get_ExecutionContext() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "ExecutionContext"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Boolean
    IL_0010:  ret
  } // end of method ConfigSelection::get_ExecutionContext

  .method public hidebysig specialname instance void 
          set_ExecutionContext(bool 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "ExecutionContext"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Boolean
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_ExecutionContext

  .method public hidebysig specialname instance int32 
          get_TimeOut() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "TimeOut"
    IL_0006:  call       instance object [System.Configuration]System.Configuration.ConfigurationElement::get_Item(string)
    IL_000b:  unbox.any  [mscorlib]System.Int32
    IL_0010:  ret
  } // end of method ConfigSelection::get_TimeOut

  .method public hidebysig specialname instance void 
          set_TimeOut(int32 'value') cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "TimeOut"
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       instance void [System.Configuration]System.Configuration.ConfigurationElement::set_Item(string,
                                                                                                                 object)
    IL_0011:  ret
  } // end of method ConfigSelection::set_TimeOut

  .property instance int32 Connections()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 0B 43 6F 6E 6E 65 63 74 69 6F 6E 73 01 00   // ...Connections..
                                                                                                                       54 51 0C 44 65 66 61 75 6C 74 56 61 6C 75 65 08   // TQ.DefaultValue.
                                                                                                                       E8 03 00 00 ) 
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 50   // ....T..MaxValueP
                                                                                                            C3 00 00 54 08 08 4D 69 6E 56 61 6C 75 65 01 00   // ...T..MinValue..
                                                                                                            00 00 ) 
    .set instance void Beetle.ConfigSelection::set_Connections(int32)
    .get instance int32 Beetle.ConfigSelection::get_Connections()
  } // end of property ConfigSelection::Connections
  .property instance int32 Pools()
  {
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 64   // ....T..MaxValued
                                                                                                            00 00 00 54 08 08 4D 69 6E 56 61 6C 75 65 01 00   // ...T..MinValue..
                                                                                                            00 00 ) 
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 05 50 6F 6F 6C 73 01 00 54 51 0C 44 65 66   // ...Pools..TQ.Def
                                                                                                                       61 75 6C 74 56 61 6C 75 65 08 01 00 00 00 )       // aultValue.....
    .set instance void Beetle.ConfigSelection::set_Pools(int32)
    .get instance int32 Beetle.ConfigSelection::get_Pools()
  } // end of property ConfigSelection::Pools
  .property instance int32 SocketSendThreads()
  {
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 64   // ....T..MaxValued
                                                                                                            00 00 00 54 08 08 4D 69 6E 56 61 6C 75 65 00 00   // ...T..MinValue..
                                                                                                            00 00 ) 
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 11 53 6F 63 6B 65 74 53 65 6E 64 54 68 72   // ...SocketSendThr
                                                                                                                       65 61 64 73 01 00 54 51 0C 44 65 66 61 75 6C 74   // eads..TQ.Default
                                                                                                                       56 61 6C 75 65 08 01 00 00 00 )                   // Value.....
    .get instance int32 Beetle.ConfigSelection::get_SocketSendThreads()
    .set instance void Beetle.ConfigSelection::set_SocketSendThreads(int32)
  } // end of property ConfigSelection::SocketSendThreads
  .property instance int32 SocketReceiveThreads()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 14 53 6F 63 6B 65 74 52 65 63 65 69 76 65   // ...SocketReceive
                                                                                                                       54 68 72 65 61 64 73 01 00 54 51 0C 44 65 66 61   // Threads..TQ.Defa
                                                                                                                       75 6C 74 56 61 6C 75 65 08 01 00 00 00 )          // ultValue.....
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 64   // ....T..MaxValued
                                                                                                            00 00 00 54 08 08 4D 69 6E 56 61 6C 75 65 00 00   // ...T..MinValue..
                                                                                                            00 00 ) 
    .get instance int32 Beetle.ConfigSelection::get_SocketReceiveThreads()
    .set instance void Beetle.ConfigSelection::set_SocketReceiveThreads(int32)
  } // end of property ConfigSelection::SocketReceiveThreads
  .property instance int32 SocketThreadSleep()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 11 53 6F 63 6B 65 74 54 68 72 65 61 64 53   // ...SocketThreadS
                                                                                                                       6C 65 65 70 01 00 54 51 0C 44 65 66 61 75 6C 74   // leep..TQ.Default
                                                                                                                       56 61 6C 75 65 08 0A 00 00 00 )                   // Value.....
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 E8   // ....T..MaxValue.
                                                                                                            03 00 00 54 08 08 4D 69 6E 56 61 6C 75 65 01 00   // ...T..MinValue..
                                                                                                            00 00 ) 
    .set instance void Beetle.ConfigSelection::set_SocketThreadSleep(int32)
    .get instance int32 Beetle.ConfigSelection::get_SocketThreadSleep()
  } // end of property ConfigSelection::SocketThreadSleep
  .property instance int32 WorkThreads()
  {
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 64   // ....T..MaxValued
                                                                                                            00 00 00 54 08 08 4D 69 6E 56 61 6C 75 65 00 00   // ...T..MinValue..
                                                                                                            00 00 ) 
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 0B 57 6F 72 6B 54 68 72 65 61 64 73 01 00   // ...WorkThreads..
                                                                                                                       54 51 0C 44 65 66 61 75 6C 74 56 61 6C 75 65 08   // TQ.DefaultValue.
                                                                                                                       01 00 00 00 ) 
    .get instance int32 Beetle.ConfigSelection::get_WorkThreads()
    .set instance void Beetle.ConfigSelection::set_WorkThreads(int32)
  } // end of property ConfigSelection::WorkThreads
  .property instance int32 WorkThreadSleep()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 0F 57 6F 72 6B 54 68 72 65 61 64 53 6C 65   // ...WorkThreadSle
                                                                                                                       65 70 01 00 54 51 0C 44 65 66 61 75 6C 74 56 61   // ep..TQ.DefaultVa
                                                                                                                       6C 75 65 08 0A 00 00 00 )                         // lue.....
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 E8   // ....T..MaxValue.
                                                                                                            03 00 00 54 08 08 4D 69 6E 56 61 6C 75 65 01 00   // ...T..MinValue..
                                                                                                            00 00 ) 
    .set instance void Beetle.ConfigSelection::set_WorkThreadSleep(int32)
    .get instance int32 Beetle.ConfigSelection::get_WorkThreadSleep()
  } // end of property ConfigSelection::WorkThreadSleep
  .property instance int32 SendBufferSize()
  {
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 80   // ....T..MaxValue.
                                                                                                            61 9C 00 54 08 08 4D 69 6E 56 61 6C 75 65 00 04   // a..T..MinValue..
                                                                                                            00 00 ) 
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 0E 53 65 6E 64 42 75 66 66 65 72 53 69 7A   // ...SendBufferSiz
                                                                                                                       65 01 00 54 51 0C 44 65 66 61 75 6C 74 56 61 6C   // e..TQ.DefaultVal
                                                                                                                       75 65 08 00 08 00 00 )                            // ue.....
    .set instance void Beetle.ConfigSelection::set_SendBufferSize(int32)
    .get instance int32 Beetle.ConfigSelection::get_SendBufferSize()
  } // end of property ConfigSelection::SendBufferSize
  .property instance int32 ReceiveBufferSize()
  {
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 80   // ....T..MaxValue.
                                                                                                            61 9C 00 54 08 08 4D 69 6E 56 61 6C 75 65 00 04   // a..T..MinValue..
                                                                                                            00 00 ) 
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 11 52 65 63 65 69 76 65 42 75 66 66 65 72   // ...ReceiveBuffer
                                                                                                                       53 69 7A 65 01 00 54 51 0C 44 65 66 61 75 6C 74   // Size..TQ.Default
                                                                                                                       56 61 6C 75 65 08 00 08 00 00 )                   // Value.....
    .get instance int32 Beetle.ConfigSelection::get_ReceiveBufferSize()
    .set instance void Beetle.ConfigSelection::set_ReceiveBufferSize(int32)
  } // end of property ConfigSelection::ReceiveBufferSize
  .property instance int32 PackageMaxSize()
  {
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 00   // ....T..MaxValue.
                                                                                                            CF 1B 06 54 08 08 4D 69 6E 56 61 6C 75 65 00 04   // ...T..MinValue..
                                                                                                            00 00 ) 
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 0E 50 61 63 6B 61 67 65 4D 61 78 53 69 7A   // ...PackageMaxSiz
                                                                                                                       65 01 00 54 51 0C 44 65 66 61 75 6C 74 56 61 6C   // e..TQ.DefaultVal
                                                                                                                       75 65 08 00 20 00 00 )                            // ue.. ..
    .get instance int32 Beetle.ConfigSelection::get_PackageMaxSize()
    .set instance void Beetle.ConfigSelection::set_PackageMaxSize(int32)
  } // end of property ConfigSelection::PackageMaxSize
  .property instance int32 StringEncodingSize()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 12 53 74 72 69 6E 67 45 6E 63 6F 64 69 6E   // ...StringEncodin
                                                                                                                       67 53 69 7A 65 01 00 54 51 0C 44 65 66 61 75 6C   // gSize..TQ.Defaul
                                                                                                                       74 56 61 6C 75 65 08 00 02 00 00 )                // tValue.....
    .custom instance void [System.Configuration]System.Configuration.IntegerValidatorAttribute::.ctor() = ( 01 00 02 00 54 08 08 4D 61 78 56 61 6C 75 65 00   // ....T..MaxValue.
                                                                                                            00 01 00 54 08 08 4D 69 6E 56 61 6C 75 65 01 00   // ...T..MinValue..
                                                                                                            00 00 ) 
    .set instance void Beetle.ConfigSelection::set_StringEncodingSize(int32)
    .get instance int32 Beetle.ConfigSelection::get_StringEncodingSize()
  } // end of property ConfigSelection::StringEncodingSize
  .property instance bool Statistics()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 0A 53 74 61 74 69 73 74 69 63 73 01 00 54   // ...Statistics..T
                                                                                                                       51 0C 44 65 66 61 75 6C 74 56 61 6C 75 65 02 00 ) // Q.DefaultValue..
    .set instance void Beetle.ConfigSelection::set_Statistics(bool)
    .get instance bool Beetle.ConfigSelection::get_Statistics()
  } // end of property ConfigSelection::Statistics
  .property instance int32 ChannelMaxQueueData()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 13 43 68 61 6E 6E 65 6C 4D 61 78 51 75 65   // ...ChannelMaxQue
                                                                                                                       75 65 44 61 74 61 01 00 54 51 0C 44 65 66 61 75   // ueData..TQ.Defau
                                                                                                                       6C 74 56 61 6C 75 65 08 00 00 00 00 )             // ltValue.....
    .set instance void Beetle.ConfigSelection::set_ChannelMaxQueueData(int32)
    .get instance int32 Beetle.ConfigSelection::get_ChannelMaxQueueData()
  } // end of property ConfigSelection::ChannelMaxQueueData
  .property instance bool ExecutionContext()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 10 45 78 65 63 75 74 69 6F 6E 43 6F 6E 74   // ...ExecutionCont
                                                                                                                       65 78 74 01 00 54 51 0C 44 65 66 61 75 6C 74 56   // ext..TQ.DefaultV
                                                                                                                       61 6C 75 65 02 00 )                               // alue..
    .get instance bool Beetle.ConfigSelection::get_ExecutionContext()
    .set instance void Beetle.ConfigSelection::set_ExecutionContext(bool)
  } // end of property ConfigSelection::ExecutionContext
  .property instance int32 TimeOut()
  {
    .custom instance void [System.Configuration]System.Configuration.ConfigurationPropertyAttribute::.ctor(string) = ( 01 00 07 54 69 6D 65 4F 75 74 01 00 54 51 0C 44   // ...TimeOut..TQ.D
                                                                                                                       65 66 61 75 6C 74 56 61 6C 75 65 08 1E 00 00 00 ) // efaultValue.....
    .get instance int32 Beetle.ConfigSelection::get_TimeOut()
    .set instance void Beetle.ConfigSelection::set_TimeOut(int32)
  } // end of property ConfigSelection::TimeOut
} // end of class Beetle.ConfigSelection

.class public auto ansi beforefieldinit Beetle.BufferWriter
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field private static literal int32 a = int32(0x00000000)
  .field assembly class [System]System.Collections.Generic.Queue`1<class b> b
  .field assembly class b c
  .field private class l d
  .field assembly int32 e
  .field private bool f
  .field assembly class Beetle.IByteConverter g
  .field private class ai h
  .field assembly class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> i
  .field public class [mscorlib]System.Text.Encoding Coding
  .field private bool j
  .field private int32 k
  .field private class n l
  .field private class Beetle.ByteArraySegment[] m
  .field private class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> n
  .field public bool EnabledVariant
  .method assembly hidebysig instance class b 
          b() cil managed
  {
    // 代码大小       57 (0x39)
    .maxstack  2
    .locals init (class b V_0,
             class Beetle.BufferWriter V_1)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.1
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_000e:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
      IL_0013:  ldc.i4.0
      IL_0014:  ble.s      IL_002c

      IL_0016:  ldarg.0
      IL_0017:  ldfld      bool Beetle.BufferWriter::j
      IL_001c:  brtrue.s   IL_002c

      IL_001e:  ldarg.0
      IL_001f:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_0024:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Peek()
      IL_0029:  stloc.0
      IL_002a:  leave.s    IL_0037

      IL_002c:  ldnull
      IL_002d:  stloc.0
      IL_002e:  leave.s    IL_0037

    }  // end .try
    finally
    {
      IL_0030:  ldloc.1
      IL_0031:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0036:  endfinally
    }  // end handler
    IL_0037:  ldloc.0
    IL_0038:  ret
  } // end of method BufferWriter::b

  .method assembly hidebysig instance class b 
          c() cil managed
  {
    // 代码大小       57 (0x39)
    .maxstack  2
    .locals init (class b V_0,
             class Beetle.BufferWriter V_1)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.1
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_000e:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
      IL_0013:  ldc.i4.0
      IL_0014:  ble.s      IL_002c

      IL_0016:  ldarg.0
      IL_0017:  ldfld      bool Beetle.BufferWriter::j
      IL_001c:  brtrue.s   IL_002c

      IL_001e:  ldarg.0
      IL_001f:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_0024:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
      IL_0029:  stloc.0
      IL_002a:  leave.s    IL_0037

      IL_002c:  ldnull
      IL_002d:  stloc.0
      IL_002e:  leave.s    IL_0037

    }  // end .try
    finally
    {
      IL_0030:  ldloc.1
      IL_0031:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0036:  endfinally
    }  // end handler
    IL_0037:  ldloc.0
    IL_0038:  ret
  } // end of method BufferWriter::c

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> 
          GetBuffers() cil managed
  {
    // 代码大小       109 (0x6d)
    .maxstack  5
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class Beetle.ByteArraySegment> V_0,
             class b V_1,
             uint8[] V_2,
             class Beetle.ByteArraySegment V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_000b:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.ByteArraySegment>::.ctor(int32)
    IL_0010:  stloc.0
    IL_0011:  br.s       IL_005d

    IL_0013:  ldarg.0
    IL_0014:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0019:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldfld      int32 b::h
    IL_0025:  newarr     [mscorlib]System.Byte
    IL_002a:  stloc.2
    IL_002b:  ldloc.1
    IL_002c:  ldfld      uint8[] b::e
    IL_0031:  ldc.i4.0
    IL_0032:  ldloc.2
    IL_0033:  ldc.i4.0
    IL_0034:  ldloc.1
    IL_0035:  ldfld      int32 b::h
    IL_003a:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_003f:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0044:  stloc.3
    IL_0045:  ldloc.3
    IL_0046:  ldloc.2
    IL_0047:  ldc.i4.0
    IL_0048:  ldloc.2
    IL_0049:  ldlen
    IL_004a:  conv.i4
    IL_004b:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                        int32,
                                                                        int32)
    IL_0050:  ldloc.0
    IL_0051:  ldloc.3
    IL_0052:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.ByteArraySegment>::Add(!0)
    IL_0057:  ldloc.1
    IL_0058:  callvirt   instance void b::Dispose()
    IL_005d:  ldarg.0
    IL_005e:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0063:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_0068:  ldc.i4.0
    IL_0069:  bgt.s      IL_0013

    IL_006b:  ldloc.0
    IL_006c:  ret
  } // end of method BufferWriter::GetBuffers

  .method public hidebysig specialname instance int32 
          get_Count() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Beetle.BufferWriter::e
    IL_0006:  ret
  } // end of method BufferWriter::get_Count

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> 
          WriteHeadSize() cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  4
    .locals init (uint8[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0006:  ldc.i4.0
    IL_0007:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int32)
    IL_000c:  stloc.0
    IL_000d:  ldarg.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.0
    IL_0010:  ldc.i4.4
    IL_0011:  call       instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::WriteReturnSegment(uint8[],
                                                                                                                                                            int32,
                                                                                                                                                            int32)
    IL_0016:  ret
  } // end of method BufferWriter::WriteHeadSize

  .method public hidebysig instance void 
          SetHeadSize(int32 size,
                      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> refdata) cil managed
  {
    // 代码大小       128 (0x80)
    .maxstack  5
    .locals init (uint8[] V_0,
             class Beetle.ByteArraySegment V_1,
             class Beetle.ByteArraySegment V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int32)
    IL_000c:  stloc.0
    IL_000d:  ldarg.2
    IL_000e:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.ByteArraySegment>::get_Count()
    IL_0013:  ldc.i4.1
    IL_0014:  bne.un.s   IL_0038

    IL_0016:  ldarg.2
    IL_0017:  ldc.i4.0
    IL_0018:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment>::get_Item(int32)
    IL_001d:  stloc.1
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.0
    IL_0020:  ldloc.1
    IL_0021:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0026:  ldloc.1
    IL_0027:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_002c:  ldloc.1
    IL_002d:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0032:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_0037:  ret

    IL_0038:  ldarg.2
    IL_0039:  ldc.i4.0
    IL_003a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment>::get_Item(int32)
    IL_003f:  stloc.1
    IL_0040:  ldarg.2
    IL_0041:  ldc.i4.1
    IL_0042:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment>::get_Item(int32)
    IL_0047:  stloc.2
    IL_0048:  ldloc.0
    IL_0049:  ldc.i4.0
    IL_004a:  ldloc.1
    IL_004b:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0050:  ldloc.1
    IL_0051:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0056:  ldloc.1
    IL_0057:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_005c:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_0061:  ldloc.0
    IL_0062:  ldloc.1
    IL_0063:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0068:  ldloc.2
    IL_0069:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_006e:  ldloc.2
    IL_006f:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0074:  ldloc.2
    IL_0075:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_007a:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_007f:  ret
  } // end of method BufferWriter::SetHeadSize

  .method private hidebysig instance void 
          a() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      class l Beetle.BufferWriter::d
    IL_0007:  callvirt   instance class b l::g()
    IL_000c:  stfld      class b Beetle.BufferWriter::c
    IL_0011:  ldarg.0
    IL_0012:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0017:  ldarg.0
    IL_0018:  ldfld      class b Beetle.BufferWriter::c
    IL_001d:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class b>::Enqueue(!0)
    IL_0022:  ret
  } // end of method BufferWriter::a

  .method assembly hidebysig instance void 
          d() cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  8
    IL_0000:  br.s       IL_0012

    IL_0002:  ldarg.0
    IL_0003:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0008:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
    IL_000d:  callvirt   instance void b::Dispose()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0018:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_001d:  ldc.i4.0
    IL_001e:  bgt.s      IL_0002

    IL_0020:  ret
  } // end of method BufferWriter::d

  .method public hidebysig instance void 
          Reset() cil managed
  {
    // 代码大小       46 (0x2e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  stfld      int32 Beetle.BufferWriter::e
    IL_0007:  br.s       IL_0019

    IL_0009:  ldarg.0
    IL_000a:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_000f:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
    IL_0014:  callvirt   instance void b::Dispose()
    IL_0019:  ldarg.0
    IL_001a:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_001f:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_0024:  ldc.i4.0
    IL_0025:  bgt.s      IL_0009

    IL_0027:  ldarg.0
    IL_0028:  call       instance void Beetle.BufferWriter::a()
    IL_002d:  ret
  } // end of method BufferWriter::Reset

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Text.Encoding coding) cil managed
  {
    // 代码大小       164 (0xa4)
    .maxstack  4
    .locals init (class Beetle.ByteArraySegment[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   100
    IL_0003:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class b>::.ctor(int32)
    IL_0008:  stfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.1
    IL_000f:  stfld      bool Beetle.BufferWriter::f
    IL_0014:  ldarg.0
    IL_0015:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_001a:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_001f:  stfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0024:  ldarg.0
    IL_0025:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_002a:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_002f:  stfld      class ai Beetle.BufferWriter::h
    IL_0034:  ldarg.0
    IL_0035:  ldc.i4.s   64
    IL_0037:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IMessage>::.ctor(int32)
    IL_003c:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
    IL_0041:  ldarg.0
    IL_0042:  newobj     instance void n::.ctor()
    IL_0047:  stfld      class n Beetle.BufferWriter::l
    IL_004c:  ldarg.0
    IL_004d:  ldc.i4.3
    IL_004e:  newarr     Beetle.ByteArraySegment
    IL_0053:  stloc.0
    IL_0054:  ldloc.0
    IL_0055:  ldc.i4.0
    IL_0056:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_005b:  stelem.ref
    IL_005c:  ldloc.0
    IL_005d:  ldc.i4.1
    IL_005e:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0063:  stelem.ref
    IL_0064:  ldloc.0
    IL_0065:  ldc.i4.2
    IL_0066:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_006b:  stelem.ref
    IL_006c:  ldloc.0
    IL_006d:  stfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0072:  ldarg.0
    IL_0073:  ldc.i4.4
    IL_0074:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.ByteArraySegment>::.ctor(int32)
    IL_0079:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_007e:  ldarg.0
    IL_007f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0084:  ldarg.0
    IL_0085:  ldarg.1
    IL_0086:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_008b:  ldarg.0
    IL_008c:  call       class l l::b()
    IL_0091:  stfld      class l Beetle.BufferWriter::d
    IL_0096:  ldarg.0
    IL_0097:  call       instance void Beetle.BufferWriter::a()
    IL_009c:  ldarg.0
    IL_009d:  ldc.i4.1
    IL_009e:  stfld      bool Beetle.BufferWriter::f
    IL_00a3:  ret
  } // end of method BufferWriter::.ctor

  .method assembly hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Text.Encoding A_0,
                               class l A_1) cil managed
  {
    // 代码大小       160 (0xa0)
    .maxstack  4
    .locals init (class Beetle.ByteArraySegment[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   100
    IL_0003:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class b>::.ctor(int32)
    IL_0008:  stfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.1
    IL_000f:  stfld      bool Beetle.BufferWriter::f
    IL_0014:  ldarg.0
    IL_0015:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_001a:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_001f:  stfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0024:  ldarg.0
    IL_0025:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_002a:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_002f:  stfld      class ai Beetle.BufferWriter::h
    IL_0034:  ldarg.0
    IL_0035:  ldc.i4.s   64
    IL_0037:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IMessage>::.ctor(int32)
    IL_003c:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
    IL_0041:  ldarg.0
    IL_0042:  newobj     instance void n::.ctor()
    IL_0047:  stfld      class n Beetle.BufferWriter::l
    IL_004c:  ldarg.0
    IL_004d:  ldc.i4.3
    IL_004e:  newarr     Beetle.ByteArraySegment
    IL_0053:  stloc.0
    IL_0054:  ldloc.0
    IL_0055:  ldc.i4.0
    IL_0056:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_005b:  stelem.ref
    IL_005c:  ldloc.0
    IL_005d:  ldc.i4.1
    IL_005e:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0063:  stelem.ref
    IL_0064:  ldloc.0
    IL_0065:  ldc.i4.2
    IL_0066:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_006b:  stelem.ref
    IL_006c:  ldloc.0
    IL_006d:  stfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0072:  ldarg.0
    IL_0073:  ldc.i4.4
    IL_0074:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.ByteArraySegment>::.ctor(int32)
    IL_0079:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_007e:  ldarg.0
    IL_007f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0084:  ldarg.0
    IL_0085:  ldarg.1
    IL_0086:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_008b:  ldarg.0
    IL_008c:  ldarg.2
    IL_008d:  stfld      class l Beetle.BufferWriter::d
    IL_0092:  ldarg.0
    IL_0093:  call       instance void Beetle.BufferWriter::a()
    IL_0098:  ldarg.0
    IL_0099:  ldc.i4.1
    IL_009a:  stfld      bool Beetle.BufferWriter::f
    IL_009f:  ret
  } // end of method BufferWriter::.ctor

  .method assembly hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Text.Encoding A_0,
                               class l A_1,
                               bool A_2) cil managed
  {
    // 代码大小       163 (0xa3)
    .maxstack  4
    .locals init (class Beetle.ByteArraySegment[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   100
    IL_0003:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class b>::.ctor(int32)
    IL_0008:  stfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.1
    IL_000f:  stfld      bool Beetle.BufferWriter::f
    IL_0014:  ldarg.0
    IL_0015:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_001a:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_001f:  stfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0024:  ldarg.0
    IL_0025:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_002a:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_002f:  stfld      class ai Beetle.BufferWriter::h
    IL_0034:  ldarg.0
    IL_0035:  ldc.i4.s   64
    IL_0037:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IMessage>::.ctor(int32)
    IL_003c:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
    IL_0041:  ldarg.0
    IL_0042:  newobj     instance void n::.ctor()
    IL_0047:  stfld      class n Beetle.BufferWriter::l
    IL_004c:  ldarg.0
    IL_004d:  ldc.i4.3
    IL_004e:  newarr     Beetle.ByteArraySegment
    IL_0053:  stloc.0
    IL_0054:  ldloc.0
    IL_0055:  ldc.i4.0
    IL_0056:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_005b:  stelem.ref
    IL_005c:  ldloc.0
    IL_005d:  ldc.i4.1
    IL_005e:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0063:  stelem.ref
    IL_0064:  ldloc.0
    IL_0065:  ldc.i4.2
    IL_0066:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_006b:  stelem.ref
    IL_006c:  ldloc.0
    IL_006d:  stfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0072:  ldarg.0
    IL_0073:  ldc.i4.4
    IL_0074:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.ByteArraySegment>::.ctor(int32)
    IL_0079:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_007e:  ldarg.0
    IL_007f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0084:  ldarg.0
    IL_0085:  ldarg.1
    IL_0086:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_008b:  ldarg.0
    IL_008c:  ldarg.2
    IL_008d:  stfld      class l Beetle.BufferWriter::d
    IL_0092:  ldarg.0
    IL_0093:  ldc.i4.1
    IL_0094:  stfld      bool Beetle.BufferWriter::f
    IL_0099:  ldarg.3
    IL_009a:  brfalse.s  IL_00a2

    IL_009c:  ldarg.0
    IL_009d:  call       instance void Beetle.BufferWriter::a()
    IL_00a2:  ret
  } // end of method BufferWriter::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Text.Encoding coding,
                               bool bufferfromPool) cil managed
  {
    // 代码大小       167 (0xa7)
    .maxstack  4
    .locals init (class Beetle.ByteArraySegment[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   100
    IL_0003:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class b>::.ctor(int32)
    IL_0008:  stfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.1
    IL_000f:  stfld      bool Beetle.BufferWriter::f
    IL_0014:  ldarg.0
    IL_0015:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_001a:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_001f:  stfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0024:  ldarg.0
    IL_0025:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_002a:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_002f:  stfld      class ai Beetle.BufferWriter::h
    IL_0034:  ldarg.0
    IL_0035:  ldc.i4.s   64
    IL_0037:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IMessage>::.ctor(int32)
    IL_003c:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
    IL_0041:  ldarg.0
    IL_0042:  newobj     instance void n::.ctor()
    IL_0047:  stfld      class n Beetle.BufferWriter::l
    IL_004c:  ldarg.0
    IL_004d:  ldc.i4.3
    IL_004e:  newarr     Beetle.ByteArraySegment
    IL_0053:  stloc.0
    IL_0054:  ldloc.0
    IL_0055:  ldc.i4.0
    IL_0056:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_005b:  stelem.ref
    IL_005c:  ldloc.0
    IL_005d:  ldc.i4.1
    IL_005e:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0063:  stelem.ref
    IL_0064:  ldloc.0
    IL_0065:  ldc.i4.2
    IL_0066:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_006b:  stelem.ref
    IL_006c:  ldloc.0
    IL_006d:  stfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0072:  ldarg.0
    IL_0073:  ldc.i4.4
    IL_0074:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.ByteArraySegment>::.ctor(int32)
    IL_0079:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_007e:  ldarg.0
    IL_007f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0084:  ldarg.0
    IL_0085:  ldarg.1
    IL_0086:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_008b:  ldarg.2
    IL_008c:  brfalse.s  IL_009f

    IL_008e:  ldarg.0
    IL_008f:  call       class l l::b()
    IL_0094:  stfld      class l Beetle.BufferWriter::d
    IL_0099:  ldarg.0
    IL_009a:  call       instance void Beetle.BufferWriter::a()
    IL_009f:  ldarg.0
    IL_00a0:  ldarg.2
    IL_00a1:  stfld      bool Beetle.BufferWriter::f
    IL_00a6:  ret
  } // end of method BufferWriter::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Text.Encoding coding,
                               bool bufferfromPool,
                               bool allotbuffer) cil managed
  {
    // 代码大小       170 (0xaa)
    .maxstack  4
    .locals init (class Beetle.ByteArraySegment[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   100
    IL_0003:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class b>::.ctor(int32)
    IL_0008:  stfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.1
    IL_000f:  stfld      bool Beetle.BufferWriter::f
    IL_0014:  ldarg.0
    IL_0015:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_001a:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_001f:  stfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0024:  ldarg.0
    IL_0025:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_002a:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_002f:  stfld      class ai Beetle.BufferWriter::h
    IL_0034:  ldarg.0
    IL_0035:  ldc.i4.s   64
    IL_0037:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IMessage>::.ctor(int32)
    IL_003c:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
    IL_0041:  ldarg.0
    IL_0042:  newobj     instance void n::.ctor()
    IL_0047:  stfld      class n Beetle.BufferWriter::l
    IL_004c:  ldarg.0
    IL_004d:  ldc.i4.3
    IL_004e:  newarr     Beetle.ByteArraySegment
    IL_0053:  stloc.0
    IL_0054:  ldloc.0
    IL_0055:  ldc.i4.0
    IL_0056:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_005b:  stelem.ref
    IL_005c:  ldloc.0
    IL_005d:  ldc.i4.1
    IL_005e:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0063:  stelem.ref
    IL_0064:  ldloc.0
    IL_0065:  ldc.i4.2
    IL_0066:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_006b:  stelem.ref
    IL_006c:  ldloc.0
    IL_006d:  stfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0072:  ldarg.0
    IL_0073:  ldc.i4.4
    IL_0074:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.ByteArraySegment>::.ctor(int32)
    IL_0079:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_007e:  ldarg.0
    IL_007f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0084:  ldarg.0
    IL_0085:  ldarg.1
    IL_0086:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_008b:  ldarg.2
    IL_008c:  brfalse.s  IL_00a2

    IL_008e:  ldarg.0
    IL_008f:  call       class l l::b()
    IL_0094:  stfld      class l Beetle.BufferWriter::d
    IL_0099:  ldarg.3
    IL_009a:  brfalse.s  IL_00a2

    IL_009c:  ldarg.0
    IL_009d:  call       instance void Beetle.BufferWriter::a()
    IL_00a2:  ldarg.0
    IL_00a3:  ldarg.2
    IL_00a4:  stfld      bool Beetle.BufferWriter::f
    IL_00a9:  ret
  } // end of method BufferWriter::.ctor

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       166 (0xa6)
    .maxstack  2
    .locals init (class Beetle.BufferWriter V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool Beetle.BufferWriter::j
      IL_000e:  brtrue     IL_009c

      IL_0013:  ldarg.0
      IL_0014:  ldc.i4.1
      IL_0015:  stfld      bool Beetle.BufferWriter::j
      IL_001a:  ldarg.0
      IL_001b:  ldfld      class ai Beetle.BufferWriter::h
      IL_0020:  brfalse.s  IL_0032

      IL_0022:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class ai Beetle.BufferWriter::h
      IL_002d:  callvirt   instance void class Beetle.ObjectPool`1<class ai>::Push(!0)
      IL_0032:  ldarg.0
      IL_0033:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
      IL_0038:  brfalse.s  IL_005c

      IL_003a:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
      IL_003f:  ldarg.0
      IL_0040:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
      IL_0045:  callvirt   instance void Beetle.IByteConverterFactory::Push(class Beetle.IByteConverter)
      IL_004a:  br.s       IL_005c

      IL_004c:  ldarg.0
      IL_004d:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_0052:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
      IL_0057:  callvirt   instance void b::Dispose()
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_0062:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
      IL_0067:  ldc.i4.0
      IL_0068:  bgt.s      IL_004c

      IL_006a:  ldarg.0
      IL_006b:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
      IL_0070:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.ByteArraySegment>::Clear()
      IL_0075:  ldarg.0
      IL_0076:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
      IL_007b:  ldc.i4.0
      IL_007c:  ldelem.ref
      IL_007d:  callvirt   instance void Beetle.ByteArraySegment::Clear()
      IL_0082:  ldarg.0
      IL_0083:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
      IL_0088:  ldc.i4.1
      IL_0089:  ldelem.ref
      IL_008a:  callvirt   instance void Beetle.ByteArraySegment::Clear()
      IL_008f:  ldarg.0
      IL_0090:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
      IL_0095:  ldc.i4.2
      IL_0096:  ldelem.ref
      IL_0097:  callvirt   instance void Beetle.ByteArraySegment::Clear()
      IL_009c:  leave.s    IL_00a5

    }  // end .try
    finally
    {
      IL_009e:  ldloc.0
      IL_009f:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_00a4:  endfinally
    }  // end handler
    IL_00a5:  ret
  } // end of method BufferWriter::Dispose

  .method public hidebysig instance void 
          Write(uint8 'value') cil managed
  {
    // 代码大小       62 (0x3e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferWriter::c
    IL_0006:  ldc.i4.1
    IL_0007:  callvirt   instance bool b::c(int32)
    IL_000c:  brfalse.s  IL_0029

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class b Beetle.BufferWriter::c
    IL_0014:  ldarg.1
    IL_0015:  callvirt   instance void b::a(uint8)
    IL_001a:  ldarg.0
    IL_001b:  dup
    IL_001c:  ldfld      int32 Beetle.BufferWriter::e
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stfld      int32 Beetle.BufferWriter::e
    IL_0028:  ret

    IL_0029:  ldarg.0
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0030:  ldarg.1
    IL_0031:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(uint8)
    IL_0036:  ldc.i4.0
    IL_0037:  ldc.i4.1
    IL_0038:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_003d:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class Beetle.ByteArraySegment bas) cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0007:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_000c:  ldarg.0
    IL_000d:  ldarg.1
    IL_000e:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0013:  ldarg.1
    IL_0014:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0019:  ldarg.1
    IL_001a:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_001f:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0024:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(valuetype [mscorlib]System.DateTime datetime) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarga.s   datetime
    IL_0003:  call       instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0008:  call       instance void Beetle.BufferWriter::Write(int64)
    IL_000d:  ret
  } // end of method BufferWriter::Write

  .method private hidebysig instance bool 
          a(class [mscorlib]System.Array A_0) cil managed
  {
    // 代码大小       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000c

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.0
    IL_0005:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_000a:  br.s       IL_0018

    IL_000c:  ldarg.0
    IL_000d:  ldarg.1
    IL_000e:  callvirt   instance int32 [mscorlib]System.Array::get_Length()
    IL_0013:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_0018:  ldarg.1
    IL_0019:  brfalse.s  IL_0025

    IL_001b:  ldarg.1
    IL_001c:  callvirt   instance int32 [mscorlib]System.Array::get_Length()
    IL_0021:  ldc.i4.0
    IL_0022:  cgt
    IL_0024:  ret

    IL_0025:  ldc.i4.0
    IL_0026:  ret
  } // end of method BufferWriter::a

  .method public hidebysig instance void 
          Write(valuetype [mscorlib]System.DateTime[] 'value') cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0029

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_0023

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelema    [mscorlib]System.DateTime
    IL_0015:  ldobj      [mscorlib]System.DateTime
    IL_001a:  call       instance void Beetle.BufferWriter::Write(valuetype [mscorlib]System.DateTime)
    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.0
    IL_0023:  ldloc.0
    IL_0024:  ldarg.1
    IL_0025:  ldlen
    IL_0026:  conv.i4
    IL_0027:  blt.s      IL_000d

    IL_0029:  ret
  } // end of method BufferWriter::Write

  .method private hidebysig instance bool 
          WriteIListLength<T>(class [mscorlib]System.Collections.Generic.IList`1<!!T> values) cil managed
  {
    // 代码大小       34 (0x22)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_000b

    IL_0003:  ldarg.1
    IL_0004:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<!!T>::get_Count()
    IL_0009:  brtrue.s   IL_0014

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.0
    IL_000d:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_0012:  ldc.i4.0
    IL_0013:  ret

    IL_0014:  ldarg.0
    IL_0015:  ldarg.1
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<!!T>::get_Count()
    IL_001b:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_0020:  ldc.i4.1
    IL_0021:  ret
  } // end of method BufferWriter::WriteIListLength

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.DateTime> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<valuetype [mscorlib]System.DateTime>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.DateTime>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(valuetype [mscorlib]System.DateTime)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<valuetype [mscorlib]System.DateTime>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(float32 'value') cil managed
  {
    // 代码大小       144 (0x90)
    .maxstack  4
    .locals init (int32 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferWriter::EnabledVariant
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  call       instance void Beetle.BufferWriter::WriteVariant(float32)
    IL_000f:  ret

    IL_0010:  ldarga.s   'value'
    IL_0012:  conv.u
    IL_0013:  ldind.i4
    IL_0014:  stloc.0
    IL_0015:  ldarg.0
    IL_0016:  ldfld      class b Beetle.BufferWriter::c
    IL_001b:  ldc.i4.4
    IL_001c:  callvirt   instance bool b::c(int32)
    IL_0021:  brfalse.s  IL_007b

    IL_0023:  ldarg.0
    IL_0024:  ldfld      class b Beetle.BufferWriter::c
    IL_0029:  ldfld      uint8[] b::e
    IL_002e:  ldarg.0
    IL_002f:  ldfld      class b Beetle.BufferWriter::c
    IL_0034:  ldfld      int32 b::g
    IL_0039:  ldelema    [mscorlib]System.Byte
    IL_003e:  stloc.1
    IL_003f:  ldloc.1
    IL_0040:  conv.i
    IL_0041:  ldloc.0
    IL_0042:  stind.i4
    IL_0043:  ldc.i4.0
    IL_0044:  conv.u
    IL_0045:  stloc.1
    IL_0046:  ldarg.0
    IL_0047:  ldfld      class b Beetle.BufferWriter::c
    IL_004c:  dup
    IL_004d:  ldfld      int32 b::g
    IL_0052:  ldc.i4.4
    IL_0053:  add
    IL_0054:  stfld      int32 b::g
    IL_0059:  ldarg.0
    IL_005a:  ldfld      class b Beetle.BufferWriter::c
    IL_005f:  dup
    IL_0060:  ldfld      int32 b::h
    IL_0065:  ldc.i4.4
    IL_0066:  add
    IL_0067:  stfld      int32 b::h
    IL_006c:  ldarg.0
    IL_006d:  dup
    IL_006e:  ldfld      int32 Beetle.BufferWriter::e
    IL_0073:  ldc.i4.4
    IL_0074:  add
    IL_0075:  stfld      int32 Beetle.BufferWriter::e
    IL_007a:  ret

    IL_007b:  ldarg.0
    IL_007c:  ldarg.0
    IL_007d:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0082:  ldloc.0
    IL_0083:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int32)
    IL_0088:  ldc.i4.0
    IL_0089:  ldc.i4.4
    IL_008a:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_008f:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(float32[] 'value') cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0020

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001a

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelem.r4
    IL_0011:  call       instance void Beetle.BufferWriter::Write(float32)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000d

    IL_0020:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<float32> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<float32>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<float32>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(float32)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<float32>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(bool 'value') cil managed
  {
    // 代码大小       79 (0x4f)
    .maxstack  5
    .locals init (uint8[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferWriter::c
    IL_0006:  ldc.i4.1
    IL_0007:  callvirt   instance bool b::c(int32)
    IL_000c:  brfalse.s  IL_003a

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class b Beetle.BufferWriter::c
    IL_0014:  ldc.i4.1
    IL_0015:  newarr     [mscorlib]System.Byte
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  ldarg.1
    IL_001e:  brtrue.s   IL_0023

    IL_0020:  ldc.i4.0
    IL_0021:  br.s       IL_0024

    IL_0023:  ldc.i4.1
    IL_0024:  stelem.i1
    IL_0025:  ldloc.0
    IL_0026:  callvirt   instance void b::a(uint8[])
    IL_002b:  ldarg.0
    IL_002c:  dup
    IL_002d:  ldfld      int32 Beetle.BufferWriter::e
    IL_0032:  ldc.i4.1
    IL_0033:  add
    IL_0034:  stfld      int32 Beetle.BufferWriter::e
    IL_0039:  ret

    IL_003a:  ldarg.0
    IL_003b:  ldarg.0
    IL_003c:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0041:  ldarg.1
    IL_0042:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(bool)
    IL_0047:  ldc.i4.0
    IL_0048:  ldc.i4.1
    IL_0049:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_004e:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(bool[] 'value') cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0020

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001a

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelem.i1
    IL_0011:  call       instance void Beetle.BufferWriter::Write(bool)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000d

    IL_0020:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<bool> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<bool>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<bool>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(bool)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<bool>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(char 'value') cil managed
  {
    // 代码大小       126 (0x7e)
    .maxstack  4
    .locals init (int16 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.1
    IL_0001:  conv.i2
    IL_0002:  stloc.0
    IL_0003:  ldarg.0
    IL_0004:  ldfld      class b Beetle.BufferWriter::c
    IL_0009:  ldc.i4.2
    IL_000a:  callvirt   instance bool b::c(int32)
    IL_000f:  brfalse.s  IL_0069

    IL_0011:  ldarg.0
    IL_0012:  ldfld      class b Beetle.BufferWriter::c
    IL_0017:  ldfld      uint8[] b::e
    IL_001c:  ldarg.0
    IL_001d:  ldfld      class b Beetle.BufferWriter::c
    IL_0022:  ldfld      int32 b::g
    IL_0027:  ldelema    [mscorlib]System.Byte
    IL_002c:  stloc.1
    IL_002d:  ldloc.1
    IL_002e:  conv.i
    IL_002f:  ldloc.0
    IL_0030:  stind.i2
    IL_0031:  ldc.i4.0
    IL_0032:  conv.u
    IL_0033:  stloc.1
    IL_0034:  ldarg.0
    IL_0035:  ldfld      class b Beetle.BufferWriter::c
    IL_003a:  dup
    IL_003b:  ldfld      int32 b::g
    IL_0040:  ldc.i4.2
    IL_0041:  add
    IL_0042:  stfld      int32 b::g
    IL_0047:  ldarg.0
    IL_0048:  ldfld      class b Beetle.BufferWriter::c
    IL_004d:  dup
    IL_004e:  ldfld      int32 b::h
    IL_0053:  ldc.i4.2
    IL_0054:  add
    IL_0055:  stfld      int32 b::h
    IL_005a:  ldarg.0
    IL_005b:  dup
    IL_005c:  ldfld      int32 Beetle.BufferWriter::e
    IL_0061:  ldc.i4.2
    IL_0062:  add
    IL_0063:  stfld      int32 Beetle.BufferWriter::e
    IL_0068:  ret

    IL_0069:  ldarg.0
    IL_006a:  ldarg.0
    IL_006b:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0070:  ldloc.0
    IL_0071:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int16)
    IL_0076:  ldc.i4.0
    IL_0077:  ldc.i4.2
    IL_0078:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_007d:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(char[] 'value') cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0020

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001a

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelem.u2
    IL_0011:  call       instance void Beetle.BufferWriter::Write(char)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000d

    IL_0020:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<char> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<char>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<char>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(char)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<char>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(float64 'value') cil managed
  {
    // 代码大小       144 (0x90)
    .maxstack  4
    .locals init (int64 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferWriter::EnabledVariant
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  call       instance void Beetle.BufferWriter::WriteVariant(float64)
    IL_000f:  ret

    IL_0010:  ldarga.s   'value'
    IL_0012:  conv.u
    IL_0013:  ldind.i8
    IL_0014:  stloc.0
    IL_0015:  ldarg.0
    IL_0016:  ldfld      class b Beetle.BufferWriter::c
    IL_001b:  ldc.i4.8
    IL_001c:  callvirt   instance bool b::c(int32)
    IL_0021:  brfalse.s  IL_007b

    IL_0023:  ldarg.0
    IL_0024:  ldfld      class b Beetle.BufferWriter::c
    IL_0029:  ldfld      uint8[] b::e
    IL_002e:  ldarg.0
    IL_002f:  ldfld      class b Beetle.BufferWriter::c
    IL_0034:  ldfld      int32 b::g
    IL_0039:  ldelema    [mscorlib]System.Byte
    IL_003e:  stloc.1
    IL_003f:  ldloc.1
    IL_0040:  conv.i
    IL_0041:  ldloc.0
    IL_0042:  stind.i8
    IL_0043:  ldc.i4.0
    IL_0044:  conv.u
    IL_0045:  stloc.1
    IL_0046:  ldarg.0
    IL_0047:  ldfld      class b Beetle.BufferWriter::c
    IL_004c:  dup
    IL_004d:  ldfld      int32 b::g
    IL_0052:  ldc.i4.8
    IL_0053:  add
    IL_0054:  stfld      int32 b::g
    IL_0059:  ldarg.0
    IL_005a:  ldfld      class b Beetle.BufferWriter::c
    IL_005f:  dup
    IL_0060:  ldfld      int32 b::h
    IL_0065:  ldc.i4.8
    IL_0066:  add
    IL_0067:  stfld      int32 b::h
    IL_006c:  ldarg.0
    IL_006d:  dup
    IL_006e:  ldfld      int32 Beetle.BufferWriter::e
    IL_0073:  ldc.i4.8
    IL_0074:  add
    IL_0075:  stfld      int32 Beetle.BufferWriter::e
    IL_007a:  ret

    IL_007b:  ldarg.0
    IL_007c:  ldarg.0
    IL_007d:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_0082:  ldloc.0
    IL_0083:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int64)
    IL_0088:  ldc.i4.0
    IL_0089:  ldc.i4.8
    IL_008a:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_008f:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(float64[] 'value') cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0020

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001a

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelem.r8
    IL_0011:  call       instance void Beetle.BufferWriter::Write(float64)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000d

    IL_0020:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<float64> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<float64>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<float64>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(float64)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<float64>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(int16 'value') cil managed
  {
    // 代码大小       123 (0x7b)
    .maxstack  4
    .locals init (uint8& pinned V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferWriter::c
    IL_0006:  ldc.i4.2
    IL_0007:  callvirt   instance bool b::c(int32)
    IL_000c:  brfalse.s  IL_0066

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class b Beetle.BufferWriter::c
    IL_0014:  ldfld      uint8[] b::e
    IL_0019:  ldarg.0
    IL_001a:  ldfld      class b Beetle.BufferWriter::c
    IL_001f:  ldfld      int32 b::g
    IL_0024:  ldelema    [mscorlib]System.Byte
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  conv.i
    IL_002c:  ldarg.1
    IL_002d:  stind.i2
    IL_002e:  ldc.i4.0
    IL_002f:  conv.u
    IL_0030:  stloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      class b Beetle.BufferWriter::c
    IL_0037:  dup
    IL_0038:  ldfld      int32 b::g
    IL_003d:  ldc.i4.2
    IL_003e:  add
    IL_003f:  stfld      int32 b::g
    IL_0044:  ldarg.0
    IL_0045:  ldfld      class b Beetle.BufferWriter::c
    IL_004a:  dup
    IL_004b:  ldfld      int32 b::h
    IL_0050:  ldc.i4.2
    IL_0051:  add
    IL_0052:  stfld      int32 b::h
    IL_0057:  ldarg.0
    IL_0058:  dup
    IL_0059:  ldfld      int32 Beetle.BufferWriter::e
    IL_005e:  ldc.i4.2
    IL_005f:  add
    IL_0060:  stfld      int32 Beetle.BufferWriter::e
    IL_0065:  ret

    IL_0066:  ldarg.0
    IL_0067:  ldarg.0
    IL_0068:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_006d:  ldarg.1
    IL_006e:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int16)
    IL_0073:  ldc.i4.0
    IL_0074:  ldc.i4.2
    IL_0075:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_007a:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(uint16 'value') cil managed
  {
    // 代码大小       123 (0x7b)
    .maxstack  4
    .locals init (uint8& pinned V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferWriter::c
    IL_0006:  ldc.i4.2
    IL_0007:  callvirt   instance bool b::c(int32)
    IL_000c:  brfalse.s  IL_0066

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class b Beetle.BufferWriter::c
    IL_0014:  ldfld      uint8[] b::e
    IL_0019:  ldarg.0
    IL_001a:  ldfld      class b Beetle.BufferWriter::c
    IL_001f:  ldfld      int32 b::g
    IL_0024:  ldelema    [mscorlib]System.Byte
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  conv.i
    IL_002c:  ldarg.1
    IL_002d:  stind.i2
    IL_002e:  ldc.i4.0
    IL_002f:  conv.u
    IL_0030:  stloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      class b Beetle.BufferWriter::c
    IL_0037:  dup
    IL_0038:  ldfld      int32 b::g
    IL_003d:  ldc.i4.2
    IL_003e:  add
    IL_003f:  stfld      int32 b::g
    IL_0044:  ldarg.0
    IL_0045:  ldfld      class b Beetle.BufferWriter::c
    IL_004a:  dup
    IL_004b:  ldfld      int32 b::h
    IL_0050:  ldc.i4.2
    IL_0051:  add
    IL_0052:  stfld      int32 b::h
    IL_0057:  ldarg.0
    IL_0058:  dup
    IL_0059:  ldfld      int32 Beetle.BufferWriter::e
    IL_005e:  ldc.i4.2
    IL_005f:  add
    IL_0060:  stfld      int32 Beetle.BufferWriter::e
    IL_0065:  ret

    IL_0066:  ldarg.0
    IL_0067:  ldarg.0
    IL_0068:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_006d:  ldarg.1
    IL_006e:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(uint16)
    IL_0073:  ldc.i4.0
    IL_0074:  ldc.i4.2
    IL_0075:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_007a:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(int16[] 'value') cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0020

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001a

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelem.i2
    IL_0011:  call       instance void Beetle.BufferWriter::Write(int16)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000d

    IL_0020:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<int16> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<int16>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<int16>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(int16)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<int16>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(int32 'value') cil managed
  {
    // 代码大小       139 (0x8b)
    .maxstack  4
    .locals init (uint8& pinned V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferWriter::EnabledVariant
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  call       instance void Beetle.BufferWriter::WriteVariant(int32)
    IL_000f:  ret

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class b Beetle.BufferWriter::c
    IL_0016:  ldc.i4.4
    IL_0017:  callvirt   instance bool b::c(int32)
    IL_001c:  brfalse.s  IL_0076

    IL_001e:  ldarg.0
    IL_001f:  ldfld      class b Beetle.BufferWriter::c
    IL_0024:  ldfld      uint8[] b::e
    IL_0029:  ldarg.0
    IL_002a:  ldfld      class b Beetle.BufferWriter::c
    IL_002f:  ldfld      int32 b::g
    IL_0034:  ldelema    [mscorlib]System.Byte
    IL_0039:  stloc.0
    IL_003a:  ldloc.0
    IL_003b:  conv.i
    IL_003c:  ldarg.1
    IL_003d:  stind.i4
    IL_003e:  ldc.i4.0
    IL_003f:  conv.u
    IL_0040:  stloc.0
    IL_0041:  ldarg.0
    IL_0042:  ldfld      class b Beetle.BufferWriter::c
    IL_0047:  dup
    IL_0048:  ldfld      int32 b::g
    IL_004d:  ldc.i4.4
    IL_004e:  add
    IL_004f:  stfld      int32 b::g
    IL_0054:  ldarg.0
    IL_0055:  ldfld      class b Beetle.BufferWriter::c
    IL_005a:  dup
    IL_005b:  ldfld      int32 b::h
    IL_0060:  ldc.i4.4
    IL_0061:  add
    IL_0062:  stfld      int32 b::h
    IL_0067:  ldarg.0
    IL_0068:  dup
    IL_0069:  ldfld      int32 Beetle.BufferWriter::e
    IL_006e:  ldc.i4.4
    IL_006f:  add
    IL_0070:  stfld      int32 Beetle.BufferWriter::e
    IL_0075:  ret

    IL_0076:  ldarg.0
    IL_0077:  ldarg.0
    IL_0078:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_007d:  ldarg.1
    IL_007e:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int32)
    IL_0083:  ldc.i4.0
    IL_0084:  ldc.i4.4
    IL_0085:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_008a:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(uint32 'value') cil managed
  {
    // 代码大小       139 (0x8b)
    .maxstack  4
    .locals init (uint8& pinned V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferWriter::EnabledVariant
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  call       instance void Beetle.BufferWriter::WriteVariant(uint32)
    IL_000f:  ret

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class b Beetle.BufferWriter::c
    IL_0016:  ldc.i4.4
    IL_0017:  callvirt   instance bool b::c(int32)
    IL_001c:  brfalse.s  IL_0076

    IL_001e:  ldarg.0
    IL_001f:  ldfld      class b Beetle.BufferWriter::c
    IL_0024:  ldfld      uint8[] b::e
    IL_0029:  ldarg.0
    IL_002a:  ldfld      class b Beetle.BufferWriter::c
    IL_002f:  ldfld      int32 b::g
    IL_0034:  ldelema    [mscorlib]System.Byte
    IL_0039:  stloc.0
    IL_003a:  ldloc.0
    IL_003b:  conv.i
    IL_003c:  ldarg.1
    IL_003d:  stind.i4
    IL_003e:  ldc.i4.0
    IL_003f:  conv.u
    IL_0040:  stloc.0
    IL_0041:  ldarg.0
    IL_0042:  ldfld      class b Beetle.BufferWriter::c
    IL_0047:  dup
    IL_0048:  ldfld      int32 b::g
    IL_004d:  ldc.i4.4
    IL_004e:  add
    IL_004f:  stfld      int32 b::g
    IL_0054:  ldarg.0
    IL_0055:  ldfld      class b Beetle.BufferWriter::c
    IL_005a:  dup
    IL_005b:  ldfld      int32 b::h
    IL_0060:  ldc.i4.4
    IL_0061:  add
    IL_0062:  stfld      int32 b::h
    IL_0067:  ldarg.0
    IL_0068:  dup
    IL_0069:  ldfld      int32 Beetle.BufferWriter::e
    IL_006e:  ldc.i4.4
    IL_006f:  add
    IL_0070:  stfld      int32 Beetle.BufferWriter::e
    IL_0075:  ret

    IL_0076:  ldarg.0
    IL_0077:  ldarg.0
    IL_0078:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_007d:  ldarg.1
    IL_007e:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(uint32)
    IL_0083:  ldc.i4.0
    IL_0084:  ldc.i4.4
    IL_0085:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_008a:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(int32[] 'value') cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0020

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001a

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelem.i4
    IL_0011:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000d

    IL_0020:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<int32> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<int32>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<int32>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<int32>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(int64 'value') cil managed
  {
    // 代码大小       139 (0x8b)
    .maxstack  4
    .locals init (uint8& pinned V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferWriter::EnabledVariant
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  call       instance void Beetle.BufferWriter::WriteVariant(int64)
    IL_000f:  ret

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class b Beetle.BufferWriter::c
    IL_0016:  ldc.i4.8
    IL_0017:  callvirt   instance bool b::c(int32)
    IL_001c:  brfalse.s  IL_0076

    IL_001e:  ldarg.0
    IL_001f:  ldfld      class b Beetle.BufferWriter::c
    IL_0024:  ldfld      uint8[] b::e
    IL_0029:  ldarg.0
    IL_002a:  ldfld      class b Beetle.BufferWriter::c
    IL_002f:  ldfld      int32 b::g
    IL_0034:  ldelema    [mscorlib]System.Byte
    IL_0039:  stloc.0
    IL_003a:  ldloc.0
    IL_003b:  conv.i
    IL_003c:  ldarg.1
    IL_003d:  stind.i8
    IL_003e:  ldc.i4.0
    IL_003f:  conv.u
    IL_0040:  stloc.0
    IL_0041:  ldarg.0
    IL_0042:  ldfld      class b Beetle.BufferWriter::c
    IL_0047:  dup
    IL_0048:  ldfld      int32 b::g
    IL_004d:  ldc.i4.8
    IL_004e:  add
    IL_004f:  stfld      int32 b::g
    IL_0054:  ldarg.0
    IL_0055:  ldfld      class b Beetle.BufferWriter::c
    IL_005a:  dup
    IL_005b:  ldfld      int32 b::h
    IL_0060:  ldc.i4.8
    IL_0061:  add
    IL_0062:  stfld      int32 b::h
    IL_0067:  ldarg.0
    IL_0068:  dup
    IL_0069:  ldfld      int32 Beetle.BufferWriter::e
    IL_006e:  ldc.i4.8
    IL_006f:  add
    IL_0070:  stfld      int32 Beetle.BufferWriter::e
    IL_0075:  ret

    IL_0076:  ldarg.0
    IL_0077:  ldarg.0
    IL_0078:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_007d:  ldarg.1
    IL_007e:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int64)
    IL_0083:  ldc.i4.0
    IL_0084:  ldc.i4.8
    IL_0085:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_008a:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(uint64 'value') cil managed
  {
    // 代码大小       139 (0x8b)
    .maxstack  4
    .locals init (uint8& pinned V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferWriter::EnabledVariant
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  call       instance void Beetle.BufferWriter::WriteVariant(uint64)
    IL_000f:  ret

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class b Beetle.BufferWriter::c
    IL_0016:  ldc.i4.8
    IL_0017:  callvirt   instance bool b::c(int32)
    IL_001c:  brfalse.s  IL_0076

    IL_001e:  ldarg.0
    IL_001f:  ldfld      class b Beetle.BufferWriter::c
    IL_0024:  ldfld      uint8[] b::e
    IL_0029:  ldarg.0
    IL_002a:  ldfld      class b Beetle.BufferWriter::c
    IL_002f:  ldfld      int32 b::g
    IL_0034:  ldelema    [mscorlib]System.Byte
    IL_0039:  stloc.0
    IL_003a:  ldloc.0
    IL_003b:  conv.i
    IL_003c:  ldarg.1
    IL_003d:  stind.i8
    IL_003e:  ldc.i4.0
    IL_003f:  conv.u
    IL_0040:  stloc.0
    IL_0041:  ldarg.0
    IL_0042:  ldfld      class b Beetle.BufferWriter::c
    IL_0047:  dup
    IL_0048:  ldfld      int32 b::g
    IL_004d:  ldc.i4.8
    IL_004e:  add
    IL_004f:  stfld      int32 b::g
    IL_0054:  ldarg.0
    IL_0055:  ldfld      class b Beetle.BufferWriter::c
    IL_005a:  dup
    IL_005b:  ldfld      int32 b::h
    IL_0060:  ldc.i4.8
    IL_0061:  add
    IL_0062:  stfld      int32 b::h
    IL_0067:  ldarg.0
    IL_0068:  dup
    IL_0069:  ldfld      int32 Beetle.BufferWriter::e
    IL_006e:  ldc.i4.8
    IL_006f:  add
    IL_0070:  stfld      int32 Beetle.BufferWriter::e
    IL_0075:  ret

    IL_0076:  ldarg.0
    IL_0077:  ldarg.0
    IL_0078:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
    IL_007d:  ldarg.1
    IL_007e:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(uint64)
    IL_0083:  ldc.i4.0
    IL_0084:  ldc.i4.8
    IL_0085:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_008a:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(int64[] 'value') cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0020

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001a

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelem.i8
    IL_0011:  call       instance void Beetle.BufferWriter::Write(int64)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000d

    IL_0020:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<int64> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<int64>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<int64>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(int64)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<int64>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(uint8[] 'value') cil managed
  {
    // 代码大小       38 (0x26)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  brfalse.s  IL_000f

    IL_0005:  ldarg.1
    IL_0006:  ldlen
    IL_0007:  conv.i4
    IL_0008:  ldc.i4.0
    IL_0009:  ble.s      IL_000f

    IL_000b:  ldarg.1
    IL_000c:  ldlen
    IL_000d:  conv.i4
    IL_000e:  stloc.0
    IL_000f:  ldarg.0
    IL_0010:  ldloc.0
    IL_0011:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  ble.s      IL_0025

    IL_001a:  ldarg.0
    IL_001b:  ldarg.1
    IL_001c:  ldc.i4.0
    IL_001d:  ldarg.1
    IL_001e:  ldlen
    IL_001f:  conv.i4
    IL_0020:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0025:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(string 'value') cil managed
  {
    // 代码大小       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       bool [mscorlib]System.String::IsNullOrEmpty(string)
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.0
    IL_000a:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_000f:  ret

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class ai Beetle.BufferWriter::h
    IL_0016:  ldarg.1
    IL_0017:  ldarg.0
    IL_0018:  ldfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_001d:  ldarg.0
    IL_001e:  ldfld      class b Beetle.BufferWriter::c
    IL_0023:  ldarg.0
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void ai::a(string,
                                             class [mscorlib]System.Text.Encoding,
                                             class b,
                                             class Beetle.BufferWriter,
                                             bool)
    IL_002a:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(string[] 'value') cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::a(class [mscorlib]System.Array)
    IL_0007:  brfalse.s  IL_0020

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001a

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  ldelem.ref
    IL_0011:  call       instance void Beetle.BufferWriter::Write(string)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldarg.1
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  blt.s      IL_000d

    IL_0020:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class [mscorlib]System.Collections.Generic.IList`1<string> values) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance bool Beetle.BufferWriter::WriteIListLength<string>(class [mscorlib]System.Collections.Generic.IList`1<!!0>)
    IL_0007:  brfalse.s  IL_0027

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_001e

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<string>::get_Item(int32)
    IL_0015:  call       instance void Beetle.BufferWriter::Write(string)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldarg.1
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<string>::get_Count()
    IL_0025:  blt.s      IL_000d

    IL_0027:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(class Beetle.IMessage obj) cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance void Beetle.IMessage::Save(class Beetle.BufferWriter)
    IL_0007:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write<(Beetle.IMessage) T>(class [mscorlib]System.Collections.Generic.IList`1<!!T> objs) cil managed
  {
    // 代码大小       59 (0x3b)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000b

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.0
    IL_0005:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_000a:  ret

    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<!!T>::get_Count()
    IL_0012:  call       instance void Beetle.BufferWriter::Write(int32)
    IL_0017:  ldc.i4.0
    IL_0018:  stloc.0
    IL_0019:  br.s       IL_0031

    IL_001b:  ldarg.0
    IL_001c:  ldarg.1
    IL_001d:  ldloc.0
    IL_001e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<!!T>::get_Item(int32)
    IL_0023:  box        !!T
    IL_0028:  call       instance void Beetle.BufferWriter::Write(class Beetle.IMessage)
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  stloc.0
    IL_0031:  ldloc.0
    IL_0032:  ldarg.1
    IL_0033:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<!!T>::get_Count()
    IL_0038:  blt.s      IL_001b

    IL_003a:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance void 
          Write(uint8[] 'value',
                int32 start,
                int32 length) cil managed
  {
    // 代码大小       193 (0xc1)
    .maxstack  5
    .locals init (uint8[] V_0,
             class b V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class n Beetle.BufferWriter::l
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  ldarg.3
    IL_0009:  callvirt   instance void n::e(uint8[],
                                            int32,
                                            int32)
    IL_000e:  ldarg.0
    IL_000f:  ldc.i4.0
    IL_0010:  stfld      int32 Beetle.BufferWriter::k
    IL_0015:  ldarg.0
    IL_0016:  ldfld      bool Beetle.BufferWriter::f
    IL_001b:  brfalse.s  IL_0082

    IL_001d:  ldarg.0
    IL_001e:  ldfld      class b Beetle.BufferWriter::c
    IL_0023:  ldarg.0
    IL_0024:  ldfld      class n Beetle.BufferWriter::l
    IL_0029:  callvirt   instance void b::a(class n)
    IL_002e:  ldarg.0
    IL_002f:  dup
    IL_0030:  ldfld      int32 Beetle.BufferWriter::k
    IL_0035:  ldarg.0
    IL_0036:  ldfld      class n Beetle.BufferWriter::l
    IL_003b:  ldfld      int32 n::d
    IL_0040:  add
    IL_0041:  stfld      int32 Beetle.BufferWriter::k
    IL_0046:  br.s       IL_0077

    IL_0048:  ldarg.0
    IL_0049:  call       instance void Beetle.BufferWriter::a()
    IL_004e:  ldarg.0
    IL_004f:  ldfld      class b Beetle.BufferWriter::c
    IL_0054:  ldarg.0
    IL_0055:  ldfld      class n Beetle.BufferWriter::l
    IL_005a:  callvirt   instance void b::a(class n)
    IL_005f:  ldarg.0
    IL_0060:  dup
    IL_0061:  ldfld      int32 Beetle.BufferWriter::k
    IL_0066:  ldarg.0
    IL_0067:  ldfld      class n Beetle.BufferWriter::l
    IL_006c:  ldfld      int32 n::d
    IL_0071:  add
    IL_0072:  stfld      int32 Beetle.BufferWriter::k
    IL_0077:  ldarg.0
    IL_0078:  ldfld      int32 Beetle.BufferWriter::k
    IL_007d:  ldarg.3
    IL_007e:  blt.s      IL_0048

    IL_0080:  br.s       IL_00ad

    IL_0082:  ldarg.0
    IL_0083:  ldarg.3
    IL_0084:  stfld      int32 Beetle.BufferWriter::k
    IL_0089:  ldarg.3
    IL_008a:  newarr     [mscorlib]System.Byte
    IL_008f:  stloc.0
    IL_0090:  ldarg.1
    IL_0091:  ldarg.2
    IL_0092:  ldloc.0
    IL_0093:  ldc.i4.0
    IL_0094:  ldarg.3
    IL_0095:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_009a:  ldloc.0
    IL_009b:  newobj     instance void b::.ctor(uint8[])
    IL_00a0:  stloc.1
    IL_00a1:  ldarg.0
    IL_00a2:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_00a7:  ldloc.1
    IL_00a8:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class b>::Enqueue(!0)
    IL_00ad:  ldarg.0
    IL_00ae:  dup
    IL_00af:  ldfld      int32 Beetle.BufferWriter::e
    IL_00b4:  ldarg.0
    IL_00b5:  ldfld      int32 Beetle.BufferWriter::k
    IL_00ba:  add
    IL_00bb:  stfld      int32 Beetle.BufferWriter::e
    IL_00c0:  ret
  } // end of method BufferWriter::Write

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> 
          WriteReturnSegment(uint8[] 'value',
                             int32 start,
                             int32 length) cil managed
  {
    // 代码大小       420 (0x1a4)
    .maxstack  5
    .locals init (int32 V_0,
             uint8[] V_1,
             class b V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_0006:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.ByteArraySegment>::Clear()
    IL_000b:  ldc.i4.0
    IL_000c:  stloc.0
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class n Beetle.BufferWriter::l
    IL_0013:  ldarg.1
    IL_0014:  ldarg.2
    IL_0015:  ldarg.3
    IL_0016:  callvirt   instance void n::e(uint8[],
                                            int32,
                                            int32)
    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.0
    IL_001d:  stfld      int32 Beetle.BufferWriter::k
    IL_0022:  ldarg.0
    IL_0023:  ldfld      bool Beetle.BufferWriter::f
    IL_0028:  brfalse    IL_013a

    IL_002d:  ldarg.0
    IL_002e:  ldfld      class b Beetle.BufferWriter::c
    IL_0033:  ldarg.0
    IL_0034:  ldfld      class n Beetle.BufferWriter::l
    IL_0039:  callvirt   instance void b::a(class n)
    IL_003e:  ldarg.0
    IL_003f:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0044:  ldloc.0
    IL_0045:  ldelem.ref
    IL_0046:  ldarg.0
    IL_0047:  ldfld      class b Beetle.BufferWriter::c
    IL_004c:  ldfld      uint8[] b::e
    IL_0051:  ldarg.0
    IL_0052:  ldfld      class b Beetle.BufferWriter::c
    IL_0057:  ldfld      int32 b::h
    IL_005c:  ldarg.0
    IL_005d:  ldfld      class n Beetle.BufferWriter::l
    IL_0062:  ldfld      int32 n::d
    IL_0067:  sub
    IL_0068:  ldarg.0
    IL_0069:  ldfld      class n Beetle.BufferWriter::l
    IL_006e:  ldfld      int32 n::d
    IL_0073:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                        int32,
                                                                        int32)
    IL_0078:  ldarg.0
    IL_0079:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_007e:  ldarg.0
    IL_007f:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0084:  ldloc.0
    IL_0085:  ldelem.ref
    IL_0086:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.ByteArraySegment>::Add(!0)
    IL_008b:  ldarg.0
    IL_008c:  dup
    IL_008d:  ldfld      int32 Beetle.BufferWriter::k
    IL_0092:  ldarg.0
    IL_0093:  ldfld      class n Beetle.BufferWriter::l
    IL_0098:  ldfld      int32 n::d
    IL_009d:  add
    IL_009e:  stfld      int32 Beetle.BufferWriter::k
    IL_00a3:  ldloc.0
    IL_00a4:  ldc.i4.1
    IL_00a5:  add
    IL_00a6:  stloc.0
    IL_00a7:  br         IL_012c

    IL_00ac:  ldarg.0
    IL_00ad:  call       instance void Beetle.BufferWriter::a()
    IL_00b2:  ldarg.0
    IL_00b3:  ldfld      class b Beetle.BufferWriter::c
    IL_00b8:  ldarg.0
    IL_00b9:  ldfld      class n Beetle.BufferWriter::l
    IL_00be:  callvirt   instance void b::a(class n)
    IL_00c3:  ldarg.0
    IL_00c4:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_00c9:  ldloc.0
    IL_00ca:  ldelem.ref
    IL_00cb:  ldarg.0
    IL_00cc:  ldfld      class b Beetle.BufferWriter::c
    IL_00d1:  ldfld      uint8[] b::e
    IL_00d6:  ldarg.0
    IL_00d7:  ldfld      class b Beetle.BufferWriter::c
    IL_00dc:  ldfld      int32 b::h
    IL_00e1:  ldarg.0
    IL_00e2:  ldfld      class n Beetle.BufferWriter::l
    IL_00e7:  ldfld      int32 n::d
    IL_00ec:  sub
    IL_00ed:  ldarg.0
    IL_00ee:  ldfld      class n Beetle.BufferWriter::l
    IL_00f3:  ldfld      int32 n::d
    IL_00f8:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                        int32,
                                                                        int32)
    IL_00fd:  ldarg.0
    IL_00fe:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_0103:  ldarg.0
    IL_0104:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0109:  ldloc.0
    IL_010a:  ldelem.ref
    IL_010b:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.ByteArraySegment>::Add(!0)
    IL_0110:  ldarg.0
    IL_0111:  dup
    IL_0112:  ldfld      int32 Beetle.BufferWriter::k
    IL_0117:  ldarg.0
    IL_0118:  ldfld      class n Beetle.BufferWriter::l
    IL_011d:  ldfld      int32 n::d
    IL_0122:  add
    IL_0123:  stfld      int32 Beetle.BufferWriter::k
    IL_0128:  ldloc.0
    IL_0129:  ldc.i4.1
    IL_012a:  add
    IL_012b:  stloc.0
    IL_012c:  ldarg.0
    IL_012d:  ldfld      int32 Beetle.BufferWriter::k
    IL_0132:  ldarg.3
    IL_0133:  blt        IL_00ac

    IL_0138:  br.s       IL_018a

    IL_013a:  ldarg.0
    IL_013b:  ldarg.3
    IL_013c:  stfld      int32 Beetle.BufferWriter::k
    IL_0141:  ldarg.3
    IL_0142:  newarr     [mscorlib]System.Byte
    IL_0147:  stloc.1
    IL_0148:  ldarg.1
    IL_0149:  ldarg.2
    IL_014a:  ldloc.1
    IL_014b:  ldc.i4.0
    IL_014c:  ldarg.3
    IL_014d:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_0152:  ldloc.1
    IL_0153:  newobj     instance void b::.ctor(uint8[])
    IL_0158:  stloc.2
    IL_0159:  ldarg.0
    IL_015a:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_015f:  ldloc.2
    IL_0160:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class b>::Enqueue(!0)
    IL_0165:  ldarg.0
    IL_0166:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_016b:  ldloc.0
    IL_016c:  ldelem.ref
    IL_016d:  ldloc.1
    IL_016e:  ldc.i4.0
    IL_016f:  ldloc.1
    IL_0170:  ldlen
    IL_0171:  conv.i4
    IL_0172:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                        int32,
                                                                        int32)
    IL_0177:  ldarg.0
    IL_0178:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_017d:  ldarg.0
    IL_017e:  ldfld      class Beetle.ByteArraySegment[] Beetle.BufferWriter::m
    IL_0183:  ldloc.0
    IL_0184:  ldelem.ref
    IL_0185:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.ByteArraySegment>::Add(!0)
    IL_018a:  ldarg.0
    IL_018b:  dup
    IL_018c:  ldfld      int32 Beetle.BufferWriter::e
    IL_0191:  ldarg.0
    IL_0192:  ldfld      int32 Beetle.BufferWriter::k
    IL_0197:  add
    IL_0198:  stfld      int32 Beetle.BufferWriter::e
    IL_019d:  ldarg.0
    IL_019e:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::n
    IL_01a3:  ret
  } // end of method BufferWriter::WriteReturnSegment

  .method public hidebysig instance void 
          WriteVariant(int32 'value') cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  3
    .locals init (uint32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  shl
    IL_0003:  ldarg.1
    IL_0004:  ldc.i4.s   31
    IL_0006:  shr
    IL_0007:  xor
    IL_0008:  stloc.0
    IL_0009:  ldarg.0
    IL_000a:  ldloc.0
    IL_000b:  call       instance void Beetle.BufferWriter::a(uint32)
    IL_0010:  ret
  } // end of method BufferWriter::WriteVariant

  .method public hidebysig instance void 
          WriteVariant(uint32 'value') cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void Beetle.BufferWriter::a(uint32)
    IL_0007:  ret
  } // end of method BufferWriter::WriteVariant

  .method public hidebysig instance void 
          WriteVariant(int64 'value') cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  3
    .locals init (uint64 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  shl
    IL_0003:  ldarg.1
    IL_0004:  ldc.i4.s   63
    IL_0006:  shr
    IL_0007:  xor
    IL_0008:  stloc.0
    IL_0009:  ldarg.0
    IL_000a:  ldloc.0
    IL_000b:  call       instance void Beetle.BufferWriter::a(uint64)
    IL_0010:  ret
  } // end of method BufferWriter::WriteVariant

  .method public hidebysig instance void 
          WriteVariant(uint64 'value') cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void Beetle.BufferWriter::a(uint64)
    IL_0007:  ret
  } // end of method BufferWriter::WriteVariant

  .method public hidebysig instance void 
          WriteVariant(float64 'value') cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarga.s   'value'
    IL_0003:  conv.u
    IL_0004:  ldind.i8
    IL_0005:  call       instance void Beetle.BufferWriter::WriteVariant(int64)
    IL_000a:  ret
  } // end of method BufferWriter::WriteVariant

  .method public hidebysig instance void 
          WriteVariant(float32 'value') cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarga.s   'value'
    IL_0003:  conv.u
    IL_0004:  ldind.i4
    IL_0005:  call       instance void Beetle.BufferWriter::WriteVariant(int32)
    IL_000a:  ret
  } // end of method BufferWriter::WriteVariant

  .method private hidebysig instance void 
          a(uint32 A_0) cil managed
  {
    // 代码大小       135 (0x87)
    .maxstack  5
    .locals init (uint8[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferWriter::c
    IL_0006:  ldc.i4.1
    IL_0007:  callvirt   instance bool b::c(int32)
    IL_000c:  brfalse.s  IL_0034

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class b Beetle.BufferWriter::c
    IL_0014:  ldarg.1
    IL_0015:  ldc.i4.s   127
    IL_0017:  and
    IL_0018:  ldc.i4     0x80
    IL_001d:  or
    IL_001e:  conv.u1
    IL_001f:  callvirt   instance void b::a(uint8)
    IL_0024:  ldarg.0
    IL_0025:  dup
    IL_0026:  ldfld      int32 Beetle.BufferWriter::e
    IL_002b:  ldc.i4.1
    IL_002c:  add
    IL_002d:  stfld      int32 Beetle.BufferWriter::e
    IL_0032:  br.s       IL_0052

    IL_0034:  ldarg.0
    IL_0035:  ldc.i4.1
    IL_0036:  newarr     [mscorlib]System.Byte
    IL_003b:  stloc.0
    IL_003c:  ldloc.0
    IL_003d:  ldc.i4.0
    IL_003e:  ldarg.1
    IL_003f:  ldc.i4.s   127
    IL_0041:  and
    IL_0042:  ldc.i4     0x80
    IL_0047:  or
    IL_0048:  conv.u1
    IL_0049:  stelem.i1
    IL_004a:  ldloc.0
    IL_004b:  ldc.i4.0
    IL_004c:  ldc.i4.1
    IL_004d:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0052:  ldarg.1
    IL_0053:  ldc.i4.7
    IL_0054:  shr.un
    IL_0055:  dup
    IL_0056:  starg.s    A_0
    IL_0058:  brtrue.s   IL_0000

    IL_005a:  ldarg.0
    IL_005b:  ldfld      class b Beetle.BufferWriter::c
    IL_0060:  ldfld      uint8[] b::e
    IL_0065:  ldarg.0
    IL_0066:  ldfld      class b Beetle.BufferWriter::c
    IL_006b:  ldfld      int32 b::g
    IL_0070:  ldc.i4.1
    IL_0071:  sub
    IL_0072:  ldelema    [mscorlib]System.Byte
    IL_0077:  dup
    IL_0078:  ldobj      [mscorlib]System.Byte
    IL_007d:  ldc.i4.s   127
    IL_007f:  and
    IL_0080:  conv.u1
    IL_0081:  stobj      [mscorlib]System.Byte
    IL_0086:  ret
  } // end of method BufferWriter::a

  .method private hidebysig instance void 
          a(uint64 A_0) cil managed
  {
    // 代码大小       141 (0x8d)
    .maxstack  5
    .locals init (uint8[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferWriter::c
    IL_0006:  ldc.i4.1
    IL_0007:  callvirt   instance bool b::c(int32)
    IL_000c:  brfalse.s  IL_0036

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class b Beetle.BufferWriter::c
    IL_0014:  ldarg.1
    IL_0015:  ldc.i4.s   127
    IL_0017:  conv.i8
    IL_0018:  and
    IL_0019:  ldc.i4     0x80
    IL_001e:  conv.i8
    IL_001f:  or
    IL_0020:  conv.u1
    IL_0021:  callvirt   instance void b::a(uint8)
    IL_0026:  ldarg.0
    IL_0027:  dup
    IL_0028:  ldfld      int32 Beetle.BufferWriter::e
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  stfld      int32 Beetle.BufferWriter::e
    IL_0034:  br.s       IL_0056

    IL_0036:  ldarg.0
    IL_0037:  ldc.i4.1
    IL_0038:  newarr     [mscorlib]System.Byte
    IL_003d:  stloc.0
    IL_003e:  ldloc.0
    IL_003f:  ldc.i4.0
    IL_0040:  ldarg.1
    IL_0041:  ldc.i4.s   127
    IL_0043:  conv.i8
    IL_0044:  and
    IL_0045:  ldc.i4     0x80
    IL_004a:  conv.i8
    IL_004b:  or
    IL_004c:  conv.u1
    IL_004d:  stelem.i1
    IL_004e:  ldloc.0
    IL_004f:  ldc.i4.0
    IL_0050:  ldc.i4.1
    IL_0051:  call       instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0056:  ldarg.1
    IL_0057:  ldc.i4.7
    IL_0058:  shr.un
    IL_0059:  dup
    IL_005a:  starg.s    A_0
    IL_005c:  ldc.i4.0
    IL_005d:  conv.i8
    IL_005e:  bne.un.s   IL_0000

    IL_0060:  ldarg.0
    IL_0061:  ldfld      class b Beetle.BufferWriter::c
    IL_0066:  ldfld      uint8[] b::e
    IL_006b:  ldarg.0
    IL_006c:  ldfld      class b Beetle.BufferWriter::c
    IL_0071:  ldfld      int32 b::g
    IL_0076:  ldc.i4.1
    IL_0077:  sub
    IL_0078:  ldelema    [mscorlib]System.Byte
    IL_007d:  dup
    IL_007e:  ldobj      [mscorlib]System.Byte
    IL_0083:  ldc.i4.s   127
    IL_0085:  and
    IL_0086:  conv.u1
    IL_0087:  stobj      [mscorlib]System.Byte
    IL_008c:  ret
  } // end of method BufferWriter::a

  .method public hidebysig instance void 
          WriteString(string 'value') cil managed
  {
    // 代码大小       27 (0x1b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class ai Beetle.BufferWriter::h
    IL_0006:  ldarg.1
    IL_0007:  ldarg.0
    IL_0008:  ldfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class b Beetle.BufferWriter::c
    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.0
    IL_0015:  callvirt   instance void ai::a(string,
                                             class [mscorlib]System.Text.Encoding,
                                             class b,
                                             class Beetle.BufferWriter,
                                             bool)
    IL_001a:  ret
  } // end of method BufferWriter::WriteString

  .method public hidebysig instance void 
          WriteShortString(string 'value') cil managed
  {
    // 代码大小       27 (0x1b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class ai Beetle.BufferWriter::h
    IL_0006:  ldarg.1
    IL_0007:  ldarg.0
    IL_0008:  ldfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class b Beetle.BufferWriter::c
    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.1
    IL_0015:  callvirt   instance void ai::b(string,
                                             class [mscorlib]System.Text.Encoding,
                                             class b,
                                             class Beetle.BufferWriter,
                                             bool)
    IL_001a:  ret
  } // end of method BufferWriter::WriteShortString

  .property instance int32 Count()
  {
    .get instance int32 Beetle.BufferWriter::get_Count()
  } // end of property BufferWriter::Count
} // end of class Beetle.BufferWriter

.class public auto ansi beforefieldinit Beetle.IpItem
       extends [mscorlib]System.Object
{
  .field private bool a
  .field private bool b
  .field private uint8[] c
  .field private uint8[] d
  .field private string e
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [System]System.Net.IPAddress ip) cil managed
  {
    // 代码大小       26 (0x1a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  stfld      bool Beetle.IpItem::a
    IL_0007:  ldarg.0
    IL_0008:  call       instance void [mscorlib]System.Object::.ctor()
    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  callvirt   instance uint8[] [System]System.Net.IPAddress::GetAddressBytes()
    IL_0014:  stfld      uint8[] Beetle.IpItem::c
    IL_0019:  ret
  } // end of method IpItem::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [System]System.Net.IPAddress startip,
                               class [System]System.Net.IPAddress endip) cil managed
  {
    // 代码大小       45 (0x2d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  stfld      bool Beetle.IpItem::a
    IL_0007:  ldarg.0
    IL_0008:  call       instance void [mscorlib]System.Object::.ctor()
    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  callvirt   instance uint8[] [System]System.Net.IPAddress::GetAddressBytes()
    IL_0014:  stfld      uint8[] Beetle.IpItem::c
    IL_0019:  ldarg.0
    IL_001a:  ldarg.2
    IL_001b:  callvirt   instance uint8[] [System]System.Net.IPAddress::GetAddressBytes()
    IL_0020:  stfld      uint8[] Beetle.IpItem::d
    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.1
    IL_0027:  stfld      bool Beetle.IpItem::b
    IL_002c:  ret
  } // end of method IpItem::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [System]System.Net.IPAddress startip,
                               class [System]System.Net.IPAddress endip,
                               bool permit) cil managed
  {
    // 代码大小       52 (0x34)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  stfld      bool Beetle.IpItem::a
    IL_0007:  ldarg.0
    IL_0008:  call       instance void [mscorlib]System.Object::.ctor()
    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  callvirt   instance uint8[] [System]System.Net.IPAddress::GetAddressBytes()
    IL_0014:  stfld      uint8[] Beetle.IpItem::c
    IL_0019:  ldarg.0
    IL_001a:  ldarg.2
    IL_001b:  callvirt   instance uint8[] [System]System.Net.IPAddress::GetAddressBytes()
    IL_0020:  stfld      uint8[] Beetle.IpItem::d
    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.1
    IL_0027:  stfld      bool Beetle.IpItem::b
    IL_002c:  ldarg.0
    IL_002d:  ldc.i4.1
    IL_002e:  stfld      bool Beetle.IpItem::a
    IL_0033:  ret
  } // end of method IpItem::.ctor

  .method public hidebysig specialname instance bool 
          get_Permit() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.IpItem::a
    IL_0006:  ret
  } // end of method IpItem::get_Permit

  .method public hidebysig instance bool 
          Verification(class [System]System.Net.IPAddress ip) cil managed
  {
    // 代码大小       231 (0xe7)
    .maxstack  4
    .locals init (bool V_0,
             uint8[] V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance uint8[] [System]System.Net.IPAddress::GetAddressBytes()
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  ldfld      bool Beetle.IpItem::b
    IL_000f:  brfalse    IL_00a0

    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  ldelem.u1
    IL_0017:  ldarg.0
    IL_0018:  ldfld      uint8[] Beetle.IpItem::c
    IL_001d:  ldc.i4.0
    IL_001e:  ldelem.u1
    IL_001f:  blt.s      IL_009c

    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.0
    IL_0023:  ldelem.u1
    IL_0024:  ldarg.0
    IL_0025:  ldfld      uint8[] Beetle.IpItem::d
    IL_002a:  ldc.i4.0
    IL_002b:  ldelem.u1
    IL_002c:  cgt
    IL_002e:  ldc.i4.0
    IL_002f:  ceq
    IL_0031:  ldloc.1
    IL_0032:  ldc.i4.1
    IL_0033:  ldelem.u1
    IL_0034:  ldarg.0
    IL_0035:  ldfld      uint8[] Beetle.IpItem::c
    IL_003a:  ldc.i4.1
    IL_003b:  ldelem.u1
    IL_003c:  clt
    IL_003e:  ldc.i4.0
    IL_003f:  ceq
    IL_0041:  and
    IL_0042:  brfalse.s  IL_009c

    IL_0044:  ldloc.1
    IL_0045:  ldc.i4.0
    IL_0046:  ldelem.u1
    IL_0047:  ldarg.0
    IL_0048:  ldfld      uint8[] Beetle.IpItem::d
    IL_004d:  ldc.i4.1
    IL_004e:  ldelem.u1
    IL_004f:  cgt
    IL_0051:  ldc.i4.0
    IL_0052:  ceq
    IL_0054:  ldloc.1
    IL_0055:  ldc.i4.2
    IL_0056:  ldelem.u1
    IL_0057:  ldarg.0
    IL_0058:  ldfld      uint8[] Beetle.IpItem::c
    IL_005d:  ldc.i4.2
    IL_005e:  ldelem.u1
    IL_005f:  clt
    IL_0061:  ldc.i4.0
    IL_0062:  ceq
    IL_0064:  and
    IL_0065:  brfalse.s  IL_009c

    IL_0067:  ldloc.1
    IL_0068:  ldc.i4.0
    IL_0069:  ldelem.u1
    IL_006a:  ldarg.0
    IL_006b:  ldfld      uint8[] Beetle.IpItem::d
    IL_0070:  ldc.i4.2
    IL_0071:  ldelem.u1
    IL_0072:  cgt
    IL_0074:  ldc.i4.0
    IL_0075:  ceq
    IL_0077:  ldloc.1
    IL_0078:  ldc.i4.3
    IL_0079:  ldelem.u1
    IL_007a:  ldarg.0
    IL_007b:  ldfld      uint8[] Beetle.IpItem::c
    IL_0080:  ldc.i4.3
    IL_0081:  ldelem.u1
    IL_0082:  clt
    IL_0084:  ldc.i4.0
    IL_0085:  ceq
    IL_0087:  and
    IL_0088:  brfalse.s  IL_009c

    IL_008a:  ldloc.1
    IL_008b:  ldc.i4.0
    IL_008c:  ldelem.u1
    IL_008d:  ldarg.0
    IL_008e:  ldfld      uint8[] Beetle.IpItem::d
    IL_0093:  ldc.i4.3
    IL_0094:  ldelem.u1
    IL_0095:  cgt
    IL_0097:  ldc.i4.0
    IL_0098:  ceq
    IL_009a:  br.s       IL_009d

    IL_009c:  ldc.i4.0
    IL_009d:  stloc.0
    IL_009e:  br.s       IL_00d8

    IL_00a0:  ldloc.1
    IL_00a1:  ldc.i4.0
    IL_00a2:  ldelem.u1
    IL_00a3:  ldarg.0
    IL_00a4:  ldfld      uint8[] Beetle.IpItem::c
    IL_00a9:  ldc.i4.0
    IL_00aa:  ldelem.u1
    IL_00ab:  ceq
    IL_00ad:  ldloc.1
    IL_00ae:  ldc.i4.1
    IL_00af:  ldelem.u1
    IL_00b0:  ldarg.0
    IL_00b1:  ldfld      uint8[] Beetle.IpItem::c
    IL_00b6:  ldc.i4.1
    IL_00b7:  ldelem.u1
    IL_00b8:  ceq
    IL_00ba:  and
    IL_00bb:  ldloc.1
    IL_00bc:  ldc.i4.2
    IL_00bd:  ldelem.u1
    IL_00be:  ldarg.0
    IL_00bf:  ldfld      uint8[] Beetle.IpItem::c
    IL_00c4:  ldc.i4.2
    IL_00c5:  ldelem.u1
    IL_00c6:  ceq
    IL_00c8:  and
    IL_00c9:  ldloc.1
    IL_00ca:  ldc.i4.3
    IL_00cb:  ldelem.u1
    IL_00cc:  ldarg.0
    IL_00cd:  ldfld      uint8[] Beetle.IpItem::c
    IL_00d2:  ldc.i4.3
    IL_00d3:  ldelem.u1
    IL_00d4:  ceq
    IL_00d6:  and
    IL_00d7:  stloc.0
    IL_00d8:  ldarg.0
    IL_00d9:  ldfld      bool Beetle.IpItem::a
    IL_00de:  brfalse.s  IL_00e2

    IL_00e0:  ldloc.0
    IL_00e1:  ret

    IL_00e2:  ldloc.0
    IL_00e3:  ldc.i4.0
    IL_00e4:  ceq
    IL_00e6:  ret
  } // end of method IpItem::Verification

  .method public hidebysig specialname instance string 
          get_AddressValue() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Beetle.IpItem::e
    IL_0006:  ret
  } // end of method IpItem::get_AddressValue

  .method public hidebysig static class Beetle.IpItem 
          From(string ipaddress,
               bool permit) cil managed
  {
    // 代码大小       112 (0x70)
    .maxstack  4
    .locals init (class Beetle.IpItem V_0,
             class [System]System.Net.IPAddress V_1,
             class [System]System.Net.IPAddress V_2,
             string[] V_3,
             char[] V_4)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldnull
    IL_0003:  stloc.1
    IL_0004:  ldnull
    IL_0005:  stloc.2
    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.1
    IL_0008:  newarr     [mscorlib]System.Char
    IL_000d:  stloc.s    V_4
    IL_000f:  ldloc.s    V_4
    IL_0011:  ldc.i4.0
    IL_0012:  ldc.i4.s   45
    IL_0014:  stelem.i2
    IL_0015:  ldloc.s    V_4
    IL_0017:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
    IL_001c:  stloc.3
    IL_001d:  ldloc.3
    IL_001e:  ldlen
    IL_001f:  conv.i4
    IL_0020:  ldc.i4.2
    IL_0021:  bne.un.s   IL_004d

    IL_0023:  ldloc.3
    IL_0024:  ldc.i4.0
    IL_0025:  ldelem.ref
    IL_0026:  ldloca.s   V_1
    IL_0028:  call       bool [System]System.Net.IPAddress::TryParse(string,
                                                                     class [System]System.Net.IPAddress&)
    IL_002d:  brfalse.s  IL_006e

    IL_002f:  ldloc.3
    IL_0030:  ldc.i4.1
    IL_0031:  ldelem.ref
    IL_0032:  ldloca.s   V_2
    IL_0034:  call       bool [System]System.Net.IPAddress::TryParse(string,
                                                                     class [System]System.Net.IPAddress&)
    IL_0039:  brfalse.s  IL_006e

    IL_003b:  ldloc.1
    IL_003c:  ldloc.2
    IL_003d:  ldarg.1
    IL_003e:  newobj     instance void Beetle.IpItem::.ctor(class [System]System.Net.IPAddress,
                                                            class [System]System.Net.IPAddress,
                                                            bool)
    IL_0043:  stloc.0
    IL_0044:  ldloc.0
    IL_0045:  ldarg.0
    IL_0046:  stfld      string Beetle.IpItem::e
    IL_004b:  br.s       IL_006e

    IL_004d:  ldloc.3
    IL_004e:  ldc.i4.0
    IL_004f:  ldelem.ref
    IL_0050:  ldloca.s   V_1
    IL_0052:  call       bool [System]System.Net.IPAddress::TryParse(string,
                                                                     class [System]System.Net.IPAddress&)
    IL_0057:  brfalse.s  IL_006e

    IL_0059:  ldloc.1
    IL_005a:  newobj     instance void Beetle.IpItem::.ctor(class [System]System.Net.IPAddress)
    IL_005f:  stloc.0
    IL_0060:  ldloc.0
    IL_0061:  ldarg.1
    IL_0062:  stfld      bool Beetle.IpItem::a
    IL_0067:  ldloc.0
    IL_0068:  ldarg.0
    IL_0069:  stfld      string Beetle.IpItem::e
    IL_006e:  ldloc.0
    IL_006f:  ret
  } // end of method IpItem::From

  .property instance bool Permit()
  {
    .get instance bool Beetle.IpItem::get_Permit()
  } // end of property IpItem::Permit
  .property instance string AddressValue()
  {
    .get instance string Beetle.IpItem::get_AddressValue()
  } // end of property IpItem::AddressValue
} // end of class Beetle.IpItem

.class public auto ansi beforefieldinit Beetle.MessageHandlerArgs
       extends [mscorlib]System.Object
{
  .field public object Message
  .field public bool Cancel
  .field public bool CleanMessage
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.0
    IL_0008:  stfld      bool Beetle.MessageHandlerArgs::CleanMessage
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.0
    IL_000f:  stfld      bool Beetle.MessageHandlerArgs::Cancel
    IL_0014:  ret
  } // end of method MessageHandlerArgs::.ctor

} // end of class Beetle.MessageHandlerArgs

.class interface public abstract auto ansi Beetle.IMessageHandler
{
  .method public hidebysig newslot abstract virtual 
          instance void  ProcessMessage(class Beetle.TcpChannel adapter,
                                        class Beetle.MessageHandlerArgs message) cil managed
  {
  } // end of method IMessageHandler::ProcessMessage

} // end of class Beetle.IMessageHandler

.class public auto ansi beforefieldinit Beetle.ChannelCreatingArgs
       extends [mscorlib]System.EventArgs
{
  .field private class [System]System.Net.Sockets.Socket a
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class Beetle.TcpServer b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private bool c
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.EventArgs::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.0
    IL_0008:  call       instance void Beetle.ChannelCreatingArgs::set_Cancel(bool)
    IL_000d:  ret
  } // end of method ChannelCreatingArgs::.ctor

  .method public hidebysig specialname instance class [System]System.Net.Sockets.Socket 
          get_Socket() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Net.Sockets.Socket Beetle.ChannelCreatingArgs::a
    IL_0006:  ret
  } // end of method ChannelCreatingArgs::get_Socket

  .method assembly hidebysig specialname 
          instance void  a(class [System]System.Net.Sockets.Socket A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [System]System.Net.Sockets.Socket Beetle.ChannelCreatingArgs::a
    IL_0007:  ret
  } // end of method ChannelCreatingArgs::a

  .method public hidebysig specialname instance class Beetle.TcpServer 
          get_Server() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.TcpServer Beetle.ChannelCreatingArgs::b
    IL_0006:  ret
  } // end of method ChannelCreatingArgs::get_Server

  .method assembly hidebysig specialname 
          instance void  a(class Beetle.TcpServer A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class Beetle.TcpServer Beetle.ChannelCreatingArgs::b
    IL_0007:  ret
  } // end of method ChannelCreatingArgs::a

  .method public hidebysig specialname instance bool 
          get_Cancel() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.ChannelCreatingArgs::c
    IL_0006:  ret
  } // end of method ChannelCreatingArgs::get_Cancel

  .method public hidebysig specialname instance void 
          set_Cancel(bool 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      bool Beetle.ChannelCreatingArgs::c
    IL_0007:  ret
  } // end of method ChannelCreatingArgs::set_Cancel

  .property instance class [System]System.Net.Sockets.Socket
          Socket()
  {
    .get instance class [System]System.Net.Sockets.Socket Beetle.ChannelCreatingArgs::get_Socket()
    .set instance void Beetle.ChannelCreatingArgs::a(class [System]System.Net.Sockets.Socket)
  } // end of property ChannelCreatingArgs::Socket
  .property instance class Beetle.TcpServer
          Server()
  {
    .get instance class Beetle.TcpServer Beetle.ChannelCreatingArgs::get_Server()
    .set instance void Beetle.ChannelCreatingArgs::a(class Beetle.TcpServer)
  } // end of property ChannelCreatingArgs::Server
  .property instance bool Cancel()
  {
    .set instance void Beetle.ChannelCreatingArgs::set_Cancel(bool)
    .get instance bool Beetle.ChannelCreatingArgs::get_Cancel()
  } // end of property ChannelCreatingArgs::Cancel
} // end of class Beetle.ChannelCreatingArgs

.class public auto ansi beforefieldinit Beetle.TcpServer
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field private bool a
  .field private object b
  .field private class r c
  .field private int32 d
  .field private class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> e
  .field private class Beetle.ChannelCreatingArgs f
  .field static assembly string g
  .field private class [System]System.Net.Sockets.Socket h
  .field private class w i
  .field private class [System]System.Collections.Generic.Queue`1<class r> j
  .field private class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> k
  .field private class Beetle.DetectTimeoutHandler l
  .field private class Beetle.EventChannglCreating m
  .field private bool n
  .field public bool EnabledVariant
  .field assembly class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> o
  .field private class [System]System.Net.IPEndPoint p
  .field private class Beetle.EventChannelConnected q
  .field private class Beetle.EventChannelDisposed r
  .field public static class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs> SocketAsyncEventArgsPools
  .field private bool s
  .field private object t
  .field private class [mscorlib]System.Text.Encoding u
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private bool v
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       159 (0x9f)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0006:  stfld      object Beetle.TcpServer::b
    IL_000b:  ldarg.0
    IL_000c:  ldc.i4     0xff
    IL_0011:  stfld      int32 Beetle.TcpServer::d
    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.s   100
    IL_0019:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception>::.ctor(int32)
    IL_001e:  stfld      class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> Beetle.TcpServer::e
    IL_0023:  ldarg.0
    IL_0024:  newobj     instance void Beetle.ChannelCreatingArgs::.ctor()
    IL_0029:  stfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
    IL_002e:  ldarg.0
    IL_002f:  ldc.i4     0x400
    IL_0034:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class r>::.ctor(int32)
    IL_0039:  stfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
    IL_003e:  ldarg.0
    IL_003f:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler>::.ctor()
    IL_0044:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
    IL_0049:  ldarg.0
    IL_004a:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_004f:  stfld      object Beetle.TcpServer::t
    IL_0054:  ldarg.0
    IL_0055:  call       instance void [mscorlib]System.Object::.ctor()
    IL_005a:  ldarg.0
    IL_005b:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0060:  call       instance void Beetle.TcpServer::set_Coding(class [mscorlib]System.Text.Encoding)
    IL_0065:  ldarg.0
    IL_0066:  ldsfld     int32 Beetle.TcpUtils::Connections
    IL_006b:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.TcpChannel>::.ctor(int32)
    IL_0070:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::o
    IL_0075:  ldarg.0
    IL_0076:  ldc.i4.0
    IL_0077:  call       instance void Beetle.TcpServer::set_EnabledChannelSendCompeletedEvent(bool)
    IL_007c:  ldarg.0
    IL_007d:  newobj     instance void Beetle.DetectTimeoutHandler::.ctor()
    IL_0082:  stfld      class Beetle.DetectTimeoutHandler Beetle.TcpServer::l
    IL_0087:  ldarg.0
    IL_0088:  ldfld      class Beetle.DetectTimeoutHandler Beetle.TcpServer::l
    IL_008d:  ldarg.0
    IL_008e:  callvirt   instance void Beetle.DetectTimeoutHandler::a(class Beetle.TcpServer)
    IL_0093:  ldarg.0
    IL_0094:  call       class w w::g()
    IL_0099:  stfld      class w Beetle.TcpServer::i
    IL_009e:  ret
  } // end of method TcpServer::.ctor

  .method public hidebysig specialname instance void 
          add_ChannelCreating(class Beetle.EventChannglCreating 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannglCreating V_0,
             class Beetle.EventChannglCreating V_1,
             class Beetle.EventChannglCreating V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannglCreating Beetle.TcpServer::m
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannglCreating
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannglCreating Beetle.TcpServer::m
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannglCreating>(!!0&,
                                                                                                                        !!0,
                                                                                                                        !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpServer::add_ChannelCreating

  .method public hidebysig specialname instance void 
          remove_ChannelCreating(class Beetle.EventChannglCreating 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannglCreating V_0,
             class Beetle.EventChannglCreating V_1,
             class Beetle.EventChannglCreating V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannglCreating Beetle.TcpServer::m
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannglCreating
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannglCreating Beetle.TcpServer::m
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannglCreating>(!!0&,
                                                                                                                        !!0,
                                                                                                                        !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpServer::remove_ChannelCreating

  .method public hidebysig specialname instance bool 
          get_IsRunning() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.TcpServer::n
    IL_0006:  ret
  } // end of method TcpServer::get_IsRunning

  .method public hidebysig specialname instance void 
          add_ChannelConnected(class Beetle.EventChannelConnected 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannelConnected V_0,
             class Beetle.EventChannelConnected V_1,
             class Beetle.EventChannelConnected V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelConnected Beetle.TcpServer::q
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannelConnected
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannelConnected Beetle.TcpServer::q
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannelConnected>(!!0&,
                                                                                                                         !!0,
                                                                                                                         !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpServer::add_ChannelConnected

  .method public hidebysig specialname instance void 
          remove_ChannelConnected(class Beetle.EventChannelConnected 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannelConnected V_0,
             class Beetle.EventChannelConnected V_1,
             class Beetle.EventChannelConnected V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelConnected Beetle.TcpServer::q
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannelConnected
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannelConnected Beetle.TcpServer::q
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannelConnected>(!!0&,
                                                                                                                         !!0,
                                                                                                                         !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpServer::remove_ChannelConnected

  .method public hidebysig specialname instance void 
          add_ChannelDisposed(class Beetle.EventChannelDisposed 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannelDisposed V_0,
             class Beetle.EventChannelDisposed V_1,
             class Beetle.EventChannelDisposed V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelDisposed Beetle.TcpServer::r
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannelDisposed
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannelDisposed Beetle.TcpServer::r
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannelDisposed>(!!0&,
                                                                                                                        !!0,
                                                                                                                        !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpServer::add_ChannelDisposed

  .method public hidebysig specialname instance void 
          remove_ChannelDisposed(class Beetle.EventChannelDisposed 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannelDisposed V_0,
             class Beetle.EventChannelDisposed V_1,
             class Beetle.EventChannelDisposed V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelDisposed Beetle.TcpServer::r
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannelDisposed
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannelDisposed Beetle.TcpServer::r
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannelDisposed>(!!0&,
                                                                                                                        !!0,
                                                                                                                        !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpServer::remove_ChannelDisposed

  .method public hidebysig specialname instance class Beetle.DetectTimeoutHandler 
          get_DetectHandler() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.DetectTimeoutHandler Beetle.TcpServer::l
    IL_0006:  ret
  } // end of method TcpServer::get_DetectHandler

  .method public hidebysig specialname instance int32 
          get_QueueConnects() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class r>::get_Count()
    IL_000b:  ret
  } // end of method TcpServer::get_QueueConnects

  .method private hidebysig instance void 
          b(class r A_0) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Queue`1<class r> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class r>::Enqueue(!0)
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method TcpServer::b

  .method private hidebysig instance class r 
          b() cil managed
  {
    // 代码大小       54 (0x36)
    .maxstack  2
    .locals init (class r V_0,
             class [System]System.Collections.Generic.Queue`1<class r> V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class r>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0029

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
      IL_0021:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class r>::Dequeue()
      IL_0026:  stloc.0
      IL_0027:  leave.s    IL_0034

      IL_0029:  ldnull
      IL_002a:  stloc.0
      IL_002b:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_002d:  ldloc.1
      IL_002e:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ldloc.0
    IL_0035:  ret
  } // end of method TcpServer::b

  .method public hidebysig specialname instance class [System]System.Net.Sockets.Socket 
          get_Socket() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Net.Sockets.Socket Beetle.TcpServer::h
    IL_0006:  ret
  } // end of method TcpServer::get_Socket

  .method public hidebysig instance void 
          SetFilter<.ctor (Beetle.IIPFiler) T>() cil managed
  {
    // 代码大小       81 (0x51)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> V_0,
             !!T V_1,
             !!T V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
      IL_0013:  ldtoken    !!T
      IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_001d:  ldloca.s   V_1
      IL_001f:  initobj    !!T
      IL_0025:  ldloc.1
      IL_0026:  box        !!T
      IL_002b:  brfalse.s  IL_0038

      IL_002d:  ldloca.s   V_2
      IL_002f:  initobj    !!T
      IL_0035:  ldloc.2
      IL_0036:  br.s       IL_003d

      IL_0038:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
      IL_003d:  box        !!T
      IL_0042:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler>::set_Item(!0,
                                                                                                                                                              !1)
      IL_0047:  leave.s    IL_0050

    }  // end .try
    finally
    {
      IL_0049:  ldloc.0
      IL_004a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_004f:  endfinally
    }  // end handler
    IL_0050:  ret
  } // end of method TcpServer::SetFilter

  .method public hidebysig instance !!T  GetFilter<.ctor (Beetle.IIPFiler) T>() cil managed
  {
    // 代码大小       117 (0x75)
    .maxstack  4
    .locals init (class [mscorlib]System.Type V_0,
             !!T V_1,
             class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> V_2,
             !!T V_3,
             !!T V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
    IL_0006:  dup
    IL_0007:  stloc.2
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldtoken    !!T
      IL_0012:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_0017:  stloc.0
      IL_0018:  ldarg.0
      IL_0019:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
      IL_001e:  ldloc.0
      IL_001f:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler>::ContainsKey(!0)
      IL_0024:  brtrue.s   IL_0058

      IL_0026:  ldarg.0
      IL_0027:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
      IL_002c:  ldloc.0
      IL_002d:  ldloca.s   V_3
      IL_002f:  initobj    !!T
      IL_0035:  ldloc.3
      IL_0036:  box        !!T
      IL_003b:  brfalse.s  IL_0049

      IL_003d:  ldloca.s   V_4
      IL_003f:  initobj    !!T
      IL_0045:  ldloc.s    V_4
      IL_0047:  br.s       IL_004e

      IL_0049:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
      IL_004e:  box        !!T
      IL_0053:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler>::Add(!0,
                                                                                                                                                         !1)
      IL_0058:  ldarg.0
      IL_0059:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
      IL_005e:  ldloc.0
      IL_005f:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler>::get_Item(!0)
      IL_0064:  unbox.any  !!T
      IL_0069:  stloc.1
      IL_006a:  leave.s    IL_0073

    }  // end .try
    finally
    {
      IL_006c:  ldloc.2
      IL_006d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0072:  endfinally
    }  // end handler
    IL_0073:  ldloc.1
    IL_0074:  ret
  } // end of method TcpServer::GetFilter

  .method public hidebysig specialname instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> 
          get_Clients() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::o
    IL_0006:  ret
  } // end of method TcpServer::get_Clients

  .method public hidebysig specialname instance class [mscorlib]System.Text.Encoding 
          get_Coding() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Text.Encoding Beetle.TcpServer::u
    IL_0006:  ret
  } // end of method TcpServer::get_Coding

  .method public hidebysig specialname instance void 
          set_Coding(class [mscorlib]System.Text.Encoding 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [mscorlib]System.Text.Encoding Beetle.TcpServer::u
    IL_0007:  ret
  } // end of method TcpServer::set_Coding

  .method private hidebysig instance void 
          b(class Beetle.TcpChannel A_0) cil managed
  {
    // 代码大小       36 (0x24)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object Beetle.TcpServer::b
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  call       instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::get_Clients()
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance bool class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::Remove(!0)
      IL_0019:  pop
      IL_001a:  leave.s    IL_0023

    }  // end .try
    finally
    {
      IL_001c:  ldloc.0
      IL_001d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0022:  endfinally
    }  // end handler
    IL_0023:  ret
  } // end of method TcpServer::b

  .method private hidebysig instance void 
          a(class Beetle.TcpChannel A_0) cil managed
  {
    // 代码大小       65 (0x41)
    .maxstack  4
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object Beetle.TcpServer::b
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  call       instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::get_Clients()
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::Add(!0)
      IL_0019:  ldarg.1
      IL_001a:  ldarg.1
      IL_001b:  ldfld      int32 Beetle.TcpChannel::d
      IL_0020:  ldarg.1
      IL_0021:  ldstr      "_data"
      IL_0026:  callvirt   instance object Beetle.TcpChannel::get_Item(string)
      IL_002b:  unbox.any  [mscorlib]System.Int32
      IL_0030:  cgt
      IL_0032:  stfld      bool Beetle.TcpChannel::ad
      IL_0037:  leave.s    IL_0040

    }  // end .try
    finally
    {
      IL_0039:  ldloc.0
      IL_003a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003f:  endfinally
    }  // end handler
    IL_0040:  ret
  } // end of method TcpServer::a

  .method public hidebysig instance class Beetle.TcpChannel[] 
          GetOnlines() cil managed
  {
    // 代码大小       73 (0x49)
    .maxstack  3
    .locals init (class Beetle.TcpChannel[] V_0,
             class [mscorlib]System.Exception V_1,
             class Beetle.TcpChannel[] V_2,
             object V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object Beetle.TcpServer::b
    IL_0006:  dup
    IL_0007:  stloc.3
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      .try
      {
        IL_000d:  ldarg.0
        IL_000e:  call       instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::get_Clients()
        IL_0013:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::get_Count()
        IL_0018:  newarr     Beetle.TcpChannel
        IL_001d:  stloc.0
        IL_001e:  ldarg.0
        IL_001f:  call       instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::get_Clients()
        IL_0024:  ldloc.0
        IL_0025:  ldc.i4.0
        IL_0026:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::CopyTo(!0[],
                                                                                                                                     int32)
        IL_002b:  ldloc.0
        IL_002c:  stloc.2
        IL_002d:  leave.s    IL_0047

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_002f:  stloc.1
        IL_0030:  ldarg.0
        IL_0031:  ldloc.1
        IL_0032:  call       instance void Beetle.TcpServer::a(class [mscorlib]System.Exception)
        IL_0037:  ldc.i4.0
        IL_0038:  newarr     Beetle.TcpChannel
        IL_003d:  stloc.2
        IL_003e:  leave.s    IL_0047

      }  // end handler
    }  // end .try
    finally
    {
      IL_0040:  ldloc.3
      IL_0041:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0046:  endfinally
    }  // end handler
    IL_0047:  ldloc.2
    IL_0048:  ret
  } // end of method TcpServer::GetOnlines

  .method public hidebysig instance void 
          GetOnlines(class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel> items) cil managed
  {
    // 代码大小       77 (0x4d)
    .maxstack  3
    .locals init (class [mscorlib]System.Exception V_0,
             object V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object Beetle.TcpServer::b
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      .try
      {
        IL_000d:  ldarg.0
        IL_000e:  call       instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::get_Clients()
        IL_0013:  ldarg.1
        IL_0014:  callvirt   instance !0[] class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel>::get_Array()
        IL_0019:  ldc.i4.0
        IL_001a:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::CopyTo(!0[],
                                                                                                                                     int32)
        IL_001f:  ldarg.1
        IL_0020:  ldc.i4.0
        IL_0021:  callvirt   instance void class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel>::b(int32)
        IL_0026:  ldarg.1
        IL_0027:  ldarg.0
        IL_0028:  call       instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::get_Clients()
        IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::get_Count()
        IL_0032:  callvirt   instance void class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel>::a(int32)
        IL_0037:  leave.s    IL_0043

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0039:  stloc.0
        IL_003a:  ldarg.0
        IL_003b:  ldloc.0
        IL_003c:  call       instance void Beetle.TcpServer::a(class [mscorlib]System.Exception)
        IL_0041:  leave.s    IL_0043

      }  // end handler
      IL_0043:  leave.s    IL_004c

    }  // end .try
    finally
    {
      IL_0045:  ldloc.1
      IL_0046:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_004b:  endfinally
    }  // end handler
    IL_004c:  ret
  } // end of method TcpServer::GetOnlines

  .method public hidebysig specialname instance class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> 
          get_Errors() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> Beetle.TcpServer::e
    IL_0006:  ret
  } // end of method TcpServer::get_Errors

  .method private hidebysig instance void 
          a(class [mscorlib]System.Exception A_0) cil managed
  {
    // 代码大小       62 (0x3e)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> Beetle.TcpServer::e
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> Beetle.TcpServer::e
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception>::get_Count()
      IL_0018:  ldc.i4.s   100
      IL_001a:  blt.s      IL_0028

      IL_001c:  ldarg.0
      IL_001d:  ldfld      class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> Beetle.TcpServer::e
      IL_0022:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception>::Dequeue()
      IL_0027:  pop
      IL_0028:  ldarg.0
      IL_0029:  ldfld      class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> Beetle.TcpServer::e
      IL_002e:  ldarg.1
      IL_002f:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception>::Enqueue(!0)
      IL_0034:  leave.s    IL_003d

    }  // end .try
    finally
    {
      IL_0036:  ldloc.0
      IL_0037:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003c:  endfinally
    }  // end handler
    IL_003d:  ret
  } // end of method TcpServer::a

  .method public hidebysig instance class Beetle.TcpChannel 
          FindClient(int64 id,
                     string name) cil managed
  {
    // 代码大小       64 (0x40)
    .maxstack  2
    .locals init (class Beetle.TcpChannel V_0,
             class Beetle.TcpChannel V_1,
             class Beetle.TcpChannel[] V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class Beetle.TcpChannel[] Beetle.TcpServer::GetOnlines()
    IL_0006:  stloc.2
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.3
    IL_0009:  br.s       IL_0036

    IL_000b:  ldloc.2
    IL_000c:  ldloc.3
    IL_000d:  ldelem.ref
    IL_000e:  stloc.0
    IL_000f:  ldarg.1
    IL_0010:  ldloc.0
    IL_0011:  callvirt   instance int64 Beetle.TcpChannel::get_ClientID()
    IL_0016:  beq.s      IL_002e

    IL_0018:  ldarg.2
    IL_0019:  call       bool [mscorlib]System.String::IsNullOrEmpty(string)
    IL_001e:  brtrue.s   IL_0032

    IL_0020:  ldloc.0
    IL_0021:  callvirt   instance string Beetle.TcpChannel::get_Name()
    IL_0026:  ldarg.2
    IL_0027:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002c:  brfalse.s  IL_0032

    IL_002e:  ldloc.0
    IL_002f:  stloc.1
    IL_0030:  leave.s    IL_003e

    IL_0032:  ldloc.3
    IL_0033:  ldc.i4.1
    IL_0034:  add
    IL_0035:  stloc.3
    IL_0036:  ldloc.3
    IL_0037:  ldloc.2
    IL_0038:  ldlen
    IL_0039:  conv.i4
    IL_003a:  blt.s      IL_000b

    IL_003c:  ldnull
    IL_003d:  ret

    IL_003e:  ldloc.1
    IL_003f:  ret
  } // end of method TcpServer::FindClient

  .method family hidebysig instance void 
          OnConnected(class Beetle.TcpChannel e) cil managed
  {
    // 代码大小       86 (0x56)
    .maxstack  4
    .locals init (class Beetle.ChannelDisposedEventArgs V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       instance void Beetle.TcpServer::a(class Beetle.TcpChannel)
      IL_0007:  ldarg.1
      IL_0008:  ldarg.0
      IL_0009:  callvirt   instance void Beetle.TcpChannel::a(class Beetle.TcpServer)
      IL_000e:  ldarg.1
      IL_000f:  ldarg.0
      IL_0010:  ldftn      instance void Beetle.TcpServer::OnClientDisposed(object,
                                                                            class Beetle.ChannelDisposedEventArgs)
      IL_0016:  newobj     instance void Beetle.EventChannelDisposed::.ctor(object,
                                                                            native int)
      IL_001b:  callvirt   instance void Beetle.TcpChannel::add_ChannelDisposed(class Beetle.EventChannelDisposed)
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class Beetle.EventChannelConnected Beetle.TcpServer::q
      IL_0026:  brfalse.s  IL_0049

      IL_0028:  ldarg.0
      IL_0029:  ldfld      class Beetle.EventChannelConnected Beetle.TcpServer::q
      IL_002e:  ldarg.0
      IL_002f:  newobj     instance void Beetle.ChannelDisposedEventArgs::.ctor()
      IL_0034:  stloc.0
      IL_0035:  ldloc.0
      IL_0036:  ldarg.1
      IL_0037:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_003c:  ldloc.0
      IL_003d:  ldc.i4.1
      IL_003e:  callvirt   instance void Beetle.ChannelDisposedEventArgs::set_Clean(bool)
      IL_0043:  ldloc.0
      IL_0044:  callvirt   instance void Beetle.EventChannelConnected::Invoke(object,
                                                                              class Beetle.ChannelEventArgs)
      IL_0049:  leave.s    IL_0055

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_004b:  stloc.1
      IL_004c:  ldloc.1
      IL_004d:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0052:  pop
      IL_0053:  leave.s    IL_0055

    }  // end handler
    IL_0055:  ret
  } // end of method TcpServer::OnConnected

  .method family hidebysig instance void 
          OnClientDisposed(object source,
                           class Beetle.ChannelDisposedEventArgs e) cil managed
  {
    // 代码大小       51 (0x33)
    .maxstack  3
    .try
    {
      IL_0000:  ldarg.2
      IL_0001:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_0006:  ldnull
      IL_0007:  callvirt   instance void Beetle.TcpChannel::a(class Beetle.TcpServer)
      IL_000c:  ldarg.0
      IL_000d:  ldarg.2
      IL_000e:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_0013:  call       instance void Beetle.TcpServer::b(class Beetle.TcpChannel)
      IL_0018:  ldarg.0
      IL_0019:  ldfld      class Beetle.EventChannelDisposed Beetle.TcpServer::r
      IL_001e:  brfalse.s  IL_002d

      IL_0020:  ldarg.0
      IL_0021:  ldfld      class Beetle.EventChannelDisposed Beetle.TcpServer::r
      IL_0026:  ldarg.0
      IL_0027:  ldarg.2
      IL_0028:  callvirt   instance void Beetle.EventChannelDisposed::Invoke(object,
                                                                             class Beetle.ChannelDisposedEventArgs)
      IL_002d:  leave.s    IL_0032

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_002f:  pop
      IL_0030:  leave.s    IL_0032

    }  // end handler
    IL_0032:  ret
  } // end of method TcpServer::OnClientDisposed

  .method family hidebysig instance void 
          OnChannelCreating(class Beetle.ChannelCreatingArgs e) cil managed
  {
    // 代码大小       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannglCreating Beetle.TcpServer::m
    IL_0006:  brfalse.s  IL_001a

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class Beetle.EventChannglCreating Beetle.TcpServer::m
    IL_000e:  ldarg.0
    IL_000f:  ldarg.0
    IL_0010:  ldfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
    IL_0015:  callvirt   instance void Beetle.EventChannglCreating::Invoke(object,
                                                                           class Beetle.ChannelCreatingArgs)
    IL_001a:  ldarg.1
    IL_001b:  ldarg.0
    IL_001c:  call       instance int32 Beetle.TcpServer::c()
    IL_0021:  ldarg.0
    IL_0022:  ldfld      int32 Beetle.TcpServer::d
    IL_0027:  clt
    IL_0029:  ldc.i4.0
    IL_002a:  ceq
    IL_002c:  callvirt   instance void Beetle.ChannelCreatingArgs::set_Cancel(bool)
    IL_0031:  ret
  } // end of method TcpServer::OnChannelCreating

  .method public hidebysig instance void 
          Open(class [System]System.Net.IPEndPoint ipendpoint) cil managed
  {
    // 代码大小       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   100
    IL_0004:  call       instance void Beetle.TcpServer::Open(class [System]System.Net.IPEndPoint,
                                                              int32)
    IL_0009:  ret
  } // end of method TcpServer::Open

  .method public hidebysig instance void 
          Open(class [System]System.Net.IPEndPoint ipendpoint,
               int32 listens) cil managed
  {
    // 代码大小       116 (0x74)
    .maxstack  5
    IL_0000:  ldsfld     bool Beetle.TcpUtils::a
    IL_0005:  brtrue.s   IL_000d

    IL_0007:  call       class Beetle.NetTcpException Beetle.NetTcpException::NotInitialize()
    IL_000c:  throw

    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  stfld      class [System]System.Net.IPEndPoint Beetle.TcpServer::p
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class [System]System.Net.Sockets.Socket Beetle.TcpServer::h
    IL_001a:  brfalse.s  IL_0022

    IL_001c:  ldarg.0
    IL_001d:  call       instance void Beetle.TcpServer::Dispose()
    IL_0022:  ldarg.0
    IL_0023:  ldc.i4.2
    IL_0024:  ldc.i4.1
    IL_0025:  ldc.i4.6
    IL_0026:  newobj     instance void [System]System.Net.Sockets.Socket::.ctor(valuetype [System]System.Net.Sockets.AddressFamily,
                                                                                valuetype [System]System.Net.Sockets.SocketType,
                                                                                valuetype [System]System.Net.Sockets.ProtocolType)
    IL_002b:  stfld      class [System]System.Net.Sockets.Socket Beetle.TcpServer::h
    IL_0030:  ldarg.0
    IL_0031:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpServer::get_Socket()
    IL_0036:  ldarg.1
    IL_0037:  callvirt   instance void [System]System.Net.Sockets.Socket::Bind(class [System]System.Net.EndPoint)
    IL_003c:  ldarg.0
    IL_003d:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpServer::get_Socket()
    IL_0042:  ldarg.2
    IL_0043:  callvirt   instance void [System]System.Net.Sockets.Socket::Listen(int32)
    IL_0048:  ldarg.0
    IL_0049:  ldc.i4.1
    IL_004a:  stfld      bool Beetle.TcpServer::n
    IL_004f:  ldarg.0
    IL_0050:  ldftn      instance void Beetle.TcpServer::b(object)
    IL_0056:  newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                      native int)
    IL_005b:  call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback)
    IL_0060:  pop
    IL_0061:  ldarg.0
    IL_0062:  ldftn      instance void Beetle.TcpServer::a(object)
    IL_0068:  newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                      native int)
    IL_006d:  call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback)
    IL_0072:  pop
    IL_0073:  ret
  } // end of method TcpServer::Open

  .method public hidebysig specialname instance bool 
          get_EnabledChannelSendCompeletedEvent() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.TcpServer::v
    IL_0006:  ret
  } // end of method TcpServer::get_EnabledChannelSendCompeletedEvent

  .method public hidebysig specialname instance void 
          set_EnabledChannelSendCompeletedEvent(bool 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      bool Beetle.TcpServer::v
    IL_0007:  ret
  } // end of method TcpServer::set_EnabledChannelSendCompeletedEvent

  .method private hidebysig instance void 
          a(class [System]System.Net.Sockets.Socket A_0) cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       void Beetle.TcpServer::ReleaseSocket(class [System]System.Net.Sockets.Socket)
    IL_0006:  ret
  } // end of method TcpServer::a

  .method private hidebysig instance void 
          b(object A_0) cil managed
  {
    // 代码大小       223 (0xdf)
    .maxstack  5
    .locals init (class [mscorlib]System.Exception V_0)
    IL_0000:  br         IL_00d3

    .try
    {
      IL_0005:  ldarg.0
      IL_0006:  ldarg.0
      IL_0007:  call       instance class r Beetle.TcpServer::b()
      IL_000c:  stfld      class r Beetle.TcpServer::c
      IL_0011:  ldarg.0
      IL_0012:  ldfld      class r Beetle.TcpServer::c
      IL_0017:  brfalse    IL_00c0

      IL_001c:  ldarg.0
      IL_001d:  ldfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
      IL_0022:  ldc.i4.0
      IL_0023:  callvirt   instance void Beetle.ChannelCreatingArgs::set_Cancel(bool)
      IL_0028:  ldarg.0
      IL_0029:  ldfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
      IL_002e:  ldarg.0
      IL_002f:  ldfld      class r Beetle.TcpServer::c
      IL_0034:  ldfld      class [System]System.Net.Sockets.Socket r::b
      IL_0039:  callvirt   instance void Beetle.ChannelCreatingArgs::a(class [System]System.Net.Sockets.Socket)
      IL_003e:  ldarg.0
      IL_003f:  ldfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
      IL_0044:  ldarg.0
      IL_0045:  callvirt   instance void Beetle.ChannelCreatingArgs::a(class Beetle.TcpServer)
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class w Beetle.TcpServer::i
      IL_0050:  ldarg.0
      IL_0051:  ldftn      instance void Beetle.TcpServer::a(class r)
      IL_0057:  newobj     instance void class [mscorlib]System.Action`1<class r>::.ctor(object,
                                                                                         native int)
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class r Beetle.TcpServer::c
      IL_0062:  ldarg.0
      IL_0063:  call       instance int32 Beetle.TcpServer::c()
      IL_0068:  ldarg.0
      IL_0069:  ldflda     int32 Beetle.TcpServer::d
      IL_006e:  callvirt   instance bool w::a(class [mscorlib]System.Action`1<class r>,
                                              class r,
                                              int32,
                                              int32&)
      IL_0073:  brtrue.s   IL_0086

      IL_0075:  ldarg.0
      IL_0076:  ldarg.0
      IL_0077:  ldfld      class r Beetle.TcpServer::c
      IL_007c:  ldfld      class [System]System.Net.Sockets.Socket r::b
      IL_0081:  call       instance void Beetle.TcpServer::a(class [System]System.Net.Sockets.Socket)
      IL_0086:  ldarg.0
      IL_0087:  ldarg.0
      IL_0088:  ldfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
      IL_008d:  call       instance void Beetle.TcpServer::OnChannelCreating(class Beetle.ChannelCreatingArgs)
      IL_0092:  ldarg.0
      IL_0093:  ldfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
      IL_0098:  callvirt   instance bool Beetle.ChannelCreatingArgs::get_Cancel()
      IL_009d:  brtrue.s   IL_00ad

      IL_009f:  ldarg.0
      IL_00a0:  ldarg.0
      IL_00a1:  ldfld      class r Beetle.TcpServer::c
      IL_00a6:  call       instance void Beetle.TcpServer::a(class r)
      IL_00ab:  br.s       IL_00c7

      IL_00ad:  ldarg.0
      IL_00ae:  ldarg.0
      IL_00af:  ldfld      class r Beetle.TcpServer::c
      IL_00b4:  ldfld      class [System]System.Net.Sockets.Socket r::b
      IL_00b9:  call       instance void Beetle.TcpServer::a(class [System]System.Net.Sockets.Socket)
      IL_00be:  br.s       IL_00c7

      IL_00c0:  ldc.i4.s   10
      IL_00c2:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)
      IL_00c7:  leave.s    IL_00d3

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_00c9:  stloc.0
      IL_00ca:  ldarg.0
      IL_00cb:  ldloc.0
      IL_00cc:  call       instance void Beetle.TcpServer::a(class [mscorlib]System.Exception)
      IL_00d1:  leave.s    IL_00d3

    }  // end handler
    IL_00d3:  ldarg.0
    IL_00d4:  ldfld      bool Beetle.TcpServer::n
    IL_00d9:  brtrue     IL_0005

    IL_00de:  ret
  } // end of method TcpServer::b

  .method private hidebysig instance bool 
          a() cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::o
    IL_0006:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::get_Count()
    IL_000b:  ldsfld     int32 l::a
    IL_0010:  clt
    IL_0012:  ret
  } // end of method TcpServer::a

  .method private hidebysig instance void 
          a(object A_0) cil managed
  {
    // 代码大小       130 (0x82)
    .maxstack  4
    .locals init (bool V_0,
             class [System]System.Net.Sockets.SocketAsyncEventArgs V_1,
             class [System]System.Net.Sockets.SocketException V_2,
             class [mscorlib]System.ObjectDisposedException V_3,
             class [mscorlib]System.Exception V_4)
    .try
    {
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_0058

      IL_0004:  ldsfld     class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs> Beetle.TcpServer::SocketAsyncEventArgsPools
      IL_0009:  callvirt   instance !0 class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::Pop()
      IL_000e:  stloc.1
      IL_000f:  ldloc.1
      IL_0010:  ldarg.0
      IL_0011:  ldftn      instance void Beetle.TcpServer::b(object,
                                                             class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_0017:  newobj     instance void class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::.ctor(object,
                                                                                                                                             native int)
      IL_001c:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::add_Completed(class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>)
      .try
      {
        IL_0021:  ldarg.0
        IL_0022:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpServer::get_Socket()
        IL_0027:  ldloc.1
        IL_0028:  callvirt   instance bool [System]System.Net.Sockets.Socket::AcceptAsync(class [System]System.Net.Sockets.SocketAsyncEventArgs)
        IL_002d:  ldc.i4.0
        IL_002e:  ceq
        IL_0030:  stloc.0
        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.1
        IL_0033:  stfld      bool Beetle.TcpServer::a
        IL_0038:  leave.s    IL_004e

      }  // end .try
      catch [System]System.Net.Sockets.SocketException 
      {
        IL_003a:  stloc.2
        IL_003b:  ldarg.0
        IL_003c:  ldloc.2
        IL_003d:  call       instance void Beetle.TcpServer::a(class [mscorlib]System.Exception)
        IL_0042:  leave.s    IL_0073

      }  // end handler
      catch [mscorlib]System.ObjectDisposedException 
      {
        IL_0044:  stloc.3
        IL_0045:  ldarg.0
        IL_0046:  ldloc.3
        IL_0047:  call       instance void Beetle.TcpServer::a(class [mscorlib]System.Exception)
        IL_004c:  leave.s    IL_0073

      }  // end handler
      IL_004e:  ldloc.0
      IL_004f:  brfalse.s  IL_0058

      IL_0051:  ldarg.0
      IL_0052:  ldloc.1
      IL_0053:  call       instance void Beetle.TcpServer::a(class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_0058:  ldarg.0
      IL_0059:  ldfld      bool Beetle.TcpServer::n
      IL_005e:  brfalse.s  IL_0073

      IL_0060:  ldloc.0
      IL_0061:  brfalse.s  IL_0073

      IL_0063:  ldarg.0
      IL_0064:  ldfld      bool Beetle.TcpServer::s
      IL_0069:  brtrue.s   IL_0073

      IL_006b:  ldarg.0
      IL_006c:  call       instance bool Beetle.TcpServer::a()
      IL_0071:  brtrue.s   IL_0004

      IL_0073:  leave.s    IL_0081

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0075:  stloc.s    V_4
      IL_0077:  ldarg.0
      IL_0078:  ldloc.s    V_4
      IL_007a:  call       instance void Beetle.TcpServer::a(class [mscorlib]System.Exception)
      IL_007f:  leave.s    IL_0081

    }  // end handler
    IL_0081:  ret
  } // end of method TcpServer::a

  .method private hidebysig instance void 
          b(object A_0,
            class [System]System.Net.Sockets.SocketAsyncEventArgs A_1) cil managed
  {
    // 代码大小       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.2
    IL_0002:  call       instance void Beetle.TcpServer::a(class [System]System.Net.Sockets.SocketAsyncEventArgs)
    IL_0007:  ldarg.0
    IL_0008:  ldnull
    IL_0009:  call       instance void Beetle.TcpServer::a(object)
    IL_000e:  ret
  } // end of method TcpServer::b

  .method private hidebysig instance void 
          a(class [System]System.Net.Sockets.SocketAsyncEventArgs A_0) cil managed
  {
    // 代码大小       113 (0x71)
    .maxstack  4
    .locals init (class r V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      .try
      {
        IL_0000:  ldarg.1
        IL_0001:  callvirt   instance valuetype [System]System.Net.Sockets.SocketError [System]System.Net.Sockets.SocketAsyncEventArgs::get_SocketError()
        IL_0006:  brtrue.s   IL_002b

        IL_0008:  ldarg.0
        IL_0009:  ldfld      bool Beetle.TcpServer::s
        IL_000e:  brtrue.s   IL_002b

        IL_0010:  newobj     instance void r::.ctor()
        IL_0015:  stloc.0
        IL_0016:  ldloc.0
        IL_0017:  ldarg.1
        IL_0018:  callvirt   instance class [System]System.Net.Sockets.Socket [System]System.Net.Sockets.SocketAsyncEventArgs::get_AcceptSocket()
        IL_001d:  stfld      class [System]System.Net.Sockets.Socket r::b
        IL_0022:  ldarg.0
        IL_0023:  ldloc.0
        IL_0024:  call       instance void Beetle.TcpServer::b(class r)
        IL_0029:  br.s       IL_0036

        IL_002b:  ldarg.1
        IL_002c:  callvirt   instance class [System]System.Net.Sockets.Socket [System]System.Net.Sockets.SocketAsyncEventArgs::get_AcceptSocket()
        IL_0031:  call       void Beetle.TcpServer::ReleaseSocket(class [System]System.Net.Sockets.Socket)
        IL_0036:  ldarg.1
        IL_0037:  ldnull
        IL_0038:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::set_AcceptSocket(class [System]System.Net.Sockets.Socket)
        IL_003d:  leave.s    IL_0049

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_003f:  stloc.1
        IL_0040:  ldarg.0
        IL_0041:  ldloc.1
        IL_0042:  call       instance void Beetle.TcpServer::a(class [mscorlib]System.Exception)
        IL_0047:  leave.s    IL_0049

      }  // end handler
      IL_0049:  leave.s    IL_0070

    }  // end .try
    finally
    {
      IL_004b:  ldarg.0
      IL_004c:  ldc.i4.0
      IL_004d:  stfld      bool Beetle.TcpServer::a
      IL_0052:  ldarg.1
      IL_0053:  ldarg.0
      IL_0054:  ldftn      instance void Beetle.TcpServer::b(object,
                                                             class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_005a:  newobj     instance void class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::.ctor(object,
                                                                                                                                             native int)
      IL_005f:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::remove_Completed(class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>)
      IL_0064:  ldsfld     class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs> Beetle.TcpServer::SocketAsyncEventArgsPools
      IL_0069:  ldarg.1
      IL_006a:  callvirt   instance void class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::Push(!0)
      IL_006f:  endfinally
    }  // end handler
    IL_0070:  ret
  } // end of method TcpServer::a

  .method private hidebysig instance void 
          a(class r A_0) cil managed
  {
    // 代码大小       175 (0xaf)
    .maxstack  2
    .locals init (class Beetle.TcpChannel V_0,
             bool V_1,
             valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<class [mscorlib]System.Type,class Beetle.IIPFiler> V_2,
             valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<class [mscorlib]System.Type,class Beetle.IIPFiler> V_3)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  ldarg.0
    IL_0005:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
    IL_000a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler>::get_Count()
    IL_000f:  ldc.i4.0
    IL_0010:  ble.s      IL_0061

    IL_0012:  ldarg.0
    IL_0013:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler> Beetle.TcpServer::k
    IL_0018:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<class [mscorlib]System.Type,class Beetle.IIPFiler>::GetEnumerator()
    IL_001d:  stloc.3
    .try
    {
      IL_001e:  br.s       IL_0048

      IL_0020:  ldloca.s   V_3
      IL_0022:  call       instance valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<!0,!1> valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<class [mscorlib]System.Type,class Beetle.IIPFiler>::get_Current()
      IL_0027:  stloc.2
      IL_0028:  ldloca.s   V_2
      IL_002a:  call       instance !1 valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<class [mscorlib]System.Type,class Beetle.IIPFiler>::get_Value()
      IL_002f:  ldarg.1
      IL_0030:  ldfld      class [System]System.Net.Sockets.Socket r::b
      IL_0035:  callvirt   instance class [System]System.Net.EndPoint [System]System.Net.Sockets.Socket::get_RemoteEndPoint()
      IL_003a:  castclass  [System]System.Net.IPEndPoint
      IL_003f:  callvirt   instance bool Beetle.IIPFiler::Execute(class [System]System.Net.IPEndPoint)
      IL_0044:  stloc.1
      IL_0045:  ldloc.1
      IL_0046:  brfalse.s  IL_0051

      IL_0048:  ldloca.s   V_3
      IL_004a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<class [mscorlib]System.Type,class Beetle.IIPFiler>::MoveNext()
      IL_004f:  brtrue.s   IL_0020

      IL_0051:  leave.s    IL_0061

    }  // end .try
    finally
    {
      IL_0053:  ldloca.s   V_3
      IL_0055:  constrained. valuetype [mscorlib]System.Collections.Generic.Dictionary`2/Enumerator<class [mscorlib]System.Type,class Beetle.IIPFiler>
      IL_005b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0060:  endfinally
    }  // end handler
    IL_0061:  ldloc.1
    IL_0062:  brfalse.s  IL_0097

    IL_0064:  ldarg.1
    IL_0065:  ldfld      class [System]System.Net.Sockets.Socket r::b
    IL_006a:  newobj     instance void Beetle.TcpChannel::.ctor(class [System]System.Net.Sockets.Socket)
    IL_006f:  stloc.0
    IL_0070:  ldloc.0
    IL_0071:  ldarg.0
    IL_0072:  ldfld      bool Beetle.TcpServer::EnabledVariant
    IL_0077:  stfld      bool Beetle.TcpChannel::EnabledVariant
    IL_007c:  ldarg.0
    IL_007d:  ldloc.0
    IL_007e:  call       instance void Beetle.TcpServer::c(class Beetle.TcpChannel)
    IL_0083:  ldloc.0
    IL_0084:  ldarg.0
    IL_0085:  call       instance bool Beetle.TcpServer::get_EnabledChannelSendCompeletedEvent()
    IL_008a:  stfld      bool Beetle.TcpChannel::EnabledSendCompeletedEvent
    IL_008f:  ldarg.0
    IL_0090:  ldloc.0
    IL_0091:  call       instance void Beetle.TcpServer::OnConnected(class Beetle.TcpChannel)
    IL_0096:  ret

    IL_0097:  ldarg.1
    IL_0098:  ldfld      class [System]System.Net.Sockets.Socket r::b
    IL_009d:  ldc.i4.2
    IL_009e:  callvirt   instance void [System]System.Net.Sockets.Socket::Shutdown(valuetype [System]System.Net.Sockets.SocketShutdown)
    IL_00a3:  ldarg.1
    IL_00a4:  ldfld      class [System]System.Net.Sockets.Socket r::b
    IL_00a9:  callvirt   instance void [System]System.Net.Sockets.Socket::Close()
    IL_00ae:  ret
  } // end of method TcpServer::a

  .method public hidebysig static void  ReleaseSocket(class [System]System.Net.Sockets.Socket socket) cil managed
  {
    // 代码大小       50 (0x32)
    .maxstack  2
    .locals init (class [System]System.Net.Sockets.Socket V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0031

    IL_0003:  ldarg.0
    IL_0004:  dup
    IL_0005:  stloc.0
    IL_0006:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      .try
      {
        IL_000b:  ldarg.0
        IL_000c:  brfalse.s  IL_0015

        IL_000e:  ldarg.0
        IL_000f:  ldc.i4.2
        IL_0010:  callvirt   instance void [System]System.Net.Sockets.Socket::Shutdown(valuetype [System]System.Net.Sockets.SocketShutdown)
        IL_0015:  leave.s    IL_001a

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0017:  pop
        IL_0018:  leave.s    IL_001a

      }  // end handler
      .try
      {
        IL_001a:  ldarg.0
        IL_001b:  brfalse.s  IL_0023

        IL_001d:  ldarg.0
        IL_001e:  callvirt   instance void [System]System.Net.Sockets.Socket::Close()
        IL_0023:  leave.s    IL_0028

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0025:  pop
        IL_0026:  leave.s    IL_0028

      }  // end handler
      IL_0028:  leave.s    IL_0031

    }  // end .try
    finally
    {
      IL_002a:  ldloc.0
      IL_002b:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0030:  endfinally
    }  // end handler
    IL_0031:  ret
  } // end of method TcpServer::ReleaseSocket

  .method public hidebysig instance void 
          Open(int32 port) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldsfld     class [System]System.Net.IPAddress [System]System.Net.IPAddress::Any
    IL_0006:  ldarg.1
    IL_0007:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_000c:  call       instance void Beetle.TcpServer::Open(class [System]System.Net.IPEndPoint)
    IL_0011:  ret
  } // end of method TcpServer::Open

  .method public hidebysig instance void 
          Open(string ip,
               int32 port) cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.s   100
    IL_0005:  call       instance void Beetle.TcpServer::Open(string,
                                                              int32,
                                                              int32)
    IL_000a:  ret
  } // end of method TcpServer::Open

  .method public hidebysig instance void 
          Open(string ip,
               int32 port,
               int32 listens) cil managed
  {
    // 代码大小       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       class [System]System.Net.IPAddress [System]System.Net.IPAddress::Parse(string)
    IL_0007:  ldarg.2
    IL_0008:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_000d:  ldarg.3
    IL_000e:  call       instance void Beetle.TcpServer::Open(class [System]System.Net.IPEndPoint,
                                                              int32)
    IL_0013:  ret
  } // end of method TcpServer::Open

  .method public hidebysig static void  SetKeepAliveValues(class [System]System.Net.Sockets.Socket socket,
                                                           uint32 first,
                                                           uint32 interval) cil managed
  {
    // 代码大小       96 (0x60)
    .maxstack  4
    .locals init (uint8[] V_0)
    IL_0000:  ldarg.1
    IL_0001:  box        [mscorlib]System.UInt32
    IL_0006:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::SizeOf(object)
    IL_000b:  ldc.i4.3
    IL_000c:  mul
    IL_000d:  newarr     [mscorlib]System.Byte
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.1
    IL_0014:  call       uint8[] [mscorlib]System.BitConverter::GetBytes(uint32)
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void [mscorlib]System.Array::CopyTo(class [mscorlib]System.Array,
                                                                      int32)
    IL_0020:  ldarg.1
    IL_0021:  call       uint8[] [mscorlib]System.BitConverter::GetBytes(uint32)
    IL_0026:  ldloc.0
    IL_0027:  ldarg.1
    IL_0028:  box        [mscorlib]System.UInt32
    IL_002d:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::SizeOf(object)
    IL_0032:  callvirt   instance void [mscorlib]System.Array::CopyTo(class [mscorlib]System.Array,
                                                                      int32)
    IL_0037:  ldarg.2
    IL_0038:  call       uint8[] [mscorlib]System.BitConverter::GetBytes(uint32)
    IL_003d:  ldloc.0
    IL_003e:  ldarg.1
    IL_003f:  box        [mscorlib]System.UInt32
    IL_0044:  call       int32 [mscorlib]System.Runtime.InteropServices.Marshal::SizeOf(object)
    IL_0049:  ldc.i4.2
    IL_004a:  mul
    IL_004b:  callvirt   instance void [mscorlib]System.Array::CopyTo(class [mscorlib]System.Array,
                                                                      int32)
    IL_0050:  ldarg.0
    IL_0051:  ldc.i4     0x98000004
    IL_0056:  conv.u8
    IL_0057:  ldloc.0
    IL_0058:  ldnull
    IL_0059:  callvirt   instance int32 [System]System.Net.Sockets.Socket::IOControl(valuetype [System]System.Net.Sockets.IOControlCode,
                                                                                     uint8[],
                                                                                     uint8[])
    IL_005e:  pop
    IL_005f:  ret
  } // end of method TcpServer::SetKeepAliveValues

  .method public hidebysig static class Beetle.TcpChannel 
          CreateClient(string ip,
                       int32 port) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [System]System.Net.IPAddress [System]System.Net.IPAddress::Parse(string)
    IL_0006:  ldarg.1
    IL_0007:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_000c:  call       class Beetle.TcpChannel Beetle.TcpServer::CreateClient(class [System]System.Net.IPEndPoint)
    IL_0011:  ret
  } // end of method TcpServer::CreateClient

  .method public hidebysig static class Beetle.TcpChannel 
          CreateClient(class [System]System.Net.IPAddress ip,
                       int32 port) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_0007:  call       class Beetle.TcpChannel Beetle.TcpServer::CreateClient(class [System]System.Net.IPEndPoint)
    IL_000c:  ret
  } // end of method TcpServer::CreateClient

  .method public hidebysig static class Beetle.TcpChannel 
          CreateClient(class [System]System.Net.IPEndPoint endpoint) cil managed
  {
    // 代码大小       25 (0x19)
    .maxstack  4
    .locals init (class [System]System.Net.Sockets.Socket V_0,
             class Beetle.TcpChannel V_1)
    IL_0000:  ldc.i4.2
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.6
    IL_0003:  newobj     instance void [System]System.Net.Sockets.Socket::.ctor(valuetype [System]System.Net.Sockets.AddressFamily,
                                                                                valuetype [System]System.Net.Sockets.SocketType,
                                                                                valuetype [System]System.Net.Sockets.ProtocolType)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  ldarg.0
    IL_000b:  callvirt   instance void [System]System.Net.Sockets.Socket::Connect(class [System]System.Net.EndPoint)
    IL_0010:  ldloc.0
    IL_0011:  newobj     instance void Beetle.TcpChannel::.ctor(class [System]System.Net.Sockets.Socket)
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  ret
  } // end of method TcpServer::CreateClient

  .method public hidebysig static class Beetle.TcpChannel 
          CreateClient<(Beetle.Package) T>(string ip,
                                           int32 port,
                                           class Beetle.EventPacketRecievMessage receive) cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [System]System.Net.IPAddress [System]System.Net.IPAddress::Parse(string)
    IL_0006:  ldarg.1
    IL_0007:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_000c:  ldarg.2
    IL_000d:  call       class Beetle.TcpChannel Beetle.TcpServer::CreateClient<!!0>(class [System]System.Net.IPEndPoint,
                                                                                     class Beetle.EventPacketRecievMessage)
    IL_0012:  ret
  } // end of method TcpServer::CreateClient

  .method public hidebysig static class Beetle.TcpChannel 
          CreateClient<(Beetle.Package) T>(class [System]System.Net.IPAddress ip,
                                           int32 port,
                                           class Beetle.EventPacketRecievMessage receive) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_0007:  ldarg.2
    IL_0008:  call       class Beetle.TcpChannel Beetle.TcpServer::CreateClient<!!0>(class [System]System.Net.IPEndPoint,
                                                                                     class Beetle.EventPacketRecievMessage)
    IL_000d:  ret
  } // end of method TcpServer::CreateClient

  .method public hidebysig static class Beetle.TcpChannel 
          CreateClient<(Beetle.Package) T>(class [System]System.Net.IPEndPoint endpoint,
                                           class Beetle.EventPacketRecievMessage receive) cil managed
  {
    // 代码大小       50 (0x32)
    .maxstack  4
    .locals init (class [System]System.Net.Sockets.Socket V_0,
             class Beetle.TcpChannel V_1)
    IL_0000:  ldsfld     bool Beetle.TcpUtils::a
    IL_0005:  brtrue.s   IL_000d

    IL_0007:  call       class Beetle.NetTcpException Beetle.NetTcpException::NotInitialize()
    IL_000c:  throw

    IL_000d:  ldc.i4.2
    IL_000e:  ldc.i4.1
    IL_000f:  ldc.i4.6
    IL_0010:  newobj     instance void [System]System.Net.Sockets.Socket::.ctor(valuetype [System]System.Net.Sockets.AddressFamily,
                                                                                valuetype [System]System.Net.Sockets.SocketType,
                                                                                valuetype [System]System.Net.Sockets.ProtocolType)
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldarg.0
    IL_0018:  callvirt   instance void [System]System.Net.Sockets.Socket::Connect(class [System]System.Net.EndPoint)
    IL_001d:  ldloc.0
    IL_001e:  newobj     instance void Beetle.TcpChannel::.ctor(class [System]System.Net.Sockets.Socket)
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  callvirt   instance class Beetle.Package Beetle.TcpChannel::SetPackage<!!0>()
    IL_002a:  ldarg.1
    IL_002b:  stfld      class Beetle.EventPacketRecievMessage Beetle.Package::ReceiveMessage
    IL_0030:  ldloc.1
    IL_0031:  ret
  } // end of method TcpServer::CreateClient

  .method public hidebysig static void  CreateClientAsync(string ip,
                                                          int32 port,
                                                          class [mscorlib]System.Action`1<class Beetle.CreateChannelArgs> result) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [System]System.Net.IPAddress [System]System.Net.IPAddress::Parse(string)
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  call       void Beetle.TcpServer::CreateClientAsync(class [System]System.Net.IPAddress,
                                                                  int32,
                                                                  class [mscorlib]System.Action`1<class Beetle.CreateChannelArgs>)
    IL_000d:  ret
  } // end of method TcpServer::CreateClientAsync

  .method public hidebysig static void  CreateClientAsync(class [System]System.Net.IPAddress ip,
                                                          int32 port,
                                                          class [mscorlib]System.Action`1<class Beetle.CreateChannelArgs> result) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_0007:  ldarg.2
    IL_0008:  call       void Beetle.TcpServer::CreateClientAsync(class [System]System.Net.IPEndPoint,
                                                                  class [mscorlib]System.Action`1<class Beetle.CreateChannelArgs>)
    IL_000d:  ret
  } // end of method TcpServer::CreateClientAsync

  .method public hidebysig static void  CreateClientAsync(class [System]System.Net.IPEndPoint endpoint,
                                                          class [mscorlib]System.Action`1<class Beetle.CreateChannelArgs> result) cil managed
  {
    // 代码大小       123 (0x7b)
    .maxstack  4
    .locals init (class [System]System.Net.Sockets.SocketAsyncEventArgs V_0,
             class [System]System.Net.Sockets.Socket V_1,
             class [mscorlib]System.Exception V_2)
    IL_0000:  ldsfld     bool Beetle.TcpUtils::a
    IL_0005:  brtrue.s   IL_000d

    IL_0007:  call       class Beetle.NetTcpException Beetle.NetTcpException::NotInitialize()
    IL_000c:  throw

    IL_000d:  ldsfld     class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs> Beetle.TcpServer::SocketAsyncEventArgsPools
    IL_0012:  callvirt   instance !0 class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::Pop()
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  ldnull
    IL_001a:  ldftn      void Beetle.TcpServer::a(object,
                                                  class [System]System.Net.Sockets.SocketAsyncEventArgs)
    IL_0020:  newobj     instance void class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::.ctor(object,
                                                                                                                                           native int)
    IL_0025:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::add_Completed(class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>)
    .try
    {
      IL_002a:  ldloc.0
      IL_002b:  ldarg.1
      IL_002c:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::set_UserToken(object)
      IL_0031:  ldc.i4.2
      IL_0032:  ldc.i4.1
      IL_0033:  ldc.i4.6
      IL_0034:  newobj     instance void [System]System.Net.Sockets.Socket::.ctor(valuetype [System]System.Net.Sockets.AddressFamily,
                                                                                  valuetype [System]System.Net.Sockets.SocketType,
                                                                                  valuetype [System]System.Net.Sockets.ProtocolType)
      IL_0039:  stloc.1
      IL_003a:  ldloc.0
      IL_003b:  ldloc.1
      IL_003c:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::set_AcceptSocket(class [System]System.Net.Sockets.Socket)
      IL_0041:  ldloc.0
      IL_0042:  ldarg.0
      IL_0043:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::set_RemoteEndPoint(class [System]System.Net.EndPoint)
      IL_0048:  ldloc.1
      IL_0049:  ldloc.0
      IL_004a:  callvirt   instance bool [System]System.Net.Sockets.Socket::ConnectAsync(class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_004f:  brtrue.s   IL_0058

      IL_0051:  ldnull
      IL_0052:  ldloc.0
      IL_0053:  call       void Beetle.TcpServer::a(object,
                                                    class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_0058:  leave.s    IL_007a

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_005a:  stloc.2
      IL_005b:  ldloc.0
      IL_005c:  ldnull
      IL_005d:  ldftn      void Beetle.TcpServer::a(object,
                                                    class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_0063:  newobj     instance void class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::.ctor(object,
                                                                                                                                             native int)
      IL_0068:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::remove_Completed(class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>)
      IL_006d:  ldsfld     class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs> Beetle.TcpServer::SocketAsyncEventArgsPools
      IL_0072:  ldloc.0
      IL_0073:  callvirt   instance void class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::Push(!0)
      IL_0078:  ldloc.2
      IL_0079:  throw

    }  // end handler
    IL_007a:  ret
  } // end of method TcpServer::CreateClientAsync

  .method public hidebysig instance void 
          Pause() cil managed
  {
    // 代码大小       38 (0x26)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object Beetle.TcpServer::b
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      bool Beetle.TcpServer::s
      IL_0013:  brtrue.s   IL_001c

      IL_0015:  ldarg.0
      IL_0016:  ldc.i4.1
      IL_0017:  stfld      bool Beetle.TcpServer::s
      IL_001c:  leave.s    IL_0025

    }  // end .try
    finally
    {
      IL_001e:  ldloc.0
      IL_001f:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0024:  endfinally
    }  // end handler
    IL_0025:  ret
  } // end of method TcpServer::Pause

  .method public hidebysig instance void 
          Resume() cil managed
  {
    // 代码大小       64 (0x40)
    .maxstack  3
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object Beetle.TcpServer::b
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      bool Beetle.TcpServer::s
      IL_0013:  brfalse.s  IL_0036

      IL_0015:  ldarg.0
      IL_0016:  ldc.i4.0
      IL_0017:  stfld      bool Beetle.TcpServer::s
      IL_001c:  ldarg.0
      IL_001d:  ldfld      bool Beetle.TcpServer::a
      IL_0022:  brtrue.s   IL_0036

      IL_0024:  ldarg.0
      IL_0025:  ldftn      instance void Beetle.TcpServer::a(object)
      IL_002b:  newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                        native int)
      IL_0030:  call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback)
      IL_0035:  pop
      IL_0036:  leave.s    IL_003f

    }  // end .try
    finally
    {
      IL_0038:  ldloc.0
      IL_0039:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003e:  endfinally
    }  // end handler
    IL_003f:  ret
  } // end of method TcpServer::Resume

  .method assembly hidebysig instance int32 
          c() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::get_Clients()
    IL_0006:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::get_Count()
    IL_000b:  ret
  } // end of method TcpServer::c

  .method private hidebysig static void  a(object A_0,
                                           class [System]System.Net.Sockets.SocketAsyncEventArgs A_1) cil managed
  {
    // 代码大小       100 (0x64)
    .maxstack  4
    .locals init (class Beetle.CreateChannelArgs V_0,
             class [mscorlib]System.Action`1<class Beetle.CreateChannelArgs> V_1)
    .try
    {
      .try
      {
        IL_0000:  newobj     instance void Beetle.CreateChannelArgs::.ctor()
        IL_0005:  stloc.0
        IL_0006:  ldloc.0
        IL_0007:  ldarg.1
        IL_0008:  callvirt   instance valuetype [System]System.Net.Sockets.SocketError [System]System.Net.Sockets.SocketAsyncEventArgs::get_SocketError()
        IL_000d:  stfld      valuetype [System]System.Net.Sockets.SocketError Beetle.CreateChannelArgs::Error
        IL_0012:  ldarg.1
        IL_0013:  callvirt   instance valuetype [System]System.Net.Sockets.SocketError [System]System.Net.Sockets.SocketAsyncEventArgs::get_SocketError()
        IL_0018:  brtrue.s   IL_002b

        IL_001a:  ldloc.0
        IL_001b:  ldarg.1
        IL_001c:  callvirt   instance class [System]System.Net.Sockets.Socket [System]System.Net.Sockets.SocketAsyncEventArgs::get_AcceptSocket()
        IL_0021:  newobj     instance void Beetle.TcpChannel::.ctor(class [System]System.Net.Sockets.Socket)
        IL_0026:  stfld      class Beetle.TcpChannel Beetle.CreateChannelArgs::Channel
        IL_002b:  ldarg.1
        IL_002c:  callvirt   instance object [System]System.Net.Sockets.SocketAsyncEventArgs::get_UserToken()
        IL_0031:  castclass  class [mscorlib]System.Action`1<class Beetle.CreateChannelArgs>
        IL_0036:  stloc.1
        IL_0037:  ldloc.1
        IL_0038:  ldloc.0
        IL_0039:  callvirt   instance void class [mscorlib]System.Action`1<class Beetle.CreateChannelArgs>::Invoke(!0)
        IL_003e:  leave.s    IL_0043

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0040:  pop
        IL_0041:  leave.s    IL_0043

      }  // end handler
      IL_0043:  leave.s    IL_0063

    }  // end .try
    finally
    {
      IL_0045:  ldarg.1
      IL_0046:  ldnull
      IL_0047:  ldftn      void Beetle.TcpServer::a(object,
                                                    class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_004d:  newobj     instance void class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::.ctor(object,
                                                                                                                                             native int)
      IL_0052:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::remove_Completed(class [mscorlib]System.EventHandler`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>)
      IL_0057:  ldsfld     class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs> Beetle.TcpServer::SocketAsyncEventArgsPools
      IL_005c:  ldarg.1
      IL_005d:  callvirt   instance void class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::Push(!0)
      IL_0062:  endfinally
    }  // end handler
    IL_0063:  ret
  } // end of method TcpServer::a

  .method assembly hidebysig instance void 
          c(class Beetle.TcpChannel A_0) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::o
    IL_0007:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::get_Count()
    IL_000c:  stfld      int32 Beetle.TcpChannel::d
    IL_0011:  ldarg.1
    IL_0012:  ldstr      "_data"
    IL_0017:  ldarg.0
    IL_0018:  ldfld      int32 Beetle.TcpServer::d
    IL_001d:  box        [mscorlib]System.Int32
    IL_0022:  callvirt   instance void Beetle.TcpChannel::set_Item(string,
                                                                   object)
    IL_0027:  ret
  } // end of method TcpServer::c

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       170 (0xaa)
    .maxstack  2
    .locals init (class Beetle.TcpChannel V_0,
             class Beetle.TcpServer V_1,
             class Beetle.TcpChannel[] V_2,
             int32 V_3,
             class [System]System.Collections.Generic.Queue`1<class r> V_4)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.1
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool Beetle.TcpServer::n
      IL_000e:  brfalse    IL_00a0

      IL_0013:  ldarg.0
      IL_0014:  call       instance class Beetle.DetectTimeoutHandler Beetle.TcpServer::get_DetectHandler()
      IL_0019:  ldnull
      IL_001a:  callvirt   instance void Beetle.DetectTimeoutHandler::a(class Beetle.TcpServer)
      IL_001f:  ldarg.0
      IL_0020:  ldc.i4.0
      IL_0021:  stfld      bool Beetle.TcpServer::n
      IL_0026:  ldarg.0
      IL_0027:  call       instance class Beetle.TcpChannel[] Beetle.TcpServer::GetOnlines()
      IL_002c:  stloc.2
      IL_002d:  ldc.i4.0
      IL_002e:  stloc.3
      IL_002f:  br.s       IL_003f

      IL_0031:  ldloc.2
      IL_0032:  ldloc.3
      IL_0033:  ldelem.ref
      IL_0034:  stloc.0
      IL_0035:  ldloc.0
      IL_0036:  callvirt   instance void Beetle.TcpChannel::Dispose()
      IL_003b:  ldloc.3
      IL_003c:  ldc.i4.1
      IL_003d:  add
      IL_003e:  stloc.3
      IL_003f:  ldloc.3
      IL_0040:  ldloc.2
      IL_0041:  ldlen
      IL_0042:  conv.i4
      IL_0043:  blt.s      IL_0031

      IL_0045:  ldarg.0
      IL_0046:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::o
      IL_004b:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::Clear()
      IL_0050:  ldarg.0
      IL_0051:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpServer::get_Socket()
      IL_0056:  brfalse.s  IL_006a

      IL_0058:  ldarg.0
      IL_0059:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpServer::get_Socket()
      IL_005e:  call       void Beetle.TcpServer::ReleaseSocket(class [System]System.Net.Sockets.Socket)
      IL_0063:  ldarg.0
      IL_0064:  ldnull
      IL_0065:  stfld      class [System]System.Net.Sockets.Socket Beetle.TcpServer::h
      IL_006a:  ldarg.0
      IL_006b:  ldfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
      IL_0070:  dup
      IL_0071:  stloc.s    V_4
      IL_0073:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
      .try
      {
        IL_0078:  ldarg.0
        IL_0079:  ldfld      class [System]System.Collections.Generic.Queue`1<class r> Beetle.TcpServer::j
        IL_007e:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class r>::Clear()
        IL_0083:  leave.s    IL_008d

      }  // end .try
      finally
      {
        IL_0085:  ldloc.s    V_4
        IL_0087:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
        IL_008c:  endfinally
      }  // end handler
      IL_008d:  ldarg.0
      IL_008e:  ldfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
      IL_0093:  ldnull
      IL_0094:  callvirt   instance void Beetle.ChannelCreatingArgs::a(class Beetle.TcpServer)
      IL_0099:  ldarg.0
      IL_009a:  ldnull
      IL_009b:  stfld      class Beetle.ChannelCreatingArgs Beetle.TcpServer::f
      IL_00a0:  leave.s    IL_00a9

    }  // end .try
    finally
    {
      IL_00a2:  ldloc.1
      IL_00a3:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_00a8:  endfinally
    }  // end handler
    IL_00a9:  ret
  } // end of method TcpServer::Dispose

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       26 (0x1a)
    .maxstack  8
    IL_0000:  ldstr      "73HWrM6iCrk="
    IL_0005:  stsfld     string Beetle.TcpServer::g
    IL_000a:  ldc.i4     0x400
    IL_000f:  newobj     instance void class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs>::.ctor(int32)
    IL_0014:  stsfld     class Beetle.ObjectPool`1<class [System]System.Net.Sockets.SocketAsyncEventArgs> Beetle.TcpServer::SocketAsyncEventArgsPools
    IL_0019:  ret
  } // end of method TcpServer::.cctor

  .event Beetle.EventChannglCreating ChannelCreating
  {
    .removeon instance void Beetle.TcpServer::remove_ChannelCreating(class Beetle.EventChannglCreating)
    .addon instance void Beetle.TcpServer::add_ChannelCreating(class Beetle.EventChannglCreating)
  } // end of event TcpServer::ChannelCreating
  .event Beetle.EventChannelConnected ChannelConnected
  {
    .removeon instance void Beetle.TcpServer::remove_ChannelConnected(class Beetle.EventChannelConnected)
    .addon instance void Beetle.TcpServer::add_ChannelConnected(class Beetle.EventChannelConnected)
  } // end of event TcpServer::ChannelConnected
  .event Beetle.EventChannelDisposed ChannelDisposed
  {
    .removeon instance void Beetle.TcpServer::remove_ChannelDisposed(class Beetle.EventChannelDisposed)
    .addon instance void Beetle.TcpServer::add_ChannelDisposed(class Beetle.EventChannelDisposed)
  } // end of event TcpServer::ChannelDisposed
  .property instance bool IsRunning()
  {
    .get instance bool Beetle.TcpServer::get_IsRunning()
  } // end of property TcpServer::IsRunning
  .property instance class Beetle.DetectTimeoutHandler
          DetectHandler()
  {
    .get instance class Beetle.DetectTimeoutHandler Beetle.TcpServer::get_DetectHandler()
  } // end of property TcpServer::DetectHandler
  .property instance int32 QueueConnects()
  {
    .get instance int32 Beetle.TcpServer::get_QueueConnects()
  } // end of property TcpServer::QueueConnects
  .property instance class [System]System.Net.Sockets.Socket
          Socket()
  {
    .get instance class [System]System.Net.Sockets.Socket Beetle.TcpServer::get_Socket()
  } // end of property TcpServer::Socket
  .property instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel>
          Clients()
  {
    .get instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::get_Clients()
  } // end of property TcpServer::Clients
  .property instance class [mscorlib]System.Text.Encoding
          Coding()
  {
    .get instance class [mscorlib]System.Text.Encoding Beetle.TcpServer::get_Coding()
    .set instance void Beetle.TcpServer::set_Coding(class [mscorlib]System.Text.Encoding)
  } // end of property TcpServer::Coding
  .property instance class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception>
          Errors()
  {
    .get instance class [System]System.Collections.Generic.Queue`1<class [mscorlib]System.Exception> Beetle.TcpServer::get_Errors()
  } // end of property TcpServer::Errors
  .property instance bool EnabledChannelSendCompeletedEvent()
  {
    .get instance bool Beetle.TcpServer::get_EnabledChannelSendCompeletedEvent()
    .set instance void Beetle.TcpServer::set_EnabledChannelSendCompeletedEvent(bool)
  } // end of property TcpServer::EnabledChannelSendCompeletedEvent
} // end of class Beetle.TcpServer

.class interface public abstract auto ansi Beetle.IMessage
{
  .method public hidebysig newslot abstract virtual 
          instance void  Save(class Beetle.BufferWriter writer) cil managed
  {
  } // end of method IMessage::Save

  .method public hidebysig newslot abstract virtual 
          instance void  Load(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IMessage::Load

} // end of class Beetle.IMessage

.class public auto ansi beforefieldinit Beetle.MessagePool`1<.ctor (Beetle.IMessage) T>
       extends [mscorlib]System.Object
{
  .field private class [System]System.Collections.Generic.Stack`1<!T> a
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 count) cil managed
  {
    // 代码大小       78 (0x4e)
    .maxstack  3
    .locals init (int32 V_0,
             !T V_1,
             !T V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4     0x400
    IL_0006:  newobj     instance void class [System]System.Collections.Generic.Stack`1<!T>::.ctor(int32)
    IL_000b:  stfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.MessagePool`1<!T>::a
    IL_0010:  ldarg.0
    IL_0011:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0016:  ldc.i4.0
    IL_0017:  stloc.0
    IL_0018:  br.s       IL_0049

    IL_001a:  ldarg.0
    IL_001b:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.MessagePool`1<!T>::a
    IL_0020:  ldloca.s   V_1
    IL_0022:  initobj    !T
    IL_0028:  ldloc.1
    IL_0029:  box        !T
    IL_002e:  brfalse.s  IL_003b

    IL_0030:  ldloca.s   V_2
    IL_0032:  initobj    !T
    IL_0038:  ldloc.2
    IL_0039:  br.s       IL_0040

    IL_003b:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
    IL_0040:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<!T>::Push(!0)
    IL_0045:  ldloc.0
    IL_0046:  ldc.i4.1
    IL_0047:  add
    IL_0048:  stloc.0
    IL_0049:  ldloc.0
    IL_004a:  ldarg.1
    IL_004b:  blt.s      IL_001a

    IL_004d:  ret
  } // end of method MessagePool`1::.ctor

  .method private hidebysig instance !T  a() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  2
    .locals init (!T V_0,
             !T V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    !T
    IL_0008:  ldloc.0
    IL_0009:  box        !T
    IL_000e:  brfalse.s  IL_001a

    IL_0010:  ldloca.s   V_1
    IL_0012:  initobj    !T
    IL_0018:  ldloc.1
    IL_0019:  ret

    IL_001a:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
    IL_001f:  ret
  } // end of method MessagePool`1::a

  .method public hidebysig instance !T  Pop() cil managed
  {
    // 代码大小       53 (0x35)
    .maxstack  2
    .locals init (!T V_0,
             class Beetle.MessagePool`1<!T> V_1)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.1
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.MessagePool`1<!T>::a
      IL_000e:  callvirt   instance int32 class [System]System.Collections.Generic.Stack`1<!T>::get_Count()
      IL_0013:  brtrue.s   IL_001e

      IL_0015:  ldarg.0
      IL_0016:  call       instance !0 class Beetle.MessagePool`1<!T>::a()
      IL_001b:  stloc.0
      IL_001c:  leave.s    IL_0033

      IL_001e:  ldarg.0
      IL_001f:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.MessagePool`1<!T>::a
      IL_0024:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<!T>::Pop()
      IL_0029:  stloc.0
      IL_002a:  leave.s    IL_0033

    }  // end .try
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0032:  endfinally
    }  // end handler
    IL_0033:  ldloc.0
    IL_0034:  ret
  } // end of method MessagePool`1::Pop

  .method public hidebysig instance void 
          Push(!T item) cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  2
    .locals init (class Beetle.MessagePool`1<!T> V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.MessagePool`1<!T>::a
      IL_000e:  ldarg.1
      IL_000f:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<!T>::Push(!0)
      IL_0014:  leave.s    IL_001d

    }  // end .try
    finally
    {
      IL_0016:  ldloc.0
      IL_0017:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_001c:  endfinally
    }  // end handler
    IL_001d:  ret
  } // end of method MessagePool`1::Push

} // end of class Beetle.MessagePool`1

.class interface public abstract auto ansi Beetle.IObjectReset
{
  .method public hidebysig newslot abstract virtual 
          instance void  Reset() cil managed
  {
  } // end of method IObjectReset::Reset

} // end of class Beetle.IObjectReset

.class interface public abstract auto ansi Beetle.ICounter
       implements Beetle.IMessage
{
  .method public hidebysig newslot abstract virtual 
          instance void  SetCount(int32 count) cil managed
  {
  } // end of method ICounter::SetCount

  .method public hidebysig newslot abstract virtual 
          instance void  Reduce() cil managed
  {
  } // end of method ICounter::Reduce

  .method public hidebysig newslot specialname abstract virtual 
          instance class [mscorlib]System.Action`1<class Beetle.ICounter> 
          get_Disposed() cil managed
  {
  } // end of method ICounter::get_Disposed

  .method public hidebysig newslot specialname abstract virtual 
          instance void  set_Disposed(class [mscorlib]System.Action`1<class Beetle.ICounter> 'value') cil managed
  {
  } // end of method ICounter::set_Disposed

  .property instance class [mscorlib]System.Action`1<class Beetle.ICounter>
          Disposed()
  {
    .get instance class [mscorlib]System.Action`1<class Beetle.ICounter> Beetle.ICounter::get_Disposed()
    .set instance void Beetle.ICounter::set_Disposed(class [mscorlib]System.Action`1<class Beetle.ICounter>)
  } // end of property ICounter::Disposed
} // end of class Beetle.ICounter

.class public abstract auto ansi beforefieldinit Beetle.Counter
       extends [mscorlib]System.Object
       implements Beetle.ICounter
{
  .field private int32 a
  .field private class [mscorlib]System.Action`1<class Beetle.ICounter> b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig newslot virtual final 
          instance void  SetCount(int32 count) cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 Beetle.Counter::a
    IL_0007:  ret
  } // end of method Counter::SetCount

  .method public hidebysig newslot virtual final 
          instance void  Reduce() cil managed
  {
    // 代码大小       60 (0x3c)
    .maxstack  3
    .locals init (class Beetle.Counter V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  dup
      IL_000a:  ldfld      int32 Beetle.Counter::a
      IL_000f:  ldc.i4.1
      IL_0010:  sub
      IL_0011:  stfld      int32 Beetle.Counter::a
      IL_0016:  ldarg.0
      IL_0017:  ldfld      int32 Beetle.Counter::a
      IL_001c:  brtrue.s   IL_0032

      IL_001e:  ldarg.0
      IL_001f:  call       instance class [mscorlib]System.Action`1<class Beetle.ICounter> Beetle.Counter::get_Disposed()
      IL_0024:  brfalse.s  IL_0032

      IL_0026:  ldarg.0
      IL_0027:  call       instance class [mscorlib]System.Action`1<class Beetle.ICounter> Beetle.Counter::get_Disposed()
      IL_002c:  ldarg.0
      IL_002d:  callvirt   instance void class [mscorlib]System.Action`1<class Beetle.ICounter>::Invoke(!0)
      IL_0032:  leave.s    IL_003b

    }  // end .try
    finally
    {
      IL_0034:  ldloc.0
      IL_0035:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003a:  endfinally
    }  // end handler
    IL_003b:  ret
  } // end of method Counter::Reduce

  .method public hidebysig newslot specialname virtual final 
          instance class [mscorlib]System.Action`1<class Beetle.ICounter> 
          get_Disposed() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Action`1<class Beetle.ICounter> Beetle.Counter::b
    IL_0006:  ret
  } // end of method Counter::get_Disposed

  .method public hidebysig newslot specialname virtual final 
          instance void  set_Disposed(class [mscorlib]System.Action`1<class Beetle.ICounter> 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [mscorlib]System.Action`1<class Beetle.ICounter> Beetle.Counter::b
    IL_0007:  ret
  } // end of method Counter::set_Disposed

  .method public hidebysig newslot abstract virtual 
          instance void  Save(class Beetle.BufferWriter writer) cil managed
  {
  } // end of method Counter::Save

  .method public hidebysig newslot abstract virtual 
          instance void  Load(class Beetle.BufferReader reader) cil managed
  {
  } // end of method Counter::Load

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Counter::.ctor

  .property instance class [mscorlib]System.Action`1<class Beetle.ICounter>
          Disposed()
  {
    .set instance void Beetle.Counter::set_Disposed(class [mscorlib]System.Action`1<class Beetle.ICounter>)
    .get instance class [mscorlib]System.Action`1<class Beetle.ICounter> Beetle.Counter::get_Disposed()
  } // end of property Counter::Disposed
} // end of class Beetle.Counter

.class public auto ansi beforefieldinit Beetle.ObjectFormatter
       extends [mscorlib]System.Object
{
  .field private static class Beetle.BufferWriter a
  .field private static class Beetle.BufferReader b
  .field public static int32 EncodingStringMaxLength
  .field private static object c
  .field private static object d
  .method public hidebysig static void  Setup() cil managed
  {
    // 代码大小       20 (0x14)
    .maxstack  8
    IL_0000:  ldsfld     int32 Beetle.ObjectFormatter::EncodingStringMaxLength
    IL_0005:  stsfld     int32 Beetle.TcpUtils::StringEncodingLength
    IL_000a:  ldc.i4.s   100
    IL_000c:  ldc.i4.1
    IL_000d:  ldc.i4.0
    IL_000e:  call       void Beetle.TcpUtils::Setup(int32,
                                                     int32,
                                                     int32)
    IL_0013:  ret
  } // end of method ObjectFormatter::Setup

  .method public hidebysig static void  Serialize(class Beetle.IMessage message,
                                                  class Beetle.ByteArraySegment segment) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0007:  call       void Beetle.ObjectFormatter::Serialize(class Beetle.IMessage,
                                                                class Beetle.ByteArraySegment,
                                                                class [mscorlib]System.Text.Encoding)
    IL_000c:  ret
  } // end of method ObjectFormatter::Serialize

  .method public hidebysig static void  Serialize(class Beetle.IMessage message,
                                                  class Beetle.ByteArraySegment segment,
                                                  class [mscorlib]System.Text.Encoding coding) cil managed
  {
    // 代码大小       84 (0x54)
    .maxstack  3
    .locals init (object V_0)
    IL_0000:  ldsfld     object Beetle.ObjectFormatter::c
    IL_0005:  dup
    IL_0006:  stloc.0
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldsfld     class Beetle.BufferWriter Beetle.ObjectFormatter::a
      IL_0011:  brtrue.s   IL_001f

      IL_0013:  ldarg.2
      IL_0014:  ldc.i4.1
      IL_0015:  newobj     instance void Beetle.BufferWriter::.ctor(class [mscorlib]System.Text.Encoding,
                                                                    bool)
      IL_001a:  stsfld     class Beetle.BufferWriter Beetle.ObjectFormatter::a
      IL_001f:  ldsfld     class Beetle.BufferWriter Beetle.ObjectFormatter::a
      IL_0024:  ldarg.2
      IL_0025:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
      IL_002a:  ldsfld     class Beetle.BufferWriter Beetle.ObjectFormatter::a
      IL_002f:  callvirt   instance void Beetle.BufferWriter::Reset()
      IL_0034:  ldarg.0
      IL_0035:  ldsfld     class Beetle.BufferWriter Beetle.ObjectFormatter::a
      IL_003a:  callvirt   instance void Beetle.IMessage::Save(class Beetle.BufferWriter)
      IL_003f:  ldarg.1
      IL_0040:  ldsfld     class Beetle.BufferWriter Beetle.ObjectFormatter::a
      IL_0045:  callvirt   instance void Beetle.ByteArraySegment::Import(class Beetle.BufferWriter)
      IL_004a:  leave.s    IL_0053

    }  // end .try
    finally
    {
      IL_004c:  ldloc.0
      IL_004d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0052:  endfinally
    }  // end handler
    IL_0053:  ret
  } // end of method ObjectFormatter::Serialize

  .method public hidebysig static !!T  Deserialize<.ctor (Beetle.IMessage) T>(class Beetle.ByteArraySegment segment) cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0006:  call       !!0 Beetle.ObjectFormatter::Deserialize<!!0>(class Beetle.ByteArraySegment,
                                                                      class [mscorlib]System.Text.Encoding)
    IL_000b:  ret
  } // end of method ObjectFormatter::Deserialize

  .method public hidebysig static !!T  Deserialize<.ctor (Beetle.IMessage) T>(class Beetle.ByteArraySegment segment,
                                                                              class [mscorlib]System.Text.Encoding coding) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldtoken    !!T
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ldarg.0
    IL_000b:  ldarg.1
    IL_000c:  call       class Beetle.IMessage Beetle.ObjectFormatter::Deserialize(class [mscorlib]System.Type,
                                                                                   class Beetle.ByteArraySegment,
                                                                                   class [mscorlib]System.Text.Encoding)
    IL_0011:  unbox.any  !!T
    IL_0016:  ret
  } // end of method ObjectFormatter::Deserialize

  .method public hidebysig static class Beetle.IMessage 
          Deserialize(class [mscorlib]System.Type 'type',
                      class Beetle.ByteArraySegment segment,
                      class [mscorlib]System.Text.Encoding coding) cil managed
  {
    // 代码大小       98 (0x62)
    .maxstack  2
    .locals init (class Beetle.IMessage V_0,
             class Beetle.IMessage V_1,
             object V_2)
    IL_0000:  ldsfld     object Beetle.ObjectFormatter::d
    IL_0005:  dup
    IL_0006:  stloc.2
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_0011:  brtrue.s   IL_001e

      IL_0013:  ldarg.2
      IL_0014:  newobj     instance void Beetle.BufferReader::.ctor(class [mscorlib]System.Text.Encoding)
      IL_0019:  stsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_001e:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_0023:  ldarg.2
      IL_0024:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
      IL_0029:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_002e:  callvirt   instance void Beetle.BufferReader::CleanBuffer()
      IL_0033:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_0038:  ldarg.1
      IL_0039:  callvirt   instance void Beetle.BufferReader::Import(class Beetle.ByteArraySegment)
      IL_003e:  ldarg.0
      IL_003f:  call       object [mscorlib]System.Activator::CreateInstance(class [mscorlib]System.Type)
      IL_0044:  castclass  Beetle.IMessage
      IL_0049:  stloc.0
      IL_004a:  ldloc.0
      IL_004b:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_0050:  callvirt   instance void Beetle.IMessage::Load(class Beetle.BufferReader)
      IL_0055:  ldloc.0
      IL_0056:  stloc.1
      IL_0057:  leave.s    IL_0060

    }  // end .try
    finally
    {
      IL_0059:  ldloc.2
      IL_005a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_005f:  endfinally
    }  // end handler
    IL_0060:  ldloc.1
    IL_0061:  ret
  } // end of method ObjectFormatter::Deserialize

  .method public hidebysig static void  Deserialize(class Beetle.IMessage msg,
                                                    class Beetle.ByteArraySegment segment) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0007:  call       void Beetle.ObjectFormatter::Deserialize(class Beetle.IMessage,
                                                                  class Beetle.ByteArraySegment,
                                                                  class [mscorlib]System.Text.Encoding)
    IL_000c:  ret
  } // end of method ObjectFormatter::Deserialize

  .method public hidebysig static void  Deserialize(class Beetle.IMessage msg,
                                                    class Beetle.ByteArraySegment segment,
                                                    class [mscorlib]System.Text.Encoding coding) cil managed
  {
    // 代码大小       83 (0x53)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  ldsfld     object Beetle.ObjectFormatter::d
    IL_0005:  dup
    IL_0006:  stloc.0
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_0011:  brtrue.s   IL_001e

      IL_0013:  ldarg.2
      IL_0014:  newobj     instance void Beetle.BufferReader::.ctor(class [mscorlib]System.Text.Encoding)
      IL_0019:  stsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_001e:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_0023:  ldarg.2
      IL_0024:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
      IL_0029:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_002e:  callvirt   instance void Beetle.BufferReader::CleanBuffer()
      IL_0033:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_0038:  ldarg.1
      IL_0039:  callvirt   instance void Beetle.BufferReader::Import(class Beetle.ByteArraySegment)
      IL_003e:  ldarg.0
      IL_003f:  ldsfld     class Beetle.BufferReader Beetle.ObjectFormatter::b
      IL_0044:  callvirt   instance void Beetle.IMessage::Load(class Beetle.BufferReader)
      IL_0049:  leave.s    IL_0052

    }  // end .try
    finally
    {
      IL_004b:  ldloc.0
      IL_004c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0051:  endfinally
    }  // end handler
    IL_0052:  ret
  } // end of method ObjectFormatter::Deserialize

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ObjectFormatter::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       31 (0x1f)
    .maxstack  8
    IL_0000:  ldc.i4     0x200
    IL_0005:  stsfld     int32 Beetle.ObjectFormatter::EncodingStringMaxLength
    IL_000a:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_000f:  stsfld     object Beetle.ObjectFormatter::c
    IL_0014:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0019:  stsfld     object Beetle.ObjectFormatter::d
    IL_001e:  ret
  } // end of method ObjectFormatter::.cctor

} // end of class Beetle.ObjectFormatter

.class public auto ansi beforefieldinit Beetle.ObjectArraySegment`1<T>
       extends [mscorlib]System.Object
{
  .field private !T[] a
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 c
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ObjectArraySegment`1::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 count) cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  newarr     !T
    IL_000d:  call       instance void class Beetle.ObjectArraySegment`1<!T>::a(!0[])
    IL_0012:  ret
  } // end of method ObjectArraySegment`1::.ctor

  .method public hidebysig instance void 
          SetArray(!T[] data,
                   int32 offset,
                   int32 count) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void class Beetle.ObjectArraySegment`1<!T>::a(!0[])
    IL_0007:  ldarg.0
    IL_0008:  ldarg.2
    IL_0009:  call       instance void class Beetle.ObjectArraySegment`1<!T>::b(int32)
    IL_000e:  ldarg.0
    IL_000f:  ldarg.3
    IL_0010:  call       instance void class Beetle.ObjectArraySegment`1<!T>::a(int32)
    IL_0015:  ret
  } // end of method ObjectArraySegment`1::SetArray

  .method public hidebysig instance void 
          SetArray(int32 offset,
                   int32 count) cil managed
  {
    // 代码大小       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void class Beetle.ObjectArraySegment`1<!T>::b(int32)
    IL_0007:  ldarg.0
    IL_0008:  ldarg.2
    IL_0009:  call       instance void class Beetle.ObjectArraySegment`1<!T>::a(int32)
    IL_000e:  ret
  } // end of method ObjectArraySegment`1::SetArray

  .method public hidebysig specialname instance !T[] 
          get_Array() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0[] class Beetle.ObjectArraySegment`1<!T>::a
    IL_0006:  ret
  } // end of method ObjectArraySegment`1::get_Array

  .method assembly hidebysig specialname 
          instance void  a(!T[] A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      !0[] class Beetle.ObjectArraySegment`1<!T>::a
    IL_0007:  ret
  } // end of method ObjectArraySegment`1::a

  .method public hidebysig specialname instance int32 
          get_Offset() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 class Beetle.ObjectArraySegment`1<!T>::b
    IL_0006:  ret
  } // end of method ObjectArraySegment`1::get_Offset

  .method assembly hidebysig specialname 
          instance void  b(int32 A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 class Beetle.ObjectArraySegment`1<!T>::b
    IL_0007:  ret
  } // end of method ObjectArraySegment`1::b

  .method public hidebysig specialname instance int32 
          get_Count() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 class Beetle.ObjectArraySegment`1<!T>::c
    IL_0006:  ret
  } // end of method ObjectArraySegment`1::get_Count

  .method assembly hidebysig specialname 
          instance void  a(int32 A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 class Beetle.ObjectArraySegment`1<!T>::c
    IL_0007:  ret
  } // end of method ObjectArraySegment`1::a

  .property instance !T[] Array()
  {
    .set instance void Beetle.ObjectArraySegment`1::a(!T[])
    .get instance !T[] Beetle.ObjectArraySegment`1::get_Array()
  } // end of property ObjectArraySegment`1::Array
  .property instance int32 Offset()
  {
    .set instance void Beetle.ObjectArraySegment`1::b(int32)
    .get instance int32 Beetle.ObjectArraySegment`1::get_Offset()
  } // end of property ObjectArraySegment`1::Offset
  .property instance int32 Count()
  {
    .get instance int32 Beetle.ObjectArraySegment`1::get_Count()
    .set instance void Beetle.ObjectArraySegment`1::a(int32)
  } // end of property ObjectArraySegment`1::Count
} // end of class Beetle.ObjectArraySegment`1

.class public auto ansi beforefieldinit Beetle.NetTcpException
       extends [mscorlib]System.Exception
{
  .field static assembly literal string a = "<Modulus>{0}<Modulus/>"
  .field static assembly literal string b = "The message is too long"
  .field static assembly literal string c = "Cross-border access to data"
  .field static assembly literal string d = " IPAddress Error!"
  .field static assembly literal string e = "{0} Type not found!"
  .field static assembly literal string f = "{0} load data error!"
  .field static assembly literal string g = "channel is disposed!"
  .field static assembly literal string h = "channel receive data timeout!"
  .field static assembly literal string i = "channel data process error!"
  .field static assembly literal string j = "channel must be connected"
  .field static assembly literal string k = "{0} config selection not found!"
  .field static assembly literal int32 l = int32(0x00002711)
  .field static assembly literal int32 m = int32(0x00002714)
  .field static assembly literal int32 n = int32(0x00002716)
  .field private int32 o
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Exception::.ctor()
    IL_0006:  ret
  } // end of method NetTcpException::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(string msg) cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void [mscorlib]System.Exception::.ctor(string)
    IL_0007:  ret
  } // end of method NetTcpException::.ctor

  .method assembly hidebysig specialname 
          instance int32  a() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Beetle.NetTcpException::o
    IL_0006:  ret
  } // end of method NetTcpException::a

  .method public hidebysig specialname instance void 
          set_ErrorNumber(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 Beetle.NetTcpException::o
    IL_0007:  ret
  } // end of method NetTcpException::set_ErrorNumber

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Exception innererr,
                               string msg) cil managed
  {
    // 代码大小       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.2
    IL_0002:  ldarg.1
    IL_0003:  call       instance void [mscorlib]System.Exception::.ctor(string,
                                                                         class [mscorlib]System.Exception)
    IL_0008:  ret
  } // end of method NetTcpException::.ctor

  .method public hidebysig static class Beetle.NetTcpException 
          ConfigSelectionNotFound(string name) cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldstr      "{0} config selection not found!"
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_000b:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_0010:  ret
  } // end of method NetTcpException::ConfigSelectionNotFound

  .method public hidebysig static class Beetle.NetTcpException 
          ObjectLoadError(string 'type',
                          class [mscorlib]System.Exception innerexception) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldstr      "{0} load data error!"
    IL_0006:  ldarg.0
    IL_0007:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_000c:  newobj     instance void Beetle.NetTcpException::.ctor(class [mscorlib]System.Exception,
                                                                     string)
    IL_0011:  ret
  } // end of method NetTcpException::ObjectLoadError

  .method public hidebysig static class Beetle.NetTcpException 
          ReadToByteArraySegment() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "read to ByteArraySegment error!"
    IL_0005:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_000a:  ret
  } // end of method NetTcpException::ReadToByteArraySegment

  .method public hidebysig static class Beetle.NetTcpException 
          ClientDataProcessError(class [mscorlib]System.Exception innerexception) cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "channel data process error!"
    IL_0006:  newobj     instance void Beetle.NetTcpException::.ctor(class [mscorlib]System.Exception,
                                                                     string)
    IL_000b:  ret
  } // end of method NetTcpException::ClientDataProcessError

  .method public hidebysig static class Beetle.NetTcpException 
          ClientReceiveTimeout() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "channel receive data timeout!"
    IL_0005:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_000a:  ret
  } // end of method NetTcpException::ClientReceiveTimeout

  .method public hidebysig static class Beetle.NetTcpException 
          ClientMustBeConnected() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "channel must be connected"
    IL_0005:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_000a:  ret
  } // end of method NetTcpException::ClientMustBeConnected

  .method public hidebysig static class Beetle.NetTcpException 
          ClientIsDisposed() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "channel is disposed!"
    IL_0005:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_000a:  ret
  } // end of method NetTcpException::ClientIsDisposed

  .method public hidebysig static class Beetle.NetTcpException 
          StringEncodingError(class [mscorlib]System.Exception innerexception) cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "string encoding error!"
    IL_0006:  newobj     instance void Beetle.NetTcpException::.ctor(class [mscorlib]System.Exception,
                                                                     string)
    IL_000b:  ret
  } // end of method NetTcpException::StringEncodingError

  .method public hidebysig static class Beetle.NetTcpException 
          StringDecodingError(class [mscorlib]System.Exception innerexception) cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "string decoding error!"
    IL_0006:  newobj     instance void Beetle.NetTcpException::.ctor(class [mscorlib]System.Exception,
                                                                     string)
    IL_000b:  ret
  } // end of method NetTcpException::StringDecodingError

  .method public hidebysig static class Beetle.NetTcpException 
          NotInitialize() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "Beetle component did not initialize!\r\nCall TcpUtil"
    + ".Setup!"
    IL_0005:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_000a:  ret
  } // end of method NetTcpException::NotInitialize

  .method public hidebysig static class Beetle.NetTcpException 
          PacketAnyalysisNotInitialize() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "Beetle Anyalysis  did not initialize!\r\nCall Packet"
    + "Analysis.Setup!"
    IL_0005:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_000a:  ret
  } // end of method NetTcpException::PacketAnyalysisNotInitialize

  .method public hidebysig static class Beetle.NetTcpException 
          TypeNotFound(string name) cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldstr      "{0} Type not found!"
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_000b:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_0010:  ret
  } // end of method NetTcpException::TypeNotFound

  .method public hidebysig static class Beetle.NetTcpException 
          DataOverflow() cil managed
  {
    // 代码大小       24 (0x18)
    .maxstack  2
    .locals init (class Beetle.NetTcpException V_0)
    IL_0000:  ldstr      "The message is too long"
    IL_0005:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4     0x2711
    IL_0011:  callvirt   instance void Beetle.NetTcpException::set_ErrorNumber(int32)
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method NetTcpException::DataOverflow

  .method public hidebysig static class Beetle.NetTcpException 
          ReadDataError(class [mscorlib]System.Exception innererror) cil managed
  {
    // 代码大小       25 (0x19)
    .maxstack  3
    .locals init (class Beetle.NetTcpException V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "Cross-border access to data"
    IL_0006:  newobj     instance void Beetle.NetTcpException::.ctor(class [mscorlib]System.Exception,
                                                                     string)
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4     0x2714
    IL_0012:  callvirt   instance void Beetle.NetTcpException::set_ErrorNumber(int32)
    IL_0017:  ldloc.0
    IL_0018:  ret
  } // end of method NetTcpException::ReadDataError

  .method public hidebysig static class Beetle.NetTcpException 
          IPAddressError() cil managed
  {
    // 代码大小       24 (0x18)
    .maxstack  2
    .locals init (class Beetle.NetTcpException V_0)
    IL_0000:  ldstr      " IPAddress Error!"
    IL_0005:  newobj     instance void Beetle.NetTcpException::.ctor(string)
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4     0x2716
    IL_0011:  callvirt   instance void Beetle.NetTcpException::set_ErrorNumber(int32)
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method NetTcpException::IPAddressError

  .property instance int32 ErrorNumber()
  {
    .set instance void Beetle.NetTcpException::set_ErrorNumber(int32)
    .get instance int32 Beetle.NetTcpException::a()
  } // end of property NetTcpException::ErrorNumber
} // end of class Beetle.NetTcpException

.class public auto ansi beforefieldinit Beetle.ChannelEventArgs
       extends [mscorlib]System.Object
{
  .field public class Beetle.TcpChannel Channel
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ChannelEventArgs::.ctor

} // end of class Beetle.ChannelEventArgs

.class public auto ansi beforefieldinit Beetle.SendMessageCompletedArgs
       extends Beetle.ChannelEventArgs
{
  .field public bool Success
  .field public class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Messages
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Beetle.ChannelEventArgs::.ctor()
    IL_0006:  ret
  } // end of method SendMessageCompletedArgs::.ctor

} // end of class Beetle.SendMessageCompletedArgs

.class public auto ansi beforefieldinit Beetle.ByteArraySegment
       extends [mscorlib]System.Object
{
  .field assembly uint8[] a
  .field public int32 Offset
  .field public int32 Count
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ByteArraySegment::.ctor

  .method public hidebysig instance void 
          Import(class Beetle.BufferWriter writer) cil managed
  {
    // 代码大小       98 (0x62)
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             class b V_2,
             class [mscorlib]System.Exception V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_004b

    IL_0006:  ldarg.1
    IL_0007:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_000c:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
    IL_0011:  stloc.2
    .try
    {
      .try
      {
        IL_0012:  ldloc.2
        IL_0013:  ldfld      uint8[] b::e
        IL_0018:  ldc.i4.0
        IL_0019:  ldarg.0
        IL_001a:  ldfld      uint8[] Beetle.ByteArraySegment::a
        IL_001f:  ldloc.0
        IL_0020:  ldloc.2
        IL_0021:  ldfld      int32 b::h
        IL_0026:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                     int32,
                                                                     class [mscorlib]System.Array,
                                                                     int32,
                                                                     int32)
        IL_002b:  ldloc.0
        IL_002c:  ldloc.2
        IL_002d:  ldfld      int32 b::h
        IL_0032:  add
        IL_0033:  stloc.0
        IL_0034:  ldloc.1
        IL_0035:  ldloc.2
        IL_0036:  ldfld      int32 b::h
        IL_003b:  add
        IL_003c:  stloc.1
        IL_003d:  leave.s    IL_0042

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_003f:  stloc.3
        IL_0040:  ldloc.3
        IL_0041:  throw

      }  // end handler
      IL_0042:  leave.s    IL_004b

    }  // end .try
    finally
    {
      IL_0044:  ldloc.2
      IL_0045:  callvirt   instance void b::Dispose()
      IL_004a:  endfinally
    }  // end handler
    IL_004b:  ldarg.1
    IL_004c:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0051:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_0056:  ldc.i4.0
    IL_0057:  bgt.s      IL_0006

    IL_0059:  ldarg.0
    IL_005a:  ldc.i4.0
    IL_005b:  ldloc.1
    IL_005c:  call       instance void Beetle.ByteArraySegment::SetInfo(int32,
                                                                        int32)
    IL_0061:  ret
  } // end of method ByteArraySegment::Import

  .method public hidebysig instance uint8 
          GetData(int32 index) cil managed
  {
    // 代码大小       16 (0x10)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_000c:  ldarg.1
    IL_000d:  add
    IL_000e:  ldelem.u1
    IL_000f:  ret
  } // end of method ByteArraySegment::GetData

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 length) cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  newarr     [mscorlib]System.Byte
    IL_000d:  call       instance void Beetle.ByteArraySegment::a(uint8[])
    IL_0012:  ret
  } // end of method ByteArraySegment::.ctor

  .method public hidebysig instance void 
          Encoding(string 'value',
                   class [mscorlib]System.Text.Encoding coding) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  6
    .locals init (int32 V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      IL_0000:  ldarg.2
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.0
      IL_0003:  ldarg.1
      IL_0004:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0009:  ldarg.0
      IL_000a:  call       instance uint8[] Beetle.ByteArraySegment::get_Array()
      IL_000f:  ldc.i4.0
      IL_0010:  callvirt   instance int32 [mscorlib]System.Text.Encoding::GetBytes(string,
                                                                                   int32,
                                                                                   int32,
                                                                                   uint8[],
                                                                                   int32)
      IL_0015:  stloc.0
      IL_0016:  ldarg.0
      IL_0017:  ldc.i4.0
      IL_0018:  ldloc.0
      IL_0019:  call       instance void Beetle.ByteArraySegment::SetInfo(int32,
                                                                          int32)
      IL_001e:  leave.s    IL_0028

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0020:  stloc.1
      IL_0021:  ldloc.1
      IL_0022:  call       class Beetle.NetTcpException Beetle.NetTcpException::StringEncodingError(class [mscorlib]System.Exception)
      IL_0027:  throw

    }  // end handler
    IL_0028:  ret
  } // end of method ByteArraySegment::Encoding

  .method public hidebysig instance void 
          Import(class Beetle.ByteArraySegment e) cil managed
  {
    // 代码大小       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0007:  ldarg.1
    IL_0008:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_000d:  ldarg.1
    IL_000e:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0013:  call       instance void Beetle.ByteArraySegment::Import(uint8[],
                                                                       int32,
                                                                       int32)
    IL_0018:  ret
  } // end of method ByteArraySegment::Import

  .method public hidebysig instance void 
          Import(uint8[] data,
                 int32 offet,
                 int32 count) cil managed
  {
    // 代码大小       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldarg.0
    IL_0003:  call       instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0008:  ldc.i4.0
    IL_0009:  ldarg.3
    IL_000a:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.3
    IL_0012:  call       instance void Beetle.ByteArraySegment::SetInfo(int32,
                                                                        int32)
    IL_0017:  ret
  } // end of method ByteArraySegment::Import

  .method public hidebysig instance string 
          Decoding(class [mscorlib]System.Text.Encoding coding) cil managed
  {
    // 代码大小       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_000d:  ldarg.0
    IL_000e:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0013:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[],
                                                                                   int32,
                                                                                   int32)
    IL_0018:  ret
  } // end of method ByteArraySegment::Decoding

  .method public hidebysig instance string 
          Decoding(class [mscorlib]System.Text.Encoding coding,
                   uint8[] data,
                   int32 poffset,
                   int32 pcount) cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldarg.3
    IL_0003:  ldarg.s    pcount
    IL_0005:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[],
                                                                                   int32,
                                                                                   int32)
    IL_000a:  ret
  } // end of method ByteArraySegment::Decoding

  .method public hidebysig instance string 
          ToBase64String(uint8[] data,
                         int32 poffset,
                         int32 pcount) cil managed
  {
    // 代码大小       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldarg.3
    IL_0003:  call       string [mscorlib]System.Convert::ToBase64String(uint8[],
                                                                         int32,
                                                                         int32)
    IL_0008:  ret
  } // end of method ByteArraySegment::ToBase64String

  .method public hidebysig instance string 
          ToBase64String() cil managed
  {
    // 代码大小       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_000c:  ldarg.0
    IL_000d:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0012:  call       string [mscorlib]System.Convert::ToBase64String(uint8[],
                                                                         int32,
                                                                         int32)
    IL_0017:  ret
  } // end of method ByteArraySegment::ToBase64String

  .method public hidebysig instance void 
          FromBase64String(string 'value') cil managed
  {
    // 代码大小       29 (0x1d)
    .maxstack  4
    .locals init (uint8[] V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      IL_0000:  ldarg.1
      IL_0001:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldloc.0
      IL_0009:  ldc.i4.0
      IL_000a:  ldloc.0
      IL_000b:  ldlen
      IL_000c:  conv.i4
      IL_000d:  call       instance void Beetle.ByteArraySegment::Import(uint8[],
                                                                         int32,
                                                                         int32)
      IL_0012:  leave.s    IL_001c

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0014:  stloc.1
      IL_0015:  ldloc.1
      IL_0016:  call       class Beetle.NetTcpException Beetle.NetTcpException::StringEncodingError(class [mscorlib]System.Exception)
      IL_001b:  throw

    }  // end handler
    IL_001c:  ret
  } // end of method ByteArraySegment::FromBase64String

  .method public hidebysig specialname instance uint8[] 
          get_Array() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0006:  ret
  } // end of method ByteArraySegment::get_Array

  .method private hidebysig specialname instance void 
          a(uint8[] A_0) cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      uint8[] Beetle.ByteArraySegment::a
    IL_0007:  ret
  } // end of method ByteArraySegment::a

  .method public hidebysig specialname instance int32 
          get_BufferLength() cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0006:  brtrue.s   IL_000a

    IL_0008:  ldc.i4.0
    IL_0009:  ret

    IL_000a:  ldarg.0
    IL_000b:  call       instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0010:  ldlen
    IL_0011:  conv.i4
    IL_0012:  ret
  } // end of method ByteArraySegment::get_BufferLength

  .method public hidebysig instance void 
          SetInfo(int32 offset,
                  int32 count) cil managed
  {
    // 代码大小       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 Beetle.ByteArraySegment::Offset
    IL_0007:  ldarg.0
    IL_0008:  ldarg.2
    IL_0009:  stfld      int32 Beetle.ByteArraySegment::Count
    IL_000e:  ret
  } // end of method ByteArraySegment::SetInfo

  .method public hidebysig instance void 
          SetInfo(uint8[] data,
                  int32 offset,
                  int32 count) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void Beetle.ByteArraySegment::a(uint8[])
    IL_0007:  ldarg.0
    IL_0008:  ldarg.2
    IL_0009:  stfld      int32 Beetle.ByteArraySegment::Offset
    IL_000e:  ldarg.0
    IL_000f:  ldarg.3
    IL_0010:  stfld      int32 Beetle.ByteArraySegment::Count
    IL_0015:  ret
  } // end of method ByteArraySegment::SetInfo

  .method public hidebysig instance void 
          Clear() cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  call       instance void Beetle.ByteArraySegment::a(uint8[])
    IL_0007:  ret
  } // end of method ByteArraySegment::Clear

  .method public hidebysig instance void 
          EncryptTo(class Beetle.ByteArraySegment segment,
                    class [mscorlib]System.Security.Cryptography.DESCryptoServiceProvider mDESProvider) cil managed
  {
    // 代码大小       89 (0x59)
    .maxstack  4
    .locals init (class [mscorlib]System.IO.MemoryStream V_0,
             class [mscorlib]System.Security.Cryptography.CryptoStream V_1)
    IL_0000:  ldarg.1
    IL_0001:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0006:  newobj     instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldarg.2
    IL_000e:  callvirt   instance class [mscorlib]System.Security.Cryptography.ICryptoTransform [mscorlib]System.Security.Cryptography.SymmetricAlgorithm::CreateEncryptor()
    IL_0013:  ldc.i4.1
    IL_0014:  newobj     instance void [mscorlib]System.Security.Cryptography.CryptoStream::.ctor(class [mscorlib]System.IO.Stream,
                                                                                                  class [mscorlib]System.Security.Cryptography.ICryptoTransform,
                                                                                                  valuetype [mscorlib]System.Security.Cryptography.CryptoStreamMode)
    IL_0019:  stloc.1
    .try
    {
      IL_001a:  ldloc.1
      IL_001b:  ldarg.0
      IL_001c:  ldfld      uint8[] Beetle.ByteArraySegment::a
      IL_0021:  ldarg.0
      IL_0022:  ldfld      int32 Beetle.ByteArraySegment::Offset
      IL_0027:  ldarg.0
      IL_0028:  ldfld      int32 Beetle.ByteArraySegment::Count
      IL_002d:  callvirt   instance void [mscorlib]System.IO.Stream::Write(uint8[],
                                                                           int32,
                                                                           int32)
      IL_0032:  ldloc.1
      IL_0033:  callvirt   instance void [mscorlib]System.Security.Cryptography.CryptoStream::FlushFinalBlock()
      IL_0038:  ldarg.1
      IL_0039:  ldc.i4.0
      IL_003a:  ldloc.0
      IL_003b:  callvirt   instance int64 [mscorlib]System.IO.Stream::get_Position()
      IL_0040:  conv.i4
      IL_0041:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(int32,
                                                                          int32)
      IL_0046:  ldloc.1
      IL_0047:  callvirt   instance void [mscorlib]System.IO.Stream::Close()
      IL_004c:  leave.s    IL_0058

    }  // end .try
    finally
    {
      IL_004e:  ldloc.1
      IL_004f:  brfalse.s  IL_0057

      IL_0051:  ldloc.1
      IL_0052:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0057:  endfinally
    }  // end handler
    IL_0058:  ret
  } // end of method ByteArraySegment::EncryptTo

  .method public hidebysig instance void 
          DecryptTo(class Beetle.ByteArraySegment segment,
                    class [mscorlib]System.Security.Cryptography.DESCryptoServiceProvider mDESProvider) cil managed
  {
    // 代码大小       96 (0x60)
    .maxstack  4
    .locals init (int32 V_0,
             class [mscorlib]System.IO.MemoryStream V_1,
             int32 V_2,
             class [mscorlib]System.Security.Cryptography.CryptoStream V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_000c:  ldarg.0
    IL_000d:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0012:  newobj     instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[],
                                                                               int32,
                                                                               int32)
    IL_0017:  stloc.1
    IL_0018:  ldc.i4.0
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldarg.2
    IL_001c:  callvirt   instance class [mscorlib]System.Security.Cryptography.ICryptoTransform [mscorlib]System.Security.Cryptography.SymmetricAlgorithm::CreateDecryptor()
    IL_0021:  ldc.i4.0
    IL_0022:  newobj     instance void [mscorlib]System.Security.Cryptography.CryptoStream::.ctor(class [mscorlib]System.IO.Stream,
                                                                                                  class [mscorlib]System.Security.Cryptography.ICryptoTransform,
                                                                                                  valuetype [mscorlib]System.Security.Cryptography.CryptoStreamMode)
    IL_0027:  stloc.3
    .try
    {
      IL_0028:  ldloc.3
      IL_0029:  callvirt   instance int32 [mscorlib]System.IO.Stream::ReadByte()
      IL_002e:  conv.u1
      IL_002f:  stloc.0
      IL_0030:  br.s       IL_0047

      IL_0032:  ldarg.1
      IL_0033:  ldfld      uint8[] Beetle.ByteArraySegment::a
      IL_0038:  ldloc.2
      IL_0039:  ldloc.0
      IL_003a:  conv.u1
      IL_003b:  stelem.i1
      IL_003c:  ldloc.2
      IL_003d:  ldc.i4.1
      IL_003e:  add
      IL_003f:  stloc.2
      IL_0040:  ldloc.3
      IL_0041:  callvirt   instance int32 [mscorlib]System.IO.Stream::ReadByte()
      IL_0046:  stloc.0
      IL_0047:  ldloc.0
      IL_0048:  ldc.i4.0
      IL_0049:  bge.s      IL_0032

      IL_004b:  ldarg.1
      IL_004c:  ldc.i4.0
      IL_004d:  ldloc.2
      IL_004e:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(int32,
                                                                          int32)
      IL_0053:  leave.s    IL_005f

    }  // end .try
    finally
    {
      IL_0055:  ldloc.3
      IL_0056:  brfalse.s  IL_005e

      IL_0058:  ldloc.3
      IL_0059:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_005e:  endfinally
    }  // end handler
    IL_005f:  ret
  } // end of method ByteArraySegment::DecryptTo

  .property instance uint8[] Array()
  {
    .get instance uint8[] Beetle.ByteArraySegment::get_Array()
    .set instance void Beetle.ByteArraySegment::a(uint8[])
  } // end of property ByteArraySegment::Array
  .property instance int32 BufferLength()
  {
    .get instance int32 Beetle.ByteArraySegment::get_BufferLength()
  } // end of property ByteArraySegment::BufferLength
} // end of class Beetle.ByteArraySegment

.class public auto ansi beforefieldinit Beetle.StringMessage
       extends [mscorlib]System.Object
       implements Beetle.IMessage
{
  .field public string Value
  .method public hidebysig newslot virtual final 
          instance void  Save(class Beetle.BufferWriter writer) cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  4
    .locals init (uint8[] V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldfld      class [mscorlib]System.Text.Encoding Beetle.BufferWriter::Coding
    IL_0006:  ldarg.0
    IL_0007:  ldfld      string Beetle.StringMessage::Value
    IL_000c:  callvirt   instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
    IL_0011:  stloc.0
    IL_0012:  ldarg.1
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  ldloc.0
    IL_0016:  ldlen
    IL_0017:  conv.i4
    IL_0018:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_001d:  ret
  } // end of method StringMessage::Save

  .method public hidebysig newslot virtual final 
          instance void  Load(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance int32 Beetle.BufferReader::get_Count()
    IL_0008:  callvirt   instance string Beetle.BufferReader::ReadString(int32)
    IL_000d:  stfld      string Beetle.StringMessage::Value
    IL_0012:  ret
  } // end of method StringMessage::Load

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method StringMessage::.ctor

} // end of class Beetle.StringMessage

.class public auto ansi sealed Beetle.EventPacketRecievMessage
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method EventPacketRecievMessage::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke(class Beetle.PacketRecieveMessagerArgs e) runtime managed
  {
  } // end of method EventPacketRecievMessage::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(class Beetle.PacketRecieveMessagerArgs e,
                      class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method EventPacketRecievMessage::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method EventPacketRecievMessage::EndInvoke

} // end of class Beetle.EventPacketRecievMessage

.class public auto ansi beforefieldinit Beetle.PacketRecieveMessagerArgs
       extends [mscorlib]System.Object
{
  .field public object TypeTag
  .field public bool CleanMessage
  .field public class Beetle.TcpChannel Channel
  .field public object Message
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class Beetle.TcpChannel channel,
                               class Beetle.IMessage msg) cil managed
  {
    // 代码大小       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      class Beetle.TcpChannel Beetle.PacketRecieveMessagerArgs::Channel
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      object Beetle.PacketRecieveMessagerArgs::Message
    IL_0014:  ret
  } // end of method PacketRecieveMessagerArgs::.ctor

} // end of class Beetle.PacketRecieveMessagerArgs

.class public auto ansi beforefieldinit Beetle.LICENSE
       extends [mscorlib]System.Object
{
  .field private static class Beetle.LICENSE a
  .field private static class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider b
  .field private string c
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private string d
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private valuetype [mscorlib]System.DateTime e
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 f
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance string 
          get_Company() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Beetle.LICENSE::c
    IL_0006:  ret
  } // end of method LICENSE::get_Company

  .method public hidebysig specialname instance void 
          set_Company(string 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string Beetle.LICENSE::c
    IL_0007:  ret
  } // end of method LICENSE::set_Company

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    // 代码大小       128 (0x80)
    .maxstack  3
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "Version:{0}\r\n"
    IL_000c:  ldarg.0
    IL_000d:  call       instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
    IL_0012:  callvirt   instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()
    IL_0017:  callvirt   instance class [mscorlib]System.Reflection.AssemblyName [mscorlib]System.Reflection.Assembly::GetName()
    IL_001c:  callvirt   instance class [mscorlib]System.Version [mscorlib]System.Reflection.AssemblyName::get_Version()
    IL_0021:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string,
                                                                                                                              object)
    IL_0026:  pop
    IL_0027:  ldloc.0
    IL_0028:  ldstr      "Company:{0}\r\n"
    IL_002d:  ldarg.0
    IL_002e:  call       instance string Beetle.LICENSE::get_Company()
    IL_0033:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string,
                                                                                                                              object)
    IL_0038:  pop
    IL_0039:  ldloc.0
    IL_003a:  ldstr      "EMail:{0}\r\n"
    IL_003f:  ldarg.0
    IL_0040:  call       instance string Beetle.LICENSE::get_EMail()
    IL_0045:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string,
                                                                                                                              object)
    IL_004a:  pop
    IL_004b:  ldloc.0
    IL_004c:  ldstr      "ValidDate:{0}\r\n"
    IL_0051:  ldarg.0
    IL_0052:  call       instance valuetype [mscorlib]System.DateTime Beetle.LICENSE::get_ValidDate()
    IL_0057:  box        [mscorlib]System.DateTime
    IL_005c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string,
                                                                                                                              object)
    IL_0061:  pop
    IL_0062:  ldloc.0
    IL_0063:  ldstr      "MaxConnections:{0}\r\n"
    IL_0068:  ldarg.0
    IL_0069:  call       instance int32 Beetle.LICENSE::get_MaxConnections()
    IL_006e:  box        [mscorlib]System.Int32
    IL_0073:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string,
                                                                                                                              object)
    IL_0078:  pop
    IL_0079:  ldloc.0
    IL_007a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_007f:  ret
  } // end of method LICENSE::ToString

  .method public hidebysig specialname instance string 
          get_EMail() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Beetle.LICENSE::d
    IL_0006:  ret
  } // end of method LICENSE::get_EMail

  .method public hidebysig specialname instance void 
          set_EMail(string 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string Beetle.LICENSE::d
    IL_0007:  ret
  } // end of method LICENSE::set_EMail

  .method public hidebysig specialname instance valuetype [mscorlib]System.DateTime 
          get_ValidDate() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype [mscorlib]System.DateTime Beetle.LICENSE::e
    IL_0006:  ret
  } // end of method LICENSE::get_ValidDate

  .method public hidebysig specialname instance void 
          set_ValidDate(valuetype [mscorlib]System.DateTime 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      valuetype [mscorlib]System.DateTime Beetle.LICENSE::e
    IL_0007:  ret
  } // end of method LICENSE::set_ValidDate

  .method public hidebysig specialname instance int32 
          get_MaxConnections() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Beetle.LICENSE::f
    IL_0006:  ret
  } // end of method LICENSE::get_MaxConnections

  .method public hidebysig specialname instance void 
          set_MaxConnections(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 Beetle.LICENSE::f
    IL_0007:  ret
  } // end of method LICENSE::set_MaxConnections

  .method public hidebysig static class Beetle.LICENSE 
          GetLICENSE() cil managed
  {
    // 代码大小       303 (0x12f)
    .maxstack  4
    .locals init (string V_0,
             class [mscorlib]System.IO.StreamReader V_1,
             string[] V_2,
             string[] V_3,
             class [mscorlib]System.Exception V_4,
             class [mscorlib]System.Type V_5,
             char[] V_6,
             char[] V_7)
    IL_0000:  ldtoken    Beetle.LICENSE
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  dup
    IL_000b:  stloc.s    V_5
    IL_000d:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0012:  ldsfld     class Beetle.LICENSE Beetle.LICENSE::a
      IL_0017:  brtrue     IL_011f

      IL_001c:  newobj     instance void Beetle.LICENSE::.ctor()
      IL_0021:  stsfld     class Beetle.LICENSE Beetle.LICENSE::a
      .try
      {
        IL_0026:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider Beetle.LICENSE::b
        IL_002b:  ldstr      "<RSAKeyValue><Modulus>mAJtZbCEOO4yg+i0Gec1LDtRTOCZHqo0yy+Q+5UPwTuk+I4y7FZ5ohgmCqNVdNNzNsj3IUgggWXbAJI58XIvXeLqrPZ64Vn5DnTSXKWttzoH6pMWFSgKnmROBthgHxA/dgY7yNUZdmXo+yQaj9ehMDCfl3H0fWnZUE428yEb7js=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
        IL_0030:  callvirt   instance void [mscorlib]System.Security.Cryptography.AsymmetricAlgorithm::FromXmlString(string)
        IL_0035:  ldtoken    Beetle.LICENSE
        IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_003f:  callvirt   instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()
        IL_0044:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_Location()
        IL_0049:  call       string [mscorlib]System.IO.Path::GetDirectoryName(string)
        IL_004e:  ldsfld     char [mscorlib]System.IO.Path::DirectorySeparatorChar
        IL_0053:  box        [mscorlib]System.Char
        IL_0058:  ldstr      "license.sn"
        IL_005d:  call       string [mscorlib]System.String::Concat(object,
                                                                    object,
                                                                    object)
        IL_0062:  stloc.0
        IL_0063:  ldloc.0
        IL_0064:  newobj     instance void [mscorlib]System.IO.StreamReader::.ctor(string)
        IL_0069:  stloc.1
        .try
        {
          IL_006a:  ldloc.1
          IL_006b:  callvirt   instance string [mscorlib]System.IO.TextReader::ReadToEnd()
          IL_0070:  ldc.i4.2
          IL_0071:  newarr     [mscorlib]System.Char
          IL_0076:  stloc.s    V_6
          IL_0078:  ldloc.s    V_6
          IL_007a:  ldc.i4.0
          IL_007b:  ldc.i4.s   13
          IL_007d:  stelem.i2
          IL_007e:  ldloc.s    V_6
          IL_0080:  ldc.i4.1
          IL_0081:  ldc.i4.s   10
          IL_0083:  stelem.i2
          IL_0084:  ldloc.s    V_6
          IL_0086:  ldc.i4.1
          IL_0087:  callvirt   instance string[] [mscorlib]System.String::Split(char[],
                                                                                valuetype [mscorlib]System.StringSplitOptions)
          IL_008c:  stloc.2
          IL_008d:  ldloc.2
          IL_008e:  ldc.i4.0
          IL_008f:  ldelem.ref
          IL_0090:  ldloc.2
          IL_0091:  ldc.i4.1
          IL_0092:  ldelem.ref
          IL_0093:  call       bool Beetle.LICENSE::a(string,
                                                      string)
          IL_0098:  brfalse.s  IL_0105

          IL_009a:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
          IL_009f:  ldloc.2
          IL_00a0:  ldc.i4.0
          IL_00a1:  ldelem.ref
          IL_00a2:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
          IL_00a7:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
          IL_00ac:  ldc.i4.1
          IL_00ad:  newarr     [mscorlib]System.Char
          IL_00b2:  stloc.s    V_7
          IL_00b4:  ldloc.s    V_7
          IL_00b6:  ldc.i4.0
          IL_00b7:  ldc.i4.s   59
          IL_00b9:  stelem.i2
          IL_00ba:  ldloc.s    V_7
          IL_00bc:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
          IL_00c1:  stloc.3
          IL_00c2:  ldsfld     class Beetle.LICENSE Beetle.LICENSE::a
          IL_00c7:  ldloc.3
          IL_00c8:  ldc.i4.0
          IL_00c9:  ldelem.ref
          IL_00ca:  callvirt   instance void Beetle.LICENSE::set_Company(string)
          IL_00cf:  ldsfld     class Beetle.LICENSE Beetle.LICENSE::a
          IL_00d4:  ldloc.3
          IL_00d5:  ldc.i4.1
          IL_00d6:  ldelem.ref
          IL_00d7:  callvirt   instance void Beetle.LICENSE::set_EMail(string)
          IL_00dc:  ldsfld     class Beetle.LICENSE Beetle.LICENSE::a
          IL_00e1:  ldloc.3
          IL_00e2:  ldc.i4.2
          IL_00e3:  ldelem.ref
          IL_00e4:  call       int32 [mscorlib]System.Int32::Parse(string)
          IL_00e9:  callvirt   instance void Beetle.LICENSE::set_MaxConnections(int32)
          IL_00ee:  ldsfld     class Beetle.LICENSE Beetle.LICENSE::a
          IL_00f3:  ldloc.3
          IL_00f4:  ldc.i4.3
          IL_00f5:  ldelem.ref
          IL_00f6:  call       int64 [mscorlib]System.Int64::Parse(string)
          IL_00fb:  newobj     instance void [mscorlib]System.DateTime::.ctor(int64)
          IL_0100:  callvirt   instance void Beetle.LICENSE::set_ValidDate(valuetype [mscorlib]System.DateTime)
          IL_0105:  leave.s    IL_0111

        }  // end .try
        finally
        {
          IL_0107:  ldloc.1
          IL_0108:  brfalse.s  IL_0110

          IL_010a:  ldloc.1
          IL_010b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          IL_0110:  endfinally
        }  // end handler
        IL_0111:  leave.s    IL_011f

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0113:  stloc.s    V_4
        IL_0115:  ldloc.s    V_4
        IL_0117:  callvirt   instance string [mscorlib]System.Exception::get_Message()
        IL_011c:  pop
        IL_011d:  leave.s    IL_011f

      }  // end handler
      IL_011f:  leave.s    IL_0129

    }  // end .try
    finally
    {
      IL_0121:  ldloc.s    V_5
      IL_0123:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0128:  endfinally
    }  // end handler
    IL_0129:  ldsfld     class Beetle.LICENSE Beetle.LICENSE::a
    IL_012e:  ret
  } // end of method LICENSE::GetLICENSE

  .method assembly hidebysig static string 
          b(string A_0) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
    IL_000b:  call       uint8[] Beetle.LICENSE::b(uint8[])
    IL_0010:  call       string [mscorlib]System.Convert::ToBase64String(uint8[])
    IL_0015:  ret
  } // end of method LICENSE::b

  .method assembly hidebysig static bool 
          a(string A_0,
            string A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
    IL_000b:  ldarg.1
    IL_000c:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0011:  call       bool Beetle.LICENSE::a(uint8[],
                                                uint8[])
    IL_0016:  ret
  } // end of method LICENSE::a

  .method assembly hidebysig static bool 
          a(uint8[] A_0,
            uint8[] A_1) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider Beetle.LICENSE::b
    IL_0005:  ldarg.0
    IL_0006:  ldstr      "MD5"
    IL_000b:  ldarg.1
    IL_000c:  callvirt   instance bool [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::VerifyData(uint8[],
                                                                                                                   object,
                                                                                                                   uint8[])
    IL_0011:  ret
  } // end of method LICENSE::a

  .method assembly hidebysig static string 
          a(string A_0) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_000b:  call       uint8[] Beetle.LICENSE::a(uint8[])
    IL_0010:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
    IL_0015:  ret
  } // end of method LICENSE::a

  .method assembly hidebysig static uint8[] 
          b(uint8[] A_0) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider Beetle.LICENSE::b
    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.0
    IL_0007:  callvirt   instance uint8[] [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::Encrypt(uint8[],
                                                                                                                   bool)
    IL_000c:  ret
  } // end of method LICENSE::b

  .method assembly hidebysig static uint8[] 
          a(uint8[] A_0) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider Beetle.LICENSE::b
    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.0
    IL_0007:  callvirt   instance uint8[] [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::Decrypt(uint8[],
                                                                                                                   bool)
    IL_000c:  ret
  } // end of method LICENSE::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method LICENSE::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  stsfld     class Beetle.LICENSE Beetle.LICENSE::a
    IL_0006:  ldc.i4     0x800
    IL_000b:  newobj     instance void [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::.ctor(int32)
    IL_0010:  stsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider Beetle.LICENSE::b
    IL_0015:  ret
  } // end of method LICENSE::.cctor

  .property instance string Company()
  {
    .set instance void Beetle.LICENSE::set_Company(string)
    .get instance string Beetle.LICENSE::get_Company()
  } // end of property LICENSE::Company
  .property instance string EMail()
  {
    .get instance string Beetle.LICENSE::get_EMail()
    .set instance void Beetle.LICENSE::set_EMail(string)
  } // end of property LICENSE::EMail
  .property instance valuetype [mscorlib]System.DateTime
          ValidDate()
  {
    .get instance valuetype [mscorlib]System.DateTime Beetle.LICENSE::get_ValidDate()
    .set instance void Beetle.LICENSE::set_ValidDate(valuetype [mscorlib]System.DateTime)
  } // end of property LICENSE::ValidDate
  .property instance int32 MaxConnections()
  {
    .get instance int32 Beetle.LICENSE::get_MaxConnections()
    .set instance void Beetle.LICENSE::set_MaxConnections(int32)
  } // end of property LICENSE::MaxConnections
} // end of class Beetle.LICENSE

.class public auto ansi beforefieldinit Beetle.TcpChannel
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .field static assembly literal string a = "<Exponent>{0}</Exponent>"
  .field private static int64 b
  .field assembly bool c
  .field public bool SynSend
  .field assembly int32 d
  .field assembly int32 e
  .field assembly class Beetle.TcpServer f
  .field private class [Smark.Core]Smark.Core.KeyValueCollection`1<object> g
  .field public string CloseStatus
  .field public int64 ReceiveBytes
  .field public int64 SendBytes
  .field public bool EnabledSendCompeletedEvent
  .field public int32 TickCount
  .field public class Beetle.EventSendMessageCompleted SendMessageCompleted
  .field assembly class Beetle.Package h
  .field public class Beetle.EventDataReceive DataReceive
  .field public class Beetle.EventMessageWriter MessageWriting
  .field public class Beetle.EventMessageWriter MessageWrited
  .field private class Beetle.EventChannelError i
  .field private class Beetle.EventChannelDisposed j
  .field static assembly string k
  .field public bool EnabledVariant
  .field assembly class z l
  .field assembly class l m
  .field assembly class l n
  .field assembly class ag o
  .field assembly class ag p
  .field assembly class ag q
  .field assembly class Beetle.ChannelReceiveEventArgs r
  .field private class [System]System.Net.Sockets.Socket s
  .field private bool t
  .field private class p u
  .field private class ac v
  .field private class Beetle.BufferWriter w
  .field assembly class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> x
  .field private class Beetle.IMessage y
  .field private bool z
  .field assembly class Beetle.SendMessageCompletedArgs aa
  .field assembly bool ab
  .field assembly bool ac
  .field assembly bool ad
  .field private valuetype Beetle.ChannelStatus ae
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private valuetype [mscorlib]System.DateTime af
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private object ag
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class [mscorlib]System.Text.Encoding ah
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int64 ai
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class [System]System.Net.IPEndPoint aj
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private string ak
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private hidebysig static void  a(class Beetle.TcpChannel A_0) cil managed
  {
    // 代码大小       72 (0x48)
    .maxstack  3
    .locals init (class [mscorlib]System.Type V_0)
    IL_0000:  ldtoken    Beetle.TcpChannel
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  dup
    IL_000b:  stloc.0
    IL_000c:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0011:  ldarg.0
      IL_0012:  ldstr      "_data"
      IL_0017:  ldc.i4     0xff
      IL_001c:  box        [mscorlib]System.Int32
      IL_0021:  callvirt   instance void Beetle.TcpChannel::set_Item(string,
                                                                     object)
      IL_0026:  ldsfld     int64 Beetle.TcpChannel::b
      IL_002b:  ldc.i4.1
      IL_002c:  conv.i8
      IL_002d:  add
      IL_002e:  stsfld     int64 Beetle.TcpChannel::b
      IL_0033:  ldarg.0
      IL_0034:  ldsfld     int64 Beetle.TcpChannel::b
      IL_0039:  callvirt   instance void Beetle.TcpChannel::a(int64)
      IL_003e:  leave.s    IL_0047

    }  // end .try
    finally
    {
      IL_0040:  ldloc.0
      IL_0041:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0046:  endfinally
    }  // end handler
    IL_0047:  ret
  } // end of method TcpChannel::a

  .method public hidebysig specialname instance class [Smark.Core]Smark.Core.KeyValueCollection`1<object> 
          get_Properties() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> Beetle.TcpChannel::g
    IL_0006:  ret
  } // end of method TcpChannel::get_Properties

  .method public hidebysig specialname instance object 
          get_Item(string name) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> Beetle.TcpChannel::g
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance !0 class [Smark.Core]Smark.Core.KeyValueCollection`1<object>::get_Item(string)
    IL_000c:  ret
  } // end of method TcpChannel::get_Item

  .method public hidebysig specialname instance void 
          set_Item(string name,
                   object 'value') cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> Beetle.TcpChannel::g
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  callvirt   instance void class [Smark.Core]Smark.Core.KeyValueCollection`1<object>::set_Item(string,
                                                                                                           !0)
    IL_000d:  ret
  } // end of method TcpChannel::set_Item

  .method public hidebysig specialname instance valuetype Beetle.ChannelStatus 
          get_Status() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype Beetle.ChannelStatus Beetle.TcpChannel::ae
    IL_0006:  ret
  } // end of method TcpChannel::get_Status

  .method public hidebysig specialname instance void 
          set_Status(valuetype Beetle.ChannelStatus 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      valuetype Beetle.ChannelStatus Beetle.TcpChannel::ae
    IL_0007:  ret
  } // end of method TcpChannel::set_Status

  .method public hidebysig specialname instance valuetype [mscorlib]System.DateTime 
          get_CreateTime() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype [mscorlib]System.DateTime Beetle.TcpChannel::af
    IL_0006:  ret
  } // end of method TcpChannel::get_CreateTime

  .method public hidebysig specialname instance void 
          set_CreateTime(valuetype [mscorlib]System.DateTime 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      valuetype [mscorlib]System.DateTime Beetle.TcpChannel::af
    IL_0007:  ret
  } // end of method TcpChannel::set_CreateTime

  .method public hidebysig specialname instance object 
          get_Tag() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object Beetle.TcpChannel::ag
    IL_0006:  ret
  } // end of method TcpChannel::get_Tag

  .method public hidebysig specialname instance void 
          set_Tag(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      object Beetle.TcpChannel::ag
    IL_0007:  ret
  } // end of method TcpChannel::set_Tag

  .method public hidebysig instance class Beetle.Package 
          SetPackage<(Beetle.Package) T>() cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  call       instance class Beetle.Package Beetle.TcpChannel::SetPackage<!!0>(class Beetle.ChannelController)
    IL_0007:  ret
  } // end of method TcpChannel::SetPackage

  .method public hidebysig instance class Beetle.Package 
          SetPackage<(Beetle.Package) T>(class Beetle.ChannelController controller) cil managed
  {
    // 代码大小       60 (0x3c)
    .maxstack  5
    .locals init (object[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldtoken    !!T
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  ldc.i4.1
    IL_000c:  newarr     [mscorlib]System.Object
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  ldarg.0
    IL_0015:  stelem.ref
    IL_0016:  ldloc.0
    IL_0017:  call       object [mscorlib]System.Activator::CreateInstance(class [mscorlib]System.Type,
                                                                           object[])
    IL_001c:  castclass  Beetle.Package
    IL_0021:  stfld      class Beetle.Package Beetle.TcpChannel::h
    IL_0026:  ldarg.1
    IL_0027:  brfalse.s  IL_0035

    IL_0029:  ldarg.1
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class Beetle.Package Beetle.TcpChannel::h
    IL_0030:  callvirt   instance void Beetle.ChannelController::Register(class Beetle.Package)
    IL_0035:  ldarg.0
    IL_0036:  ldfld      class Beetle.Package Beetle.TcpChannel::h
    IL_003b:  ret
  } // end of method TcpChannel::SetPackage

  .method public hidebysig specialname instance class Beetle.Package 
          get_Package() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.Package Beetle.TcpChannel::h
    IL_0006:  ret
  } // end of method TcpChannel::get_Package

  .method public hidebysig specialname instance void 
          add_ChannelError(class Beetle.EventChannelError 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannelError V_0,
             class Beetle.EventChannelError V_1,
             class Beetle.EventChannelError V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelError Beetle.TcpChannel::i
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannelError
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannelError Beetle.TcpChannel::i
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannelError>(!!0&,
                                                                                                                     !!0,
                                                                                                                     !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpChannel::add_ChannelError

  .method public hidebysig specialname instance void 
          remove_ChannelError(class Beetle.EventChannelError 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannelError V_0,
             class Beetle.EventChannelError V_1,
             class Beetle.EventChannelError V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelError Beetle.TcpChannel::i
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannelError
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannelError Beetle.TcpChannel::i
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannelError>(!!0&,
                                                                                                                     !!0,
                                                                                                                     !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpChannel::remove_ChannelError

  .method public hidebysig specialname instance void 
          add_ChannelDisposed(class Beetle.EventChannelDisposed 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannelDisposed V_0,
             class Beetle.EventChannelDisposed V_1,
             class Beetle.EventChannelDisposed V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelDisposed Beetle.TcpChannel::j
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannelDisposed
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannelDisposed Beetle.TcpChannel::j
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannelDisposed>(!!0&,
                                                                                                                        !!0,
                                                                                                                        !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpChannel::add_ChannelDisposed

  .method public hidebysig specialname instance void 
          remove_ChannelDisposed(class Beetle.EventChannelDisposed 'value') cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (class Beetle.EventChannelDisposed V_0,
             class Beetle.EventChannelDisposed V_1,
             class Beetle.EventChannelDisposed V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelDisposed Beetle.TcpChannel::j
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  Beetle.EventChannelDisposed
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class Beetle.EventChannelDisposed Beetle.TcpChannel::j
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class Beetle.EventChannelDisposed>(!!0&,
                                                                                                                        !!0,
                                                                                                                        !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TcpChannel::remove_ChannelDisposed

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [System]System.Net.Sockets.Socket socket) cil managed
  {
    // 代码大小       284 (0x11c)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.m1
    IL_0002:  stfld      int32 Beetle.TcpChannel::d
    IL_0007:  ldarg.0
    IL_0008:  newobj     instance void class [Smark.Core]Smark.Core.KeyValueCollection`1<object>::.ctor()
    IL_000d:  stfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> Beetle.TcpChannel::g
    IL_0012:  ldarg.0
    IL_0013:  ldstr      ""
    IL_0018:  stfld      string Beetle.TcpChannel::CloseStatus
    IL_001d:  ldarg.0
    IL_001e:  call       int32 [mscorlib]System.Environment::get_TickCount()
    IL_0023:  stfld      int32 Beetle.TcpChannel::TickCount
    IL_0028:  ldarg.0
    IL_0029:  newobj     instance void Beetle.ChannelReceiveEventArgs::.ctor()
    IL_002e:  stfld      class Beetle.ChannelReceiveEventArgs Beetle.TcpChannel::r
    IL_0033:  ldarg.0
    IL_0034:  newobj     instance void p::.ctor()
    IL_0039:  stfld      class p Beetle.TcpChannel::u
    IL_003e:  ldarg.0
    IL_003f:  newobj     instance void ac::.ctor()
    IL_0044:  stfld      class ac Beetle.TcpChannel::v
    IL_0049:  ldarg.0
    IL_004a:  ldc.i4     0x100
    IL_004f:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>::.ctor(int32)
    IL_0054:  stfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
    IL_0059:  ldarg.0
    IL_005a:  newobj     instance void Beetle.SendMessageCompletedArgs::.ctor()
    IL_005f:  stfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
    IL_0064:  ldarg.0
    IL_0065:  call       instance void [mscorlib]System.Object::.ctor()
    IL_006a:  ldsfld     bool Beetle.TcpUtils::a
    IL_006f:  brtrue.s   IL_0077

    IL_0071:  call       class Beetle.NetTcpException Beetle.NetTcpException::NotInitialize()
    IL_0076:  throw

    IL_0077:  ldarg.0
    IL_0078:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_007d:  call       instance void Beetle.TcpChannel::set_Coding(class [mscorlib]System.Text.Encoding)
    IL_0082:  ldarg.0
    IL_0083:  ldarg.1
    IL_0084:  stfld      class [System]System.Net.Sockets.Socket Beetle.TcpChannel::s
    IL_0089:  ldarg.0
    IL_008a:  call       class l l::b()
    IL_008f:  stfld      class l Beetle.TcpChannel::m
    IL_0094:  ldarg.0
    IL_0095:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_009a:  call       instance void Beetle.TcpChannel::set_CreateTime(valuetype [mscorlib]System.DateTime)
    IL_009f:  ldarg.0
    IL_00a0:  ldsfld     class l Beetle.TcpUtils::b
    IL_00a5:  stfld      class l Beetle.TcpChannel::n
    IL_00aa:  ldarg.0
    IL_00ab:  call       class ag Beetle.TcpUtils::b()
    IL_00b0:  stfld      class ag Beetle.TcpChannel::o
    IL_00b5:  ldarg.0
    IL_00b6:  call       class ag Beetle.TcpUtils::d()
    IL_00bb:  stfld      class ag Beetle.TcpChannel::p
    IL_00c0:  ldarg.0
    IL_00c1:  call       class ag Beetle.TcpUtils::c()
    IL_00c6:  stfld      class ag Beetle.TcpChannel::q
    IL_00cb:  ldarg.0
    IL_00cc:  call       void Beetle.TcpChannel::a(class Beetle.TcpChannel)
    IL_00d1:  ldarg.0
    IL_00d2:  ldarg.1
    IL_00d3:  callvirt   instance class [System]System.Net.EndPoint [System]System.Net.Sockets.Socket::get_RemoteEndPoint()
    IL_00d8:  castclass  [System]System.Net.IPEndPoint
    IL_00dd:  call       instance void Beetle.TcpChannel::a(class [System]System.Net.IPEndPoint)
    IL_00e2:  ldarg.0
    IL_00e3:  ldarg.0
    IL_00e4:  call       instance class [mscorlib]System.Text.Encoding Beetle.TcpChannel::get_Coding()
    IL_00e9:  ldarg.0
    IL_00ea:  ldfld      class l Beetle.TcpChannel::n
    IL_00ef:  newobj     instance void Beetle.BufferWriter::.ctor(class [mscorlib]System.Text.Encoding,
                                                                  class l)
    IL_00f4:  stfld      class Beetle.BufferWriter Beetle.TcpChannel::w
    IL_00f9:  ldarg.0
    IL_00fa:  ldfld      class Beetle.ChannelReceiveEventArgs Beetle.TcpChannel::r
    IL_00ff:  ldarg.0
    IL_0100:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_0105:  ldarg.0
    IL_0106:  call       class z z::f()
    IL_010b:  stfld      class z Beetle.TcpChannel::l
    IL_0110:  ldarg.0
    IL_0111:  ldsfld     bool b::c
    IL_0116:  stfld      bool Beetle.TcpChannel::c
    IL_011b:  ret
  } // end of method TcpChannel::.ctor

  .method public hidebysig specialname instance class [System]System.Net.Sockets.Socket 
          get_Socket() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Net.Sockets.Socket Beetle.TcpChannel::s
    IL_0006:  ret
  } // end of method TcpChannel::get_Socket

  .method assembly hidebysig instance void 
          a(bool A_0) cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      bool Beetle.TcpChannel::t
    IL_0007:  ret
  } // end of method TcpChannel::a

  .method public hidebysig instance bool 
          Send(object message) cil managed
  {
    // 代码大小       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_0005

    IL_0003:  ldc.i4.0
    IL_0004:  ret

    IL_0005:  ldarg.0
    IL_0006:  call       instance class Beetle.Package Beetle.TcpChannel::get_Package()
    IL_000b:  brfalse.s  IL_0025

    IL_000d:  ldarg.0
    IL_000e:  ldarg.0
    IL_000f:  call       instance class Beetle.Package Beetle.TcpChannel::get_Package()
    IL_0014:  ldarg.1
    IL_0015:  callvirt   instance object Beetle.Package::WriteCast(object)
    IL_001a:  castclass  Beetle.IMessage
    IL_001f:  call       instance bool Beetle.TcpChannel::a(class Beetle.IMessage)
    IL_0024:  ret

    IL_0025:  ldarg.0
    IL_0026:  ldarg.1
    IL_0027:  castclass  Beetle.IMessage
    IL_002c:  call       instance bool Beetle.TcpChannel::a(class Beetle.IMessage)
    IL_0031:  ret
  } // end of method TcpChannel::Send

  .method private hidebysig instance bool 
          a(class Beetle.IMessage A_0) cil managed
  {
    // 代码大小       152 (0x98)
    .maxstack  2
    .locals init (bool V_0,
             class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
    IL_0006:  brfalse.s  IL_0018

    IL_0008:  ldarg.0
    IL_0009:  ldfld      bool Beetle.TcpChannel::ad
    IL_000e:  brtrue.s   IL_0018

    IL_0010:  ldarg.0
    IL_0011:  ldfld      bool Beetle.TcpChannel::ab
    IL_0016:  brfalse.s  IL_002d

    IL_0018:  ldarg.1
    IL_0019:  isinst     Beetle.ICounter
    IL_001e:  brfalse.s  IL_002b

    IL_0020:  ldarg.1
    IL_0021:  castclass  Beetle.ICounter
    IL_0026:  callvirt   instance void Beetle.ICounter::Reduce()
    IL_002b:  ldc.i4.0
    IL_002c:  ret

    IL_002d:  ldarg.0
    IL_002e:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
    IL_0033:  dup
    IL_0034:  stloc.1
    IL_0035:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_003a:  ldarg.1
      IL_003b:  brfalse.s  IL_0049

      IL_003d:  ldarg.0
      IL_003e:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
      IL_0043:  ldarg.1
      IL_0044:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>::Enqueue(!0)
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
      IL_004f:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>::get_Count()
      IL_0054:  ldc.i4.0
      IL_0055:  ble.s      IL_008b

      IL_0057:  ldarg.0
      IL_0058:  ldfld      bool Beetle.TcpChannel::t
      IL_005d:  brtrue.s   IL_008b

      IL_005f:  ldarg.0
      IL_0060:  ldfld      bool Beetle.TcpChannel::ad
      IL_0065:  brtrue.s   IL_008b

      IL_0067:  ldarg.0
      IL_0068:  ldfld      class p Beetle.TcpChannel::u
      IL_006d:  ldarg.0
      IL_006e:  stfld      class Beetle.TcpChannel ad::f
      IL_0073:  ldarg.0
      IL_0074:  ldfld      class ag Beetle.TcpChannel::p
      IL_0079:  ldarg.0
      IL_007a:  ldfld      class p Beetle.TcpChannel::u
      IL_007f:  callvirt   instance void ag::a(class ad)
      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.1
      IL_0086:  call       instance void Beetle.TcpChannel::a(bool)
      IL_008b:  ldc.i4.1
      IL_008c:  stloc.0
      IL_008d:  leave.s    IL_0096

    }  // end .try
    finally
    {
      IL_008f:  ldloc.1
      IL_0090:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0095:  endfinally
    }  // end handler
    IL_0096:  ldloc.0
    IL_0097:  ret
  } // end of method TcpChannel::a

  .method assembly hidebysig instance void 
          d() cil managed
  {
    // 代码大小       354 (0x162)
    .maxstack  3
    .locals init (class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> V_0,
             object V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldfld      bool Beetle.TcpChannel::ad
    IL_000e:  brfalse.s  IL_0011

    IL_0010:  ret

    IL_0011:  ldarg.0
    IL_0012:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
    IL_0017:  dup
    IL_0018:  stloc.0
    IL_0019:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_001e:  ldarg.0
      IL_001f:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
      IL_0024:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>::get_Count()
      IL_0029:  brtrue.s   IL_0030

      IL_002b:  leave      IL_0161

      IL_0030:  ldarg.0
      IL_0031:  ldfld      bool Beetle.TcpChannel::EnabledSendCompeletedEvent
      IL_0036:  brfalse.s  IL_005b

      IL_0038:  ldarg.0
      IL_0039:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
      IL_003e:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
      IL_0043:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.IMessage>::get_Count()
      IL_0048:  ldc.i4.0
      IL_0049:  ble.s      IL_005b

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
      IL_0051:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
      IL_0056:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.IMessage>::Clear()
      IL_005b:  ldarg.0
      IL_005c:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
      IL_0061:  callvirt   instance void Beetle.BufferWriter::Reset()
      IL_0066:  ldarg.0
      IL_0067:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
      IL_006c:  ldarg.0
      IL_006d:  ldfld      bool Beetle.TcpChannel::EnabledVariant
      IL_0072:  stfld      bool Beetle.BufferWriter::EnabledVariant
      IL_0077:  br         IL_013b

      IL_007c:  ldarg.0
      IL_007d:  ldarg.0
      IL_007e:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
      IL_0083:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>::Dequeue()
      IL_0088:  stfld      class Beetle.IMessage Beetle.TcpChannel::y
      IL_008d:  ldarg.0
      IL_008e:  ldfld      bool Beetle.TcpChannel::EnabledSendCompeletedEvent
      IL_0093:  brfalse.s  IL_00ab

      IL_0095:  ldarg.0
      IL_0096:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
      IL_009b:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
      IL_00a0:  ldarg.0
      IL_00a1:  ldfld      class Beetle.IMessage Beetle.TcpChannel::y
      IL_00a6:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.IMessage>::Add(!0)
      IL_00ab:  ldarg.0
      IL_00ac:  ldfld      class Beetle.EventMessageWriter Beetle.TcpChannel::MessageWriting
      IL_00b1:  brtrue.s   IL_00c6

      IL_00b3:  ldarg.0
      IL_00b4:  ldfld      class Beetle.IMessage Beetle.TcpChannel::y
      IL_00b9:  ldarg.0
      IL_00ba:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
      IL_00bf:  callvirt   instance void Beetle.IMessage::Save(class Beetle.BufferWriter)
      IL_00c4:  br.s       IL_011e

      IL_00c6:  ldarg.0
      IL_00c7:  ldfld      class Beetle.EventMessageWriter Beetle.TcpChannel::MessageWriting
      IL_00cc:  ldarg.0
      IL_00cd:  ldfld      class Beetle.IMessage Beetle.TcpChannel::y
      IL_00d2:  ldarg.0
      IL_00d3:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
      IL_00d8:  callvirt   instance void Beetle.EventMessageWriter::Invoke(class Beetle.IMessage,
                                                                           class Beetle.BufferWriter)
      IL_00dd:  ldsfld     object Beetle.TcpUtils::k
      IL_00e2:  dup
      IL_00e3:  stloc.1
      IL_00e4:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
      .try
      {
        IL_00e9:  ldsfld     int64 Beetle.TcpUtils::SendMessages
        IL_00ee:  ldc.i4.1
        IL_00ef:  conv.i8
        IL_00f0:  add
        IL_00f1:  stsfld     int64 Beetle.TcpUtils::SendMessages
        IL_00f6:  leave.s    IL_00ff

      }  // end .try
      finally
      {
        IL_00f8:  ldloc.1
        IL_00f9:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
        IL_00fe:  endfinally
      }  // end handler
      IL_00ff:  ldarg.0
      IL_0100:  ldfld      class Beetle.EventMessageWriter Beetle.TcpChannel::MessageWrited
      IL_0105:  brfalse.s  IL_011e

      IL_0107:  ldarg.0
      IL_0108:  ldfld      class Beetle.EventMessageWriter Beetle.TcpChannel::MessageWrited
      IL_010d:  ldarg.0
      IL_010e:  ldfld      class Beetle.IMessage Beetle.TcpChannel::y
      IL_0113:  ldarg.0
      IL_0114:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
      IL_0119:  callvirt   instance void Beetle.EventMessageWriter::Invoke(class Beetle.IMessage,
                                                                           class Beetle.BufferWriter)
      IL_011e:  ldarg.0
      IL_011f:  ldfld      class Beetle.IMessage Beetle.TcpChannel::y
      IL_0124:  isinst     Beetle.ICounter
      IL_0129:  brfalse.s  IL_013b

      IL_012b:  ldarg.0
      IL_012c:  ldfld      class Beetle.IMessage Beetle.TcpChannel::y
      IL_0131:  castclass  Beetle.ICounter
      IL_0136:  callvirt   instance void Beetle.ICounter::Reduce()
      IL_013b:  ldarg.0
      IL_013c:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
      IL_0141:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>::get_Count()
      IL_0146:  ldc.i4.0
      IL_0147:  bgt        IL_007c

      IL_014c:  leave.s    IL_0155

    }  // end .try
    finally
    {
      IL_014e:  ldloc.0
      IL_014f:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0154:  endfinally
    }  // end handler
    IL_0155:  ldarg.0
    IL_0156:  ldarg.0
    IL_0157:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
    IL_015c:  call       instance void Beetle.TcpChannel::b(class Beetle.BufferWriter)
    IL_0161:  ret
  } // end of method TcpChannel::d

  .method private hidebysig instance void 
          a(class Beetle.BufferWriter A_0) cil managed
  {
    // 代码大小       186 (0xba)
    .maxstack  5
    .locals init (class b V_0,
             int32 V_1,
             int32 V_2,
             object V_3)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br         IL_0099

    IL_0009:  ldarg.1
    IL_000a:  callvirt   instance class b Beetle.BufferWriter::c()
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  brfalse    IL_0099

    .try
    {
      IL_0016:  ldc.i4.0
      IL_0017:  stloc.1
      IL_0018:  br.s       IL_0087

      IL_001a:  ldarg.0
      IL_001b:  ldfld      bool Beetle.TcpChannel::ad
      IL_0020:  brtrue.s   IL_0090

      IL_0022:  ldarg.0
      IL_0023:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
      IL_0028:  ldloc.0
      IL_0029:  ldfld      uint8[] b::e
      IL_002e:  ldloc.1
      IL_002f:  ldloc.0
      IL_0030:  ldfld      int32 b::h
      IL_0035:  ldloc.1
      IL_0036:  sub
      IL_0037:  ldc.i4.0
      IL_0038:  callvirt   instance int32 [System]System.Net.Sockets.Socket::Send(uint8[],
                                                                                  int32,
                                                                                  int32,
                                                                                  valuetype [System]System.Net.Sockets.SocketFlags)
      IL_003d:  stloc.2
      IL_003e:  ldloc.1
      IL_003f:  ldloc.2
      IL_0040:  add
      IL_0041:  stloc.1
      IL_0042:  ldsfld     bool Beetle.TcpUtils::Statistics
      IL_0047:  brfalse.s  IL_0087

      IL_0049:  ldsfld     object Beetle.TcpUtils::d
      IL_004e:  dup
      IL_004f:  stloc.3
      IL_0050:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
      .try
      {
        IL_0055:  ldsfld     int64 Beetle.TcpUtils::SendBytes
        IL_005a:  ldloc.2
        IL_005b:  conv.i8
        IL_005c:  add
        IL_005d:  stsfld     int64 Beetle.TcpUtils::SendBytes
        IL_0062:  ldarg.0
        IL_0063:  dup
        IL_0064:  ldfld      int64 Beetle.TcpChannel::SendBytes
        IL_0069:  ldloc.2
        IL_006a:  conv.i8
        IL_006b:  add
        IL_006c:  stfld      int64 Beetle.TcpChannel::SendBytes
        IL_0071:  ldsfld     int64 Beetle.TcpUtils::SendDataIO
        IL_0076:  ldc.i4.1
        IL_0077:  conv.i8
        IL_0078:  add
        IL_0079:  stsfld     int64 Beetle.TcpUtils::SendDataIO
        IL_007e:  leave.s    IL_0087

      }  // end .try
      finally
      {
        IL_0080:  ldloc.3
        IL_0081:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
        IL_0086:  endfinally
      }  // end handler
      IL_0087:  ldloc.1
      IL_0088:  ldloc.0
      IL_0089:  ldfld      int32 b::h
      IL_008e:  blt.s      IL_001a

      IL_0090:  leave.s    IL_0099

    }  // end .try
    finally
    {
      IL_0092:  ldloc.0
      IL_0093:  callvirt   instance void b::Dispose()
      IL_0098:  endfinally
    }  // end handler
    IL_0099:  ldarg.1
    IL_009a:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_009f:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_00a4:  ldc.i4.0
    IL_00a5:  bgt        IL_0009

    IL_00aa:  ldarg.0
    IL_00ab:  ldc.i4.0
    IL_00ac:  call       instance void Beetle.TcpChannel::a(bool)
    IL_00b1:  ldarg.0
    IL_00b2:  ldnull
    IL_00b3:  call       instance bool Beetle.TcpChannel::Send(object)
    IL_00b8:  pop
    IL_00b9:  ret
  } // end of method TcpChannel::a

  .method assembly hidebysig instance void 
          b(class Beetle.BufferWriter A_0) cil managed
  {
    // 代码大小       102 (0x66)
    .maxstack  2
    .locals init (class b V_0,
             class a V_1,
             class [mscorlib]System.Exception V_2)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  ldarg.0
      IL_0003:  ldfld      bool Beetle.TcpChannel::SynSend
      IL_0008:  brfalse.s  IL_0013

      IL_000a:  ldarg.0
      IL_000b:  ldarg.1
      IL_000c:  call       instance void Beetle.TcpChannel::a(class Beetle.BufferWriter)
      IL_0011:  br.s       IL_0057

      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance class b Beetle.BufferWriter::b()
      IL_0019:  stloc.0
      IL_001a:  ldloc.0
      IL_001b:  brfalse.s  IL_0057

      IL_001d:  ldarg.0
      IL_001e:  ldfld      bool Beetle.TcpChannel::ad
      IL_0023:  brtrue.s   IL_0051

      IL_0025:  ldloc.0
      IL_0026:  callvirt   instance class a b::b()
      IL_002b:  stloc.1
      IL_002c:  ldloc.1
      IL_002d:  ldarg.0
      IL_002e:  callvirt   instance void a::a(class Beetle.TcpChannel)
      IL_0033:  ldloc.1
      IL_0034:  ldarg.1
      IL_0035:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::set_UserToken(object)
      IL_003a:  ldarg.0
      IL_003b:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
      IL_0040:  ldloc.1
      IL_0041:  callvirt   instance bool [System]System.Net.Sockets.Socket::SendAsync(class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_0046:  brtrue.s   IL_0057

      IL_0048:  ldloc.1
      IL_0049:  ldloc.1
      IL_004a:  callvirt   instance void a::a(class a)
      IL_004f:  br.s       IL_0057

      IL_0051:  ldloc.0
      IL_0052:  callvirt   instance void b::Dispose()
      IL_0057:  leave.s    IL_0065

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0059:  stloc.2
      IL_005a:  ldloc.0
      IL_005b:  brfalse.s  IL_0063

      IL_005d:  ldloc.0
      IL_005e:  callvirt   instance void b::Dispose()
      IL_0063:  ldloc.2
      IL_0064:  throw

    }  // end handler
    IL_0065:  ret
  } // end of method TcpChannel::b

  .method public hidebysig instance void 
          BeginReceive() cil managed
  {
    // 代码大小       44 (0x2c)
    .maxstack  8
    IL_0000:  ldsfld     bool Beetle.TcpUtils::UseReceiveAsyncQueue
    IL_0005:  brfalse.s  IL_0025

    IL_0007:  ldarg.0
    IL_0008:  ldfld      class ac Beetle.TcpChannel::v
    IL_000d:  ldarg.0
    IL_000e:  stfld      class Beetle.TcpChannel ad::f
    IL_0013:  ldarg.0
    IL_0014:  ldfld      class ag Beetle.TcpChannel::q
    IL_0019:  ldarg.0
    IL_001a:  ldfld      class ac Beetle.TcpChannel::v
    IL_001f:  callvirt   instance void ag::a(class ad)
    IL_0024:  ret

    IL_0025:  ldarg.0
    IL_0026:  call       instance void Beetle.TcpChannel::c()
    IL_002b:  ret
  } // end of method TcpChannel::BeginReceive

  .method assembly hidebysig instance void 
          c() cil managed
  {
    // 代码大小       111 (0x6f)
    .maxstack  3
    .locals init (class a V_0,
             class [mscorlib]System.Exception V_1,
             class Beetle.ChannelErrorEventArgs V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.TcpChannel::ad
    IL_0006:  brtrue.s   IL_006e

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class z Beetle.TcpChannel::l
    IL_000e:  ldfld      bool z::a
    IL_0013:  brfalse.s  IL_006e

    IL_0015:  ldsfld     class h Beetle.TcpUtils::f
    IL_001a:  callvirt   instance class a h::d()
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ldarg.0
    IL_0022:  callvirt   instance void a::a(class Beetle.TcpChannel)
    .try
    {
      IL_0027:  ldarg.0
      IL_0028:  ldarg.0
      IL_0029:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
      IL_002e:  ldloc.0
      IL_002f:  callvirt   instance bool [System]System.Net.Sockets.Socket::ReceiveAsync(class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_0034:  stfld      bool Beetle.TcpChannel::z
      IL_0039:  ldarg.0
      IL_003a:  ldfld      bool Beetle.TcpChannel::z
      IL_003f:  brtrue.s   IL_0048

      IL_0041:  ldloc.0
      IL_0042:  ldloc.0
      IL_0043:  callvirt   instance void a::b(class a)
      IL_0048:  leave.s    IL_006e

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_004a:  stloc.1
      IL_004b:  ldloc.0
      IL_004c:  callvirt   instance void a::d()
      IL_0051:  ldarg.0
      IL_0052:  newobj     instance void Beetle.ChannelErrorEventArgs::.ctor()
      IL_0057:  stloc.2
      IL_0058:  ldloc.2
      IL_0059:  ldarg.0
      IL_005a:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_005f:  ldloc.2
      IL_0060:  ldloc.1
      IL_0061:  callvirt   instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
      IL_0066:  ldloc.2
      IL_0067:  call       instance void Beetle.TcpChannel::CallChannelError(class Beetle.ChannelErrorEventArgs)
      IL_006c:  leave.s    IL_006e

    }  // end handler
    IL_006e:  ret
  } // end of method TcpChannel::c

  .method assembly hidebysig instance void 
          e() cil managed
  {
    // 代码大小       44 (0x2c)
    .maxstack  3
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Beetle.EventSendMessageCompleted Beetle.TcpChannel::SendMessageCompleted
      IL_0006:  brfalse.s  IL_0026

      IL_0008:  ldarg.0
      IL_0009:  ldfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
      IL_000e:  ldarg.0
      IL_000f:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class Beetle.EventSendMessageCompleted Beetle.TcpChannel::SendMessageCompleted
      IL_001a:  ldarg.0
      IL_001b:  ldarg.0
      IL_001c:  ldfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
      IL_0021:  callvirt   instance void Beetle.EventSendMessageCompleted::Invoke(object,
                                                                                  class Beetle.SendMessageCompletedArgs)
      IL_0026:  leave.s    IL_002b

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0028:  pop
      IL_0029:  leave.s    IL_002b

    }  // end handler
    IL_002b:  ret
  } // end of method TcpChannel::e

  .method assembly hidebysig instance void 
          a(class Beetle.ChannelReceiveEventArgs A_0) cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance void Beetle.TcpChannel::OnDataReceive(class Beetle.ChannelReceiveEventArgs)
    IL_0007:  ret
  } // end of method TcpChannel::a

  .method family hidebysig newslot virtual 
          instance void  OnDataReceive(class Beetle.ChannelReceiveEventArgs e) cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventDataReceive Beetle.TcpChannel::DataReceive
    IL_0006:  brfalse.s  IL_001d

    IL_0008:  ldarg.0
    IL_0009:  ldfld      bool Beetle.TcpChannel::ac
    IL_000e:  brtrue.s   IL_001d

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class Beetle.EventDataReceive Beetle.TcpChannel::DataReceive
    IL_0016:  ldarg.0
    IL_0017:  ldarg.1
    IL_0018:  callvirt   instance void Beetle.EventDataReceive::Invoke(object,
                                                                       class Beetle.ChannelReceiveEventArgs)
    IL_001d:  ret
  } // end of method TcpChannel::OnDataReceive

  .method public hidebysig instance void 
          CallChannelError(class Beetle.ChannelErrorEventArgs e) cil managed
  {
    // 代码大小       26 (0x1a)
    .maxstack  3
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  dup
      IL_0002:  ldvirtftn  instance void Beetle.TcpChannel::OnChannelError(object)
      IL_0008:  newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                        native int)
      IL_000d:  ldarg.1
      IL_000e:  call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback,
                                                                                         object)
      IL_0013:  pop
      IL_0014:  leave.s    IL_0019

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0016:  pop
      IL_0017:  leave.s    IL_0019

    }  // end handler
    IL_0019:  ret
  } // end of method TcpChannel::CallChannelError

  .method family hidebysig newslot virtual 
          instance void  OnChannelError(object e) cil managed
  {
    // 代码大小       92 (0x5c)
    .maxstack  3
    .locals init (class Beetle.ChannelErrorEventArgs V_0)
    IL_0000:  ldarg.1
    IL_0001:  castclass  Beetle.ChannelErrorEventArgs
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class Beetle.EventChannelError Beetle.TcpChannel::i
      IL_000d:  brfalse.s  IL_0024

      IL_000f:  ldarg.0
      IL_0010:  ldfld      bool Beetle.TcpChannel::ad
      IL_0015:  brtrue.s   IL_0024

      IL_0017:  ldarg.0
      IL_0018:  ldfld      class Beetle.EventChannelError Beetle.TcpChannel::i
      IL_001d:  ldarg.0
      IL_001e:  ldloc.0
      IL_001f:  callvirt   instance void Beetle.EventChannelError::Invoke(object,
                                                                          class Beetle.ChannelErrorEventArgs)
      IL_0024:  leave.s    IL_0029

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0026:  pop
      IL_0027:  leave.s    IL_0029

    }  // end handler
    .try
    {
      IL_0029:  ldloc.0
      IL_002a:  callvirt   instance class [mscorlib]System.Exception Beetle.ChannelErrorEventArgs::get_Exception()
      IL_002f:  isinst     [System]System.Net.Sockets.SocketException
      IL_0034:  brtrue.s   IL_0050

      IL_0036:  ldloc.0
      IL_0037:  callvirt   instance class [mscorlib]System.Exception Beetle.ChannelErrorEventArgs::get_Exception()
      IL_003c:  isinst     [mscorlib]System.ObjectDisposedException
      IL_0041:  brtrue.s   IL_0050

      IL_0043:  ldloc.0
      IL_0044:  callvirt   instance class [mscorlib]System.Exception Beetle.ChannelErrorEventArgs::get_Exception()
      IL_0049:  isinst     Beetle.NetTcpException
      IL_004e:  brfalse.s  IL_0056

      IL_0050:  ldarg.0
      IL_0051:  call       instance void Beetle.TcpChannel::Dispose()
      IL_0056:  leave.s    IL_005b

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0058:  pop
      IL_0059:  leave.s    IL_005b

    }  // end handler
    IL_005b:  ret
  } // end of method TcpChannel::OnChannelError

  .method family hidebysig newslot virtual 
          instance void  OnChannelDisposed(class Beetle.ChannelDisposedEventArgs e) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.EventChannelDisposed Beetle.TcpChannel::j
    IL_0006:  brfalse.s  IL_0015

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class Beetle.EventChannelDisposed Beetle.TcpChannel::j
    IL_000e:  ldarg.0
    IL_000f:  ldarg.1
    IL_0010:  callvirt   instance void Beetle.EventChannelDisposed::Invoke(object,
                                                                           class Beetle.ChannelDisposedEventArgs)
    IL_0015:  ret
  } // end of method TcpChannel::OnChannelDisposed

  .method public hidebysig specialname instance class [mscorlib]System.Text.Encoding 
          get_Coding() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Text.Encoding Beetle.TcpChannel::ah
    IL_0006:  ret
  } // end of method TcpChannel::get_Coding

  .method public hidebysig specialname instance void 
          set_Coding(class [mscorlib]System.Text.Encoding 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [mscorlib]System.Text.Encoding Beetle.TcpChannel::ah
    IL_0007:  ret
  } // end of method TcpChannel::set_Coding

  .method public hidebysig specialname instance int64 
          get_ClientID() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int64 Beetle.TcpChannel::ai
    IL_0006:  ret
  } // end of method TcpChannel::get_ClientID

  .method assembly hidebysig specialname 
          instance void  a(int64 A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int64 Beetle.TcpChannel::ai
    IL_0007:  ret
  } // end of method TcpChannel::a

  .method private hidebysig instance void 
          a() cil managed
  {
    // 代码大小       38 (0x26)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.TcpServer Beetle.TcpChannel::f
    IL_0006:  brfalse.s  IL_0025

    IL_0008:  ldarg.0
    IL_0009:  ldarg.0
    IL_000a:  ldfld      class Beetle.TcpServer Beetle.TcpChannel::f
    IL_000f:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.TcpChannel> Beetle.TcpServer::o
    IL_0014:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.TcpChannel>::get_Count()
    IL_0019:  ldsfld     int32 l::a
    IL_001e:  cgt
    IL_0020:  stfld      bool Beetle.TcpChannel::ab
    IL_0025:  ret
  } // end of method TcpChannel::a

  .method public hidebysig specialname instance class Beetle.TcpServer 
          get_Server() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.TcpServer Beetle.TcpChannel::f
    IL_0006:  ret
  } // end of method TcpChannel::get_Server

  .method assembly hidebysig specialname 
          instance void  a(class Beetle.TcpServer A_0) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class Beetle.TcpServer Beetle.TcpChannel::f
    IL_0007:  ldarg.0
    IL_0008:  call       instance void Beetle.TcpChannel::a()
    IL_000d:  ret
  } // end of method TcpChannel::a

  .method public hidebysig specialname instance class [System]System.Net.IPEndPoint 
          get_EndPoint() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Net.IPEndPoint Beetle.TcpChannel::aj
    IL_0006:  ret
  } // end of method TcpChannel::get_EndPoint

  .method assembly hidebysig specialname 
          instance void  a(class [System]System.Net.IPEndPoint A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [System]System.Net.IPEndPoint Beetle.TcpChannel::aj
    IL_0007:  ret
  } // end of method TcpChannel::a

  .method public hidebysig specialname instance string 
          get_Name() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Beetle.TcpChannel::ak
    IL_0006:  ret
  } // end of method TcpChannel::get_Name

  .method public hidebysig specialname instance void 
          set_Name(string 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string Beetle.TcpChannel::ak
    IL_0007:  ret
  } // end of method TcpChannel::set_Name

  .method assembly hidebysig instance void 
          b() cil managed
  {
    // 代码大小       252 (0xfc)
    .maxstack  2
    .locals init (class Beetle.IMessage V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  stfld      bool Beetle.TcpChannel::ac
    IL_0007:  ldarg.0
    IL_0008:  call       instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
    IL_000d:  call       void Beetle.TcpServer::ReleaseSocket(class [System]System.Net.Sockets.Socket)
    IL_0012:  ldarg.0
    IL_0013:  ldnull
    IL_0014:  stfld      class [System]System.Net.Sockets.Socket Beetle.TcpChannel::s
    IL_0019:  ldarg.0
    IL_001a:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
    IL_001f:  brfalse.s  IL_002c

    IL_0021:  ldarg.0
    IL_0022:  ldfld      class Beetle.BufferWriter Beetle.TcpChannel::w
    IL_0027:  callvirt   instance void Beetle.BufferWriter::Dispose()
    IL_002c:  ldarg.0
    IL_002d:  ldnull
    IL_002e:  stfld      class l Beetle.TcpChannel::n
    IL_0033:  ldarg.0
    IL_0034:  ldfld      class Beetle.Package Beetle.TcpChannel::h
    IL_0039:  brfalse.s  IL_004d

    IL_003b:  ldarg.0
    IL_003c:  ldfld      class Beetle.Package Beetle.TcpChannel::h
    IL_0041:  callvirt   instance void Beetle.Package::Dispose()
    IL_0046:  ldarg.0
    IL_0047:  ldnull
    IL_0048:  stfld      class Beetle.Package Beetle.TcpChannel::h
    .try
    {
      IL_004d:  br.s       IL_006e

      IL_004f:  ldarg.0
      IL_0050:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
      IL_0055:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>::Dequeue()
      IL_005a:  stloc.0
      IL_005b:  ldloc.0
      IL_005c:  isinst     Beetle.ICounter
      IL_0061:  brfalse.s  IL_006e

      IL_0063:  ldloc.0
      IL_0064:  castclass  Beetle.ICounter
      IL_0069:  callvirt   instance void Beetle.ICounter::Reduce()
      IL_006e:  ldarg.0
      IL_006f:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
      IL_0074:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>::get_Count()
      IL_0079:  ldc.i4.0
      IL_007a:  bgt.s      IL_004f

      IL_007c:  leave.s    IL_0081

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_007e:  pop
      IL_007f:  leave.s    IL_0081

    }  // end handler
    IL_0081:  ldarg.0
    IL_0082:  ldfld      class ac Beetle.TcpChannel::v
    IL_0087:  ldnull
    IL_0088:  stfld      class Beetle.TcpChannel ad::f
    IL_008d:  ldarg.0
    IL_008e:  ldnull
    IL_008f:  stfld      class ac Beetle.TcpChannel::v
    IL_0094:  ldarg.0
    IL_0095:  ldfld      class p Beetle.TcpChannel::u
    IL_009a:  ldnull
    IL_009b:  stfld      class Beetle.TcpChannel ad::f
    IL_00a0:  ldarg.0
    IL_00a1:  ldnull
    IL_00a2:  stfld      class p Beetle.TcpChannel::u
    IL_00a7:  ldarg.0
    IL_00a8:  ldnull
    IL_00a9:  stfld      class Beetle.EventMessageWriter Beetle.TcpChannel::MessageWriting
    IL_00ae:  ldarg.0
    IL_00af:  ldfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> Beetle.TcpChannel::g
    IL_00b4:  callvirt   instance void class [Smark.Core]Smark.Core.KeyValueCollection`1<object>::Clear()
    IL_00b9:  ldarg.0
    IL_00ba:  ldnull
    IL_00bb:  stfld      class l Beetle.TcpChannel::n
    IL_00c0:  ldarg.0
    IL_00c1:  ldnull
    IL_00c2:  stfld      class Beetle.BufferWriter Beetle.TcpChannel::w
    IL_00c7:  ldarg.0
    IL_00c8:  ldfld      class Beetle.ChannelReceiveEventArgs Beetle.TcpChannel::r
    IL_00cd:  ldnull
    IL_00ce:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_00d3:  ldarg.0
    IL_00d4:  ldnull
    IL_00d5:  stfld      class Beetle.ChannelReceiveEventArgs Beetle.TcpChannel::r
    IL_00da:  ldarg.0
    IL_00db:  ldnull
    IL_00dc:  call       instance void Beetle.TcpChannel::a(class Beetle.TcpServer)
    IL_00e1:  ldarg.0
    IL_00e2:  ldnull
    IL_00e3:  call       instance void Beetle.TcpChannel::set_Tag(object)
    IL_00e8:  ldarg.0
    IL_00e9:  ldfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
    IL_00ee:  ldnull
    IL_00ef:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_00f4:  ldarg.0
    IL_00f5:  ldnull
    IL_00f6:  stfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
    IL_00fb:  ret
  } // end of method TcpChannel::b

  .method public hidebysig specialname instance bool 
          get_IsDisposed() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.TcpChannel::ad
    IL_0006:  ret
  } // end of method TcpChannel::get_IsDisposed

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       111 (0x6f)
    .maxstack  2
    .locals init (class Beetle.ChannelDisposedEventArgs V_0,
             class Beetle.TcpChannel V_1)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.1
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool Beetle.TcpChannel::ad
      IL_000e:  brtrue.s   IL_0065

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.1
      IL_0012:  stfld      bool Beetle.TcpChannel::ad
      IL_0017:  newobj     instance void Beetle.ChannelDisposedEventArgs::.ctor()
      IL_001c:  stloc.0
      IL_001d:  ldloc.0
      IL_001e:  ldarg.0
      IL_001f:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.TcpChannel::x
      IL_0024:  callvirt   instance void Beetle.ChannelDisposedEventArgs::a(class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>)
      IL_0029:  ldloc.0
      IL_002a:  ldc.i4.1
      IL_002b:  callvirt   instance void Beetle.ChannelDisposedEventArgs::set_Clean(bool)
      IL_0030:  ldloc.0
      IL_0031:  ldarg.0
      IL_0032:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      .try
      {
        IL_0037:  ldarg.0
        IL_0038:  ldloc.0
        IL_0039:  callvirt   instance void Beetle.TcpChannel::OnChannelDisposed(class Beetle.ChannelDisposedEventArgs)
        IL_003e:  leave.s    IL_0043

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0040:  pop
        IL_0041:  leave.s    IL_0043

      }  // end handler
      IL_0043:  ldloc.0
      IL_0044:  callvirt   instance bool Beetle.ChannelDisposedEventArgs::get_Clean()
      IL_0049:  brfalse.s  IL_005a

      IL_004b:  ldloc.0
      IL_004c:  ldnull
      IL_004d:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_0052:  ldarg.0
      IL_0053:  call       instance void Beetle.TcpChannel::b()
      IL_0058:  br.s       IL_0065

      IL_005a:  call       class Beetle.ChannelDisposedQueue Beetle.ChannelDisposedQueue::get_DisposedQueue()
      IL_005f:  ldloc.0
      IL_0060:  callvirt   instance void Beetle.ChannelDisposedQueue::Add(class Beetle.ChannelDisposedEventArgs)
      IL_0065:  leave.s    IL_006e

    }  // end .try
    finally
    {
      IL_0067:  ldloc.1
      IL_0068:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_006d:  endfinally
    }  // end handler
    IL_006e:  ret
  } // end of method TcpChannel::Dispose

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stsfld     int64 Beetle.TcpChannel::b
    IL_0007:  ldstr      "V/r2I2CpRaI="
    IL_000c:  stsfld     string Beetle.TcpChannel::k
    IL_0011:  ret
  } // end of method TcpChannel::.cctor

  .event Beetle.EventChannelError ChannelError
  {
    .addon instance void Beetle.TcpChannel::add_ChannelError(class Beetle.EventChannelError)
    .removeon instance void Beetle.TcpChannel::remove_ChannelError(class Beetle.EventChannelError)
  } // end of event TcpChannel::ChannelError
  .event Beetle.EventChannelDisposed ChannelDisposed
  {
    .removeon instance void Beetle.TcpChannel::remove_ChannelDisposed(class Beetle.EventChannelDisposed)
    .addon instance void Beetle.TcpChannel::add_ChannelDisposed(class Beetle.EventChannelDisposed)
  } // end of event TcpChannel::ChannelDisposed
  .property instance class [Smark.Core]Smark.Core.KeyValueCollection`1<object>
          Properties()
  {
    .get instance class [Smark.Core]Smark.Core.KeyValueCollection`1<object> Beetle.TcpChannel::get_Properties()
  } // end of property TcpChannel::Properties
  .property instance object Item(string)
  {
    .get instance object Beetle.TcpChannel::get_Item(string)
    .set instance void Beetle.TcpChannel::set_Item(string,
                                                   object)
  } // end of property TcpChannel::Item
  .property instance valuetype Beetle.ChannelStatus
          Status()
  {
    .get instance valuetype Beetle.ChannelStatus Beetle.TcpChannel::get_Status()
    .set instance void Beetle.TcpChannel::set_Status(valuetype Beetle.ChannelStatus)
  } // end of property TcpChannel::Status
  .property instance valuetype [mscorlib]System.DateTime
          CreateTime()
  {
    .get instance valuetype [mscorlib]System.DateTime Beetle.TcpChannel::get_CreateTime()
    .set instance void Beetle.TcpChannel::set_CreateTime(valuetype [mscorlib]System.DateTime)
  } // end of property TcpChannel::CreateTime
  .property instance object Tag()
  {
    .get instance object Beetle.TcpChannel::get_Tag()
    .set instance void Beetle.TcpChannel::set_Tag(object)
  } // end of property TcpChannel::Tag
  .property instance class Beetle.Package
          Package()
  {
    .get instance class Beetle.Package Beetle.TcpChannel::get_Package()
  } // end of property TcpChannel::Package
  .property instance class [System]System.Net.Sockets.Socket
          Socket()
  {
    .get instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
  } // end of property TcpChannel::Socket
  .property instance class [mscorlib]System.Text.Encoding
          Coding()
  {
    .get instance class [mscorlib]System.Text.Encoding Beetle.TcpChannel::get_Coding()
    .set instance void Beetle.TcpChannel::set_Coding(class [mscorlib]System.Text.Encoding)
  } // end of property TcpChannel::Coding
  .property instance int64 ClientID()
  {
    .get instance int64 Beetle.TcpChannel::get_ClientID()
    .set instance void Beetle.TcpChannel::a(int64)
  } // end of property TcpChannel::ClientID
  .property instance class Beetle.TcpServer
          Server()
  {
    .get instance class Beetle.TcpServer Beetle.TcpChannel::get_Server()
    .set instance void Beetle.TcpChannel::a(class Beetle.TcpServer)
  } // end of property TcpChannel::Server
  .property instance class [System]System.Net.IPEndPoint
          EndPoint()
  {
    .get instance class [System]System.Net.IPEndPoint Beetle.TcpChannel::get_EndPoint()
    .set instance void Beetle.TcpChannel::a(class [System]System.Net.IPEndPoint)
  } // end of property TcpChannel::EndPoint
  .property instance string Name()
  {
    .get instance string Beetle.TcpChannel::get_Name()
    .set instance void Beetle.TcpChannel::set_Name(string)
  } // end of property TcpChannel::Name
  .property instance bool IsDisposed()
  {
    .get instance bool Beetle.TcpChannel::get_IsDisposed()
  } // end of property TcpChannel::IsDisposed
} // end of class Beetle.TcpChannel

.class public auto ansi beforefieldinit Beetle.ObjectPool`1<.ctor T>
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field family class [System]System.Collections.Generic.Stack`1<!T> mObjects
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 counts) cil managed
  {
    // 代码大小       73 (0x49)
    .maxstack  3
    .locals init (int32 V_0,
             !T V_1,
             !T V_2)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [System]System.Collections.Generic.Stack`1<!T>::.ctor()
    IL_0006:  stfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ldc.i4.0
    IL_0012:  stloc.0
    IL_0013:  br.s       IL_0044

    IL_0015:  ldarg.0
    IL_0016:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
    IL_001b:  ldloca.s   V_1
    IL_001d:  initobj    !T
    IL_0023:  ldloc.1
    IL_0024:  box        !T
    IL_0029:  brfalse.s  IL_0036

    IL_002b:  ldloca.s   V_2
    IL_002d:  initobj    !T
    IL_0033:  ldloc.2
    IL_0034:  br.s       IL_003b

    IL_0036:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
    IL_003b:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<!T>::Push(!0)
    IL_0040:  ldloc.0
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.0
    IL_0044:  ldloc.0
    IL_0045:  ldarg.1
    IL_0046:  blt.s      IL_0015

    IL_0048:  ret
  } // end of method ObjectPool`1::.ctor

  .method public hidebysig newslot virtual 
          instance !T  Pop() cil managed
  {
    // 代码大小       85 (0x55)
    .maxstack  2
    .locals init (!T V_0,
             class [System]System.Collections.Generic.Stack`1<!T> V_1,
             !T V_2,
             !T V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Stack`1<!T>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0029

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
      IL_0021:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<!T>::Pop()
      IL_0026:  stloc.0
      IL_0027:  leave.s    IL_0053

      IL_0029:  ldloca.s   V_2
      IL_002b:  initobj    !T
      IL_0031:  ldloc.2
      IL_0032:  box        !T
      IL_0037:  brfalse.s  IL_0044

      IL_0039:  ldloca.s   V_3
      IL_003b:  initobj    !T
      IL_0041:  ldloc.3
      IL_0042:  br.s       IL_0049

      IL_0044:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
      IL_0049:  stloc.0
      IL_004a:  leave.s    IL_0053

    }  // end .try
    finally
    {
      IL_004c:  ldloc.1
      IL_004d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0052:  endfinally
    }  // end handler
    IL_0053:  ldloc.0
    IL_0054:  ret
  } // end of method ObjectPool`1::Pop

  .method public hidebysig newslot virtual 
          instance void  Push(!T obj) cil managed
  {
    // 代码大小       45 (0x2d)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Stack`1<!T> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.1
      IL_000e:  box        !T
      IL_0013:  brtrue.s   IL_0017

      IL_0015:  leave.s    IL_002c

      IL_0017:  ldarg.0
      IL_0018:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
      IL_001d:  ldarg.1
      IL_001e:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<!T>::Push(!0)
      IL_0023:  leave.s    IL_002c

    }  // end .try
    finally
    {
      IL_0025:  ldloc.0
      IL_0026:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_002b:  endfinally
    }  // end handler
    IL_002c:  ret
  } // end of method ObjectPool`1::Push

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       76 (0x4c)
    .maxstack  2
    IL_0000:  ldtoken    !T
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ldtoken    [mscorlib]System.IDisposable
    IL_000f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  bne.un.s   IL_0040

    IL_0016:  br.s       IL_0032

    IL_0018:  ldarg.0
    IL_0019:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
    IL_001e:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<!T>::Pop()
    IL_0023:  box        !T
    IL_0028:  castclass  [mscorlib]System.IDisposable
    IL_002d:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
    IL_0032:  ldarg.0
    IL_0033:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
    IL_0038:  callvirt   instance int32 class [System]System.Collections.Generic.Stack`1<!T>::get_Count()
    IL_003d:  ldc.i4.0
    IL_003e:  bgt.s      IL_0018

    IL_0040:  ldarg.0
    IL_0041:  ldfld      class [System]System.Collections.Generic.Stack`1<!0> class Beetle.ObjectPool`1<!T>::mObjects
    IL_0046:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<!T>::Clear()
    IL_004b:  ret
  } // end of method ObjectPool`1::Dispose

} // end of class Beetle.ObjectPool`1

.class interface public abstract auto ansi Beetle.IByteConverter
{
  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(char 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(int16 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(int32 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(int64 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(float32 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(float64 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(uint8 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(bool 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(uint16 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(uint32 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance uint8[]  GetBytes(uint64 'value') cil managed
  {
  } // end of method IByteConverter::GetBytes

  .method public hidebysig newslot abstract virtual 
          instance char  ReadChar(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadChar

  .method public hidebysig newslot abstract virtual 
          instance int16  ReadInt16(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadInt16

  .method public hidebysig newslot abstract virtual 
          instance int32  ReadInt32(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadInt32

  .method public hidebysig newslot abstract virtual 
          instance int64  ReadInt64(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadInt64

  .method public hidebysig newslot abstract virtual 
          instance float32  ReadFloat(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadFloat

  .method public hidebysig newslot abstract virtual 
          instance float64  ReadDouble(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadDouble

  .method public hidebysig newslot abstract virtual 
          instance uint8  ReadByte(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadByte

  .method public hidebysig newslot abstract virtual 
          instance bool  ReadBool(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadBool

  .method public hidebysig newslot abstract virtual 
          instance uint16  ReadUInt16(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadUInt16

  .method public hidebysig newslot abstract virtual 
          instance uint32  ReadUInt32(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadUInt32

  .method public hidebysig newslot abstract virtual 
          instance uint64  ReadUInt64(class Beetle.BufferReader reader) cil managed
  {
  } // end of method IByteConverter::ReadUInt64

} // end of class Beetle.IByteConverter

.class public auto ansi sealed Beetle.EventMessageWriter
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method EventMessageWriter::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke(class Beetle.IMessage msg,
                                class Beetle.BufferWriter writer) runtime managed
  {
  } // end of method EventMessageWriter::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(class Beetle.IMessage msg,
                      class Beetle.BufferWriter writer,
                      class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method EventMessageWriter::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method EventMessageWriter::EndInvoke

} // end of class Beetle.EventMessageWriter

.class public auto ansi beforefieldinit Beetle.BytesMessage
       extends [mscorlib]System.Object
       implements Beetle.IMessage
{
  .field public uint8[] Value
  .method public hidebysig newslot virtual final 
          instance void  Save(class Beetle.BufferWriter writer) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] Beetle.BytesMessage::Value
    IL_0007:  ldc.i4.0
    IL_0008:  ldarg.0
    IL_0009:  ldfld      uint8[] Beetle.BytesMessage::Value
    IL_000e:  ldlen
    IL_000f:  conv.i4
    IL_0010:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0015:  ret
  } // end of method BytesMessage::Save

  .method public hidebysig newslot virtual final 
          instance void  Load(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance uint8[] Beetle.BufferReader::ToBytes()
    IL_0007:  stfld      uint8[] Beetle.BytesMessage::Value
    IL_000c:  ret
  } // end of method BytesMessage::Load

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method BytesMessage::.ctor

} // end of class Beetle.BytesMessage

.class public auto ansi beforefieldinit Beetle.BufferReader
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field private static literal int32 a = int32(0x80000000)
  .field private static literal int64 b = int64(0x8000000000000000)
  .field assembly class [mscorlib]System.Collections.Generic.IList`1<class b> c
  .field private int32 d
  .field private class b e
  .field private int32 f
  .field private class ai g
  .field private class Beetle.IByteConverter h
  .field private class b i
  .field public class [mscorlib]System.Text.Encoding Coding
  .field private int32 j
  .field private class Beetle.ByteArraySegment k
  .field private class Beetle.ByteArraySegment l
  .field private bool m
  .field public bool EnabledVariant
  .method public hidebysig specialname instance int32 
          get_Count() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Beetle.BufferReader::f
    IL_0006:  ret
  } // end of method BufferReader::get_Count

  .method public hidebysig instance uint8[] 
          ToBytes() cil managed
  {
    // 代码大小       80 (0x50)
    .maxstack  5
    .locals init (uint8[] V_0,
             class b V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Beetle.BufferReader::get_Count()
    IL_0006:  newarr     [mscorlib]System.Byte
    IL_000b:  stloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.2
    IL_000e:  ldc.i4.0
    IL_000f:  stloc.3
    IL_0010:  br.s       IL_0040

    IL_0012:  ldarg.0
    IL_0013:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_0018:  ldloc.3
    IL_0019:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class b>::get_Item(int32)
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldfld      uint8[] b::e
    IL_0025:  ldc.i4.0
    IL_0026:  ldloc.0
    IL_0027:  ldloc.2
    IL_0028:  ldloc.1
    IL_0029:  ldfld      int32 b::h
    IL_002e:  call       void [mscorlib]System.Array::Copy(class [mscorlib]System.Array,
                                                           int32,
                                                           class [mscorlib]System.Array,
                                                           int32,
                                                           int32)
    IL_0033:  ldloc.2
    IL_0034:  ldloc.1
    IL_0035:  ldfld      int32 b::h
    IL_003a:  add
    IL_003b:  stloc.2
    IL_003c:  ldloc.3
    IL_003d:  ldc.i4.1
    IL_003e:  add
    IL_003f:  stloc.3
    IL_0040:  ldloc.3
    IL_0041:  ldarg.0
    IL_0042:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_0047:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class b>::get_Count()
    IL_004c:  blt.s      IL_0012

    IL_004e:  ldloc.0
    IL_004f:  ret
  } // end of method BufferReader::ToBytes

  .method assembly hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Text.Encoding A_0,
                               class [mscorlib]System.Collections.Generic.IList`1<class b> A_1) cil managed
  {
    // 代码大小       136 (0x88)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.8
    IL_0002:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class b>::.ctor(int32)
    IL_0007:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_000c:  ldarg.0
    IL_000d:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_0012:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_0017:  stfld      class ai Beetle.BufferReader::g
    IL_001c:  ldarg.0
    IL_001d:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_0022:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_0027:  stfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_002c:  ldarg.0
    IL_002d:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0032:  stfld      class Beetle.ByteArraySegment Beetle.BufferReader::k
    IL_0037:  ldarg.0
    IL_0038:  call       instance void [mscorlib]System.Object::.ctor()
    IL_003d:  ldarg.0
    IL_003e:  ldarg.2
    IL_003f:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_0044:  ldarg.0
    IL_0045:  ldarg.1
    IL_0046:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    IL_004b:  ldc.i4.0
    IL_004c:  stloc.0
    IL_004d:  br.s       IL_0078

    IL_004f:  ldarg.0
    IL_0050:  dup
    IL_0051:  ldfld      int32 Beetle.BufferReader::f
    IL_0056:  ldarg.2
    IL_0057:  ldloc.0
    IL_0058:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class b>::get_Item(int32)
    IL_005d:  ldfld      int32 b::h
    IL_0062:  add
    IL_0063:  stfld      int32 Beetle.BufferReader::f
    IL_0068:  ldarg.2
    IL_0069:  ldloc.0
    IL_006a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class b>::get_Item(int32)
    IL_006f:  callvirt   instance void b::c()
    IL_0074:  ldloc.0
    IL_0075:  ldc.i4.1
    IL_0076:  add
    IL_0077:  stloc.0
    IL_0078:  ldloc.0
    IL_0079:  ldarg.2
    IL_007a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class b>::get_Count()
    IL_007f:  blt.s      IL_004f

    IL_0081:  ldarg.0
    IL_0082:  call       instance void Beetle.BufferReader::c()
    IL_0087:  ret
  } // end of method BufferReader::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       73 (0x49)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.8
    IL_0002:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class b>::.ctor(int32)
    IL_0007:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_000c:  ldarg.0
    IL_000d:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_0012:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_0017:  stfld      class ai Beetle.BufferReader::g
    IL_001c:  ldarg.0
    IL_001d:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_0022:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_0027:  stfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_002c:  ldarg.0
    IL_002d:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0032:  stfld      class Beetle.ByteArraySegment Beetle.BufferReader::k
    IL_0037:  ldarg.0
    IL_0038:  call       instance void [mscorlib]System.Object::.ctor()
    IL_003d:  ldarg.0
    IL_003e:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0043:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    IL_0048:  ret
  } // end of method BufferReader::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Text.Encoding coding) cil managed
  {
    // 代码大小       69 (0x45)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.8
    IL_0002:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class b>::.ctor(int32)
    IL_0007:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_000c:  ldarg.0
    IL_000d:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_0012:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_0017:  stfld      class ai Beetle.BufferReader::g
    IL_001c:  ldarg.0
    IL_001d:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_0022:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_0027:  stfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_002c:  ldarg.0
    IL_002d:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0032:  stfld      class Beetle.ByteArraySegment Beetle.BufferReader::k
    IL_0037:  ldarg.0
    IL_0038:  call       instance void [mscorlib]System.Object::.ctor()
    IL_003d:  ldarg.0
    IL_003e:  ldarg.1
    IL_003f:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    IL_0044:  ret
  } // end of method BufferReader::.ctor

  .method public hidebysig instance void 
          Import(class Beetle.BufferWriter write) cil managed
  {
    // 代码大小       106 (0x6a)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  stfld      int32 Beetle.BufferReader::d
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.0
    IL_0009:  stfld      int32 Beetle.BufferReader::f
    IL_000e:  br.s       IL_0055

    IL_0010:  ldarg.0
    IL_0011:  ldarg.1
    IL_0012:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0017:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
    IL_001c:  stfld      class b Beetle.BufferReader::i
    IL_0021:  ldarg.0
    IL_0022:  ldfld      class b Beetle.BufferReader::i
    IL_0027:  callvirt   instance void b::c()
    IL_002c:  ldarg.0
    IL_002d:  dup
    IL_002e:  ldfld      int32 Beetle.BufferReader::f
    IL_0033:  ldarg.0
    IL_0034:  ldfld      class b Beetle.BufferReader::i
    IL_0039:  ldfld      int32 b::h
    IL_003e:  add
    IL_003f:  stfld      int32 Beetle.BufferReader::f
    IL_0044:  ldarg.0
    IL_0045:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_004a:  ldarg.0
    IL_004b:  ldfld      class b Beetle.BufferReader::i
    IL_0050:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class b>::Add(!0)
    IL_0055:  ldarg.1
    IL_0056:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_005b:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_0060:  ldc.i4.0
    IL_0061:  bgt.s      IL_0010

    IL_0063:  ldarg.0
    IL_0064:  call       instance void Beetle.BufferReader::c()
    IL_0069:  ret
  } // end of method BufferReader::Import

  .method public hidebysig instance void 
          Import(class Beetle.ByteArraySegment segment) cil managed
  {
    // 代码大小       46 (0x2e)
    .maxstack  2
    .locals init (class b V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  stfld      int32 Beetle.BufferReader::d
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.0
    IL_0009:  stfld      int32 Beetle.BufferReader::f
    IL_000e:  newobj     instance void b::.ctor()
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldarg.1
    IL_0016:  callvirt   instance void b::a(class Beetle.ByteArraySegment)
    IL_001b:  ldarg.0
    IL_001c:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_0021:  ldloc.0
    IL_0022:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class b>::Add(!0)
    IL_0027:  ldarg.0
    IL_0028:  call       instance void Beetle.BufferReader::c()
    IL_002d:  ret
  } // end of method BufferReader::Import

  .method public hidebysig instance void 
          CleanBuffer() cil managed
  {
    // 代码大小       51 (0x33)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0019

    IL_0004:  ldarg.0
    IL_0005:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_000a:  ldloc.0
    IL_000b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class b>::get_Item(int32)
    IL_0010:  callvirt   instance void b::Dispose()
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.1
    IL_0017:  add
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldarg.0
    IL_001b:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_0020:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class b>::get_Count()
    IL_0025:  blt.s      IL_0004

    IL_0027:  ldarg.0
    IL_0028:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_002d:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class b>::Clear()
    IL_0032:  ret
  } // end of method BufferReader::CleanBuffer

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Text.Encoding coding,
                               class Beetle.BufferWriter write) cil managed
  {
    // 代码大小       160 (0xa0)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.8
    IL_0002:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class b>::.ctor(int32)
    IL_0007:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_000c:  ldarg.0
    IL_000d:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_0012:  callvirt   instance !0 class Beetle.ObjectPool`1<class ai>::Pop()
    IL_0017:  stfld      class ai Beetle.BufferReader::g
    IL_001c:  ldarg.0
    IL_001d:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_0022:  callvirt   instance class Beetle.IByteConverter Beetle.IByteConverterFactory::Pop()
    IL_0027:  stfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_002c:  ldarg.0
    IL_002d:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0032:  stfld      class Beetle.ByteArraySegment Beetle.BufferReader::k
    IL_0037:  ldarg.0
    IL_0038:  call       instance void [mscorlib]System.Object::.ctor()
    IL_003d:  ldarg.0
    IL_003e:  ldarg.1
    IL_003f:  stfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    IL_0044:  br.s       IL_008b

    IL_0046:  ldarg.0
    IL_0047:  ldarg.2
    IL_0048:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_004d:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
    IL_0052:  stfld      class b Beetle.BufferReader::i
    IL_0057:  ldarg.0
    IL_0058:  ldfld      class b Beetle.BufferReader::i
    IL_005d:  callvirt   instance void b::c()
    IL_0062:  ldarg.0
    IL_0063:  dup
    IL_0064:  ldfld      int32 Beetle.BufferReader::f
    IL_0069:  ldarg.0
    IL_006a:  ldfld      class b Beetle.BufferReader::i
    IL_006f:  ldfld      int32 b::h
    IL_0074:  add
    IL_0075:  stfld      int32 Beetle.BufferReader::f
    IL_007a:  ldarg.0
    IL_007b:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_0080:  ldarg.0
    IL_0081:  ldfld      class b Beetle.BufferReader::i
    IL_0086:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class b>::Add(!0)
    IL_008b:  ldarg.2
    IL_008c:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
    IL_0091:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_0096:  ldc.i4.0
    IL_0097:  bgt.s      IL_0046

    IL_0099:  ldarg.0
    IL_009a:  call       instance void Beetle.BufferReader::c()
    IL_009f:  ret
  } // end of method BufferReader::.ctor

  .method private hidebysig instance void 
          c() cil managed
  {
    // 代码大小       38 (0x26)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 Beetle.BufferReader::d
    IL_000d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class b>::get_Item(int32)
    IL_0012:  stfld      class b Beetle.BufferReader::e
    IL_0017:  ldarg.0
    IL_0018:  dup
    IL_0019:  ldfld      int32 Beetle.BufferReader::d
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stfld      int32 Beetle.BufferReader::d
    IL_0025:  ret
  } // end of method BufferReader::c

  .method public hidebysig instance uint8[] 
          ReadBytes(int32 count) cil managed
  {
    // 代码大小       73 (0x49)
    .maxstack  5
    .locals init (class Beetle.ByteArraySegment V_0,
             uint8[] V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_000e:  brtrue.s   IL_0027

    IL_0010:  ldloc.0
    IL_0011:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0016:  ldloc.0
    IL_0017:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_001c:  ldlen
    IL_001d:  conv.i4
    IL_001e:  bne.un.s   IL_0027

    IL_0020:  ldloc.0
    IL_0021:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0026:  ret

    IL_0027:  ldarg.1
    IL_0028:  newarr     [mscorlib]System.Byte
    IL_002d:  stloc.1
    IL_002e:  ldloc.0
    IL_002f:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0034:  ldloc.0
    IL_0035:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_003a:  ldloc.1
    IL_003b:  ldc.i4.0
    IL_003c:  ldloc.0
    IL_003d:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0042:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_0047:  ldloc.1
    IL_0048:  ret
  } // end of method BufferReader::ReadBytes

  .method public hidebysig instance void 
          ReadByteArray(class Beetle.ByteArraySegment bas) cil managed
  {
    // 代码大小       16 (0x10)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.1
    IL_000a:  call       instance void Beetle.BufferReader::Read(int32,
                                                                 class Beetle.ByteArraySegment)
    IL_000f:  ret
  } // end of method BufferReader::ReadByteArray

  .method public hidebysig instance uint8[] 
          ReadByteArray() cil managed
  {
    // 代码大小       26 (0x1a)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ble.s      IL_0013

    IL_000b:  ldarg.0
    IL_000c:  ldloc.0
    IL_000d:  call       instance uint8[] Beetle.BufferReader::ReadBytes(int32)
    IL_0012:  ret

    IL_0013:  ldc.i4.0
    IL_0014:  newarr     [mscorlib]System.Byte
    IL_0019:  ret
  } // end of method BufferReader::ReadByteArray

  .method public hidebysig instance void 
          Read(int32 count,
               class Beetle.ByteArraySegment bas) cil managed
  {
    // 代码大小       241 (0xf1)
    .maxstack  5
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  callvirt   instance int32 Beetle.ByteArraySegment::get_BufferLength()
    IL_0007:  bgt.s      IL_0011

    IL_0009:  ldarg.2
    IL_000a:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_000f:  brtrue.s   IL_0017

    IL_0011:  call       class Beetle.NetTcpException Beetle.NetTcpException::ReadToByteArraySegment()
    IL_0016:  throw

    IL_0017:  ldarg.0
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class b Beetle.BufferReader::e
    IL_001e:  ldarg.1
    IL_001f:  callvirt   instance class Beetle.ByteArraySegment b::d(int32)
    IL_0024:  stfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_0029:  ldarg.0
    IL_002a:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_002f:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0034:  ldarg.1
    IL_0035:  bne.un.s   IL_006d

    IL_0037:  ldarg.0
    IL_0038:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_003d:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0042:  ldarg.0
    IL_0043:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_0048:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_004d:  ldarg.2
    IL_004e:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0053:  ldc.i4.0
    IL_0054:  ldarg.0
    IL_0055:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_005a:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_005f:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_0064:  ldarg.2
    IL_0065:  ldc.i4.0
    IL_0066:  ldarg.1
    IL_0067:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(int32,
                                                                        int32)
    IL_006c:  ret

    IL_006d:  ldarg.0
    IL_006e:  ldc.i4.0
    IL_006f:  stfld      int32 Beetle.BufferReader::j
    IL_0074:  ldarg.0
    IL_0075:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_007a:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_007f:  ldarg.0
    IL_0080:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_0085:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_008a:  ldarg.2
    IL_008b:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0090:  ldarg.0
    IL_0091:  ldfld      int32 Beetle.BufferReader::j
    IL_0096:  ldarg.0
    IL_0097:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_009c:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_00a1:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_00a6:  ldarg.0
    IL_00a7:  dup
    IL_00a8:  ldfld      int32 Beetle.BufferReader::j
    IL_00ad:  ldarg.0
    IL_00ae:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_00b3:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_00b8:  add
    IL_00b9:  stfld      int32 Beetle.BufferReader::j
    IL_00be:  ldarg.0
    IL_00bf:  ldfld      int32 Beetle.BufferReader::j
    IL_00c4:  ldarg.1
    IL_00c5:  bge.s      IL_00e8

    IL_00c7:  ldarg.0
    IL_00c8:  call       instance void Beetle.BufferReader::c()
    IL_00cd:  ldarg.0
    IL_00ce:  ldarg.0
    IL_00cf:  ldfld      class b Beetle.BufferReader::e
    IL_00d4:  ldarg.1
    IL_00d5:  ldarg.0
    IL_00d6:  ldfld      int32 Beetle.BufferReader::j
    IL_00db:  sub
    IL_00dc:  callvirt   instance class Beetle.ByteArraySegment b::d(int32)
    IL_00e1:  stfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_00e6:  br.s       IL_0074

    IL_00e8:  ldarg.2
    IL_00e9:  ldc.i4.0
    IL_00ea:  ldarg.1
    IL_00eb:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(int32,
                                                                        int32)
    IL_00f0:  ret
  } // end of method BufferReader::Read

  .method public hidebysig instance class Beetle.ByteArraySegment 
          Read(int32 count) cil managed
  {
    // 代码大小       185 (0xb9)
    .maxstack  5
    .locals init (uint8[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      class b Beetle.BufferReader::e
    IL_0007:  ldarg.1
    IL_0008:  callvirt   instance class Beetle.ByteArraySegment b::d(int32)
    IL_000d:  stfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_0012:  ldarg.0
    IL_0013:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_0018:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_001d:  ldarg.1
    IL_001e:  bne.un.s   IL_0027

    IL_0020:  ldarg.0
    IL_0021:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_0026:  ret

    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.0
    IL_0029:  stfld      int32 Beetle.BufferReader::j
    IL_002e:  ldarg.1
    IL_002f:  newarr     [mscorlib]System.Byte
    IL_0034:  stloc.0
    IL_0035:  ldarg.0
    IL_0036:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_003b:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0040:  ldarg.0
    IL_0041:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_0046:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_004b:  ldloc.0
    IL_004c:  ldarg.0
    IL_004d:  ldfld      int32 Beetle.BufferReader::j
    IL_0052:  ldarg.0
    IL_0053:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_0058:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_005d:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_0062:  ldarg.0
    IL_0063:  dup
    IL_0064:  ldfld      int32 Beetle.BufferReader::j
    IL_0069:  ldarg.0
    IL_006a:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_006f:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0074:  add
    IL_0075:  stfld      int32 Beetle.BufferReader::j
    IL_007a:  ldarg.0
    IL_007b:  ldfld      int32 Beetle.BufferReader::j
    IL_0080:  ldarg.1
    IL_0081:  bge.s      IL_00a4

    IL_0083:  ldarg.0
    IL_0084:  call       instance void Beetle.BufferReader::c()
    IL_0089:  ldarg.0
    IL_008a:  ldarg.0
    IL_008b:  ldfld      class b Beetle.BufferReader::e
    IL_0090:  ldarg.1
    IL_0091:  ldarg.0
    IL_0092:  ldfld      int32 Beetle.BufferReader::j
    IL_0097:  sub
    IL_0098:  callvirt   instance class Beetle.ByteArraySegment b::d(int32)
    IL_009d:  stfld      class Beetle.ByteArraySegment Beetle.BufferReader::l
    IL_00a2:  br.s       IL_0035

    IL_00a4:  ldarg.0
    IL_00a5:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::k
    IL_00aa:  ldloc.0
    IL_00ab:  ldc.i4.0
    IL_00ac:  ldarg.1
    IL_00ad:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                        int32,
                                                                        int32)
    IL_00b2:  ldarg.0
    IL_00b3:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::k
    IL_00b8:  ret
  } // end of method BufferReader::Read

  .method private hidebysig instance !!T[] 
          createArray<T>() cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  1
    .locals init (!!T[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0006:  newarr     !!T
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method BufferReader::createArray

  .method private hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<!!T> 
          createList<T>([out] int32& count) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<!!T> V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0007:  stind.i4
    IL_0008:  ldarg.1
    IL_0009:  ldind.i4
    IL_000a:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<!!T>::.ctor(int32)
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method BufferReader::createList

  .method public hidebysig instance uint8 
          ReadByte() cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_0006:  ldarg.0
    IL_0007:  callvirt   instance uint8 Beetle.IByteConverter::ReadByte(class Beetle.BufferReader)
    IL_000c:  ret
  } // end of method BufferReader::ReadByte

  .method public hidebysig instance bool 
          ReadBool() cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_0006:  ldarg.0
    IL_0007:  callvirt   instance bool Beetle.IByteConverter::ReadBool(class Beetle.BufferReader)
    IL_000c:  ret
  } // end of method BufferReader::ReadBool

  .method public hidebysig instance bool[] 
          ReadBools() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  3
    .locals init (bool[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<bool>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0018

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldarg.0
    IL_000e:  call       instance bool Beetle.BufferReader::ReadBool()
    IL_0013:  stelem.i1
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_000b

    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method BufferReader::ReadBools

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<bool> 
          ReadBoolList() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<bool> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<bool>(int32&)
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_001d

    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  call       instance bool Beetle.BufferReader::ReadBool()
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<bool>::Add(!0)
    IL_0019:  ldloc.2
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldloc.0
    IL_001f:  blt.s      IL_000d

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method BufferReader::ReadBoolList

  .method public hidebysig instance char 
          ReadChar() cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance int16 Beetle.BufferReader::ReadInt16()
    IL_0006:  conv.u2
    IL_0007:  ret
  } // end of method BufferReader::ReadChar

  .method public hidebysig instance char[] 
          ReadChars() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  3
    .locals init (char[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<char>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0018

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldarg.0
    IL_000e:  call       instance char Beetle.BufferReader::ReadChar()
    IL_0013:  stelem.i2
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_000b

    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method BufferReader::ReadChars

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<char> 
          ReadCharList() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<char> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<char>(int32&)
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_001d

    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  call       instance char Beetle.BufferReader::ReadChar()
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<char>::Add(!0)
    IL_0019:  ldloc.2
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldloc.0
    IL_001f:  blt.s      IL_000d

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method BufferReader::ReadCharList

  .method public hidebysig instance float64 
          ReadDouble() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  1
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferReader::EnabledVariant
    IL_0006:  brfalse.s  IL_0014

    IL_0008:  ldarg.0
    IL_0009:  call       instance int64 Beetle.BufferReader::Read64Variant()
    IL_000e:  stloc.0
    IL_000f:  ldloca.s   V_0
    IL_0011:  conv.u
    IL_0012:  ldind.r8
    IL_0013:  ret

    IL_0014:  ldarg.0
    IL_0015:  call       instance int64 Beetle.BufferReader::ReadInt64()
    IL_001a:  stloc.0
    IL_001b:  ldloca.s   V_0
    IL_001d:  conv.u
    IL_001e:  ldind.r8
    IL_001f:  ret
  } // end of method BufferReader::ReadDouble

  .method public hidebysig instance float64[] 
          ReadDoubles() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  3
    .locals init (float64[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<float64>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0018

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldarg.0
    IL_000e:  call       instance float64 Beetle.BufferReader::ReadDouble()
    IL_0013:  stelem.r8
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_000b

    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method BufferReader::ReadDoubles

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<float64> 
          ReadDoubleList() cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<float64> V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<float64>(int32&)
    IL_000a:  stloc.1
    IL_000b:  ldc.i4.0
    IL_000c:  stloc.2
    IL_000d:  br.s       IL_001f

    IL_000f:  ldloc.1
    IL_0010:  ldarg.0
    IL_0011:  call       instance float64 Beetle.BufferReader::ReadDouble()
    IL_0016:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<float64>::Add(!0)
    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.1
    IL_001d:  add
    IL_001e:  stloc.2
    IL_001f:  ldloc.2
    IL_0020:  ldloc.0
    IL_0021:  blt.s      IL_000f

    IL_0023:  ldloc.1
    IL_0024:  ret
  } // end of method BufferReader::ReadDoubleList

  .method public hidebysig instance int16 
          ReadInt16() cil managed
  {
    // 代码大小       83 (0x53)
    .maxstack  3
    .locals init (int16 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferReader::e
    IL_0006:  ldc.i4.2
    IL_0007:  callvirt   instance bool b::a(int32)
    IL_000c:  brfalse.s  IL_0046

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class b Beetle.BufferReader::e
    IL_0014:  ldfld      uint8[] b::e
    IL_0019:  ldarg.0
    IL_001a:  ldfld      class b Beetle.BufferReader::e
    IL_001f:  ldfld      int32 b::g
    IL_0024:  ldelema    [mscorlib]System.Byte
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  conv.i
    IL_002c:  ldind.i2
    IL_002d:  stloc.0
    IL_002e:  ldc.i4.0
    IL_002f:  conv.u
    IL_0030:  stloc.1
    IL_0031:  ldarg.0
    IL_0032:  ldfld      class b Beetle.BufferReader::e
    IL_0037:  dup
    IL_0038:  ldfld      int32 b::g
    IL_003d:  ldc.i4.2
    IL_003e:  add
    IL_003f:  stfld      int32 b::g
    IL_0044:  ldloc.0
    IL_0045:  ret

    IL_0046:  ldarg.0
    IL_0047:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_004c:  ldarg.0
    IL_004d:  callvirt   instance int16 Beetle.IByteConverter::ReadInt16(class Beetle.BufferReader)
    IL_0052:  ret
  } // end of method BufferReader::ReadInt16

  .method public hidebysig instance uint16 
          ReadUInt16() cil managed
  {
    // 代码大小       83 (0x53)
    .maxstack  3
    .locals init (uint16 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferReader::e
    IL_0006:  ldc.i4.2
    IL_0007:  callvirt   instance bool b::a(int32)
    IL_000c:  brfalse.s  IL_0046

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class b Beetle.BufferReader::e
    IL_0014:  ldfld      uint8[] b::e
    IL_0019:  ldarg.0
    IL_001a:  ldfld      class b Beetle.BufferReader::e
    IL_001f:  ldfld      int32 b::g
    IL_0024:  ldelema    [mscorlib]System.Byte
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  conv.i
    IL_002c:  ldind.u2
    IL_002d:  stloc.0
    IL_002e:  ldc.i4.0
    IL_002f:  conv.u
    IL_0030:  stloc.1
    IL_0031:  ldarg.0
    IL_0032:  ldfld      class b Beetle.BufferReader::e
    IL_0037:  dup
    IL_0038:  ldfld      int32 b::g
    IL_003d:  ldc.i4.2
    IL_003e:  add
    IL_003f:  stfld      int32 b::g
    IL_0044:  ldloc.0
    IL_0045:  ret

    IL_0046:  ldarg.0
    IL_0047:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_004c:  ldarg.0
    IL_004d:  callvirt   instance uint16 Beetle.IByteConverter::ReadUInt16(class Beetle.BufferReader)
    IL_0052:  ret
  } // end of method BufferReader::ReadUInt16

  .method public hidebysig instance int16[] 
          ReadInt16s() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  3
    .locals init (int16[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<int16>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0018

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldarg.0
    IL_000e:  call       instance int16 Beetle.BufferReader::ReadInt16()
    IL_0013:  stelem.i2
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_000b

    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method BufferReader::ReadInt16s

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<int16> 
          ReadInt16List() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<int16> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<int16>(int32&)
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_001d

    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  call       instance int16 Beetle.BufferReader::ReadInt16()
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int16>::Add(!0)
    IL_0019:  ldloc.2
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldloc.0
    IL_001f:  blt.s      IL_000d

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method BufferReader::ReadInt16List

  .method public hidebysig instance uint32 
          ReadUInt32() cil managed
  {
    // 代码大小       98 (0x62)
    .maxstack  3
    .locals init (uint32 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferReader::EnabledVariant
    IL_0006:  brfalse.s  IL_000f

    IL_0008:  ldarg.0
    IL_0009:  call       instance uint32 Beetle.BufferReader::ReadU32Variant()
    IL_000e:  ret

    IL_000f:  ldarg.0
    IL_0010:  ldfld      class b Beetle.BufferReader::e
    IL_0015:  ldc.i4.4
    IL_0016:  callvirt   instance bool b::a(int32)
    IL_001b:  brfalse.s  IL_0055

    IL_001d:  ldarg.0
    IL_001e:  ldfld      class b Beetle.BufferReader::e
    IL_0023:  ldfld      uint8[] b::e
    IL_0028:  ldarg.0
    IL_0029:  ldfld      class b Beetle.BufferReader::e
    IL_002e:  ldfld      int32 b::g
    IL_0033:  ldelema    [mscorlib]System.Byte
    IL_0038:  stloc.1
    IL_0039:  ldloc.1
    IL_003a:  conv.i
    IL_003b:  ldind.u4
    IL_003c:  stloc.0
    IL_003d:  ldc.i4.0
    IL_003e:  conv.u
    IL_003f:  stloc.1
    IL_0040:  ldarg.0
    IL_0041:  ldfld      class b Beetle.BufferReader::e
    IL_0046:  dup
    IL_0047:  ldfld      int32 b::g
    IL_004c:  ldc.i4.4
    IL_004d:  add
    IL_004e:  stfld      int32 b::g
    IL_0053:  ldloc.0
    IL_0054:  ret

    IL_0055:  ldarg.0
    IL_0056:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_005b:  ldarg.0
    IL_005c:  callvirt   instance uint32 Beetle.IByteConverter::ReadUInt32(class Beetle.BufferReader)
    IL_0061:  ret
  } // end of method BufferReader::ReadUInt32

  .method public hidebysig instance int32 
          ReadInt32() cil managed
  {
    // 代码大小       98 (0x62)
    .maxstack  3
    .locals init (int32 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferReader::EnabledVariant
    IL_0006:  brfalse.s  IL_000f

    IL_0008:  ldarg.0
    IL_0009:  call       instance int32 Beetle.BufferReader::Read32Variant()
    IL_000e:  ret

    IL_000f:  ldarg.0
    IL_0010:  ldfld      class b Beetle.BufferReader::e
    IL_0015:  ldc.i4.4
    IL_0016:  callvirt   instance bool b::a(int32)
    IL_001b:  brfalse.s  IL_0055

    IL_001d:  ldarg.0
    IL_001e:  ldfld      class b Beetle.BufferReader::e
    IL_0023:  ldfld      uint8[] b::e
    IL_0028:  ldarg.0
    IL_0029:  ldfld      class b Beetle.BufferReader::e
    IL_002e:  ldfld      int32 b::g
    IL_0033:  ldelema    [mscorlib]System.Byte
    IL_0038:  stloc.1
    IL_0039:  ldloc.1
    IL_003a:  conv.i
    IL_003b:  ldind.i4
    IL_003c:  stloc.0
    IL_003d:  ldc.i4.0
    IL_003e:  conv.u
    IL_003f:  stloc.1
    IL_0040:  ldarg.0
    IL_0041:  ldfld      class b Beetle.BufferReader::e
    IL_0046:  dup
    IL_0047:  ldfld      int32 b::g
    IL_004c:  ldc.i4.4
    IL_004d:  add
    IL_004e:  stfld      int32 b::g
    IL_0053:  ldloc.0
    IL_0054:  ret

    IL_0055:  ldarg.0
    IL_0056:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_005b:  ldarg.0
    IL_005c:  callvirt   instance int32 Beetle.IByteConverter::ReadInt32(class Beetle.BufferReader)
    IL_0061:  ret
  } // end of method BufferReader::ReadInt32

  .method public hidebysig instance int32[] 
          ReadInt32s() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  3
    .locals init (int32[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<int32>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0018

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldarg.0
    IL_000e:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0013:  stelem.i4
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_000b

    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method BufferReader::ReadInt32s

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<int32> 
          ReadInt32List() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<int32> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<int32>(int32&)
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_001d

    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0019:  ldloc.2
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldloc.0
    IL_001f:  blt.s      IL_000d

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method BufferReader::ReadInt32List

  .method public hidebysig instance uint64 
          ReadUInt64() cil managed
  {
    // 代码大小       98 (0x62)
    .maxstack  3
    .locals init (uint64 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferReader::EnabledVariant
    IL_0006:  brfalse.s  IL_000f

    IL_0008:  ldarg.0
    IL_0009:  call       instance uint64 Beetle.BufferReader::ReadU64Variant()
    IL_000e:  ret

    IL_000f:  ldarg.0
    IL_0010:  ldfld      class b Beetle.BufferReader::e
    IL_0015:  ldc.i4.8
    IL_0016:  callvirt   instance bool b::a(int32)
    IL_001b:  brfalse.s  IL_0055

    IL_001d:  ldarg.0
    IL_001e:  ldfld      class b Beetle.BufferReader::e
    IL_0023:  ldfld      uint8[] b::e
    IL_0028:  ldarg.0
    IL_0029:  ldfld      class b Beetle.BufferReader::e
    IL_002e:  ldfld      int32 b::g
    IL_0033:  ldelema    [mscorlib]System.Byte
    IL_0038:  stloc.1
    IL_0039:  ldloc.1
    IL_003a:  conv.i
    IL_003b:  ldind.i8
    IL_003c:  stloc.0
    IL_003d:  ldc.i4.0
    IL_003e:  conv.u
    IL_003f:  stloc.1
    IL_0040:  ldarg.0
    IL_0041:  ldfld      class b Beetle.BufferReader::e
    IL_0046:  dup
    IL_0047:  ldfld      int32 b::g
    IL_004c:  ldc.i4.8
    IL_004d:  add
    IL_004e:  stfld      int32 b::g
    IL_0053:  ldloc.0
    IL_0054:  ret

    IL_0055:  ldarg.0
    IL_0056:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_005b:  ldarg.0
    IL_005c:  callvirt   instance uint64 Beetle.IByteConverter::ReadUInt64(class Beetle.BufferReader)
    IL_0061:  ret
  } // end of method BufferReader::ReadUInt64

  .method public hidebysig instance int64 
          ReadInt64() cil managed
  {
    // 代码大小       98 (0x62)
    .maxstack  3
    .locals init (int64 V_0,
             uint8& pinned V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferReader::EnabledVariant
    IL_0006:  brfalse.s  IL_000f

    IL_0008:  ldarg.0
    IL_0009:  call       instance int64 Beetle.BufferReader::Read64Variant()
    IL_000e:  ret

    IL_000f:  ldarg.0
    IL_0010:  ldfld      class b Beetle.BufferReader::e
    IL_0015:  ldc.i4.8
    IL_0016:  callvirt   instance bool b::a(int32)
    IL_001b:  brfalse.s  IL_0055

    IL_001d:  ldarg.0
    IL_001e:  ldfld      class b Beetle.BufferReader::e
    IL_0023:  ldfld      uint8[] b::e
    IL_0028:  ldarg.0
    IL_0029:  ldfld      class b Beetle.BufferReader::e
    IL_002e:  ldfld      int32 b::g
    IL_0033:  ldelema    [mscorlib]System.Byte
    IL_0038:  stloc.1
    IL_0039:  ldloc.1
    IL_003a:  conv.i
    IL_003b:  ldind.i8
    IL_003c:  stloc.0
    IL_003d:  ldc.i4.0
    IL_003e:  conv.u
    IL_003f:  stloc.1
    IL_0040:  ldarg.0
    IL_0041:  ldfld      class b Beetle.BufferReader::e
    IL_0046:  dup
    IL_0047:  ldfld      int32 b::g
    IL_004c:  ldc.i4.8
    IL_004d:  add
    IL_004e:  stfld      int32 b::g
    IL_0053:  ldloc.0
    IL_0054:  ret

    IL_0055:  ldarg.0
    IL_0056:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
    IL_005b:  ldarg.0
    IL_005c:  callvirt   instance int64 Beetle.IByteConverter::ReadInt64(class Beetle.BufferReader)
    IL_0061:  ret
  } // end of method BufferReader::ReadInt64

  .method public hidebysig instance int64[] 
          ReadInt64s() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  3
    .locals init (int64[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<int64>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0018

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldarg.0
    IL_000e:  call       instance int64 Beetle.BufferReader::ReadInt64()
    IL_0013:  stelem.i8
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_000b

    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method BufferReader::ReadInt64s

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<int64> 
          ReadInt64List() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<int64> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<int64>(int32&)
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_001d

    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  call       instance int64 Beetle.BufferReader::ReadInt64()
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int64>::Add(!0)
    IL_0019:  ldloc.2
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldloc.0
    IL_001f:  blt.s      IL_000d

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method BufferReader::ReadInt64List

  .method public hidebysig instance float32 
          ReadFloat() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.BufferReader::EnabledVariant
    IL_0006:  brfalse.s  IL_0014

    IL_0008:  ldarg.0
    IL_0009:  call       instance int32 Beetle.BufferReader::Read32Variant()
    IL_000e:  stloc.0
    IL_000f:  ldloca.s   V_0
    IL_0011:  conv.u
    IL_0012:  ldind.r4
    IL_0013:  ret

    IL_0014:  ldarg.0
    IL_0015:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_001a:  stloc.0
    IL_001b:  ldloca.s   V_0
    IL_001d:  conv.u
    IL_001e:  ldind.r4
    IL_001f:  ret
  } // end of method BufferReader::ReadFloat

  .method public hidebysig instance float32[] 
          ReadFloats() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  3
    .locals init (float32[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<float32>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0018

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldarg.0
    IL_000e:  call       instance float32 Beetle.BufferReader::ReadFloat()
    IL_0013:  stelem.r4
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_000b

    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method BufferReader::ReadFloats

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<float32> 
          ReadFloatList() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<float32> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<float32>(int32&)
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_001d

    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  call       instance float32 Beetle.BufferReader::ReadFloat()
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<float32>::Add(!0)
    IL_0019:  ldloc.2
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldloc.0
    IL_001f:  blt.s      IL_000d

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method BufferReader::ReadFloatList

  .method public hidebysig instance valuetype [mscorlib]System.DateTime 
          ReadDate() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance int64 Beetle.BufferReader::ReadInt64()
    IL_0006:  newobj     instance void [mscorlib]System.DateTime::.ctor(int64)
    IL_000b:  ret
  } // end of method BufferReader::ReadDate

  .method public hidebysig instance valuetype [mscorlib]System.DateTime[] 
          ReadDates() cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.DateTime[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<valuetype [mscorlib]System.DateTime>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0021

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldelema    [mscorlib]System.DateTime
    IL_0012:  ldarg.0
    IL_0013:  call       instance valuetype [mscorlib]System.DateTime Beetle.BufferReader::ReadDate()
    IL_0018:  stobj      [mscorlib]System.DateTime
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldloc.0
    IL_0023:  ldlen
    IL_0024:  conv.i4
    IL_0025:  blt.s      IL_000b

    IL_0027:  ldloc.0
    IL_0028:  ret
  } // end of method BufferReader::ReadDates

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.DateTime> 
          ReadDateTimeList() cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.DateTime> V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<valuetype [mscorlib]System.DateTime>(int32&)
    IL_000a:  stloc.1
    IL_000b:  ldc.i4.0
    IL_000c:  stloc.2
    IL_000d:  br.s       IL_001f

    IL_000f:  ldloc.1
    IL_0010:  ldarg.0
    IL_0011:  call       instance valuetype [mscorlib]System.DateTime Beetle.BufferReader::ReadDate()
    IL_0016:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<valuetype [mscorlib]System.DateTime>::Add(!0)
    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.1
    IL_001d:  add
    IL_001e:  stloc.2
    IL_001f:  ldloc.2
    IL_0020:  ldloc.0
    IL_0021:  blt.s      IL_000f

    IL_0023:  ldloc.1
    IL_0024:  ret
  } // end of method BufferReader::ReadDateTimeList

  .method public hidebysig instance string 
          ReadString() cil managed
  {
    // 代码大小       135 (0x87)
    .maxstack  4
    .locals init (int32 V_0,
             string V_1,
             class Beetle.ByteArraySegment V_2)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldsfld     int32 Beetle.TcpUtils::DataPacketMaxLength
    IL_000d:  ble.s      IL_0015

    IL_000f:  call       class Beetle.NetTcpException Beetle.NetTcpException::DataOverflow()
    IL_0014:  pop
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  ble.s      IL_0085

    IL_0019:  ldarg.0
    IL_001a:  ldfld      class b Beetle.BufferReader::e
    IL_001f:  ldloc.0
    IL_0020:  callvirt   instance bool b::a(int32)
    IL_0025:  brfalse.s  IL_005f

    IL_0027:  ldarg.0
    IL_0028:  ldfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    IL_002d:  ldarg.0
    IL_002e:  ldfld      class b Beetle.BufferReader::e
    IL_0033:  ldfld      uint8[] b::e
    IL_0038:  ldarg.0
    IL_0039:  ldfld      class b Beetle.BufferReader::e
    IL_003e:  ldfld      int32 b::g
    IL_0043:  ldloc.0
    IL_0044:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[],
                                                                                   int32,
                                                                                   int32)
    IL_0049:  stloc.1
    IL_004a:  ldarg.0
    IL_004b:  ldfld      class b Beetle.BufferReader::e
    IL_0050:  dup
    IL_0051:  ldfld      int32 b::g
    IL_0056:  ldloc.0
    IL_0057:  add
    IL_0058:  stfld      int32 b::g
    IL_005d:  ldloc.1
    IL_005e:  ret

    IL_005f:  ldarg.0
    IL_0060:  ldloc.0
    IL_0061:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0066:  stloc.2
    IL_0067:  ldarg.0
    IL_0068:  ldfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    IL_006d:  ldloc.2
    IL_006e:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0073:  ldloc.2
    IL_0074:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0079:  ldloc.2
    IL_007a:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_007f:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[],
                                                                                   int32,
                                                                                   int32)
    IL_0084:  ret

    IL_0085:  ldnull
    IL_0086:  ret
  } // end of method BufferReader::ReadString

  .method public hidebysig instance string 
          ReadShortString() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (uint16 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance uint16 Beetle.BufferReader::ReadUInt16()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldsfld     int32 Beetle.TcpUtils::DataPacketMaxLength
    IL_000d:  ble.s      IL_0015

    IL_000f:  call       class Beetle.NetTcpException Beetle.NetTcpException::DataOverflow()
    IL_0014:  pop
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  ble.s      IL_0021

    IL_0019:  ldarg.0
    IL_001a:  ldloc.0
    IL_001b:  call       instance string Beetle.BufferReader::ReadString(int32)
    IL_0020:  ret

    IL_0021:  ldnull
    IL_0022:  ret
  } // end of method BufferReader::ReadShortString

  .method public hidebysig instance string[] 
          ReadStrings() cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  3
    .locals init (string[] V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance !!0[] Beetle.BufferReader::createArray<string>()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0018

    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  ldarg.0
    IL_000e:  call       instance string Beetle.BufferReader::ReadString()
    IL_0013:  stelem.ref
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_000b

    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method BufferReader::ReadStrings

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<string> 
          ReadStringList() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<string> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance class [mscorlib]System.Collections.Generic.IList`1<!!0> Beetle.BufferReader::createList<string>(int32&)
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.2
    IL_000b:  br.s       IL_001d

    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  call       instance string Beetle.BufferReader::ReadString()
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<string>::Add(!0)
    IL_0019:  ldloc.2
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldloc.0
    IL_001f:  blt.s      IL_000d

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method BufferReader::ReadStringList

  .method public hidebysig instance !!T  ReadObject<.ctor (Beetle.IMessage, Beetle.IObjectReset) T>(class Beetle.MessagePool`1<!!T> pool) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  2
    .locals init (!!T V_0)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance !0 class Beetle.MessagePool`1<!!T>::Pop()
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  ldarg.0
    IL_000a:  constrained. !!T
    IL_0010:  callvirt   instance void Beetle.IMessage::Load(class Beetle.BufferReader)
    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method BufferReader::ReadObject

  .method public hidebysig instance !!T  ReadObject<.ctor (Beetle.IMessage) T>() cil managed
  {
    // 代码大小       49 (0x31)
    .maxstack  2
    .locals init (!!T V_0,
             !!T V_1,
             !!T V_2)
    IL_0000:  ldloca.s   V_1
    IL_0002:  initobj    !!T
    IL_0008:  ldloc.1
    IL_0009:  box        !!T
    IL_000e:  brfalse.s  IL_001b

    IL_0010:  ldloca.s   V_2
    IL_0012:  initobj    !!T
    IL_0018:  ldloc.2
    IL_0019:  br.s       IL_0020

    IL_001b:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0020:  stloc.0
    IL_0021:  ldloca.s   V_0
    IL_0023:  ldarg.0
    IL_0024:  constrained. !!T
    IL_002a:  callvirt   instance void Beetle.IMessage::Load(class Beetle.BufferReader)
    IL_002f:  ldloc.0
    IL_0030:  ret
  } // end of method BufferReader::ReadObject

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<!!T> 
          ReadObjects<.ctor (Beetle.IMessage, Beetle.IObjectReset) T>(class Beetle.MessagePool`1<!!T> pool) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.List`1<!!T> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<!!T>::.ctor(int32)
    IL_000d:  stloc.1
    IL_000e:  ldc.i4.0
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_0023

    IL_0012:  ldloc.1
    IL_0013:  ldarg.0
    IL_0014:  ldarg.1
    IL_0015:  call       instance !!0 Beetle.BufferReader::ReadObject<!!0>(class Beetle.MessagePool`1<!!0>)
    IL_001a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<!!T>::Add(!0)
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.2
    IL_0023:  ldloc.2
    IL_0024:  ldloc.0
    IL_0025:  blt.s      IL_0012

    IL_0027:  ldloc.1
    IL_0028:  ret
  } // end of method BufferReader::ReadObjects

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IList`1<!!T> 
          ReadObjects<.ctor (Beetle.IMessage) T>() cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.List`1<!!T> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Beetle.BufferReader::ReadInt32()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<!!T>::.ctor(int32)
    IL_000d:  stloc.1
    IL_000e:  ldc.i4.0
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_0022

    IL_0012:  ldloc.1
    IL_0013:  ldarg.0
    IL_0014:  call       instance !!0 Beetle.BufferReader::ReadObject<!!0>()
    IL_0019:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<!!T>::Add(!0)
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.2
    IL_0022:  ldloc.2
    IL_0023:  ldloc.0
    IL_0024:  blt.s      IL_0012

    IL_0026:  ldloc.1
    IL_0027:  ret
  } // end of method BufferReader::ReadObjects

  .method public hidebysig instance string 
          ReadString(int32 count) cil managed
  {
    // 代码大小       108 (0x6c)
    .maxstack  4
    .locals init (string V_0,
             class Beetle.ByteArraySegment V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class b Beetle.BufferReader::e
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance bool b::a(int32)
    IL_000c:  brfalse.s  IL_0046

    IL_000e:  ldarg.0
    IL_000f:  ldfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    IL_0014:  ldarg.0
    IL_0015:  ldfld      class b Beetle.BufferReader::e
    IL_001a:  ldfld      uint8[] b::e
    IL_001f:  ldarg.0
    IL_0020:  ldfld      class b Beetle.BufferReader::e
    IL_0025:  ldfld      int32 b::g
    IL_002a:  ldarg.1
    IL_002b:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[],
                                                                                   int32,
                                                                                   int32)
    IL_0030:  stloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      class b Beetle.BufferReader::e
    IL_0037:  dup
    IL_0038:  ldfld      int32 b::g
    IL_003d:  ldarg.1
    IL_003e:  add
    IL_003f:  stfld      int32 b::g
    IL_0044:  ldloc.0
    IL_0045:  ret

    IL_0046:  ldarg.0
    IL_0047:  ldarg.1
    IL_0048:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_004d:  stloc.1
    IL_004e:  ldarg.0
    IL_004f:  ldfld      class [mscorlib]System.Text.Encoding Beetle.BufferReader::Coding
    IL_0054:  ldloc.1
    IL_0055:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_005a:  ldloc.1
    IL_005b:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0060:  ldloc.1
    IL_0061:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_0066:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[],
                                                                                   int32,
                                                                                   int32)
    IL_006b:  ret
  } // end of method BufferReader::ReadString

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       142 (0x8e)
    .maxstack  2
    .locals init (int32 V_0,
             class Beetle.BufferReader V_1)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.1
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool Beetle.BufferReader::m
      IL_000e:  brtrue.s   IL_0084

      IL_0010:  ldc.i4.0
      IL_0011:  stloc.0
      IL_0012:  br.s       IL_0029

      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
      IL_001a:  ldloc.0
      IL_001b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class b>::get_Item(int32)
      IL_0020:  callvirt   instance void b::Dispose()
      IL_0025:  ldloc.0
      IL_0026:  ldc.i4.1
      IL_0027:  add
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
      IL_0030:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class b>::get_Count()
      IL_0035:  blt.s      IL_0014

      IL_0037:  ldarg.0
      IL_0038:  ldfld      class ai Beetle.BufferReader::g
      IL_003d:  brfalse.s  IL_004f

      IL_003f:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
      IL_0044:  ldarg.0
      IL_0045:  ldfld      class ai Beetle.BufferReader::g
      IL_004a:  callvirt   instance void class Beetle.ObjectPool`1<class ai>::Push(!0)
      IL_004f:  ldarg.0
      IL_0050:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
      IL_0055:  brfalse.s  IL_0067

      IL_0057:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class Beetle.IByteConverter Beetle.BufferReader::h
      IL_0062:  callvirt   instance void Beetle.IByteConverterFactory::Push(class Beetle.IByteConverter)
      IL_0067:  ldarg.0
      IL_0068:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class b> Beetle.BufferReader::c
      IL_006d:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class b>::Clear()
      IL_0072:  ldarg.0
      IL_0073:  ldfld      class Beetle.ByteArraySegment Beetle.BufferReader::k
      IL_0078:  callvirt   instance void Beetle.ByteArraySegment::Clear()
      IL_007d:  ldarg.0
      IL_007e:  ldc.i4.1
      IL_007f:  stfld      bool Beetle.BufferReader::m
      IL_0084:  leave.s    IL_008d

    }  // end .try
    finally
    {
      IL_0086:  ldloc.1
      IL_0087:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_008c:  endfinally
    }  // end handler
    IL_008d:  ret
  } // end of method BufferReader::Dispose

  .method public hidebysig instance float32 
          ReadFloatVariant() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Beetle.BufferReader::Read32Variant()
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  conv.u
    IL_000a:  ldind.r4
    IL_000b:  ret
  } // end of method BufferReader::ReadFloatVariant

  .method public hidebysig instance float64 
          ReadDoubleVariant() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  1
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int64 Beetle.BufferReader::Read64Variant()
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  conv.u
    IL_000a:  ldind.r8
    IL_000b:  ret
  } // end of method BufferReader::ReadDoubleVariant

  .method public hidebysig instance int32 
          Read32Variant() cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance uint32 Beetle.BufferReader::b()
    IL_0007:  call       instance int32 Beetle.BufferReader::a(uint32)
    IL_000c:  ret
  } // end of method BufferReader::Read32Variant

  .method public hidebysig instance uint32 
          ReadU32Variant() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance uint32 Beetle.BufferReader::b()
    IL_0006:  ret
  } // end of method BufferReader::ReadU32Variant

  .method public hidebysig instance int64 
          Read64Variant() cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance uint64 Beetle.BufferReader::a()
    IL_0007:  call       instance int64 Beetle.BufferReader::a(uint64)
    IL_000c:  ret
  } // end of method BufferReader::Read64Variant

  .method public hidebysig instance uint64 
          ReadU64Variant() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance uint64 Beetle.BufferReader::a()
    IL_0006:  ret
  } // end of method BufferReader::ReadU64Variant

  .method private hidebysig instance int32 
          a(uint32 A_0) cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  and
    IL_0005:  neg
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  shr
    IL_0009:  ldc.i4     0x7fffffff
    IL_000e:  and
    IL_000f:  xor
    IL_0010:  ret
  } // end of method BufferReader::a

  .method private hidebysig instance int64 
          a(uint64 A_0) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  3
    .locals init (int64 V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  and
    IL_0006:  neg
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  shr
    IL_000a:  ldc.i8     0x7fffffffffffffff
    IL_0013:  and
    IL_0014:  xor
    IL_0015:  ret
  } // end of method BufferReader::a

  .method private hidebysig instance uint32 
          b() cil managed
  {
    // 代码大小       177 (0xb1)
    .maxstack  3
    .locals init (uint32 V_0,
             uint32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0007:  ldc.i4.0
    IL_0008:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4     0x80
    IL_0014:  and
    IL_0015:  brtrue.s   IL_0019

    IL_0017:  ldloc.0
    IL_0018:  ret

    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.s   127
    IL_001c:  and
    IL_001d:  stloc.0
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4.1
    IL_0020:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0025:  ldc.i4.0
    IL_0026:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_002b:  stloc.1
    IL_002c:  ldloc.0
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.s   127
    IL_0030:  and
    IL_0031:  ldc.i4.7
    IL_0032:  shl
    IL_0033:  or
    IL_0034:  stloc.0
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4     0x80
    IL_003b:  and
    IL_003c:  brtrue.s   IL_0040

    IL_003e:  ldloc.0
    IL_003f:  ret

    IL_0040:  ldarg.0
    IL_0041:  ldc.i4.1
    IL_0042:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0047:  ldc.i4.0
    IL_0048:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_004d:  stloc.1
    IL_004e:  ldloc.0
    IL_004f:  ldloc.1
    IL_0050:  ldc.i4.s   127
    IL_0052:  and
    IL_0053:  ldc.i4.s   14
    IL_0055:  shl
    IL_0056:  or
    IL_0057:  stloc.0
    IL_0058:  ldloc.1
    IL_0059:  ldc.i4     0x80
    IL_005e:  and
    IL_005f:  brtrue.s   IL_0063

    IL_0061:  ldloc.0
    IL_0062:  ret

    IL_0063:  ldarg.0
    IL_0064:  ldc.i4.1
    IL_0065:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_006a:  ldc.i4.0
    IL_006b:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_0070:  stloc.1
    IL_0071:  ldloc.0
    IL_0072:  ldloc.1
    IL_0073:  ldc.i4.s   127
    IL_0075:  and
    IL_0076:  ldc.i4.s   21
    IL_0078:  shl
    IL_0079:  or
    IL_007a:  stloc.0
    IL_007b:  ldloc.1
    IL_007c:  ldc.i4     0x80
    IL_0081:  and
    IL_0082:  brtrue.s   IL_0086

    IL_0084:  ldloc.0
    IL_0085:  ret

    IL_0086:  ldarg.0
    IL_0087:  ldc.i4.1
    IL_0088:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_008d:  ldc.i4.0
    IL_008e:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_0093:  stloc.1
    IL_0094:  ldloc.0
    IL_0095:  ldloc.1
    IL_0096:  ldc.i4.s   28
    IL_0098:  shl
    IL_0099:  or
    IL_009a:  stloc.0
    IL_009b:  ldloc.1
    IL_009c:  ldc.i4     0xf0
    IL_00a1:  and
    IL_00a2:  brtrue.s   IL_00a6

    IL_00a4:  ldloc.0
    IL_00a5:  ret

    IL_00a6:  ldstr      "ReadUInt32Variant Error!"
    IL_00ab:  newobj     instance void [mscorlib]System.OverflowException::.ctor(string)
    IL_00b0:  throw
  } // end of method BufferReader::b

  .method private hidebysig instance uint64 
          a() cil managed
  {
    // 代码大小       398 (0x18e)
    .maxstack  3
    .locals init (uint64 V_0,
             uint64 V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0007:  ldc.i4.0
    IL_0008:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_000d:  conv.u8
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4     0x80
    IL_0015:  conv.i8
    IL_0016:  and
    IL_0017:  ldc.i4.0
    IL_0018:  conv.i8
    IL_0019:  bne.un.s   IL_001d

    IL_001b:  ldloc.0
    IL_001c:  ret

    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.s   127
    IL_0020:  conv.i8
    IL_0021:  and
    IL_0022:  stloc.0
    IL_0023:  ldarg.0
    IL_0024:  ldc.i4.1
    IL_0025:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_002a:  ldc.i4.0
    IL_002b:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_0030:  conv.u8
    IL_0031:  stloc.1
    IL_0032:  ldloc.0
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.s   127
    IL_0036:  conv.i8
    IL_0037:  and
    IL_0038:  ldc.i4.7
    IL_0039:  shl
    IL_003a:  or
    IL_003b:  stloc.0
    IL_003c:  ldloc.1
    IL_003d:  ldc.i4     0x80
    IL_0042:  conv.i8
    IL_0043:  and
    IL_0044:  ldc.i4.0
    IL_0045:  conv.i8
    IL_0046:  bne.un.s   IL_004a

    IL_0048:  ldloc.0
    IL_0049:  ret

    IL_004a:  ldarg.0
    IL_004b:  ldc.i4.1
    IL_004c:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0051:  ldc.i4.0
    IL_0052:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_0057:  conv.u8
    IL_0058:  stloc.1
    IL_0059:  ldloc.0
    IL_005a:  ldloc.1
    IL_005b:  ldc.i4.s   127
    IL_005d:  conv.i8
    IL_005e:  and
    IL_005f:  ldc.i4.s   14
    IL_0061:  shl
    IL_0062:  or
    IL_0063:  stloc.0
    IL_0064:  ldloc.1
    IL_0065:  ldc.i4     0x80
    IL_006a:  conv.i8
    IL_006b:  and
    IL_006c:  ldc.i4.0
    IL_006d:  conv.i8
    IL_006e:  bne.un.s   IL_0072

    IL_0070:  ldloc.0
    IL_0071:  ret

    IL_0072:  ldarg.0
    IL_0073:  ldc.i4.1
    IL_0074:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0079:  ldc.i4.0
    IL_007a:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_007f:  conv.u8
    IL_0080:  stloc.1
    IL_0081:  ldloc.0
    IL_0082:  ldloc.1
    IL_0083:  ldc.i4.s   127
    IL_0085:  conv.i8
    IL_0086:  and
    IL_0087:  ldc.i4.s   21
    IL_0089:  shl
    IL_008a:  or
    IL_008b:  stloc.0
    IL_008c:  ldloc.1
    IL_008d:  ldc.i4     0x80
    IL_0092:  conv.i8
    IL_0093:  and
    IL_0094:  ldc.i4.0
    IL_0095:  conv.i8
    IL_0096:  bne.un.s   IL_009a

    IL_0098:  ldloc.0
    IL_0099:  ret

    IL_009a:  ldarg.0
    IL_009b:  ldc.i4.1
    IL_009c:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_00a1:  ldc.i4.0
    IL_00a2:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_00a7:  conv.u8
    IL_00a8:  stloc.1
    IL_00a9:  ldloc.0
    IL_00aa:  ldloc.1
    IL_00ab:  ldc.i4.s   127
    IL_00ad:  conv.i8
    IL_00ae:  and
    IL_00af:  ldc.i4.s   28
    IL_00b1:  shl
    IL_00b2:  or
    IL_00b3:  stloc.0
    IL_00b4:  ldloc.1
    IL_00b5:  ldc.i4     0x80
    IL_00ba:  conv.i8
    IL_00bb:  and
    IL_00bc:  ldc.i4.0
    IL_00bd:  conv.i8
    IL_00be:  bne.un.s   IL_00c2

    IL_00c0:  ldloc.0
    IL_00c1:  ret

    IL_00c2:  ldarg.0
    IL_00c3:  ldc.i4.1
    IL_00c4:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_00c9:  ldc.i4.0
    IL_00ca:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_00cf:  conv.u8
    IL_00d0:  stloc.1
    IL_00d1:  ldloc.0
    IL_00d2:  ldloc.1
    IL_00d3:  ldc.i4.s   127
    IL_00d5:  conv.i8
    IL_00d6:  and
    IL_00d7:  ldc.i4.s   35
    IL_00d9:  shl
    IL_00da:  or
    IL_00db:  stloc.0
    IL_00dc:  ldloc.1
    IL_00dd:  ldc.i4     0x80
    IL_00e2:  conv.i8
    IL_00e3:  and
    IL_00e4:  ldc.i4.0
    IL_00e5:  conv.i8
    IL_00e6:  bne.un.s   IL_00ea

    IL_00e8:  ldloc.0
    IL_00e9:  ret

    IL_00ea:  ldarg.0
    IL_00eb:  ldc.i4.1
    IL_00ec:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_00f1:  ldc.i4.0
    IL_00f2:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_00f7:  conv.u8
    IL_00f8:  stloc.1
    IL_00f9:  ldloc.0
    IL_00fa:  ldloc.1
    IL_00fb:  ldc.i4.s   127
    IL_00fd:  conv.i8
    IL_00fe:  and
    IL_00ff:  ldc.i4.s   42
    IL_0101:  shl
    IL_0102:  or
    IL_0103:  stloc.0
    IL_0104:  ldloc.1
    IL_0105:  ldc.i4     0x80
    IL_010a:  conv.i8
    IL_010b:  and
    IL_010c:  ldc.i4.0
    IL_010d:  conv.i8
    IL_010e:  bne.un.s   IL_0112

    IL_0110:  ldloc.0
    IL_0111:  ret

    IL_0112:  ldarg.0
    IL_0113:  ldc.i4.1
    IL_0114:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0119:  ldc.i4.0
    IL_011a:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_011f:  conv.u8
    IL_0120:  stloc.1
    IL_0121:  ldloc.0
    IL_0122:  ldloc.1
    IL_0123:  ldc.i4.s   127
    IL_0125:  conv.i8
    IL_0126:  and
    IL_0127:  ldc.i4.s   49
    IL_0129:  shl
    IL_012a:  or
    IL_012b:  stloc.0
    IL_012c:  ldloc.1
    IL_012d:  ldc.i4     0x80
    IL_0132:  conv.i8
    IL_0133:  and
    IL_0134:  ldc.i4.0
    IL_0135:  conv.i8
    IL_0136:  bne.un.s   IL_013a

    IL_0138:  ldloc.0
    IL_0139:  ret

    IL_013a:  ldarg.0
    IL_013b:  ldc.i4.1
    IL_013c:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0141:  ldc.i4.0
    IL_0142:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_0147:  conv.u8
    IL_0148:  stloc.1
    IL_0149:  ldloc.0
    IL_014a:  ldloc.1
    IL_014b:  ldc.i4.s   127
    IL_014d:  conv.i8
    IL_014e:  and
    IL_014f:  ldc.i4.s   56
    IL_0151:  shl
    IL_0152:  or
    IL_0153:  stloc.0
    IL_0154:  ldloc.1
    IL_0155:  ldc.i4     0x80
    IL_015a:  conv.i8
    IL_015b:  and
    IL_015c:  ldc.i4.0
    IL_015d:  conv.i8
    IL_015e:  bne.un.s   IL_0162

    IL_0160:  ldloc.0
    IL_0161:  ret

    IL_0162:  ldarg.0
    IL_0163:  ldc.i4.1
    IL_0164:  call       instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0169:  ldc.i4.0
    IL_016a:  callvirt   instance uint8 Beetle.ByteArraySegment::GetData(int32)
    IL_016f:  conv.u8
    IL_0170:  stloc.1
    IL_0171:  ldloc.0
    IL_0172:  ldloc.1
    IL_0173:  ldc.i4.s   63
    IL_0175:  shl
    IL_0176:  or
    IL_0177:  stloc.0
    IL_0178:  ldloc.1
    IL_0179:  ldc.i4.s   -2
    IL_017b:  conv.i8
    IL_017c:  and
    IL_017d:  ldc.i4.0
    IL_017e:  conv.i8
    IL_017f:  bne.un.s   IL_0183

    IL_0181:  ldloc.0
    IL_0182:  ret

    IL_0183:  ldstr      "ReadUInt64Variant Error!"
    IL_0188:  newobj     instance void [mscorlib]System.OverflowException::.ctor(string)
    IL_018d:  throw
  } // end of method BufferReader::a

  .property instance int32 Count()
  {
    .get instance int32 Beetle.BufferReader::get_Count()
  } // end of property BufferReader::Count
} // end of class Beetle.BufferReader

.class public auto ansi beforefieldinit Beetle.DetectTimeoutHandler
       extends Beetle.Task
{
  .field private class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel> a
  .field private class Beetle.TcpServer b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance class Beetle.TcpServer 
          get_Server() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.TcpServer Beetle.DetectTimeoutHandler::b
    IL_0006:  ret
  } // end of method DetectTimeoutHandler::get_Server

  .method assembly hidebysig specialname 
          instance void  a(class Beetle.TcpServer A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class Beetle.TcpServer Beetle.DetectTimeoutHandler::b
    IL_0007:  ret
  } // end of method DetectTimeoutHandler::a

  .method family hidebysig virtual instance void 
          Execute() cil managed
  {
    // 代码大小       119 (0x77)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class Beetle.TcpServer Beetle.DetectTimeoutHandler::get_Server()
    IL_0006:  brfalse.s  IL_0070

    IL_0008:  ldarg.0
    IL_0009:  call       instance class Beetle.TcpServer Beetle.DetectTimeoutHandler::get_Server()
    IL_000e:  callvirt   instance bool Beetle.TcpServer::get_IsRunning()
    IL_0013:  brfalse.s  IL_0070

    IL_0015:  call       int32 [mscorlib]System.Environment::get_TickCount()
    IL_001a:  stloc.0
    IL_001b:  ldarg.0
    IL_001c:  call       instance class Beetle.TcpServer Beetle.DetectTimeoutHandler::get_Server()
    IL_0021:  ldarg.0
    IL_0022:  ldfld      class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel> Beetle.DetectTimeoutHandler::a
    IL_0027:  callvirt   instance void Beetle.TcpServer::GetOnlines(class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel>)
    IL_002c:  ldc.i4.0
    IL_002d:  stloc.1
    IL_002e:  br.s       IL_0061

    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel> Beetle.DetectTimeoutHandler::a
    IL_0037:  callvirt   instance !0[] class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel>::get_Array()
    IL_003c:  ldloc.1
    IL_003d:  ldelem.ref
    IL_003e:  ldfld      int32 Beetle.TcpChannel::TickCount
    IL_0043:  sub
    IL_0044:  ldsfld     int32 Beetle.TcpUtils::TimeOutms
    IL_0049:  blt.s      IL_005d

    IL_004b:  ldarg.0
    IL_004c:  ldfld      class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel> Beetle.DetectTimeoutHandler::a
    IL_0051:  callvirt   instance !0[] class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel>::get_Array()
    IL_0056:  ldloc.1
    IL_0057:  ldelem.ref
    IL_0058:  callvirt   instance void Beetle.TcpChannel::Dispose()
    IL_005d:  ldloc.1
    IL_005e:  ldc.i4.1
    IL_005f:  add
    IL_0060:  stloc.1
    IL_0061:  ldloc.1
    IL_0062:  ldarg.0
    IL_0063:  ldfld      class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel> Beetle.DetectTimeoutHandler::a
    IL_0068:  callvirt   instance int32 class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel>::get_Count()
    IL_006d:  blt.s      IL_0030

    IL_006f:  ret

    IL_0070:  ldarg.0
    IL_0071:  call       instance void Beetle.Task::Dispose()
    IL_0076:  ret
  } // end of method DetectTimeoutHandler::Execute

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldsfld     int32 Beetle.TcpUtils::Connections
    IL_0006:  ldc.i4.2
    IL_0007:  mul
    IL_0008:  newobj     instance void class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel>::.ctor(int32)
    IL_000d:  stfld      class Beetle.ObjectArraySegment`1<class Beetle.TcpChannel> Beetle.DetectTimeoutHandler::a
    IL_0012:  ldarg.0
    IL_0013:  call       instance void Beetle.Task::.ctor()
    IL_0018:  ret
  } // end of method DetectTimeoutHandler::.ctor

  .property instance class Beetle.TcpServer
          Server()
  {
    .get instance class Beetle.TcpServer Beetle.DetectTimeoutHandler::get_Server()
    .set instance void Beetle.DetectTimeoutHandler::a(class Beetle.TcpServer)
  } // end of property DetectTimeoutHandler::Server
} // end of class Beetle.DetectTimeoutHandler

.class public auto ansi beforefieldinit Beetle.ChannelReceiveEventArgs
       extends Beetle.ChannelEventArgs
{
  .field assembly class Beetle.ByteArraySegment a
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Beetle.ChannelEventArgs::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_000c:  stfld      class Beetle.ByteArraySegment Beetle.ChannelReceiveEventArgs::a
    IL_0011:  ret
  } // end of method ChannelReceiveEventArgs::.ctor

  .method public hidebysig specialname instance class Beetle.ByteArraySegment 
          get_Data() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.ByteArraySegment Beetle.ChannelReceiveEventArgs::a
    IL_0006:  ret
  } // end of method ChannelReceiveEventArgs::get_Data

  .property instance class Beetle.ByteArraySegment
          Data()
  {
    .get instance class Beetle.ByteArraySegment Beetle.ChannelReceiveEventArgs::get_Data()
  } // end of property ChannelReceiveEventArgs::Data
} // end of class Beetle.ChannelReceiveEventArgs

.class public abstract auto ansi beforefieldinit Beetle.HeadSizeOfPackage
       extends Beetle.Package
{
  .field private int32 a
  .field private uint8[] b
  .field private int32 c
  .field private int32 d
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.4
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  stfld      uint8[] Beetle.HeadSizeOfPackage::b
    IL_000c:  ldarg.0
    IL_000d:  call       instance void Beetle.Package::.ctor()
    IL_0012:  ret
  } // end of method HeadSizeOfPackage::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class Beetle.TcpChannel channel) cil managed
  {
    // 代码大小       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.4
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  stfld      uint8[] Beetle.HeadSizeOfPackage::b
    IL_000c:  ldarg.0
    IL_000d:  ldarg.1
    IL_000e:  call       instance void Beetle.Package::.ctor(class Beetle.TcpChannel)
    IL_0013:  ret
  } // end of method HeadSizeOfPackage::.ctor

  .method family hidebysig newslot abstract virtual 
          instance class Beetle.IMessage 
          ReadMessageByType(class Beetle.BufferReader reader,
                            [out] object& typeTag) cil managed
  {
  } // end of method HeadSizeOfPackage::ReadMessageByType

  .method family hidebysig newslot abstract virtual 
          instance void  WriteMessageType(class Beetle.IMessage msg,
                                          class Beetle.BufferWriter writer) cil managed
  {
  } // end of method HeadSizeOfPackage::WriteMessageType

  .method public hidebysig virtual instance void 
          Import(uint8[] data,
                 int32 start,
                 int32 count) cil managed
  {
    // 代码大小       459 (0x1cb)
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.3
    IL_0002:  stfld      int32 Beetle.HeadSizeOfPackage::d
    IL_0007:  br         IL_01be

    IL_000c:  ldarg.0
    IL_000d:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_0012:  brtrue     IL_013e

    IL_0017:  ldarg.0
    IL_0018:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_001d:  ldc.i4.4
    IL_001e:  bge.s      IL_0048

    IL_0020:  ldarg.0
    IL_0021:  ldarg.0
    IL_0022:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_0027:  stfld      int32 Beetle.HeadSizeOfPackage::c
    IL_002c:  ldarg.1
    IL_002d:  ldarg.2
    IL_002e:  ldarg.0
    IL_002f:  ldfld      uint8[] Beetle.HeadSizeOfPackage::b
    IL_0034:  ldc.i4.0
    IL_0035:  ldarg.0
    IL_0036:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_003b:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_0040:  ldarg.0
    IL_0041:  ldc.i4.0
    IL_0042:  stfld      int32 Beetle.HeadSizeOfPackage::d
    IL_0047:  ret

    IL_0048:  ldarg.0
    IL_0049:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_004e:  callvirt   instance void Beetle.BufferWriter::Reset()
    IL_0053:  ldarg.0
    IL_0054:  ldfld      int32 Beetle.HeadSizeOfPackage::c
    IL_0059:  brtrue.s   IL_00b8

    IL_005b:  ldarg.0
    IL_005c:  ldarg.1
    IL_005d:  ldarg.2
    IL_005e:  call       int32 [mscorlib]System.BitConverter::ToInt32(uint8[],
                                                                      int32)
    IL_0063:  stfld      int32 Beetle.HeadSizeOfPackage::a
    IL_0068:  ldarg.0
    IL_0069:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_006e:  ldsfld     int32 Beetle.TcpUtils::DataPacketMaxLength
    IL_0073:  bgt.s      IL_007e

    IL_0075:  ldarg.0
    IL_0076:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_007b:  ldc.i4.0
    IL_007c:  bgt.s      IL_0084

    IL_007e:  call       class Beetle.NetTcpException Beetle.NetTcpException::DataOverflow()
    IL_0083:  throw

    IL_0084:  ldarg.0
    IL_0085:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_008a:  ldarg.1
    IL_008b:  ldarg.2
    IL_008c:  ldc.i4.4
    IL_008d:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0092:  ldarg.2
    IL_0093:  ldc.i4.4
    IL_0094:  add
    IL_0095:  starg.s    start
    IL_0097:  ldarg.0
    IL_0098:  ldarg.0
    IL_0099:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_009e:  ldc.i4.4
    IL_009f:  sub
    IL_00a0:  stfld      int32 Beetle.HeadSizeOfPackage::a
    IL_00a5:  ldarg.0
    IL_00a6:  ldarg.0
    IL_00a7:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_00ac:  ldc.i4.4
    IL_00ad:  sub
    IL_00ae:  stfld      int32 Beetle.HeadSizeOfPackage::d
    IL_00b3:  br         IL_013e

    IL_00b8:  ldc.i4.4
    IL_00b9:  ldarg.0
    IL_00ba:  ldfld      int32 Beetle.HeadSizeOfPackage::c
    IL_00bf:  sub
    IL_00c0:  stloc.0
    IL_00c1:  ldarg.1
    IL_00c2:  ldarg.2
    IL_00c3:  ldarg.0
    IL_00c4:  ldfld      uint8[] Beetle.HeadSizeOfPackage::b
    IL_00c9:  ldarg.0
    IL_00ca:  ldfld      int32 Beetle.HeadSizeOfPackage::c
    IL_00cf:  ldloc.0
    IL_00d0:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_00d5:  ldarg.0
    IL_00d6:  ldc.i4.0
    IL_00d7:  stfld      int32 Beetle.HeadSizeOfPackage::c
    IL_00dc:  ldarg.0
    IL_00dd:  ldarg.0
    IL_00de:  ldfld      uint8[] Beetle.HeadSizeOfPackage::b
    IL_00e3:  ldc.i4.0
    IL_00e4:  call       int32 [mscorlib]System.BitConverter::ToInt32(uint8[],
                                                                      int32)
    IL_00e9:  stfld      int32 Beetle.HeadSizeOfPackage::a
    IL_00ee:  ldarg.0
    IL_00ef:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_00f4:  ldsfld     int32 Beetle.TcpUtils::DataPacketMaxLength
    IL_00f9:  bgt.s      IL_0104

    IL_00fb:  ldarg.0
    IL_00fc:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_0101:  ldc.i4.0
    IL_0102:  bgt.s      IL_010a

    IL_0104:  call       class Beetle.NetTcpException Beetle.NetTcpException::DataOverflow()
    IL_0109:  throw

    IL_010a:  ldarg.0
    IL_010b:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0110:  ldarg.0
    IL_0111:  ldfld      uint8[] Beetle.HeadSizeOfPackage::b
    IL_0116:  ldc.i4.0
    IL_0117:  ldc.i4.4
    IL_0118:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_011d:  ldarg.2
    IL_011e:  ldloc.0
    IL_011f:  add
    IL_0120:  starg.s    start
    IL_0122:  ldarg.0
    IL_0123:  ldarg.0
    IL_0124:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_0129:  ldc.i4.4
    IL_012a:  sub
    IL_012b:  stfld      int32 Beetle.HeadSizeOfPackage::a
    IL_0130:  ldarg.0
    IL_0131:  ldarg.0
    IL_0132:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_0137:  ldloc.0
    IL_0138:  sub
    IL_0139:  stfld      int32 Beetle.HeadSizeOfPackage::d
    IL_013e:  ldarg.0
    IL_013f:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_0144:  ldarg.0
    IL_0145:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_014a:  blt.s      IL_0191

    IL_014c:  ldarg.0
    IL_014d:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0152:  ldarg.1
    IL_0153:  ldarg.2
    IL_0154:  ldarg.0
    IL_0155:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_015a:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_015f:  ldarg.0
    IL_0160:  ldarg.0
    IL_0161:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0166:  callvirt   instance void Beetle.Package::OnMessageDataReader(class Beetle.BufferWriter)
    IL_016b:  ldarg.2
    IL_016c:  ldarg.0
    IL_016d:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_0172:  add
    IL_0173:  starg.s    start
    IL_0175:  ldarg.0
    IL_0176:  ldarg.0
    IL_0177:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_017c:  ldarg.0
    IL_017d:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_0182:  sub
    IL_0183:  stfld      int32 Beetle.HeadSizeOfPackage::d
    IL_0188:  ldarg.0
    IL_0189:  ldc.i4.0
    IL_018a:  stfld      int32 Beetle.HeadSizeOfPackage::a
    IL_018f:  br.s       IL_01be

    IL_0191:  ldarg.0
    IL_0192:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0197:  ldarg.1
    IL_0198:  ldarg.2
    IL_0199:  ldarg.0
    IL_019a:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_019f:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_01a4:  ldarg.0
    IL_01a5:  ldarg.0
    IL_01a6:  ldfld      int32 Beetle.HeadSizeOfPackage::a
    IL_01ab:  ldarg.0
    IL_01ac:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_01b1:  sub
    IL_01b2:  stfld      int32 Beetle.HeadSizeOfPackage::a
    IL_01b7:  ldarg.0
    IL_01b8:  ldc.i4.0
    IL_01b9:  stfld      int32 Beetle.HeadSizeOfPackage::d
    IL_01be:  ldarg.0
    IL_01bf:  ldfld      int32 Beetle.HeadSizeOfPackage::d
    IL_01c4:  ldc.i4.0
    IL_01c5:  bgt        IL_000c

    IL_01ca:  ret
  } // end of method HeadSizeOfPackage::Import

  .method public hidebysig virtual instance void 
          MessageWrite(class Beetle.IMessage msg,
                       class Beetle.BufferWriter writer) cil managed
  {
    // 代码大小       45 (0x2d)
    .maxstack  3
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> V_1)
    IL_0000:  ldarg.2
    IL_0001:  ldfld      int32 Beetle.BufferWriter::e
    IL_0006:  stloc.0
    IL_0007:  ldarg.2
    IL_0008:  callvirt   instance class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment> Beetle.BufferWriter::WriteHeadSize()
    IL_000d:  stloc.1
    IL_000e:  ldarg.0
    IL_000f:  ldarg.1
    IL_0010:  ldarg.2
    IL_0011:  callvirt   instance void Beetle.HeadSizeOfPackage::WriteMessageType(class Beetle.IMessage,
                                                                                  class Beetle.BufferWriter)
    IL_0016:  ldarg.1
    IL_0017:  ldarg.2
    IL_0018:  callvirt   instance void Beetle.IMessage::Save(class Beetle.BufferWriter)
    IL_001d:  ldarg.2
    IL_001e:  ldarg.2
    IL_001f:  callvirt   instance int32 Beetle.BufferWriter::get_Count()
    IL_0024:  ldloc.0
    IL_0025:  sub
    IL_0026:  ldloc.1
    IL_0027:  callvirt   instance void Beetle.BufferWriter::SetHeadSize(int32,
                                                                        class [mscorlib]System.Collections.Generic.IList`1<class Beetle.ByteArraySegment>)
    IL_002c:  ret
  } // end of method HeadSizeOfPackage::MessageWrite

  .method public hidebysig virtual instance class Beetle.IMessage 
          MessageRead(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       62 (0x3e)
    .maxstack  3
    .locals init (class Beetle.IMessage V_0,
             object V_1,
             class [mscorlib]System.Exception V_2,
             class Beetle.NetTcpException V_3)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  ldc.i4.4
    IL_0004:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0009:  pop
    IL_000a:  ldarg.0
    IL_000b:  ldarg.1
    IL_000c:  ldloca.s   V_1
    IL_000e:  callvirt   instance class Beetle.IMessage Beetle.HeadSizeOfPackage::ReadMessageByType(class Beetle.BufferReader,
                                                                                                    object&)
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  brtrue.s   IL_0023

    IL_0017:  ldloc.1
    IL_0018:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_001d:  call       class Beetle.NetTcpException Beetle.NetTcpException::TypeNotFound(string)
    IL_0022:  throw

    .try
    {
      IL_0023:  ldloc.0
      IL_0024:  ldarg.1
      IL_0025:  callvirt   instance void Beetle.IMessage::Load(class Beetle.BufferReader)
      IL_002a:  leave.s    IL_003c

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_002c:  stloc.2
      IL_002d:  ldloc.1
      IL_002e:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0033:  ldloc.2
      IL_0034:  call       class Beetle.NetTcpException Beetle.NetTcpException::ObjectLoadError(string,
                                                                                                class [mscorlib]System.Exception)
      IL_0039:  stloc.3
      IL_003a:  ldloc.3
      IL_003b:  throw

    }  // end handler
    IL_003c:  ldloc.0
    IL_003d:  ret
  } // end of method HeadSizeOfPackage::MessageRead

  .method family hidebysig virtual instance void 
          OnDisposed() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Beetle.Package::OnDisposed()
    IL_0006:  ret
  } // end of method HeadSizeOfPackage::OnDisposed

} // end of class Beetle.HeadSizeOfPackage

.class public abstract auto ansi beforefieldinit Beetle.EofDataOfPackage
       extends Beetle.Package
{
  .field private int32 a
  .field private bool b
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  stfld      bool Beetle.EofDataOfPackage::b
    IL_0007:  ldarg.0
    IL_0008:  call       instance void Beetle.Package::.ctor()
    IL_000d:  ret
  } // end of method EofDataOfPackage::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class Beetle.TcpChannel channel) cil managed
  {
    // 代码大小       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  stfld      bool Beetle.EofDataOfPackage::b
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  call       instance void Beetle.Package::.ctor(class Beetle.TcpChannel)
    IL_000e:  ret
  } // end of method EofDataOfPackage::.ctor

  .method family hidebysig newslot abstract virtual 
          instance class Beetle.IMessage 
          ReadMessageByType(class Beetle.BufferReader reader,
                            [out] object& typeTag) cil managed
  {
  } // end of method EofDataOfPackage::ReadMessageByType

  .method family hidebysig newslot abstract virtual 
          instance void  WriteMessageType(class Beetle.IMessage msg,
                                          class Beetle.BufferWriter writer) cil managed
  {
  } // end of method EofDataOfPackage::WriteMessageType

  .method family hidebysig newslot specialname abstract virtual 
          instance uint8[]  get_EofData() cil managed
  {
  } // end of method EofDataOfPackage::get_EofData

  .method public hidebysig virtual instance void 
          MessageWrite(class Beetle.IMessage msg,
                       class Beetle.BufferWriter writer) cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  callvirt   instance void Beetle.EofDataOfPackage::WriteMessageType(class Beetle.IMessage,
                                                                                 class Beetle.BufferWriter)
    IL_0008:  ldarg.1
    IL_0009:  ldarg.2
    IL_000a:  callvirt   instance void Beetle.IMessage::Save(class Beetle.BufferWriter)
    IL_000f:  ldarg.2
    IL_0010:  ldarg.0
    IL_0011:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_0016:  ldc.i4.0
    IL_0017:  ldarg.0
    IL_0018:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_001d:  ldlen
    IL_001e:  conv.i4
    IL_001f:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0024:  ret
  } // end of method EofDataOfPackage::MessageWrite

  .method public hidebysig virtual instance class Beetle.IMessage 
          MessageRead(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       52 (0x34)
    .maxstack  3
    .locals init (object V_0,
             class Beetle.IMessage V_1,
             class [mscorlib]System.Exception V_2,
             class Beetle.NetTcpException V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldloca.s   V_0
    IL_0004:  callvirt   instance class Beetle.IMessage Beetle.EofDataOfPackage::ReadMessageByType(class Beetle.BufferReader,
                                                                                                   object&)
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  brtrue.s   IL_0019

    IL_000d:  ldloc.0
    IL_000e:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0013:  call       class Beetle.NetTcpException Beetle.NetTcpException::TypeNotFound(string)
    IL_0018:  throw

    .try
    {
      IL_0019:  ldloc.1
      IL_001a:  ldarg.1
      IL_001b:  callvirt   instance void Beetle.IMessage::Load(class Beetle.BufferReader)
      IL_0020:  leave.s    IL_0032

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0022:  stloc.2
      IL_0023:  ldloc.0
      IL_0024:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0029:  ldloc.2
      IL_002a:  call       class Beetle.NetTcpException Beetle.NetTcpException::ObjectLoadError(string,
                                                                                                class [mscorlib]System.Exception)
      IL_002f:  stloc.3
      IL_0030:  ldloc.3
      IL_0031:  throw

    }  // end handler
    IL_0032:  ldloc.1
    IL_0033:  ret
  } // end of method EofDataOfPackage::MessageRead

  .method public hidebysig virtual instance void 
          Import(uint8[] data,
                 int32 start,
                 int32 count) cil managed
  {
    // 代码大小       434 (0x1b2)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             bool V_3,
             int32 V_4,
             int32 V_5,
             bool V_6,
             int32 V_7,
             int32 V_8)
    IL_0000:  ldarg.2
    IL_0001:  ldarg.3
    IL_0002:  add
    IL_0003:  stloc.0
    IL_0004:  br         IL_01aa

    IL_0009:  ldarg.0
    IL_000a:  ldfld      bool Beetle.EofDataOfPackage::b
    IL_000f:  brfalse.s  IL_0023

    IL_0011:  ldarg.0
    IL_0012:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0017:  callvirt   instance void Beetle.BufferWriter::Reset()
    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.0
    IL_001e:  stfld      bool Beetle.EofDataOfPackage::b
    IL_0023:  ldarg.0
    IL_0024:  ldfld      int32 Beetle.EofDataOfPackage::a
    IL_0029:  brtrue     IL_0129

    IL_002e:  ldarg.1
    IL_002f:  ldarg.0
    IL_0030:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_0035:  ldarg.2
    IL_0036:  ldarg.3
    IL_0037:  call       int32 Beetle.Package::ByteIndexOf(uint8[],
                                                           uint8[],
                                                           int32,
                                                           int32)
    IL_003c:  stloc.1
    IL_003d:  ldloc.1
    IL_003e:  ldc.i4.m1
    IL_003f:  bne.un     IL_00d8

    IL_0044:  ldarg.0
    IL_0045:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_004a:  callvirt   instance int32 Beetle.BufferWriter::get_Count()
    IL_004f:  ldarg.3
    IL_0050:  add
    IL_0051:  ldsfld     int32 Beetle.TcpUtils::DataPacketMaxLength
    IL_0056:  ble.s      IL_005e

    IL_0058:  call       class Beetle.NetTcpException Beetle.NetTcpException::DataOverflow()
    IL_005d:  throw

    IL_005e:  ldarg.0
    IL_005f:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0064:  ldarg.1
    IL_0065:  ldarg.2
    IL_0066:  ldarg.3
    IL_0067:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_006c:  ldc.i4.0
    IL_006d:  stloc.2
    IL_006e:  ldc.i4.0
    IL_006f:  stloc.3
    IL_0070:  ldloc.0
    IL_0071:  ldarg.0
    IL_0072:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_0077:  ldlen
    IL_0078:  conv.i4
    IL_0079:  ldc.i4.1
    IL_007a:  sub
    IL_007b:  sub
    IL_007c:  stloc.s    V_4
    IL_007e:  br.s       IL_00c3

    IL_0080:  ldc.i4.1
    IL_0081:  stloc.3
    IL_0082:  ldc.i4.0
    IL_0083:  stloc.s    V_5
    IL_0085:  br.s       IL_00a9

    IL_0087:  ldloc.s    V_4
    IL_0089:  ldloc.s    V_5
    IL_008b:  add
    IL_008c:  ldloc.0
    IL_008d:  bge.s      IL_00b5

    IL_008f:  ldarg.1
    IL_0090:  ldloc.s    V_4
    IL_0092:  ldloc.s    V_5
    IL_0094:  add
    IL_0095:  ldelem.u1
    IL_0096:  ldarg.0
    IL_0097:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_009c:  ldloc.s    V_5
    IL_009e:  ldelem.u1
    IL_009f:  beq.s      IL_00a3

    IL_00a1:  ldc.i4.0
    IL_00a2:  stloc.3
    IL_00a3:  ldloc.s    V_5
    IL_00a5:  ldc.i4.1
    IL_00a6:  add
    IL_00a7:  stloc.s    V_5
    IL_00a9:  ldloc.s    V_5
    IL_00ab:  ldarg.0
    IL_00ac:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_00b1:  ldlen
    IL_00b2:  conv.i4
    IL_00b3:  blt.s      IL_0087

    IL_00b5:  ldloc.3
    IL_00b6:  brfalse.s  IL_00bd

    IL_00b8:  ldloc.s    V_4
    IL_00ba:  stloc.2
    IL_00bb:  br.s       IL_00c8

    IL_00bd:  ldloc.s    V_4
    IL_00bf:  ldc.i4.1
    IL_00c0:  add
    IL_00c1:  stloc.s    V_4
    IL_00c3:  ldloc.s    V_4
    IL_00c5:  ldloc.0
    IL_00c6:  blt.s      IL_0080

    IL_00c8:  ldloc.2
    IL_00c9:  brfalse    IL_01b1

    IL_00ce:  ldarg.0
    IL_00cf:  ldloc.0
    IL_00d0:  ldloc.2
    IL_00d1:  sub
    IL_00d2:  stfld      int32 Beetle.EofDataOfPackage::a
    IL_00d7:  ret

    IL_00d8:  ldarg.0
    IL_00d9:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_00de:  callvirt   instance int32 Beetle.BufferWriter::get_Count()
    IL_00e3:  ldarg.3
    IL_00e4:  add
    IL_00e5:  ldsfld     int32 Beetle.TcpUtils::DataPacketMaxLength
    IL_00ea:  ble.s      IL_00f2

    IL_00ec:  call       class Beetle.NetTcpException Beetle.NetTcpException::DataOverflow()
    IL_00f1:  throw

    IL_00f2:  ldarg.0
    IL_00f3:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_00f8:  ldarg.1
    IL_00f9:  ldarg.2
    IL_00fa:  ldarg.3
    IL_00fb:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0100:  ldloc.1
    IL_0101:  ldc.i4.1
    IL_0102:  add
    IL_0103:  starg.s    start
    IL_0105:  ldloc.0
    IL_0106:  ldarg.2
    IL_0107:  sub
    IL_0108:  starg.s    count
    IL_010a:  ldarg.0
    IL_010b:  ldarg.0
    IL_010c:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0111:  callvirt   instance void Beetle.Package::OnMessageDataReader(class Beetle.BufferWriter)
    IL_0116:  ldarg.0
    IL_0117:  ldc.i4.0
    IL_0118:  stfld      int32 Beetle.EofDataOfPackage::a
    IL_011d:  ldarg.0
    IL_011e:  ldc.i4.1
    IL_011f:  stfld      bool Beetle.EofDataOfPackage::b
    IL_0124:  br         IL_01aa

    IL_0129:  ldc.i4.1
    IL_012a:  stloc.s    V_6
    IL_012c:  ldarg.0
    IL_012d:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_0132:  ldlen
    IL_0133:  conv.i4
    IL_0134:  ldarg.0
    IL_0135:  ldfld      int32 Beetle.EofDataOfPackage::a
    IL_013a:  sub
    IL_013b:  stloc.s    V_7
    IL_013d:  ldarg.0
    IL_013e:  ldfld      int32 Beetle.EofDataOfPackage::a
    IL_0143:  stloc.s    V_8
    IL_0145:  br.s       IL_0166

    IL_0147:  ldarg.1
    IL_0148:  ldloc.s    V_8
    IL_014a:  ldarg.0
    IL_014b:  ldfld      int32 Beetle.EofDataOfPackage::a
    IL_0150:  sub
    IL_0151:  ldelem.u1
    IL_0152:  ldarg.0
    IL_0153:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_0158:  ldloc.s    V_8
    IL_015a:  ldelem.u1
    IL_015b:  beq.s      IL_0160

    IL_015d:  ldc.i4.0
    IL_015e:  stloc.s    V_6
    IL_0160:  ldloc.s    V_8
    IL_0162:  ldc.i4.1
    IL_0163:  add
    IL_0164:  stloc.s    V_8
    IL_0166:  ldloc.s    V_8
    IL_0168:  ldarg.0
    IL_0169:  callvirt   instance uint8[] Beetle.EofDataOfPackage::get_EofData()
    IL_016e:  ldlen
    IL_016f:  conv.i4
    IL_0170:  blt.s      IL_0147

    IL_0172:  ldloc.s    V_6
    IL_0174:  brfalse.s  IL_01a3

    IL_0176:  ldarg.0
    IL_0177:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_017c:  ldarg.1
    IL_017d:  ldarg.2
    IL_017e:  ldloc.s    V_7
    IL_0180:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                  int32,
                                                                  int32)
    IL_0185:  ldarg.2
    IL_0186:  ldloc.s    V_7
    IL_0188:  add
    IL_0189:  starg.s    start
    IL_018b:  ldloc.0
    IL_018c:  ldarg.2
    IL_018d:  sub
    IL_018e:  starg.s    count
    IL_0190:  ldarg.0
    IL_0191:  ldarg.0
    IL_0192:  ldfld      class Beetle.BufferWriter Beetle.Package::Writer
    IL_0197:  callvirt   instance void Beetle.Package::OnMessageDataReader(class Beetle.BufferWriter)
    IL_019c:  ldarg.0
    IL_019d:  ldc.i4.1
    IL_019e:  stfld      bool Beetle.EofDataOfPackage::b
    IL_01a3:  ldarg.0
    IL_01a4:  ldc.i4.0
    IL_01a5:  stfld      int32 Beetle.EofDataOfPackage::a
    IL_01aa:  ldarg.3
    IL_01ab:  ldc.i4.0
    IL_01ac:  bgt        IL_0009

    IL_01b1:  ret
  } // end of method EofDataOfPackage::Import

  .property instance uint8[] EofData()
  {
    .get instance uint8[] Beetle.EofDataOfPackage::get_EofData()
  } // end of property EofDataOfPackage::EofData
} // end of class Beetle.EofDataOfPackage

.class public auto ansi beforefieldinit Beetle.ChannelDisposedEventArgs
       extends Beetle.ChannelEventArgs
{
  .field assembly int32 a
  .field private int32 b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> c
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private bool d
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Beetle.ChannelEventArgs::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.1
    IL_0008:  call       instance void Beetle.ChannelDisposedEventArgs::set_Clean(bool)
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.s   50
    IL_0010:  call       instance void Beetle.ChannelDisposedEventArgs::set_Delay(int32)
    IL_0015:  ret
  } // end of method ChannelDisposedEventArgs::.ctor

  .method public hidebysig specialname instance int32 
          get_Delay() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Beetle.ChannelDisposedEventArgs::b
    IL_0006:  ret
  } // end of method ChannelDisposedEventArgs::get_Delay

  .method public hidebysig specialname instance void 
          set_Delay(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 Beetle.ChannelDisposedEventArgs::b
    IL_0007:  ret
  } // end of method ChannelDisposedEventArgs::set_Delay

  .method public hidebysig specialname instance class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> 
          get_Message() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.ChannelDisposedEventArgs::c
    IL_0006:  ret
  } // end of method ChannelDisposedEventArgs::get_Message

  .method assembly hidebysig specialname 
          instance void  a(class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.ChannelDisposedEventArgs::c
    IL_0007:  ret
  } // end of method ChannelDisposedEventArgs::a

  .method public hidebysig specialname instance bool 
          get_Clean() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool Beetle.ChannelDisposedEventArgs::d
    IL_0006:  ret
  } // end of method ChannelDisposedEventArgs::get_Clean

  .method public hidebysig specialname instance void 
          set_Clean(bool 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      bool Beetle.ChannelDisposedEventArgs::d
    IL_0007:  ret
  } // end of method ChannelDisposedEventArgs::set_Clean

  .property instance int32 Delay()
  {
    .get instance int32 Beetle.ChannelDisposedEventArgs::get_Delay()
    .set instance void Beetle.ChannelDisposedEventArgs::set_Delay(int32)
  } // end of property ChannelDisposedEventArgs::Delay
  .property instance class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>
          Message()
  {
    .get instance class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage> Beetle.ChannelDisposedEventArgs::get_Message()
    .set instance void Beetle.ChannelDisposedEventArgs::a(class [System]System.Collections.Generic.Queue`1<class Beetle.IMessage>)
  } // end of property ChannelDisposedEventArgs::Message
  .property instance bool Clean()
  {
    .set instance void Beetle.ChannelDisposedEventArgs::set_Clean(bool)
    .get instance bool Beetle.ChannelDisposedEventArgs::get_Clean()
  } // end of property ChannelDisposedEventArgs::Clean
} // end of class Beetle.ChannelDisposedEventArgs

.class interface public abstract auto ansi Beetle.IIPFiler
{
  .method public hidebysig newslot abstract virtual 
          instance bool  Execute(class [System]System.Net.IPEndPoint poing) cil managed
  {
  } // end of method IIPFiler::Execute

} // end of class Beetle.IIPFiler

.class interface public abstract auto ansi Beetle.IByteConverterFactory
{
  .method public hidebysig newslot abstract virtual 
          instance class Beetle.IByteConverter 
          Pop() cil managed
  {
  } // end of method IByteConverterFactory::Pop

  .method public hidebysig newslot abstract virtual 
          instance void  Init(int32 count) cil managed
  {
  } // end of method IByteConverterFactory::Init

  .method public hidebysig newslot abstract virtual 
          instance void  Push(class Beetle.IByteConverter converter) cil managed
  {
  } // end of method IByteConverterFactory::Push

  .method public hidebysig newslot abstract virtual 
          instance void  Clear() cil managed
  {
  } // end of method IByteConverterFactory::Clear

} // end of class Beetle.IByteConverterFactory

.class public auto ansi beforefieldinit Beetle.ChannelController
       extends [mscorlib]System.Object
{
  .field private static class [mscorlib]System.Collections.Hashtable a
  .field public class [Smark.Core]Smark.Core.AsyncDelegate`2<class Beetle.TcpChannel,object> Receive
  .method public hidebysig instance void 
          Register(class Beetle.Package package) cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldftn      instance void Beetle.ChannelController::a(class Beetle.PacketRecieveMessagerArgs)
    IL_0008:  newobj     instance void Beetle.EventPacketRecievMessage::.ctor(object,
                                                                              native int)
    IL_000d:  stfld      class Beetle.EventPacketRecievMessage Beetle.Package::ReceiveMessage
    IL_0012:  ret
  } // end of method ChannelController::Register

  .method private hidebysig instance void 
          a(class Beetle.PacketRecieveMessagerArgs A_0) cil managed
  {
    // 代码大小       131 (0x83)
    .maxstack  3
    .locals init (object V_0,
             class [mscorlib]System.Type V_1,
             class i V_2)
    IL_0000:  ldarg.1
    IL_0001:  ldfld      object Beetle.PacketRecieveMessagerArgs::Message
    IL_0006:  stloc.0
    IL_0007:  ldsfld     class [mscorlib]System.Collections.Hashtable Beetle.ChannelController::a
    IL_000c:  callvirt   instance int32 [mscorlib]System.Collections.Hashtable::get_Count()
    IL_0011:  brtrue.s   IL_002e

    IL_0013:  ldarg.0
    IL_0014:  ldfld      class [Smark.Core]Smark.Core.AsyncDelegate`2<class Beetle.TcpChannel,object> Beetle.ChannelController::Receive
    IL_0019:  brfalse.s  IL_0082

    IL_001b:  ldarg.0
    IL_001c:  ldfld      class [Smark.Core]Smark.Core.AsyncDelegate`2<class Beetle.TcpChannel,object> Beetle.ChannelController::Receive
    IL_0021:  ldarg.1
    IL_0022:  ldfld      class Beetle.TcpChannel Beetle.PacketRecieveMessagerArgs::Channel
    IL_0027:  ldloc.0
    IL_0028:  callvirt   instance void class [Smark.Core]Smark.Core.AsyncDelegate`2<class Beetle.TcpChannel,object>::Invoke(!0,
                                                                                                                            !1)
    IL_002d:  ret

    IL_002e:  ldloc.0
    IL_002f:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
    IL_0034:  stloc.1
    IL_0035:  ldsfld     class [mscorlib]System.Collections.Hashtable Beetle.ChannelController::a
    IL_003a:  ldloc.1
    IL_003b:  callvirt   instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
    IL_0040:  castclass  i
    IL_0045:  stloc.2
    IL_0046:  ldloc.2
    IL_0047:  brfalse.s  IL_0068

    IL_0049:  ldloc.2
    IL_004a:  ldarg.1
    IL_004b:  ldfld      class Beetle.TcpChannel Beetle.PacketRecieveMessagerArgs::Channel
    IL_0050:  ldloc.0
    IL_0051:  callvirt   instance void i::d(class Beetle.TcpChannel,
                                            object)
    IL_0056:  ldarg.1
    IL_0057:  ldloc.2
    IL_0058:  ldfld      class Beetle.MessageHandlerArgs i::e
    IL_005d:  ldfld      bool Beetle.MessageHandlerArgs::CleanMessage
    IL_0062:  stfld      bool Beetle.PacketRecieveMessagerArgs::CleanMessage
    IL_0067:  ret

    IL_0068:  ldarg.0
    IL_0069:  ldfld      class [Smark.Core]Smark.Core.AsyncDelegate`2<class Beetle.TcpChannel,object> Beetle.ChannelController::Receive
    IL_006e:  brfalse.s  IL_0082

    IL_0070:  ldarg.0
    IL_0071:  ldfld      class [Smark.Core]Smark.Core.AsyncDelegate`2<class Beetle.TcpChannel,object> Beetle.ChannelController::Receive
    IL_0076:  ldarg.1
    IL_0077:  ldfld      class Beetle.TcpChannel Beetle.PacketRecieveMessagerArgs::Channel
    IL_007c:  ldloc.0
    IL_007d:  callvirt   instance void class [Smark.Core]Smark.Core.AsyncDelegate`2<class Beetle.TcpChannel,object>::Invoke(!0,
                                                                                                                            !1)
    IL_0082:  ret
  } // end of method ChannelController::a

  .method public hidebysig static void  RegisterHandler(class Beetle.IMessageHandler 'handler') cil managed
  {
    // 代码大小       120 (0x78)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.IList`1<class i> V_0,
             int32 V_1,
             class i V_2,
             class [mscorlib]System.Collections.Hashtable V_3)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0077

    IL_0003:  ldsfld     class [mscorlib]System.Collections.Hashtable Beetle.ChannelController::a
    IL_0008:  dup
    IL_0009:  stloc.3
    IL_000a:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000f:  ldarg.0
      IL_0010:  call       class [mscorlib]System.Collections.Generic.IList`1<class i> i::d(class Beetle.IMessageHandler)
      IL_0015:  stloc.0
      IL_0016:  ldloc.0
      IL_0017:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class i>::get_Count()
      IL_001c:  ldc.i4.0
      IL_001d:  ble.s      IL_006e

      IL_001f:  ldc.i4.0
      IL_0020:  stloc.1
      IL_0021:  br.s       IL_0065

      IL_0023:  ldloc.0
      IL_0024:  ldloc.1
      IL_0025:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class i>::get_Item(int32)
      IL_002a:  stloc.2
      IL_002b:  ldsfld     class [mscorlib]System.Collections.Hashtable Beetle.ChannelController::a
      IL_0030:  ldloc.2
      IL_0031:  ldfld      class [mscorlib]System.Type i::c
      IL_0036:  callvirt   instance bool [mscorlib]System.Collections.Hashtable::ContainsKey(object)
      IL_003b:  brtrue.s   IL_0050

      IL_003d:  ldsfld     class [mscorlib]System.Collections.Hashtable Beetle.ChannelController::a
      IL_0042:  ldloc.2
      IL_0043:  ldfld      class [mscorlib]System.Type i::c
      IL_0048:  ldloc.2
      IL_0049:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                     object)
      IL_004e:  br.s       IL_0061

      IL_0050:  ldsfld     class [mscorlib]System.Collections.Hashtable Beetle.ChannelController::a
      IL_0055:  ldloc.2
      IL_0056:  ldfld      class [mscorlib]System.Type i::c
      IL_005b:  ldloc.2
      IL_005c:  callvirt   instance void [mscorlib]System.Collections.Hashtable::set_Item(object,
                                                                                          object)
      IL_0061:  ldloc.1
      IL_0062:  ldc.i4.1
      IL_0063:  add
      IL_0064:  stloc.1
      IL_0065:  ldloc.1
      IL_0066:  ldloc.0
      IL_0067:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class i>::get_Count()
      IL_006c:  blt.s      IL_0023

      IL_006e:  leave.s    IL_0077

    }  // end .try
    finally
    {
      IL_0070:  ldloc.3
      IL_0071:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0076:  endfinally
    }  // end handler
    IL_0077:  ret
  } // end of method ChannelController::RegisterHandler

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ChannelController::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldc.i4.s   32
    IL_0002:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32)
    IL_0007:  stsfld     class [mscorlib]System.Collections.Hashtable Beetle.ChannelController::a
    IL_000c:  ret
  } // end of method ChannelController::.cctor

} // end of class Beetle.ChannelController

.class public auto ansi sealed Beetle.ChannelStatus
       extends [mscorlib]System.Enum
{
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype Beetle.ChannelStatus None = int32(0x00000000)
  .field public static literal valuetype Beetle.ChannelStatus Security = int32(0x00000001)
} // end of class Beetle.ChannelStatus

.class public auto ansi Beetle.TcpUtils
       extends [mscorlib]System.Object
{
  .field static assembly bool a
  .field public static class Beetle.ChannelController DefaultController
  .field static assembly class l b
  .field static assembly object c
  .field static assembly object d
  .field static assembly class Beetle.ObjectPool`1<class ai> e
  .field static assembly class h f
  .field public static class Beetle.ObjectPool`1<class Beetle.BufferReader> BufferReaderPool
  .field private static class [mscorlib]System.Collections.Generic.IList`1<class ag> g
  .field private static class [mscorlib]System.Collections.Generic.IList`1<class ag> h
  .field private static class [mscorlib]System.Collections.Generic.IList`1<class ag> i
  .field public static class Beetle.IByteConverterFactory ByteConverters
  .field public static int32 SocketDespatchSleepTime
  .field public static int32 WorkDespatchSleepTime
  .field public static bool Statistics
  .field public static int64 SendBytes
  .field public static int64 ReceiveBytes
  .field public static int64 SendDataIO
  .field public static int64 ReceiveDataIO
  .field public static bool UseReceiveAsyncQueue
  .field public static int32 ChannelMaxQueueData
  .field public static int32 DataPacketMaxLength
  .field public static int32 SendPacketSpacing
  .field public static int32 SendMessageSpacing
  .field public static int32 SendBufferLength
  .field public static int32 ReceiveBufferLength
  .field public static int32 Connections
  .field public static int32 StringEncodingLength
  .field public static bool UseStringEncoding
  .field public static int32 TimeOutms
  .field public static bool ExecutionContext
  .field public static int32 BufferPoolSize
  .field static assembly object j
  .field public static int64 ReceiveMessages
  .field static assembly object k
  .field public static int64 SendMessages
  .field static assembly int32 l
  .field private static int32 m
  .field private static int32 n
  .field private static int32 o
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       252 (0xfc)
    .maxstack  1
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     bool Beetle.TcpUtils::a
    IL_0006:  newobj     instance void Beetle.ChannelController::.ctor()
    IL_000b:  stsfld     class Beetle.ChannelController Beetle.TcpUtils::DefaultController
    IL_0010:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0015:  stsfld     object Beetle.TcpUtils::c
    IL_001a:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_001f:  stsfld     object Beetle.TcpUtils::d
    IL_0024:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ag>::.ctor()
    IL_0029:  stsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::g
    IL_002e:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ag>::.ctor()
    IL_0033:  stsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::h
    IL_0038:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ag>::.ctor()
    IL_003d:  stsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
    IL_0042:  newobj     instance void y::.ctor()
    IL_0047:  stsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_004c:  ldc.i4.s   10
    IL_004e:  stsfld     int32 Beetle.TcpUtils::SocketDespatchSleepTime
    IL_0053:  ldc.i4.s   10
    IL_0055:  stsfld     int32 Beetle.TcpUtils::WorkDespatchSleepTime
    IL_005a:  ldc.i4.0
    IL_005b:  stsfld     int32 Beetle.TcpUtils::ChannelMaxQueueData
    IL_0060:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0065:  stsfld     object Beetle.TcpUtils::j
    IL_006a:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_006f:  stsfld     object Beetle.TcpUtils::k
    IL_0074:  ldc.i4.0
    IL_0075:  stsfld     int32 Beetle.TcpUtils::m
    IL_007a:  ldc.i4.0
    IL_007b:  stsfld     int32 Beetle.TcpUtils::n
    IL_0080:  ldc.i4.0
    IL_0081:  stsfld     int32 Beetle.TcpUtils::o
    IL_0086:  ldc.i4     0x200c
    IL_008b:  stsfld     int32 Beetle.TcpUtils::DataPacketMaxLength
    IL_0090:  ldc.i4     0x800
    IL_0095:  stsfld     int32 Beetle.TcpUtils::SendBufferLength
    IL_009a:  ldc.i4     0x800
    IL_009f:  stsfld     int32 Beetle.TcpUtils::ReceiveBufferLength
    IL_00a4:  ldc.i4.1
    IL_00a5:  stsfld     int32 Beetle.TcpUtils::l
    IL_00aa:  ldc.i4     0x3e8
    IL_00af:  stsfld     int32 Beetle.TcpUtils::Connections
    IL_00b4:  ldc.i4.1
    IL_00b5:  stsfld     bool Beetle.TcpUtils::UseReceiveAsyncQueue
    IL_00ba:  ldc.i4.0
    IL_00bb:  stsfld     bool Beetle.TcpUtils::Statistics
    IL_00c0:  ldc.i4.0
    IL_00c1:  stsfld     int32 Beetle.TcpUtils::SendPacketSpacing
    IL_00c6:  ldc.i4.0
    IL_00c7:  stsfld     int32 Beetle.TcpUtils::SendMessageSpacing
    IL_00cc:  ldc.i4     0x200
    IL_00d1:  stsfld     int32 Beetle.TcpUtils::StringEncodingLength
    IL_00d6:  ldc.i4.1
    IL_00d7:  stsfld     bool Beetle.TcpUtils::UseStringEncoding
    IL_00dc:  ldc.i4     0x3e8
    IL_00e1:  stsfld     int32 Beetle.TcpUtils::BufferPoolSize
    IL_00e6:  ldc.i4.0
    IL_00e7:  stsfld     bool Beetle.TcpUtils::ExecutionContext
    IL_00ec:  ldc.i4     0x7530
    IL_00f1:  stsfld     int32 Beetle.TcpUtils::TimeOutms
    IL_00f6:  call       void Beetle.TcpUtils::f()
    IL_00fb:  ret
  } // end of method TcpUtils::.cctor

  .method private hidebysig static void  f() cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TcpUtils::f

  .method public hidebysig static int32  GetSendBufferStatus() cil managed
  {
    // 代码大小       20 (0x14)
    .maxstack  8
    IL_0000:  ldsfld     class l Beetle.TcpUtils::b
    IL_0005:  brtrue.s   IL_0009

    IL_0007:  ldc.i4.0
    IL_0008:  ret

    IL_0009:  ldsfld     class l Beetle.TcpUtils::b
    IL_000e:  callvirt   instance int32 l::f()
    IL_0013:  ret
  } // end of method TcpUtils::GetSendBufferStatus

  .method public hidebysig static void  Setup(string configname) cil managed
  {
    // 代码大小       168 (0xa8)
    .maxstack  5
    .locals init (class Beetle.ConfigSelection V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       object [System.Configuration]System.Configuration.ConfigurationManager::GetSection(string)
    IL_0006:  castclass  Beetle.ConfigSelection
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_0016

    IL_000f:  ldarg.0
    IL_0010:  call       class Beetle.NetTcpException Beetle.NetTcpException::ConfigSelectionNotFound(string)
    IL_0015:  throw

    IL_0016:  ldloc.0
    IL_0017:  callvirt   instance bool Beetle.ConfigSelection::get_Statistics()
    IL_001c:  stsfld     bool Beetle.TcpUtils::Statistics
    IL_0021:  ldloc.0
    IL_0022:  callvirt   instance int32 Beetle.ConfigSelection::get_SocketThreadSleep()
    IL_0027:  stsfld     int32 Beetle.TcpUtils::SocketDespatchSleepTime
    IL_002c:  ldloc.0
    IL_002d:  callvirt   instance int32 Beetle.ConfigSelection::get_WorkThreadSleep()
    IL_0032:  stsfld     int32 Beetle.TcpUtils::WorkDespatchSleepTime
    IL_0037:  ldloc.0
    IL_0038:  callvirt   instance int32 Beetle.ConfigSelection::get_StringEncodingSize()
    IL_003d:  stsfld     int32 Beetle.TcpUtils::StringEncodingLength
    IL_0042:  ldloc.0
    IL_0043:  callvirt   instance int32 Beetle.ConfigSelection::get_ReceiveBufferSize()
    IL_0048:  stsfld     int32 Beetle.TcpUtils::ReceiveBufferLength
    IL_004d:  ldloc.0
    IL_004e:  callvirt   instance int32 Beetle.ConfigSelection::get_SendBufferSize()
    IL_0053:  stsfld     int32 Beetle.TcpUtils::SendBufferLength
    IL_0058:  ldloc.0
    IL_0059:  callvirt   instance int32 Beetle.ConfigSelection::get_PackageMaxSize()
    IL_005e:  stsfld     int32 Beetle.TcpUtils::DataPacketMaxLength
    IL_0063:  ldloc.0
    IL_0064:  callvirt   instance int32 Beetle.ConfigSelection::get_TimeOut()
    IL_0069:  stsfld     int32 Beetle.TcpUtils::TimeOutms
    IL_006e:  ldloc.0
    IL_006f:  callvirt   instance int32 Beetle.ConfigSelection::get_ChannelMaxQueueData()
    IL_0074:  stsfld     int32 Beetle.TcpUtils::ChannelMaxQueueData
    IL_0079:  ldloc.0
    IL_007a:  callvirt   instance bool Beetle.ConfigSelection::get_ExecutionContext()
    IL_007f:  stsfld     bool Beetle.TcpUtils::ExecutionContext
    IL_0084:  ldloc.0
    IL_0085:  callvirt   instance int32 Beetle.ConfigSelection::get_Connections()
    IL_008a:  ldloc.0
    IL_008b:  callvirt   instance int32 Beetle.ConfigSelection::get_Pools()
    IL_0090:  ldloc.0
    IL_0091:  callvirt   instance int32 Beetle.ConfigSelection::get_SocketSendThreads()
    IL_0096:  ldloc.0
    IL_0097:  callvirt   instance int32 Beetle.ConfigSelection::get_SocketReceiveThreads()
    IL_009c:  ldloc.0
    IL_009d:  callvirt   instance int32 Beetle.ConfigSelection::get_WorkThreads()
    IL_00a2:  call       void Beetle.TcpUtils::Setup(int32,
                                                     int32,
                                                     int32,
                                                     int32,
                                                     int32)
    IL_00a7:  ret
  } // end of method TcpUtils::Setup

  .method public hidebysig static void  Setup(int32 maxconnections,
                                              int32 bufferpools,
                                              int32 senddespatchs,
                                              int32 receivedespatch,
                                              int32 workdespatchs) cil managed
  {
    // 代码大小       235 (0xeb)
    .maxstack  4
    .locals init (class ag V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  call       void Beetle.TcpUtils::Clean()
    IL_0005:  ldarg.0
    IL_0006:  stsfld     int32 Beetle.TcpUtils::Connections
    IL_000b:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_0010:  ldsfld     int32 Beetle.TcpUtils::Connections
    IL_0015:  callvirt   instance void Beetle.IByteConverterFactory::Init(int32)
    IL_001a:  ldsfld     int32 Beetle.TcpUtils::Connections
    IL_001f:  ldsfld     int32 Beetle.TcpUtils::ReceiveBufferLength
    IL_0024:  newobj     instance void h::.ctor(int32,
                                                int32)
    IL_0029:  stsfld     class h Beetle.TcpUtils::f
    IL_002e:  ldsfld     int32 Beetle.TcpUtils::Connections
    IL_0033:  newobj     instance void class Beetle.ObjectPool`1<class ai>::.ctor(int32)
    IL_0038:  stsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_003d:  ldsfld     int32 Beetle.TcpUtils::Connections
    IL_0042:  ldc.i4.2
    IL_0043:  div
    IL_0044:  newobj     instance void class Beetle.ObjectPool`1<class Beetle.BufferReader>::.ctor(int32)
    IL_0049:  stsfld     class Beetle.ObjectPool`1<class Beetle.BufferReader> Beetle.TcpUtils::BufferReaderPool
    IL_004e:  ldarg.1
    IL_004f:  call       void Beetle.TcpUtils::a(int32)
    IL_0054:  ldsfld     int32 Beetle.TcpUtils::Connections
    IL_0059:  ldc.i4.2
    IL_005a:  div
    IL_005b:  ldsfld     int32 Beetle.TcpUtils::SendBufferLength
    IL_0060:  ldc.i4.1
    IL_0061:  newobj     instance void l::.ctor(int32,
                                                int32,
                                                bool)
    IL_0066:  stsfld     class l Beetle.TcpUtils::b
    IL_006b:  ldc.i4.0
    IL_006c:  stloc.1
    IL_006d:  br.s       IL_008f

    IL_006f:  newobj     instance void ag::.ctor()
    IL_0074:  stloc.0
    IL_0075:  ldloc.0
    IL_0076:  ldsfld     int32 Beetle.TcpUtils::SocketDespatchSleepTime
    IL_007b:  stfld      int32 ag::c
    IL_0080:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::g
    IL_0085:  ldloc.0
    IL_0086:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::Add(!0)
    IL_008b:  ldloc.1
    IL_008c:  ldc.i4.1
    IL_008d:  add
    IL_008e:  stloc.1
    IL_008f:  ldloc.1
    IL_0090:  ldarg.2
    IL_0091:  blt.s      IL_006f

    IL_0093:  ldc.i4.0
    IL_0094:  stloc.2
    IL_0095:  br.s       IL_00b7

    IL_0097:  newobj     instance void ag::.ctor()
    IL_009c:  stloc.0
    IL_009d:  ldloc.0
    IL_009e:  ldsfld     int32 Beetle.TcpUtils::SocketDespatchSleepTime
    IL_00a3:  stfld      int32 ag::c
    IL_00a8:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::h
    IL_00ad:  ldloc.0
    IL_00ae:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::Add(!0)
    IL_00b3:  ldloc.2
    IL_00b4:  ldc.i4.1
    IL_00b5:  add
    IL_00b6:  stloc.2
    IL_00b7:  ldloc.2
    IL_00b8:  ldarg.3
    IL_00b9:  blt.s      IL_0097

    IL_00bb:  ldc.i4.0
    IL_00bc:  stloc.3
    IL_00bd:  br.s       IL_00df

    IL_00bf:  newobj     instance void ag::.ctor()
    IL_00c4:  stloc.0
    IL_00c5:  ldloc.0
    IL_00c6:  ldsfld     int32 Beetle.TcpUtils::WorkDespatchSleepTime
    IL_00cb:  stfld      int32 ag::c
    IL_00d0:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
    IL_00d5:  ldloc.0
    IL_00d6:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::Add(!0)
    IL_00db:  ldloc.3
    IL_00dc:  ldc.i4.1
    IL_00dd:  add
    IL_00de:  stloc.3
    IL_00df:  ldloc.3
    IL_00e0:  ldarg.s    workdespatchs
    IL_00e2:  blt.s      IL_00bf

    IL_00e4:  ldc.i4.1
    IL_00e5:  stsfld     bool Beetle.TcpUtils::a
    IL_00ea:  ret
  } // end of method TcpUtils::Setup

  .method private hidebysig static void  a(int32 A_0) cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  call       void l::d()
    IL_0005:  ldarg.0
    IL_0006:  ldsfld     int32 Beetle.TcpUtils::BufferPoolSize
    IL_000b:  call       void l::a(int32,
                                   int32)
    IL_0010:  ret
  } // end of method TcpUtils::a

  .method public hidebysig static void  Setup(int32 maxconnections,
                                              int32 bufferpools,
                                              int32 despatchs) cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.2
    IL_0004:  ldarg.2
    IL_0005:  call       void Beetle.TcpUtils::Setup(int32,
                                                     int32,
                                                     int32,
                                                     int32,
                                                     int32)
    IL_000a:  ret
  } // end of method TcpUtils::Setup

  .method public hidebysig static void  Clean() cil managed
  {
    // 代码大小       133 (0x85)
    .maxstack  1
    IL_0000:  ldsfld     class Beetle.ObjectPool`1<class Beetle.BufferReader> Beetle.TcpUtils::BufferReaderPool
    IL_0005:  brfalse.s  IL_0017

    IL_0007:  ldsfld     class Beetle.ObjectPool`1<class Beetle.BufferReader> Beetle.TcpUtils::BufferReaderPool
    IL_000c:  callvirt   instance void class Beetle.ObjectPool`1<class Beetle.BufferReader>::Dispose()
    IL_0011:  ldnull
    IL_0012:  stsfld     class Beetle.ObjectPool`1<class Beetle.BufferReader> Beetle.TcpUtils::BufferReaderPool
    IL_0017:  ldsfld     class h Beetle.TcpUtils::f
    IL_001c:  brfalse.s  IL_0028

    IL_001e:  ldsfld     class h Beetle.TcpUtils::f
    IL_0023:  callvirt   instance void h::Dispose()
    IL_0028:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_002d:  brfalse.s  IL_0039

    IL_002f:  ldsfld     class Beetle.IByteConverterFactory Beetle.TcpUtils::ByteConverters
    IL_0034:  callvirt   instance void Beetle.IByteConverterFactory::Clear()
    IL_0039:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_003e:  brfalse.s  IL_004a

    IL_0040:  ldsfld     class Beetle.ObjectPool`1<class ai> Beetle.TcpUtils::e
    IL_0045:  callvirt   instance void class Beetle.ObjectPool`1<class ai>::Dispose()
    IL_004a:  call       void Beetle.TcpUtils::e()
    IL_004f:  ldsfld     class l Beetle.TcpUtils::b
    IL_0054:  brfalse.s  IL_0066

    IL_0056:  ldsfld     class l Beetle.TcpUtils::b
    IL_005b:  callvirt   instance void l::Dispose()
    IL_0060:  ldnull
    IL_0061:  stsfld     class l Beetle.TcpUtils::b
    IL_0066:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::h
    IL_006b:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::Clear()
    IL_0070:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::g
    IL_0075:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::Clear()
    IL_007a:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
    IL_007f:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::Clear()
    IL_0084:  ret
  } // end of method TcpUtils::Clean

  .method private hidebysig static void  e() cil managed
  {
    // 代码大小       149 (0x95)
    .maxstack  1
    .locals init (class ag V_0,
             class ag V_1,
             class ag V_2,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag> V_3,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag> V_4,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag> V_5)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::g
    IL_0005:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class ag>::GetEnumerator()
    IL_000a:  stloc.3
    .try
    {
      IL_000b:  br.s       IL_001a

      IL_000d:  ldloc.3
      IL_000e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag>::get_Current()
      IL_0013:  stloc.0
      IL_0014:  ldloc.0
      IL_0015:  callvirt   instance void ag::Dispose()
      IL_001a:  ldloc.3
      IL_001b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0020:  brtrue.s   IL_000d

      IL_0022:  leave.s    IL_002e

    }  // end .try
    finally
    {
      IL_0024:  ldloc.3
      IL_0025:  brfalse.s  IL_002d

      IL_0027:  ldloc.3
      IL_0028:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002d:  endfinally
    }  // end handler
    IL_002e:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::h
    IL_0033:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class ag>::GetEnumerator()
    IL_0038:  stloc.s    V_4
    .try
    {
      IL_003a:  br.s       IL_004a

      IL_003c:  ldloc.s    V_4
      IL_003e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag>::get_Current()
      IL_0043:  stloc.1
      IL_0044:  ldloc.1
      IL_0045:  callvirt   instance void ag::Dispose()
      IL_004a:  ldloc.s    V_4
      IL_004c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0051:  brtrue.s   IL_003c

      IL_0053:  leave.s    IL_0061

    }  // end .try
    finally
    {
      IL_0055:  ldloc.s    V_4
      IL_0057:  brfalse.s  IL_0060

      IL_0059:  ldloc.s    V_4
      IL_005b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0060:  endfinally
    }  // end handler
    IL_0061:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
    IL_0066:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class ag>::GetEnumerator()
    IL_006b:  stloc.s    V_5
    .try
    {
      IL_006d:  br.s       IL_007d

      IL_006f:  ldloc.s    V_5
      IL_0071:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag>::get_Current()
      IL_0076:  stloc.2
      IL_0077:  ldloc.2
      IL_0078:  callvirt   instance void ag::Dispose()
      IL_007d:  ldloc.s    V_5
      IL_007f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0084:  brtrue.s   IL_006f

      IL_0086:  leave.s    IL_0094

    }  // end .try
    finally
    {
      IL_0088:  ldloc.s    V_5
      IL_008a:  brfalse.s  IL_0093

      IL_008c:  ldloc.s    V_5
      IL_008e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0093:  endfinally
    }  // end handler
    IL_0094:  ret
  } // end of method TcpUtils::e

  .method assembly hidebysig static class ag 
          d() cil managed
  {
    // 代码大小       74 (0x4a)
    .maxstack  2
    .locals init (class ag V_0,
             class [mscorlib]System.Collections.Generic.IList`1<class ag> V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::g
    IL_0005:  dup
    IL_0006:  stloc.1
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldsfld     int32 Beetle.TcpUtils::m
      IL_0011:  ldc.i4.1
      IL_0012:  add
      IL_0013:  stsfld     int32 Beetle.TcpUtils::m
      IL_0018:  ldsfld     int32 Beetle.TcpUtils::m
      IL_001d:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::g
      IL_0022:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::get_Count()
      IL_0027:  blt.s      IL_002f

      IL_0029:  ldc.i4.0
      IL_002a:  stsfld     int32 Beetle.TcpUtils::m
      IL_002f:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::g
      IL_0034:  ldsfld     int32 Beetle.TcpUtils::m
      IL_0039:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class ag>::get_Item(int32)
      IL_003e:  stloc.0
      IL_003f:  leave.s    IL_0048

    }  // end .try
    finally
    {
      IL_0041:  ldloc.1
      IL_0042:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0047:  endfinally
    }  // end handler
    IL_0048:  ldloc.0
    IL_0049:  ret
  } // end of method TcpUtils::d

  .method assembly hidebysig static class ag 
          c() cil managed
  {
    // 代码大小       74 (0x4a)
    .maxstack  2
    .locals init (class ag V_0,
             class [mscorlib]System.Collections.Generic.IList`1<class ag> V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::h
    IL_0005:  dup
    IL_0006:  stloc.1
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldsfld     int32 Beetle.TcpUtils::n
      IL_0011:  ldc.i4.1
      IL_0012:  add
      IL_0013:  stsfld     int32 Beetle.TcpUtils::n
      IL_0018:  ldsfld     int32 Beetle.TcpUtils::n
      IL_001d:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::h
      IL_0022:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::get_Count()
      IL_0027:  blt.s      IL_002f

      IL_0029:  ldc.i4.0
      IL_002a:  stsfld     int32 Beetle.TcpUtils::n
      IL_002f:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::h
      IL_0034:  ldsfld     int32 Beetle.TcpUtils::n
      IL_0039:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class ag>::get_Item(int32)
      IL_003e:  stloc.0
      IL_003f:  leave.s    IL_0048

    }  // end .try
    finally
    {
      IL_0041:  ldloc.1
      IL_0042:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0047:  endfinally
    }  // end handler
    IL_0048:  ldloc.0
    IL_0049:  ret
  } // end of method TcpUtils::c

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IList`1<int32> 
          GetBufferPoolStatus() cil managed
  {
    // 代码大小       6 (0x6)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Collections.Generic.IList`1<int32> l::c()
    IL_0005:  ret
  } // end of method TcpUtils::GetBufferPoolStatus

  .method public hidebysig static int32  GetAsyncEventPoolStatus() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldsfld     class h Beetle.TcpUtils::f
    IL_0005:  callvirt   instance int32 h::b()
    IL_000a:  ret
  } // end of method TcpUtils::GetAsyncEventPoolStatus

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IList`1<int32> 
          GetSendDespatchStatus() cil managed
  {
    // 代码大小       60 (0x3c)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0,
             class ag V_1,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag> V_2)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::g
    IL_000b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class ag>::GetEnumerator()
    IL_0010:  stloc.2
    .try
    {
      IL_0011:  br.s       IL_0026

      IL_0013:  ldloc.2
      IL_0014:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag>::get_Current()
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance int32 ag::d()
      IL_0021:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
      IL_0026:  ldloc.2
      IL_0027:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_002c:  brtrue.s   IL_0013

      IL_002e:  leave.s    IL_003a

    }  // end .try
    finally
    {
      IL_0030:  ldloc.2
      IL_0031:  brfalse.s  IL_0039

      IL_0033:  ldloc.2
      IL_0034:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0039:  endfinally
    }  // end handler
    IL_003a:  ldloc.0
    IL_003b:  ret
  } // end of method TcpUtils::GetSendDespatchStatus

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IList`1<int32> 
          GetReceiveDespatchStatus() cil managed
  {
    // 代码大小       60 (0x3c)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0,
             class ag V_1,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag> V_2)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::h
    IL_000b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class ag>::GetEnumerator()
    IL_0010:  stloc.2
    .try
    {
      IL_0011:  br.s       IL_0026

      IL_0013:  ldloc.2
      IL_0014:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class ag>::get_Current()
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance int32 ag::d()
      IL_0021:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
      IL_0026:  ldloc.2
      IL_0027:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_002c:  brtrue.s   IL_0013

      IL_002e:  leave.s    IL_003a

    }  // end .try
    finally
    {
      IL_0030:  ldloc.2
      IL_0031:  brfalse.s  IL_0039

      IL_0033:  ldloc.2
      IL_0034:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0039:  endfinally
    }  // end handler
    IL_003a:  ldloc.0
    IL_003b:  ret
  } // end of method TcpUtils::GetReceiveDespatchStatus

  .method public hidebysig static void  Setup(int32 maxonlineconnections,
                                              int32 despatchs) cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  conv.r8
    IL_0003:  ldc.r8     1000.
    IL_000c:  div
    IL_000d:  call       float64 [mscorlib]System.Math::Ceiling(float64)
    IL_0012:  call       int32 [mscorlib]System.Convert::ToInt32(float64)
    IL_0017:  ldc.i4.2
    IL_0018:  mul
    IL_0019:  ldarg.1
    IL_001a:  call       void Beetle.TcpUtils::Setup(int32,
                                                     int32,
                                                     int32)
    IL_001f:  ret
  } // end of method TcpUtils::Setup

  .method public hidebysig static void  Setup(int32 maxonlineconnections) cil managed
  {
    // 代码大小       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  call       void Beetle.TcpUtils::Setup(int32,
                                                     int32,
                                                     int32)
    IL_0008:  ret
  } // end of method TcpUtils::Setup

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IList`1<int32> 
          GetWorkDespatchsStatus() cil managed
  {
    // 代码大小       58 (0x3a)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.IList`1<int32> V_0,
             int32 V_1)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
    IL_000b:  brfalse.s  IL_0038

    IL_000d:  ldc.i4.0
    IL_000e:  stloc.1
    IL_000f:  br.s       IL_002b

    IL_0011:  ldloc.0
    IL_0012:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
    IL_0017:  ldloc.1
    IL_0018:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class ag>::get_Item(int32)
    IL_001d:  callvirt   instance int32 ag::d()
    IL_0022:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::get_Count()
    IL_0036:  blt.s      IL_0011

    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method TcpUtils::GetWorkDespatchsStatus

  .method assembly hidebysig static class ag 
          b() cil managed
  {
    // 代码大小       100 (0x64)
    .maxstack  2
    .locals init (class ag V_0,
             class [mscorlib]System.Collections.Generic.IList`1<class ag> V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
    IL_0005:  dup
    IL_0006:  stloc.1
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
      IL_0011:  brfalse.s  IL_0020

      IL_0013:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
      IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::get_Count()
      IL_001d:  ldc.i4.0
      IL_001e:  ble.s      IL_0026

      IL_0020:  call       class Beetle.NetTcpException Beetle.NetTcpException::NotInitialize()
      IL_0025:  pop
      IL_0026:  ldsfld     int32 Beetle.TcpUtils::o
      IL_002b:  ldc.i4.1
      IL_002c:  add
      IL_002d:  stsfld     int32 Beetle.TcpUtils::o
      IL_0032:  ldsfld     int32 Beetle.TcpUtils::o
      IL_0037:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
      IL_003c:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.ICollection`1<class ag>::get_Count()
      IL_0041:  bne.un.s   IL_0049

      IL_0043:  ldc.i4.0
      IL_0044:  stsfld     int32 Beetle.TcpUtils::o
      IL_0049:  ldsfld     class [mscorlib]System.Collections.Generic.IList`1<class ag> Beetle.TcpUtils::i
      IL_004e:  ldsfld     int32 Beetle.TcpUtils::o
      IL_0053:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IList`1<class ag>::get_Item(int32)
      IL_0058:  stloc.0
      IL_0059:  leave.s    IL_0062

    }  // end .try
    finally
    {
      IL_005b:  ldloc.1
      IL_005c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0061:  endfinally
    }  // end handler
    IL_0062:  ldloc.0
    IL_0063:  ret
  } // end of method TcpUtils::b

  .method assembly hidebysig static object 
          a() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldc.i4     0x800
    IL_0005:  newobj     instance void [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::.ctor(int32)
    IL_000a:  ret
  } // end of method TcpUtils::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TcpUtils::.ctor

} // end of class Beetle.TcpUtils

.class public auto ansi beforefieldinit Beetle.DefaultIPFilter
       extends [mscorlib]System.Object
       implements Beetle.IIPFiler
{
  .field private class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> a
  .method public hidebysig instance void 
          Add(class Beetle.IpItem item) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem>::Add(!0)
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method DefaultIPFilter::Add

  .method public hidebysig instance void 
          Remove(string ipvalue,
                 bool permit) cil managed
  {
    // 代码大小       161 (0xa1)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IpItem> V_0,
             class Beetle.IpItem V_1,
             class Beetle.IpItem V_2,
             class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> V_3,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Beetle.IpItem> V_4,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<class Beetle.IpItem> V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
    IL_0006:  dup
    IL_0007:  stloc.3
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem>::.ctor()
      IL_0012:  stloc.0
      IL_0013:  ldarg.0
      IL_0014:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
      IL_0019:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem>::GetEnumerator()
      IL_001e:  stloc.s    V_4
      .try
      {
        IL_0020:  br.s       IL_0048

        IL_0022:  ldloca.s   V_4
        IL_0024:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Beetle.IpItem>::get_Current()
        IL_0029:  stloc.1
        IL_002a:  ldloc.1
        IL_002b:  callvirt   instance string Beetle.IpItem::get_AddressValue()
        IL_0030:  ldarg.1
        IL_0031:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                       string)
        IL_0036:  brfalse.s  IL_0048

        IL_0038:  ldloc.1
        IL_0039:  callvirt   instance bool Beetle.IpItem::get_Permit()
        IL_003e:  ldarg.2
        IL_003f:  bne.un.s   IL_0048

        IL_0041:  ldloc.0
        IL_0042:  ldloc.1
        IL_0043:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<class Beetle.IpItem>::Add(!0)
        IL_0048:  ldloca.s   V_4
        IL_004a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Beetle.IpItem>::MoveNext()
        IL_004f:  brtrue.s   IL_0022

        IL_0051:  leave.s    IL_0061

      }  // end .try
      finally
      {
        IL_0053:  ldloca.s   V_4
        IL_0055:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class Beetle.IpItem>
        IL_005b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0060:  endfinally
      }  // end handler
      IL_0061:  ldloc.0
      IL_0062:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class Beetle.IpItem>::GetEnumerator()
      IL_0067:  stloc.s    V_5
      .try
      {
        IL_0069:  br.s       IL_0080

        IL_006b:  ldloc.s    V_5
        IL_006d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class Beetle.IpItem>::get_Current()
        IL_0072:  stloc.2
        IL_0073:  ldarg.0
        IL_0074:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
        IL_0079:  ldloc.2
        IL_007a:  callvirt   instance bool class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem>::Remove(!0)
        IL_007f:  pop
        IL_0080:  ldloc.s    V_5
        IL_0082:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0087:  brtrue.s   IL_006b

        IL_0089:  leave.s    IL_0097

      }  // end .try
      finally
      {
        IL_008b:  ldloc.s    V_5
        IL_008d:  brfalse.s  IL_0096

        IL_008f:  ldloc.s    V_5
        IL_0091:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0096:  endfinally
      }  // end handler
      IL_0097:  leave.s    IL_00a0

    }  // end .try
    finally
    {
      IL_0099:  ldloc.3
      IL_009a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_009f:  endfinally
    }  // end handler
    IL_00a0:  ret
  } // end of method DefaultIPFilter::Remove

  .method public hidebysig instance void 
          Add(string ipvalue,
              bool permit) cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  2
    .locals init (class Beetle.IpItem V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  call       class Beetle.IpItem Beetle.IpItem::From(string,
                                                                 bool)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  brfalse.s  IL_0012

    IL_000b:  ldarg.0
    IL_000c:  ldloc.0
    IL_000d:  call       instance void Beetle.DefaultIPFilter::Add(class Beetle.IpItem)
    IL_0012:  ret
  } // end of method DefaultIPFilter::Add

  .method public hidebysig instance class Beetle.IpItem[] 
          GetItems() cil managed
  {
    // 代码大小       56 (0x38)
    .maxstack  3
    .locals init (class Beetle.IpItem[] V_0,
             class Beetle.IpItem[] V_1,
             class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
    IL_0006:  dup
    IL_0007:  stloc.2
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
      IL_0013:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem>::get_Count()
      IL_0018:  newarr     Beetle.IpItem
      IL_001d:  stloc.0
      IL_001e:  ldarg.0
      IL_001f:  ldfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
      IL_0024:  ldloc.0
      IL_0025:  ldc.i4.0
      IL_0026:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem>::CopyTo(!0[],
                                                                                                                        int32)
      IL_002b:  ldloc.0
      IL_002c:  stloc.1
      IL_002d:  leave.s    IL_0036

    }  // end .try
    finally
    {
      IL_002f:  ldloc.2
      IL_0030:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0035:  endfinally
    }  // end handler
    IL_0036:  ldloc.1
    IL_0037:  ret
  } // end of method DefaultIPFilter::GetItems

  .method public hidebysig newslot virtual final 
          instance bool  Execute(class [System]System.Net.IPEndPoint point) cil managed
  {
    // 代码大小       64 (0x40)
    .maxstack  2
    .locals init (bool V_0,
             class Beetle.IpItem[] V_1,
             class Beetle.IpItem V_2,
             class Beetle.IpItem[] V_3,
             int32 V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  call       instance class Beetle.IpItem[] Beetle.DefaultIPFilter::GetItems()
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldlen
    IL_000b:  conv.i4
    IL_000c:  brtrue.s   IL_0010

    IL_000e:  ldc.i4.1
    IL_000f:  ret

    IL_0010:  ldarg.0
    IL_0011:  call       instance class Beetle.IpItem[] Beetle.DefaultIPFilter::GetItems()
    IL_0016:  stloc.3
    IL_0017:  ldc.i4.0
    IL_0018:  stloc.s    V_4
    IL_001a:  br.s       IL_0037

    IL_001c:  ldloc.3
    IL_001d:  ldloc.s    V_4
    IL_001f:  ldelem.ref
    IL_0020:  stloc.2
    IL_0021:  ldloc.2
    IL_0022:  ldarg.1
    IL_0023:  callvirt   instance class [System]System.Net.IPAddress [System]System.Net.IPEndPoint::get_Address()
    IL_0028:  callvirt   instance bool Beetle.IpItem::Verification(class [System]System.Net.IPAddress)
    IL_002d:  stloc.0
    IL_002e:  ldloc.0
    IL_002f:  brfalse.s  IL_003e

    IL_0031:  ldloc.s    V_4
    IL_0033:  ldc.i4.1
    IL_0034:  add
    IL_0035:  stloc.s    V_4
    IL_0037:  ldloc.s    V_4
    IL_0039:  ldloc.3
    IL_003a:  ldlen
    IL_003b:  conv.i4
    IL_003c:  blt.s      IL_001c

    IL_003e:  ldloc.0
    IL_003f:  ret
  } // end of method DefaultIPFilter::Execute

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem>::.ctor()
    IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<class Beetle.IpItem> Beetle.DefaultIPFilter::a
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ret
  } // end of method DefaultIPFilter::.ctor

} // end of class Beetle.DefaultIPFilter

.class public abstract auto ansi beforefieldinit Beetle.ServerBase
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field private class Beetle.TcpServer a
  .field private bool b
  .method public hidebysig instance void 
          Open(string ip,
               int32 port) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       class [System]System.Net.IPAddress [System]System.Net.IPAddress::Parse(string)
    IL_0007:  ldarg.2
    IL_0008:  call       instance void Beetle.ServerBase::Open(class [System]System.Net.IPAddress,
                                                               int32)
    IL_000d:  ret
  } // end of method ServerBase::Open

  .method public hidebysig specialname instance class Beetle.TcpServer 
          get_Server() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.TcpServer Beetle.ServerBase::a
    IL_0006:  ret
  } // end of method ServerBase::get_Server

  .method public hidebysig instance void 
          Open(int32 port) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldsfld     class [System]System.Net.IPAddress [System]System.Net.IPAddress::Any
    IL_0006:  ldarg.1
    IL_0007:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_000c:  call       instance void Beetle.ServerBase::Open(class [System]System.Net.IPEndPoint)
    IL_0011:  ret
  } // end of method ServerBase::Open

  .method public hidebysig instance void 
          Open(class [System]System.Net.IPAddress ip,
               int32 port) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_0008:  call       instance void Beetle.ServerBase::Open(class [System]System.Net.IPEndPoint)
    IL_000d:  ret
  } // end of method ServerBase::Open

  .method public hidebysig instance void 
          Open(class [System]System.Net.IPEndPoint ep) cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.TcpServer::.ctor()
    IL_0006:  stfld      class Beetle.TcpServer Beetle.ServerBase::a
    IL_000b:  ldarg.0
    IL_000c:  ldarg.0
    IL_000d:  ldfld      class Beetle.TcpServer Beetle.ServerBase::a
    IL_0012:  callvirt   instance void Beetle.ServerBase::OnBindEvent(class Beetle.TcpServer)
    IL_0017:  ldarg.0
    IL_0018:  ldfld      class Beetle.TcpServer Beetle.ServerBase::a
    IL_001d:  ldarg.1
    IL_001e:  callvirt   instance void Beetle.TcpServer::Open(class [System]System.Net.IPEndPoint)
    IL_0023:  ldarg.0
    IL_0024:  callvirt   instance void Beetle.ServerBase::OnOpening()
    IL_0029:  ret
  } // end of method ServerBase::Open

  .method family hidebysig newslot virtual 
          instance void  OnOpening() cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ServerBase::OnOpening

  .method family hidebysig newslot virtual 
          instance void  OnBindEvent(class Beetle.TcpServer server) cil managed
  {
    // 代码大小       58 (0x3a)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  dup
    IL_0003:  ldvirtftn  instance void Beetle.ServerBase::OnConnected(object,
                                                                      class Beetle.ChannelEventArgs)
    IL_0009:  newobj     instance void Beetle.EventChannelConnected::.ctor(object,
                                                                           native int)
    IL_000e:  callvirt   instance void Beetle.TcpServer::add_ChannelConnected(class Beetle.EventChannelConnected)
    IL_0013:  ldarg.1
    IL_0014:  ldarg.0
    IL_0015:  dup
    IL_0016:  ldvirtftn  instance void Beetle.ServerBase::OnDisposed(object,
                                                                     class Beetle.ChannelDisposedEventArgs)
    IL_001c:  newobj     instance void Beetle.EventChannelDisposed::.ctor(object,
                                                                          native int)
    IL_0021:  callvirt   instance void Beetle.TcpServer::add_ChannelDisposed(class Beetle.EventChannelDisposed)
    IL_0026:  ldarg.1
    IL_0027:  ldarg.0
    IL_0028:  dup
    IL_0029:  ldvirtftn  instance void Beetle.ServerBase::OnCreatingChannel(object,
                                                                            class Beetle.ChannelCreatingArgs)
    IL_002f:  newobj     instance void Beetle.EventChannglCreating::.ctor(object,
                                                                          native int)
    IL_0034:  callvirt   instance void Beetle.TcpServer::add_ChannelCreating(class Beetle.EventChannglCreating)
    IL_0039:  ret
  } // end of method ServerBase::OnBindEvent

  .method family hidebysig newslot virtual 
          instance void  OnCreatingChannel(object sender,
                                           class Beetle.ChannelCreatingArgs e) cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ServerBase::OnCreatingChannel

  .method family hidebysig newslot virtual 
          instance void  OnConnected(object sender,
                                     class Beetle.ChannelEventArgs e) cil managed
  {
    // 代码大小       60 (0x3c)
    .maxstack  8
    IL_0000:  ldarg.2
    IL_0001:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_0006:  ldarg.0
    IL_0007:  dup
    IL_0008:  ldvirtftn  instance void Beetle.ServerBase::OnReceive(object,
                                                                    class Beetle.ChannelReceiveEventArgs)
    IL_000e:  newobj     instance void Beetle.EventDataReceive::.ctor(object,
                                                                      native int)
    IL_0013:  stfld      class Beetle.EventDataReceive Beetle.TcpChannel::DataReceive
    IL_0018:  ldarg.2
    IL_0019:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_001e:  ldarg.0
    IL_001f:  dup
    IL_0020:  ldvirtftn  instance void Beetle.ServerBase::OnError(object,
                                                                  class Beetle.ChannelErrorEventArgs)
    IL_0026:  newobj     instance void Beetle.EventChannelError::.ctor(object,
                                                                       native int)
    IL_002b:  callvirt   instance void Beetle.TcpChannel::add_ChannelError(class Beetle.EventChannelError)
    IL_0030:  ldarg.2
    IL_0031:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_0036:  callvirt   instance void Beetle.TcpChannel::BeginReceive()
    IL_003b:  ret
  } // end of method ServerBase::OnConnected

  .method family hidebysig newslot virtual 
          instance void  OnReceive(object sender,
                                   class Beetle.ChannelReceiveEventArgs e) cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ServerBase::OnReceive

  .method family hidebysig newslot virtual 
          instance void  OnDisposed(object sender,
                                    class Beetle.ChannelDisposedEventArgs e) cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ServerBase::OnDisposed

  .method family hidebysig newslot virtual 
          instance void  OnError(object sender,
                                 class Beetle.ChannelErrorEventArgs e) cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ServerBase::OnError

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       59 (0x3b)
    .maxstack  2
    .locals init (class Beetle.ServerBase V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool Beetle.ServerBase::b
      IL_000e:  brtrue.s   IL_0031

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.1
      IL_0012:  stfld      bool Beetle.ServerBase::b
      IL_0017:  ldarg.0
      IL_0018:  ldfld      class Beetle.TcpServer Beetle.ServerBase::a
      IL_001d:  brfalse.s  IL_0031

      IL_001f:  ldarg.0
      IL_0020:  ldfld      class Beetle.TcpServer Beetle.ServerBase::a
      IL_0025:  callvirt   instance void Beetle.TcpServer::Dispose()
      IL_002a:  ldarg.0
      IL_002b:  ldnull
      IL_002c:  stfld      class Beetle.TcpServer Beetle.ServerBase::a
      IL_0031:  leave.s    IL_003a

    }  // end .try
    finally
    {
      IL_0033:  ldloc.0
      IL_0034:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0039:  endfinally
    }  // end handler
    IL_003a:  ret
  } // end of method ServerBase::Dispose

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ServerBase::.ctor

  .property instance class Beetle.TcpServer
          Server()
  {
    .get instance class Beetle.TcpServer Beetle.ServerBase::get_Server()
  } // end of property ServerBase::Server
} // end of class Beetle.ServerBase

.class public abstract auto ansi beforefieldinit Beetle.ServerBase`1<(Beetle.Package) T>
       extends Beetle.ServerBase
{
  .method family hidebysig virtual instance void 
          OnConnected(object sender,
                      class Beetle.ChannelEventArgs e) cil managed
  {
    // 代码大小       65 (0x41)
    .maxstack  4
    IL_0000:  ldarg.2
    IL_0001:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_0006:  callvirt   instance class Beetle.Package Beetle.TcpChannel::SetPackage<!T>()
    IL_000b:  ldarg.0
    IL_000c:  dup
    IL_000d:  ldvirtftn  instance void class Beetle.ServerBase`1<!T>::OnMessageReceive(class Beetle.PacketRecieveMessagerArgs)
    IL_0013:  newobj     instance void Beetle.EventPacketRecievMessage::.ctor(object,
                                                                              native int)
    IL_0018:  stfld      class Beetle.EventPacketRecievMessage Beetle.Package::ReceiveMessage
    IL_001d:  ldarg.2
    IL_001e:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_0023:  ldarg.0
    IL_0024:  dup
    IL_0025:  ldvirtftn  instance void Beetle.ServerBase::OnError(object,
                                                                  class Beetle.ChannelErrorEventArgs)
    IL_002b:  newobj     instance void Beetle.EventChannelError::.ctor(object,
                                                                       native int)
    IL_0030:  callvirt   instance void Beetle.TcpChannel::add_ChannelError(class Beetle.EventChannelError)
    IL_0035:  ldarg.2
    IL_0036:  ldfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
    IL_003b:  callvirt   instance void Beetle.TcpChannel::BeginReceive()
    IL_0040:  ret
  } // end of method ServerBase`1::OnConnected

  .method family hidebysig newslot virtual 
          instance void  OnMessageReceive(class Beetle.PacketRecieveMessagerArgs e) cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ServerBase`1::OnMessageReceive

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Beetle.ServerBase::.ctor()
    IL_0006:  ret
  } // end of method ServerBase`1::.ctor

} // end of class Beetle.ServerBase`1

.class public auto ansi sealed Beetle.EventDataReceive
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method EventDataReceive::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke(object sender,
                                class Beetle.ChannelReceiveEventArgs e) runtime managed
  {
  } // end of method EventDataReceive::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(object sender,
                      class Beetle.ChannelReceiveEventArgs e,
                      class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method EventDataReceive::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method EventDataReceive::EndInvoke

} // end of class Beetle.EventDataReceive

.class public auto ansi sealed Beetle.EventChannelError
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method EventChannelError::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke(object sender,
                                class Beetle.ChannelErrorEventArgs e) runtime managed
  {
  } // end of method EventChannelError::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(object sender,
                      class Beetle.ChannelErrorEventArgs e,
                      class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method EventChannelError::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method EventChannelError::EndInvoke

} // end of class Beetle.EventChannelError

.class public auto ansi sealed Beetle.EventChannelDisposed
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method EventChannelDisposed::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke(object sender,
                                class Beetle.ChannelDisposedEventArgs e) runtime managed
  {
  } // end of method EventChannelDisposed::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(object sender,
                      class Beetle.ChannelDisposedEventArgs e,
                      class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method EventChannelDisposed::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method EventChannelDisposed::EndInvoke

} // end of class Beetle.EventChannelDisposed

.class public auto ansi sealed Beetle.EventChannelConnected
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method EventChannelConnected::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke(object sender,
                                class Beetle.ChannelEventArgs e) runtime managed
  {
  } // end of method EventChannelConnected::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(object sender,
                      class Beetle.ChannelEventArgs e,
                      class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method EventChannelConnected::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method EventChannelConnected::EndInvoke

} // end of class Beetle.EventChannelConnected

.class public auto ansi sealed Beetle.EventChannglCreating
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method EventChannglCreating::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke(object sender,
                                class Beetle.ChannelCreatingArgs e) runtime managed
  {
  } // end of method EventChannglCreating::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(object sender,
                      class Beetle.ChannelCreatingArgs e,
                      class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method EventChannglCreating::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method EventChannglCreating::EndInvoke

} // end of class Beetle.EventChannglCreating

.class public auto ansi beforefieldinit Beetle.CreateChannelArgs
       extends [mscorlib]System.Object
{
  .field public class Beetle.TcpChannel Channel
  .field public valuetype [System]System.Net.Sockets.SocketError Error
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method CreateChannelArgs::.ctor

} // end of class Beetle.CreateChannelArgs

.class public abstract auto ansi beforefieldinit Beetle.CacheMessagecs
       extends [mscorlib]System.Object
       implements Beetle.IMessage,
                  Beetle.IObjectReset
{
  .method public hidebysig newslot abstract virtual 
          instance void  Reset() cil managed
  {
  } // end of method CacheMessagecs::Reset

  .method public hidebysig newslot abstract virtual 
          instance void  Save(class Beetle.BufferWriter writer) cil managed
  {
  } // end of method CacheMessagecs::Save

  .method public hidebysig newslot abstract virtual 
          instance void  Load(class Beetle.BufferReader reader) cil managed
  {
  } // end of method CacheMessagecs::Load

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method CacheMessagecs::.ctor

} // end of class Beetle.CacheMessagecs

.class public auto ansi sealed Beetle.EventSendMessageCompleted
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method EventSendMessageCompleted::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke(object sender,
                                class Beetle.SendMessageCompletedArgs e) runtime managed
  {
  } // end of method EventSendMessageCompleted::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(object sender,
                      class Beetle.SendMessageCompletedArgs e,
                      class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method EventSendMessageCompleted::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method EventSendMessageCompleted::EndInvoke

} // end of class Beetle.EventSendMessageCompleted

.class public auto ansi beforefieldinit Beetle.TcpSyncChannel`1<.ctor (Beetle.Package) T>
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .field private bool a
  .field private bool b
  .field private class Beetle.BufferWriter c
  .field private uint8[] d
  .field private object e
  .field private class Beetle.Package f
  .field private class [Smark.Core]Smark.Core.KeyValueCollection`1<object> g
  .field private class [System]System.Net.Sockets.Socket h
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 i
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class [mscorlib]System.Text.Encoding j
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       56 (0x38)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4     0x2000
    IL_0006:  newarr     [mscorlib]System.Byte
    IL_000b:  stfld      uint8[] class Beetle.TcpSyncChannel`1<!T>::d
    IL_0010:  ldarg.0
    IL_0011:  newobj     instance void class [Smark.Core]Smark.Core.KeyValueCollection`1<object>::.ctor()
    IL_0016:  stfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> class Beetle.TcpSyncChannel`1<!T>::g
    IL_001b:  ldarg.0
    IL_001c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0021:  ldarg.0
    IL_0022:  ldc.i4     0x2710
    IL_0027:  call       instance void class Beetle.TcpSyncChannel`1<!T>::set_ReceiveTimeout(int32)
    IL_002c:  ldarg.0
    IL_002d:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0032:  call       instance void class Beetle.TcpSyncChannel`1<!T>::set_Coding(class [mscorlib]System.Text.Encoding)
    IL_0037:  ret
  } // end of method TcpSyncChannel`1::.ctor

  .method public hidebysig specialname instance class Beetle.Package 
          get_Package() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
    IL_0006:  ret
  } // end of method TcpSyncChannel`1::get_Package

  .method public hidebysig specialname instance class [System]System.Net.Sockets.Socket 
          get_Socket() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Net.Sockets.Socket class Beetle.TcpSyncChannel`1<!T>::h
    IL_0006:  ret
  } // end of method TcpSyncChannel`1::get_Socket

  .method private hidebysig specialname instance void 
          a(class [System]System.Net.Sockets.Socket A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [System]System.Net.Sockets.Socket class Beetle.TcpSyncChannel`1<!T>::h
    IL_0007:  ret
  } // end of method TcpSyncChannel`1::a

  .method public hidebysig specialname instance class [Smark.Core]Smark.Core.KeyValueCollection`1<object> 
          get_Properties() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> class Beetle.TcpSyncChannel`1<!T>::g
    IL_0006:  ret
  } // end of method TcpSyncChannel`1::get_Properties

  .method public hidebysig specialname instance object 
          get_Item(string name) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> class Beetle.TcpSyncChannel`1<!T>::g
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance !0 class [Smark.Core]Smark.Core.KeyValueCollection`1<object>::get_Item(string)
    IL_000c:  ret
  } // end of method TcpSyncChannel`1::get_Item

  .method public hidebysig specialname instance void 
          set_Item(string name,
                   object 'value') cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [Smark.Core]Smark.Core.KeyValueCollection`1<object> class Beetle.TcpSyncChannel`1<!T>::g
    IL_0006:  ldarg.1
    IL_0007:  ldarg.2
    IL_0008:  callvirt   instance void class [Smark.Core]Smark.Core.KeyValueCollection`1<object>::set_Item(string,
                                                                                                           !0)
    IL_000d:  ret
  } // end of method TcpSyncChannel`1::set_Item

  .method public hidebysig specialname instance int32 
          get_ReceiveTimeout() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 class Beetle.TcpSyncChannel`1<!T>::i
    IL_0006:  ret
  } // end of method TcpSyncChannel`1::get_ReceiveTimeout

  .method public hidebysig specialname instance void 
          set_ReceiveTimeout(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 class Beetle.TcpSyncChannel`1<!T>::i
    IL_0007:  ret
  } // end of method TcpSyncChannel`1::set_ReceiveTimeout

  .method public hidebysig specialname instance class [mscorlib]System.Text.Encoding 
          get_Coding() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Text.Encoding class Beetle.TcpSyncChannel`1<!T>::j
    IL_0006:  ret
  } // end of method TcpSyncChannel`1::get_Coding

  .method public hidebysig specialname instance void 
          set_Coding(class [mscorlib]System.Text.Encoding 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [mscorlib]System.Text.Encoding class Beetle.TcpSyncChannel`1<!T>::j
    IL_0007:  ret
  } // end of method TcpSyncChannel`1::set_Coding

  .method family hidebysig newslot virtual 
          instance void  OnInit() cil managed
  {
    // 代码大小       81 (0x51)
    .maxstack  5
    IL_0000:  ldarg.0
    IL_0001:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
    IL_0006:  box        !T
    IL_000b:  stfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
    IL_0010:  ldarg.0
    IL_0011:  ldfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
    IL_0016:  callvirt   instance void Beetle.Package::GetPools()
    IL_001b:  ldarg.0
    IL_001c:  ldfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
    IL_0021:  callvirt   instance void Beetle.Package::CreateWriterReader()
    IL_0026:  ldarg.0
    IL_0027:  ldfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
    IL_002c:  ldarg.0
    IL_002d:  ldftn      instance void class Beetle.TcpSyncChannel`1<!T>::a(class Beetle.PacketRecieveMessagerArgs)
    IL_0033:  newobj     instance void Beetle.EventPacketRecievMessage::.ctor(object,
                                                                              native int)
    IL_0038:  stfld      class Beetle.EventPacketRecievMessage Beetle.Package::ReceiveMessage
    IL_003d:  ldarg.0
    IL_003e:  ldarg.0
    IL_003f:  call       instance class [mscorlib]System.Text.Encoding class Beetle.TcpSyncChannel`1<!T>::get_Coding()
    IL_0044:  ldc.i4.1
    IL_0045:  ldc.i4.0
    IL_0046:  newobj     instance void Beetle.BufferWriter::.ctor(class [mscorlib]System.Text.Encoding,
                                                                  bool,
                                                                  bool)
    IL_004b:  stfld      class Beetle.BufferWriter class Beetle.TcpSyncChannel`1<!T>::c
    IL_0050:  ret
  } // end of method TcpSyncChannel`1::OnInit

  .method private hidebysig instance void 
          a(class Beetle.PacketRecieveMessagerArgs A_0) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldfld      object Beetle.PacketRecieveMessagerArgs::Message
    IL_0007:  stfld      object class Beetle.TcpSyncChannel`1<!T>::e
    IL_000c:  ret
  } // end of method TcpSyncChannel`1::a

  .method family hidebysig instance object 
          OnSend(class Beetle.IMessage data) cil managed
  {
    // 代码大小       89 (0x59)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  call       instance bool class Beetle.TcpSyncChannel`1<!T>::get_Connected()
    IL_0006:  brtrue.s   IL_000e

    IL_0008:  call       class Beetle.NetTcpException Beetle.NetTcpException::ClientMustBeConnected()
    IL_000d:  throw

    IL_000e:  ldarg.0
    IL_000f:  ldfld      bool class Beetle.TcpSyncChannel`1<!T>::a
    IL_0014:  brfalse.s  IL_001c

    IL_0016:  call       class Beetle.NetTcpException Beetle.NetTcpException::ClientIsDisposed()
    IL_001b:  throw

    IL_001c:  ldarg.0
    IL_001d:  ldnull
    IL_001e:  stfld      object class Beetle.TcpSyncChannel`1<!T>::e
    IL_0023:  ldarg.0
    IL_0024:  ldfld      class Beetle.BufferWriter class Beetle.TcpSyncChannel`1<!T>::c
    IL_0029:  callvirt   instance void Beetle.BufferWriter::Reset()
    IL_002e:  ldarg.0
    IL_002f:  ldfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
    IL_0034:  ldarg.1
    IL_0035:  ldarg.0
    IL_0036:  ldfld      class Beetle.BufferWriter class Beetle.TcpSyncChannel`1<!T>::c
    IL_003b:  callvirt   instance void Beetle.Package::MessageWrite(class Beetle.IMessage,
                                                                    class Beetle.BufferWriter)
    IL_0040:  ldarg.0
    IL_0041:  ldarg.0
    IL_0042:  ldfld      class Beetle.BufferWriter class Beetle.TcpSyncChannel`1<!T>::c
    IL_0047:  call       instance void class Beetle.TcpSyncChannel`1<!T>::a(class Beetle.BufferWriter)
    IL_004c:  ldarg.0
    IL_004d:  call       instance void class Beetle.TcpSyncChannel`1<!T>::a()
    IL_0052:  ldarg.0
    IL_0053:  ldfld      object class Beetle.TcpSyncChannel`1<!T>::e
    IL_0058:  ret
  } // end of method TcpSyncChannel`1::OnSend

  .method private hidebysig instance void 
          a(class Beetle.BufferWriter A_0) cil managed
  {
    // 代码大小       100 (0x64)
    .maxstack  6
    .locals init (int32 V_0,
             class b V_1,
             class [mscorlib]System.Exception V_2)
    .try
    {
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_0040

      IL_0004:  ldarg.0
      IL_0005:  ldfld      class Beetle.BufferWriter class Beetle.TcpSyncChannel`1<!T>::c
      IL_000a:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_000f:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
      IL_0014:  stloc.1
      IL_0015:  ldc.i4.0
      IL_0016:  stloc.0
      IL_0017:  br.s       IL_0037

      IL_0019:  ldloc.0
      IL_001a:  ldarg.0
      IL_001b:  call       instance class [System]System.Net.Sockets.Socket class Beetle.TcpSyncChannel`1<!T>::get_Socket()
      IL_0020:  ldloc.1
      IL_0021:  ldfld      uint8[] b::e
      IL_0026:  ldloc.0
      IL_0027:  ldloc.1
      IL_0028:  ldfld      int32 b::h
      IL_002d:  ldloc.0
      IL_002e:  sub
      IL_002f:  ldc.i4.0
      IL_0030:  callvirt   instance int32 [System]System.Net.Sockets.Socket::Send(uint8[],
                                                                                  int32,
                                                                                  int32,
                                                                                  valuetype [System]System.Net.Sockets.SocketFlags)
      IL_0035:  add
      IL_0036:  stloc.0
      IL_0037:  ldloc.0
      IL_0038:  ldloc.1
      IL_0039:  ldfld      int32 b::h
      IL_003e:  blt.s      IL_0019

      IL_0040:  ldarg.0
      IL_0041:  ldfld      class Beetle.BufferWriter class Beetle.TcpSyncChannel`1<!T>::c
      IL_0046:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_004b:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
      IL_0050:  ldc.i4.0
      IL_0051:  bgt.s      IL_0004

      IL_0053:  leave.s    IL_0063

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0055:  stloc.2
      IL_0056:  ldarg.0
      IL_0057:  call       instance void class Beetle.TcpSyncChannel`1<!T>::Dispose()
      IL_005c:  ldloc.2
      IL_005d:  call       class Beetle.NetTcpException Beetle.NetTcpException::ClientDataProcessError(class [mscorlib]System.Exception)
      IL_0062:  throw

    }  // end handler
    IL_0063:  ret
  } // end of method TcpSyncChannel`1::a

  .method private hidebysig instance void 
          a() cil managed
  {
    // 代码大小       100 (0x64)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1,
             class Beetle.NetTcpException V_2,
             class [mscorlib]System.Exception V_3)
    .try
    {
      IL_0000:  call       int32 [mscorlib]System.Environment::get_TickCount()
      IL_0005:  stloc.0
      IL_0006:  br.s       IL_0042

      IL_0008:  call       int32 [mscorlib]System.Environment::get_TickCount()
      IL_000d:  ldloc.0
      IL_000e:  sub
      IL_000f:  ldarg.0
      IL_0010:  call       instance int32 class Beetle.TcpSyncChannel`1<!T>::get_ReceiveTimeout()
      IL_0015:  ble.s      IL_001d

      IL_0017:  call       class Beetle.NetTcpException Beetle.NetTcpException::ClientReceiveTimeout()
      IL_001c:  throw

      IL_001d:  ldarg.0
      IL_001e:  call       instance class [System]System.Net.Sockets.Socket class Beetle.TcpSyncChannel`1<!T>::get_Socket()
      IL_0023:  ldarg.0
      IL_0024:  ldfld      uint8[] class Beetle.TcpSyncChannel`1<!T>::d
      IL_0029:  callvirt   instance int32 [System]System.Net.Sockets.Socket::Receive(uint8[])
      IL_002e:  stloc.1
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
      IL_0035:  ldarg.0
      IL_0036:  ldfld      uint8[] class Beetle.TcpSyncChannel`1<!T>::d
      IL_003b:  ldc.i4.0
      IL_003c:  ldloc.1
      IL_003d:  callvirt   instance void Beetle.Package::Import(uint8[],
                                                                int32,
                                                                int32)
      IL_0042:  ldarg.0
      IL_0043:  ldfld      object class Beetle.TcpSyncChannel`1<!T>::e
      IL_0048:  brfalse.s  IL_0008

      IL_004a:  leave.s    IL_0063

    }  // end .try
    catch Beetle.NetTcpException 
    {
      IL_004c:  stloc.2
      IL_004d:  ldarg.0
      IL_004e:  call       instance void class Beetle.TcpSyncChannel`1<!T>::Dispose()
      IL_0053:  ldloc.2
      IL_0054:  throw

    }  // end handler
    catch [mscorlib]System.Exception 
    {
      IL_0055:  stloc.3
      IL_0056:  ldarg.0
      IL_0057:  call       instance void class Beetle.TcpSyncChannel`1<!T>::Dispose()
      IL_005c:  ldloc.3
      IL_005d:  call       class Beetle.NetTcpException Beetle.NetTcpException::ClientDataProcessError(class [mscorlib]System.Exception)
      IL_0062:  throw

    }  // end handler
    IL_0063:  ret
  } // end of method TcpSyncChannel`1::a

  .method public hidebysig specialname instance bool 
          get_IsDisplsed() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool class Beetle.TcpSyncChannel`1<!T>::a
    IL_0006:  ret
  } // end of method TcpSyncChannel`1::get_IsDisplsed

  .method public hidebysig specialname instance bool 
          get_Connected() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool class Beetle.TcpSyncChannel`1<!T>::b
    IL_0006:  ret
  } // end of method TcpSyncChannel`1::get_Connected

  .method family hidebysig newslot virtual 
          instance void  OnDisposed() cil managed
  {
    // 代码大小       31 (0x1f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
    IL_0006:  brfalse.s  IL_0013

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class Beetle.Package class Beetle.TcpSyncChannel`1<!T>::f
    IL_000e:  callvirt   instance void Beetle.Package::Dispose()
    IL_0013:  ldarg.0
    IL_0014:  call       instance class [System]System.Net.Sockets.Socket class Beetle.TcpSyncChannel`1<!T>::get_Socket()
    IL_0019:  call       void Beetle.TcpServer::ReleaseSocket(class [System]System.Net.Sockets.Socket)
    IL_001e:  ret
  } // end of method TcpSyncChannel`1::OnDisposed

  .method public hidebysig newslot virtual 
          instance object  Send(object data) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  castclass  Beetle.IMessage
    IL_0007:  call       instance object class Beetle.TcpSyncChannel`1<!T>::OnSend(class Beetle.IMessage)
    IL_000c:  ret
  } // end of method TcpSyncChannel`1::Send

  .method public hidebysig instance void 
          Connect(string host,
                  int32 port) cil managed
  {
    // 代码大小       111 (0x6f)
    .maxstack  5
    .locals init (class [System]System.Net.IPAddress[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool class Beetle.TcpSyncChannel`1<!T>::b
    IL_0006:  brtrue.s   IL_006e

    IL_0008:  ldsfld     bool Beetle.TcpUtils::a
    IL_000d:  brtrue.s   IL_0015

    IL_000f:  call       class Beetle.NetTcpException Beetle.NetTcpException::NotInitialize()
    IL_0014:  throw

    IL_0015:  ldarg.0
    IL_0016:  ldfld      bool class Beetle.TcpSyncChannel`1<!T>::a
    IL_001b:  brfalse.s  IL_0023

    IL_001d:  call       class Beetle.NetTcpException Beetle.NetTcpException::ClientIsDisposed()
    IL_0022:  throw

    IL_0023:  ldarg.1
    IL_0024:  call       class [System]System.Net.IPAddress[] [System]System.Net.Dns::GetHostAddresses(string)
    IL_0029:  stloc.0
    IL_002a:  ldarg.0
    IL_002b:  ldc.i4.2
    IL_002c:  ldc.i4.1
    IL_002d:  ldc.i4.6
    IL_002e:  newobj     instance void [System]System.Net.Sockets.Socket::.ctor(valuetype [System]System.Net.Sockets.AddressFamily,
                                                                                valuetype [System]System.Net.Sockets.SocketType,
                                                                                valuetype [System]System.Net.Sockets.ProtocolType)
    IL_0033:  call       instance void class Beetle.TcpSyncChannel`1<!T>::a(class [System]System.Net.Sockets.Socket)
    IL_0038:  ldarg.0
    IL_0039:  call       instance class [System]System.Net.Sockets.Socket class Beetle.TcpSyncChannel`1<!T>::get_Socket()
    IL_003e:  ldloc.0
    IL_003f:  ldc.i4.0
    IL_0040:  ldelem.ref
    IL_0041:  ldarg.2
    IL_0042:  newobj     instance void [System]System.Net.IPEndPoint::.ctor(class [System]System.Net.IPAddress,
                                                                            int32)
    IL_0047:  callvirt   instance void [System]System.Net.Sockets.Socket::Connect(class [System]System.Net.EndPoint)
    IL_004c:  ldarg.0
    IL_004d:  callvirt   instance void class Beetle.TcpSyncChannel`1<!T>::OnInit()
    IL_0052:  ldarg.0
    IL_0053:  ldc.i4.1
    IL_0054:  stfld      bool class Beetle.TcpSyncChannel`1<!T>::b
    IL_0059:  ldarg.0
    IL_005a:  call       instance class [System]System.Net.Sockets.Socket class Beetle.TcpSyncChannel`1<!T>::get_Socket()
    IL_005f:  ldc.i4     0x4e20
    IL_0064:  ldc.i4     0x2710
    IL_0069:  call       void Beetle.TcpServer::SetKeepAliveValues(class [System]System.Net.Sockets.Socket,
                                                                   uint32,
                                                                   uint32)
    IL_006e:  ret
  } // end of method TcpSyncChannel`1::Connect

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       39 (0x27)
    .maxstack  2
    .locals init (class Beetle.TcpSyncChannel`1<!T> V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool class Beetle.TcpSyncChannel`1<!T>::a
      IL_000e:  brtrue.s   IL_001d

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.1
      IL_0012:  stfld      bool class Beetle.TcpSyncChannel`1<!T>::a
      IL_0017:  ldarg.0
      IL_0018:  callvirt   instance void class Beetle.TcpSyncChannel`1<!T>::OnDisposed()
      IL_001d:  leave.s    IL_0026

    }  // end .try
    finally
    {
      IL_001f:  ldloc.0
      IL_0020:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0025:  endfinally
    }  // end handler
    IL_0026:  ret
  } // end of method TcpSyncChannel`1::Dispose

  .property instance class Beetle.Package
          Package()
  {
    .get instance class Beetle.Package Beetle.TcpSyncChannel`1::get_Package()
  } // end of property TcpSyncChannel`1::Package
  .property instance class [System]System.Net.Sockets.Socket
          Socket()
  {
    .get instance class [System]System.Net.Sockets.Socket Beetle.TcpSyncChannel`1::get_Socket()
    .set instance void Beetle.TcpSyncChannel`1::a(class [System]System.Net.Sockets.Socket)
  } // end of property TcpSyncChannel`1::Socket
  .property instance class [Smark.Core]Smark.Core.KeyValueCollection`1<object>
          Properties()
  {
    .get instance class [Smark.Core]Smark.Core.KeyValueCollection`1<object> Beetle.TcpSyncChannel`1::get_Properties()
  } // end of property TcpSyncChannel`1::Properties
  .property instance object Item(string)
  {
    .get instance object Beetle.TcpSyncChannel`1::get_Item(string)
    .set instance void Beetle.TcpSyncChannel`1::set_Item(string,
                                                         object)
  } // end of property TcpSyncChannel`1::Item
  .property instance int32 ReceiveTimeout()
  {
    .get instance int32 Beetle.TcpSyncChannel`1::get_ReceiveTimeout()
    .set instance void Beetle.TcpSyncChannel`1::set_ReceiveTimeout(int32)
  } // end of property TcpSyncChannel`1::ReceiveTimeout
  .property instance class [mscorlib]System.Text.Encoding
          Coding()
  {
    .get instance class [mscorlib]System.Text.Encoding Beetle.TcpSyncChannel`1::get_Coding()
    .set instance void Beetle.TcpSyncChannel`1::set_Coding(class [mscorlib]System.Text.Encoding)
  } // end of property TcpSyncChannel`1::Coding
  .property instance bool IsDisplsed()
  {
    .get instance bool Beetle.TcpSyncChannel`1::get_IsDisplsed()
  } // end of property TcpSyncChannel`1::IsDisplsed
  .property instance bool Connected()
  {
    .get instance bool Beetle.TcpSyncChannel`1::get_Connected()
  } // end of property TcpSyncChannel`1::Connected
} // end of class Beetle.TcpSyncChannel`1

.class public auto ansi beforefieldinit Beetle.ChannelErrorEventArgs
       extends Beetle.ChannelEventArgs
{
  .field private class [mscorlib]System.Exception a
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance class [mscorlib]System.Exception 
          get_Exception() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Exception Beetle.ChannelErrorEventArgs::a
    IL_0006:  ret
  } // end of method ChannelErrorEventArgs::get_Exception

  .method public hidebysig specialname instance void 
          set_Exception(class [mscorlib]System.Exception 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class [mscorlib]System.Exception Beetle.ChannelErrorEventArgs::a
    IL_0007:  ret
  } // end of method ChannelErrorEventArgs::set_Exception

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void Beetle.ChannelEventArgs::.ctor()
    IL_0006:  ret
  } // end of method ChannelErrorEventArgs::.ctor

  .property instance class [mscorlib]System.Exception
          Exception()
  {
    .get instance class [mscorlib]System.Exception Beetle.ChannelErrorEventArgs::get_Exception()
    .set instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
  } // end of property ChannelErrorEventArgs::Exception
} // end of class Beetle.ChannelErrorEventArgs

.class public auto ansi beforefieldinit Beetle.ByteArrayPool
       extends [mscorlib]System.Object
{
  .field private class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> a
  .field private int32 b
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 initcount,
                               int32 length) cil managed
  {
    // 代码大小       54 (0x36)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment>::.ctor()
    IL_0006:  stfld      class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> Beetle.ByteArrayPool::a
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldarg.2
    IL_0013:  stfld      int32 Beetle.ByteArrayPool::b
    IL_0018:  ldc.i4.0
    IL_0019:  stloc.0
    IL_001a:  br.s       IL_0031

    IL_001c:  ldarg.0
    IL_001d:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> Beetle.ByteArrayPool::a
    IL_0022:  ldarg.0
    IL_0023:  call       instance class Beetle.ByteArraySegment Beetle.ByteArrayPool::a()
    IL_0028:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment>::Push(!0)
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  stloc.0
    IL_0031:  ldloc.0
    IL_0032:  ldarg.1
    IL_0033:  blt.s      IL_001c

    IL_0035:  ret
  } // end of method ByteArrayPool::.ctor

  .method private hidebysig instance class Beetle.ByteArraySegment 
          a() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Beetle.ByteArrayPool::b
    IL_0006:  newobj     instance void Beetle.ByteArraySegment::.ctor(int32)
    IL_000b:  ret
  } // end of method ByteArrayPool::a

  .method public hidebysig specialname instance int32 
          get_Count() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> Beetle.ByteArrayPool::a
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment>::get_Count()
    IL_000b:  ret
  } // end of method ByteArrayPool::get_Count

  .method public hidebysig instance class Beetle.ByteArraySegment 
          Pop() cil managed
  {
    // 代码大小       59 (0x3b)
    .maxstack  2
    .locals init (class Beetle.ByteArraySegment V_0,
             class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> Beetle.ByteArrayPool::a
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> Beetle.ByteArrayPool::a
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0029

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> Beetle.ByteArrayPool::a
      IL_0021:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment>::Pop()
      IL_0026:  stloc.0
      IL_0027:  leave.s    IL_0039

      IL_0029:  ldarg.0
      IL_002a:  call       instance class Beetle.ByteArraySegment Beetle.ByteArrayPool::a()
      IL_002f:  stloc.0
      IL_0030:  leave.s    IL_0039

    }  // end .try
    finally
    {
      IL_0032:  ldloc.1
      IL_0033:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0038:  endfinally
    }  // end handler
    IL_0039:  ldloc.0
    IL_003a:  ret
  } // end of method ByteArrayPool::Pop

  .method public hidebysig instance void 
          Push(class Beetle.ByteArraySegment item) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> Beetle.ByteArrayPool::a
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment> Beetle.ByteArrayPool::a
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<class Beetle.ByteArraySegment>::Push(!0)
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method ByteArrayPool::Push

  .property instance int32 Count()
  {
    .get instance int32 Beetle.ByteArrayPool::get_Count()
  } // end of property ByteArrayPool::Count
} // end of class Beetle.ByteArrayPool

.class public auto ansi beforefieldinit Beetle.Caching.CacheManager
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit a
         extends Beetle.Task
  {
    .field private class Beetle.Caching.CacheManager a
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance class Beetle.Caching.CacheManager 
            a() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // 代码大小       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Beetle.Caching.CacheManager Beetle.Caching.CacheManager/a::a
      IL_0006:  ret
    } // end of method a::a

    .method public hidebysig specialname 
            instance void  a(class Beetle.Caching.CacheManager A_0) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // 代码大小       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class Beetle.Caching.CacheManager Beetle.Caching.CacheManager/a::a
      IL_0007:  ret
    } // end of method a::a

    .method family hidebysig virtual instance void 
            Execute() cil managed
    {
      // 代码大小       132 (0x84)
      .maxstack  3
      .locals init (object[] V_0,
               object V_1,
               class [mscorlib]System.Collections.Hashtable V_2,
               object[] V_3,
               int32 V_4)
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  call       instance class Beetle.Caching.CacheManager Beetle.Caching.CacheManager/a::a()
      IL_0008:  ldfld      class [mscorlib]System.Collections.Hashtable Beetle.Caching.CacheManager::b
      IL_000d:  dup
      IL_000e:  stloc.2
      IL_000f:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
      .try
      {
        IL_0014:  ldarg.0
        IL_0015:  call       instance class Beetle.Caching.CacheManager Beetle.Caching.CacheManager/a::a()
        IL_001a:  ldfld      class [mscorlib]System.Collections.Hashtable Beetle.Caching.CacheManager::b
        IL_001f:  callvirt   instance class [mscorlib]System.Collections.ICollection [mscorlib]System.Collections.Hashtable::get_Keys()
        IL_0024:  callvirt   instance int32 [mscorlib]System.Collections.ICollection::get_Count()
        IL_0029:  newarr     [mscorlib]System.Object
        IL_002e:  stloc.0
        IL_002f:  ldarg.0
        IL_0030:  call       instance class Beetle.Caching.CacheManager Beetle.Caching.CacheManager/a::a()
        IL_0035:  ldfld      class [mscorlib]System.Collections.Hashtable Beetle.Caching.CacheManager::b
        IL_003a:  callvirt   instance class [mscorlib]System.Collections.ICollection [mscorlib]System.Collections.Hashtable::get_Keys()
        IL_003f:  ldloc.0
        IL_0040:  ldc.i4.0
        IL_0041:  callvirt   instance void [mscorlib]System.Collections.ICollection::CopyTo(class [mscorlib]System.Array,
                                                                                            int32)
        IL_0046:  leave.s    IL_004f

      }  // end .try
      finally
      {
        IL_0048:  ldloc.2
        IL_0049:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
        IL_004e:  endfinally
      }  // end handler
      IL_004f:  ldloc.0
      IL_0050:  stloc.3
      IL_0051:  ldc.i4.0
      IL_0052:  stloc.s    V_4
      IL_0054:  br.s       IL_007c

      IL_0056:  ldloc.3
      IL_0057:  ldloc.s    V_4
      IL_0059:  ldelem.ref
      IL_005a:  stloc.1
      IL_005b:  ldarg.0
      IL_005c:  call       instance class Beetle.Caching.CacheManager Beetle.Caching.CacheManager/a::a()
      IL_0061:  ldfld      class [mscorlib]System.Collections.Hashtable Beetle.Caching.CacheManager::b
      IL_0066:  ldloc.1
      IL_0067:  callvirt   instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
      IL_006c:  castclass  e
      IL_0071:  callvirt   instance void e::a()
      IL_0076:  ldloc.s    V_4
      IL_0078:  ldc.i4.1
      IL_0079:  add
      IL_007a:  stloc.s    V_4
      IL_007c:  ldloc.s    V_4
      IL_007e:  ldloc.3
      IL_007f:  ldlen
      IL_0080:  conv.i4
      IL_0081:  blt.s      IL_0056

      IL_0083:  ret
    } // end of method a::Execute

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // 代码大小       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void Beetle.Task::.ctor()
      IL_0006:  ret
    } // end of method a::.ctor

  } // end of class a

  .field private class Beetle.Caching.CacheManager/a a
  .field private class [mscorlib]System.Collections.Hashtable b
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4     0xea60
    IL_0006:  call       instance void Beetle.Caching.CacheManager::.ctor(int32)
    IL_000b:  ret
  } // end of method CacheManager::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 eacheExpiredtime) cil managed
  {
    // 代码大小       59 (0x3b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.Caching.CacheManager/a::.ctor()
    IL_0006:  stfld      class Beetle.Caching.CacheManager/a Beetle.Caching.CacheManager::a
    IL_000b:  ldarg.0
    IL_000c:  ldc.i4     0x400
    IL_0011:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32)
    IL_0016:  stfld      class [mscorlib]System.Collections.Hashtable Beetle.Caching.CacheManager::b
    IL_001b:  ldarg.0
    IL_001c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0021:  ldarg.0
    IL_0022:  ldfld      class Beetle.Caching.CacheManager/a Beetle.Caching.CacheManager::a
    IL_0027:  ldarg.0
    IL_0028:  callvirt   instance void Beetle.Caching.CacheManager/a::a(class Beetle.Caching.CacheManager)
    IL_002d:  ldarg.0
    IL_002e:  ldfld      class Beetle.Caching.CacheManager/a Beetle.Caching.CacheManager::a
    IL_0033:  ldarg.1
    IL_0034:  ldc.i4.0
    IL_0035:  callvirt   instance void Beetle.Task::Start(int32,
                                                          int32)
    IL_003a:  ret
  } // end of method CacheManager::.ctor

  .method public hidebysig instance void 
          Remove(string group,
                 string key) cil managed
  {
    // 代码大小       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class e Beetle.Caching.CacheManager::a(string)
    IL_0007:  ldarg.2
    IL_0008:  callvirt   instance void e::b(string)
    IL_000d:  ret
  } // end of method CacheManager::Remove

  .method public hidebysig instance void 
          Set(string group,
              string key,
              object data,
              valuetype [mscorlib]System.DateTime expired) cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  4
    .locals init (class e V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class e Beetle.Caching.CacheManager::a(string)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.2
    IL_000a:  ldarg.3
    IL_000b:  ldarg.s    expired
    IL_000d:  callvirt   instance void e::a(string,
                                            object,
                                            valuetype [mscorlib]System.DateTime)
    IL_0012:  ret
  } // end of method CacheManager::Set

  .method public hidebysig instance object 
          Get(string group,
              string key) cil managed
  {
    // 代码大小       16 (0x10)
    .maxstack  2
    .locals init (class e V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class e Beetle.Caching.CacheManager::a(string)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.2
    IL_000a:  callvirt   instance object e::a(string)
    IL_000f:  ret
  } // end of method CacheManager::Get

  .method private hidebysig instance class e 
          a(string A_0) cil managed
  {
    // 代码大小       66 (0x42)
    .maxstack  3
    .locals init (class e V_0,
             class e V_1,
             class [mscorlib]System.Collections.Hashtable V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Hashtable Beetle.Caching.CacheManager::b
    IL_0006:  dup
    IL_0007:  stloc.2
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [mscorlib]System.Collections.Hashtable Beetle.Caching.CacheManager::b
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
      IL_0019:  castclass  e
      IL_001e:  stloc.0
      IL_001f:  ldloc.0
      IL_0020:  brtrue.s   IL_0035

      IL_0022:  newobj     instance void e::.ctor()
      IL_0027:  stloc.0
      IL_0028:  ldarg.0
      IL_0029:  ldfld      class [mscorlib]System.Collections.Hashtable Beetle.Caching.CacheManager::b
      IL_002e:  ldarg.1
      IL_002f:  ldloc.0
      IL_0030:  callvirt   instance void [mscorlib]System.Collections.Hashtable::set_Item(object,
                                                                                          object)
      IL_0035:  ldloc.0
      IL_0036:  stloc.1
      IL_0037:  leave.s    IL_0040

    }  // end .try
    finally
    {
      IL_0039:  ldloc.2
      IL_003a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003f:  endfinally
    }  // end handler
    IL_0040:  ldloc.1
    IL_0041:  ret
  } // end of method CacheManager::a

} // end of class Beetle.Caching.CacheManager

.class private auto ansi beforefieldinit i
       extends [mscorlib]System.Object
{
  .field public class Beetle.IMessageHandler a
  .field public class [Smark.Core]Smark.Core.MethodHandler b
  .field public class [mscorlib]System.Type c
  .field private static class [mscorlib]System.Collections.Hashtable d
  .field public class Beetle.MessageHandlerArgs e
  .method public hidebysig static class [mscorlib]System.Collections.Generic.IList`1<class i> 
          d(class Beetle.IMessageHandler A_0) cil managed
  {
    // 代码大小       181 (0xb5)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class i> V_0,
             class [mscorlib]System.Type V_1,
             class [mscorlib]System.Reflection.MethodInfo V_2,
             class [mscorlib]System.Reflection.ParameterInfo[] V_3,
             class i V_4,
             class [mscorlib]System.Collections.Generic.IList`1<class i> V_5,
             class [mscorlib]System.Collections.Hashtable V_6,
             class [mscorlib]System.Reflection.MethodInfo[] V_7,
             int32 V_8)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Hashtable i::d
    IL_0005:  dup
    IL_0006:  stloc.s    V_6
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
      IL_0013:  stloc.1
      IL_0014:  ldsfld     class [mscorlib]System.Collections.Hashtable i::d
      IL_0019:  ldloc.1
      IL_001a:  callvirt   instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
      IL_001f:  castclass  class [mscorlib]System.Collections.Generic.List`1<class i>
      IL_0024:  stloc.0
      IL_0025:  ldloc.0
      IL_0026:  brtrue.s   IL_00a5

      IL_0028:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class i>::.ctor()
      IL_002d:  stloc.0
      IL_002e:  ldloc.1
      IL_002f:  ldc.i4.s   20
      IL_0031:  callvirt   instance class [mscorlib]System.Reflection.MethodInfo[] [mscorlib]System.Type::GetMethods(valuetype [mscorlib]System.Reflection.BindingFlags)
      IL_0036:  stloc.s    V_7
      IL_0038:  ldc.i4.0
      IL_0039:  stloc.s    V_8
      IL_003b:  br.s       IL_009d

      IL_003d:  ldloc.s    V_7
      IL_003f:  ldloc.s    V_8
      IL_0041:  ldelem.ref
      IL_0042:  stloc.2
      IL_0043:  ldloc.2
      IL_0044:  callvirt   instance class [mscorlib]System.Reflection.ParameterInfo[] [mscorlib]System.Reflection.MethodBase::GetParameters()
      IL_0049:  stloc.3
      IL_004a:  ldloc.3
      IL_004b:  ldlen
      IL_004c:  conv.i4
      IL_004d:  ldc.i4.2
      IL_004e:  bne.un.s   IL_0097

      IL_0050:  ldloc.3
      IL_0051:  ldc.i4.0
      IL_0052:  ldelem.ref
      IL_0053:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Reflection.ParameterInfo::get_ParameterType()
      IL_0058:  ldtoken    Beetle.TcpChannel
      IL_005d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_0062:  bne.un.s   IL_0097

      IL_0064:  newobj     instance void i::.ctor()
      IL_0069:  stloc.s    V_4
      IL_006b:  ldloc.s    V_4
      IL_006d:  ldarg.0
      IL_006e:  stfld      class Beetle.IMessageHandler i::a
      IL_0073:  ldloc.s    V_4
      IL_0075:  ldloc.2
      IL_0076:  newobj     instance void [Smark.Core]Smark.Core.MethodHandler::.ctor(class [mscorlib]System.Reflection.MethodInfo)
      IL_007b:  stfld      class [Smark.Core]Smark.Core.MethodHandler i::b
      IL_0080:  ldloc.s    V_4
      IL_0082:  ldloc.3
      IL_0083:  ldc.i4.1
      IL_0084:  ldelem.ref
      IL_0085:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Reflection.ParameterInfo::get_ParameterType()
      IL_008a:  stfld      class [mscorlib]System.Type i::c
      IL_008f:  ldloc.0
      IL_0090:  ldloc.s    V_4
      IL_0092:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class i>::Add(!0)
      IL_0097:  ldloc.s    V_8
      IL_0099:  ldc.i4.1
      IL_009a:  add
      IL_009b:  stloc.s    V_8
      IL_009d:  ldloc.s    V_8
      IL_009f:  ldloc.s    V_7
      IL_00a1:  ldlen
      IL_00a2:  conv.i4
      IL_00a3:  blt.s      IL_003d

      IL_00a5:  ldloc.0
      IL_00a6:  stloc.s    V_5
      IL_00a8:  leave.s    IL_00b2

    }  // end .try
    finally
    {
      IL_00aa:  ldloc.s    V_6
      IL_00ac:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_00b1:  endfinally
    }  // end handler
    IL_00b2:  ldloc.s    V_5
    IL_00b4:  ret
  } // end of method i::d

  .method public hidebysig instance void 
          d(class Beetle.TcpChannel A_0,
            object A_1) cil managed
  {
    // 代码大小       95 (0x5f)
    .maxstack  5
    .locals init (object[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.MessageHandlerArgs i::e
    IL_0006:  ldarg.2
    IL_0007:  stfld      object Beetle.MessageHandlerArgs::Message
    IL_000c:  ldarg.0
    IL_000d:  ldfld      class Beetle.MessageHandlerArgs i::e
    IL_0012:  ldc.i4.0
    IL_0013:  stfld      bool Beetle.MessageHandlerArgs::Cancel
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.IMessageHandler i::a
    IL_001e:  ldarg.1
    IL_001f:  ldarg.0
    IL_0020:  ldfld      class Beetle.MessageHandlerArgs i::e
    IL_0025:  callvirt   instance void Beetle.IMessageHandler::ProcessMessage(class Beetle.TcpChannel,
                                                                              class Beetle.MessageHandlerArgs)
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class Beetle.MessageHandlerArgs i::e
    IL_0030:  ldfld      bool Beetle.MessageHandlerArgs::Cancel
    IL_0035:  brtrue.s   IL_005e

    IL_0037:  ldarg.0
    IL_0038:  ldfld      class [Smark.Core]Smark.Core.MethodHandler i::b
    IL_003d:  callvirt   instance class [Smark.Core]Smark.Core.FastMethodHandler [Smark.Core]Smark.Core.MethodHandler::get_Execute()
    IL_0042:  ldarg.0
    IL_0043:  ldfld      class Beetle.IMessageHandler i::a
    IL_0048:  ldc.i4.2
    IL_0049:  newarr     [mscorlib]System.Object
    IL_004e:  stloc.0
    IL_004f:  ldloc.0
    IL_0050:  ldc.i4.0
    IL_0051:  ldarg.1
    IL_0052:  stelem.ref
    IL_0053:  ldloc.0
    IL_0054:  ldc.i4.1
    IL_0055:  ldarg.2
    IL_0056:  stelem.ref
    IL_0057:  ldloc.0
    IL_0058:  callvirt   instance object [Smark.Core]Smark.Core.FastMethodHandler::Invoke(object,
                                                                                          object[])
    IL_005d:  pop
    IL_005e:  ret
  } // end of method i::d

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.MessageHandlerArgs::.ctor()
    IL_0006:  stfld      class Beetle.MessageHandlerArgs i::e
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ret
  } // end of method i::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       16 (0x10)
    .maxstack  8
    IL_0000:  ldc.i4     0x200
    IL_0005:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32)
    IL_000a:  stsfld     class [mscorlib]System.Collections.Hashtable i::d
    IL_000f:  ret
  } // end of method i::.cctor

} // end of class i

.class private auto ansi beforefieldinit n
       extends [mscorlib]System.Object
{
  .field public uint8[] a
  .field public int32 b
  .field public int32 c
  .field public int32 d
  .method public hidebysig instance void 
          e(uint8[] A_0,
            int32 A_1,
            int32 A_2) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      uint8[] n::a
    IL_0007:  ldarg.0
    IL_0008:  ldarg.2
    IL_0009:  stfld      int32 n::b
    IL_000e:  ldarg.0
    IL_000f:  ldarg.3
    IL_0010:  stfld      int32 n::c
    IL_0015:  ret
  } // end of method n::e

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method n::.ctor

} // end of class n

.class private auto ansi beforefieldinit r
       extends [mscorlib]System.Object
{
  .field public class Beetle.ByteArraySegment a
  .field public class [System]System.Net.Sockets.Socket b
  .field static assembly string c
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0006:  stfld      class Beetle.ByteArraySegment r::a
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ret
  } // end of method r::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "V/r2I2CpRaI="
    IL_0005:  stsfld     string r::c
    IL_000a:  ret
  } // end of method r::.cctor

} // end of class r

.class private auto ansi beforefieldinit e
       extends [mscorlib]System.Object
{
  .field private class [mscorlib]System.Collections.Hashtable a
  .field private class q b
  .field private class q c
  .method public hidebysig instance void 
          a() cil managed
  {
    // 代码大小       154 (0x9a)
    .maxstack  2
    .locals init (class q V_0,
             class [mscorlib]System.Collections.Hashtable V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Hashtable e::a
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class q e::c
      IL_0013:  brfalse.s  IL_0090

      IL_0015:  br.s       IL_007b

      IL_0017:  ldarg.0
      IL_0018:  ldfld      class q e::c
      IL_001d:  stloc.0
      IL_001e:  ldloc.0
      IL_001f:  callvirt   instance class q q::c()
      IL_0024:  brfalse.s  IL_0034

      IL_0026:  ldarg.0
      IL_0027:  ldloc.0
      IL_0028:  callvirt   instance class q q::c()
      IL_002d:  stfld      class q e::c
      IL_0032:  br.s       IL_003b

      IL_0034:  ldarg.0
      IL_0035:  ldnull
      IL_0036:  stfld      class q e::c
      IL_003b:  ldarg.0
      IL_003c:  ldfld      class q e::c
      IL_0041:  brfalse.s  IL_004f

      IL_0043:  ldarg.0
      IL_0044:  ldfld      class q e::c
      IL_0049:  ldnull
      IL_004a:  callvirt   instance void q::a(class q)
      IL_004f:  ldloc.0
      IL_0050:  callvirt   instance class q q::c()
      IL_0055:  brfalse.s  IL_0063

      IL_0057:  ldloc.0
      IL_0058:  callvirt   instance class q q::c()
      IL_005d:  ldnull
      IL_005e:  callvirt   instance void q::a(class q)
      IL_0063:  ldloc.0
      IL_0064:  ldnull
      IL_0065:  callvirt   instance void q::b(class q)
      IL_006a:  ldarg.0
      IL_006b:  ldfld      class [mscorlib]System.Collections.Hashtable e::a
      IL_0070:  ldloc.0
      IL_0071:  callvirt   instance string q::b()
      IL_0076:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Remove(object)
      IL_007b:  ldarg.0
      IL_007c:  ldfld      class q e::c
      IL_0081:  brfalse.s  IL_0090

      IL_0083:  ldarg.0
      IL_0084:  ldfld      class q e::c
      IL_0089:  callvirt   instance bool q::d()
      IL_008e:  brtrue.s   IL_0017

      IL_0090:  leave.s    IL_0099

    }  // end .try
    finally
    {
      IL_0092:  ldloc.1
      IL_0093:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0098:  endfinally
    }  // end handler
    IL_0099:  ret
  } // end of method e::a

  .method public hidebysig instance void 
          b(string A_0) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Hashtable V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Hashtable e::a
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [mscorlib]System.Collections.Hashtable e::a
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Remove(object)
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method e::b

  .method public hidebysig instance void 
          a(string A_0,
            object A_1,
            valuetype [mscorlib]System.DateTime A_2) cil managed
  {
    // 代码大小       190 (0xbe)
    .maxstack  3
    .locals init (class q V_0,
             class [mscorlib]System.Collections.Hashtable V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Hashtable e::a
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [mscorlib]System.Collections.Hashtable e::a
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
      IL_0019:  castclass  q
      IL_001e:  stloc.0
      IL_001f:  ldloc.0
      IL_0020:  brfalse.s  IL_0056

      IL_0022:  ldloc.0
      IL_0023:  callvirt   instance class q q::a()
      IL_0028:  brfalse.s  IL_003b

      IL_002a:  ldloc.0
      IL_002b:  callvirt   instance class q q::a()
      IL_0030:  ldloc.0
      IL_0031:  callvirt   instance class q q::c()
      IL_0036:  callvirt   instance void q::b(class q)
      IL_003b:  ldloc.0
      IL_003c:  callvirt   instance class q q::c()
      IL_0041:  brfalse.s  IL_0069

      IL_0043:  ldloc.0
      IL_0044:  callvirt   instance class q q::c()
      IL_0049:  ldloc.0
      IL_004a:  callvirt   instance class q q::a()
      IL_004f:  callvirt   instance void q::a(class q)
      IL_0054:  br.s       IL_0069

      IL_0056:  newobj     instance void q::.ctor()
      IL_005b:  stloc.0
      IL_005c:  ldarg.0
      IL_005d:  ldfld      class [mscorlib]System.Collections.Hashtable e::a
      IL_0062:  ldarg.1
      IL_0063:  ldloc.0
      IL_0064:  callvirt   instance void [mscorlib]System.Collections.Hashtable::set_Item(object,
                                                                                          object)
      IL_0069:  ldloc.0
      IL_006a:  ldarg.2
      IL_006b:  callvirt   instance void q::a(object)
      IL_0070:  ldloc.0
      IL_0071:  ldarg.1
      IL_0072:  callvirt   instance void q::a(string)
      IL_0077:  ldloc.0
      IL_0078:  ldarg.3
      IL_0079:  callvirt   instance void q::a(valuetype [mscorlib]System.DateTime)
      IL_007e:  ldarg.0
      IL_007f:  ldfld      class q e::b
      IL_0084:  brfalse.s  IL_009e

      IL_0086:  ldloc.0
      IL_0087:  ldarg.0
      IL_0088:  ldfld      class q e::b
      IL_008d:  callvirt   instance void q::a(class q)
      IL_0092:  ldarg.0
      IL_0093:  ldfld      class q e::b
      IL_0098:  ldloc.0
      IL_0099:  callvirt   instance void q::b(class q)
      IL_009e:  ldarg.0
      IL_009f:  ldloc.0
      IL_00a0:  stfld      class q e::b
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      class q e::c
      IL_00ab:  brtrue.s   IL_00b4

      IL_00ad:  ldarg.0
      IL_00ae:  ldloc.0
      IL_00af:  stfld      class q e::c
      IL_00b4:  leave.s    IL_00bd

    }  // end .try
    finally
    {
      IL_00b6:  ldloc.1
      IL_00b7:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_00bc:  endfinally
    }  // end handler
    IL_00bd:  ret
  } // end of method e::a

  .method public hidebysig instance object 
          a(string A_0) cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  2
    .locals init (class q V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Hashtable e::a
    IL_0006:  ldarg.1
    IL_0007:  callvirt   instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
    IL_000c:  castclass  q
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  brfalse.s  IL_001c

    IL_0015:  ldloc.0
    IL_0016:  callvirt   instance object q::f()
    IL_001b:  ret

    IL_001c:  ldnull
    IL_001d:  ret
  } // end of method e::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4     0x2000
    IL_0006:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32)
    IL_000b:  stfld      class [mscorlib]System.Collections.Hashtable e::a
    IL_0010:  ldarg.0
    IL_0011:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0016:  ret
  } // end of method e::.ctor

} // end of class e

.class private auto ansi beforefieldinit m
       extends [mscorlib]System.Object
{
  .field private static object a
  .field private static class [mscorlib]System.Collections.Hashtable b
  .method public hidebysig static void  a(int32 A_0) cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32)
    IL_0006:  stsfld     class [mscorlib]System.Collections.Hashtable m::b
    IL_000b:  ret
  } // end of method m::a

  .method public hidebysig static uint8[] 
          a(string A_0,
            class [mscorlib]System.Text.Encoding A_1,
            class ai A_2) cil managed
  {
    // 代码大小       114 (0x72)
    .maxstack  5
    .locals init (uint8[] V_0,
             class Beetle.ByteArraySegment V_1,
             object V_2)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Hashtable m::b
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
    IL_000b:  castclass  uint8[]
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  brtrue.s   IL_0070

    IL_0014:  ldsfld     object m::a
    IL_0019:  dup
    IL_001a:  stloc.2
    IL_001b:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0020:  ldsfld     class [mscorlib]System.Collections.Hashtable m::b
      IL_0025:  ldarg.0
      IL_0026:  callvirt   instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
      IL_002b:  castclass  uint8[]
      IL_0030:  stloc.0
      IL_0031:  ldloc.0
      IL_0032:  brtrue.s   IL_0067

      IL_0034:  ldnull
      IL_0035:  stloc.1
      IL_0036:  ldloc.1
      IL_0037:  ldfld      int32 Beetle.ByteArraySegment::Count
      IL_003c:  newarr     [mscorlib]System.Byte
      IL_0041:  stloc.0
      IL_0042:  ldloc.1
      IL_0043:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
      IL_0048:  ldloc.1
      IL_0049:  ldfld      int32 Beetle.ByteArraySegment::Offset
      IL_004e:  ldloc.0
      IL_004f:  ldc.i4.0
      IL_0050:  ldloc.1
      IL_0051:  ldfld      int32 Beetle.ByteArraySegment::Count
      IL_0056:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                   int32,
                                                                   class [mscorlib]System.Array,
                                                                   int32,
                                                                   int32)
      IL_005b:  ldsfld     class [mscorlib]System.Collections.Hashtable m::b
      IL_0060:  ldarg.0
      IL_0061:  ldloc.0
      IL_0062:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                     object)
      IL_0067:  leave.s    IL_0070

    }  // end .try
    finally
    {
      IL_0069:  ldloc.2
      IL_006a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_006f:  endfinally
    }  // end handler
    IL_0070:  ldloc.0
    IL_0071:  ret
  } // end of method m::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method m::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       26 (0x1a)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0005:  stsfld     object m::a
    IL_000a:  ldc.i4     0x2800
    IL_000f:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32)
    IL_0014:  stsfld     class [mscorlib]System.Collections.Hashtable m::b
    IL_0019:  ret
  } // end of method m::.cctor

} // end of class m

.class private auto ansi beforefieldinit t
       extends [mscorlib]System.Attribute
{
  .field private class [System]System.Collections.Generic.Queue`1<class a> a
  .field private class a b
  .field private bool c
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.Threading.WaitCallback V_0)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4     0x400
    IL_0008:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class a>::.ctor(int32)
    IL_000d:  stfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
    IL_0012:  ldarg.0
    IL_0013:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0018:  ldloc.0
    IL_0019:  brtrue.s   IL_0028

    IL_001b:  ldarg.0
    IL_001c:  ldftn      instance void t::a(object)
    IL_0022:  newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                      native int)
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback)
    IL_002e:  pop
    IL_002f:  ret
  } // end of method t::.ctor

  .method public hidebysig instance void 
          a(class a A_0) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Queue`1<class a> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class a>::Enqueue(!0)
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method t::a

  .method public hidebysig specialname instance int32 
          c() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class a>::get_Count()
    IL_000b:  ret
  } // end of method t::c

  .method family hidebysig instance class a 
          a() cil managed
  {
    // 代码大小       54 (0x36)
    .maxstack  2
    .locals init (class a V_0,
             class [System]System.Collections.Generic.Queue`1<class a> V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class a>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0029

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
      IL_0021:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class a>::Dequeue()
      IL_0026:  stloc.0
      IL_0027:  leave.s    IL_0034

      IL_0029:  ldnull
      IL_002a:  stloc.0
      IL_002b:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_002d:  ldloc.1
      IL_002e:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ldloc.0
    IL_0035:  ret
  } // end of method t::a

  .method family hidebysig newslot virtual 
          instance void  b() cil managed
  {
    // 代码大小       153 (0x99)
    .maxstack  4
    .locals init (class Beetle.TcpChannel V_0,
             class [mscorlib]System.Exception V_1,
             class Beetle.ChannelErrorEventArgs V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance class a t::a()
    IL_0007:  stfld      class a t::b
    IL_000c:  ldarg.0
    IL_000d:  ldfld      class a t::b
    IL_0012:  brfalse.s  IL_008e

    .try
    {
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class a t::b
      IL_001a:  callvirt   instance class Beetle.TcpChannel a::b()
      IL_001f:  stloc.0
      IL_0020:  ldloc.0
      IL_0021:  callvirt   instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
      IL_0026:  brfalse.s  IL_0089

      .try
      {
        .try
        {
          IL_0028:  ldloc.0
          IL_0029:  ldfld      class Beetle.ChannelReceiveEventArgs Beetle.TcpChannel::r
          IL_002e:  ldfld      class Beetle.ByteArraySegment Beetle.ChannelReceiveEventArgs::a
          IL_0033:  ldarg.0
          IL_0034:  ldfld      class a t::b
          IL_0039:  callvirt   instance uint8[] [System]System.Net.Sockets.SocketAsyncEventArgs::get_Buffer()
          IL_003e:  ldc.i4.0
          IL_003f:  ldarg.0
          IL_0040:  ldfld      class a t::b
          IL_0045:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
          IL_004a:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                              int32,
                                                                              int32)
          IL_004f:  ldloc.0
          IL_0050:  ldloc.0
          IL_0051:  ldfld      class Beetle.ChannelReceiveEventArgs Beetle.TcpChannel::r
          IL_0056:  callvirt   instance void Beetle.TcpChannel::a(class Beetle.ChannelReceiveEventArgs)
          IL_005b:  leave.s    IL_007b

        }  // end .try
        catch [mscorlib]System.Exception 
        {
          IL_005d:  stloc.1
          IL_005e:  ldloc.0
          IL_005f:  newobj     instance void Beetle.ChannelErrorEventArgs::.ctor()
          IL_0064:  stloc.2
          IL_0065:  ldloc.2
          IL_0066:  ldloc.0
          IL_0067:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
          IL_006c:  ldloc.2
          IL_006d:  ldloc.1
          IL_006e:  callvirt   instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
          IL_0073:  ldloc.2
          IL_0074:  callvirt   instance void Beetle.TcpChannel::CallChannelError(class Beetle.ChannelErrorEventArgs)
          IL_0079:  leave.s    IL_007b

        }  // end handler
        IL_007b:  leave.s    IL_0089

      }  // end .try
      finally
      {
        IL_007d:  ldarg.0
        IL_007e:  ldfld      class a t::b
        IL_0083:  callvirt   instance void a::d()
        IL_0088:  endfinally
      }  // end handler
      IL_0089:  leave.s    IL_0098

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_008b:  pop
      IL_008c:  leave.s    IL_0098

    }  // end handler
    IL_008e:  ldsfld     int32 Beetle.TcpUtils::SocketDespatchSleepTime
    IL_0093:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)
    IL_0098:  ret
  } // end of method t::b

  .method public hidebysig newslot virtual 
          instance void  d() cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  8
    IL_0000:  ldsfld     bool Beetle.TcpUtils::ExecutionContext
    IL_0005:  brtrue.s   IL_0015

    IL_0007:  call       valuetype [mscorlib]System.Threading.AsyncFlowControl [mscorlib]System.Threading.ExecutionContext::SuppressFlow()
    IL_000c:  pop
    IL_000d:  br.s       IL_0015

    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance void t::b()
    IL_0015:  ldarg.0
    IL_0016:  ldfld      bool t::c
    IL_001b:  brfalse.s  IL_000f

    IL_001d:  ret
  } // end of method t::d

  .method public hidebysig instance void 
          e() cil managed
  {
    // 代码大小       65 (0x41)
    .maxstack  2
    .locals init (class t V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool t::c
      IL_000e:  brtrue.s   IL_0037

      IL_0010:  br.s       IL_0022

      IL_0012:  ldarg.0
      IL_0013:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
      IL_0018:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class a>::Dequeue()
      IL_001d:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::Dispose()
      IL_0022:  ldarg.0
      IL_0023:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> t::a
      IL_0028:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class a>::get_Count()
      IL_002d:  ldc.i4.0
      IL_002e:  bgt.s      IL_0012

      IL_0030:  ldarg.0
      IL_0031:  ldc.i4.1
      IL_0032:  stfld      bool t::c
      IL_0037:  leave.s    IL_0040

    }  // end .try
    finally
    {
      IL_0039:  ldloc.0
      IL_003a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003f:  endfinally
    }  // end handler
    IL_0040:  ret
  } // end of method t::e

  .method private hidebysig instance void 
          a(object A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance void t::d()
    IL_0006:  ret
  } // end of method t::a

} // end of class t

.class private auto ansi beforefieldinit ab
       extends [mscorlib]System.Object
{
  .field static assembly string a
  .method public hidebysig static class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider 
          a() cil managed
  {
    .custom instance void af::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldc.i4     0x800
    IL_0005:  newobj     instance void [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::.ctor(int32)
    IL_000a:  ret
  } // end of method ab::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ab::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "ZpBMBet2b45vHtFqeU5lDeTdKI70zrINkgMhjlYl9lYFiTnxjO"
    + "V9OfT"
    IL_0005:  stsfld     string ab::a
    IL_000a:  ret
  } // end of method ab::.cctor

} // end of class ab

.class private auto ansi beforefieldinit af
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 40 00 00 00 00 00 )                         // ..@.....
  .field static assembly string a
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method af::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldstr      "zoBd/Cz/pdkTh2ylx0uLR2fPBQoxXaUg3BtYj83rpP3S/qMa0h"
    + "kFXw6ZSTY6U2MaLRbEFMxqJo/JCEZjZkCXWGR7s+l1yek6gLbx8nGaRl9xULqnOvM1j/zlW"
    + "/1iy5Uy8IJ1rZJgzpEINOmL1TFnqICCU8/KqyhUhcJYNtc6itm00GmEsEnOuAYUyTE1JoUF"
    + "h/woSlQnxIl9YVDtmDWvoErCkYOqloIktDr49H3Dmhad2PtjaUyV/UDr1pG7afvcP0hPg9B"
    + "fp0LaP5Hl4mdtvNxb2ueDZObTzFkxYqYNCevbhW8ObGC8MBSwnNx5VSclsPgf9IycbD4JfD"
    + "Gox52jt3D0I3pEnQ13Y0JWGCvBpXAWp9NXVFtOcRvSg3GteipY5h8LRVLXbQhhI+JNgDAnj"
    + "EN4V9knksdQZqCaKtaS8I5O/cK35aXf900Y4G/uNptitq9XO4ku1nWEBZMs8/WhRXDMiUV8"
    + "lmSmqp64/1QoHzUUujHsJZkLcSVB0MRjZUTXcv5S97li5Vks5azk4sppeyEbp4lXWJoz9nt"
    + "fkUbQSGqPB3/Zfq+pFi+W3F+bIf0GSFd6B8GarwFGQIzzDVFzCGpcjQ79VzpXOX9DIb4H8p"
    + "+mW++XFLdfysuBAY1Bw9BRMjty229EaUnmh4C"
    IL_0005:  stsfld     string af::a
    IL_000a:  ret
  } // end of method af::.cctor

} // end of class af

.class private auto ansi beforefieldinit x
       extends [mscorlib]System.Object
{
  .field private static object a
  .field private static class [Smark.Core]Smark.Core.DESCrypto b
  .method public hidebysig static bool  a(string A_0,
                                          string A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
    IL_000b:  ldarg.1
    IL_000c:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0011:  call       bool x::a(uint8[],
                                   uint8[])
    IL_0016:  ret
  } // end of method x::a

  .method public hidebysig static bool  a(uint8[] A_0,
                                          uint8[] A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldsfld     object x::a
    IL_0005:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_000a:  ldarg.0
    IL_000b:  ldstr      "MD5"
    IL_0010:  ldarg.1
    IL_0011:  callvirt   instance bool [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::VerifyData(uint8[],
                                                                                                                   object,
                                                                                                                   uint8[])
    IL_0016:  ret
  } // end of method x::a

  .method public hidebysig static string 
          c() cil managed
  {
    // 代码大小       77 (0x4d)
    .maxstack  3
    .locals init (class [mscorlib]System.Reflection.FieldInfo V_0,
             string V_1,
             class [mscorlib]System.Reflection.FieldInfo[] V_2,
             int32 V_3)
    IL_0000:  ldtoken    b
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ldc.i4.s   24
    IL_000c:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo[] [mscorlib]System.Type::GetFields(valuetype [mscorlib]System.Reflection.BindingFlags)
    IL_0011:  stloc.2
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.3
    IL_0014:  br.s       IL_0043

    IL_0016:  ldloc.2
    IL_0017:  ldloc.3
    IL_0018:  ldelem.ref
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldtoken    d
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldc.i4.0
    IL_0026:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                       bool)
    IL_002b:  ldlen
    IL_002c:  conv.i4
    IL_002d:  ldc.i4.0
    IL_002e:  ble.s      IL_003f

    IL_0030:  ldloc.0
    IL_0031:  ldnull
    IL_0032:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
    IL_0037:  castclass  [mscorlib]System.String
    IL_003c:  stloc.1
    IL_003d:  leave.s    IL_004b

    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.1
    IL_0041:  add
    IL_0042:  stloc.3
    IL_0043:  ldloc.3
    IL_0044:  ldloc.2
    IL_0045:  ldlen
    IL_0046:  conv.i4
    IL_0047:  blt.s      IL_0016

    IL_0049:  ldnull
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  ret
  } // end of method x::c

  .method public hidebysig static string 
          b() cil managed
  {
    // 代码大小       97 (0x61)
    .maxstack  3
    .locals init (class [mscorlib]System.Reflection.FieldInfo V_0,
             string V_1,
             class [mscorlib]System.Reflection.FieldInfo[] V_2,
             int32 V_3)
    IL_0000:  call       void x::a()
    IL_0005:  ldtoken    b
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.s   24
    IL_0011:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo[] [mscorlib]System.Type::GetFields(valuetype [mscorlib]System.Reflection.BindingFlags)
    IL_0016:  stloc.2
    IL_0017:  ldc.i4.0
    IL_0018:  stloc.3
    IL_0019:  br.s       IL_0057

    IL_001b:  ldloc.2
    IL_001c:  ldloc.3
    IL_001d:  ldelem.ref
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  ldtoken    u
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldc.i4.0
    IL_002b:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                       bool)
    IL_0030:  ldlen
    IL_0031:  conv.i4
    IL_0032:  ldc.i4.0
    IL_0033:  ble.s      IL_0053

    IL_0035:  ldloc.0
    IL_0036:  ldnull
    IL_0037:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
    IL_003c:  castclass  [mscorlib]System.String
    IL_0041:  ldsfld     string ab::a
    IL_0046:  ldsfld     string af::a
    IL_004b:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0050:  stloc.1
    IL_0051:  leave.s    IL_005f

    IL_0053:  ldloc.3
    IL_0054:  ldc.i4.1
    IL_0055:  add
    IL_0056:  stloc.3
    IL_0057:  ldloc.3
    IL_0058:  ldloc.2
    IL_0059:  ldlen
    IL_005a:  conv.i4
    IL_005b:  blt.s      IL_001b

    IL_005d:  ldnull
    IL_005e:  ret

    IL_005f:  ldloc.1
    IL_0060:  ret
  } // end of method x::b

  .method private hidebysig static void  a() cil managed
  {
    // 代码大小       50 (0x32)
    .maxstack  3
    .locals init (class [Smark.Core]Smark.Core.DESCrypto/DESInfo V_0)
    IL_0000:  ldsfld     class [Smark.Core]Smark.Core.DESCrypto x::b
    IL_0005:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldsfld     string w::i
    IL_0011:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0016:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_IV(uint8[])
    IL_001b:  ldloc.0
    IL_001c:  ldsfld     string r::c
    IL_0021:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0026:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_Key(uint8[])
    IL_002b:  ldloc.0
    IL_002c:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto::set_KeyInfo(class [Smark.Core]Smark.Core.DESCrypto/DESInfo)
    IL_0031:  ret
  } // end of method x::a

  .method public hidebysig static void  a([out] string& A_0,
                                          [out] string& A_1) cil managed
  {
    // 代码大小       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       string x::b()
    IL_0006:  stind.ref
    IL_0007:  ldarg.1
    IL_0008:  call       string x::c()
    IL_000d:  stind.ref
    IL_000e:  ldarg.0
    IL_000f:  ldsfld     class [Smark.Core]Smark.Core.DESCrypto x::b
    IL_0014:  ldarg.0
    IL_0015:  ldind.ref
    IL_0016:  callvirt   instance string [Smark.Core]Smark.Core.DESCrypto::Decrypt(string)
    IL_001b:  stind.ref
    IL_001c:  ldarg.1
    IL_001d:  ldsfld     class [Smark.Core]Smark.Core.DESCrypto x::b
    IL_0022:  ldarg.1
    IL_0023:  ldind.ref
    IL_0024:  callvirt   instance string [Smark.Core]Smark.Core.DESCrypto::Decrypt(string)
    IL_0029:  stind.ref
    IL_002a:  ret
  } // end of method x::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method x::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  stsfld     object x::a
    IL_0006:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto::.ctor()
    IL_000b:  stsfld     class [Smark.Core]Smark.Core.DESCrypto x::b
    IL_0010:  ret
  } // end of method x::.cctor

} // end of class x

.class private auto ansi w
       extends [mscorlib]System.Object
{
  .field public static valuetype [mscorlib]System.DateTime a
  .field public bool b
  .field public string c
  .field public string d
  .field public int32 e
  .field public valuetype [mscorlib]System.DateTime f
  .field private static class w g
  .field private static object h
  .field static assembly string i
  .field private static string[] j
  .field private static string[] k
  .field private static string l
  .field private static string m
  .field private static class [mscorlib]System.IO.StreamReader n
  .field private static string o
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       48 (0x30)
    .maxstack  8
    IL_0000:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0005:  stsfld     valuetype [mscorlib]System.DateTime w::a
    IL_000a:  ldnull
    IL_000b:  stsfld     class w w::g
    IL_0010:  ldnull
    IL_0011:  stsfld     object w::h
    IL_0016:  ldstr      "73HWrM6iCrk="
    IL_001b:  stsfld     string w::i
    IL_0020:  ldc.i4     0x800
    IL_0025:  newobj     instance void [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::.ctor(int32)
    IL_002a:  stsfld     object w::h
    IL_002f:  ret
  } // end of method w::.cctor

  .method public hidebysig static uint8[] 
          g(uint8[] A_0) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldsfld     object w::h
    IL_0005:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance uint8[] [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::Encrypt(uint8[],
                                                                                                                   bool)
    IL_0011:  ret
  } // end of method w::g

  .method public hidebysig static uint8[] 
          a(uint8[] A_0) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldsfld     object w::h
    IL_0005:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance uint8[] [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::Decrypt(uint8[],
                                                                                                                   bool)
    IL_0011:  ret
  } // end of method w::a

  .method public hidebysig static bool  a(string A_0,
                                          string A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
    IL_000b:  ldarg.1
    IL_000c:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0011:  call       bool w::a(uint8[],
                                   uint8[])
    IL_0016:  ret
  } // end of method w::a

  .method public hidebysig static bool  a(uint8[] A_0,
                                          uint8[] A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldsfld     object w::h
    IL_0005:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_000a:  ldarg.0
    IL_000b:  ldstr      "MD5"
    IL_0010:  ldarg.1
    IL_0011:  callvirt   instance bool [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::VerifyData(uint8[],
                                                                                                                   object,
                                                                                                                   uint8[])
    IL_0016:  ret
  } // end of method w::a

  .method private hidebysig static void  i(string A_0) cil managed
  {
    // 代码大小       31 (0x1f)
    .maxstack  4
    .locals init (char[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     [mscorlib]System.Char
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.s   13
    IL_000c:  stelem.i2
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  ldc.i4.s   10
    IL_0011:  stelem.i2
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.1
    IL_0014:  callvirt   instance string[] [mscorlib]System.String::Split(char[],
                                                                          valuetype [mscorlib]System.StringSplitOptions)
    IL_0019:  stsfld     string[] w::j
    IL_001e:  ret
  } // end of method w::i

  .method private hidebysig static void  h(string A_0) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  4
    .locals init (char[] V_0)
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_000b:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
    IL_0010:  ldc.i4.1
    IL_0011:  newarr     [mscorlib]System.Char
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldc.i4.s   59
    IL_001b:  stelem.i2
    IL_001c:  ldloc.0
    IL_001d:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
    IL_0022:  stsfld     string[] w::k
    IL_0027:  ret
  } // end of method w::h

  .method private hidebysig static void  j() cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  8
    IL_0000:  ldsfld     class w w::g
    IL_0005:  ldsfld     string[] w::k
    IL_000a:  ldc.i4.0
    IL_000b:  ldelem.ref
    IL_000c:  stfld      string w::c
    IL_0011:  ldsfld     class w w::g
    IL_0016:  ldsfld     string[] w::k
    IL_001b:  ldc.i4.1
    IL_001c:  ldelem.ref
    IL_001d:  stfld      string w::d
    IL_0022:  ret
  } // end of method w::j

  .method private hidebysig static void  i() cil managed
  {
    // 代码大小       92 (0x5c)
    .maxstack  3
    IL_0000:  ldsfld     class w w::g
    IL_0005:  ldsfld     string[] w::k
    IL_000a:  ldc.i4.2
    IL_000b:  ldelem.ref
    IL_000c:  call       int32 [mscorlib]System.Int32::Parse(string)
    IL_0011:  stfld      int32 w::e
    IL_0016:  ldsfld     string[] w::k
    IL_001b:  ldc.i4.2
    IL_001c:  ldelem.ref
    IL_001d:  call       int32 [mscorlib]System.Int32::Parse(string)
    IL_0022:  stsfld     int32 l::a
    IL_0027:  ldsfld     class w w::g
    IL_002c:  ldsfld     string[] w::k
    IL_0031:  ldc.i4.3
    IL_0032:  ldelem.ref
    IL_0033:  call       int64 [mscorlib]System.Int64::Parse(string)
    IL_0038:  newobj     instance void [mscorlib]System.DateTime::.ctor(int64)
    IL_003d:  stfld      valuetype [mscorlib]System.DateTime w::f
    IL_0042:  ldsfld     class w w::g
    IL_0047:  ldfld      valuetype [mscorlib]System.DateTime w::f
    IL_004c:  ldsfld     valuetype [mscorlib]System.DateTime w::a
    IL_0051:  call       bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                        valuetype [mscorlib]System.DateTime)
    IL_0056:  stsfld     bool b::d
    IL_005b:  ret
  } // end of method w::i

  .method private hidebysig static void  a(string A_0,
                                           class w A_1) cil managed
  {
    // 代码大小       72 (0x48)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  call       void w::i(string)
    IL_0006:  ldsfld     string[] w::j
    IL_000b:  ldc.i4.0
    IL_000c:  ldelem.ref
    IL_000d:  ldsfld     string[] w::j
    IL_0012:  ldc.i4.1
    IL_0013:  ldelem.ref
    IL_0014:  call       bool w::a(string,
                                   string)
    IL_0019:  brfalse.s  IL_0047

    IL_001b:  ldsfld     string[] w::j
    IL_0020:  ldc.i4.0
    IL_0021:  ldelem.ref
    IL_0022:  call       void w::h(string)
    IL_0027:  call       void w::j()
    IL_002c:  call       void w::i()
    IL_0031:  ldarg.1
    IL_0032:  ldarg.1
    IL_0033:  ldfld      valuetype [mscorlib]System.DateTime w::f
    IL_0038:  ldsfld     valuetype [mscorlib]System.DateTime w::a
    IL_003d:  call       bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                        valuetype [mscorlib]System.DateTime)
    IL_0042:  stfld      bool w::b
    IL_0047:  ret
  } // end of method w::a

  .method private hidebysig static void  h() cil managed
  {
    // 代码大小       16 (0x10)
    .maxstack  8
    IL_0000:  ldsfld     string w::o
    IL_0005:  newobj     instance void [mscorlib]System.IO.StreamReader::.ctor(string)
    IL_000a:  stsfld     class [mscorlib]System.IO.StreamReader w::n
    IL_000f:  ret
  } // end of method w::h

  .method private hidebysig static void  g(string A_0) cil managed
  {
    // 代码大小       51 (0x33)
    .maxstack  8
    IL_0000:  ldtoken    w
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  callvirt   instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()
    IL_000f:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_Location()
    IL_0014:  call       string [mscorlib]System.IO.Path::GetDirectoryName(string)
    IL_0019:  ldsfld     char [mscorlib]System.IO.Path::DirectorySeparatorChar
    IL_001e:  box        [mscorlib]System.Char
    IL_0023:  ldsfld     string w::l
    IL_0028:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_002d:  stsfld     string w::o
    IL_0032:  ret
  } // end of method w::g

  .method private hidebysig static void  a(string A_0) cil managed
  {
    // 代码大小       21 (0x15)
    .maxstack  8
    IL_0000:  ldsfld     object w::h
    IL_0005:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_000a:  ldsfld     string w::m
    IL_000f:  callvirt   instance void [mscorlib]System.Security.Cryptography.AsymmetricAlgorithm::FromXmlString(string)
    IL_0014:  ret
  } // end of method w::a

  .method public hidebysig static class w 
          g() cil managed
  {
    // 代码大小       143 (0x8f)
    .maxstack  2
    .locals init (class [mscorlib]System.Type V_0,
             class [mscorlib]System.IO.StreamReader V_1)
    IL_0000:  ldtoken    w
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  dup
    IL_000b:  stloc.0
    IL_000c:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0011:  ldsfld     class w w::g
      IL_0016:  brtrue.s   IL_0071

      IL_0018:  newobj     instance void w::.ctor()
      IL_001d:  stsfld     class w w::g
      .try
      {
        IL_0022:  ldsflda    string w::m
        IL_0027:  ldsflda    string w::l
        IL_002c:  call       void x::a(string&,
                                       string&)
        IL_0031:  ldnull
        IL_0032:  call       void w::a(string)
        IL_0037:  ldsfld     string w::m
        IL_003c:  call       void w::g(string)
        IL_0041:  call       void w::h()
        IL_0046:  ldsfld     class [mscorlib]System.IO.StreamReader w::n
        IL_004b:  stloc.1
        .try
        {
          IL_004c:  ldsfld     class [mscorlib]System.IO.StreamReader w::n
          IL_0051:  callvirt   instance string [mscorlib]System.IO.TextReader::ReadToEnd()
          IL_0056:  ldsfld     class w w::g
          IL_005b:  call       void w::a(string,
                                         class w)
          IL_0060:  leave.s    IL_006c

        }  // end .try
        finally
        {
          IL_0062:  ldloc.1
          IL_0063:  brfalse.s  IL_006b

          IL_0065:  ldloc.1
          IL_0066:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          IL_006b:  endfinally
        }  // end handler
        IL_006c:  leave.s    IL_0071

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_006e:  pop
        IL_006f:  leave.s    IL_0071

      }  // end handler
      IL_0071:  leave.s    IL_007a

    }  // end .try
    finally
    {
      IL_0073:  ldloc.0
      IL_0074:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0079:  endfinally
    }  // end handler
    IL_007a:  ldsfld     class w w::g
    IL_007f:  ldfld      bool w::b
    IL_0084:  stsfld     bool b::c
    IL_0089:  ldsfld     class w w::g
    IL_008e:  ret
  } // end of method w::g

  .method private hidebysig instance int32 
          a() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 w::e
    IL_0006:  ret
  } // end of method w::a

  .method public hidebysig instance bool 
          a(class [mscorlib]System.Action`1<class r> A_0,
            class r A_1,
            int32 A_2,
            [out] int32& A_3) cil managed
  {
    // 代码大小       32 (0x20)
    .maxstack  8
    IL_0000:  ldarg.s    A_3
    IL_0002:  ldarg.0
    IL_0003:  call       instance int32 w::a()
    IL_0008:  stind.i4
    IL_0009:  ldarg.0
    IL_000a:  ldfld      bool w::b
    IL_000f:  brfalse.s  IL_001e

    IL_0011:  ldarg.3
    IL_0012:  ldarg.0
    IL_0013:  ldfld      int32 w::e
    IL_0018:  cgt
    IL_001a:  ldc.i4.0
    IL_001b:  ceq
    IL_001d:  ret

    IL_001e:  ldc.i4.0
    IL_001f:  ret
  } // end of method w::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method w::.ctor

} // end of class w

.class private auto ansi beforefieldinit aa
       extends [mscorlib]System.Object
{
  .method assembly hidebysig static object 
          c() cil managed
  {
    // 代码大小       73 (0x49)
    .maxstack  3
    .locals init (class [mscorlib]System.Reflection.MethodInfo V_0,
             object V_1,
             class [mscorlib]System.Reflection.MethodInfo[] V_2,
             int32 V_3)
    IL_0000:  ldtoken    ab
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ldc.i4.s   24
    IL_000c:  callvirt   instance class [mscorlib]System.Reflection.MethodInfo[] [mscorlib]System.Type::GetMethods(valuetype [mscorlib]System.Reflection.BindingFlags)
    IL_0011:  stloc.2
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.3
    IL_0014:  br.s       IL_003f

    IL_0016:  ldloc.2
    IL_0017:  ldloc.3
    IL_0018:  ldelem.ref
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldtoken    af
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldc.i4.0
    IL_0026:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                       bool)
    IL_002b:  ldlen
    IL_002c:  conv.i4
    IL_002d:  ldc.i4.0
    IL_002e:  ble.s      IL_003b

    IL_0030:  ldloc.0
    IL_0031:  ldnull
    IL_0032:  ldnull
    IL_0033:  callvirt   instance object [mscorlib]System.Reflection.MethodBase::Invoke(object,
                                                                                        object[])
    IL_0038:  stloc.1
    IL_0039:  leave.s    IL_0047

    IL_003b:  ldloc.3
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  stloc.3
    IL_003f:  ldloc.3
    IL_0040:  ldloc.2
    IL_0041:  ldlen
    IL_0042:  conv.i4
    IL_0043:  blt.s      IL_0016

    IL_0045:  ldnull
    IL_0046:  ret

    IL_0047:  ldloc.1
    IL_0048:  ret
  } // end of method aa::c

  .method assembly hidebysig static string 
          b() cil managed
  {
    // 代码大小       77 (0x4d)
    .maxstack  3
    .locals init (class [mscorlib]System.Reflection.FieldInfo V_0,
             string V_1,
             class [mscorlib]System.Reflection.FieldInfo[] V_2,
             int32 V_3)
    IL_0000:  ldtoken    ag
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ldc.i4.s   24
    IL_000c:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo[] [mscorlib]System.Type::GetFields(valuetype [mscorlib]System.Reflection.BindingFlags)
    IL_0011:  stloc.2
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.3
    IL_0014:  br.s       IL_0043

    IL_0016:  ldloc.2
    IL_0017:  ldloc.3
    IL_0018:  ldelem.ref
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldtoken    t
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldc.i4.0
    IL_0026:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                       bool)
    IL_002b:  ldlen
    IL_002c:  conv.i4
    IL_002d:  ldc.i4.0
    IL_002e:  ble.s      IL_003f

    IL_0030:  ldloc.0
    IL_0031:  ldnull
    IL_0032:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
    IL_0037:  castclass  [mscorlib]System.String
    IL_003c:  stloc.1
    IL_003d:  leave.s    IL_004b

    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.1
    IL_0041:  add
    IL_0042:  stloc.3
    IL_0043:  ldloc.3
    IL_0044:  ldloc.2
    IL_0045:  ldlen
    IL_0046:  conv.i4
    IL_0047:  blt.s      IL_0016

    IL_0049:  ldnull
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  ret
  } // end of method aa::b

  .method assembly hidebysig static string 
          a() cil managed
  {
    // 代码大小       77 (0x4d)
    .maxstack  3
    .locals init (class [mscorlib]System.Reflection.FieldInfo V_0,
             string V_1,
             class [mscorlib]System.Reflection.FieldInfo[] V_2,
             int32 V_3)
    IL_0000:  ldtoken    ag
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ldc.i4.s   24
    IL_000c:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo[] [mscorlib]System.Type::GetFields(valuetype [mscorlib]System.Reflection.BindingFlags)
    IL_0011:  stloc.2
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.3
    IL_0014:  br.s       IL_0043

    IL_0016:  ldloc.2
    IL_0017:  ldloc.3
    IL_0018:  ldelem.ref
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldtoken    b
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldc.i4.0
    IL_0026:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                       bool)
    IL_002b:  ldlen
    IL_002c:  conv.i4
    IL_002d:  ldc.i4.0
    IL_002e:  ble.s      IL_003f

    IL_0030:  ldloc.0
    IL_0031:  ldnull
    IL_0032:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
    IL_0037:  castclass  [mscorlib]System.String
    IL_003c:  stloc.1
    IL_003d:  leave.s    IL_004b

    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.1
    IL_0041:  add
    IL_0042:  stloc.3
    IL_0043:  ldloc.3
    IL_0044:  ldloc.2
    IL_0045:  ldlen
    IL_0046:  conv.i4
    IL_0047:  blt.s      IL_0016

    IL_0049:  ldnull
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  ret
  } // end of method aa::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method aa::.ctor

} // end of class aa

.class private auto ansi ae
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit a
         extends [mscorlib]System.Object
  {
    .field public string a
    .field public string b
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // 代码大小       29 (0x1d)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  call       string aa::a()
      IL_000c:  stfld      string ae/a::a
      IL_0011:  ldarg.0
      IL_0012:  call       string aa::b()
      IL_0017:  stfld      string ae/a::b
      IL_001c:  ret
    } // end of method a::.ctor

    .method public hidebysig specialname 
            instance string  c() cil managed
    {
      // 代码大小       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ae/a::a
      IL_0006:  call       string ae::f(string)
      IL_000b:  ret
    } // end of method a::c

    .method public hidebysig specialname 
            instance string  d() cil managed
    {
      // 代码大小       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ae/a::b
      IL_0006:  call       string ae::f(string)
      IL_000b:  ret
    } // end of method a::d

  } // end of class a

  .field public bool a
  .field public string b
  .field public string c
  .field public int32 d
  .field public valuetype [mscorlib]System.DateTime e
  .field private static class ae f
  .field private static class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider g
  .field private static string h
  .field private static string i
  .field private static class ae/a j
  .field private static string[] k
  .field private static string[] l
  .field private static valuetype [mscorlib]System.DateTime m
  .field private static char[] n
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       71 (0x47)
    .maxstack  3
    .locals init (char[] V_0)
    IL_0000:  ldnull
    IL_0001:  stsfld     class ae ae::f
    IL_0006:  ldnull
    IL_0007:  stsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider ae::g
    IL_000c:  newobj     instance void ae/a::.ctor()
    IL_0011:  stsfld     class ae/a ae::j
    IL_0016:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_001b:  stsfld     valuetype [mscorlib]System.DateTime ae::m
    IL_0020:  ldc.i4.2
    IL_0021:  newarr     [mscorlib]System.Char
    IL_0026:  stloc.0
    IL_0027:  ldloc.0
    IL_0028:  ldc.i4.0
    IL_0029:  ldc.i4.s   13
    IL_002b:  stelem.i2
    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.1
    IL_002e:  ldc.i4.s   10
    IL_0030:  stelem.i2
    IL_0031:  ldloc.0
    IL_0032:  stsfld     char[] ae::n
    IL_0037:  call       object aa::c()
    IL_003c:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_0041:  stsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider ae::g
    IL_0046:  ret
  } // end of method ae::.cctor

  .method public hidebysig static bool  f(string A_0,
                                          string A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
    IL_000b:  ldarg.1
    IL_000c:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0011:  call       bool ae::f(uint8[],
                                    uint8[])
    IL_0016:  ret
  } // end of method ae::f

  .method public hidebysig static bool  f(uint8[] A_0,
                                          uint8[] A_1) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider ae::g
    IL_0005:  ldarg.0
    IL_0006:  ldstr      "MD5"
    IL_000b:  ldarg.1
    IL_000c:  callvirt   instance bool [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::VerifyData(uint8[],
                                                                                                                   object,
                                                                                                                   uint8[])
    IL_0011:  ret
  } // end of method ae::f

  .method public hidebysig static string 
          h(string A_0) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_000b:  call       uint8[] ae::f(uint8[])
    IL_0010:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
    IL_0015:  ret
  } // end of method ae::h

  .method public hidebysig static uint8[] 
          f(uint8[] A_0) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider ae::g
    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.0
    IL_0007:  callvirt   instance uint8[] [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::Decrypt(uint8[],
                                                                                                                   bool)
    IL_000c:  ret
  } // end of method ae::f

  .method private hidebysig static void  g(string A_0) cil managed
  {
    // 代码大小       146 (0x92)
    .maxstack  4
    .locals init (char[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldsfld     char[] ae::n
    IL_0006:  ldc.i4.1
    IL_0007:  callvirt   instance string[] [mscorlib]System.String::Split(char[],
                                                                          valuetype [mscorlib]System.StringSplitOptions)
    IL_000c:  stsfld     string[] ae::k
    IL_0011:  ldsfld     string[] ae::k
    IL_0016:  ldc.i4.0
    IL_0017:  ldelem.ref
    IL_0018:  ldsfld     string[] ae::k
    IL_001d:  ldc.i4.1
    IL_001e:  ldelem.ref
    IL_001f:  call       bool ae::f(string,
                                    string)
    IL_0024:  brfalse.s  IL_0091

    IL_0026:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_002b:  ldsfld     string[] ae::k
    IL_0030:  ldc.i4.0
    IL_0031:  ldelem.ref
    IL_0032:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0037:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
    IL_003c:  ldc.i4.1
    IL_003d:  newarr     [mscorlib]System.Char
    IL_0042:  stloc.0
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.0
    IL_0045:  ldc.i4.s   59
    IL_0047:  stelem.i2
    IL_0048:  ldloc.0
    IL_0049:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
    IL_004e:  stsfld     string[] ae::l
    IL_0053:  call       void ae::g()
    IL_0058:  ldsfld     class ae ae::f
    IL_005d:  ldsfld     string[] ae::l
    IL_0062:  ldc.i4.3
    IL_0063:  ldelem.ref
    IL_0064:  call       int64 [mscorlib]System.Int64::Parse(string)
    IL_0069:  newobj     instance void [mscorlib]System.DateTime::.ctor(int64)
    IL_006e:  stfld      valuetype [mscorlib]System.DateTime ae::e
    IL_0073:  ldsfld     class ae ae::f
    IL_0078:  ldsfld     class ae ae::f
    IL_007d:  ldfld      valuetype [mscorlib]System.DateTime ae::e
    IL_0082:  ldsfld     valuetype [mscorlib]System.DateTime ae::m
    IL_0087:  call       bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                        valuetype [mscorlib]System.DateTime)
    IL_008c:  stfld      bool ae::a
    IL_0091:  ret
  } // end of method ae::g

  .method private hidebysig static void  g() cil managed
  {
    // 代码大小       57 (0x39)
    .maxstack  8
    IL_0000:  ldsfld     class ae ae::f
    IL_0005:  ldsfld     string[] ae::l
    IL_000a:  ldc.i4.0
    IL_000b:  ldelem.ref
    IL_000c:  stfld      string ae::b
    IL_0011:  ldsfld     class ae ae::f
    IL_0016:  ldsfld     string[] ae::l
    IL_001b:  ldc.i4.1
    IL_001c:  ldelem.ref
    IL_001d:  stfld      string ae::c
    IL_0022:  ldsfld     class ae ae::f
    IL_0027:  ldsfld     string[] ae::l
    IL_002c:  ldc.i4.2
    IL_002d:  ldelem.ref
    IL_002e:  call       int32 [mscorlib]System.Int32::Parse(string)
    IL_0033:  stfld      int32 ae::d
    IL_0038:  ret
  } // end of method ae::g

  .method public hidebysig static class ae 
          f() cil managed
  {
    // 代码大小       170 (0xaa)
    .maxstack  3
    .locals init (string V_0,
             class [mscorlib]System.IO.StreamReader V_1,
             class [mscorlib]System.Type V_2)
    IL_0000:  ldtoken    ae
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  dup
    IL_000b:  stloc.2
    IL_000c:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0011:  ldsfld     class ae ae::f
      IL_0016:  brtrue.s   IL_008c

      IL_0018:  newobj     instance void ae::.ctor()
      IL_001d:  stsfld     class ae ae::f
      .try
      {
        IL_0022:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider ae::g
        IL_0027:  ldsfld     class ae/a ae::j
        IL_002c:  callvirt   instance string ae/a::c()
        IL_0031:  callvirt   instance void [mscorlib]System.Security.Cryptography.AsymmetricAlgorithm::FromXmlString(string)
        IL_0036:  ldtoken    w
        IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_0040:  callvirt   instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()
        IL_0045:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_Location()
        IL_004a:  call       string [mscorlib]System.IO.Path::GetDirectoryName(string)
        IL_004f:  ldsfld     char [mscorlib]System.IO.Path::DirectorySeparatorChar
        IL_0054:  box        [mscorlib]System.Char
        IL_0059:  ldsfld     class ae/a ae::j
        IL_005e:  callvirt   instance string ae/a::d()
        IL_0063:  call       string [mscorlib]System.String::Concat(object,
                                                                    object,
                                                                    object)
        IL_0068:  stloc.0
        IL_0069:  ldloc.0
        IL_006a:  newobj     instance void [mscorlib]System.IO.StreamReader::.ctor(string)
        IL_006f:  stloc.1
        .try
        {
          IL_0070:  ldloc.1
          IL_0071:  callvirt   instance string [mscorlib]System.IO.TextReader::ReadToEnd()
          IL_0076:  call       void ae::g(string)
          IL_007b:  leave.s    IL_0087

        }  // end .try
        finally
        {
          IL_007d:  ldloc.1
          IL_007e:  brfalse.s  IL_0086

          IL_0080:  ldloc.1
          IL_0081:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          IL_0086:  endfinally
        }  // end handler
        IL_0087:  leave.s    IL_008c

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0089:  pop
        IL_008a:  leave.s    IL_008c

      }  // end handler
      IL_008c:  leave.s    IL_0095

    }  // end .try
    finally
    {
      IL_008e:  ldloc.2
      IL_008f:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0094:  endfinally
    }  // end handler
    IL_0095:  ldsfld     class ae ae::f
    IL_009a:  ldfld      bool ae::a
    IL_009f:  stsfld     bool b::c
    IL_00a4:  ldsfld     class ae ae::f
    IL_00a9:  ret
  } // end of method ae::f

  .method assembly hidebysig static string 
          f(string A_0) cil managed
  {
    // 代码大小       59 (0x3b)
    .maxstack  3
    .locals init (class [Smark.Core]Smark.Core.DESCrypto V_0,
             class [Smark.Core]Smark.Core.DESCrypto/DESInfo V_1)
    IL_0000:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::.ctor()
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldsfld     string Beetle.TcpServer::g
    IL_0013:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0018:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_IV(uint8[])
    IL_001d:  ldloc.1
    IL_001e:  ldsfld     string Beetle.TcpChannel::k
    IL_0023:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0028:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_Key(uint8[])
    IL_002d:  ldloc.1
    IL_002e:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto::set_KeyInfo(class [Smark.Core]Smark.Core.DESCrypto/DESInfo)
    IL_0033:  ldloc.0
    IL_0034:  ldarg.0
    IL_0035:  callvirt   instance string [Smark.Core]Smark.Core.DESCrypto::Decrypt(string)
    IL_003a:  ret
  } // end of method ae::f

  .method public hidebysig instance bool 
          f(int32 A_0) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool ae::a
    IL_0006:  brfalse.s  IL_0015

    IL_0008:  ldarg.1
    IL_0009:  ldarg.0
    IL_000a:  ldfld      int32 ae::d
    IL_000f:  cgt
    IL_0011:  ldc.i4.0
    IL_0012:  ceq
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } // end of method ae::f

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ae::.ctor

} // end of class ae

.class private auto ansi j
       extends [mscorlib]System.Object
{
  .field private static literal string a = "/qMa0hkFXw6ZSTY6U2MaLRbEFMxqJo/JCEZjZkCXWGR7s+l1ye"
  + "k6gLbx8nGaRl9xULqnOvM1j/zlW/1iy5Uy8IJ1rZJgzpEINOmL1TFnqICCU8/KqyhUhcJYNtc6itm00GmEsEnOuAYUy"
  + "TE1JoUFh/woSlQnxIl9YVDtmDWvoErCkYOqloIktDr49H3Dmhad2PtjaUyV/UDr1pG7afvcP0hPg9Bfp0LaP5Hl4mdt"
  + "vNxb2ueDZObTzFkxYqYNCevbhW8ObGC8MBSwnNx5VSclsPgf9IycbD4JfDGox52jt3D0I3pEnQ13Y0JWGCvBpXAWp9N"
  + "XVFtOcRvSg3GteipY5h8LRVLXbQhhI+JNgDAnjEN4V9knksdQZqCaKtaS8I5O/cK35aXf900Y4G/uNptitq9XO4ku1n"
  + "WEBZMs8/WhRXDMiUV8lmSmqp64/1QoHzUUujHsJZkLcSVB0MRjZUTXcv5S97li5Vks5azk4sppeyEbp4lXWJoz9ntfk"
  + "UbQSGqPB3/Zfq+pFi+W3F+bIf0GSFd6B8GarwFGQIzzDVFzCGpcjQ79VzpXOX9DIb4H8p+mW++XFLdfysuBAY1Bw9BR"
  + "Mjty229EaUnmh4C"
  .field static assembly string b
  .field private static class [Smark.Core]Smark.Core.DESCrypto c
  .field static assembly valuetype [mscorlib]System.DateTime d
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       80 (0x50)
    .maxstack  3
    .locals init (class [Smark.Core]Smark.Core.DESCrypto/DESInfo V_0)
    IL_0000:  ldstr      "73HWrM6iCrk="
    IL_0005:  stsfld     string j::b
    IL_000a:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto::.ctor()
    IL_000f:  stsfld     class [Smark.Core]Smark.Core.DESCrypto j::c
    IL_0014:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0019:  stsfld     valuetype [mscorlib]System.DateTime j::d
    IL_001e:  ldsfld     class [Smark.Core]Smark.Core.DESCrypto j::c
    IL_0023:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::.ctor()
    IL_0028:  stloc.0
    IL_0029:  ldloc.0
    IL_002a:  ldsfld     string j::b
    IL_002f:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0034:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_IV(uint8[])
    IL_0039:  ldloc.0
    IL_003a:  ldsfld     string z::h
    IL_003f:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0044:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_Key(uint8[])
    IL_0049:  ldloc.0
    IL_004a:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto::set_KeyInfo(class [Smark.Core]Smark.Core.DESCrypto/DESInfo)
    IL_004f:  ret
  } // end of method j::.cctor

  .method assembly hidebysig static void 
          b([out] string& A_0) cil managed
  {
    // 代码大小       85 (0x55)
    .maxstack  3
    .locals init (class [mscorlib]System.Reflection.FieldInfo V_0,
             class [mscorlib]System.Reflection.FieldInfo[] V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  stind.ref
    IL_0003:  ldtoken    p
    IL_0008:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000d:  ldc.i4.s   24
    IL_000f:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo[] [mscorlib]System.Type::GetFields(valuetype [mscorlib]System.Reflection.BindingFlags)
    IL_0014:  stloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  stloc.2
    IL_0017:  br.s       IL_0045

    IL_0019:  ldloc.1
    IL_001a:  ldloc.2
    IL_001b:  ldelem.ref
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldtoken    ai
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  ldc.i4.0
    IL_0029:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                       bool)
    IL_002e:  ldlen
    IL_002f:  conv.i4
    IL_0030:  ldc.i4.0
    IL_0031:  ble.s      IL_0041

    IL_0033:  ldarg.0
    IL_0034:  ldloc.0
    IL_0035:  ldnull
    IL_0036:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
    IL_003b:  castclass  [mscorlib]System.String
    IL_0040:  stind.ref
    IL_0041:  ldloc.2
    IL_0042:  ldc.i4.1
    IL_0043:  add
    IL_0044:  stloc.2
    IL_0045:  ldloc.2
    IL_0046:  ldloc.1
    IL_0047:  ldlen
    IL_0048:  conv.i4
    IL_0049:  blt.s      IL_0019

    IL_004b:  ldarg.0
    IL_004c:  ldarg.0
    IL_004d:  ldind.ref
    IL_004e:  call       string j::b(string)
    IL_0053:  stind.ref
    IL_0054:  ret
  } // end of method j::b

  .method assembly hidebysig static void 
          a([out] string& A_0) cil managed
  {
    // 代码大小       107 (0x6b)
    .maxstack  3
    .locals init (class [mscorlib]System.Reflection.FieldInfo V_0,
             class [mscorlib]System.Reflection.FieldInfo[] V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  stind.ref
    IL_0003:  ldtoken    p
    IL_0008:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000d:  ldc.i4.s   24
    IL_000f:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo[] [mscorlib]System.Type::GetFields(valuetype [mscorlib]System.Reflection.BindingFlags)
    IL_0014:  stloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  stloc.2
    IL_0017:  br.s       IL_0045

    IL_0019:  ldloc.1
    IL_001a:  ldloc.2
    IL_001b:  ldelem.ref
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldtoken    k
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  ldc.i4.0
    IL_0029:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                       bool)
    IL_002e:  ldlen
    IL_002f:  conv.i4
    IL_0030:  ldc.i4.0
    IL_0031:  ble.s      IL_0041

    IL_0033:  ldarg.0
    IL_0034:  ldloc.0
    IL_0035:  ldnull
    IL_0036:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
    IL_003b:  castclass  [mscorlib]System.String
    IL_0040:  stind.ref
    IL_0041:  ldloc.2
    IL_0042:  ldc.i4.1
    IL_0043:  add
    IL_0044:  stloc.2
    IL_0045:  ldloc.2
    IL_0046:  ldloc.1
    IL_0047:  ldlen
    IL_0048:  conv.i4
    IL_0049:  blt.s      IL_0019

    IL_004b:  ldarg.0
    IL_004c:  ldarg.0
    IL_004d:  ldind.ref
    IL_004e:  ldstr      "/qMa0hkFXw6ZSTY6U2MaLRbEFMxqJo/JCEZjZkCXWGR7s+l1ye"
    + "k6gLbx8nGaRl9xULqnOvM1j/zlW/1iy5Uy8IJ1rZJgzpEINOmL1TFnqICCU8/KqyhUhcJYN"
    + "tc6itm00GmEsEnOuAYUyTE1JoUFh/woSlQnxIl9YVDtmDWvoErCkYOqloIktDr49H3Dmhad"
    + "2PtjaUyV/UDr1pG7afvcP0hPg9Bfp0LaP5Hl4mdtvNxb2ueDZObTzFkxYqYNCevbhW8ObGC"
    + "8MBSwnNx5VSclsPgf9IycbD4JfDGox52jt3D0I3pEnQ13Y0JWGCvBpXAWp9NXVFtOcRvSg3"
    + "GteipY5h8LRVLXbQhhI+JNgDAnjEN4V9knksdQZqCaKtaS8I5O/cK35aXf900Y4G/uNptit"
    + "q9XO4ku1nWEBZMs8/WhRXDMiUV8lmSmqp64/1QoHzUUujHsJZkLcSVB0MRjZUTXcv5S97li"
    + "5Vks5azk4sppeyEbp4lXWJoz9ntfkUbQSGqPB3/Zfq+pFi+W3F+bIf0GSFd6B8GarwFGQIz"
    + "zDVFzCGpcjQ79VzpXOX9DIb4H8p+mW++XFLdfysuBAY1Bw9BRMjty229EaUnmh4C"
    IL_0053:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0058:  call       string j::b(string)
    IL_005d:  stind.ref
    IL_005e:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider o::a
    IL_0063:  ldarg.0
    IL_0064:  ldind.ref
    IL_0065:  callvirt   instance void [mscorlib]System.Security.Cryptography.AsymmetricAlgorithm::FromXmlString(string)
    IL_006a:  ret
  } // end of method j::a

  .method assembly hidebysig static string 
          b(string A_0) cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldsfld     class [Smark.Core]Smark.Core.DESCrypto j::c
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance string [Smark.Core]Smark.Core.DESCrypto::Decrypt(string)
    IL_000b:  ret
  } // end of method j::b

  .method assembly hidebysig static valuetype [mscorlib]System.DateTime 
          a(int64 A_0) cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void [mscorlib]System.DateTime::.ctor(int64)
    IL_0006:  ret
  } // end of method j::a

  .method assembly hidebysig static string[] 
          a(string A_0) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  4
    .locals init (char[] V_0)
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_000b:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
    IL_0010:  ldc.i4.1
    IL_0011:  newarr     [mscorlib]System.Char
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldc.i4.s   59
    IL_001b:  stelem.i2
    IL_001c:  ldloc.0
    IL_001d:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
    IL_0022:  ret
  } // end of method j::a

  .method assembly hidebysig static void 
          a(class z A_0,
            string[] A_1) cil managed
  {
    // 代码大小       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  ldelem.ref
    IL_0004:  stfld      string z::b
    IL_0009:  ldarg.0
    IL_000a:  ldarg.1
    IL_000b:  ldc.i4.1
    IL_000c:  ldelem.ref
    IL_000d:  stfld      string z::c
    IL_0012:  ret
  } // end of method j::a

  .method assembly hidebysig static void 
          a(class z A_0,
            string A_1) cil managed
  {
    // 代码大小       111 (0x6f)
    .maxstack  4
    .locals init (string[] V_0,
             string[] V_1,
             char[] V_2)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     [mscorlib]System.Char
    IL_0007:  stloc.2
    IL_0008:  ldloc.2
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.s   13
    IL_000c:  stelem.i2
    IL_000d:  ldloc.2
    IL_000e:  ldc.i4.1
    IL_000f:  ldc.i4.s   10
    IL_0011:  stelem.i2
    IL_0012:  ldloc.2
    IL_0013:  ldc.i4.1
    IL_0014:  callvirt   instance string[] [mscorlib]System.String::Split(char[],
                                                                          valuetype [mscorlib]System.StringSplitOptions)
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.0
    IL_001c:  ldelem.ref
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.1
    IL_001f:  ldelem.ref
    IL_0020:  call       bool o::a(string,
                                   string)
    IL_0025:  brfalse.s  IL_006e

    IL_0027:  ldloc.0
    IL_0028:  ldc.i4.0
    IL_0029:  ldelem.ref
    IL_002a:  call       string[] j::a(string)
    IL_002f:  stloc.1
    IL_0030:  ldarg.0
    IL_0031:  ldloc.1
    IL_0032:  call       void j::a(class z,
                                   string[])
    IL_0037:  ldarg.0
    IL_0038:  ldloc.1
    IL_0039:  ldc.i4.2
    IL_003a:  ldelem.ref
    IL_003b:  call       int32 [mscorlib]System.Int32::Parse(string)
    IL_0040:  stfld      int32 z::d
    IL_0045:  ldarg.0
    IL_0046:  ldloc.1
    IL_0047:  ldc.i4.3
    IL_0048:  ldelem.ref
    IL_0049:  call       int64 [mscorlib]System.Int64::Parse(string)
    IL_004e:  call       valuetype [mscorlib]System.DateTime j::a(int64)
    IL_0053:  stfld      valuetype [mscorlib]System.DateTime z::e
    IL_0058:  ldarg.0
    IL_0059:  ldarg.0
    IL_005a:  ldfld      valuetype [mscorlib]System.DateTime z::e
    IL_005f:  ldsfld     valuetype [mscorlib]System.DateTime j::d
    IL_0064:  call       bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                        valuetype [mscorlib]System.DateTime)
    IL_0069:  stfld      bool z::a
    IL_006e:  ret
  } // end of method j::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method j::.ctor

} // end of class j

.class private auto ansi o
       extends [mscorlib]System.Object
{
  .field static assembly class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider a
  .field private static string b
  .field private static string c
  .field private static string d
  .field private static class [mscorlib]System.IO.StreamReader e
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  stsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider o::a
    IL_0006:  call       object Beetle.TcpUtils::a()
    IL_000b:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_0010:  stsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider o::a
    IL_0015:  ret
  } // end of method o::.cctor

  .method public hidebysig static bool  a(string A_0,
                                          string A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
    IL_000b:  ldarg.1
    IL_000c:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0011:  call       bool o::a(uint8[],
                                   uint8[])
    IL_0016:  ret
  } // end of method o::a

  .method public hidebysig static bool  a(uint8[] A_0,
                                          uint8[] A_1) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider o::a
    IL_0005:  ldarg.0
    IL_0006:  ldstr      "MD5"
    IL_000b:  ldarg.1
    IL_000c:  callvirt   instance bool [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::VerifyData(uint8[],
                                                                                                                   object,
                                                                                                                   uint8[])
    IL_0011:  ret
  } // end of method o::a

  .method private hidebysig static void  c() cil managed
  {
    // 代码大小       51 (0x33)
    .maxstack  8
    IL_0000:  ldtoken    w
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  callvirt   instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()
    IL_000f:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_Location()
    IL_0014:  call       string [mscorlib]System.IO.Path::GetDirectoryName(string)
    IL_0019:  ldsfld     char [mscorlib]System.IO.Path::DirectorySeparatorChar
    IL_001e:  box        [mscorlib]System.Char
    IL_0023:  ldsfld     string o::b
    IL_0028:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_002d:  stsfld     string o::d
    IL_0032:  ret
  } // end of method o::c

  .method private hidebysig static void  b() cil managed
  {
    // 代码大小       16 (0x10)
    .maxstack  8
    IL_0000:  ldsfld     string o::d
    IL_0005:  newobj     instance void [mscorlib]System.IO.StreamReader::.ctor(string)
    IL_000a:  stsfld     class [mscorlib]System.IO.StreamReader o::e
    IL_000f:  ret
  } // end of method o::b

  .method public hidebysig static class z 
          a() cil managed
  {
    // 代码大小       88 (0x58)
    .maxstack  2
    .locals init (class z V_0,
             class [mscorlib]System.IO.StreamReader V_1)
    IL_0000:  newobj     instance void z::.ctor()
    IL_0005:  stloc.0
    .try
    {
      IL_0006:  ldsflda    string o::b
      IL_000b:  call       void j::b(string&)
      IL_0010:  ldsflda    string o::c
      IL_0015:  call       void j::a(string&)
      IL_001a:  call       void o::c()
      IL_001f:  call       void o::b()
      IL_0024:  ldsfld     class [mscorlib]System.IO.StreamReader o::e
      IL_0029:  stloc.1
      .try
      {
        IL_002a:  ldloc.0
        IL_002b:  ldsfld     class [mscorlib]System.IO.StreamReader o::e
        IL_0030:  callvirt   instance string [mscorlib]System.IO.TextReader::ReadToEnd()
        IL_0035:  call       void j::a(class z,
                                       string)
        IL_003a:  leave.s    IL_0046

      }  // end .try
      finally
      {
        IL_003c:  ldloc.1
        IL_003d:  brfalse.s  IL_0045

        IL_003f:  ldloc.1
        IL_0040:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0045:  endfinally
      }  // end handler
      IL_0046:  leave.s    IL_004b

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0048:  pop
      IL_0049:  leave.s    IL_004b

    }  // end handler
    IL_004b:  ldloc.0
    IL_004c:  ldfld      bool z::a
    IL_0051:  stsfld     bool b::c
    IL_0056:  ldloc.0
    IL_0057:  ret
  } // end of method o::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method o::.ctor

} // end of class o

.class private auto ansi beforefieldinit z
       extends [mscorlib]System.Object
{
  .field public bool a
  .field public string b
  .field public string c
  .field public int32 d
  .field public valuetype [mscorlib]System.DateTime e
  .field private static class z f
  .field private static object g
  .field static assembly string h
  .method public hidebysig static uint8[] 
          f(uint8[] A_0) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldsfld     object z::g
    IL_0005:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance uint8[] [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::Decrypt(uint8[],
                                                                                                                   bool)
    IL_0011:  ret
  } // end of method z::f

  .method public hidebysig static class z 
          f() cil managed
  {
    // 代码大小       49 (0x31)
    .maxstack  2
    .locals init (class [mscorlib]System.Type V_0)
    IL_0000:  ldtoken    z
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  dup
    IL_000b:  stloc.0
    IL_000c:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0011:  ldsfld     class z z::f
      IL_0016:  brtrue.s   IL_0022

      IL_0018:  call       class z o::a()
      IL_001d:  stsfld     class z z::f
      IL_0022:  leave.s    IL_002b

    }  // end .try
    finally
    {
      IL_0024:  ldloc.0
      IL_0025:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_002a:  endfinally
    }  // end handler
    IL_002b:  ldsfld     class z z::f
    IL_0030:  ret
  } // end of method z::f

  .method public hidebysig instance bool 
          g() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool z::a
    IL_0006:  ret
  } // end of method z::g

  .method assembly hidebysig static string 
          f(string A_0) cil managed
  {
    // 代码大小       59 (0x3b)
    .maxstack  3
    .locals init (class [Smark.Core]Smark.Core.DESCrypto V_0,
             class [Smark.Core]Smark.Core.DESCrypto/DESInfo V_1)
    IL_0000:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::.ctor()
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldsfld     string Beetle.TcpServer::g
    IL_0013:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0018:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_IV(uint8[])
    IL_001d:  ldloc.1
    IL_001e:  ldsfld     string Beetle.TcpChannel::k
    IL_0023:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0028:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_Key(uint8[])
    IL_002d:  ldloc.1
    IL_002e:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto::set_KeyInfo(class [Smark.Core]Smark.Core.DESCrypto/DESInfo)
    IL_0033:  ldloc.0
    IL_0034:  ldarg.0
    IL_0035:  callvirt   instance string [Smark.Core]Smark.Core.DESCrypto::Decrypt(string)
    IL_003a:  ret
  } // end of method z::f

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method z::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  stsfld     class z z::f
    IL_0006:  ldnull
    IL_0007:  stsfld     object z::g
    IL_000c:  ldstr      "V/r2I2CpRaI="
    IL_0011:  stsfld     string z::h
    IL_0016:  ret
  } // end of method z::.cctor

} // end of class z

.class private auto ansi beforefieldinit d
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 80 09 00 00 00 00 ) 
  .field private string a
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance string 
          a() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string d::a
    IL_0006:  ret
  } // end of method d::a

  .method public hidebysig specialname instance void 
          a(string A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string d::a
    IL_0007:  ret
  } // end of method d::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method d::.ctor

} // end of class d

.class private auto ansi beforefieldinit u
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 80 09 00 00 00 00 ) 
  .field private string a
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance string 
          b() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string u::a
    IL_0006:  ret
  } // end of method u::b

  .method public hidebysig specialname instance void 
          a(string A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string u::a
    IL_0007:  ret
  } // end of method u::a

  .method public hidebysig specialname instance int32 
          a() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 u::b
    IL_0006:  ret
  } // end of method u::a

  .method public hidebysig specialname instance void 
          a(int32 A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 u::b
    IL_0007:  ret
  } // end of method u::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method u::.ctor

} // end of class u

.class private auto ansi beforefieldinit c
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 80 09 00 00 00 00 ) 
  .field private string a
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance string 
          a() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string c::a
    IL_0006:  ret
  } // end of method c::a

  .method public hidebysig specialname instance void 
          a(string A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string c::a
    IL_0007:  ret
  } // end of method c::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method c::.ctor

} // end of class c

.class private auto ansi beforefieldinit k
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 80 09 00 00 00 00 ) 
  .field private string a
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance string 
          b() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string k::a
    IL_0006:  ret
  } // end of method k::b

  .method public hidebysig specialname instance void 
          a(string A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string k::a
    IL_0007:  ret
  } // end of method k::a

  .method public hidebysig specialname instance int32 
          a() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 k::b
    IL_0006:  ret
  } // end of method k::a

  .method public hidebysig specialname instance void 
          a(int32 A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 k::b
    IL_0007:  ret
  } // end of method k::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method k::.ctor

} // end of class k

.class private auto ansi beforefieldinit f
       extends [mscorlib]System.Object
{
  .field public bool a
  .field public string b
  .field public string c
  .field public int32 d
  .field public valuetype [mscorlib]System.DateTime e
  .field private static class ae f
  .field private static object g
  .field private static string h
  .field private static string i
  .field private static char[] j
  .method public hidebysig static bool  f(string A_0,
                                          string A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
    IL_000b:  ldarg.1
    IL_000c:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0011:  call       bool f::f(uint8[],
                                   uint8[])
    IL_0016:  ret
  } // end of method f::f

  .method public hidebysig static bool  f(uint8[] A_0,
                                          uint8[] A_1) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldsfld     object f::g
    IL_0005:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_000a:  ldarg.0
    IL_000b:  ldstr      "MD5"
    IL_0010:  ldarg.1
    IL_0011:  callvirt   instance bool [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::VerifyData(uint8[],
                                                                                                                   object,
                                                                                                                   uint8[])
    IL_0016:  ret
  } // end of method f::f

  .method public hidebysig static string 
          g(string A_0) cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
    IL_0005:  ldarg.0
    IL_0006:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_000b:  call       uint8[] f::f(uint8[])
    IL_0010:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
    IL_0015:  ret
  } // end of method f::g

  .method public hidebysig static uint8[] 
          f(uint8[] A_0) cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldsfld     object f::g
    IL_0005:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance uint8[] [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider::Decrypt(uint8[],
                                                                                                                   bool)
    IL_0011:  ret
  } // end of method f::f

  .method public hidebysig static class ae 
          f() cil managed
  {
    // 代码大小       583 (0x247)
    .maxstack  4
    .locals init (class [mscorlib]System.Reflection.MethodInfo V_0,
             class [mscorlib]System.Reflection.FieldInfo V_1,
             class [mscorlib]System.Reflection.FieldInfo V_2,
             string V_3,
             class [mscorlib]System.IO.StreamReader V_4,
             string[] V_5,
             string[] V_6,
             class [mscorlib]System.Type V_7,
             class [mscorlib]System.Reflection.MethodInfo[] V_8,
             int32 V_9,
             class [mscorlib]System.Reflection.FieldInfo[] V_10,
             int32 V_11,
             class [mscorlib]System.Reflection.FieldInfo[] V_12,
             int32 V_13,
             char[] V_14)
    IL_0000:  ldtoken    ae
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  dup
    IL_000b:  stloc.s    V_7
    IL_000d:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0012:  ldsfld     class ae f::f
      IL_0017:  brtrue     IL_0237

      IL_001c:  newobj     instance void ae::.ctor()
      IL_0021:  stsfld     class ae f::f
      .try
      {
        IL_0026:  ldtoken    ab
        IL_002b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_0030:  ldc.i4.s   24
        IL_0032:  callvirt   instance class [mscorlib]System.Reflection.MethodInfo[] [mscorlib]System.Type::GetMethods(valuetype [mscorlib]System.Reflection.BindingFlags)
        IL_0037:  stloc.s    V_8
        IL_0039:  ldc.i4.0
        IL_003a:  stloc.s    V_9
        IL_003c:  br.s       IL_006d

        IL_003e:  ldloc.s    V_8
        IL_0040:  ldloc.s    V_9
        IL_0042:  ldelem.ref
        IL_0043:  stloc.0
        IL_0044:  ldloc.0
        IL_0045:  ldtoken    af
        IL_004a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_004f:  ldc.i4.0
        IL_0050:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                           bool)
        IL_0055:  ldlen
        IL_0056:  conv.i4
        IL_0057:  ldc.i4.0
        IL_0058:  ble.s      IL_0067

        IL_005a:  ldloc.0
        IL_005b:  ldnull
        IL_005c:  ldnull
        IL_005d:  callvirt   instance object [mscorlib]System.Reflection.MethodBase::Invoke(object,
                                                                                            object[])
        IL_0062:  stsfld     object f::g
        IL_0067:  ldloc.s    V_9
        IL_0069:  ldc.i4.1
        IL_006a:  add
        IL_006b:  stloc.s    V_9
        IL_006d:  ldloc.s    V_9
        IL_006f:  ldloc.s    V_8
        IL_0071:  ldlen
        IL_0072:  conv.i4
        IL_0073:  blt.s      IL_003e

        IL_0075:  ldtoken    ag
        IL_007a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_007f:  ldc.i4.s   24
        IL_0081:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo[] [mscorlib]System.Type::GetFields(valuetype [mscorlib]System.Reflection.BindingFlags)
        IL_0086:  stloc.s    V_10
        IL_0088:  ldc.i4.0
        IL_0089:  stloc.s    V_11
        IL_008b:  br.s       IL_00c0

        IL_008d:  ldloc.s    V_10
        IL_008f:  ldloc.s    V_11
        IL_0091:  ldelem.ref
        IL_0092:  stloc.1
        IL_0093:  ldloc.1
        IL_0094:  ldtoken    t
        IL_0099:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_009e:  ldc.i4.0
        IL_009f:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                           bool)
        IL_00a4:  ldlen
        IL_00a5:  conv.i4
        IL_00a6:  ldc.i4.0
        IL_00a7:  ble.s      IL_00ba

        IL_00a9:  ldloc.1
        IL_00aa:  ldnull
        IL_00ab:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
        IL_00b0:  castclass  [mscorlib]System.String
        IL_00b5:  stsfld     string f::h
        IL_00ba:  ldloc.s    V_11
        IL_00bc:  ldc.i4.1
        IL_00bd:  add
        IL_00be:  stloc.s    V_11
        IL_00c0:  ldloc.s    V_11
        IL_00c2:  ldloc.s    V_10
        IL_00c4:  ldlen
        IL_00c5:  conv.i4
        IL_00c6:  blt.s      IL_008d

        IL_00c8:  ldtoken    ag
        IL_00cd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_00d2:  ldc.i4.s   24
        IL_00d4:  callvirt   instance class [mscorlib]System.Reflection.FieldInfo[] [mscorlib]System.Type::GetFields(valuetype [mscorlib]System.Reflection.BindingFlags)
        IL_00d9:  stloc.s    V_12
        IL_00db:  ldc.i4.0
        IL_00dc:  stloc.s    V_13
        IL_00de:  br.s       IL_0113

        IL_00e0:  ldloc.s    V_12
        IL_00e2:  ldloc.s    V_13
        IL_00e4:  ldelem.ref
        IL_00e5:  stloc.2
        IL_00e6:  ldloc.2
        IL_00e7:  ldtoken    b
        IL_00ec:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_00f1:  ldc.i4.0
        IL_00f2:  callvirt   instance object[] [mscorlib]System.Reflection.MemberInfo::GetCustomAttributes(class [mscorlib]System.Type,
                                                                                                           bool)
        IL_00f7:  ldlen
        IL_00f8:  conv.i4
        IL_00f9:  ldc.i4.0
        IL_00fa:  ble.s      IL_010d

        IL_00fc:  ldloc.2
        IL_00fd:  ldnull
        IL_00fe:  callvirt   instance object [mscorlib]System.Reflection.FieldInfo::GetValue(object)
        IL_0103:  castclass  [mscorlib]System.String
        IL_0108:  stsfld     string f::i
        IL_010d:  ldloc.s    V_13
        IL_010f:  ldc.i4.1
        IL_0110:  add
        IL_0111:  stloc.s    V_13
        IL_0113:  ldloc.s    V_13
        IL_0115:  ldloc.s    V_12
        IL_0117:  ldlen
        IL_0118:  conv.i4
        IL_0119:  blt.s      IL_00e0

        IL_011b:  ldsfld     object f::g
        IL_0120:  castclass  [mscorlib]System.Security.Cryptography.RSACryptoServiceProvider
        IL_0125:  ldsfld     string f::i
        IL_012a:  call       string f::f(string)
        IL_012f:  callvirt   instance void [mscorlib]System.Security.Cryptography.AsymmetricAlgorithm::FromXmlString(string)
        IL_0134:  ldtoken    w
        IL_0139:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
        IL_013e:  callvirt   instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()
        IL_0143:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_Location()
        IL_0148:  call       string [mscorlib]System.IO.Path::GetDirectoryName(string)
        IL_014d:  ldsfld     char [mscorlib]System.IO.Path::DirectorySeparatorChar
        IL_0152:  box        [mscorlib]System.Char
        IL_0157:  ldsfld     string f::h
        IL_015c:  call       string f::f(string)
        IL_0161:  call       string [mscorlib]System.String::Concat(object,
                                                                    object,
                                                                    object)
        IL_0166:  stloc.3
        IL_0167:  ldloc.3
        IL_0168:  newobj     instance void [mscorlib]System.IO.StreamReader::.ctor(string)
        IL_016d:  stloc.s    V_4
        .try
        {
          IL_016f:  ldloc.s    V_4
          IL_0171:  callvirt   instance string [mscorlib]System.IO.TextReader::ReadToEnd()
          IL_0176:  ldsfld     char[] f::j
          IL_017b:  ldc.i4.1
          IL_017c:  callvirt   instance string[] [mscorlib]System.String::Split(char[],
                                                                                valuetype [mscorlib]System.StringSplitOptions)
          IL_0181:  stloc.s    V_5
          IL_0183:  ldloc.s    V_5
          IL_0185:  ldc.i4.0
          IL_0186:  ldelem.ref
          IL_0187:  ldloc.s    V_5
          IL_0189:  ldc.i4.1
          IL_018a:  ldelem.ref
          IL_018b:  call       bool f::f(string,
                                         string)
          IL_0190:  brfalse    IL_0224

          IL_0195:  call       class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
          IL_019a:  ldloc.s    V_5
          IL_019c:  ldc.i4.0
          IL_019d:  ldelem.ref
          IL_019e:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
          IL_01a3:  callvirt   instance string [mscorlib]System.Text.Encoding::GetString(uint8[])
          IL_01a8:  ldc.i4.1
          IL_01a9:  newarr     [mscorlib]System.Char
          IL_01ae:  stloc.s    V_14
          IL_01b0:  ldloc.s    V_14
          IL_01b2:  ldc.i4.0
          IL_01b3:  ldc.i4.s   59
          IL_01b5:  stelem.i2
          IL_01b6:  ldloc.s    V_14
          IL_01b8:  callvirt   instance string[] [mscorlib]System.String::Split(char[])
          IL_01bd:  stloc.s    V_6
          IL_01bf:  ldsfld     class ae f::f
          IL_01c4:  ldloc.s    V_6
          IL_01c6:  ldc.i4.0
          IL_01c7:  ldelem.ref
          IL_01c8:  stfld      string ae::b
          IL_01cd:  ldsfld     class ae f::f
          IL_01d2:  ldloc.s    V_6
          IL_01d4:  ldc.i4.1
          IL_01d5:  ldelem.ref
          IL_01d6:  stfld      string ae::c
          IL_01db:  ldsfld     class ae f::f
          IL_01e0:  ldloc.s    V_6
          IL_01e2:  ldc.i4.2
          IL_01e3:  ldelem.ref
          IL_01e4:  call       int32 [mscorlib]System.Int32::Parse(string)
          IL_01e9:  stfld      int32 ae::d
          IL_01ee:  ldsfld     class ae f::f
          IL_01f3:  ldloc.s    V_6
          IL_01f5:  ldc.i4.3
          IL_01f6:  ldelem.ref
          IL_01f7:  call       int64 [mscorlib]System.Int64::Parse(string)
          IL_01fc:  newobj     instance void [mscorlib]System.DateTime::.ctor(int64)
          IL_0201:  stfld      valuetype [mscorlib]System.DateTime ae::e
          IL_0206:  ldsfld     class ae f::f
          IL_020b:  ldsfld     class ae f::f
          IL_0210:  ldfld      valuetype [mscorlib]System.DateTime ae::e
          IL_0215:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
          IL_021a:  call       bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                              valuetype [mscorlib]System.DateTime)
          IL_021f:  stfld      bool ae::a
          IL_0224:  leave.s    IL_0232

        }  // end .try
        finally
        {
          IL_0226:  ldloc.s    V_4
          IL_0228:  brfalse.s  IL_0231

          IL_022a:  ldloc.s    V_4
          IL_022c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          IL_0231:  endfinally
        }  // end handler
        IL_0232:  leave.s    IL_0237

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0234:  pop
        IL_0235:  leave.s    IL_0237

      }  // end handler
      IL_0237:  leave.s    IL_0241

    }  // end .try
    finally
    {
      IL_0239:  ldloc.s    V_7
      IL_023b:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0240:  endfinally
    }  // end handler
    IL_0241:  ldsfld     class ae f::f
    IL_0246:  ret
  } // end of method f::f

  .method assembly hidebysig static string 
          f(string A_0) cil managed
  {
    // 代码大小       59 (0x3b)
    .maxstack  3
    .locals init (class [Smark.Core]Smark.Core.DESCrypto V_0,
             class [Smark.Core]Smark.Core.DESCrypto/DESInfo V_1)
    IL_0000:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj     instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::.ctor()
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldsfld     string Beetle.TcpServer::g
    IL_0013:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0018:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_IV(uint8[])
    IL_001d:  ldloc.1
    IL_001e:  ldsfld     string Beetle.TcpChannel::k
    IL_0023:  call       uint8[] [mscorlib]System.Convert::FromBase64String(string)
    IL_0028:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto/DESInfo::set_Key(uint8[])
    IL_002d:  ldloc.1
    IL_002e:  callvirt   instance void [Smark.Core]Smark.Core.DESCrypto::set_KeyInfo(class [Smark.Core]Smark.Core.DESCrypto/DESInfo)
    IL_0033:  ldloc.0
    IL_0034:  ldarg.0
    IL_0035:  callvirt   instance string [Smark.Core]Smark.Core.DESCrypto::Decrypt(string)
    IL_003a:  ret
  } // end of method f::f

  .method public hidebysig instance bool 
          f(int32 A_0) cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool f::a
    IL_0006:  brfalse.s  IL_0015

    IL_0008:  ldarg.1
    IL_0009:  ldarg.0
    IL_000a:  ldfld      int32 f::d
    IL_000f:  cgt
    IL_0011:  ldc.i4.0
    IL_0012:  ceq
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } // end of method f::f

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method f::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       36 (0x24)
    .maxstack  3
    .locals init (char[] V_0)
    IL_0000:  ldnull
    IL_0001:  stsfld     class ae f::f
    IL_0006:  ldnull
    IL_0007:  stsfld     object f::g
    IL_000c:  ldc.i4.2
    IL_000d:  newarr     [mscorlib]System.Char
    IL_0012:  stloc.0
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  ldc.i4.s   13
    IL_0017:  stelem.i2
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  stelem.i2
    IL_001d:  ldloc.0
    IL_001e:  stsfld     char[] f::j
    IL_0023:  ret
  } // end of method f::.cctor

} // end of class f

.class private auto ansi beforefieldinit g
       extends [mscorlib]System.Object
{
  .field private class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> a
  .field private class Beetle.TcpChannel b
  .field private bool c
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.Threading.WaitCallback V_0)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4     0x400
    IL_0008:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel>::.ctor(int32)
    IL_000d:  stfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
    IL_0012:  ldarg.0
    IL_0013:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0018:  ldloc.0
    IL_0019:  brtrue.s   IL_0028

    IL_001b:  ldarg.0
    IL_001c:  ldftn      instance void g::a(object)
    IL_0022:  newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                      native int)
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback)
    IL_002e:  pop
    IL_002f:  ret
  } // end of method g::.ctor

  .method public hidebysig instance void 
          a(class Beetle.TcpChannel A_0) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel>::Enqueue(!0)
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method g::a

  .method public hidebysig specialname instance int32 
          c() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel>::get_Count()
    IL_000b:  ret
  } // end of method g::c

  .method family hidebysig instance class Beetle.TcpChannel 
          a() cil managed
  {
    // 代码大小       54 (0x36)
    .maxstack  2
    .locals init (class Beetle.TcpChannel V_0,
             class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0029

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
      IL_0021:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel>::Dequeue()
      IL_0026:  stloc.0
      IL_0027:  leave.s    IL_0034

      IL_0029:  ldnull
      IL_002a:  stloc.0
      IL_002b:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_002d:  ldloc.1
      IL_002e:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ldloc.0
    IL_0035:  ret
  } // end of method g::a

  .method family hidebysig newslot virtual 
          instance void  b() cil managed
  {
    // 代码大小       102 (0x66)
    .maxstack  3
    .locals init (class [mscorlib]System.Exception V_0,
             class Beetle.ChannelErrorEventArgs V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance class Beetle.TcpChannel g::a()
    IL_0007:  stfld      class Beetle.TcpChannel g::b
    IL_000c:  ldarg.0
    IL_000d:  ldfld      class Beetle.TcpChannel g::b
    IL_0012:  brfalse.s  IL_005b

    .try
    {
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class Beetle.TcpChannel g::b
      IL_001a:  ldfld      bool Beetle.TcpChannel::ad
      IL_001f:  brtrue.s   IL_0056

      .try
      {
        IL_0021:  ldarg.0
        IL_0022:  ldfld      class Beetle.TcpChannel g::b
        IL_0027:  callvirt   instance void Beetle.TcpChannel::c()
        IL_002c:  leave.s    IL_0056

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_002e:  stloc.0
        IL_002f:  ldarg.0
        IL_0030:  ldfld      class Beetle.TcpChannel g::b
        IL_0035:  newobj     instance void Beetle.ChannelErrorEventArgs::.ctor()
        IL_003a:  stloc.1
        IL_003b:  ldloc.1
        IL_003c:  ldarg.0
        IL_003d:  ldfld      class Beetle.TcpChannel g::b
        IL_0042:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
        IL_0047:  ldloc.1
        IL_0048:  ldloc.0
        IL_0049:  callvirt   instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
        IL_004e:  ldloc.1
        IL_004f:  callvirt   instance void Beetle.TcpChannel::CallChannelError(class Beetle.ChannelErrorEventArgs)
        IL_0054:  leave.s    IL_0056

      }  // end handler
      IL_0056:  leave.s    IL_0065

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0058:  pop
      IL_0059:  leave.s    IL_0065

    }  // end handler
    IL_005b:  ldsfld     int32 Beetle.TcpUtils::SocketDespatchSleepTime
    IL_0060:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)
    IL_0065:  ret
  } // end of method g::b

  .method public hidebysig newslot virtual 
          instance void  d() cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  8
    IL_0000:  ldsfld     bool Beetle.TcpUtils::ExecutionContext
    IL_0005:  brtrue.s   IL_0015

    IL_0007:  call       valuetype [mscorlib]System.Threading.AsyncFlowControl [mscorlib]System.Threading.ExecutionContext::SuppressFlow()
    IL_000c:  pop
    IL_000d:  br.s       IL_0015

    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance void g::b()
    IL_0015:  ldarg.0
    IL_0016:  ldfld      bool g::c
    IL_001b:  brfalse.s  IL_000f

    IL_001d:  ret
  } // end of method g::d

  .method public hidebysig instance void 
          e() cil managed
  {
    // 代码大小       65 (0x41)
    .maxstack  2
    .locals init (class g V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool g::c
      IL_000e:  brtrue.s   IL_0037

      IL_0010:  br.s       IL_0022

      IL_0012:  ldarg.0
      IL_0013:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
      IL_0018:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel>::Dequeue()
      IL_001d:  callvirt   instance void Beetle.TcpChannel::Dispose()
      IL_0022:  ldarg.0
      IL_0023:  ldfld      class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel> g::a
      IL_0028:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class Beetle.TcpChannel>::get_Count()
      IL_002d:  ldc.i4.0
      IL_002e:  bgt.s      IL_0012

      IL_0030:  ldarg.0
      IL_0031:  ldc.i4.1
      IL_0032:  stfld      bool g::c
      IL_0037:  leave.s    IL_0040

    }  // end .try
    finally
    {
      IL_0039:  ldloc.0
      IL_003a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003f:  endfinally
    }  // end handler
    IL_0040:  ret
  } // end of method g::e

  .method private hidebysig instance void 
          a(object A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance void g::d()
    IL_0006:  ret
  } // end of method g::a

} // end of class g

.class private auto ansi beforefieldinit v
       extends [mscorlib]System.Object
       implements Beetle.IByteConverter
{
  .field private uint8[] a
  .field private uint8[] b
  .field private uint8[] c
  .field private uint8[] d
  .field private class Beetle.ByteArraySegment e
  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(char 'value') cil managed
  {
    // 代码大小       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i2
    IL_0003:  call       instance uint8[] v::GetBytes(int16)
    IL_0008:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(int16 'value') cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  2
    .locals init (uint8& pinned V_0,
             uint8[] V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] v::b
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  brfalse.s  IL_000f

    IL_000a:  ldloc.1
    IL_000b:  ldlen
    IL_000c:  conv.i4
    IL_000d:  brtrue.s   IL_0014

    IL_000f:  ldc.i4.0
    IL_0010:  conv.u
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001c

    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  ldelema    [mscorlib]System.Byte
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  conv.i
    IL_001e:  ldarg.1
    IL_001f:  stind.i2
    IL_0020:  ldc.i4.0
    IL_0021:  conv.u
    IL_0022:  stloc.0
    IL_0023:  ldarg.0
    IL_0024:  ldfld      uint8[] v::b
    IL_0029:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(int32 'value') cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  2
    .locals init (uint8& pinned V_0,
             uint8[] V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] v::c
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  brfalse.s  IL_000f

    IL_000a:  ldloc.1
    IL_000b:  ldlen
    IL_000c:  conv.i4
    IL_000d:  brtrue.s   IL_0014

    IL_000f:  ldc.i4.0
    IL_0010:  conv.u
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001c

    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  ldelema    [mscorlib]System.Byte
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  conv.i
    IL_001e:  ldarg.1
    IL_001f:  stind.i4
    IL_0020:  ldc.i4.0
    IL_0021:  conv.u
    IL_0022:  stloc.0
    IL_0023:  ldarg.0
    IL_0024:  ldfld      uint8[] v::c
    IL_0029:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(int64 'value') cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  2
    .locals init (uint8& pinned V_0,
             uint8[] V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] v::d
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  brfalse.s  IL_000f

    IL_000a:  ldloc.1
    IL_000b:  ldlen
    IL_000c:  conv.i4
    IL_000d:  brtrue.s   IL_0014

    IL_000f:  ldc.i4.0
    IL_0010:  conv.u
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001c

    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  ldelema    [mscorlib]System.Byte
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  conv.i
    IL_001e:  ldarg.1
    IL_001f:  stind.i8
    IL_0020:  ldc.i4.0
    IL_0021:  conv.u
    IL_0022:  stloc.0
    IL_0023:  ldarg.0
    IL_0024:  ldfld      uint8[] v::d
    IL_0029:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(float32 'value') cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarga.s   'value'
    IL_0003:  conv.u
    IL_0004:  ldind.i4
    IL_0005:  call       instance uint8[] v::GetBytes(int32)
    IL_000a:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(float64 'value') cil managed
  {
    // 代码大小       11 (0xb)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarga.s   'value'
    IL_0003:  conv.u
    IL_0004:  ldind.i8
    IL_0005:  call       instance uint8[] v::GetBytes(int64)
    IL_000a:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(uint8 'value') cil managed
  {
    // 代码大小       16 (0x10)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] v::a
    IL_0006:  ldc.i4.0
    IL_0007:  ldarg.1
    IL_0008:  stelem.i1
    IL_0009:  ldarg.0
    IL_000a:  ldfld      uint8[] v::a
    IL_000f:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(bool 'value') cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_000e

    IL_0003:  ldarg.0
    IL_0004:  ldfld      uint8[] v::a
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.1
    IL_000b:  stelem.i1
    IL_000c:  br.s       IL_0017

    IL_000e:  ldarg.0
    IL_000f:  ldfld      uint8[] v::a
    IL_0014:  ldc.i4.0
    IL_0015:  ldc.i4.0
    IL_0016:  stelem.i1
    IL_0017:  ldarg.0
    IL_0018:  ldfld      uint8[] v::a
    IL_001d:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance char  ReadChar(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.2
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       char [mscorlib]System.BitConverter::ToChar(uint8[],
                                                                    int32)
    IL_0028:  ret
  } // end of method v::ReadChar

  .method public hidebysig newslot virtual final 
          instance int16  ReadInt16(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.2
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       int16 [mscorlib]System.BitConverter::ToInt16(uint8[],
                                                                      int32)
    IL_0028:  ret
  } // end of method v::ReadInt16

  .method public hidebysig newslot virtual final 
          instance int32  ReadInt32(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.4
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       int32 [mscorlib]System.BitConverter::ToInt32(uint8[],
                                                                      int32)
    IL_0028:  ret
  } // end of method v::ReadInt32

  .method public hidebysig newslot virtual final 
          instance int64  ReadInt64(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       int64 [mscorlib]System.BitConverter::ToInt64(uint8[],
                                                                      int32)
    IL_0028:  ret
  } // end of method v::ReadInt64

  .method public hidebysig newslot virtual final 
          instance float32  ReadFloat(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.4
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       float32 [mscorlib]System.BitConverter::ToSingle(uint8[],
                                                                         int32)
    IL_0028:  ret
  } // end of method v::ReadFloat

  .method public hidebysig newslot virtual final 
          instance float64  ReadDouble(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       float64 [mscorlib]System.BitConverter::ToDouble(uint8[],
                                                                         int32)
    IL_0028:  ret
  } // end of method v::ReadDouble

  .method public hidebysig newslot virtual final 
          instance uint8  ReadByte(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  ldelem.u1
    IL_0024:  ret
  } // end of method v::ReadByte

  .method public hidebysig newslot virtual final 
          instance bool  ReadBool(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       bool [mscorlib]System.BitConverter::ToBoolean(uint8[],
                                                                       int32)
    IL_0028:  ret
  } // end of method v::ReadBool

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(uint16 'value') cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  2
    .locals init (uint8& pinned V_0,
             uint8[] V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] v::b
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  brfalse.s  IL_000f

    IL_000a:  ldloc.1
    IL_000b:  ldlen
    IL_000c:  conv.i4
    IL_000d:  brtrue.s   IL_0014

    IL_000f:  ldc.i4.0
    IL_0010:  conv.u
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001c

    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  ldelema    [mscorlib]System.Byte
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  conv.i
    IL_001e:  ldarg.1
    IL_001f:  stind.i2
    IL_0020:  ldc.i4.0
    IL_0021:  conv.u
    IL_0022:  stloc.0
    IL_0023:  ldarg.0
    IL_0024:  ldfld      uint8[] v::b
    IL_0029:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(uint32 'value') cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  2
    .locals init (uint8& pinned V_0,
             uint8[] V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] v::c
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  brfalse.s  IL_000f

    IL_000a:  ldloc.1
    IL_000b:  ldlen
    IL_000c:  conv.i4
    IL_000d:  brtrue.s   IL_0014

    IL_000f:  ldc.i4.0
    IL_0010:  conv.u
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001c

    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  ldelema    [mscorlib]System.Byte
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  conv.i
    IL_001e:  ldarg.1
    IL_001f:  stind.i4
    IL_0020:  ldc.i4.0
    IL_0021:  conv.u
    IL_0022:  stloc.0
    IL_0023:  ldarg.0
    IL_0024:  ldfld      uint8[] v::c
    IL_0029:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint8[]  GetBytes(uint64 'value') cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  2
    .locals init (uint8& pinned V_0,
             uint8[] V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] v::d
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  brfalse.s  IL_000f

    IL_000a:  ldloc.1
    IL_000b:  ldlen
    IL_000c:  conv.i4
    IL_000d:  brtrue.s   IL_0014

    IL_000f:  ldc.i4.0
    IL_0010:  conv.u
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001c

    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  ldelema    [mscorlib]System.Byte
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  conv.i
    IL_001e:  ldarg.1
    IL_001f:  stind.i8
    IL_0020:  ldc.i4.0
    IL_0021:  conv.u
    IL_0022:  stloc.0
    IL_0023:  ldarg.0
    IL_0024:  ldfld      uint8[] v::d
    IL_0029:  ret
  } // end of method v::GetBytes

  .method public hidebysig newslot virtual final 
          instance uint16  ReadUInt16(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.2
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       uint16 [mscorlib]System.BitConverter::ToUInt16(uint8[],
                                                                        int32)
    IL_0028:  ret
  } // end of method v::ReadUInt16

  .method public hidebysig newslot virtual final 
          instance uint32  ReadUInt32(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.4
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       uint32 [mscorlib]System.BitConverter::ToUInt32(uint8[],
                                                                        int32)
    IL_0028:  ret
  } // end of method v::ReadUInt32

  .method public hidebysig newslot virtual final 
          instance uint64  ReadUInt64(class Beetle.BufferReader reader) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  callvirt   instance class Beetle.ByteArraySegment Beetle.BufferReader::Read(int32)
    IL_0008:  stfld      class Beetle.ByteArraySegment v::e
    IL_000d:  ldarg.0
    IL_000e:  ldfld      class Beetle.ByteArraySegment v::e
    IL_0013:  callvirt   instance uint8[] Beetle.ByteArraySegment::get_Array()
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment v::e
    IL_001e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0023:  call       uint64 [mscorlib]System.BitConverter::ToUInt64(uint8[],
                                                                        int32)
    IL_0028:  ret
  } // end of method v::ReadUInt64

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       55 (0x37)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  stfld      uint8[] v::a
    IL_000c:  ldarg.0
    IL_000d:  ldc.i4.2
    IL_000e:  newarr     [mscorlib]System.Byte
    IL_0013:  stfld      uint8[] v::b
    IL_0018:  ldarg.0
    IL_0019:  ldc.i4.4
    IL_001a:  newarr     [mscorlib]System.Byte
    IL_001f:  stfld      uint8[] v::c
    IL_0024:  ldarg.0
    IL_0025:  ldc.i4.8
    IL_0026:  newarr     [mscorlib]System.Byte
    IL_002b:  stfld      uint8[] v::d
    IL_0030:  ldarg.0
    IL_0031:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0036:  ret
  } // end of method v::.ctor

} // end of class v

.class private auto ansi beforefieldinit ai
       extends [mscorlib]System.Attribute
{
  .field private uint8[] a
  .field private char[] b
  .field private class Beetle.ByteArraySegment c
  .field private int32 d
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0006:  stfld      class Beetle.ByteArraySegment ai::c
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldsfld     int32 Beetle.TcpUtils::StringEncodingLength
    IL_0017:  newarr     [mscorlib]System.Byte
    IL_001c:  stfld      uint8[] ai::a
    IL_0021:  ldarg.0
    IL_0022:  ldsfld     int32 Beetle.TcpUtils::StringEncodingLength
    IL_0027:  newarr     [mscorlib]System.Char
    IL_002c:  stfld      char[] ai::b
    IL_0031:  ret
  } // end of method ai::.ctor

  .method public hidebysig instance void 
          b(string A_0,
            class [mscorlib]System.Text.Encoding A_1,
            class b A_2,
            class Beetle.BufferWriter A_3,
            bool A_4) cil managed
  {
    // 代码大小       292 (0x124)
    .maxstack  7
    .locals init (int32 V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      IL_0000:  ldarg.s    A_3
      IL_0002:  ldfld      bool Beetle.BufferWriter::EnabledVariant
      IL_0007:  brtrue     IL_00d8

      IL_000c:  ldarg.3
      IL_000d:  ldarg.1
      IL_000e:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0013:  ldc.i4.2
      IL_0014:  mul
      IL_0015:  ldc.i4.2
      IL_0016:  add
      IL_0017:  callvirt   instance bool b::c(int32)
      IL_001c:  brfalse    IL_00d8

      IL_0021:  ldarg.3
      IL_0022:  ldfld      int32 b::g
      IL_0027:  stloc.0
      IL_0028:  ldarg.s    A_4
      IL_002a:  brfalse.s  IL_0057

      IL_002c:  ldarg.3
      IL_002d:  dup
      IL_002e:  ldfld      int32 b::g
      IL_0033:  ldc.i4.2
      IL_0034:  add
      IL_0035:  stfld      int32 b::g
      IL_003a:  ldarg.3
      IL_003b:  dup
      IL_003c:  ldfld      int32 b::h
      IL_0041:  ldc.i4.2
      IL_0042:  add
      IL_0043:  stfld      int32 b::h
      IL_0048:  ldarg.s    A_3
      IL_004a:  dup
      IL_004b:  ldfld      int32 Beetle.BufferWriter::e
      IL_0050:  ldc.i4.2
      IL_0051:  add
      IL_0052:  stfld      int32 Beetle.BufferWriter::e
      IL_0057:  ldarg.0
      IL_0058:  ldarg.2
      IL_0059:  ldarg.1
      IL_005a:  ldc.i4.0
      IL_005b:  ldarg.1
      IL_005c:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0061:  ldarg.3
      IL_0062:  ldfld      uint8[] b::e
      IL_0067:  ldarg.3
      IL_0068:  ldfld      int32 b::g
      IL_006d:  callvirt   instance int32 [mscorlib]System.Text.Encoding::GetBytes(string,
                                                                                   int32,
                                                                                   int32,
                                                                                   uint8[],
                                                                                   int32)
      IL_0072:  stfld      int32 ai::d
      IL_0077:  ldarg.3
      IL_0078:  dup
      IL_0079:  ldfld      int32 b::g
      IL_007e:  ldarg.0
      IL_007f:  ldfld      int32 ai::d
      IL_0084:  add
      IL_0085:  stfld      int32 b::g
      IL_008a:  ldarg.3
      IL_008b:  dup
      IL_008c:  ldfld      int32 b::h
      IL_0091:  ldarg.0
      IL_0092:  ldfld      int32 ai::d
      IL_0097:  add
      IL_0098:  stfld      int32 b::h
      IL_009d:  ldarg.s    A_3
      IL_009f:  dup
      IL_00a0:  ldfld      int32 Beetle.BufferWriter::e
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      int32 ai::d
      IL_00ab:  add
      IL_00ac:  stfld      int32 Beetle.BufferWriter::e
      IL_00b1:  ldarg.s    A_4
      IL_00b3:  brfalse.s  IL_0119

      IL_00b5:  ldarg.s    A_3
      IL_00b7:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
      IL_00bc:  ldarg.0
      IL_00bd:  ldfld      int32 ai::d
      IL_00c2:  conv.u2
      IL_00c3:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(uint16)
      IL_00c8:  ldc.i4.0
      IL_00c9:  ldarg.3
      IL_00ca:  ldfld      uint8[] b::e
      IL_00cf:  ldloc.0
      IL_00d0:  ldc.i4.2
      IL_00d1:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                   int32,
                                                                   class [mscorlib]System.Array,
                                                                   int32,
                                                                   int32)
      IL_00d6:  br.s       IL_0119

      IL_00d8:  ldarg.0
      IL_00d9:  ldarg.2
      IL_00da:  ldarg.1
      IL_00db:  ldc.i4.0
      IL_00dc:  ldarg.1
      IL_00dd:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_00e2:  ldarg.0
      IL_00e3:  ldfld      uint8[] ai::a
      IL_00e8:  ldc.i4.0
      IL_00e9:  callvirt   instance int32 [mscorlib]System.Text.Encoding::GetBytes(string,
                                                                                   int32,
                                                                                   int32,
                                                                                   uint8[],
                                                                                   int32)
      IL_00ee:  stfld      int32 ai::d
      IL_00f3:  ldarg.s    A_4
      IL_00f5:  brfalse.s  IL_0105

      IL_00f7:  ldarg.s    A_3
      IL_00f9:  ldarg.0
      IL_00fa:  ldfld      int32 ai::d
      IL_00ff:  conv.u2
      IL_0100:  callvirt   instance void Beetle.BufferWriter::Write(uint16)
      IL_0105:  ldarg.s    A_3
      IL_0107:  ldarg.0
      IL_0108:  ldfld      uint8[] ai::a
      IL_010d:  ldc.i4.0
      IL_010e:  ldarg.0
      IL_010f:  ldfld      int32 ai::d
      IL_0114:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                    int32,
                                                                    int32)
      IL_0119:  leave.s    IL_0123

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_011b:  stloc.1
      IL_011c:  ldloc.1
      IL_011d:  call       class Beetle.NetTcpException Beetle.NetTcpException::StringEncodingError(class [mscorlib]System.Exception)
      IL_0122:  throw

    }  // end handler
    IL_0123:  ret
  } // end of method ai::b

  .method public hidebysig instance void 
          a(string A_0,
            class [mscorlib]System.Text.Encoding A_1,
            class b A_2,
            class Beetle.BufferWriter A_3,
            bool A_4) cil managed
  {
    // 代码大小       290 (0x122)
    .maxstack  7
    .locals init (int32 V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      IL_0000:  ldarg.s    A_3
      IL_0002:  ldfld      bool Beetle.BufferWriter::EnabledVariant
      IL_0007:  brtrue     IL_00d7

      IL_000c:  ldarg.3
      IL_000d:  ldarg.1
      IL_000e:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0013:  ldc.i4.2
      IL_0014:  mul
      IL_0015:  ldc.i4.4
      IL_0016:  add
      IL_0017:  callvirt   instance bool b::c(int32)
      IL_001c:  brfalse    IL_00d7

      IL_0021:  ldarg.3
      IL_0022:  ldfld      int32 b::g
      IL_0027:  stloc.0
      IL_0028:  ldarg.s    A_4
      IL_002a:  brfalse.s  IL_0057

      IL_002c:  ldarg.3
      IL_002d:  dup
      IL_002e:  ldfld      int32 b::g
      IL_0033:  ldc.i4.4
      IL_0034:  add
      IL_0035:  stfld      int32 b::g
      IL_003a:  ldarg.3
      IL_003b:  dup
      IL_003c:  ldfld      int32 b::h
      IL_0041:  ldc.i4.4
      IL_0042:  add
      IL_0043:  stfld      int32 b::h
      IL_0048:  ldarg.s    A_3
      IL_004a:  dup
      IL_004b:  ldfld      int32 Beetle.BufferWriter::e
      IL_0050:  ldc.i4.4
      IL_0051:  add
      IL_0052:  stfld      int32 Beetle.BufferWriter::e
      IL_0057:  ldarg.0
      IL_0058:  ldarg.2
      IL_0059:  ldarg.1
      IL_005a:  ldc.i4.0
      IL_005b:  ldarg.1
      IL_005c:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_0061:  ldarg.3
      IL_0062:  ldfld      uint8[] b::e
      IL_0067:  ldarg.3
      IL_0068:  ldfld      int32 b::g
      IL_006d:  callvirt   instance int32 [mscorlib]System.Text.Encoding::GetBytes(string,
                                                                                   int32,
                                                                                   int32,
                                                                                   uint8[],
                                                                                   int32)
      IL_0072:  stfld      int32 ai::d
      IL_0077:  ldarg.3
      IL_0078:  dup
      IL_0079:  ldfld      int32 b::g
      IL_007e:  ldarg.0
      IL_007f:  ldfld      int32 ai::d
      IL_0084:  add
      IL_0085:  stfld      int32 b::g
      IL_008a:  ldarg.3
      IL_008b:  dup
      IL_008c:  ldfld      int32 b::h
      IL_0091:  ldarg.0
      IL_0092:  ldfld      int32 ai::d
      IL_0097:  add
      IL_0098:  stfld      int32 b::h
      IL_009d:  ldarg.s    A_3
      IL_009f:  dup
      IL_00a0:  ldfld      int32 Beetle.BufferWriter::e
      IL_00a5:  ldarg.0
      IL_00a6:  ldfld      int32 ai::d
      IL_00ab:  add
      IL_00ac:  stfld      int32 Beetle.BufferWriter::e
      IL_00b1:  ldarg.s    A_4
      IL_00b3:  brfalse.s  IL_0117

      IL_00b5:  ldarg.s    A_3
      IL_00b7:  ldfld      class Beetle.IByteConverter Beetle.BufferWriter::g
      IL_00bc:  ldarg.0
      IL_00bd:  ldfld      int32 ai::d
      IL_00c2:  callvirt   instance uint8[] Beetle.IByteConverter::GetBytes(int32)
      IL_00c7:  ldc.i4.0
      IL_00c8:  ldarg.3
      IL_00c9:  ldfld      uint8[] b::e
      IL_00ce:  ldloc.0
      IL_00cf:  ldc.i4.4
      IL_00d0:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                   int32,
                                                                   class [mscorlib]System.Array,
                                                                   int32,
                                                                   int32)
      IL_00d5:  br.s       IL_0117

      IL_00d7:  ldarg.0
      IL_00d8:  ldarg.2
      IL_00d9:  ldarg.1
      IL_00da:  ldc.i4.0
      IL_00db:  ldarg.1
      IL_00dc:  callvirt   instance int32 [mscorlib]System.String::get_Length()
      IL_00e1:  ldarg.0
      IL_00e2:  ldfld      uint8[] ai::a
      IL_00e7:  ldc.i4.0
      IL_00e8:  callvirt   instance int32 [mscorlib]System.Text.Encoding::GetBytes(string,
                                                                                   int32,
                                                                                   int32,
                                                                                   uint8[],
                                                                                   int32)
      IL_00ed:  stfld      int32 ai::d
      IL_00f2:  ldarg.s    A_4
      IL_00f4:  brfalse.s  IL_0103

      IL_00f6:  ldarg.s    A_3
      IL_00f8:  ldarg.0
      IL_00f9:  ldfld      int32 ai::d
      IL_00fe:  callvirt   instance void Beetle.BufferWriter::Write(int32)
      IL_0103:  ldarg.s    A_3
      IL_0105:  ldarg.0
      IL_0106:  ldfld      uint8[] ai::a
      IL_010b:  ldc.i4.0
      IL_010c:  ldarg.0
      IL_010d:  ldfld      int32 ai::d
      IL_0112:  callvirt   instance void Beetle.BufferWriter::Write(uint8[],
                                                                    int32,
                                                                    int32)
      IL_0117:  leave.s    IL_0121

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0119:  stloc.1
      IL_011a:  ldloc.1
      IL_011b:  call       class Beetle.NetTcpException Beetle.NetTcpException::StringEncodingError(class [mscorlib]System.Exception)
      IL_0120:  throw

    }  // end handler
    IL_0121:  ret
  } // end of method ai::a

  .method public hidebysig instance string 
          a(uint8[] A_0,
            int32 A_1,
            int32 A_2,
            class [mscorlib]System.Text.Encoding A_3) cil managed
  {
    // 代码大小       44 (0x2c)
    .maxstack  6
    .locals init (int32 V_0,
             class [mscorlib]System.Exception V_1,
             string V_2)
    .try
    {
      IL_0000:  ldarg.s    A_3
      IL_0002:  ldarg.1
      IL_0003:  ldarg.2
      IL_0004:  ldarg.3
      IL_0005:  ldarg.0
      IL_0006:  ldfld      char[] ai::b
      IL_000b:  ldc.i4.0
      IL_000c:  callvirt   instance int32 [mscorlib]System.Text.Encoding::GetChars(uint8[],
                                                                                   int32,
                                                                                   int32,
                                                                                   char[],
                                                                                   int32)
      IL_0011:  stloc.0
      IL_0012:  ldarg.0
      IL_0013:  ldfld      char[] ai::b
      IL_0018:  ldc.i4.0
      IL_0019:  ldloc.0
      IL_001a:  newobj     instance void [mscorlib]System.String::.ctor(char[],
                                                                        int32,
                                                                        int32)
      IL_001f:  stloc.2
      IL_0020:  leave.s    IL_002a

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0022:  stloc.1
      IL_0023:  ldloc.1
      IL_0024:  call       class Beetle.NetTcpException Beetle.NetTcpException::StringDecodingError(class [mscorlib]System.Exception)
      IL_0029:  throw

    }  // end handler
    IL_002a:  ldloc.2
    IL_002b:  ret
  } // end of method ai::a

} // end of class ai

.class private auto ansi beforefieldinit q
       extends [mscorlib]System.Object
{
  .field private string a
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private object b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private valuetype [mscorlib]System.DateTime c
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class q d
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class q e
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance string 
          b() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string q::a
    IL_0006:  ret
  } // end of method q::b

  .method public hidebysig specialname instance void 
          a(string A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string q::a
    IL_0007:  ret
  } // end of method q::a

  .method public hidebysig specialname instance object 
          f() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object q::b
    IL_0006:  ret
  } // end of method q::f

  .method public hidebysig specialname instance void 
          a(object A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      object q::b
    IL_0007:  ret
  } // end of method q::a

  .method public hidebysig specialname instance valuetype [mscorlib]System.DateTime 
          e() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype [mscorlib]System.DateTime q::c
    IL_0006:  ret
  } // end of method q::e

  .method public hidebysig specialname instance void 
          a(valuetype [mscorlib]System.DateTime A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      valuetype [mscorlib]System.DateTime q::c
    IL_0007:  ret
  } // end of method q::a

  .method public hidebysig specialname instance bool 
          d() cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0005:  ldarg.0
    IL_0006:  call       instance valuetype [mscorlib]System.DateTime q::e()
    IL_000b:  call       bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                        valuetype [mscorlib]System.DateTime)
    IL_0010:  ret
  } // end of method q::d

  .method public hidebysig specialname instance class q 
          c() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class q q::d
    IL_0006:  ret
  } // end of method q::c

  .method public hidebysig specialname instance void 
          b(class q A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class q q::d
    IL_0007:  ret
  } // end of method q::b

  .method public hidebysig specialname instance class q 
          a() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class q q::e
    IL_0006:  ret
  } // end of method q::a

  .method public hidebysig specialname instance void 
          a(class q A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class q q::e
    IL_0007:  ret
  } // end of method q::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method q::.ctor

} // end of class q

.class private abstract auto ansi beforefieldinit ad
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field public class a e
  .field public class Beetle.TcpChannel f
  .method public hidebysig newslot abstract virtual 
          instance void  a() cil managed
  {
  } // end of method ad::a

  .method public hidebysig newslot abstract virtual 
          instance void  Dispose() cil managed
  {
  } // end of method ad::Dispose

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ad::.ctor

} // end of class ad

.class private auto ansi beforefieldinit p
       extends ad
{
  .field public static string b
  .custom instance void k::.ctor() = ( 01 00 00 00 ) 
  .field public static string c
  .custom instance void ai::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig virtual instance void 
          a() cil managed
  {
    // 代码大小       54 (0x36)
    .maxstack  3
    .locals init (class [mscorlib]System.Exception V_0,
             class Beetle.ChannelErrorEventArgs V_1)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Beetle.TcpChannel ad::f
      IL_0006:  callvirt   instance void Beetle.TcpChannel::d()
      IL_000b:  leave.s    IL_0035

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_000d:  stloc.0
      IL_000e:  ldarg.0
      IL_000f:  ldfld      class Beetle.TcpChannel ad::f
      IL_0014:  newobj     instance void Beetle.ChannelErrorEventArgs::.ctor()
      IL_0019:  stloc.1
      IL_001a:  ldloc.1
      IL_001b:  ldarg.0
      IL_001c:  ldfld      class Beetle.TcpChannel ad::f
      IL_0021:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_0026:  ldloc.1
      IL_0027:  ldloc.0
      IL_0028:  callvirt   instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
      IL_002d:  ldloc.1
      IL_002e:  callvirt   instance void Beetle.TcpChannel::CallChannelError(class Beetle.ChannelErrorEventArgs)
      IL_0033:  leave.s    IL_0035

    }  // end handler
    IL_0035:  ret
  } // end of method p::a

  .method public hidebysig virtual instance void 
          Dispose() cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method p::Dispose

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ad::.ctor()
    IL_0006:  ret
  } // end of method p::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       21 (0x15)
    .maxstack  8
    IL_0000:  ldstr      "KntezZn3/HTLy0OE4rsKQNgpdkZpBMBet2b45vHtFqeU5lDeTd"
    + "KI70zrINkgMhjlYl9lYFiTnxjOV9OfTzoBd/Cz/pdkTh2ylx0uLR2fPBQoxXaUg3BtYj83r"
    + "pP3S"
    IL_0005:  stsfld     string p::b
    IL_000a:  ldstr      "baxxfKKgHV9XwOpsrn1uYA=="
    IL_000f:  stsfld     string p::c
    IL_0014:  ret
  } // end of method p::.cctor

} // end of class p

.class private auto ansi beforefieldinit ac
       extends ad
{
  .method public hidebysig virtual instance void 
          a() cil managed
  {
    // 代码大小       74 (0x4a)
    .maxstack  3
    .locals init (class [mscorlib]System.Exception V_0,
             class Beetle.ChannelErrorEventArgs V_1)
    IL_0000:  ldsfld     bool b::d
    IL_0005:  brfalse.s  IL_0049

    IL_0007:  ldarg.0
    IL_0008:  ldfld      class Beetle.TcpChannel ad::f
    IL_000d:  callvirt   instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
    IL_0012:  brfalse.s  IL_0049

    .try
    {
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class Beetle.TcpChannel ad::f
      IL_001a:  callvirt   instance void Beetle.TcpChannel::c()
      IL_001f:  leave.s    IL_0049

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0021:  stloc.0
      IL_0022:  ldarg.0
      IL_0023:  ldfld      class Beetle.TcpChannel ad::f
      IL_0028:  newobj     instance void Beetle.ChannelErrorEventArgs::.ctor()
      IL_002d:  stloc.1
      IL_002e:  ldloc.1
      IL_002f:  ldarg.0
      IL_0030:  ldfld      class Beetle.TcpChannel ad::f
      IL_0035:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_003a:  ldloc.1
      IL_003b:  ldloc.0
      IL_003c:  callvirt   instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
      IL_0041:  ldloc.1
      IL_0042:  callvirt   instance void Beetle.TcpChannel::CallChannelError(class Beetle.ChannelErrorEventArgs)
      IL_0047:  leave.s    IL_0049

    }  // end handler
    IL_0049:  ret
  } // end of method ac::a

  .method public hidebysig virtual instance void 
          Dispose() cil managed
  {
    // 代码大小       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ac::Dispose

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ad::.ctor()
    IL_0006:  ret
  } // end of method ac::.ctor

} // end of class ac

.class private auto ansi beforefieldinit s
       extends ad
{
  .field public class Beetle.Package b
  .field public class Beetle.IMessage c
  .field public object d
  .method public hidebysig virtual instance void 
          a() cil managed
  {
    // 代码大小       84 (0x54)
    .maxstack  3
    .locals init (class [mscorlib]System.Exception V_0,
             class Beetle.ChannelErrorEventArgs V_1)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class Beetle.Package s::b
      IL_0006:  ldfld      class Beetle.TcpChannel Beetle.Package::Channel
      IL_000b:  ldfld      bool Beetle.TcpChannel::ab
      IL_0010:  brtrue.s   IL_0029

      IL_0012:  ldarg.0
      IL_0013:  ldfld      class Beetle.Package s::b
      IL_0018:  ldarg.0
      IL_0019:  ldfld      class Beetle.IMessage s::c
      IL_001e:  ldarg.0
      IL_001f:  ldfld      object s::d
      IL_0024:  callvirt   instance void Beetle.Package::a(class Beetle.IMessage,
                                                           object)
      IL_0029:  leave.s    IL_0053

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_002b:  stloc.0
      IL_002c:  ldarg.0
      IL_002d:  ldfld      class Beetle.TcpChannel ad::f
      IL_0032:  newobj     instance void Beetle.ChannelErrorEventArgs::.ctor()
      IL_0037:  stloc.1
      IL_0038:  ldloc.1
      IL_0039:  ldarg.0
      IL_003a:  ldfld      class Beetle.TcpChannel ad::f
      IL_003f:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_0044:  ldloc.1
      IL_0045:  ldloc.0
      IL_0046:  callvirt   instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
      IL_004b:  ldloc.1
      IL_004c:  callvirt   instance void Beetle.TcpChannel::CallChannelError(class Beetle.ChannelErrorEventArgs)
      IL_0051:  leave.s    IL_0053

    }  // end handler
    IL_0053:  ret
  } // end of method s::a

  .method public hidebysig virtual instance void 
          Dispose() cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  stfld      class Beetle.Package s::b
    IL_0007:  ldarg.0
    IL_0008:  ldnull
    IL_0009:  stfld      class Beetle.TcpChannel ad::f
    IL_000e:  ldarg.0
    IL_000f:  ldnull
    IL_0010:  stfld      class Beetle.IMessage s::c
    IL_0015:  ret
  } // end of method s::Dispose

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ad::.ctor()
    IL_0006:  ret
  } // end of method s::.ctor

} // end of class s

.class private auto ansi beforefieldinit ah
       extends ad
{
  .method public hidebysig virtual instance void 
          a() cil managed
  {
    // 代码大小       149 (0x95)
    .maxstack  4
    .locals init (class [mscorlib]System.Exception V_0,
             class Beetle.ChannelErrorEventArgs V_1)
    IL_0000:  ldsfld     bool b::c
    IL_0005:  brfalse    IL_0094

    IL_000a:  ldarg.0
    IL_000b:  ldfld      class Beetle.TcpChannel ad::f
    IL_0010:  callvirt   instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
    IL_0015:  brfalse.s  IL_0094

    .try
    {
      IL_0017:  ldarg.0
      IL_0018:  ldfld      class Beetle.TcpChannel ad::f
      IL_001d:  ldfld      class Beetle.ChannelReceiveEventArgs Beetle.TcpChannel::r
      IL_0022:  ldfld      class Beetle.ByteArraySegment Beetle.ChannelReceiveEventArgs::a
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class a ad::e
      IL_002d:  callvirt   instance uint8[] [System]System.Net.Sockets.SocketAsyncEventArgs::get_Buffer()
      IL_0032:  ldc.i4.0
      IL_0033:  ldarg.0
      IL_0034:  ldfld      class a ad::e
      IL_0039:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
      IL_003e:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                          int32,
                                                                          int32)
      IL_0043:  ldarg.0
      IL_0044:  ldfld      class Beetle.TcpChannel ad::f
      IL_0049:  ldarg.0
      IL_004a:  ldfld      class Beetle.TcpChannel ad::f
      IL_004f:  ldfld      class Beetle.ChannelReceiveEventArgs Beetle.TcpChannel::r
      IL_0054:  callvirt   instance void Beetle.TcpChannel::a(class Beetle.ChannelReceiveEventArgs)
      IL_0059:  ldarg.0
      IL_005a:  ldfld      class Beetle.TcpChannel ad::f
      IL_005f:  ldflda     int32 Beetle.TcpChannel::e
      IL_0064:  call       int32 [mscorlib]System.Threading.Interlocked::Decrement(int32&)
      IL_0069:  pop
      IL_006a:  leave.s    IL_0094

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_006c:  stloc.0
      IL_006d:  ldarg.0
      IL_006e:  ldfld      class Beetle.TcpChannel ad::f
      IL_0073:  newobj     instance void Beetle.ChannelErrorEventArgs::.ctor()
      IL_0078:  stloc.1
      IL_0079:  ldloc.1
      IL_007a:  ldarg.0
      IL_007b:  ldfld      class Beetle.TcpChannel ad::f
      IL_0080:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
      IL_0085:  ldloc.1
      IL_0086:  ldloc.0
      IL_0087:  callvirt   instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
      IL_008c:  ldloc.1
      IL_008d:  callvirt   instance void Beetle.TcpChannel::CallChannelError(class Beetle.ChannelErrorEventArgs)
      IL_0092:  leave.s    IL_0094

    }  // end handler
    IL_0094:  ret
  } // end of method ah::a

  .method public hidebysig virtual instance void 
          Dispose() cil managed
  {
    // 代码大小       26 (0x1a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class a ad::e
    IL_0006:  callvirt   instance void a::d()
    IL_000b:  ldarg.0
    IL_000c:  ldnull
    IL_000d:  stfld      class Beetle.TcpChannel ad::f
    IL_0012:  ldarg.0
    IL_0013:  ldnull
    IL_0014:  stfld      class a ad::e
    IL_0019:  ret
  } // end of method ah::Dispose

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ad::.ctor()
    IL_0006:  ret
  } // end of method ah::.ctor

} // end of class ah

.class private auto ansi beforefieldinit y
       extends [mscorlib]System.Object
       implements Beetle.IByteConverterFactory
{
  .field private class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter> a
  .method public hidebysig newslot virtual final 
          instance class Beetle.IByteConverter 
          Pop() cil managed
  {
    // 代码大小       53 (0x35)
    .maxstack  2
    .locals init (class Beetle.IByteConverter V_0,
             class y V_1)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.1
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter> y::a
      IL_000e:  callvirt   instance int32 class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter>::get_Count()
      IL_0013:  ldc.i4.0
      IL_0014:  ble.s      IL_0024

      IL_0016:  ldarg.0
      IL_0017:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter> y::a
      IL_001c:  callvirt   instance !0 class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter>::Pop()
      IL_0021:  stloc.0
      IL_0022:  leave.s    IL_0033

      IL_0024:  newobj     instance void v::.ctor()
      IL_0029:  stloc.0
      IL_002a:  leave.s    IL_0033

    }  // end .try
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0032:  endfinally
    }  // end handler
    IL_0033:  ldloc.0
    IL_0034:  ret
  } // end of method y::Pop

  .method public hidebysig newslot virtual final 
          instance void  Push(class Beetle.IByteConverter converter) cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  2
    .locals init (class y V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter> y::a
      IL_000e:  ldarg.1
      IL_000f:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter>::Push(!0)
      IL_0014:  leave.s    IL_001d

    }  // end .try
    finally
    {
      IL_0016:  ldloc.0
      IL_0017:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_001c:  endfinally
    }  // end handler
    IL_001d:  ret
  } // end of method y::Push

  .method public hidebysig newslot virtual final 
          instance void  Init(int32 count) cil managed
  {
    // 代码大小       41 (0x29)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter>::.ctor(int32)
    IL_0007:  stfld      class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter> y::a
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0024

    IL_0010:  ldarg.0
    IL_0011:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter> y::a
    IL_0016:  newobj     instance void v::.ctor()
    IL_001b:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter>::Push(!0)
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldarg.1
    IL_0026:  blt.s      IL_0010

    IL_0028:  ret
  } // end of method y::Init

  .method public hidebysig newslot virtual final 
          instance void  Clear() cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  2
    .locals init (class y V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter> y::a
      IL_000e:  brfalse.s  IL_001b

      IL_0010:  ldarg.0
      IL_0011:  ldfld      class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter> y::a
      IL_0016:  callvirt   instance void class [System]System.Collections.Generic.Stack`1<class Beetle.IByteConverter>::Clear()
      IL_001b:  leave.s    IL_0024

    }  // end .try
    finally
    {
      IL_001d:  ldloc.0
      IL_001e:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0023:  endfinally
    }  // end handler
    IL_0024:  ret
  } // end of method y::Clear

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method y::.ctor

} // end of class y

.class private auto ansi ag
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field public static string a
  .custom instance void b::.ctor() = ( 01 00 00 00 ) 
  .field public static string b
  .custom instance void t::.ctor() = ( 01 00 00 00 ) 
  .field public int32 c
  .field private class [System]System.Collections.Generic.Queue`1<class ad> d
  .field private class ad e
  .field private bool f
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       27 (0x1b)
    .maxstack  8
    IL_0000:  ldstr      "KntezZn3/HTLy0OE4rsKQNgpdkZpBMBet2b45vHtFqeU5lDeTd"
    + "KI70zrINkgMhjlYl9lYFiTnxjOV9OfTzoBd/Cz/pdkTh2ylx0uLR2fPBQoxXaUg3BtYj83r"
    + "pP3S/qMa0hkFXw6ZSTY6U2MaLRbEFMxqJo/JCEZjZkCXWGR7s+l1yek6gLbx8nGaRl9xULq"
    + "nOvM1j/zlW/1iy5Uy8IJ1rZJgzpEINOmL1TFnqICCU8/KqyhUhcJYNtc6itm00GmEsEnOuA"
    + "YUyTE1JoUFh/woSlQnxIl9YVDtmDWvoErCkYOqloIktDr49H3Dmhad2PtjaUyV/UDr1pG7a"
    + "fvcP0hPg9Bfp0LaP5Hl4mdtvNxb2ueDZObTzFkxYqYNCevbhW8ObGC8MBSwnNx5VSclsPgf"
    + "9IycbD4JfDGox52jt3D0I3pEnQ13Y0JWGCvBpXAWp9NXVFtOcRvSg3GteipY5h8LRVLXbQh"
    + "hI+JNgDAnjEN4V9knksdQZqCaKtaS8I5O/cK35aXf900Y4G/uNptitq9XO4ku1nWEBZMs8/"
    + "WhRXDMiUV8lmSmqp64/1QoHzUUujHsJZkLcSVB0MRjZUTXcv5S97li5Vks5azk4sppeyEbp"
    + "4lXWJoz9ntfkUbQSGqPB3/Zfq+pFi+W3F+bIf0GSFd6B8GarwFGQIzzDVFzCGpcjQ79VzpX"
    + "OX9DIb4H8p+mW++XFLdfysuBAY1Bw9BRMjty229EaUnmh4C"
    IL_0005:  stsfld     string ag::a
    IL_000a:  ldstr      "baxxfKKgHV9XwOpsrn1uYA=="
    IL_000f:  stsfld     string ag::b
    IL_0014:  call       class w w::g()
    IL_0019:  pop
    IL_001a:  ret
  } // end of method ag::.cctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       56 (0x38)
    .maxstack  3
    .locals init (class [mscorlib]System.Threading.WaitCallback V_0)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  stfld      int32 ag::c
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4     0x400
    IL_0010:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class ad>::.ctor(int32)
    IL_0015:  stfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
    IL_001a:  ldarg.0
    IL_001b:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0020:  ldloc.0
    IL_0021:  brtrue.s   IL_0030

    IL_0023:  ldarg.0
    IL_0024:  ldftn      instance void ag::a(object)
    IL_002a:  newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                      native int)
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback)
    IL_0036:  pop
    IL_0037:  ret
  } // end of method ag::.ctor

  .method public hidebysig instance void 
          a(class ad A_0) cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Queue`1<class ad> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.1
      IL_000e:  brtrue.s   IL_0012

      IL_0010:  leave.s    IL_0027

      IL_0012:  ldarg.0
      IL_0013:  ldfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
      IL_0018:  ldarg.1
      IL_0019:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class ad>::Enqueue(!0)
      IL_001e:  leave.s    IL_0027

    }  // end .try
    finally
    {
      IL_0020:  ldloc.0
      IL_0021:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0026:  endfinally
    }  // end handler
    IL_0027:  ret
  } // end of method ag::a

  .method public hidebysig specialname instance int32 
          d() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class ad>::get_Count()
    IL_000b:  ret
  } // end of method ag::d

  .method family hidebysig instance class ad 
          a() cil managed
  {
    // 代码大小       54 (0x36)
    .maxstack  2
    .locals init (class ad V_0,
             class [System]System.Collections.Generic.Queue`1<class ad> V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
    IL_0006:  dup
    IL_0007:  stloc.1
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class ad>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  ble.s      IL_0029

      IL_001b:  ldarg.0
      IL_001c:  ldfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
      IL_0021:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class ad>::Dequeue()
      IL_0026:  stloc.0
      IL_0027:  leave.s    IL_0034

      IL_0029:  ldnull
      IL_002a:  stloc.0
      IL_002b:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_002d:  ldloc.1
      IL_002e:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ldloc.0
    IL_0035:  ret
  } // end of method ag::a

  .method family hidebysig newslot virtual 
          instance void  b() cil managed
  {
    // 代码大小       67 (0x43)
    .maxstack  2
    .locals init (class ad V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance class ad ag::a()
    IL_0007:  stfld      class ad ag::e
    IL_000c:  ldarg.0
    IL_000d:  ldfld      class ad ag::e
    IL_0012:  brfalse.s  IL_0037

    .try
    {
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class ad ag::e
      IL_001a:  stloc.0
      .try
      {
        IL_001b:  ldarg.0
        IL_001c:  ldfld      class ad ag::e
        IL_0021:  callvirt   instance void ad::a()
        IL_0026:  leave.s    IL_0032

      }  // end .try
      finally
      {
        IL_0028:  ldloc.0
        IL_0029:  brfalse.s  IL_0031

        IL_002b:  ldloc.0
        IL_002c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0031:  endfinally
      }  // end handler
      IL_0032:  leave.s    IL_0042

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0034:  pop
      IL_0035:  leave.s    IL_0042

    }  // end handler
    IL_0037:  ldarg.0
    IL_0038:  ldfld      int32 ag::c
    IL_003d:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)
    IL_0042:  ret
  } // end of method ag::b

  .method public hidebysig newslot virtual 
          instance void  e() cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  8
    IL_0000:  ldsfld     bool Beetle.TcpUtils::ExecutionContext
    IL_0005:  brtrue.s   IL_0015

    IL_0007:  call       valuetype [mscorlib]System.Threading.AsyncFlowControl [mscorlib]System.Threading.ExecutionContext::SuppressFlow()
    IL_000c:  pop
    IL_000d:  br.s       IL_0015

    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance void ag::b()
    IL_0015:  ldarg.0
    IL_0016:  ldfld      bool ag::f
    IL_001b:  brfalse.s  IL_000f

    IL_001d:  ret
  } // end of method ag::e

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       65 (0x41)
    .maxstack  2
    .locals init (class ag V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool ag::f
      IL_000e:  brtrue.s   IL_0037

      IL_0010:  br.s       IL_0022

      IL_0012:  ldarg.0
      IL_0013:  ldfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
      IL_0018:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class ad>::Dequeue()
      IL_001d:  callvirt   instance void ad::Dispose()
      IL_0022:  ldarg.0
      IL_0023:  ldfld      class [System]System.Collections.Generic.Queue`1<class ad> ag::d
      IL_0028:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class ad>::get_Count()
      IL_002d:  ldc.i4.0
      IL_002e:  bgt.s      IL_0012

      IL_0030:  ldarg.0
      IL_0031:  ldc.i4.1
      IL_0032:  stfld      bool ag::f
      IL_0037:  leave.s    IL_0040

    }  // end .try
    finally
    {
      IL_0039:  ldloc.0
      IL_003a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_003f:  endfinally
    }  // end handler
    IL_0040:  ret
  } // end of method ag::Dispose

  .method private hidebysig instance void 
          a(object A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance void ag::e()
    IL_0006:  ret
  } // end of method ag::a

} // end of class ag

.class private auto ansi beforefieldinit b
       extends [mscorlib]System.Attribute
       implements [mscorlib]System.IDisposable
{
  .field public static string a
  .custom instance void u::.ctor() = ( 01 00 00 00 ) 
  .field public static string b
  .custom instance void d::.ctor() = ( 01 00 00 00 ) 
  .field public static bool c
  .field public static bool d
  .field public uint8[] e
  .field public int32 f
  .field public int32 g
  .field public int32 h
  .field private class a i
  .field private int32 j
  .field private class Beetle.ByteArraySegment k
  .field assembly class l l
  .field private bool m
  .method assembly hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0006:  stfld      class Beetle.ByteArraySegment b::k
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0011:  ret
  } // end of method b::.ctor

  .method public hidebysig instance bool 
          a(int32 A_0) cil managed
  {
    // 代码大小       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 b::h
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 b::g
    IL_000c:  sub
    IL_000d:  ldarg.1
    IL_000e:  cgt
    IL_0010:  ret
  } // end of method b::a

  .method public hidebysig instance bool 
          c(int32 A_0) cil managed
  {
    // 代码大小       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 b::f
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 b::h
    IL_000c:  sub
    IL_000d:  ldarg.1
    IL_000e:  clt
    IL_0010:  ldc.i4.0
    IL_0011:  ceq
    IL_0013:  ret
  } // end of method b::c

  .method public hidebysig instance void 
          a(uint8 A_0) cil managed
  {
    // 代码大小       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] b::e
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 b::g
    IL_000c:  ldarg.1
    IL_000d:  stelem.i1
    IL_000e:  ldarg.0
    IL_000f:  dup
    IL_0010:  ldfld      int32 b::g
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stfld      int32 b::g
    IL_001c:  ldarg.0
    IL_001d:  dup
    IL_001e:  ldfld      int32 b::h
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stfld      int32 b::h
    IL_002a:  ret
  } // end of method b::a

  .method public hidebysig instance void 
          a(uint8[] A_0) cil managed
  {
    .param [1]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       59 (0x3b)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0034

    IL_0004:  ldarg.0
    IL_0005:  ldfld      uint8[] b::e
    IL_000a:  ldarg.0
    IL_000b:  ldfld      int32 b::g
    IL_0010:  ldarg.1
    IL_0011:  ldloc.0
    IL_0012:  ldelem.u1
    IL_0013:  stelem.i1
    IL_0014:  ldarg.0
    IL_0015:  dup
    IL_0016:  ldfld      int32 b::g
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stfld      int32 b::g
    IL_0022:  ldarg.0
    IL_0023:  dup
    IL_0024:  ldfld      int32 b::h
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stfld      int32 b::h
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.1
    IL_0032:  add
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  ldarg.1
    IL_0036:  ldlen
    IL_0037:  conv.i4
    IL_0038:  blt.s      IL_0004

    IL_003a:  ret
  } // end of method b::a

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(uint8[] A_0) cil managed
  {
    // 代码大小       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0006:  stfld      class Beetle.ByteArraySegment b::k
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldarg.1
    IL_0013:  stfld      uint8[] b::e
    IL_0018:  ldarg.0
    IL_0019:  ldarg.1
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  stfld      int32 b::f
    IL_0021:  ldarg.0
    IL_0022:  ldc.i4.0
    IL_0023:  stfld      int32 b::g
    IL_0028:  ldarg.0
    IL_0029:  ldarg.1
    IL_002a:  ldlen
    IL_002b:  conv.i4
    IL_002c:  stfld      int32 b::h
    IL_0031:  ret
  } // end of method b::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_0,
                               class a A_1) cil managed
  {
    // 代码大小       63 (0x3f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0006:  stfld      class Beetle.ByteArraySegment b::k
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldarg.1
    IL_0013:  stfld      int32 b::f
    IL_0018:  ldarg.0
    IL_0019:  ldarg.1
    IL_001a:  newarr     [mscorlib]System.Byte
    IL_001f:  stfld      uint8[] b::e
    IL_0024:  ldarg.0
    IL_0025:  ldarg.2
    IL_0026:  stfld      class a b::i
    IL_002b:  ldarg.0
    IL_002c:  ldfld      class a b::i
    IL_0031:  ldarg.0
    IL_0032:  ldfld      uint8[] b::e
    IL_0037:  ldc.i4.0
    IL_0038:  ldarg.1
    IL_0039:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::SetBuffer(uint8[],
                                                                                                  int32,
                                                                                                  int32)
    IL_003e:  ret
  } // end of method b::.ctor

  .method public hidebysig instance void 
          d() cil managed
  {
    // 代码大小       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class a b::i
    IL_0006:  brfalse.s  IL_0014

    IL_0008:  ldarg.0
    IL_0009:  ldfld      class a b::i
    IL_000e:  ldnull
    IL_000f:  callvirt   instance void a::a(class Beetle.TcpChannel)
    IL_0014:  ret
  } // end of method b::d

  .method public hidebysig specialname instance class a 
          b() cil managed
  {
    // 代码大小       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class a b::i
    IL_0006:  ldc.i4.0
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 b::h
    IL_000d:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::SetBuffer(int32,
                                                                                                  int32)
    IL_0012:  ldarg.0
    IL_0013:  ldfld      class a b::i
    IL_0018:  ret
  } // end of method b::b

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_0) cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Beetle.ByteArraySegment::.ctor()
    IL_0006:  stfld      class Beetle.ByteArraySegment b::k
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldarg.1
    IL_0013:  stfld      int32 b::f
    IL_0018:  ldarg.0
    IL_0019:  ldarg.1
    IL_001a:  newarr     [mscorlib]System.Byte
    IL_001f:  stfld      uint8[] b::e
    IL_0024:  ret
  } // end of method b::.ctor

  .method public hidebysig instance void 
          a(class Beetle.ByteArraySegment A_0) cil managed
  {
    // 代码大小       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldfld      uint8[] Beetle.ByteArraySegment::a
    IL_0007:  stfld      uint8[] b::e
    IL_000c:  ldarg.0
    IL_000d:  ldarg.1
    IL_000e:  ldfld      int32 Beetle.ByteArraySegment::Offset
    IL_0013:  stfld      int32 b::g
    IL_0018:  ldarg.0
    IL_0019:  ldarg.1
    IL_001a:  ldfld      int32 Beetle.ByteArraySegment::Count
    IL_001f:  stfld      int32 b::h
    IL_0024:  ret
  } // end of method b::a

  .method public hidebysig instance void 
          a(class [mscorlib]System.Array A_0,
            int32 A_1,
            int32 A_2) cil managed
  {
    // 代码大小       30 (0x1e)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldarg.0
    IL_0003:  ldfld      uint8[] b::e
    IL_0008:  ldc.i4.0
    IL_0009:  ldarg.3
    IL_000a:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.0
    IL_0011:  stfld      int32 b::g
    IL_0016:  ldarg.0
    IL_0017:  ldarg.3
    IL_0018:  stfld      int32 b::h
    IL_001d:  ret
  } // end of method b::a

  .method public hidebysig instance void 
          a(class n A_0) cil managed
  {
    // 代码大小       182 (0xb6)
    .maxstack  5
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.0
    IL_0002:  stfld      int32 n::d
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 b::g
    IL_000d:  ldarg.1
    IL_000e:  ldfld      int32 n::c
    IL_0013:  add
    IL_0014:  ldarg.0
    IL_0015:  ldfld      int32 b::f
    IL_001a:  ble.s      IL_0031

    IL_001c:  ldarg.1
    IL_001d:  ldarg.0
    IL_001e:  ldfld      int32 b::f
    IL_0023:  ldarg.0
    IL_0024:  ldfld      int32 b::g
    IL_0029:  sub
    IL_002a:  stfld      int32 n::d
    IL_002f:  br.s       IL_003d

    IL_0031:  ldarg.1
    IL_0032:  ldarg.1
    IL_0033:  ldfld      int32 n::c
    IL_0038:  stfld      int32 n::d
    IL_003d:  ldarg.1
    IL_003e:  ldfld      int32 n::d
    IL_0043:  ldc.i4.0
    IL_0044:  ble.s      IL_00b5

    IL_0046:  ldarg.1
    IL_0047:  ldfld      uint8[] n::a
    IL_004c:  ldarg.1
    IL_004d:  ldfld      int32 n::b
    IL_0052:  ldarg.0
    IL_0053:  ldfld      uint8[] b::e
    IL_0058:  ldarg.0
    IL_0059:  ldfld      int32 b::g
    IL_005e:  ldarg.1
    IL_005f:  ldfld      int32 n::d
    IL_0064:  call       void [mscorlib]System.Buffer::BlockCopy(class [mscorlib]System.Array,
                                                                 int32,
                                                                 class [mscorlib]System.Array,
                                                                 int32,
                                                                 int32)
    IL_0069:  ldarg.0
    IL_006a:  dup
    IL_006b:  ldfld      int32 b::g
    IL_0070:  ldarg.1
    IL_0071:  ldfld      int32 n::d
    IL_0076:  add
    IL_0077:  stfld      int32 b::g
    IL_007c:  ldarg.0
    IL_007d:  dup
    IL_007e:  ldfld      int32 b::h
    IL_0083:  ldarg.1
    IL_0084:  ldfld      int32 n::d
    IL_0089:  add
    IL_008a:  stfld      int32 b::h
    IL_008f:  ldarg.1
    IL_0090:  ldarg.1
    IL_0091:  ldfld      int32 n::c
    IL_0096:  ldarg.1
    IL_0097:  ldfld      int32 n::d
    IL_009c:  sub
    IL_009d:  stfld      int32 n::c
    IL_00a2:  ldarg.1
    IL_00a3:  dup
    IL_00a4:  ldfld      int32 n::b
    IL_00a9:  ldarg.1
    IL_00aa:  ldfld      int32 n::d
    IL_00af:  add
    IL_00b0:  stfld      int32 n::b
    IL_00b5:  ret
  } // end of method b::a

  .method public hidebysig instance class Beetle.ByteArraySegment 
          d(int32 A_0) cil managed
  {
    // 代码大小       97 (0x61)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 b::j
    IL_0007:  ldarg.0
    IL_0008:  ldfld      int32 b::g
    IL_000d:  ldarg.1
    IL_000e:  add
    IL_000f:  ldarg.0
    IL_0010:  ldfld      int32 b::h
    IL_0015:  ble.s      IL_002a

    IL_0017:  ldarg.0
    IL_0018:  ldarg.0
    IL_0019:  ldfld      int32 b::h
    IL_001e:  ldarg.0
    IL_001f:  ldfld      int32 b::g
    IL_0024:  sub
    IL_0025:  stfld      int32 b::j
    IL_002a:  ldarg.0
    IL_002b:  ldfld      class Beetle.ByteArraySegment b::k
    IL_0030:  ldarg.0
    IL_0031:  ldfld      uint8[] b::e
    IL_0036:  ldarg.0
    IL_0037:  ldfld      int32 b::g
    IL_003c:  ldarg.0
    IL_003d:  ldfld      int32 b::j
    IL_0042:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                        int32,
                                                                        int32)
    IL_0047:  ldarg.0
    IL_0048:  dup
    IL_0049:  ldfld      int32 b::g
    IL_004e:  ldarg.0
    IL_004f:  ldfld      int32 b::j
    IL_0054:  add
    IL_0055:  stfld      int32 b::g
    IL_005a:  ldarg.0
    IL_005b:  ldfld      class Beetle.ByteArraySegment b::k
    IL_0060:  ret
  } // end of method b::d

  .method public hidebysig instance void 
          c() cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  call       instance void b::b(int32)
    IL_0007:  ret
  } // end of method b::c

  .method public hidebysig instance void 
          b(int32 A_0) cil managed
  {
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  stfld      int32 b::g
    IL_0007:  ret
  } // end of method b::b

  .method public hidebysig instance class Beetle.ByteArraySegment 
          i() cil managed
  {
    // 代码大小       31 (0x1f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.ByteArraySegment b::k
    IL_0006:  ldarg.0
    IL_0007:  ldfld      uint8[] b::e
    IL_000c:  ldc.i4.0
    IL_000d:  ldarg.0
    IL_000e:  ldfld      int32 b::h
    IL_0013:  callvirt   instance void Beetle.ByteArraySegment::SetInfo(uint8[],
                                                                        int32,
                                                                        int32)
    IL_0018:  ldarg.0
    IL_0019:  ldfld      class Beetle.ByteArraySegment b::k
    IL_001e:  ret
  } // end of method b::i

  .method public hidebysig instance void 
          a() cil managed
  {
    // 代码大小       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  stfld      bool b::m
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.0
    IL_0009:  stfld      int32 b::g
    IL_000e:  ldarg.0
    IL_000f:  ldc.i4.0
    IL_0010:  stfld      int32 b::h
    IL_0015:  ret
  } // end of method b::a

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       53 (0x35)
    .maxstack  2
    .locals init (class b V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool b::m
      IL_000e:  brtrue.s   IL_002b

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.1
      IL_0012:  stfld      bool b::m
      IL_0017:  ldarg.0
      IL_0018:  ldfld      class l b::l
      IL_001d:  brfalse.s  IL_002b

      IL_001f:  ldarg.0
      IL_0020:  ldfld      class l b::l
      IL_0025:  ldarg.0
      IL_0026:  callvirt   instance void l::a(class b)
      IL_002b:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_002d:  ldloc.0
      IL_002e:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ret
  } // end of method b::Dispose

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       33 (0x21)
    .maxstack  8
    IL_0000:  ldstr      "KntezZn3/HTLy0OE4rsKQNgpdk"
    IL_0005:  stsfld     string b::a
    IL_000a:  ldstr      "baxxfKKgHV9XwOpsrn1uYA=="
    IL_000f:  stsfld     string b::b
    IL_0014:  ldc.i4.0
    IL_0015:  stsfld     bool b::c
    IL_001a:  ldc.i4.0
    IL_001b:  stsfld     bool b::d
    IL_0020:  ret
  } // end of method b::.cctor

} // end of class b

.class private auto ansi h
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field family class [System]System.Collections.Generic.Queue`1<class a> a
  .field private int32 b
  .field private bool c
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  call       class z z::f()
    IL_0005:  pop
    IL_0006:  ret
  } // end of method h::.cctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_0) cil managed
  {
    // 代码大小       48 (0x30)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class a>::.ctor(int32)
    IL_000d:  stfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_002b

    IL_0016:  ldarg.0
    IL_0017:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_001c:  ldarg.0
    IL_001d:  call       instance class a h::c()
    IL_0022:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class a>::Enqueue(!0)
    IL_0027:  ldloc.0
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  ldarg.1
    IL_002d:  blt.s      IL_0016

    IL_002f:  ret
  } // end of method h::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_0,
                               int32 A_1) cil managed
  {
    // 代码大小       55 (0x37)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.2
    IL_0008:  stfld      int32 h::b
    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class a>::.ctor(int32)
    IL_0014:  stfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_0019:  ldc.i4.0
    IL_001a:  stloc.0
    IL_001b:  br.s       IL_0032

    IL_001d:  ldarg.0
    IL_001e:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_0023:  ldarg.0
    IL_0024:  call       instance class a h::c()
    IL_0029:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class a>::Enqueue(!0)
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.1
    IL_0030:  add
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  ldarg.1
    IL_0034:  blt.s      IL_001d

    IL_0036:  ret
  } // end of method h::.ctor

  .method public hidebysig specialname instance int32 
          b() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class a>::get_Count()
    IL_000b:  ret
  } // end of method h::b

  .method family hidebysig instance class a 
          c() cil managed
  {
    // 代码大小       40 (0x28)
    .maxstack  2
    .locals init (class a V_0)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldfld      int32 h::b
    IL_0008:  ldc.i4.0
    IL_0009:  ble.s      IL_0019

    IL_000b:  ldarg.0
    IL_000c:  ldfld      int32 h::b
    IL_0011:  newobj     instance void a::.ctor(int32)
    IL_0016:  stloc.0
    IL_0017:  br.s       IL_001f

    IL_0019:  newobj     instance void a::.ctor()
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  ldarg.0
    IL_0021:  callvirt   instance void a::a(class h)
    IL_0026:  ldloc.0
    IL_0027:  ret
  } // end of method h::c

  .method public hidebysig newslot virtual 
          instance class a  d() cil managed
  {
    // 代码大小       69 (0x45)
    .maxstack  2
    .locals init (class a V_0,
             class a V_1,
             class [System]System.Collections.Generic.Queue`1<class a> V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_0006:  dup
    IL_0007:  stloc.2
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldnull
      IL_000e:  stloc.0
      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
      IL_0015:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class a>::get_Count()
      IL_001a:  ldc.i4.0
      IL_001b:  ble.s      IL_002b

      IL_001d:  ldarg.0
      IL_001e:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
      IL_0023:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class a>::Dequeue()
      IL_0028:  stloc.0
      IL_0029:  br.s       IL_0032

      IL_002b:  ldarg.0
      IL_002c:  call       instance class a h::c()
      IL_0031:  stloc.0
      IL_0032:  ldloc.0
      IL_0033:  callvirt   instance void a::c()
      IL_0038:  ldloc.0
      IL_0039:  stloc.1
      IL_003a:  leave.s    IL_0043

    }  // end .try
    finally
    {
      IL_003c:  ldloc.2
      IL_003d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0042:  endfinally
    }  // end handler
    IL_0043:  ldloc.1
    IL_0044:  ret
  } // end of method h::d

  .method public hidebysig newslot virtual 
          instance void  a(class a A_0) cil managed
  {
    // 代码大小       42 (0x2a)
    .maxstack  2
    .locals init (class [System]System.Collections.Generic.Queue`1<class a> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.1
      IL_000e:  ldnull
      IL_000f:  callvirt   instance void a::a(class Beetle.TcpChannel)
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
      IL_001a:  ldarg.1
      IL_001b:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class a>::Enqueue(!0)
      IL_0020:  leave.s    IL_0029

    }  // end .try
    finally
    {
      IL_0022:  ldloc.0
      IL_0023:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0028:  endfinally
    }  // end handler
    IL_0029:  ret
  } // end of method h::a

  .method private hidebysig instance void 
          a() cil managed
  {
    // 代码大小       36 (0x24)
    .maxstack  2
    .locals init (class a V_0)
    IL_0000:  br.s       IL_0015

    IL_0002:  ldarg.0
    IL_0003:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_0008:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class a>::Dequeue()
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldnull
    IL_0010:  callvirt   instance void a::a(class h)
    IL_0015:  ldarg.0
    IL_0016:  ldfld      class [System]System.Collections.Generic.Queue`1<class a> h::a
    IL_001b:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class a>::get_Count()
    IL_0020:  ldc.i4.0
    IL_0021:  bgt.s      IL_0002

    IL_0023:  ret
  } // end of method h::a

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       39 (0x27)
    .maxstack  2
    .locals init (class h V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool h::c
      IL_000e:  brtrue.s   IL_001d

      IL_0010:  ldarg.0
      IL_0011:  call       instance void h::a()
      IL_0016:  ldarg.0
      IL_0017:  ldc.i4.1
      IL_0018:  stfld      bool h::c
      IL_001d:  leave.s    IL_0026

    }  // end .try
    finally
    {
      IL_001f:  ldloc.0
      IL_0020:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0025:  endfinally
    }  // end handler
    IL_0026:  ret
  } // end of method h::Dispose

} // end of class h

.class private auto ansi beforefieldinit a
       extends [System]System.Net.Sockets.SocketAsyncEventArgs
{
  .field assembly class ah a
  .field private class h b
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class Beetle.TcpChannel c
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_0) cil managed
  {
    // 代码大小       25 (0x19)
    .maxstack  4
    .locals init (uint8[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Net.Sockets.SocketAsyncEventArgs::.ctor()
    IL_0006:  ldarg.1
    IL_0007:  newarr     [mscorlib]System.Byte
    IL_000c:  stloc.0
    IL_000d:  ldarg.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.0
    IL_0010:  ldloc.0
    IL_0011:  ldlen
    IL_0012:  conv.i4
    IL_0013:  call       instance void [System]System.Net.Sockets.SocketAsyncEventArgs::SetBuffer(uint8[],
                                                                                                  int32,
                                                                                                  int32)
    IL_0018:  ret
  } // end of method a::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Net.Sockets.SocketAsyncEventArgs::.ctor()
    IL_0006:  ret
  } // end of method a::.ctor

  .method public hidebysig specialname instance class h 
          a() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class h a::b
    IL_0006:  ret
  } // end of method a::a

  .method public hidebysig specialname instance void 
          a(class h A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class h a::b
    IL_0007:  ret
  } // end of method a::a

  .method assembly hidebysig specialname 
          instance class Beetle.TcpChannel 
          b() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Beetle.TcpChannel a::c
    IL_0006:  ret
  } // end of method a::b

  .method public hidebysig specialname instance void 
          a(class Beetle.TcpChannel A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // 代码大小       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      class Beetle.TcpChannel a::c
    IL_0007:  ret
  } // end of method a::a

  .method public hidebysig instance void 
          c() cil managed
  {
    // 代码大小       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance uint8[] [System]System.Net.Sockets.SocketAsyncEventArgs::get_Buffer()
    IL_0006:  brfalse.s  IL_0017

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldarg.0
    IL_000b:  call       instance uint8[] [System]System.Net.Sockets.SocketAsyncEventArgs::get_Buffer()
    IL_0010:  ldlen
    IL_0011:  conv.i4
    IL_0012:  call       instance void [System]System.Net.Sockets.SocketAsyncEventArgs::SetBuffer(int32,
                                                                                                  int32)
    IL_0017:  ret
  } // end of method a::c

  .method public hidebysig instance void 
          d() cil managed
  {
    // 代码大小       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  call       instance void a::a(class Beetle.TcpChannel)
    IL_0007:  ldarg.0
    IL_0008:  call       instance class h a::a()
    IL_000d:  brfalse.s  IL_001b

    IL_000f:  ldarg.0
    IL_0010:  call       instance class h a::a()
    IL_0015:  ldarg.0
    IL_0016:  callvirt   instance void h::a(class a)
    IL_001b:  ret
  } // end of method a::d

  .method family hidebysig virtual instance void 
          OnCompleted(class [System]System.Net.Sockets.SocketAsyncEventArgs e) cil managed
  {
    // 代码大小       71 (0x47)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void [System]System.Net.Sockets.SocketAsyncEventArgs::OnCompleted(class [System]System.Net.Sockets.SocketAsyncEventArgs)
    IL_0007:  ldarg.1
    IL_0008:  callvirt   instance valuetype [System]System.Net.Sockets.SocketAsyncOperation [System]System.Net.Sockets.SocketAsyncEventArgs::get_LastOperation()
    IL_000d:  ldc.i4.7
    IL_000e:  bne.un.s   IL_001d

    IL_0010:  ldarg.0
    IL_0011:  ldarg.1
    IL_0012:  castclass  a
    IL_0017:  call       instance void a::a(class a)
    IL_001c:  ret

    IL_001d:  ldarg.1
    IL_001e:  callvirt   instance valuetype [System]System.Net.Sockets.SocketAsyncOperation [System]System.Net.Sockets.SocketAsyncEventArgs::get_LastOperation()
    IL_0023:  ldc.i4.4
    IL_0024:  bne.un.s   IL_0033

    IL_0026:  ldarg.0
    IL_0027:  ldarg.1
    IL_0028:  castclass  a
    IL_002d:  call       instance void a::b(class a)
    IL_0032:  ret

    IL_0033:  ldarg.1
    IL_0034:  callvirt   instance valuetype [System]System.Net.Sockets.SocketError [System]System.Net.Sockets.SocketAsyncEventArgs::get_SocketError()
    IL_0039:  brfalse.s  IL_0046

    IL_003b:  ldarg.0
    IL_003c:  call       instance class Beetle.TcpChannel a::b()
    IL_0041:  callvirt   instance void Beetle.TcpChannel::Dispose()
    IL_0046:  ret
  } // end of method a::OnCompleted

  .method assembly hidebysig instance void 
          b(class a A_0) cil managed
  {
    // 代码大小       349 (0x15d)
    .maxstack  5
    .locals init (class Beetle.TcpChannel V_0,
             object V_1,
             object[] V_2)
    .try
    {
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance class Beetle.TcpChannel a::b()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
      IL_000d:  ldc.i4.0
      IL_000e:  ble        IL_00f6

      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance valuetype [System]System.Net.Sockets.SocketError [System]System.Net.Sockets.SocketAsyncEventArgs::get_SocketError()
      IL_0019:  brtrue     IL_00f6

      IL_001e:  ldloc.0
      IL_001f:  ldfld      bool Beetle.TcpChannel::ad
      IL_0024:  brtrue     IL_00f6

      IL_0029:  ldloc.0
      IL_002a:  call       int32 [mscorlib]System.Environment::get_TickCount()
      IL_002f:  stfld      int32 Beetle.TcpChannel::TickCount
      IL_0034:  ldsfld     bool Beetle.TcpUtils::Statistics
      IL_0039:  brfalse.s  IL_0080

      IL_003b:  ldsfld     object Beetle.TcpUtils::c
      IL_0040:  dup
      IL_0041:  stloc.1
      IL_0042:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
      .try
      {
        IL_0047:  ldsflda    int64 Beetle.TcpUtils::ReceiveBytes
        IL_004c:  ldarg.1
        IL_004d:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
        IL_0052:  conv.i8
        IL_0053:  call       int64 [mscorlib]System.Threading.Interlocked::Add(int64&,
                                                                               int64)
        IL_0058:  pop
        IL_0059:  ldloc.0
        IL_005a:  ldflda     int64 Beetle.TcpChannel::ReceiveBytes
        IL_005f:  ldarg.1
        IL_0060:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
        IL_0065:  conv.i8
        IL_0066:  call       int64 [mscorlib]System.Threading.Interlocked::Add(int64&,
                                                                               int64)
        IL_006b:  pop
        IL_006c:  ldsflda    int64 Beetle.TcpUtils::ReceiveDataIO
        IL_0071:  call       int64 [mscorlib]System.Threading.Interlocked::Increment(int64&)
        IL_0076:  pop
        IL_0077:  leave.s    IL_0080

      }  // end .try
      finally
      {
        IL_0079:  ldloc.1
        IL_007a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
        IL_007f:  endfinally
      }  // end handler
      IL_0080:  ldarg.1
      IL_0081:  ldfld      class ah a::a
      IL_0086:  brtrue.s   IL_0093

      IL_0088:  ldarg.1
      IL_0089:  newobj     instance void ah::.ctor()
      IL_008e:  stfld      class ah a::a
      IL_0093:  ldarg.1
      IL_0094:  ldfld      class ah a::a
      IL_0099:  ldloc.0
      IL_009a:  stfld      class Beetle.TcpChannel ad::f
      IL_009f:  ldarg.1
      IL_00a0:  ldfld      class ah a::a
      IL_00a5:  ldarg.1
      IL_00a6:  stfld      class a ad::e
      IL_00ab:  ldloc.0
      IL_00ac:  ldfld      class ag Beetle.TcpChannel::o
      IL_00b1:  ldarg.1
      IL_00b2:  ldfld      class ah a::a
      IL_00b7:  callvirt   instance void ag::a(class ad)
      IL_00bc:  ldloc.0
      IL_00bd:  ldflda     int32 Beetle.TcpChannel::e
      IL_00c2:  call       int32 [mscorlib]System.Threading.Interlocked::Increment(int32&)
      IL_00c7:  pop
      IL_00c8:  ldsfld     int32 Beetle.TcpUtils::ChannelMaxQueueData
      IL_00cd:  ldc.i4.0
      IL_00ce:  ble.s      IL_00ee

      IL_00d0:  ldloc.0
      IL_00d1:  ldfld      int32 Beetle.TcpChannel::e
      IL_00d6:  ldsfld     int32 Beetle.TcpUtils::ChannelMaxQueueData
      IL_00db:  ble.s      IL_00ee

      IL_00dd:  ldloc.0
      IL_00de:  ldstr      "Channel Queue Data Overflow!"
      IL_00e3:  stfld      string Beetle.TcpChannel::CloseStatus
      IL_00e8:  ldloc.0
      IL_00e9:  callvirt   instance void Beetle.TcpChannel::Dispose()
      IL_00ee:  ldloc.0
      IL_00ef:  callvirt   instance void Beetle.TcpChannel::BeginReceive()
      IL_00f4:  br.s       IL_0157

      IL_00f6:  ldloc.0
      IL_00f7:  ldstr      "Receive:{0},{1},{2},{3}"
      IL_00fc:  ldc.i4.4
      IL_00fd:  newarr     [mscorlib]System.Object
      IL_0102:  stloc.2
      IL_0103:  ldloc.2
      IL_0104:  ldc.i4.0
      IL_0105:  ldarg.1
      IL_0106:  callvirt   instance valuetype [System]System.Net.Sockets.SocketError [System]System.Net.Sockets.SocketAsyncEventArgs::get_SocketError()
      IL_010b:  box        [System]System.Net.Sockets.SocketError
      IL_0110:  stelem.ref
      IL_0111:  ldloc.2
      IL_0112:  ldc.i4.1
      IL_0113:  ldarg.1
      IL_0114:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
      IL_0119:  box        [mscorlib]System.Int32
      IL_011e:  stelem.ref
      IL_011f:  ldloc.2
      IL_0120:  ldc.i4.2
      IL_0121:  ldloc.0
      IL_0122:  ldfld      bool Beetle.TcpChannel::ac
      IL_0127:  box        [mscorlib]System.Boolean
      IL_012c:  stelem.ref
      IL_012d:  ldloc.2
      IL_012e:  ldc.i4.3
      IL_012f:  ldloc.0
      IL_0130:  ldfld      class z Beetle.TcpChannel::l
      IL_0135:  ldfld      bool z::a
      IL_013a:  box        [mscorlib]System.Boolean
      IL_013f:  stelem.ref
      IL_0140:  ldloc.2
      IL_0141:  call       string [mscorlib]System.String::Format(string,
                                                                  object[])
      IL_0146:  stfld      string Beetle.TcpChannel::CloseStatus
      IL_014b:  ldarg.1
      IL_014c:  callvirt   instance void a::d()
      IL_0151:  ldloc.0
      IL_0152:  callvirt   instance void Beetle.TcpChannel::Dispose()
      IL_0157:  leave.s    IL_015c

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0159:  pop
      IL_015a:  leave.s    IL_015c

    }  // end handler
    IL_015c:  ret
  } // end of method a::b

  .method assembly hidebysig instance void 
          a(class a A_0) cil managed
  {
    // 代码大小       590 (0x24e)
    .maxstack  5
    .locals init (class Beetle.BufferWriter V_0,
             class b V_1,
             class Beetle.TcpChannel V_2,
             int32 V_3,
             int32 V_4,
             class [mscorlib]System.Exception V_5,
             class Beetle.ChannelErrorEventArgs V_6,
             object V_7,
             object[] V_8)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance object [System]System.Net.Sockets.SocketAsyncEventArgs::get_UserToken()
    IL_0006:  castclass  Beetle.BufferWriter
    IL_000b:  stloc.0
    .try
    {
      IL_000c:  ldarg.1
      IL_000d:  callvirt   instance class Beetle.TcpChannel a::b()
      IL_0012:  stloc.2
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance valuetype [System]System.Net.Sockets.SocketError [System]System.Net.Sockets.SocketAsyncEventArgs::get_SocketError()
      IL_0019:  brtrue     IL_0186

      IL_001e:  ldarg.1
      IL_001f:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
      IL_0024:  ldc.i4.0
      IL_0025:  ble        IL_0186

      IL_002a:  ldloc.2
      IL_002b:  ldfld      bool Beetle.TcpChannel::ad
      IL_0030:  brtrue     IL_0186

      IL_0035:  ldloc.2
      IL_0036:  ldfld      class z Beetle.TcpChannel::l
      IL_003b:  ldfld      bool z::a
      IL_0040:  brfalse    IL_0186

      IL_0045:  ldsfld     bool Beetle.TcpUtils::Statistics
      IL_004a:  brfalse.s  IL_0093

      IL_004c:  ldsfld     object Beetle.TcpUtils::d
      IL_0051:  dup
      IL_0052:  stloc.s    V_7
      IL_0054:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
      .try
      {
        IL_0059:  ldsflda    int64 Beetle.TcpUtils::SendBytes
        IL_005e:  ldarg.1
        IL_005f:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
        IL_0064:  conv.i8
        IL_0065:  call       int64 [mscorlib]System.Threading.Interlocked::Add(int64&,
                                                                               int64)
        IL_006a:  pop
        IL_006b:  ldloc.2
        IL_006c:  ldflda     int64 Beetle.TcpChannel::SendBytes
        IL_0071:  ldarg.1
        IL_0072:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
        IL_0077:  conv.i8
        IL_0078:  call       int64 [mscorlib]System.Threading.Interlocked::Add(int64&,
                                                                               int64)
        IL_007d:  pop
        IL_007e:  ldsflda    int64 Beetle.TcpUtils::SendDataIO
        IL_0083:  call       int64 [mscorlib]System.Threading.Interlocked::Increment(int64&)
        IL_0088:  pop
        IL_0089:  leave.s    IL_0093

      }  // end .try
      finally
      {
        IL_008b:  ldloc.s    V_7
        IL_008d:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
        IL_0092:  endfinally
      }  // end handler
      IL_0093:  ldarg.1
      IL_0094:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
      IL_0099:  ldarg.1
      IL_009a:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_Count()
      IL_009f:  bge.s      IL_00e4

      IL_00a1:  ldarg.1
      IL_00a2:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_Offset()
      IL_00a7:  ldarg.1
      IL_00a8:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
      IL_00ad:  add
      IL_00ae:  stloc.3
      IL_00af:  ldarg.1
      IL_00b0:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_Count()
      IL_00b5:  ldarg.1
      IL_00b6:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
      IL_00bb:  sub
      IL_00bc:  stloc.s    V_4
      IL_00be:  ldarg.1
      IL_00bf:  ldloc.3
      IL_00c0:  ldloc.s    V_4
      IL_00c2:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::SetBuffer(int32,
                                                                                                    int32)
      IL_00c7:  ldloc.2
      IL_00c8:  callvirt   instance class [System]System.Net.Sockets.Socket Beetle.TcpChannel::get_Socket()
      IL_00cd:  ldarg.1
      IL_00ce:  callvirt   instance bool [System]System.Net.Sockets.Socket::SendAsync(class [System]System.Net.Sockets.SocketAsyncEventArgs)
      IL_00d3:  brtrue     IL_0218

      IL_00d8:  ldarg.0
      IL_00d9:  ldarg.1
      IL_00da:  call       instance void a::a(class a)
      IL_00df:  br         IL_0218

      IL_00e4:  ldloc.0
      IL_00e5:  callvirt   instance class b Beetle.BufferWriter::b()
      IL_00ea:  stloc.1
      IL_00eb:  ldarg.1
      IL_00ec:  ldnull
      IL_00ed:  callvirt   instance void a::a(class Beetle.TcpChannel)
      IL_00f2:  ldarg.1
      IL_00f3:  ldnull
      IL_00f4:  callvirt   instance void [System]System.Net.Sockets.SocketAsyncEventArgs::set_UserToken(object)
      IL_00f9:  ldloc.0
      IL_00fa:  callvirt   instance class b Beetle.BufferWriter::c()
      IL_00ff:  stloc.1
      IL_0100:  ldloc.1
      IL_0101:  brfalse.s  IL_0109

      IL_0103:  ldloc.1
      IL_0104:  callvirt   instance void b::Dispose()
      IL_0109:  ldloc.0
      IL_010a:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> Beetle.BufferWriter::b
      IL_010f:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
      IL_0114:  ldc.i4.0
      IL_0115:  ble.s      IL_0135

      IL_0117:  ldsfld     int32 Beetle.TcpUtils::SendPacketSpacing
      IL_011c:  ldc.i4.0
      IL_011d:  ble.s      IL_0129

      IL_011f:  ldsfld     int32 Beetle.TcpUtils::SendPacketSpacing
      IL_0124:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)
      IL_0129:  ldloc.2
      IL_012a:  ldloc.0
      IL_012b:  callvirt   instance void Beetle.TcpChannel::b(class Beetle.BufferWriter)
      IL_0130:  br         IL_0218

      IL_0135:  ldsfld     int32 Beetle.TcpUtils::SendMessageSpacing
      IL_013a:  ldc.i4.0
      IL_013b:  ble.s      IL_0147

      IL_013d:  ldsfld     int32 Beetle.TcpUtils::SendMessageSpacing
      IL_0142:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)
      IL_0147:  ldloc.2
      IL_0148:  ldfld      bool Beetle.TcpChannel::EnabledSendCompeletedEvent
      IL_014d:  brfalse.s  IL_0172

      IL_014f:  ldloc.2
      IL_0150:  ldfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
      IL_0155:  ldc.i4.1
      IL_0156:  stfld      bool Beetle.SendMessageCompletedArgs::Success
      IL_015b:  ldloc.2
      IL_015c:  ldfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
      IL_0161:  ldloc.0
      IL_0162:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
      IL_0167:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.SendMessageCompletedArgs::Messages
      IL_016c:  ldloc.2
      IL_016d:  callvirt   instance void Beetle.TcpChannel::e()
      IL_0172:  ldloc.2
      IL_0173:  ldc.i4.0
      IL_0174:  callvirt   instance void Beetle.TcpChannel::a(bool)
      IL_0179:  ldloc.2
      IL_017a:  ldnull
      IL_017b:  callvirt   instance bool Beetle.TcpChannel::Send(object)
      IL_0180:  pop
      IL_0181:  br         IL_0218

      IL_0186:  ldloc.2
      IL_0187:  ldfld      bool Beetle.TcpChannel::EnabledSendCompeletedEvent
      IL_018c:  brfalse.s  IL_01b1

      IL_018e:  ldloc.2
      IL_018f:  ldfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
      IL_0194:  ldc.i4.0
      IL_0195:  stfld      bool Beetle.SendMessageCompletedArgs::Success
      IL_019a:  ldloc.2
      IL_019b:  ldfld      class Beetle.SendMessageCompletedArgs Beetle.TcpChannel::aa
      IL_01a0:  ldloc.0
      IL_01a1:  ldfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.BufferWriter::i
      IL_01a6:  stfld      class [mscorlib]System.Collections.Generic.IList`1<class Beetle.IMessage> Beetle.SendMessageCompletedArgs::Messages
      IL_01ab:  ldloc.2
      IL_01ac:  callvirt   instance void Beetle.TcpChannel::e()
      IL_01b1:  ldloc.2
      IL_01b2:  ldstr      "Send:{0},{1},{2},{3}"
      IL_01b7:  ldc.i4.4
      IL_01b8:  newarr     [mscorlib]System.Object
      IL_01bd:  stloc.s    V_8
      IL_01bf:  ldloc.s    V_8
      IL_01c1:  ldc.i4.0
      IL_01c2:  ldarg.1
      IL_01c3:  callvirt   instance valuetype [System]System.Net.Sockets.SocketError [System]System.Net.Sockets.SocketAsyncEventArgs::get_SocketError()
      IL_01c8:  box        [System]System.Net.Sockets.SocketError
      IL_01cd:  stelem.ref
      IL_01ce:  ldloc.s    V_8
      IL_01d0:  ldc.i4.1
      IL_01d1:  ldarg.1
      IL_01d2:  callvirt   instance int32 [System]System.Net.Sockets.SocketAsyncEventArgs::get_BytesTransferred()
      IL_01d7:  box        [mscorlib]System.Int32
      IL_01dc:  stelem.ref
      IL_01dd:  ldloc.s    V_8
      IL_01df:  ldc.i4.2
      IL_01e0:  ldloc.2
      IL_01e1:  ldfld      bool Beetle.TcpChannel::ac
      IL_01e6:  box        [mscorlib]System.Boolean
      IL_01eb:  stelem.ref
      IL_01ec:  ldloc.s    V_8
      IL_01ee:  ldc.i4.3
      IL_01ef:  ldloc.2
      IL_01f0:  ldfld      class z Beetle.TcpChannel::l
      IL_01f5:  ldfld      bool z::a
      IL_01fa:  box        [mscorlib]System.Boolean
      IL_01ff:  stelem.ref
      IL_0200:  ldloc.s    V_8
      IL_0202:  call       string [mscorlib]System.String::Format(string,
                                                                  object[])
      IL_0207:  stfld      string Beetle.TcpChannel::CloseStatus
      IL_020c:  ldloc.0
      IL_020d:  callvirt   instance void Beetle.BufferWriter::Dispose()
      IL_0212:  ldloc.2
      IL_0213:  callvirt   instance void Beetle.TcpChannel::Dispose()
      IL_0218:  leave.s    IL_024d

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_021a:  stloc.s    V_5
      .try
      {
        IL_021c:  ldarg.0
        IL_021d:  call       instance class Beetle.TcpChannel a::b()
        IL_0222:  newobj     instance void Beetle.ChannelErrorEventArgs::.ctor()
        IL_0227:  stloc.s    V_6
        IL_0229:  ldloc.s    V_6
        IL_022b:  ldarg.0
        IL_022c:  call       instance class Beetle.TcpChannel a::b()
        IL_0231:  stfld      class Beetle.TcpChannel Beetle.ChannelEventArgs::Channel
        IL_0236:  ldloc.s    V_6
        IL_0238:  ldloc.s    V_5
        IL_023a:  callvirt   instance void Beetle.ChannelErrorEventArgs::set_Exception(class [mscorlib]System.Exception)
        IL_023f:  ldloc.s    V_6
        IL_0241:  callvirt   instance void Beetle.TcpChannel::CallChannelError(class Beetle.ChannelErrorEventArgs)
        IL_0246:  leave.s    IL_024b

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0248:  pop
        IL_0249:  leave.s    IL_024b

      }  // end handler
      IL_024b:  leave.s    IL_024d

    }  // end handler
    IL_024d:  ret
  } // end of method a::a

} // end of class a

.class private auto ansi l
       extends [mscorlib]System.Object
       implements [mscorlib]System.IDisposable
{
  .field public static int32 a
  .field static family class [mscorlib]System.Collections.Generic.List`1<class l> b
  .field private static int32 c
  .field private bool d
  .field private object e
  .field family class [System]System.Collections.Generic.Queue`1<class b> f
  .field family int32 g
  .field private bool h
  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // 代码大小       23 (0x17)
    .maxstack  8
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class l>::.ctor()
    IL_0005:  stsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
    IL_000a:  ldc.i4.0
    IL_000b:  stsfld     int32 l::c
    IL_0010:  call       class ae ae::f()
    IL_0015:  pop
    IL_0016:  ret
  } // end of method l::.cctor

  .method public hidebysig static void  a(int32 A_0,
                                          int32 A_1) cil managed
  {
    // 代码大小       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldsfld     int32 Beetle.TcpUtils::SendBufferLength
    IL_0007:  call       void l::a(int32,
                                   int32,
                                   int32)
    IL_000c:  ret
  } // end of method l::a

  .method public hidebysig specialname instance int32 
          f() cil managed
  {
    // 代码大小       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
    IL_0006:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_000b:  ret
  } // end of method l::f

  .method public hidebysig static void  a(int32 A_0,
                                          int32 A_1,
                                          int32 A_2) cil managed
  {
    // 代码大小       51 (0x33)
    .maxstack  4
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.List`1<class l> V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
    IL_0005:  dup
    IL_0006:  stloc.1
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldc.i4.0
      IL_000d:  stloc.0
      IL_000e:  br.s       IL_0025

      IL_0010:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
      IL_0015:  ldarg.1
      IL_0016:  ldarg.2
      IL_0017:  newobj     instance void l::.ctor(int32,
                                                  int32)
      IL_001c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class l>::Add(!0)
      IL_0021:  ldloc.0
      IL_0022:  ldc.i4.1
      IL_0023:  add
      IL_0024:  stloc.0
      IL_0025:  ldloc.0
      IL_0026:  ldarg.0
      IL_0027:  blt.s      IL_0010

      IL_0029:  leave.s    IL_0032

    }  // end .try
    finally
    {
      IL_002b:  ldloc.1
      IL_002c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0031:  endfinally
    }  // end handler
    IL_0032:  ret
  } // end of method l::a

  .method public hidebysig static void  d() cil managed
  {
    // 代码大小       69 (0x45)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.List`1<class l> V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
    IL_0005:  dup
    IL_0006:  stloc.1
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldc.i4.0
      IL_000d:  stloc.0
      IL_000e:  br.s       IL_0024

      IL_0010:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class l>::get_Item(int32)
      IL_001b:  callvirt   instance void l::Dispose()
      IL_0020:  ldloc.0
      IL_0021:  ldc.i4.1
      IL_0022:  add
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
      IL_002a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class l>::get_Count()
      IL_002f:  blt.s      IL_0010

      IL_0031:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
      IL_0036:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class l>::Clear()
      IL_003b:  leave.s    IL_0044

    }  // end .try
    finally
    {
      IL_003d:  ldloc.1
      IL_003e:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0043:  endfinally
    }  // end handler
    IL_0044:  ret
  } // end of method l::d

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IList`1<int32> 
          c() cil managed
  {
    // 代码大小       56 (0x38)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0,
             int32 V_1)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0029

    IL_000a:  ldloc.0
    IL_000b:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
    IL_0010:  ldloc.1
    IL_0011:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class l>::get_Item(int32)
    IL_0016:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
    IL_001b:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
    IL_0020:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
    IL_002f:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class l>::get_Count()
    IL_0034:  blt.s      IL_000a

    IL_0036:  ldloc.0
    IL_0037:  ret
  } // end of method l::c

  .method public hidebysig static class l 
          b() cil managed
  {
    // 代码大小       74 (0x4a)
    .maxstack  2
    .locals init (class l V_0,
             class [mscorlib]System.Collections.Generic.List`1<class l> V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
    IL_0005:  dup
    IL_0006:  stloc.1
    IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000c:  ldsfld     int32 l::c
      IL_0011:  ldc.i4.1
      IL_0012:  add
      IL_0013:  stsfld     int32 l::c
      IL_0018:  ldsfld     int32 l::c
      IL_001d:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
      IL_0022:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.List`1<class l>::get_Count()
      IL_0027:  blt.s      IL_002f

      IL_0029:  ldc.i4.0
      IL_002a:  stsfld     int32 l::c
      IL_002f:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<class l> l::b
      IL_0034:  ldsfld     int32 l::c
      IL_0039:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class l>::get_Item(int32)
      IL_003e:  stloc.0
      IL_003f:  leave.s    IL_0048

    }  // end .try
    finally
    {
      IL_0041:  ldloc.1
      IL_0042:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0047:  endfinally
    }  // end handler
    IL_0048:  ldloc.0
    IL_0049:  ret
  } // end of method l::b

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_0,
                               int32 A_1) cil managed
  {
    // 代码大小       67 (0x43)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0006:  stfld      object l::e
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldarg.2
    IL_0013:  stfld      int32 l::g
    IL_0018:  ldarg.0
    IL_0019:  ldarg.1
    IL_001a:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class b>::.ctor(int32)
    IL_001f:  stfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
    IL_0024:  ldc.i4.0
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_003e

    IL_0028:  ldarg.0
    IL_0029:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
    IL_002e:  ldarg.0
    IL_002f:  ldarg.2
    IL_0030:  call       instance class b l::a(int32)
    IL_0035:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class b>::Enqueue(!0)
    IL_003a:  ldloc.0
    IL_003b:  ldc.i4.1
    IL_003c:  add
    IL_003d:  stloc.0
    IL_003e:  ldloc.0
    IL_003f:  ldarg.1
    IL_0040:  blt.s      IL_0028

    IL_0042:  ret
  } // end of method l::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_0,
                               int32 A_1,
                               bool A_2) cil managed
  {
    // 代码大小       74 (0x4a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void [mscorlib]System.Object::.ctor()
    IL_0006:  stfld      object l::e
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ldarg.0
    IL_0012:  ldarg.3
    IL_0013:  stfld      bool l::d
    IL_0018:  ldarg.0
    IL_0019:  ldarg.2
    IL_001a:  stfld      int32 l::g
    IL_001f:  ldarg.0
    IL_0020:  ldarg.1
    IL_0021:  newobj     instance void class [System]System.Collections.Generic.Queue`1<class b>::.ctor(int32)
    IL_0026:  stfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
    IL_002b:  ldc.i4.0
    IL_002c:  stloc.0
    IL_002d:  br.s       IL_0045

    IL_002f:  ldarg.0
    IL_0030:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
    IL_0035:  ldarg.0
    IL_0036:  ldarg.2
    IL_0037:  call       instance class b l::a(int32)
    IL_003c:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class b>::Enqueue(!0)
    IL_0041:  ldloc.0
    IL_0042:  ldc.i4.1
    IL_0043:  add
    IL_0044:  stloc.0
    IL_0045:  ldloc.0
    IL_0046:  ldarg.1
    IL_0047:  blt.s      IL_002f

    IL_0049:  ret
  } // end of method l::.ctor

  .method family hidebysig instance class b 
          a(int32 A_0) cil managed
  {
    // 代码大小       38 (0x26)
    .maxstack  3
    .locals init (class b V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool l::d
    IL_0006:  brfalse.s  IL_0016

    IL_0008:  ldarg.1
    IL_0009:  newobj     instance void a::.ctor()
    IL_000e:  newobj     instance void b::.ctor(int32,
                                                class a)
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_001d

    IL_0016:  ldarg.1
    IL_0017:  newobj     instance void b::.ctor(int32)
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldarg.0
    IL_001f:  stfld      class l b::l
    IL_0024:  ldloc.0
    IL_0025:  ret
  } // end of method l::a

  .method public hidebysig newslot virtual 
          instance class b  g() cil managed
  {
    // 代码大小       72 (0x48)
    .maxstack  2
    .locals init (class b V_0,
             class b V_1,
             object V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object l::e
    IL_0006:  dup
    IL_0007:  stloc.2
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
      IL_0013:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
      IL_0018:  ldc.i4.0
      IL_0019:  bgt.s      IL_0029

      IL_001b:  ldarg.0
      IL_001c:  ldarg.0
      IL_001d:  ldfld      int32 l::g
      IL_0022:  call       instance class b l::a(int32)
      IL_0027:  br.s       IL_0034

      IL_0029:  ldarg.0
      IL_002a:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
      IL_002f:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
      IL_0034:  stloc.0
      IL_0035:  ldloc.0
      IL_0036:  callvirt   instance void b::a()
      IL_003b:  ldloc.0
      IL_003c:  stloc.1
      IL_003d:  leave.s    IL_0046

    }  // end .try
    finally
    {
      IL_003f:  ldloc.2
      IL_0040:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0045:  endfinally
    }  // end handler
    IL_0046:  ldloc.1
    IL_0047:  ret
  } // end of method l::g

  .method public hidebysig newslot virtual 
          instance void  a(class b A_0) cil managed
  {
    // 代码大小       35 (0x23)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object l::e
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
      IL_0013:  ldarg.1
      IL_0014:  callvirt   instance void class [System]System.Collections.Generic.Queue`1<class b>::Enqueue(!0)
      IL_0019:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method l::a

  .method private hidebysig instance void 
          a() cil managed
  {
    // 代码大小       56 (0x38)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      object l::e
    IL_0006:  dup
    IL_0007:  stloc.0
    IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_000d:  br.s       IL_0020

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
      IL_0015:  callvirt   instance !0 class [System]System.Collections.Generic.Queue`1<class b>::Dequeue()
      IL_001a:  ldnull
      IL_001b:  stfld      class l b::l
      IL_0020:  ldarg.0
      IL_0021:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
      IL_0026:  callvirt   instance int32 class [System]System.Collections.Generic.Queue`1<class b>::get_Count()
      IL_002b:  ldc.i4.0
      IL_002c:  bgt.s      IL_000f

      IL_002e:  leave.s    IL_0037

    }  // end .try
    finally
    {
      IL_0030:  ldloc.0
      IL_0031:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0036:  endfinally
    }  // end handler
    IL_0037:  ret
  } // end of method l::a

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // 代码大小       39 (0x27)
    .maxstack  2
    .locals init (class l V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Threading.Monitor::Enter(object)
    .try
    {
      IL_0008:  ldarg.0
      IL_0009:  ldfld      bool l::h
      IL_000e:  brtrue.s   IL_001d

      IL_0010:  ldarg.0
      IL_0011:  call       instance void l::a()
      IL_0016:  ldarg.0
      IL_0017:  ldc.i4.1
      IL_0018:  stfld      bool l::h
      IL_001d:  leave.s    IL_0026

    }  // end .try
    finally
    {
      IL_001f:  ldloc.0
      IL_0020:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0025:  endfinally
    }  // end handler
    IL_0026:  ret
  } // end of method l::Dispose

  .method public hidebysig instance void 
          e() cil managed
  {
    // 代码大小       54 (0x36)
    .maxstack  1
    .locals init (class b V_0,
             valuetype [System]System.Collections.Generic.Queue`1/Enumerator<class b> V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [System]System.Collections.Generic.Queue`1<class b> l::f
    IL_0006:  callvirt   instance valuetype [System]System.Collections.Generic.Queue`1/Enumerator<!0> class [System]System.Collections.Generic.Queue`1<class b>::GetEnumerator()
    IL_000b:  stloc.1
    .try
    {
      IL_000c:  br.s       IL_001c

      IL_000e:  ldloca.s   V_1
      IL_0010:  call       instance !0 valuetype [System]System.Collections.Generic.Queue`1/Enumerator<class b>::get_Current()
      IL_0015:  stloc.0
      IL_0016:  ldloc.0
      IL_0017:  callvirt   instance void b::d()
      IL_001c:  ldloca.s   V_1
      IL_001e:  call       instance bool valuetype [System]System.Collections.Generic.Queue`1/Enumerator<class b>::MoveNext()
      IL_0023:  brtrue.s   IL_000e

      IL_0025:  leave.s    IL_0035

    }  // end .try
    finally
    {
      IL_0027:  ldloca.s   V_1
      IL_0029:  constrained. valuetype [System]System.Collections.Generic.Queue`1/Enumerator<class b>
      IL_002f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0034:  endfinally
    }  // end handler
    IL_0035:  ret
  } // end of method l::e

} // end of class l


// =============================================================

// *********** 反汇编完成 ***********************
// 警告: 创建了 Win32 资源文件 D:\worktable\FutSystems\TL_Update\lib_old\Beetle.res
