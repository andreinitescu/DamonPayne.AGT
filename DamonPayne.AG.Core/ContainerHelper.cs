using System;
using Microsoft.Practices.Unity;

namespace DamonPayne.AG.Core
{
    public class ContainerHelper
    {
        static ContainerHelper()
        {
            _c = new UnityContainer();
        }

        static IUnityContainer _c;

        public static T Resolve<T>()
        {
            return _c.Resolve<T>();
        }

        public static IUnityContainer I
        {
            get { return _c; }
        }
    }
}
