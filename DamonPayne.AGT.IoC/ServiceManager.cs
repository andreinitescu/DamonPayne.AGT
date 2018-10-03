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
using System.Linq;
using System.Reflection;

namespace DamonPayne.AG.IoC
{
/*Creative Commons - attribution-noncommerical-share alike 3.0 Unported
          You are free to copy, distribute, and transmit this work.
          You are free to adapt the work.
        Under the following conditions:
          -You must attribute the author(Damon Payne, http://www.damonpayne.com) for this work but not in a way 
          that suggests the author endorses you or your use of the work.
          -You may not use this work for commercial purposes
          -If you alter, transform, or build upon this work, you are free to distribute the work under the same or similar license to this one.
 * */
       
    public class ServiceManager
    {
        static ServiceManager()
        {
            _services = new Dictionary<Type, object>();
        }

        private static Dictionary<Type, object> _services;

        /// <summary>
        /// Pass in a list of strong types and determine what service interfaces they provide to us:
        /// </summary>
        /// <param name="types"></param>
        public static void Manage(List<Type> types) //TODO: change to IEnumerable<Type>
        {
            Type baseServiceType = typeof(IService);

            foreach (Type t in types)
            {
                object instance = ObtainDefaultInstance(t);
                ManageInstance(instance);
            }
        }

        public static void ManageInstance(object provider)
        {
            Type baseServiceType = typeof(IService);
            Type t = provider.GetType();
            Type[] interfaces = t.GetInterfaces();
            foreach (Type iface in interfaces)
            {
                if (iface.InterfaceExtends(baseServiceType) && !_services.ContainsKey(iface))
                {
                    _services.Add(iface, provider);
                    CheckLateBinding(iface, provider);
                }
            }
        }

        private static void CheckLateBinding(Type interfaceType, object provider)
        {
            if (ComponentBuilder.LateBinding.ContainsKey(interfaceType))
            {
                List<object> clients = ComponentBuilder.LateBinding[interfaceType];
                foreach (object c in clients)
                {
                    var propQuery = from p in c.GetType().GetProperties().AsQueryable<PropertyInfo>()
                                    where p.PropertyType == interfaceType
                                    select p;
                    foreach (PropertyInfo propInfo in propQuery)
                    {
                        propInfo.SetValue(c, provider, null);
                    }
                }
                ComponentBuilder.LateBinding.Remove(interfaceType);
            }
        }


        /// <summary>
        /// Since an object could implement more than one interface, we may have in in here under multiple keys
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected static object ObtainDefaultInstance(Type t)
        {
            object o = null;
            var q = from val in _services.Values.AsQueryable<object>()
                    where val.GetType().IsAssignableFrom(t)
                    select val;

            o = q.FirstOrDefault<object>();
            if (null == o)
            {
                o = ComponentBuilder.Create(t);
                ServiceManager.ManageInstance(o);
                if (o is IService)
                {
                    //TODO: startup?
                    //((IService)o).Startup();
                }
            }

            return o;
        }

        /// <summary>
        /// Get the registered type 
        /// </summary>
        /// <typeparam name="TServiceType"></typeparam>
        /// <returns></returns>
        public static TServiceType Resolve<TServiceType>()
        {
            TServiceType svc = default(TServiceType);
            Type t = typeof(TServiceType);
            svc = (TServiceType)Resolve(t);            
            return svc;
        }

        /// <summary>
        /// Get the registered type 
        /// </summary>
        /// <typeparam name="TServiceType"></typeparam>
        /// <returns></returns>
        public static object Resolve(Type t)
        {
            object svc = null;
            if (_services.ContainsKey(t))
            {
                svc = _services[t];
            }
            else
            {
                throw new NoImplementationException();
            }
            return svc;
        }


    }
}
