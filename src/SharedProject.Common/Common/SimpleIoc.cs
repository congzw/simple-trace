using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace

namespace SimpleTrace.Common
{
    public class SimpleIoc
    {
        public SimpleIoc()
        {
            Services = new ConcurrentDictionary<Type, Func<object>>();
        }

        public bool CanResolve<T>()
        {
            var theFindType = TryFindType<T>();
            return theFindType != null;
        }

        public T Resolve<T>()
        {
            //thanks to => https://stackoverflow.com/questions/2742276/how-do-i-check-if-a-type-is-a-subtype-or-the-type-of-an-object
            //public class Base { }
            //public class Derived : Base { }

            //Use Type.IsSubclassOf 
            //typeof(Derived).IsSubclassOf(typeof(Base)).Dump(); => true
            //typeof(Base).IsSubclassOf(typeof(Base)).Dump(); => false

            //Use Type.IsAssignableFrom
            //typeof(Base).IsAssignableFrom(typeof(Derived)).Dump(); => true
            //typeof(Base).IsAssignableFrom(typeof(Base)).Dump(); => true
            //typeof(int[]).IsAssignableFrom(typeof(uint[])).Dump(); => true

            var theFindType = TryFindType<T>();
            if (theFindType == null)
            {
                return default(T);
            }

            return (T)Services[theFindType]();
        }

        //public IEnumerable<T> ResolveAll<T>()
        //public void RegisterAll<T>(Func<T> factory)

        private Type TryFindType<T>()
        {
            var theType = typeof(T);
            var theFindType = Services.Keys.FirstOrDefault(t => IsSameOrSubclass(theType, t));
            return theFindType;
        }

        private bool IsSameOrSubclass(Type baseType, Type descendantType)
        {
            //return descendantType.IsSubclassOf(baseType) || descendantType == baseType;
            return baseType.IsAssignableFrom(descendantType);
        }

        public void Register<T>(Func<T> factory)
        {
            var theType = typeof(T);
            if (Services.ContainsKey(theType))
            {
                throw new InvalidOperationException("not allowed register more then once");
            }
            Services[theType] = () => factory();
        }

        protected IDictionary<Type, Func<object>> Services { get; set; }

        public static SimpleIoc Instance = new SimpleIoc();
    }
}
