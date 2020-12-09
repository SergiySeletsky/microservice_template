using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core
{
    public interface IStorageService : ICoreService
    {
        IQueryable<T> Get<T>() where T : class;
        T Save<T>(T obj) where T : class;
        void Delete<T>(T obj) where T : class;
        T Update<T>(T obj) where T : class;
    }
}
