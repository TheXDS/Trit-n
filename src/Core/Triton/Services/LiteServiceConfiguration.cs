using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    public class LiteServiceConfiguration : ILiteServiceConfiguration
    {
        ILiteCrudTransactionFactory IServiceConfigurationBase<ILiteCrudTransactionFactory>.CrudTransactionFactory { get; } = new LiteTransactionFactory();

        ITransactionConfiguration IServiceConfigurationBase.TransactionConfiguration => TransactionConfiguration;

        TransactionConfiguration TransactionConfiguration { get; } = new TransactionConfiguration();
    }
}