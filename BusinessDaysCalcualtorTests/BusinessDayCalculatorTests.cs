using BusinessDaysCalculator;
using Microsoft.Extensions.Configuration;

namespace BusinessDaysCalcualtorTests;
public class BusinessDayCalculatorTests
{
    private BusinessDayCalculator CreateCalculatorWithHolidays()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Holidays:StaticHolidays:0", "01/01"},  // New Year's Day
            {"Holidays:StaticHolidays:1", "12/25"},  // Christmas
            {"Holidays:DynamicHolidays:0", "RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=4TH TH"}  // Thanksgiving
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        return new BusinessDayCalculator(configuration);
    }

    [Fact]
    public void GetBusinessDays_RegularScenario_ReturnsCorrectCount()
    {
        var calculator = CreateCalculatorWithHolidays();
        var startDate = new DateTime(2023, 6, 1);
        var endDate = new DateTime(2023, 6, 7);

        var businessDays = calculator.GetBusinessDays(startDate, endDate);

        Assert.Equal(5, businessDays);
    }

    [Fact]
    public void GetBusinessDays_IncludingStaticHoliday_ReturnsCorrectCount()
    {
        var calculator = CreateCalculatorWithHolidays();
        var startDate = new DateTime(2023, 12, 24);
        var endDate = new DateTime(2023, 12, 26);

        var businessDays = calculator.GetBusinessDays(startDate, endDate);

        Assert.Equal(1, businessDays);
    }

    [Fact]
    public void GetBusinessDays_StartDateAfterEndDate_ReturnsZero()
    {
        var calculator = CreateCalculatorWithHolidays();
        var startDate = new DateTime(2023, 12, 31);
        var endDate = new DateTime(2023, 12, 30);

        var businessDays = calculator.GetBusinessDays(startDate, endDate);

        Assert.Equal(0, businessDays);
    }

    [Fact]
    public void GetBusinessDays_EntireRangeOnWeekend_ReturnsZero()
    {
        var calculator = CreateCalculatorWithHolidays();
        var startDate = new DateTime(2023, 7, 8);
        var endDate = new DateTime(2023, 7, 9);

        var businessDays = calculator.GetBusinessDays(startDate, endDate);

        Assert.Equal(0, businessDays);
    }
}
