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
            string command = "dotnet dev-certs https --trust";

            // Call the method to execute the command
            RunConsoleCommand(command);

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
            _process.Kill();
            _process.WaitForExit();
            _process.Dispose();
        }

        public static async Task<string> RunConsoleCommand(string command)
        {
            try
            {
                // Create a new ProcessStartInfo to execute the command
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",         // Run the command using cmd.exe
                    Arguments = $"/C {command}",  // /C means to run the command and then terminate
                    RedirectStandardOutput = true, // Redirect the output so we can read it
                    RedirectStandardError = true,  // Redirect error output
                    UseShellExecute = false,      // Don't use the shell to execute the command
                    CreateNoWindow = true         // Don't show a new window for the command
                };

                // Start the process and capture the output
                using (Process process = Process.Start(startInfo))
                {
                    Thread.Sleep(20000);
                    if (process != null)
                    {
                        // Read the output and errors if any
                        string output = await process.StandardOutput.ReadToEndAsync();
                        string error = await process.StandardError.ReadToEndAsync();
                        // Wait for the command to complete
                        process.WaitForExit();

                        // Print the output and errors
                        if (!string.IsNullOrEmpty(output))
                        {
                            Console.WriteLine("Output: " + output);
                        }

                        if (!string.IsNullOrEmpty(error))
                        {
                            Console.WriteLine("Error: " + error);
                        }
                        return output;
                    }
                    Console.WriteLine("Process is null");
                    return "";
                }
            }
            catch (Exception ex)
            {
                // Catch any exceptions and print them
                Console.WriteLine("An error occurred: " + ex.Message);
                return ex.ToString();
            }
        }
    }
}
