// Decompiled with JetBrains decompiler
// Type: RotMG.Common.Networking.IO.DataTypeIO
// Assembly: RotMG.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E215226E-DA58-471F-A0E6-90796E3D60B1
// Assembly location: C:\Users\julia\Documents\GitHub\fabiano-swagger-of-doom\RotMG.Common.dll

using RotMG.Common.IO;
using System;
using System.Reflection;

#nullable disable
namespace RotMG.Common.Networking.IO
{
  public class DataTypeIO
  {
    static DataTypeIO()
    {
      foreach (Type type1 in typeof (DataTypeIO).Assembly.GetTypes())
      {
        if (!type1.IsAbstract && !type1.IsInterface && type1.GetCustomAttributes(typeof (DataTypeAttribute), true).Length != 0)
        {
          MethodInfo method1 = type1.GetMethod("InitIO", BindingFlags.Static | BindingFlags.Public);
          if (method1 == (MethodInfo) null)
          {
            MethodInfo method2 = type1.GetMethod("Load");
            MethodInfo method3 = type1.GetMethod("Save");
            MethodInfo method4 = type1.GetMethod("SizeOf");
            if (method2 == (MethodInfo) null || method3 == (MethodInfo) null || method4 == (MethodInfo) null)
              throw new NotSupportedException("Invalid data type.");
            Type type2 = typeof (DataTypeIO.IOData<>).MakeGenericType(type1);
            Delegate delegate1 = Delegate.CreateDelegate(typeof (DataTypeIO.LoadFunc<>).MakeGenericType(type1), method2);
            type2.GetField("LoadFunc").SetValue((object) null, (object) delegate1);
            Delegate delegate2 = Delegate.CreateDelegate(typeof (DataTypeIO.SaveFunc<>).MakeGenericType(type1), method3);
            type2.GetField("SaveFunc").SetValue((object) null, (object) delegate2);
            Delegate delegate3 = Delegate.CreateDelegate(typeof (DataTypeIO.SizeFunc<>).MakeGenericType(type1), method4);
            type2.GetField("SizeFunc").SetValue((object) null, (object) delegate3);
            type2.GetField("Initialized").SetValue((object) null, (object) true);
          }
          else
            method1.Invoke((object) null, (object[]) null);
        }
      }
    }

    public static void Init<T>(Action<InitDataTypeIO<T>> initFunc)
    {
      initFunc(new InitDataTypeIO<T>());
    }

    public static T Load<T>(ref ByteBuffer buffer) => DataTypeIO.IOData<T>.LoadFunc(ref buffer);

    public static void Save<T>(ref ByteBuffer buffer, T item)
    {
      DataTypeIO.IOData<T>.SaveFunc(ref buffer, item);
    }

    public static int SizeOf<T>(T item) => DataTypeIO.IOData<T>.SizeFunc(item);

    internal static class IOData<T>
    {
      public static bool Initialized;
      public static DataTypeIO.LoadFunc<T> LoadFunc;
      public static DataTypeIO.SaveFunc<T> SaveFunc;
      public static DataTypeIO.SizeFunc<T> SizeFunc;
    }

    internal delegate T LoadFunc<T>(ref ByteBuffer buffer);

    internal delegate void SaveFunc<T>(ref ByteBuffer buffer, T item);

    internal delegate int SizeFunc<T>(T item);
  }
}
