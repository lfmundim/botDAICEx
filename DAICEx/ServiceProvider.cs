using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Host;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.ContainerProvider;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.NavigationExtensions;

namespace DAICEx
{
    public class ServiceProvider : Container, IServiceContainer
    {
        public ServiceProvider()
        {
            Options.AllowOverridingRegistrations = true;
            ContainerProvider.DefaultRegister(this);
            RegisterSingleton<INoAction, NoActionService>();
            RegisterSingleton<IEventNotificator, EventNotificator>();
        }
        public void RegisterService(Type serviceType, object instance)
        {
            RegisterSingleton(serviceType, instance);
            if(instance is MySettings)
            {
                RegisterSingleton((instance as MySettings).mpaSettings);
                var navSettings = new NavigationSettings((instance as MySettings).mpaSettings);
                RegisterSingleton(navSettings);
                //(instance as MySettings).mpaSettings;
            }
        }

        public void RegisterService(Type serviceType, Func<object> instanceFactory)
        {
            RegisterSingleton(serviceType, instanceFactory);
        }

    }
}
