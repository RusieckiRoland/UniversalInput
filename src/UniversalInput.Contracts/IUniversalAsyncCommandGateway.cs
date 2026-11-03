using System.Threading;
using System.Threading.Tasks;

namespace UniversalInput.Contracts
{
    public interface IUniversalAsyncCommandGateway<TResult>
    {
        Task<TResult> ProcessCommandAsync(UniversalCommandEventArgs args, CancellationToken cancellationToken);
    }
}
