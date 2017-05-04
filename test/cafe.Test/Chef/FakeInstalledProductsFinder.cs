using System.Collections.Generic;
using cafe.LocalSystem;

namespace cafe.Test.Chef
{
    public class FakeInstalledProductsFinder : IInstalledProductsFinder
    {
        public FakeInstalledProductsFinder(params ProductInstallationMetaData[] metaData)
        {
            foreach (var element in metaData)
            {
                InstalledProducts.Add(element);
            }
        }

        public IEnumerable<ProductInstallationMetaData> GetInstalledProducts()
        {
            return InstalledProducts;
        }

        public IList<ProductInstallationMetaData> InstalledProducts { get; } = new List<ProductInstallationMetaData>();
    }
}