using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core.Storage
{
    internal class StorageService : IStorageService
    {
        private readonly CompositionContainer container;
        public StorageService(CompositionContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            var a = typeof (Repository<>).FullName;
        }

        public IQueryable<T> Get<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public T Save<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public T Update<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
