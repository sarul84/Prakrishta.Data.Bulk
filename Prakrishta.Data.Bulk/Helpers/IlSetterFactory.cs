using System.Reflection;
using System.Reflection.Emit;

namespace Prakrishta.Data.Bulk.Helpers
{
    public static class IlSetterFactory
    {
        public static Action<object, object?> CreateSetter(PropertyInfo prop)
        {
            var dm = new DynamicMethod(
                $"set_{prop.Name}_{Guid.NewGuid():N}",
                null,
                new[] { typeof(object), typeof(object) },
                prop.DeclaringType!.Module,
                skipVisibility: true);

            var il = dm.GetILGenerator();

            // Load instance
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, prop.DeclaringType!);

            // Load value
            il.Emit(OpCodes.Ldarg_1);

            if (prop.PropertyType.IsValueType)
                il.Emit(OpCodes.Unbox_Any, prop.PropertyType);
            else
                il.Emit(OpCodes.Castclass, prop.PropertyType);

            var setter = prop.GetSetMethod(nonPublic: true)!;

            if (setter.IsVirtual)
                il.Emit(OpCodes.Callvirt, setter);
            else
                il.Emit(OpCodes.Call, setter);

            il.Emit(OpCodes.Ret);

            return (Action<object, object?>)dm.CreateDelegate(typeof(Action<object, object?>));
        }

    }
}
