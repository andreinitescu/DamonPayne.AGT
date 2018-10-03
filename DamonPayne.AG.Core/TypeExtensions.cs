using System;
using System.Linq;

namespace DamonPayne.AG.Core
{
    public static class TypeExtensions
    {

        /// <summary>
        /// Check to see if one interface extends another
        /// </summary>
        /// <param name="t">the type to test, which should be an Interface</param>
        /// <param name="interfaceType">The Target Interface to see if <paramref name="t">t</paramref> extends</param>
        /// <returns></returns>
        public static bool InterfaceExtends(this Type t, Type interfaceType)
        {
            if (t.IsInterface && interfaceType.IsInterface)
            {
                Type[] interfaces = t.GetInterfaces();
                foreach (Type test in interfaces)
                {
                    Type targetTest = test.GetInterface(interfaceType.FullName, false);
                    if (test.Equals(interfaceType) || null != targetTest)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ImplementsInterface(this Type t, Type intefaceType)
        {
            if (intefaceType.IsInterface)
            {
                Type[] interfaces = t.GetInterfaces();
                if (interfaces.Contains<Type>(intefaceType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ImplementsInterface(this object o, Type intefaceType) 
        {
            return ImplementsInterface(o.GetType(), intefaceType);
        }      

    }
}
