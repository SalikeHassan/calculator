namespace BusinessDaysCalculator;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddTransient<BusinessDayCalculator>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var calculator = serviceProvider.GetService<BusinessDayCalculator>();

        var startDate = new DateTime(2023, 11, 1);
        var endDate = new DateTime(2023, 11, 30);

        var businessDays = calculator.GetBusinessDays(startDate, endDate);

        Console.WriteLine($"Business days between {startDate.ToShortDateString()} and {endDate.ToShortDateString()}: {businessDays}");
    }
}