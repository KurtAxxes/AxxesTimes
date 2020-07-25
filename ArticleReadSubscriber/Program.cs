using ArticleReadSubscriber.Handlers;
using AxxesTimes.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace ArticleReadSubscriber
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        static async Task Main()
        {
            Console.Title = "ArticleReadSubscriber";

            IConfiguration Configuration = new ConfigurationBuilder()
                                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                    .Build();

            serviceProvider = AddRegistrations(Configuration);

            await StartListeningAsync();
        }

        private static ServiceProvider AddRegistrations(IConfiguration Configuration)
        {
            //setup our DI
            return new ServiceCollection()
                .AddSingleton(Configuration)
                .AddScoped<IArticlesRepository, ArticlesRepository>()
                .BuildServiceProvider();
        }

        private static async Task StartListeningAsync()
        {
            // configure nservicebus
            var endpointConfiguration = new EndpointConfiguration("ArticleReadSubscriber");
            endpointConfiguration.RegisterComponents(
                registration: components =>
                {
                    components.ConfigureComponent(
                        componentFactory: builder =>
                        {
                            return new ArticleReadHandler(serviceProvider.GetService<IArticlesRepository>());
                        },
                        dependencyLifecycle: DependencyLifecycle.InstancePerUnitOfWork);
                });

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Delayed(
                // this is the number of delayed retries, when immediate retries don't fix it.
                // you can specify the number of retries (default = 3) + increased delay time between every retry (default = 10s).
                delayed =>
                {
                    delayed.NumberOfRetries(2);
                    delayed.TimeIncrease(TimeSpan.FromSeconds(2));
                });
            recoverability.Immediate(
                // this is the number of immediate retries when an error occurs (default = 5).
                // you can specify the number of retries.
                immediate =>
                {
                    immediate.NumberOfRetries(1);
                });

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            // start listening for incoming messages
            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                                                 .ConfigureAwait(false);

            Console.WriteLine("Press <any key> to exit.");
            Console.ReadKey();

            // stop listening
            await endpointInstance.Stop()
                                  .ConfigureAwait(false);
        }
    }
}
