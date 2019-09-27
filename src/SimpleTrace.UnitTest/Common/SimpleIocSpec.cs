using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable CheckNamespace

namespace SimpleTrace.Common
{
    [TestClass]
    public class SimpleIocSpec
    {
        [TestMethod]
        public void CanResolve_SameType_ShouldOk()
        {
            var simpleIoc = new SimpleIoc();
            simpleIoc.Register(() => new IocBase());
            simpleIoc.CanResolve<IocBase>().ShouldTrue();
        }

        [TestMethod]
        public void CanResolve_SameTypeList_ShouldOk()
        {
            var simpleIoc = new SimpleIoc();
            simpleIoc.Register(() => new List<IocFoo>());
            simpleIoc.CanResolve<List<IocFoo>>().ShouldTrue();
            simpleIoc.CanResolve<IList<IocFoo>>().ShouldTrue();
            simpleIoc.CanResolve<IEnumerable<IocFoo>>().ShouldTrue();
        }

        [TestMethod]
        public void CanResolve_SubType_ShouldOk()
        {
            var simpleIoc = new SimpleIoc();
            simpleIoc.Register(() => new IocFoo());
            simpleIoc.CanResolve<IocFoo>().ShouldTrue();
            simpleIoc.CanResolve<IocBase>().ShouldTrue();
            simpleIoc.CanResolve<IIocBase>().ShouldTrue();
        }

        [TestMethod]
        public void Resolve_SameTypeList_ShouldOk()
        {
            var simpleIoc = new SimpleIoc();
            simpleIoc.Register(() => new List<IocFoo>());
            simpleIoc.Resolve<List<IocFoo>>().ShouldNotNull();
            simpleIoc.Resolve<IList<IocFoo>>().ShouldNotNull();
            simpleIoc.Resolve<IEnumerable<IocFoo>>().ShouldNotNull();
        }
    }

    public interface IIocBase { }
    public class IocBase: IIocBase { }
    public class IocFoo : IocBase{}

}
