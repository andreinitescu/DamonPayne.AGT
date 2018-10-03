using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Reflection.Emit;

namespace DamonPayne.AG.IoC
{
    public class InterfaceMocker
    {
        static InterfaceMocker()
        {
            _proxies = new Dictionary<string, object>();
        }

        /// <summary>
        /// Only define one dynamic assembly
        /// </summary>
        /// <returns></returns>
        private static ModuleBuilder GetDynamicModuleBuilder()
        {
            if (null == _dynamicModuleBuilder)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                AssemblyName proxyName = new AssemblyName("DamonPayne.IoC.TempProxy");
                AssemblyBuilder aBuilder = currentDomain.DefineDynamicAssembly(proxyName, AssemblyBuilderAccess.Run);
                //
                ModuleBuilder module = aBuilder.DefineDynamicModule(proxyName.Name);
                _dynamicProxyAssembly = aBuilder;
                _dynamicModuleBuilder = module;
            }

            return _dynamicModuleBuilder;
        }

        private static AssemblyBuilder _dynamicProxyAssembly;
        private static ModuleBuilder _dynamicModuleBuilder;
        private static Dictionary<string, object> _proxies;

        public static object GenerateProxy(Type t)
        {
            if(!t.IsInterface)
            {
                throw new ArgumentException(t + " Is not an interface");
            }
            //Useful:
            //http://msdn.microsoft.com/en-us/library/3y322t50(VS.95).aspx

            string proxyName = t.Name+"TempProxy";
            if (_proxies.ContainsKey(proxyName))
            {
                return _proxies[proxyName];
            }

            object o = null;
            ModuleBuilder module = GetDynamicModuleBuilder();
            //
            TypeBuilder typeBuilder = module.DefineType(proxyName, 
                TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.Class,typeof(object) );
            typeBuilder.AddInterfaceImplementation(t);

            ConstructorBuilder conBuilder = 
                typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName, CallingConventions.Standard, Type.EmptyTypes);
            //Define the reflection ConstructorInfor for System.Object
            ConstructorInfo conObj = typeof(object).GetConstructor(Type.EmptyTypes);

            //call constructor of base object
            ILGenerator il = conBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, conObj);
            il.Emit(OpCodes.Ret);

            List<MethodInfo> interfaceMethods = new List<MethodInfo>();
            interfaceMethods.AddRange(t.GetMethods());

            Type[] extraInterfaces = t.GetInterfaces();
            foreach (Type extraIface in extraInterfaces)
            {
                interfaceMethods.AddRange(extraIface.GetMethods());
            }

            foreach (MethodInfo method in interfaceMethods)
            {
                ParameterInfo[] mParams = method.GetParameters();
                Type[] paramTypes = new Type[mParams.Length];
                for(int i = 0; i < mParams.Length; ++i)
                {
                    paramTypes[i] = mParams[i].ParameterType;
                }

                MethodBuilder methodBuilder = 
                    typeBuilder.DefineMethod(method.Name, 
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.SpecialName, method.ReturnType, paramTypes);
                
                ILGenerator ilGen = methodBuilder.GetILGenerator();
                if (method.Name.Equals("Startup") || method.Name.Equals("Shutdown")) //TODO: dehackify this
                {
                    ilGen.Emit(OpCodes.Ret);
                }
                else
                {
                    ilGen.ThrowException(typeof(NoImplementationException));
                }                
            }

            Type finalType = typeBuilder.CreateType();

            o = Activator.CreateInstance(finalType);
            _proxies.Add(proxyName, o);
            return o;
        }
    }

    public class NoImplementationException : System.Exception
    {
        public NoImplementationException()
            : base("No implementation was registered for this service type")
        {

        }

        public NoImplementationException(string message) : base(message)
        {

        }

    }
}
