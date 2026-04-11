using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Data;
using PhysioBook.Data.Entities;
using PhysioBook.Data.Enums;
using PhysioBook.Features.Bookings.Queries.GetAvailability;

namespace PhysioBook.Tests.Bookings;

public class GetAvailabilityHandlerTests
{
    private class TestDbContextFactory : IDbContextFactory<PhysioBookContext>
    {
        private readonly DbContextOptions<PhysioBookContext> _options;
        public TestDbContextFactory(DbContextOptions<PhysioBookContext> options) => _options = options;
        public PhysioBookContext CreateDbContext() => new PhysioBookContext(_options);
    }

    private static DbContextOptions<PhysioBookContext> CreateOptions(string dbName)
    {
        return new DbContextOptionsBuilder<PhysioBookContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private static Service CreateService(Guid id, string name, int durationMinutes)
    {
        return new Service
        {
            Id = id,
            Name = name,
            Description = "Test service",
            DurationMinutes = durationMinutes,
            Price = 500m,
            IsActive = true
        };
    }

    private static TimeSlot CreateTimeSlot(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        return new TimeSlot
        {
            Id = Guid.NewGuid(),
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            IsAvailable = true
        };
    }

    private static Booking CreateBooking(Guid serviceId, DateTime startTime, DateTime endTime, BookingStatus status)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            ServiceId = serviceId,
            ClientId = "test-user-id",
            StartTime = startTime,
            EndTime = endTime,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Find a future date that falls on the given day of week.
    /// </summary>
    private static DateOnly GetNextDateForDayOfWeek(DayOfWeek dayOfWeek)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysUntil = ((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7; // Always pick next week to ensure future date
        return today.AddDays(daysUntil);
    }

    [Fact]
    public async Task Empty_Calendar_Returns_All_Slots()
    {
        // 60-min service, TimeSlot 08:00-12:00, no bookings
        // Expected slots: 08:00, 08:30, 09:00, 09:30, 10:00, 10:30, 11:00 = 7
        var dbName = nameof(Empty_Calendar_Returns_All_Slots);
        var options = CreateOptions(dbName);
        var serviceId = Guid.NewGuid();
        var queryDate = GetNextDateForDayOfWeek(DayOfWeek.Monday);

        using (var context = new PhysioBookContext(options))
        {
            context.Services.Add(CreateService(serviceId, "Massage", 60));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            await context.SaveChangesAsync();
        }

        var factory = new TestDbContextFactory(options);
        var handler = new GetAvailabilityQueryHandler(factory);
        var query = new GetAvailabilityQuery { ServiceId = serviceId, Date = queryDate };

        var result = await handler.Handle(query, CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(7);
        result.AvailableSlots[0].StartTime.Should().Be("08:00");
        result.AvailableSlots[1].StartTime.Should().Be("08:30");
        result.AvailableSlots[2].StartTime.Should().Be("09:00");
        result.AvailableSlots[3].StartTime.Should().Be("09:30");
        result.AvailableSlots[4].StartTime.Should().Be("10:00");
        result.AvailableSlots[5].StartTime.Should().Be("10:30");
        result.AvailableSlots[6].StartTime.Should().Be("11:00");
    }

    [Fact]
    public async Task Booked_Slot_Is_Excluded()
    {
        // 60-min service, TimeSlot 08:00-12:00, Booking 09:00-10:00 Confirmed
        // Excluded: 08:30 (08:30-09:30 overlaps), 09:00 (09:00-10:00 overlaps), 09:30 (09:30-10:30 overlaps)
        // Remaining: 08:00, 10:00, 10:30, 11:00 = 4
        var dbName = nameof(Booked_Slot_Is_Excluded);
        var options = CreateOptions(dbName);
        var serviceId = Guid.NewGuid();
        var queryDate = GetNextDateForDayOfWeek(DayOfWeek.Monday);
        var dateAsDateTime = queryDate.ToDateTime(TimeOnly.MinValue);

        using (var context = new PhysioBookContext(options))
        {
            context.Services.Add(CreateService(serviceId, "Massage", 60));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            context.Bookings.Add(CreateBooking(
                serviceId,
                dateAsDateTime.AddHours(9),
                dateAsDateTime.AddHours(10),
                BookingStatus.Confirmed));
            await context.SaveChangesAsync();
        }

        var factory = new TestDbContextFactory(options);
        var handler = new GetAvailabilityQueryHandler(factory);
        var query = new GetAvailabilityQuery { ServiceId = serviceId, Date = queryDate };

        var result = await handler.Handle(query, CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(4);
        var startTimes = result.AvailableSlots.Select(s => s.StartTime).ToList();
        startTimes.Should().NotContain("08:30");
        startTimes.Should().NotContain("09:00");
        startTimes.Should().NotContain("09:30");
        startTimes.Should().Contain("08:00");
        startTimes.Should().Contain("10:00");
        startTimes.Should().Contain("10:30");
        startTimes.Should().Contain("11:00");
    }

    [Fact]
    public async Task Cancelled_Booking_Is_Ignored()
    {
        // 60-min service, TimeSlot 08:00-12:00, Booking 09:00-10:00 Cancelled
        // Cancelled bookings should not block slots, so all 7 slots available
        var dbName = nameof(Cancelled_Booking_Is_Ignored);
        var options = CreateOptions(dbName);
        var serviceId = Guid.NewGuid();
        var queryDate = GetNextDateForDayOfWeek(DayOfWeek.Monday);
        var dateAsDateTime = queryDate.ToDateTime(TimeOnly.MinValue);

        using (var context = new PhysioBookContext(options))
        {
            context.Services.Add(CreateService(serviceId, "Massage", 60));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            context.Bookings.Add(CreateBooking(
                serviceId,
                dateAsDateTime.AddHours(9),
                dateAsDateTime.AddHours(10),
                BookingStatus.Cancelled));
            await context.SaveChangesAsync();
        }

        var factory = new TestDbContextFactory(options);
        var handler = new GetAvailabilityQueryHandler(factory);
        var query = new GetAvailabilityQuery { ServiceId = serviceId, Date = queryDate };

        var result = await handler.Handle(query, CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(7);
        var startTimes = result.AvailableSlots.Select(s => s.StartTime).ToList();
        startTimes.Should().Contain("08:30");
        startTimes.Should().Contain("09:00");
        startTimes.Should().Contain("09:30");
    }

    [Fact]
    public async Task Boundary_End_Of_TimeSlot()
    {
        // 45-min service, TimeSlot 08:00-12:00
        // Slots: 08:00+45=08:45, 08:30+45=09:15, ... 11:00+45=11:45<=12:00, 11:30+45=12:15>12:00
        // Valid: 08:00, 08:30, 09:00, 09:30, 10:00, 10:30, 11:00 = 7
        var dbName = nameof(Boundary_End_Of_TimeSlot);
        var options = CreateOptions(dbName);
        var serviceId = Guid.NewGuid();
        var queryDate = GetNextDateForDayOfWeek(DayOfWeek.Monday);

        using (var context = new PhysioBookContext(options))
        {
            context.Services.Add(CreateService(serviceId, "Short Massage", 45));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            await context.SaveChangesAsync();
        }

        var factory = new TestDbContextFactory(options);
        var handler = new GetAvailabilityQueryHandler(factory);
        var query = new GetAvailabilityQuery { ServiceId = serviceId, Date = queryDate };

        var result = await handler.Handle(query, CancellationToken.None);

        var startTimes = result.AvailableSlots.Select(s => s.StartTime).ToList();
        startTimes.Should().Contain("11:00");
        startTimes.Should().NotContain("11:30");

        // Verify 11:00 end time is 11:45
        var slot1100 = result.AvailableSlots.First(s => s.StartTime == "11:00");
        slot1100.EndTime.Should().Be("11:45");
    }

    [Fact]
    public async Task Weekend_Returns_No_Slots()
    {
        // TimeSlots only for Monday, query for Saturday
        var dbName = nameof(Weekend_Returns_No_Slots);
        var options = CreateOptions(dbName);
        var serviceId = Guid.NewGuid();
        var queryDate = GetNextDateForDayOfWeek(DayOfWeek.Saturday);

        using (var context = new PhysioBookContext(options))
        {
            context.Services.Add(CreateService(serviceId, "Massage", 60));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Tuesday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Wednesday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Thursday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Friday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            await context.SaveChangesAsync();
        }

        var factory = new TestDbContextFactory(options);
        var handler = new GetAvailabilityQueryHandler(factory);
        var query = new GetAvailabilityQuery { ServiceId = serviceId, Date = queryDate };

        var result = await handler.Handle(query, CancellationToken.None);

        result.AvailableSlots.Should().BeEmpty();
    }

    [Fact]
    public async Task Multiple_TimeSlots_Per_Day()
    {
        // 60-min service, TimeSlots 08:00-12:00 + 13:00-17:00
        // Morning: 08:00, 08:30, 09:00, 09:30, 10:00, 10:30, 11:00 = 7
        // Afternoon: 13:00, 13:30, 14:00, 14:30, 15:00, 15:30, 16:00 = 7
        // Total = 14, gap 12:00-13:00 has no slots
        var dbName = nameof(Multiple_TimeSlots_Per_Day);
        var options = CreateOptions(dbName);
        var serviceId = Guid.NewGuid();
        var queryDate = GetNextDateForDayOfWeek(DayOfWeek.Monday);

        using (var context = new PhysioBookContext(options))
        {
            context.Services.Add(CreateService(serviceId, "Massage", 60));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0)));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(13, 0), new TimeOnly(17, 0)));
            await context.SaveChangesAsync();
        }

        var factory = new TestDbContextFactory(options);
        var handler = new GetAvailabilityQueryHandler(factory);
        var query = new GetAvailabilityQuery { ServiceId = serviceId, Date = queryDate };

        var result = await handler.Handle(query, CancellationToken.None);

        result.AvailableSlots.Should().HaveCount(14);
        var startTimes = result.AvailableSlots.Select(s => s.StartTime).ToList();
        startTimes.Should().NotContain("12:00");
        startTimes.Should().NotContain("12:30");
        startTimes.Should().Contain("08:00");
        startTimes.Should().Contain("13:00");
        startTimes.Should().Contain("16:00");
    }

    [Fact]
    public async Task NonExistent_Service_Throws()
    {
        var dbName = nameof(NonExistent_Service_Throws);
        var options = CreateOptions(dbName);
        var queryDate = GetNextDateForDayOfWeek(DayOfWeek.Monday);

        var factory = new TestDbContextFactory(options);
        var handler = new GetAvailabilityQueryHandler(factory);
        var query = new GetAvailabilityQuery { ServiceId = Guid.NewGuid(), Date = queryDate };

        var act = () => handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Short_Service_Generates_More_Slots()
    {
        // 20-min service with TimeSlot 08:00-09:00
        // 08:00+20=08:20<=09:00, 08:30+20=08:50<=09:00, 09:00+20=09:20>09:00
        // Slots: 08:00, 08:30 = 2 slots
        var dbName = nameof(Short_Service_Generates_More_Slots) + "_short";
        var options = CreateOptions(dbName);
        var serviceIdShort = Guid.NewGuid();
        var queryDate = GetNextDateForDayOfWeek(DayOfWeek.Monday);

        using (var context = new PhysioBookContext(options))
        {
            context.Services.Add(CreateService(serviceIdShort, "Quick Check", 20));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(9, 0)));
            await context.SaveChangesAsync();
        }

        // Compare with 60-min service in a separate DB
        var dbName2 = nameof(Short_Service_Generates_More_Slots) + "_long";
        var options2 = CreateOptions(dbName2);
        var serviceIdLong = Guid.NewGuid();

        using (var context = new PhysioBookContext(options2))
        {
            context.Services.Add(CreateService(serviceIdLong, "Full Massage", 60));
            context.TimeSlots.Add(CreateTimeSlot(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(9, 0)));
            await context.SaveChangesAsync();
        }

        var factoryShort = new TestDbContextFactory(options);
        var handlerShort = new GetAvailabilityQueryHandler(factoryShort);
        var queryShort = new GetAvailabilityQuery { ServiceId = serviceIdShort, Date = queryDate };
        var resultShort = await handlerShort.Handle(queryShort, CancellationToken.None);

        var factoryLong = new TestDbContextFactory(options2);
        var handlerLong = new GetAvailabilityQueryHandler(factoryLong);
        var queryLong = new GetAvailabilityQuery { ServiceId = serviceIdLong, Date = queryDate };
        var resultLong = await handlerLong.Handle(queryLong, CancellationToken.None);

        resultShort.AvailableSlots.Count.Should().BeGreaterThan(resultLong.AvailableSlots.Count);
        resultShort.AvailableSlots.Should().HaveCount(2);
        resultLong.AvailableSlots.Should().HaveCount(1);
    }
}
