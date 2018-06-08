using Autofac;
using Common.Log;
using Lykke.Service.BitcoinGold.Sign.Core.Settings.ServiceSettings;
using Lykke.SettingsReader;

namespace Lykke.Service.BitcoinGold.Sign.Modules
{
    public class SignInModule : Module
    {
        private readonly IReloadingManager<BitcoinGoldSignSettings> _settings;
        private readonly ILog _log;

        public SignInModule(IReloadingManager<BitcoinGoldSignSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            // TODO: Add your dependencies here
        }
    }
}
