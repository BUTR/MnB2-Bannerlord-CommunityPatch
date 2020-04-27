using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  public class MsCorEe {

    [DllImport("mscoree")]
    public static extern int CLRCreateInstance(
      in Guid clsId,
      in Guid rIId,
      [MarshalAs(UnmanagedType.IUnknown)] out object instance
    );

    public static object CLRCreateInstance(in Guid clsId, in Guid rIId) {
      var hr = MsCorEe.CLRCreateInstance(clsId, rIId, out var instance);
      if (hr != 0)
        Marshal.ThrowExceptionForHR(hr);
      return instance;
    }

    public static TInterface CLRCreateInstance<TClass, TInterface>() where TInterface : class {
      var clsId = typeof(TClass).GUID;
      var iId = typeof(TInterface).GUID;
      return CLRCreateInstance(clsId, iId) as TInterface;
    }

    public static TTarget CLRCreateInstance<TClass, TInterface, TTarget>() where TTarget : class where TInterface : class
      => CLRCreateInstance<TClass, TInterface>() as TTarget;

    public static TTarget CLRCreateInstance<TTarget>() where TTarget : class {
      var type = typeof(TTarget);
      var iId = type.GUID;
      var coClass = type.GetCustomAttribute<CoClassAttribute>().CoClass;
      var clsId = coClass.GUID;
      return CLRCreateInstance(clsId, iId) as TTarget;
    }

  }

}