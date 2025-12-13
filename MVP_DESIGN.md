# Bookity - MVP Domain Model & Architecture

> **Version:** MVP 1.0  
> **Target Framework:** .NET 10  
> **Architecture:** Clean Architecture + DDD  
> **Model:** Booking Marketplace (like Grab, Airbnb)

---

## 1. Platform Vision

### 1.1 What is Bookity?

A **marketplace for any bookable service** where anyone can:
- Register as a **Provider** and publish their services/resources
- Let customers discover and book time slots

### 1.2 Supported Business Types

| Type | Example | Booking Mode | Staff Needed? |
|------|---------|--------------|---------------|
| Sports facility | Badminton court, Football field | **Direct** | ❌ |
| Rental space | Meeting room, Photo studio | **Direct** | ❌ |
| Solo service | Freelance consultant, Tutor | **Direct** | ❌ |
| Service business | Spa, Salon, Clinic | **Staff-based** | ✅ |

### 1.3 Two Booking Modes

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         BOOKING MODES                                    │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  MODE 1: DIRECT (No Staff)                                              │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  Customer ──────► Service ──────► Booking                       │   │
│  │                                                                  │   │
│  │  Example: Book "Badminton Court 1" for Saturday 8:00-9:00       │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                          │
│  MODE 2: STAFF-BASED (Staff Required)                                   │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  Customer ──────► Service ──────► Staff ──────► Booking         │   │
│  │                                                                  │   │
│  │  Example: Book "Deep Tissue Massage" with "Emma" for Friday 10:00│   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 2. MVP Scope

### 2.1 Included Features ✅

| Feature | Description |
|---------|-------------|
| Provider Registration | Anyone can register as a service provider |
| Service Management | Create services (direct or staff-based) |
| Staff Management | **Optional** - only for staff-based services |
| Customer Profile | Basic customer information |
| Booking Flow | Book directly or with staff selection |
| Availability Check | Check available time slots |

### 2.2 Deferred to Full Version ⏳

- Cancellation policies & fees
- Buffer time between bookings
- Staff availability blocks (vacation)
- Service categories
- Booking rescheduling
- Status history tracking

---

## 3. Domain Model

### 3.1 Aggregate Overview

```
┌────────────────────────────────────────────────────────────────────┐
│                      MVP AGGREGATES (5)                            │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐          │
│  │ Provider │  │ Service  │  │  Staff   │  │ Customer │          │
│  │  (Root)  │  │  (Root)  │  │  (Root)  │  │  (Root)  │          │
│  │          │  │          │  │ OPTIONAL │  │          │          │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘          │
│                                                                    │
│                      ┌──────────┐                                  │
│                      │ Booking  │  ⭐ Core                         │
│                      │  (Root)  │                                  │
│                      └──────────┘                                  │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### 3.2 Provider Aggregate

```
Provider (Aggregate Root)
├── ProviderId : ProviderId
├── Name : string
├── Slug : string (URL-friendly, unique)
├── Description : string?
├── Email : string
├── Phone : string?
├── TimeZone : string (IANA format, e.g., "America/New_York")
├── IsActive : bool
├── CreatedAt : DateTime
├── UpdatedAt : DateTime
│
├── Methods:
│   ├── static Create(name, slug, email, timeZone)
│   ├── Update(name, description, email, phone)
│   ├── Activate()
│   └── Deactivate()
```

**Examples:**
- "Ace Badminton Club" (Sports facility owner)
- "Serenity Spa & Wellness" (Spa owner)
- "John Smith Consulting" (Solo consultant)

### 3.3 Service Aggregate ⭐

```
Service (Aggregate Root)
├── ServiceId : ServiceId
├── ProviderId : ProviderId
├── Name : string
├── Description : string?
├── DurationMinutes : int
├── Price : decimal
├── Currency : string (ISO 4217)
├── BookingMode : BookingMode (Direct | StaffBased)  ⭐
├── Schedule : WeeklySchedule? (for Direct mode only)
├── MaxConcurrentBookings : int (default: 1)
├── IsActive : bool
├── CreatedAt : DateTime
├── UpdatedAt : DateTime
│
├── Methods:
│   ├── static Create(providerId, name, duration, price, currency, bookingMode)
│   ├── Update(name, description, duration, price)
│   ├── SetSchedule(weeklySchedule)
│   ├── SetMaxConcurrentBookings(max)
│   ├── RequiresStaff() : bool
│   ├── Activate()
│   └── Deactivate()
```

**Schedule Location by Mode:**
- `BookingMode.Direct` → Schedule on **Service**
- `BookingMode.StaffBased` → Schedule on **Staff**

### 3.4 Staff Aggregate (OPTIONAL)

```
Staff (Aggregate Root) - Only for StaffBased services
├── StaffId : StaffId
├── ProviderId : ProviderId
├── Name : string
├── Email : string?
├── Phone : string?
├── Schedule : WeeklySchedule
├── ServiceIds : List<ServiceId>
├── IsActive : bool
├── CreatedAt : DateTime
├── UpdatedAt : DateTime
│
├── Methods:
│   ├── static Create(providerId, name)
│   ├── Update(name, email, phone)
│   ├── SetSchedule(weeklySchedule)
│   ├── AssignService(serviceId)
│   ├── UnassignService(serviceId)
│   ├── IsAvailableAt(dayOfWeek, time) : bool
│   ├── Activate()
│   └── Deactivate()
```

### 3.5 Customer Aggregate

```
Customer (Aggregate Root)
├── CustomerId : CustomerId
├── Name : string
├── Email : string
├── Phone : string?
├── CreatedAt : DateTime
├── UpdatedAt : DateTime
│
├── Methods:
│   ├── static Create(name, email, phone?)
│   └── Update(name, email, phone)
```

### 3.6 Booking Aggregate ⭐

```
Booking (Aggregate Root)
├── BookingId : BookingId
├── BookingNumber : string
├── ProviderId : ProviderId
├── ServiceId : ServiceId
├── StaffId : StaffId? ────────────────────── NULLABLE (Direct mode = null)
├── CustomerId : CustomerId
├── Date : DateOnly
├── StartTime : TimeOnly
├── EndTime : TimeOnly
├── ServiceName : string (snapshot)
├── ServicePrice : decimal (snapshot)
├── ServiceCurrency : string (snapshot)
├── StaffName : string? (snapshot)  ───────── NULLABLE
├── Status : BookingStatus
├── CustomerNotes : string?
├── CancellationReason : string?
├── CreatedAt : DateTime
├── UpdatedAt : DateTime
│
├── Methods:
│   ├── static CreateDirect(...)
│   ├── static CreateWithStaff(...)
│   ├── Confirm()
│   ├── Cancel(reason?)
│   ├── Complete()
│   ├── MarkAsNoShow()
│   └── HasStaff() : bool
```

**Status Flow:**
```
PENDING ──► CONFIRMED ──► COMPLETED
    │           │
    │           ▼
    └──────► CANCELLED
                │
          CONFIRMED ──► NO_SHOW
```

---

## 4. Real-World Examples

### 4.1 Badminton Court (Direct Mode)

```
┌─────────────────────────────────────────────────────────────────────────┐
│            PROVIDER: "Ace Badminton Club"                               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   SERVICES (BookingMode: Direct):                                       │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "Court 1" - 60 min - $25 USD                                │      │
│   │ Schedule: Mon-Sun 6:00-22:00 | MaxConcurrent: 1             │      │
│   └─────────────────────────────────────────────────────────────┘      │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "Court 2" - 60 min - $25 USD                                │      │
│   │ Schedule: Mon-Sun 6:00-22:00 | MaxConcurrent: 1             │      │
│   └─────────────────────────────────────────────────────────────┘      │
│                                                                          │
│   STAFF: None                                                           │
│   FLOW: Customer ──► Select "Court 1" ──► Pick time ──► Book           │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### 4.2 Spa (Staff-Based Mode)

```
┌─────────────────────────────────────────────────────────────────────────┐
│            PROVIDER: "Serenity Spa & Wellness"                          │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   SERVICES (BookingMode: StaffBased):                                   │
│   ┌──────────────────────┐  ┌──────────────────────┐                   │
│   │ "Swedish Massage"    │  │ "Deep Tissue Massage"│                   │
│   │  60 min, $80 USD     │  │  60 min, $95 USD     │                   │
│   └──────────────────────┘  └──────────────────────┘                   │
│                                                                          │
│   STAFF:                                                                │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "Emma" - Services: Swedish, Deep Tissue                     │      │
│   │          Schedule: Mon-Fri 9:00-18:00                       │      │
│   └─────────────────────────────────────────────────────────────┘      │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "Sarah" - Services: Swedish                                 │      │
│   │           Schedule: Mon-Sat 10:00-19:00                     │      │
│   └─────────────────────────────────────────────────────────────┘      │
│                                                                          │
│   FLOW: Customer ──► Select Service ──► Pick Staff ──► Pick time ──► Book│
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 5. Value Objects

### 5.1 Strongly-Typed IDs

```csharp
public readonly record struct ProviderId(Guid Value)
{
    public static ProviderId New() => new(Guid.NewGuid());
}

public readonly record struct ServiceId(Guid Value)
{
    public static ServiceId New() => new(Guid.NewGuid());
}

public readonly record struct StaffId(Guid Value)
{
    public static StaffId New() => new(Guid.NewGuid());
}

public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
}

public readonly record struct BookingId(Guid Value)
{
    public static BookingId New() => new(Guid.NewGuid());
}
```

### 5.2 Enums

```csharp
public enum BookingMode
{
    Direct = 1,      // Courts, rooms, solo consultants
    StaffBased = 2   // Spa, salon, clinic
}

public enum BookingStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4,
    NoShow = 5
}
```

### 5.3 Schedule Value Objects

```csharp
public sealed record WorkingHours
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    public WorkingHours(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
            throw new DomainException("End time must be after start time");
        StartTime = startTime;
        EndTime = endTime;
    }

    public bool Contains(TimeOnly time) => time >= StartTime && time < EndTime;
}

public sealed record WeeklySchedule
{
    private readonly Dictionary<DayOfWeek, WorkingHours?> _days;
    
    public IReadOnlyDictionary<DayOfWeek, WorkingHours?> Days => _days;

    public WeeklySchedule(Dictionary<DayOfWeek, WorkingHours?> days)
    {
        _days = days ?? throw new ArgumentNullException(nameof(days));
    }

    public bool IsWorkingDay(DayOfWeek day) => 
        _days.TryGetValue(day, out var hours) && hours != null;

    public WorkingHours? GetHours(DayOfWeek day) => _days.GetValueOrDefault(day);
}
```

---

## 6. Domain Events

```csharp
// Provider
public sealed record ProviderCreatedEvent(
    ProviderId ProviderId, string Name, string Slug, DateTime CreatedAt) : IDomainEvent;

// Service
public sealed record ServiceCreatedEvent(
    ServiceId ServiceId, ProviderId ProviderId, string Name, 
    BookingMode BookingMode, DateTime CreatedAt) : IDomainEvent;

// Staff
public sealed record StaffCreatedEvent(
    StaffId StaffId, ProviderId ProviderId, string Name) : IDomainEvent;

public sealed record StaffScheduleUpdatedEvent(
    StaffId StaffId, DateTime UpdatedAt) : IDomainEvent;

// Booking
public sealed record BookingCreatedEvent(
    BookingId BookingId, string BookingNumber, ProviderId ProviderId,
    ServiceId ServiceId, StaffId? StaffId, CustomerId CustomerId,
    DateOnly Date, TimeOnly StartTime) : IDomainEvent;

public sealed record BookingConfirmedEvent(
    BookingId BookingId, DateTime ConfirmedAt) : IDomainEvent;

public sealed record BookingCancelledEvent(
    BookingId BookingId, string? Reason, DateTime CancelledAt) : IDomainEvent;
```

---

## 7. Domain Services

```csharp
public interface IBookingAvailabilityService
{
    Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsForServiceAsync(
        ServiceId serviceId, DateOnly date, CancellationToken ct = default);

    Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsForStaffAsync(
        StaffId staffId, ServiceId serviceId, DateOnly date, CancellationToken ct = default);

    Task<bool> IsSlotAvailableAsync(
        ServiceId serviceId, StaffId? staffId, DateOnly date,
        TimeOnly startTime, TimeOnly endTime, CancellationToken ct = default);
}

public interface IBookingNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken ct = default);
}

public record TimeSlotDto(TimeOnly StartTime, TimeOnly EndTime);
```

---

## 8. Repository Interfaces

```csharp
public interface IRepository<TAggregate, TId> where TAggregate : IAggregateRoot
{
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task AddAsync(TAggregate aggregate, CancellationToken ct = default);
    Task UpdateAsync(TAggregate aggregate, CancellationToken ct = default);
    Task DeleteAsync(TAggregate aggregate, CancellationToken ct = default);
}

public interface IProviderRepository : IRepository<Provider, ProviderId>
{
    Task<Provider?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
}

public interface IServiceRepository : IRepository<Service, ServiceId>
{
    Task<IReadOnlyList<Service>> GetByProviderIdAsync(ProviderId providerId, CancellationToken ct = default);
}

public interface IStaffRepository : IRepository<Staff, StaffId>
{
    Task<IReadOnlyList<Staff>> GetByProviderIdAsync(ProviderId providerId, CancellationToken ct = default);
    Task<IReadOnlyList<Staff>> GetByServiceIdAsync(ServiceId serviceId, CancellationToken ct = default);
}

public interface ICustomerRepository : IRepository<Customer, CustomerId>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
}

public interface IBookingRepository : IRepository<Booking, BookingId>
{
    Task<Booking?> GetByBookingNumberAsync(string bookingNumber, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetByServiceAndDateAsync(ServiceId serviceId, DateOnly date, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetByStaffAndDateAsync(StaffId staffId, DateOnly date, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken ct = default);
}
```

---

## 9. Project Structure

```
📁 Bookity/
├── 📁 src/
│   ├── 📦 Bookity.Domain/
│   │   ├── 📁 Aggregates/
│   │   │   ├── 📁 Providers/
│   │   │   │   ├── 📄 Provider.cs
│   │   │   │   ├── 📄 ProviderId.cs
│   │   │   │   └── 📄 IProviderRepository.cs
│   │   │   ├── 📁 Services/
│   │   │   │   ├── 📄 Service.cs
│   │   │   │   ├── 📄 ServiceId.cs
│   │   │   │   ├── 📄 BookingMode.cs
│   │   │   │   └── 📄 IServiceRepository.cs
│   │   │   ├── 📁 Staff/
│   │   │   │   ├── 📄 Staff.cs
│   │   │   │   ├── 📄 StaffId.cs
│   │   │   │   └── 📄 IStaffRepository.cs
│   │   │   ├── 📁 Customers/
│   │   │   │   ├── 📄 Customer.cs
│   │   │   │   ├── 📄 CustomerId.cs
│   │   │   │   └── 📄 ICustomerRepository.cs
│   │   │   └── 📁 Bookings/
│   │   │       ├── 📄 Booking.cs
│   │   │       ├── 📄 BookingId.cs
│   │   │       ├── 📄 BookingStatus.cs
│   │   │       └── 📄 IBookingRepository.cs
│   │   ├── 📁 Common/
│   │   │   ├── 📄 Entity.cs
│   │   │   ├── 📄 AggregateRoot.cs
│   │   │   ├── 📄 IAggregateRoot.cs
│   │   │   ├── 📄 IDomainEvent.cs
│   │   │   ├── 📄 IRepository.cs
│   │   │   └── 📄 DomainException.cs
│   │   ├── 📁 ValueObjects/
│   │   │   ├── 📄 WorkingHours.cs
│   │   │   └── 📄 WeeklySchedule.cs
│   │   ├── 📁 Events/
│   │   └── 📁 Services/
│   │       ├── 📄 IBookingAvailabilityService.cs
│   │       └── 📄 IBookingNumberGenerator.cs
│   │
│   ├── 📦 Bookity.Application/ ⭐ CQRS + MediatR
│   │   ├── 📁 Common/
│   │   │   ├── 📄 IUnitOfWork.cs
│   │   │   ├── 📄 Result.cs
│   │   │   ├── 📄 Error.cs
│   │   │   ├── 📁 Behaviors/
│   │   │   │   ├── 📄 ValidationBehavior.cs
│   │   │   │   ├── 📄 LoggingBehavior.cs
│   │   │   │   └── 📄 UnitOfWorkBehavior.cs
│   │   │   └── 📁 Mapping/
│   │   │       └── 📄 MappingConfig.cs
│   │   │
│   │   └── 📁 Features/
│   │       ├── 📁 Providers/
│   │       │   ├── 📁 Commands/
│   │       │   │   ├── 📁 CreateProvider/
│   │       │   │   │   ├── 📄 CreateProviderCommand.cs
│   │       │   │   │   ├── 📄 CreateProviderCommandHandler.cs
│   │       │   │   │   └── 📄 CreateProviderCommandValidator.cs
│   │       │   │   ├── 📁 UpdateProvider/
│   │       │   │   │   ├── 📄 UpdateProviderCommand.cs
│   │       │   │   │   ├── 📄 UpdateProviderCommandHandler.cs
│   │       │   │   │   └── 📄 UpdateProviderCommandValidator.cs
│   │       │   │   └── 📁 ActivateProvider/
│   │       │   │       ├── 📄 ActivateProviderCommand.cs
│   │       │   │       └── 📄 ActivateProviderCommandHandler.cs
│   │       │   ├── 📁 Queries/
│   │       │   │   ├── 📁 GetProviderById/
│   │       │   │   │   ├── 📄 GetProviderByIdQuery.cs
│   │       │   │   │   └── 📄 GetProviderByIdQueryHandler.cs
│   │       │   │   └── 📁 GetProviderBySlug/
│   │       │   │       ├── 📄 GetProviderBySlugQuery.cs
│   │       │   │       └── 📄 GetProviderBySlugQueryHandler.cs
│   │       │   └── 📁 DTOs/
│   │       │       ├── 📄 ProviderDto.cs
│   │       │       └── 📄 ProviderDetailsDto.cs
│   │       │
│   │       ├── 📁 Services/
│   │       │   ├── 📁 Commands/
│   │       │   │   ├── 📁 CreateService/
│   │       │   │   │   ├── 📄 CreateServiceCommand.cs
│   │       │   │   │   ├── 📄 CreateServiceCommandHandler.cs
│   │       │   │   │   └── 📄 CreateServiceCommandValidator.cs
│   │       │   │   ├── 📁 UpdateService/
│   │       │   │   │   ├── 📄 UpdateServiceCommand.cs
│   │       │   │   │   ├── 📄 UpdateServiceCommandHandler.cs
│   │       │   │   │   └── 📄 UpdateServiceCommandValidator.cs
│   │       │   │   └── 📁 SetServiceSchedule/
│   │       │   │       ├── 📄 SetServiceScheduleCommand.cs
│   │       │   │       ├── 📄 SetServiceScheduleCommandHandler.cs
│   │       │   │       └── 📄 SetServiceScheduleCommandValidator.cs
│   │       │   ├── 📁 Queries/
│   │       │   │   ├── 📁 GetServiceById/
│   │       │   │   │   ├── 📄 GetServiceByIdQuery.cs
│   │       │   │   │   └── 📄 GetServiceByIdQueryHandler.cs
│   │       │   │   └── 📁 GetServicesByProvider/
│   │       │   │       ├── 📄 GetServicesByProviderQuery.cs
│   │       │   │       └── 📄 GetServicesByProviderQueryHandler.cs
│   │       │   └── 📁 DTOs/
│   │       │       ├── 📄 ServiceDto.cs
│   │       │       └── 📄 ServiceDetailsDto.cs
│   │       │
│   │       ├── 📁 Staff/
│   │       │   ├── 📁 Commands/
│   │       │   │   ├── 📁 CreateStaff/
│   │       │   │   │   ├── 📄 CreateStaffCommand.cs
│   │       │   │   │   ├── 📄 CreateStaffCommandHandler.cs
│   │       │   │   │   └── 📄 CreateStaffCommandValidator.cs
│   │       │   │   ├── 📁 AssignStaffToService/
│   │       │   │   │   ├── 📄 AssignStaffToServiceCommand.cs
│   │       │   │   │   ├── 📄 AssignStaffToServiceCommandHandler.cs
│   │       │   │   │   └── 📄 AssignStaffToServiceCommandValidator.cs
│   │       │   │   └── 📁 SetStaffSchedule/
│   │       │   │       ├── 📄 SetStaffScheduleCommand.cs
│   │       │   │       ├── 📄 SetStaffScheduleCommandHandler.cs
│   │       │   │       └── 📄 SetStaffScheduleCommandValidator.cs
│   │       │   ├── 📁 Queries/
│   │       │   │   ├── 📁 GetStaffById/
│   │       │   │   │   ├── 📄 GetStaffByIdQuery.cs
│   │       │   │   │   └── 📄 GetStaffByIdQueryHandler.cs
│   │       │   │   └── 📁 GetStaffByService/
│   │       │   │       ├── 📄 GetStaffByServiceQuery.cs
│   │       │   │       └── 📄 GetStaffByServiceQueryHandler.cs
│   │       │   └── 📁 DTOs/
│   │       │       ├── 📄 StaffDto.cs
│   │       │       └── 📄 StaffDetailsDto.cs
│   │       │
│   │       ├── 📁 Customers/
│   │       │   ├── 📁 Commands/
│   │       │   │   ├── 📁 CreateCustomer/
│   │       │   │   │   ├── 📄 CreateCustomerCommand.cs
│   │       │   │   │   ├── 📄 CreateCustomerCommandHandler.cs
│   │       │   │   │   └── 📄 CreateCustomerCommandValidator.cs
│   │       │   │   └── 📁 UpdateCustomer/
│   │       │   │       ├── 📄 UpdateCustomerCommand.cs
│   │       │   │       ├── 📄 UpdateCustomerCommandHandler.cs
│   │       │   │       └── 📄 UpdateCustomerCommandValidator.cs
│   │       │   ├── 📁 Queries/
│   │       │   │   ├── 📁 GetCustomerById/
│   │       │   │   │   ├── 📄 GetCustomerByIdQuery.cs
│   │       │   │   │   └── 📄 GetCustomerByIdQueryHandler.cs
│   │       │   │   └── 📁 GetCustomerByEmail/
│   │       │   │       ├── 📄 GetCustomerByEmailQuery.cs
│   │       │   │       └── 📄 GetCustomerByEmailQueryHandler.cs
│   │       │   └── 📁 DTOs/
│   │       │       └── 📄 CustomerDto.cs
│   │       │
│   │       └── 📁 Bookings/
│   │           ├── 📁 Commands/
│   │           │   ├── 📁 CreateDirectBooking/
│   │           │   │   ├── 📄 CreateDirectBookingCommand.cs
│   │           │   │   ├── 📄 CreateDirectBookingCommandHandler.cs
│   │           │   │   └── 📄 CreateDirectBookingCommandValidator.cs
│   │           │   ├── 📁 CreateStaffBooking/
│   │           │   │   ├── 📄 CreateStaffBookingCommand.cs
│   │           │   │   ├── 📄 CreateStaffBookingCommandHandler.cs
│   │           │   │   └── 📄 CreateStaffBookingCommandValidator.cs
│   │           │   ├── 📁 ConfirmBooking/
│   │           │   │   ├── 📄 ConfirmBookingCommand.cs
│   │           │   │   └── 📄 ConfirmBookingCommandHandler.cs
│   │           │   └── 📁 CancelBooking/
│   │           │       ├── 📄 CancelBookingCommand.cs
│   │           │       ├── 📄 CancelBookingCommandHandler.cs
│   │           │       └── 📄 CancelBookingCommandValidator.cs
│   │           ├── 📁 Queries/
│   │           │   ├── 📁 GetBookingById/
│   │           │   │   ├── 📄 GetBookingByIdQuery.cs
│   │           │   │   └── 📄 GetBookingByIdQueryHandler.cs
│   │           │   ├── 📁 GetAvailableSlots/
│   │           │   │   ├── 📄 GetAvailableSlotsQuery.cs
│   │           │   │   └── 📄 GetAvailableSlotsQueryHandler.cs
│   │           │   └── 📁 GetCustomerBookings/
│   │           │       ├── 📄 GetCustomerBookingsQuery.cs
│   │           │       └── 📄 GetCustomerBookingsQueryHandler.cs
│   │           └── 📁 DTOs/
│   │               ├── 📄 BookingDto.cs
│   │               ├── 📄 BookingDetailsDto.cs
│   │               └── 📄 TimeSlotDto.cs
│   │
│   ├── 📦 Bookity.Infrastructure/
│   │   ├── 📁 Persistence/
│   │   │   ├── 📄 ApplicationDbContext.cs
│   │   │   ├── 📄 UnitOfWork.cs
│   │   │   ├── 📁 Configurations/
│   │   │   │   ├── 📄 ProviderConfiguration.cs
│   │   │   │   ├── 📄 ServiceConfiguration.cs
│   │   │   │   ├── 📄 StaffConfiguration.cs
│   │   │   │   ├── 📄 CustomerConfiguration.cs
│   │   │   │   └── 📄 BookingConfiguration.cs
│   │   │   └── 📁 Repositories/
│   │   │       ├── 📄 ProviderRepository.cs
│   │   │       ├── 📄 ServiceRepository.cs
│   │   │       ├── 📄 StaffRepository.cs
│   │   │       ├── 📄 CustomerRepository.cs
│   │   │       └── 📄 BookingRepository.cs
│   │   ├── 📁 Services/
│   │   │   ├── 📄 BookingAvailabilityService.cs
│   │   │   └── 📄 BookingNumberGenerator.cs
│   │   └── 📄 DependencyInjection.cs
│   │
│   └── 📦 Bookity.Api/
│       ├── 📁 Controllers/
│       │   ├── 📄 ProvidersController.cs
│       │   ├── 📄 ServicesController.cs
│       │   ├── 📄 StaffController.cs
│       │   ├── 📄 CustomersController.cs
│       │   └── 📄 BookingsController.cs
│       ├── 📁 Middleware/
│       │   ├── 📄 ExceptionHandlingMiddleware.cs
│       │   └── 📄 RequestLoggingMiddleware.cs
│       ├── 📄 Program.cs
│       └── 📄 DependencyInjection.cs
│
└── 📁 tests/
    ├── 📦 Bookity.Domain.Tests/
    ├── 📦 Bookity.Application.Tests/
    └── 📦 Bookity.Api.Tests/
```

### 9.1 Application Layer - CQRS Architecture ⭐

The Application layer follows the **CQRS (Command Query Responsibility Segregation)** pattern using **MediatR** library.

#### 9.1.1 Core Principles

**Command Query Separation:**
- **Commands**: Modify state, return `Result<T>` or `Result`
- **Queries**: Read-only, return DTOs, never modify state
- **Handlers**: One handler per command/query (Single Responsibility)

**Vertical Slice Architecture:**
Each feature is a self-contained vertical slice with Commands, Queries, Handlers, Validators, and DTOs.

#### 9.1.2 MediatR Request/Response Pattern

```csharp
// Command example
public sealed record CreateProviderCommand(
    string Name,
    string Slug,
    string Email,
    string TimeZone) : IRequest<Result<Guid>>;

// Query example
public sealed record GetProviderByIdQuery(Guid ProviderId) : IRequest<Result<ProviderDetailsDto>>;

// Handler example
public sealed class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, Result<Guid>>
{
    private readonly IProviderRepository _providerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProviderCommandHandler(IProviderRepository providerRepository, IUnitOfWork unitOfWork)
    {
        _providerRepository = providerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
    {
        // 1. Check slug uniqueness
        if (await _providerRepository.SlugExistsAsync(request.Slug, cancellationToken))
            return Result.Failure<Guid>(Error.Conflict("Provider.SlugExists", "Slug already exists"));

        // 2. Create domain entity
        var provider = Provider.Create(request.Name, request.Slug, request.Email, request.TimeZone);

        // 3. Persist
        await _providerRepository.AddAsync(provider, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Return result
        return Result.Success(provider.Id.Value);
    }
}
```

#### 9.1.3 Pipeline Behaviors

MediatR pipeline behaviors wrap all requests with cross-cutting concerns:

**ValidationBehavior** - FluentValidation integration:
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

**LoggingBehavior** - Request/response logging:
```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
}
```

**UnitOfWorkBehavior** - Automatic transaction management for commands:
```csharp
public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // Only wrap commands, not queries
        if (!typeof(TRequest).Name.EndsWith("Command"))
            return await next();

        var response = await next();
        await _unitOfWork.SaveChangesAsync(ct);
        return response;
    }
}
```

#### 9.1.4 Result Pattern

```csharp
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static new Result<T> Failure(Error error) => new(default, false, error);
}

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static Error NotFound(string code, string message) => new(code, message);
    public static Error Validation(string code, string message) => new(code, message);
    public static Error Conflict(string code, string message) => new(code, message);
}
```

#### 9.1.5 FluentValidation Integration

Each command has a corresponding validator using FluentValidation:

```csharp
public class CreateProviderCommandValidator : AbstractValidator<CreateProviderCommand>
{
    public CreateProviderCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Provider name is required")
            .MaximumLength(200).WithMessage("Provider name must not exceed 200 characters");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .Matches("^[a-z0-9-]+$").WithMessage("Slug must contain only lowercase letters, numbers, and hyphens");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.TimeZone)
            .NotEmpty().WithMessage("TimeZone is required")
            .Must(BeValidTimeZone).WithMessage("Invalid IANA timezone");
    }

    private bool BeValidTimeZone(string timeZone)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

#### 9.1.6 Dependency Injection Setup

```csharp
// In Bookity.Application/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline Behaviors (order matters!)
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

        return services;
    }
}
```

#### 9.1.7 Required NuGet Packages

```xml
<ItemGroup>
  <!-- MediatR -->
  <PackageReference Include="MediatR" Version="12.2.0" />

  <!-- FluentValidation -->
  <PackageReference Include="FluentValidation" Version="11.9.0" />
  <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
</ItemGroup>
```

---

## 10. API Endpoints (CQRS Integration)

All endpoints delegate to MediatR handlers (Commands/Queries) following CQRS pattern.

### 10.1 API Controller Pattern

Controllers are thin wrappers that send requests to MediatR:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly ISender _sender;

    public ProvidersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProvider([FromBody] CreateProviderRequest request, CancellationToken ct)
    {
        var command = new CreateProviderCommand(request.Name, request.Slug, request.Email, request.TimeZone);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProviderById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProviderById(Guid id, CancellationToken ct)
    {
        var query = new GetProviderByIdQuery(id);
        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error);
    }
}
```

### 10.2 Endpoint Mapping

| Method | Endpoint | Handler | Description |
|--------|----------|---------|-------------|
| **Providers** |
| POST | `/api/providers` | `CreateProviderCommandHandler` | Register provider |
| PUT | `/api/providers/{id}` | `UpdateProviderCommandHandler` | Update provider |
| GET | `/api/providers/{id}` | `GetProviderByIdQueryHandler` | Get provider |
| GET | `/api/providers/by-slug/{slug}` | `GetProviderBySlugQueryHandler` | Get by slug |
| POST | `/api/providers/{id}/activate` | `ActivateProviderCommandHandler` | Activate |
| POST | `/api/providers/{id}/deactivate` | `DeactivateProviderCommandHandler` | Deactivate |
| **Services** |
| POST | `/api/providers/{providerId}/services` | `CreateServiceCommandHandler` | Create service |
| PUT | `/api/services/{id}` | `UpdateServiceCommandHandler` | Update service |
| GET | `/api/services/{id}` | `GetServiceByIdQueryHandler` | Get service |
| GET | `/api/providers/{providerId}/services` | `GetServicesByProviderQueryHandler` | List services |
| PUT | `/api/services/{id}/schedule` | `SetServiceScheduleCommandHandler` | Set schedule (Direct) |
| POST | `/api/services/{id}/activate` | `ActivateServiceCommandHandler` | Activate |
| POST | `/api/services/{id}/deactivate` | `DeactivateServiceCommandHandler` | Deactivate |
| **Staff** |
| POST | `/api/providers/{providerId}/staff` | `CreateStaffCommandHandler` | Create staff |
| PUT | `/api/staff/{id}` | `UpdateStaffCommandHandler` | Update staff |
| GET | `/api/staff/{id}` | `GetStaffByIdQueryHandler` | Get staff |
| GET | `/api/providers/{providerId}/staff` | `GetStaffByProviderQueryHandler` | List staff |
| GET | `/api/services/{serviceId}/staff` | `GetStaffByServiceQueryHandler` | Get staff for service |
| PUT | `/api/staff/{id}/schedule` | `SetStaffScheduleCommandHandler` | Set schedule |
| POST | `/api/staff/{id}/assign-service` | `AssignStaffToServiceCommandHandler` | Assign to service |
| POST | `/api/staff/{id}/unassign-service` | `UnassignStaffFromServiceCommandHandler` | Unassign |
| POST | `/api/staff/{id}/activate` | `ActivateStaffCommandHandler` | Activate |
| POST | `/api/staff/{id}/deactivate` | `DeactivateStaffCommandHandler` | Deactivate |
| **Customers** |
| POST | `/api/customers` | `CreateCustomerCommandHandler` | Create customer |
| PUT | `/api/customers/{id}` | `UpdateCustomerCommandHandler` | Update customer |
| GET | `/api/customers/{id}` | `GetCustomerByIdQueryHandler` | Get customer |
| GET | `/api/customers/by-email/{email}` | `GetCustomerByEmailQueryHandler` | Get by email |
| **Bookings** |
| POST | `/api/bookings/direct` | `CreateDirectBookingCommandHandler` | Create direct booking |
| POST | `/api/bookings/staff` | `CreateStaffBookingCommandHandler` | Create staff booking |
| GET | `/api/bookings/{id}` | `GetBookingByIdQueryHandler` | Get booking |
| GET | `/api/bookings/by-number/{number}` | `GetBookingByNumberQueryHandler` | Get by booking number |
| GET | `/api/customers/{customerId}/bookings` | `GetCustomerBookingsQueryHandler` | Customer history |
| POST | `/api/bookings/{id}/confirm` | `ConfirmBookingCommandHandler` | Confirm |
| POST | `/api/bookings/{id}/cancel` | `CancelBookingCommandHandler` | Cancel |
| POST | `/api/bookings/{id}/complete` | `CompleteBookingCommandHandler` | Complete |
| POST | `/api/bookings/{id}/mark-no-show` | `MarkBookingAsNoShowCommandHandler` | Mark no-show |
| **Availability** |
| GET | `/api/services/{serviceId}/availability?date={date}` | `GetAvailableSlotsQueryHandler` | Direct mode slots |
| GET | `/api/staff/{staffId}/availability?serviceId={serviceId}&date={date}` | `GetAvailableSlotsQueryHandler` | Staff mode slots |

### 10.3 Request/Response DTOs

Controllers use request/response DTOs separate from domain entities:

```csharp
// Request DTO
public sealed record CreateProviderRequest(
    string Name,
    string Slug,
    string Email,
    string TimeZone,
    string? Description = null,
    string? Phone = null);

// Response DTO
public sealed record ProviderDetailsDto(
    Guid Id,
    string Name,
    string Slug,
    string Email,
    string TimeZone,
    string? Description,
    string? Phone,
    bool IsActive,
    DateTime CreatedAt);

// Booking DTOs
public sealed record CreateDirectBookingRequest(
    Guid ServiceId,
    Guid CustomerId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? CustomerNotes = null);

public sealed record CreateStaffBookingRequest(
    Guid ServiceId,
    Guid StaffId,
    Guid CustomerId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? CustomerNotes = null);

public sealed record BookingDetailsDto(
    Guid Id,
    string BookingNumber,
    Guid ProviderId,
    Guid ServiceId,
    Guid? StaffId,
    Guid CustomerId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string ServiceName,
    decimal ServicePrice,
    string ServiceCurrency,
    string? StaffName,
    string Status,
    string? CustomerNotes,
    string? CancellationReason,
    DateTime CreatedAt);
```

### 10.4 Error Handling

Errors are handled consistently using the Result pattern:

```csharp
// In controller
var result = await _sender.Send(command, ct);

if (!result.IsSuccess)
{
    return result.Error.Code switch
    {
        var code when code.Contains("NotFound") => NotFound(result.Error),
        var code when code.Contains("Conflict") => Conflict(result.Error),
        var code when code.Contains("Validation") => BadRequest(result.Error),
        _ => StatusCode(500, result.Error)
    };
}

return Ok(result.Value);
```

Or using a global exception middleware:

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { errors = ex.Errors });
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
        }
    }
}
```

---

## 11. Summary

| Entity | Purpose |
|--------|---------|
| **Provider** | Business/person offering services on the platform |
| **Service** | Bookable offering (court, room, massage, consultation) |
| **Staff** | Optional - person who delivers staff-based services |
| **Customer** | Person making bookings |
| **Booking** | Reservation connecting customer to service |

| Mode | Staff? | Schedule On | Examples |
|------|--------|-------------|----------|
| Direct | ❌ | Service | Court, Room, Solo consultant |
| StaffBased | ✅ | Staff | Spa, Salon, Clinic |

---

*MVP Domain Model for Bookity - Booking Marketplace*