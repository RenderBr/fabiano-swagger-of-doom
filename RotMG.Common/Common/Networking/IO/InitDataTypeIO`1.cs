// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.IO.InitDataTypeIO`1
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace RotMG.Common.Networking.IO
{
  public class InitDataTypeIO<T>
  {
    private readonly Type typeT;
    private readonly bool isValueType;
    private readonly DynamicMethod load;
    private readonly DynamicMethod save;
    private readonly DynamicMethod size;
    private readonly ILGenerator loadGen;
    private readonly ILGenerator saveGen;
    private readonly ILGenerator sizeGen;
    private int sizeAccumulator;

    internal InitDataTypeIO()
    {
      this.typeT = typeof (T);
      this.isValueType = this.typeT.IsValueType;
      this.load = new DynamicMethod("Load_" + this.typeT.FullName, this.typeT, new Type[1]
      {
        typeof (ByteBuffer).MakeByRefType()
      }, true);
      this.save = new DynamicMethod("Save_" + this.typeT.FullName, typeof (void), new Type[2]
      {
        typeof (ByteBuffer).MakeByRefType(),
        this.typeT
      }, true);
      this.size = new DynamicMethod("Size_" + this.typeT.FullName, typeof (int), new Type[1]
      {
        this.typeT
      }, true);
      this.loadGen = this.load.GetILGenerator();
      this.saveGen = this.save.GetILGenerator();
      this.sizeGen = this.size.GetILGenerator();
      this.Init();
    }

    private void Init()
    {
      this.loadGen.DeclareLocal(typeof (T));
      this.loadGen.DeclareLocal(typeof (int));
      if (this.isValueType)
      {
        this.loadGen.Emit(OpCodes.Ldloca_S, (byte) 0);
        this.loadGen.Emit(OpCodes.Initobj, typeof (T));
      }
      else
      {
        this.loadGen.Emit(OpCodes.Newobj, typeof (T).GetConstructor(Type.EmptyTypes));
        this.loadGen.Emit(OpCodes.Stloc_0);
      }
      this.sizeGen.DeclareLocal(typeof (int));
    }

    private void EmitLoadCall(string method)
    {
      this.loadGen.Emit(OpCodes.Ldarg_0);
      this.loadGen.Emit(OpCodes.Call, typeof (ByteBuffer).GetMethod(method));
    }

    private void EmitLoad(MemberInfo member, FieldType type)
    {
      if (this.isValueType)
        this.loadGen.Emit(OpCodes.Ldloca_S, (byte) 0);
      else
        this.loadGen.Emit(OpCodes.Ldloc_0);
      switch (type)
      {
        case FieldType.UTF:
          this.EmitLoadCall("ReadUTF");
          break;
        case FieldType.UTF32:
          this.EmitLoadCall("ReadUTF32");
          break;
        case FieldType.Byte:
          this.EmitLoadCall("ReadByte");
          break;
        case FieldType.Bool:
          this.EmitLoadCall("ReadBool");
          break;
        case FieldType.Int16:
          this.EmitLoadCall("ReadUInt16");
          break;
        case FieldType.Int32:
          this.EmitLoadCall("ReadUInt32");
          break;
        case FieldType.Int64:
          this.EmitLoadCall("ReadUInt64");
          break;
        case FieldType.Float32:
          this.EmitLoadCall("ReadFloat32");
          break;
        case FieldType.Float64:
          this.EmitLoadCall("ReadFloat64");
          break;
        case FieldType.Buffer16:
          this.EmitLoadCall("ReadBuffer16");
          break;
        case FieldType.UTF32Array:
          this.EmitLoadCall("ReadUTF32Array");
          break;
        case FieldType.IntArray:
          this.EmitLoadCall("ReadIntArray");
          break;
        case FieldType.DataArray:
          Type type1 = (object) (member as PropertyInfo) == null ? ((FieldInfo) member).FieldType.GetElementType() : ((PropertyInfo) member).PropertyType.GetElementType();
          this.loadGen.Emit(OpCodes.Ldarg_0);
          this.loadGen.Emit(OpCodes.Call, typeof (IOHelper).GetMethod("ReadDataArray").MakeGenericMethod(type1));
          break;
        case FieldType.DataType:
          Type type2 = (object) (member as PropertyInfo) == null ? ((FieldInfo) member).FieldType : ((PropertyInfo) member).PropertyType;
          this.loadGen.Emit(OpCodes.Ldarg_0);
          this.loadGen.Emit(OpCodes.Call, typeof (DataTypeIO).GetMethod("Load").MakeGenericMethod(type2));
          break;
        default:
          throw new NotSupportedException();
      }
      if ((object) (member as PropertyInfo) != null)
        this.loadGen.Emit(OpCodes.Call, ((PropertyInfo) member).GetSetMethod());
      else
        this.loadGen.Emit(OpCodes.Stfld, (FieldInfo) member);
    }

    private void EmitSaveCall(string method, Action loadField)
    {
      this.saveGen.Emit(OpCodes.Ldarg_0);
      loadField();
      this.saveGen.Emit(OpCodes.Call, typeof (ByteBuffer).GetMethod(method));
    }

    private void EmitSave(MemberInfo member, FieldType type)
    {
      Action loadField = (Action) (() =>
      {
        this.saveGen.Emit(OpCodes.Ldarg_1);
        if ((object) (member as PropertyInfo) != null)
          this.saveGen.Emit(OpCodes.Call, ((PropertyInfo) member).GetGetMethod());
        else
          this.saveGen.Emit(OpCodes.Ldfld, (FieldInfo) member);
      });
      switch (type)
      {
        case FieldType.UTF:
          this.EmitSaveCall("WriteUTF", loadField);
          break;
        case FieldType.UTF32:
          this.EmitSaveCall("WriteUTF32", loadField);
          break;
        case FieldType.Byte:
          this.EmitSaveCall("WriteByte", loadField);
          break;
        case FieldType.Bool:
          this.EmitSaveCall("WriteBool", loadField);
          break;
        case FieldType.Int16:
          this.EmitSaveCall("WriteUInt16", loadField);
          break;
        case FieldType.Int32:
          this.EmitSaveCall("WriteUInt32", loadField);
          break;
        case FieldType.Int64:
          this.EmitSaveCall("WriteUInt64", loadField);
          break;
        case FieldType.Float32:
          this.EmitSaveCall("WriteFloat32", loadField);
          break;
        case FieldType.Float64:
          this.EmitSaveCall("WriteFloat64", loadField);
          break;
        case FieldType.Buffer16:
          this.EmitSaveCall("WriteBuffer16", loadField);
          break;
        case FieldType.UTF32Array:
          this.EmitSaveCall("WriteUTF32Array", loadField);
          break;
        case FieldType.IntArray:
          this.EmitSaveCall("WriteIntArray", loadField);
          break;
        case FieldType.DataArray:
          Type type1 = (object) (member as PropertyInfo) == null ? ((FieldInfo) member).FieldType.GetElementType() : ((PropertyInfo) member).PropertyType.GetElementType();
          this.saveGen.Emit(OpCodes.Ldarg_0);
          loadField();
          this.saveGen.Emit(OpCodes.Call, typeof (IOHelper).GetMethod("WriteDataArray").MakeGenericMethod(type1));
          break;
        case FieldType.DataType:
          Type type2 = (object) (member as PropertyInfo) == null ? ((FieldInfo) member).FieldType : ((PropertyInfo) member).PropertyType;
          this.saveGen.Emit(OpCodes.Ldarg_0);
          loadField();
          this.saveGen.Emit(OpCodes.Call, typeof (DataTypeIO).GetMethod("Save").MakeGenericMethod(type2));
          break;
        default:
          throw new NotSupportedException();
      }
    }

    private void EmitSizeCall(string method, Action loadField)
    {
      this.sizeGen.Emit(OpCodes.Ldloca_S, (byte) 0);
      loadField();
      this.sizeGen.Emit(OpCodes.Call, typeof (IOHelper).GetMethod(method));
    }

    private void EmitSize(MemberInfo member, FieldType type)
    {
      switch (type)
      {
        case FieldType.Byte:
        case FieldType.Bool:
          ++this.sizeAccumulator;
          break;
        case FieldType.Int16:
          this.sizeAccumulator += 2;
          break;
        case FieldType.Int32:
        case FieldType.Float32:
          this.sizeAccumulator += 4;
          break;
        case FieldType.Int64:
        case FieldType.Float64:
          this.sizeAccumulator += 8;
          break;
        default:
          if (this.sizeAccumulator != 0)
          {
            this.sizeGen.Emit(OpCodes.Ldloc_0);
            this.sizeGen.Emit(OpCodes.Ldc_I4, this.sizeAccumulator);
            this.sizeGen.Emit(OpCodes.Add);
            this.sizeGen.Emit(OpCodes.Stloc_0);
            this.sizeAccumulator = 0;
          }
          Action loadField = (Action) (() =>
          {
            this.sizeGen.Emit(OpCodes.Ldarg_0);
            if ((object) (member as PropertyInfo) != null)
              this.sizeGen.Emit(OpCodes.Call, ((PropertyInfo) member).GetGetMethod());
            else
              this.sizeGen.Emit(OpCodes.Ldfld, (FieldInfo) member);
          });
          switch (type)
          {
            case FieldType.UTF:
              this.EmitSizeCall("NextUTF", loadField);
              return;
            case FieldType.UTF32:
              this.EmitSizeCall("NextUTF32", loadField);
              return;
            case FieldType.Buffer16:
              this.EmitSizeCall("NextBuffer16", loadField);
              return;
            case FieldType.UTF32Array:
              this.EmitSizeCall("NextUTF32Array", loadField);
              return;
            case FieldType.IntArray:
              this.EmitSizeCall("NextIntArray", loadField);
              return;
            case FieldType.DataArray:
              Type type1 = (object) (member as PropertyInfo) == null ? ((FieldInfo) member).FieldType.GetElementType() : ((PropertyInfo) member).PropertyType.GetElementType();
              this.sizeGen.Emit(OpCodes.Ldloc_0);
              loadField();
              this.sizeGen.Emit(OpCodes.Call, typeof (IOHelper).GetMethod("SizeOfDataArray").MakeGenericMethod(type1));
              this.sizeGen.Emit(OpCodes.Add);
              this.sizeGen.Emit(OpCodes.Stloc_0);
              return;
            case FieldType.DataType:
              Type type2 = (object) (member as PropertyInfo) == null ? ((FieldInfo) member).FieldType : ((PropertyInfo) member).PropertyType;
              this.sizeGen.Emit(OpCodes.Ldloc_0);
              loadField();
              this.sizeGen.Emit(OpCodes.Call, typeof (DataTypeIO).GetMethod("SizeOf").MakeGenericMethod(type2));
              this.sizeGen.Emit(OpCodes.Add);
              this.sizeGen.Emit(OpCodes.Stloc_0);
              return;
            default:
              throw new NotSupportedException();
          }
      }
    }

    public InitDataTypeIO<T> Field<TField>(Expression<Func<T, TField>> field, FieldType type)
    {
      if (!(field.Body is MemberExpression))
        throw new ArgumentException("Invalid lambda expression; expected MemberExpression.", nameof (field));
      MemberInfo member = ((MemberExpression) field.Body).Member;
      if ((object) (member as PropertyInfo) == null && (object) (member as FieldInfo) == null)
        throw new ArgumentException("Invalid MemberInfo; expected PropertyInfo/FieldInfo.", nameof (field));
      this.EmitLoad(member, type);
      this.EmitSave(member, type);
      this.EmitSize(member, type);
      return this;
    }

    public void End()
    {
      this.loadGen.Emit(OpCodes.Ldloc_0);
      this.loadGen.Emit(OpCodes.Ret);
      DataTypeIO.IOData<T>.LoadFunc = (DataTypeIO.LoadFunc<T>) this.load.CreateDelegate(typeof (DataTypeIO.LoadFunc<T>));
      this.saveGen.Emit(OpCodes.Ret);
      DataTypeIO.IOData<T>.SaveFunc = (DataTypeIO.SaveFunc<T>) this.save.CreateDelegate(typeof (DataTypeIO.SaveFunc<T>));
      if (this.sizeAccumulator != 0)
      {
        this.sizeGen.Emit(OpCodes.Ldloc_0);
        this.sizeGen.Emit(OpCodes.Ldc_I4, this.sizeAccumulator);
        this.sizeGen.Emit(OpCodes.Add);
      }
      else
        this.sizeGen.Emit(OpCodes.Ldloc_0);
      this.sizeGen.Emit(OpCodes.Ret);
      DataTypeIO.IOData<T>.SizeFunc = (DataTypeIO.SizeFunc<T>) this.size.CreateDelegate(typeof (DataTypeIO.SizeFunc<T>));
      DataTypeIO.IOData<T>.Initialized = true;
    }
  }
}
