﻿using AutofacModule = Autofac.Module;

namespace Shapeshifter.WindowsDesktop.Infrastructure.Dependencies
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Threading;

    using Autofac;
    using Autofac.Builder;

    using Controls.Clipboard.Designer.Helpers;
    using Controls.Clipboard.Designer.Services.Interfaces;

    using Environment;
    using Environment.Interfaces;

    using Interfaces;

    using Native;

    using Threading;

    public class DefaultWiringModule : AutofacModule
    {
        readonly IEnvironmentInformation environmentInformation;

        readonly Action<ContainerBuilder> callback;

        public DefaultWiringModule(
            IEnvironmentInformation environmentInformation)
        {
            this.environmentInformation = environmentInformation;
        }

        public DefaultWiringModule(Action<ContainerBuilder> callback = null)
            : this(new EnvironmentInformation())
        {
            this.callback = callback;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterAssemblyTypes(builder, typeof(DefaultWiringModule).Assembly);
            RegisterAssemblyTypes(builder, NativeAssemblyHelper.Assembly);

            RegisterMainThread(builder);

            var environmentInformation = RegisterEnvironmentInformation(builder);
            if (environmentInformation.IsInDesignTime)
            {
                DesignTimeContainerHelper.RegisterFakes(builder);
            }

            callback?.Invoke(builder);

            base.Load(builder);
        }

        static void RegisterMainThread(ContainerBuilder builder)
        {
            builder.RegisterInstance(new UserInterfaceThread(Dispatcher.CurrentDispatcher))
                   .AsImplementedInterfaces();
        }

        IEnvironmentInformation RegisterEnvironmentInformation(ContainerBuilder builder)
        {
            var environmentInformation = this.environmentInformation ?? new EnvironmentInformation();
            builder
                .RegisterInstance(environmentInformation)
                .As<IEnvironmentInformation>()
                .SingleInstance();
            return environmentInformation;
        }

        void RegisterAssemblyTypes(ContainerBuilder builder, Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                {
                    continue;
                }

                var interfaces = type.GetInterfaces();
                if (interfaces.Contains(typeof(IDesignerService)) && !environmentInformation.IsInDesignTime)
                {
                    continue;
                }

                IRegistrationBuilder<object, ReflectionActivatorData, object> registration;
                if (type.IsGenericType)
                {
                    registration = builder.RegisterGeneric(type);

                    var genericInterfaces = interfaces
                        .Where(x => x.IsGenericType);
                    foreach (var genericInterface in genericInterfaces)
                    {
                        registration.As(genericInterface);
                    }
                }
                else
                {
                    var standardRegistration = builder.RegisterType(type);
                    standardRegistration.AsSelf();
                    standardRegistration.AsImplementedInterfaces();

                    registration = standardRegistration;
                }

                registration.FindConstructorsWith(new PublicConstructorFinder());

                if (interfaces.Contains(typeof(ISingleInstance)))
                {
                    registration.SingleInstance();
                }
            }
        }
    }
}