using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.API
{
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var app = builder.Build();

            app.Run();
        }
    }
}
