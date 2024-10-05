using System.Linq.Expressions;

namespace Project.Shared.Interfaces.Data;

public interface IProjection<TIn, TOut>
{
    public static abstract Expression<Func<TIn,TOut>> Projection { get; }
}
