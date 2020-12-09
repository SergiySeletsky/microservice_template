using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web.Http.Dependencies;

namespace MicroService.Core.WebServer
{
    internal class DependencyResolver : IDependencyResolver
    {
        private readonly CompositionContainer container;

        public DependencyResolver(CompositionContainer container)
        {
            this.container = container;
        }

        public IDependencyScope BeginScope()
        {
            var filteredCat = new FilteredCatalog(container.Catalog, def => def.Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName) && ((CreationPolicy)def.Metadata[CompositionConstants.PartCreationPolicyMetadataName]) == CreationPolicy.NonShared);
            var scopeContainer = new CompositionContainer(filteredCat, container);
            return new DependencyResolver(scopeContainer); //NonShared policy
        }

        public object GetService(Type serviceType)
        {
            return container.GetExportedValueOrDefault<object>(AttributedModelServices.GetContractName(serviceType));
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}
