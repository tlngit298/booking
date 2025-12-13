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
│   │   │   └── 📄 DomainException.cs
│   │   ├── 📁 ValueObjects/
│   │   │   ├── 📄 WorkingHours.cs
│   │   │   └── 📄 WeeklySchedule.cs
│   │   ├── 📁 Events/
│   │   └── 📁 Services/
│   │       ├── 📄 IBookingAvailabilityService.cs
│   │       └── 📄 IBookingNumberGenerator.cs
│   │
│   ├── 📦 Bookity.Application/
│   │   ├── 📁 Common/
│   │   │   ├── 📄 IUnitOfWork.cs
│   │   │   ├── 📄 Result.cs
│   │   │   └── 📁 Behaviors/
│   │   └── 📁 Features/
│   │       ├── 📁 Providers/
│   │       ├── 📁 Services/
│   │       ├── 📁 Staff/
│   │       ├── 📁 Customers/
│   │       └── 📁 Bookings/
│   │
│   ├── 📦 Bookity.Infrastructure/
│   │   ├── 📁 Persistence/
│   │   │   ├── 📄 ApplicationDbContext.cs
│   │   │   ├── 📄 UnitOfWork.cs
│   │   │   ├── 📁 Configurations/
│   │   │   └── 📁 Repositories/
│   │   ├── 📁 Services/
│   │   └── 📄 DependencyInjection.cs
│   │
│   └── 📦 Bookity.Api/
│       ├── 📁 Controllers/
│       ├── 📁 Middleware/
│       └── 📄 Program.cs
│
└── 📁 tests/
```

---

## 10. API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| **Providers** |
| POST | `/api/providers` | Register provider |
| GET | `/api/providers/{id}` | Get provider |
| GET | `/api/providers/by-slug/{slug}` | Get by slug |
| **Services** |
| POST | `/api/providers/{providerId}/services` | Create service |
| GET | `/api/providers/{providerId}/services` | List services |
| PUT | `/api/services/{id}/schedule` | Set schedule (Direct) |
| **Staff** |
| POST | `/api/providers/{providerId}/staff` | Create staff |
| GET | `/api/services/{serviceId}/staff` | Get staff for service |
| PUT | `/api/staff/{id}/schedule` | Set schedule |
| **Bookings** |
| POST | `/api/bookings` | Create booking |
| GET | `/api/bookings/{id}` | Get booking |
| POST | `/api/bookings/{id}/confirm` | Confirm |
| POST | `/api/bookings/{id}/cancel` | Cancel |
| **Availability** |
| GET | `/api/services/{serviceId}/availability?date=` | Direct mode |
| GET | `/api/staff/{staffId}/availability?serviceId=&date=` | Staff mode |

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