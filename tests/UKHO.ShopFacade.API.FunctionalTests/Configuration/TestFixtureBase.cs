using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace UKHO.ShopFacade.API.FunctionalTests.Configuration
{
    public class TestFixtureBase
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly ShopFacadeConfiguration _shoFacadeConfiguration;
        private Process _process;

        protected ServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }

        public TestFixtureBase()
        {
            _serviceProvider = TestServiceConfiguration.ConfigureServices();
            _shoFacadeConfiguration = _serviceProvider!.GetRequiredService<IOptions<ShopFacadeConfiguration>>().Value;
        }


        [OneTimeSetUp]
        public void Setup()
        {

            var processStartInfo = new ProcessStartInfo
            {
                FileName = _shoFacadeConfiguration.AddsMockExePath,
                //Arguments = "",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _process = new Process { StartInfo = processStartInfo };
            _process.Start();

            Console.WriteLine("Process started successfully.");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _serviceProvider?.Dispose();
            _process.Dispose();
        }
    }
}
