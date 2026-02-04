using System.Reflection;
using System.Reflection.Emit;

namespace Prakrishta.Data.Bulk.Core;

public static class IlGetterFactory
{
    public static Func<object, object?> CreateGetter(PropertyInfo prop)
    {
        var dm = new DynamicMethod(
            $"get_{prop.Name}_{Guid.NewGuid():N}",
            typeof(object),
            new[] { typeof(object) },
            prop.DeclaringType!.Module,
            skipVisibility: true);

        var il = dm.GetILGenerator();

        // arg0: object instance → cast to declaring type
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Castclass, prop.DeclaringType!);

        var getter = prop.GetGetMethod(nonPublic: true)!;

        if (getter.IsVirtual)
            il.Emit(OpCodes.Callvirt, getter);
        else
            il.Emit(OpCodes.Call, getter);

        if (prop.PropertyType.IsValueType)
            il.Emit(OpCodes.Box, prop.PropertyType);

        il.Emit(OpCodes.Ret);

        return (Func<object, object?>)dm.CreateDelegate(typeof(Func<object, object?>));
    }
}