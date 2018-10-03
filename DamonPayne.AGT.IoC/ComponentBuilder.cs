/*Creative Commons - attribution-noncommerical-share alike 3.0 Unported
          You are free to copy, distribute, and transmit this work.
          You are free to adapt the work.
        Under the following conditions:
          -You must attribute the author(Damon Payne, http://www.damonpayne.com) for this work but not in a way 
          that suggests the author endorses you or your use of the work.
          -You may not use this work for commercial purposes
          -If you alter, transform, or build upon this work, you are free to distribute the work under the same or similar license to this one.
        */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DamonPayne.AG.IoC
{
    public class ComponentBuilder
    {

        static ComponentBuilder()
        {
            LateBinding = new Dictionary<Type, List<object>>();
        }

        /// <summary>
        /// Build an object that is a- or uses a- Serviced Component.
        /// Presenters are created per-instance, IServices are recursively resolved from the ServiceManager.
        /// </summary>
        /// <typeparam name="T">The Type to create and Build</typeparam>
        /// <returns></returns>
        public static T Create<T>() where T :new ()
        {
            T instance = default(T);
            instance = (T)Create(typeof(T));           
            return instance;
        }

        public static object Create(Type t)
        {
            object instance = Activator.CreateInstance(t);
            ResolveServiceDependencies(instance);
            BuildPresenters(instance);
            ServiceManager.ManageInstance(instance);//May result in duplicate check, but I'm OK with that
            return instance;
        }

        internal static Dictionary<Type, List<object>> LateBinding;

        internal static void ResolveServiceDependencies(object component) 
        {
            PropertyInfo[] publicProps = component.GetType().GetProperties();
            foreach (PropertyInfo pi in publicProps)
            {
                Type propType = pi.PropertyType;
                if (propType.IsInterface && propType.InterfaceExtends(typeof(IService)))
                {
                    object svc = null;
                    try
                    {
                        svc = ServiceManager.Resolve(propType);
                    }
                    catch (NoImplementationException)//TODO: fix this so an exception is not thrown?
                    {
                        svc = SetupLateBinding(propType, component);
                    }
                    pi.SetValue(component, svc, null);
                    ResolveServiceDependencies(svc);//TODO: Smartest way to do this?
                }
            }
        }

        private static object SetupLateBinding(Type interfaceType, object client)
        {
            object svc = InterfaceMocker.GenerateProxy(interfaceType);

            if (!LateBinding.ContainsKey(interfaceType))
            {
                LateBinding.Add(interfaceType, new List<object>());
            }

            LateBinding[interfaceType].Add(client);

            return svc;
        }

        internal static void BuildPresenters(object component)
        {
            PropertyInfo[] publicProps = component.GetType().GetProperties();
            foreach (PropertyInfo pi in publicProps)
            {
                object[] attrs = pi.GetCustomAttributes(typeof(PresenterAttribute), true);
                if (null != attrs && attrs.Length > 0)
                {
                    //We rely on the convention that the Presenter has a public constructor with a view as argument, 
                    //where said view is implemented by "component"
                    object presenter = Activator.CreateInstance(pi.PropertyType, component);
                    pi.SetValue(component, presenter, null);
                    ResolveServiceDependencies(presenter);
                }
            }
        }

    }
}
