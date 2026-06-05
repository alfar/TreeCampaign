namespace Common.Infrastructure.Abstractions;

public static class TransitionHelper
{
    public static void Transition<TOld, TNew, TKey>(this IUnitOfWork unitOfWork, TOld oldAggregate, TNew newAggregate)
        where TOld : class
        where TNew : class
    {
        unitOfWork.GetRepository<TOld, TKey>().Delete(oldAggregate);
        unitOfWork.GetRepository<TNew, TKey>().Add(newAggregate);
    }
}