using Autofac;
using Lykke.BitcoinGold.Sign.Services;
using Lykke.Service.BitcoinGold.Sign.Core.Services;
using Lykke.Service.BitcoinGold.Sign.Core.Settings.ServiceSettings;
using Lykke.SettingsReader;

namespace Lykke.Service.BitcoinGold.Sign.Modules
{
    public class SignInModule : Module
    {
        private readonly IReloadingManager<BitcoinGoldSignSettings> _settings;

        public SignInModule(IReloadingManager<BitcoinGoldSignSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            // TODO: Add your dependencies here

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
        }
    }
}
