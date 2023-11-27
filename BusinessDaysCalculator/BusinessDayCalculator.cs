using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Microsoft.Extensions.Configuration;
namespace BusinessDaysCalculator;

public class BusinessDayCalculator
{
    private readonly List<DateTime> staticHolidays;
    private readonly List<CalendarEvent> dynamicHolidayEvents;

    public BusinessDayCalculator(IConfiguration configuration)
    {
        staticHolidays = configuration.GetSection("Holidays:StaticHolidays")
                                      .Get<List<string>>()
                                      .Select(date => DateTime.ParseExact(date, "MM/dd", null))
                                      .ToList();

        dynamicHolidayEvents = configuration.GetSection("Holidays:DynamicHolidays")
                                            .Get<List<string>>()
                                            .Select(rule => new CalendarEvent
                                            {
                                                Start = new CalDateTime(DateTime.MinValue.Year, 1, 1),
                                                RecurrenceRules = new List<RecurrencePattern> { new RecurrencePattern(rule) }
                                            })
                                            .ToList();
    }

    public int GetBusinessDays(DateTime start, DateTime end)
    {
        int businessDays = 0;
        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            if (IsWeekend(date) || IsHoliday(date)) continue;
            businessDays++;
        }
        return businessDays;
    }

    private bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    private bool IsHoliday(DateTime date)
    {
        if (staticHolidays.Any(holiday => holiday.Month == date.Month && holiday.Day == date.Day))
            return true;

        var periodStart = new CalDateTime(date);
        var periodEnd = new CalDateTime(date.AddDays(1));
        foreach (var holidayEvent in dynamicHolidayEvents)
        {
            if (holidayEvent.GetOccurrences(periodStart, periodEnd).Any())
                return true;
        }

        return false;
    }
}

