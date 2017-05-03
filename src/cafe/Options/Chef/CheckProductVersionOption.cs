using System;
using System.Collections.Generic;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options.Chef
{
    public class CheckProductVersionOption : ServerConnectionOption<IProductServer<ProductStatus>>
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CheckProductVersionOption).FullName);

        private readonly string _productName;

        public CheckProductVersionOption(string productName, Func<IProductServer<ProductStatus>> serverFactory)
            : base(serverFactory, $"Checks whether {productName} is on the supplied version")
        {
            _productName = productName;
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Checks if {_productName} is on version {FindVersion(args)}";
        }

        private static string FindVersion(IEnumerable<Argument> args)
        {
            return args.FindValueFromLabel("version:").Value;
        }

        protected override Result RunCore(IProductServer<ProductStatus> client, Argument[] args)
        {
            var taskStatus = client.GetStatus();
            var expectedVersion = FindVersion(args);
            var productStatus = taskStatus.Result;
            var actualVersion = productStatus.Version;
            Logger.Info($"{_productName} is on version {actualVersion}. Expected version is {expectedVersion}");
            return expectedVersion == actualVersion
                ? Result.Successful()
                : Result.Failure(
                    $"Expecting {_productName} to be on version {expectedVersion} but instead it is on version {actualVersion}");
        }
    }
}