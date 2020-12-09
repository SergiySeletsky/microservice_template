using System;
using System.Linq;

namespace MicroService.Core
{
    public interface IRepository<T> : IDisposable, IQueryable<T>  where T : class
    {
        T Save(T entity);
        void Delete(T entity);
        T Update(T entity);
    }
}
