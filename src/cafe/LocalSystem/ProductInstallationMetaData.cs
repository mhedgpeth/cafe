namespace cafe.LocalSystem
{
    public class ProductInstallationMetaData
    {
        public string DisplayName { get; set; }
        public string Publisher { get; set; }
        public string DisplayVersion { get; set; }
        public string UninstallString { get; set; }
        public string Parent { get; set; }

        public override string ToString()
        {
            return $"{DisplayName} version {DisplayVersion} by {Publisher}";
        }
    }
}