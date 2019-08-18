using System.Threading;
using System.Threading.Tasks;

namespace SDPApp.Application.Abstraction
{
    public interface IQueryHandler<TQuery,TView>
    {
        Task<TView> Handle(TQuery query, CancellationToken token);
    }
}