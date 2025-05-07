using System;
using ATMSystem.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ATMSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                IServiceProvider serviceProvider = services.BuildServiceProvider();

                var atm = serviceProvider.GetRequiredService<ATM>();
                atm.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            string connectionString = "Server=localhost;Database=atm_system;Uid=root;Pwd=root;";
           services.AddSingleton<IDatabase>(provider => new Database(connectionString));

            services.AddSingleton<ATM>();
        }
    }
}