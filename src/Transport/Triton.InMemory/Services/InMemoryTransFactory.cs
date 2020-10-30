using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.InMemory.Services
{
    public class InMemoryTransFactory : ITransactionFactory
    {
        public ICrudReadWriteTransaction GetTransaction(TransactionConfiguration configuration)
        {
            return new InMemoryCrudTransaction(configuration);
        }
    }
}
