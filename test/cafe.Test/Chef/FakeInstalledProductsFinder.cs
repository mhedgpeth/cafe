using System.Collections.Generic;
using cafe.LocalSystem;
using NuGet.Packaging;

namespace cafe.Test.Chef
{
    public class FakeInstalledProductsFinder : IInstalledProductsFinder
    {
        public FakeInstalledProductsFinder(params ProductInstallationMetaData[] metaData)
        {
            InstalledProducts.AddRange(metaData);
        }

        public IEnumerable<ProductInstallationMetaData> GetInstalledProducts()
        {
            return InstalledProducts;
        }

        public IList<ProductInstallationMetaData> InstalledProducts { get; } = new List<ProductInstallationMetaData>();
    }
}