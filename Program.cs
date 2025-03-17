using System;
using Microsoft.Extensions.DependencyInjection;

namespace ATMSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Setup Dependency Injection
                var services = new ServiceCollection();
                ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                // Get the ATM instance and start it
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

        private static void ConfigureServices(ServiceCollection services)
        {
            // Connection string - update with your MySQL credentials
            string connectionString = "Server=localhost;Database=atm_system;Uid=root;Pwd=root;";

            // Register MySQL database
            services.AddSingleton(new Database(connectionString));

            // Register ATM
            services.AddSingleton<ATM>();
        }
    }
}