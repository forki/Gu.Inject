﻿namespace Gu.Inject.Tests
{
    using System;

    using Gu.Inject.Tests.Types;

    using NUnit.Framework;

    public partial class KernelTests
    {
        public class Bind
        {
            [TestCase(typeof(IWith), typeof(With<DefaultCtor>))]
            [TestCase(typeof(OneToMany.Abstract), typeof(OneToMany.Concrete1))]
            [TestCase(typeof(OneToMany.Abstract), typeof(OneToMany.Concrete2))]
            [TestCase(typeof(OneToMany.IAbstract), typeof(OneToMany.Concrete1))]
            [TestCase(typeof(OneToMany.IAbstract), typeof(OneToMany.Concrete2))]
            [TestCase(typeof(OneToMany.IConcrete), typeof(OneToMany.Concrete1))]
            [TestCase(typeof(OneToMany.IConcrete), typeof(OneToMany.Concrete2))]
            [TestCase(typeof(OneToMany.IConcrete), typeof(OneToMany.Concrete2))]
            [TestCase(typeof(With<OneToMany.IConcrete>), typeof(With<OneToMany.Concrete2>))]
            [TestCase(typeof(IWith<OneToMany.IConcrete>), typeof(With<OneToMany.Concrete2>))]
            public void BindThenGet(Type from, Type to)
            {
                using (var kernel = new Kernel())
                {
                    kernel.Bind(from, to);
                    var actual = kernel.Get(from);
                    Assert.AreSame(actual, kernel.Get(to));
                }
            }

            [Test]
            public void BindInstanceThenGet()
            {
                using (var kernel = new Kernel())
                {
                    var instance = new DefaultCtor();
                    kernel.Bind(instance);
                    var actual = kernel.Get<DefaultCtor>();
                    Assert.AreSame(actual, instance);
                }
            }

            [Test]
            public void BindInstanceAndInterfaceThenGet()
            {
                var instance = new With<DefaultCtor>(new DefaultCtor());

                using (var kernel = new Kernel())
                {
                    kernel.Bind(instance);
                    kernel.Bind<IWith<DefaultCtor>, With<DefaultCtor>>();

                    object actual = kernel.Get<With<DefaultCtor>>();
                    Assert.AreSame(actual, instance);

                    actual = kernel.Get<IWith<DefaultCtor>>();
                    Assert.AreSame(actual, instance);
                }
            }

            [Test]
            public void BounInstanceIsNotDisposed()
            {
                using (var disposable = new Disposable())
                {
                    using (var kernel = new Kernel())
                    {
                        kernel.Bind(disposable);
                        var actual = kernel.Get<Disposable>();
                        Assert.AreSame(actual, disposable);
                    }

                    Assert.AreEqual(0, disposable.Disposed);
                }
            }

            [Test]
            public void ThrowsIfHasResolved()
            {
                using (var kernel = new Kernel())
                {
                    kernel.Get<DefaultCtor>();
                    var exception = Assert.Throws<InvalidOperationException>(() => kernel.Bind<IWith, With<DefaultCtor>>());
                    Assert.AreEqual("Bind not allowed after Get.", exception.Message);
                }
            }

            [Test]
            public void ThrowsIfHasBinding()
            {
                using (var kernel = new Kernel())
                {
                    kernel.Bind<IWith, With<DefaultCtor>>();
                    var exception = Assert.Throws<InvalidOperationException>(() => kernel.Bind<IWith, With<DefaultCtor>>());
                    Assert.AreEqual("IWith already has a binding to With<DefaultCtor>", exception.Message);
                }
            }
        }
    }
}