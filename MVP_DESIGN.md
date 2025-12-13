# Bookity - MVP Domain Model & Architecture (v2)

> **Version:** MVP 2.0  
> **Target Framework:** .NET 10  
> **Architecture:** Clean Architecture + DDD  
> **Scope:** Minimal Viable Product  
> **Key Change:** Staff is OPTIONAL - supports both resource-based and staff-based bookings

---

## 1. Platform Vision

### 1.1 What is Bookity?

A **marketplace for any bookable service** where anyone can:
- Publish their services/resources for booking
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
│  │  Example: Book "Meeting Room A" for Monday 14:00-16:00          │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                          │
│  MODE 2: STAFF-BASED (Staff Required)                                   │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  Customer ──────► Service ──────► Staff ──────► Booking         │   │
│  │                                                                  │   │
│  │  Example: Book "Thai Massage" with "Linh" for Friday 10:00      │   │
│  │  Example: Book "Haircut" with "Minh" for Tuesday 15:00          │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 2. MVP Scope

### 2.1 Included Features ✅

| Feature | Description |
|---------|-------------|
| Tenant Registration | Anyone can register their business |
| Service Management | Create services (direct or staff-based) |
| Staff Management | **Optional** - only for staff-based services |
| Customer Profile | Basic customer information |
| Booking Flow | Book directly or with staff selection |
| Availability Check | Check available time slots |

### 2.2 Deferred to V2 ⏳

- Cancellation policies & fees
- Buffer time between bookings
- Staff availability blocks (vacation)
- Service categories
- Booking rescheduling
- Multiple resources per service

---

## 3. Domain Model

### 3.1 Aggregate Overview

```
┌────────────────────────────────────────────────────────────────────┐
│                      MVP AGGREGATES (5)                            │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐          │
│  │  Tenant  │  │ Service  │  │  Staff   │  │ Customer │          │
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

### 3.2 Tenant Aggregate

```
Tenant (Aggregate Root)
├── TenantId : TenantId
├── Name : string
├── Slug : string (URL-friendly, unique)
├── Description : string?
├── Email : string
├── Phone : string?
├── TimeZone : string (IANA format, e.g., "Asia/Ho_Chi_Minh")
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
- "Sân cầu lông Thành Công" (Badminton yard)
- "Sunny Spa & Massage" (Spa)
- "Phòng họp ABC Building" (Meeting rooms)

### 3.3 Service Aggregate ⭐ (Key Changes)

```
Service (Aggregate Root)
├── ServiceId : ServiceId
├── TenantId : TenantId
├── Name : string
├── Description : string?
├── DurationMinutes : int
├── Price : decimal
├── Currency : string (ISO 4217)
├── BookingMode : BookingMode (Direct | StaffBased)  ⭐ NEW
├── Schedule : WeeklySchedule (Value Object)         ⭐ Moved from Staff
│   └── Days : Dictionary<DayOfWeek, WorkingHours?>
├── MaxConcurrentBookings : int (default: 1)         ⭐ NEW (for courts, rooms)
├── IsActive : bool
├── CreatedAt : DateTime
├── UpdatedAt : DateTime
│
├── Methods:
│   ├── static Create(tenantId, name, duration, price, currency, bookingMode)
│   ├── Update(name, description, duration, price)
│   ├── SetSchedule(weeklySchedule)
│   ├── SetMaxConcurrentBookings(max)
│   ├── RequiresStaff() : bool
│   ├── Activate()
│   └── Deactivate()
```

**Key Design Decision:**
- `BookingMode.Direct` → Schedule is on **Service** (e.g., Court open 6:00-22:00)
- `BookingMode.StaffBased` → Schedule is on **Staff** (e.g., Linh works 9:00-18:00)

**Examples:**

| Service | BookingMode | MaxConcurrent | Schedule On |
|---------|-------------|---------------|-------------|
| "Court 1" | Direct | 1 | Service |
| "Meeting Room A" | Direct | 1 | Service |
| "Group Yoga Class" | Direct | 10 | Service |
| "Thai Massage" | StaffBased | N/A | Staff |
| "Haircut" | StaffBased | N/A | Staff |

### 3.4 Staff Aggregate (OPTIONAL)

```
Staff (Aggregate Root) - Only for StaffBased services
├── StaffId : StaffId
├── TenantId : TenantId
├── Name : string
├── Email : string?
├── Phone : string?
├── Schedule : WeeklySchedule (Value Object)
│   └── Days : Dictionary<DayOfWeek, WorkingHours?>
├── ServiceIds : List<ServiceId> (assigned services)
├── IsActive : bool
├── CreatedAt : DateTime
├── UpdatedAt : DateTime
│
├── Methods:
│   ├── static Create(tenantId, name)
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
├── TenantId : TenantId
├── ServiceId : ServiceId
├── StaffId : StaffId? (nullable - only for StaffBased)  ⭐ OPTIONAL
├── CustomerId : CustomerId
├── Date : DateOnly
├── StartTime : TimeOnly
├── EndTime : TimeOnly
├── ServiceName : string (snapshot)
├── ServicePrice : decimal (snapshot)
├── ServiceCurrency : string (snapshot)
├── StaffName : string? (snapshot, if applicable)        ⭐ OPTIONAL
├── Status : BookingStatus
├── CustomerNotes : string?
├── CancellationReason : string?
├── CreatedAt : DateTime
├── UpdatedAt : DateTime
│
├── Methods:
│   ├── static CreateDirect(tenantId, serviceId, customerId, date, slot, serviceSnapshot)
│   ├── static CreateWithStaff(tenantId, serviceId, staffId, customerId, date, slot, serviceSnapshot, staffName)
│   ├── Confirm()
│   ├── Cancel(reason?)
│   ├── Complete()
│   ├── MarkAsNoShow()
│   └── HasStaff() : bool
```

---

## 4. Real-World Examples

### 4.1 Badminton Court (Direct Mode)

```
┌─────────────────────────────────────────────────────────────────────────┐
│              TENANT: "Sân Cầu Lông Thành Công"                          │
│              Timezone: Asia/Ho_Chi_Minh                                  │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   SERVICES (BookingMode: Direct):                                       │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "Sân 1"                                                      │      │
│   │  Duration: 60 min | Price: 100,000 VND                      │      │
│   │  Schedule: Mon-Sun 6:00-22:00                               │      │
│   │  MaxConcurrentBookings: 1                                    │      │
│   └─────────────────────────────────────────────────────────────┘      │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "Sân 2"                                                      │      │
│   │  Duration: 60 min | Price: 100,000 VND                      │      │
│   │  Schedule: Mon-Sun 6:00-22:00                               │      │
│   │  MaxConcurrentBookings: 1                                    │      │
│   └─────────────────────────────────────────────────────────────┘      │
│                                                                          │
│   STAFF: None (not needed)                                              │
│                                                                          │
│   BOOKING FLOW:                                                         │
│   Customer ──► Select "Sân 1" ──► Pick time ──► Book                   │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### 4.2 Spa (Staff-Based Mode)

```
┌─────────────────────────────────────────────────────────────────────────┐
│              TENANT: "Sunny Spa & Massage"                              │
│              Timezone: Asia/Ho_Chi_Minh                                  │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   SERVICES (BookingMode: StaffBased):                                   │
│   ┌─────────────────┐  ┌─────────────────┐                             │
│   │ "Thai Massage"  │  │ "Foot Massage"  │                             │
│   │  60 min, 500K   │  │  45 min, 300K   │                             │
│   │  (no schedule)  │  │  (no schedule)  │                             │
│   └─────────────────┘  └─────────────────┘                             │
│                                                                          │
│   STAFF (each has own schedule):                                        │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "Linh" - Services: Thai Massage, Foot Massage               │      │
│   │          Schedule: Mon-Fri 9:00-18:00                       │      │
│   └─────────────────────────────────────────────────────────────┘      │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "Hoa" - Services: Thai Massage                              │      │
│   │         Schedule: Mon-Sat 10:00-19:00                       │      │
│   └─────────────────────────────────────────────────────────────┘      │
│                                                                          │
│   BOOKING FLOW:                                                         │
│   Customer ──► Select "Thai Massage" ──► Pick Staff ──► Pick time ──► Book│
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### 4.3 Freelance Consultant (Direct Mode, Solo)

```
┌─────────────────────────────────────────────────────────────────────────┐
│              TENANT: "Tuan - Business Consultant"                       │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   SERVICES (BookingMode: Direct):                                       │
│   ┌─────────────────────────────────────────────────────────────┐      │
│   │ "1-on-1 Consultation"                                        │      │
│   │  Duration: 60 min | Price: 1,000,000 VND                    │      │
│   │  Schedule: Mon-Fri 9:00-17:00                               │      │
│   │  MaxConcurrentBookings: 1                                    │      │
│   └─────────────────────────────────────────────────────────────┘      │
│                                                                          │
│   STAFF: None (solo business - owner IS the service provider)          │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 5. Value Objects

### 5.1 Strongly-Typed IDs

```csharp
public readonly record struct TenantId(Guid Value)
{
    public static TenantId New() => new(Guid.NewGuid());
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
    Direct = 1,      // Book service directly (courts, rooms, solo)
    StaffBased = 2   // Book service with specific staff (spa, salon)
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

    public WorkingHours? GetHours(DayOfWeek day) => 
        _days.GetValueOrDefault(day);
}
```

---

## 6. Domain Events

```csharp
// Booking Events
public sealed record BookingCreatedEvent(
    BookingId BookingId,
    string BookingNumber,
    TenantId TenantId,
    ServiceId ServiceId,
    StaffId? StaffId,  // Nullable
    CustomerId CustomerId,
    DateOnly Date,
    TimeOnly StartTime) : IDomainEvent;

public sealed record BookingConfirmedEvent(
    BookingId BookingId,
    DateTime ConfirmedAt) : IDomainEvent;

public sealed record BookingCancelledEvent(
    BookingId BookingId,
    string? Reason,
    DateTime CancelledAt) : IDomainEvent;

// Service Events
public sealed record ServiceCreatedEvent(
    ServiceId ServiceId,
    TenantId TenantId,
    string Name,
    BookingMode BookingMode) : IDomainEvent;

// Staff Events (only for StaffBased services)
public sealed record StaffCreatedEvent(
    StaffId StaffId,
    TenantId TenantId,
    string Name) : IDomainEvent;
```

---

## 7. Domain Services

```csharp
public interface IBookingAvailabilityService
{
    /// <summary>
    /// Get available slots for DIRECT booking mode
    /// </summary>
    Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsForServiceAsync(
        ServiceId serviceId,
        DateOnly date,
        CancellationToken ct = default);

    /// <summary>
    /// Get available slots for STAFF-BASED booking mode
    /// </summary>
    Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsForStaffAsync(
        StaffId staffId,
        ServiceId serviceId,
        DateOnly date,
        CancellationToken ct = default);

    /// <summary>
    /// Check if slot is available (handles both modes)
    /// </summary>
    Task<bool> IsSlotAvailableAsync(
        ServiceId serviceId,
        StaffId? staffId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken ct = default);
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
public interface IServiceRepository : IRepository<Service, ServiceId>
{
    Task<IReadOnlyList<Service>> GetByTenantIdAsync(
        TenantId tenantId, CancellationToken ct = default);
    
    Task<IReadOnlyList<Service>> GetStaffBasedByTenantIdAsync(
        TenantId tenantId, CancellationToken ct = default);
}

public interface IStaffRepository : IRepository<Staff, StaffId>
{
    Task<IReadOnlyList<Staff>> GetByTenantIdAsync(
        TenantId tenantId, CancellationToken ct = default);
    
    Task<IReadOnlyList<Staff>> GetByServiceIdAsync(
        ServiceId serviceId, CancellationToken ct = default);
}

public interface IBookingRepository : IRepository<Booking, BookingId>
{
    Task<Booking?> GetByBookingNumberAsync(
        string bookingNumber, CancellationToken ct = default);
    
    /// <summary>
    /// For Direct mode - get bookings by service and date
    /// </summary>
    Task<IReadOnlyList<Booking>> GetByServiceAndDateAsync(
        ServiceId serviceId,
        DateOnly date,
        CancellationToken ct = default);
    
    /// <summary>
    /// For StaffBased mode - get bookings by staff and date
    /// </summary>
    Task<IReadOnlyList<Booking>> GetByStaffAndDateAsync(
        StaffId staffId,
        DateOnly date,
        CancellationToken ct = default);
    
    Task<IReadOnlyList<Booking>> GetByCustomerIdAsync(
        CustomerId customerId,
        CancellationToken ct = default);
}
```

---

## 9. Project Structure

```
📁 Bookity/
├── 📁 src/
│   ├── 📦 Bookity.Domain/
│   │   ├── 📁 Aggregates/
│   │   │   ├── 📁 Tenants/
│   │   │   │   ├── 📄 Tenant.cs
│   │   │   │   ├── 📄 TenantId.cs
│   │   │   │   └── 📄 ITenantRepository.cs
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
│   │   │   ├── 📄 BookingCreatedEvent.cs
│   │   │   ├── 📄 BookingConfirmedEvent.cs
│   │   │   └── 📄 BookingCancelledEvent.cs
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
│   │       ├── 📁 Tenants/
│   │       │   ├── 📁 Commands/CreateTenant/
│   │       │   └── 📁 Queries/GetTenant/
│   │       ├── 📁 Services/
│   │       │   ├── 📁 Commands/CreateService/
│   │       │   ├── 📁 Commands/UpdateServiceSchedule/
│   │       │   └── 📁 Queries/GetServicesByTenant/
│   │       ├── 📁 Staff/
│   │       │   ├── 📁 Commands/CreateStaff/
│   │       │   ├── 📁 Commands/AssignServiceToStaff/
│   │       │   └── 📁 Queries/GetStaffByService/
│   │       ├── 📁 Customers/
│   │       └── 📁 Bookings/
│   │           ├── 📁 Commands/CreateBooking/
│   │           ├── 📁 Commands/ConfirmBooking/
│   │           ├── 📁 Commands/CancelBooking/
│   │           └── 📁 Queries/GetAvailableSlots/
│   │
│   ├── 📦 Bookity.Infrastructure/
│   │   ├── 📁 Persistence/
│   │   │   ├── 📄 ApplicationDbContext.cs
│   │   │   ├── 📁 Configurations/
│   │   │   └── 📁 Repositories/
│   │   ├── 📁 Services/
│   │   │   ├── 📄 BookingAvailabilityService.cs
│   │   │   └── 📄 BookingNumberGenerator.cs
│   │   └── 📄 DependencyInjection.cs
│   │
│   └── 📦 Bookity.Api/
│       ├── 📁 Controllers/
│       └── 📄 Program.cs
│
└── 📁 tests/
```

---

## 10. Core Code Examples

### 10.1 Service Aggregate

```csharp
public sealed class Service : AggregateRoot<ServiceId>
{
    public TenantId TenantId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = null!;
    public BookingMode BookingMode { get; private set; }
    public WeeklySchedule? Schedule { get; private set; }  // For Direct mode
    public int MaxConcurrentBookings { get; private set; } = 1;
    public bool IsActive { get; private set; }

    private Service() { }

    public static Service Create(
        TenantId tenantId,
        string name,
        int durationMinutes,
        decimal price,
        string currency,
        BookingMode bookingMode)
    {
        if (durationMinutes <= 0)
            throw new DomainException("Duration must be positive");
        if (price < 0)
            throw new DomainException("Price cannot be negative");

        var service = new Service
        {
            Id = ServiceId.New(),
            TenantId = tenantId,
            Name = name,
            DurationMinutes = durationMinutes,
            Price = price,
            Currency = currency,
            BookingMode = bookingMode,
            MaxConcurrentBookings = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        service.AddDomainEvent(new ServiceCreatedEvent(
            service.Id, tenantId, name, bookingMode));

        return service;
    }

    public void SetSchedule(WeeklySchedule schedule)
    {
        if (BookingMode == BookingMode.StaffBased)
            throw new DomainException("Staff-based services don't have their own schedule");
        
        Schedule = schedule;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMaxConcurrentBookings(int max)
    {
        if (max <= 0)
            throw new DomainException("Max concurrent bookings must be positive");
        
        MaxConcurrentBookings = max;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool RequiresStaff() => BookingMode == BookingMode.StaffBased;
}
```

### 10.2 Booking Aggregate

```csharp
public sealed class Booking : AggregateRoot<BookingId>
{
    public string BookingNumber { get; private set; } = null!;
    public TenantId TenantId { get; private set; }
    public ServiceId ServiceId { get; private set; }
    public StaffId? StaffId { get; private set; }  // Nullable for Direct mode
    public CustomerId CustomerId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string ServiceName { get; private set; } = null!;
    public decimal ServicePrice { get; private set; }
    public string ServiceCurrency { get; private set; } = null!;
    public string? StaffName { get; private set; }  // Nullable
    public BookingStatus Status { get; private set; }
    public string? CustomerNotes { get; private set; }
    public string? CancellationReason { get; private set; }

    private Booking() { }

    /// <summary>
    /// Create booking for DIRECT mode (no staff)
    /// </summary>
    public static Booking CreateDirect(
        string bookingNumber,
        TenantId tenantId,
        ServiceId serviceId,
        CustomerId customerId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string serviceName,
        decimal servicePrice,
        string serviceCurrency)
    {
        var booking = new Booking
        {
            Id = BookingId.New(),
            BookingNumber = bookingNumber,
            TenantId = tenantId,
            ServiceId = serviceId,
            StaffId = null,
            CustomerId = customerId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            ServiceName = serviceName,
            ServicePrice = servicePrice,
            ServiceCurrency = serviceCurrency,
            StaffName = null,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        booking.AddDomainEvent(new BookingCreatedEvent(
            booking.Id, bookingNumber, tenantId, serviceId,
            null, customerId, date, startTime));

        return booking;
    }

    /// <summary>
    /// Create booking for STAFF-BASED mode
    /// </summary>
    public static Booking CreateWithStaff(
        string bookingNumber,
        TenantId tenantId,
        ServiceId serviceId,
        StaffId staffId,
        CustomerId customerId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string serviceName,
        decimal servicePrice,
        string serviceCurrency,
        string staffName)
    {
        var booking = new Booking
        {
            Id = BookingId.New(),
            BookingNumber = bookingNumber,
            TenantId = tenantId,
            ServiceId = serviceId,
            StaffId = staffId,
            CustomerId = customerId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            ServiceName = serviceName,
            ServicePrice = servicePrice,
            ServiceCurrency = serviceCurrency,
            StaffName = staffName,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        booking.AddDomainEvent(new BookingCreatedEvent(
            booking.Id, bookingNumber, tenantId, serviceId,
            staffId, customerId, date, startTime));

        return booking;
    }

    public bool HasStaff() => StaffId.HasValue;

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException($"Cannot confirm booking in {Status} status");

        Status = BookingStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BookingConfirmedEvent(Id, DateTime.UtcNow));
    }

    public void Cancel(string? reason = null)
    {
        if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
            throw new DomainException($"Cannot cancel booking in {Status} status");

        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BookingCancelledEvent(Id, reason, DateTime.UtcNow));
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new DomainException($"Cannot complete booking in {Status} status");

        Status = BookingStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsNoShow()
    {
        if (Status != BookingStatus.Confirmed)
            throw new DomainException($"Cannot mark as no-show in {Status} status");

        Status = BookingStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### 10.3 CreateBooking Command Handler

```csharp
public sealed record CreateBookingCommand(
    Guid TenantId,
    Guid ServiceId,
    Guid? StaffId,  // Nullable - only for StaffBased
    Guid CustomerId,
    DateOnly Date,
    TimeOnly StartTime,
    string? CustomerNotes) : IRequest<Result<BookingResponse>>;

public sealed class CreateBookingCommandHandler 
    : IRequestHandler<CreateBookingCommand, Result<BookingResponse>>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingAvailabilityService _availabilityService;
    private readonly IBookingNumberGenerator _numberGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<BookingResponse>> Handle(
        CreateBookingCommand request,
        CancellationToken ct)
    {
        // 1. Get service
        var service = await _serviceRepository.GetByIdAsync(
            new ServiceId(request.ServiceId), ct);
        if (service is null)
            return Result.Failure<BookingResponse>("Service not found");

        // 2. Validate booking mode
        if (service.RequiresStaff() && !request.StaffId.HasValue)
            return Result.Failure<BookingResponse>("Staff is required for this service");

        if (!service.RequiresStaff() && request.StaffId.HasValue)
            return Result.Failure<BookingResponse>("This service does not support staff selection");

        // 3. Calculate end time
        var endTime = request.StartTime.AddMinutes(service.DurationMinutes);

        // 4. Check availability
        var staffId = request.StaffId.HasValue ? new StaffId(request.StaffId.Value) : (StaffId?)null;
        var isAvailable = await _availabilityService.IsSlotAvailableAsync(
            service.Id, staffId, request.Date, request.StartTime, endTime, ct);
        
        if (!isAvailable)
            return Result.Failure<BookingResponse>("Time slot is not available");

        // 5. Generate booking number
        var bookingNumber = await _numberGenerator.GenerateAsync(ct);

        // 6. Create booking based on mode
        Booking booking;
        
        if (service.RequiresStaff())
        {
            var staff = await _staffRepository.GetByIdAsync(staffId!.Value, ct);
            if (staff is null)
                return Result.Failure<BookingResponse>("Staff not found");

            booking = Booking.CreateWithStaff(
                bookingNumber,
                new TenantId(request.TenantId),
                service.Id,
                staff.Id,
                new CustomerId(request.CustomerId),
                request.Date,
                request.StartTime,
                endTime,
                service.Name,
                service.Price,
                service.Currency,
                staff.Name);
        }
        else
        {
            booking = Booking.CreateDirect(
                bookingNumber,
                new TenantId(request.TenantId),
                service.Id,
                new CustomerId(request.CustomerId),
                request.Date,
                request.StartTime,
                endTime,
                service.Name,
                service.Price,
                service.Currency);
        }

        // 7. Persist
        await _bookingRepository.AddAsync(booking, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new BookingResponse(
            booking.Id.Value,
            booking.BookingNumber,
            booking.ServiceName,
            booking.StaffName,
            booking.Date,
            booking.StartTime,
            booking.EndTime,
            booking.Status.ToString()));
    }
}

public record BookingResponse(
    Guid Id,
    string BookingNumber,
    string ServiceName,
    string? StaffName,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Status);
```

---

## 11. API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| **Tenants** |
| POST | `/api/tenants` | Register tenant |
| GET | `/api/tenants/{id}` | Get tenant |
| GET | `/api/tenants/by-slug/{slug}` | Get tenant by slug |
| **Services** |
| POST | `/api/tenants/{tenantId}/services` | Create service |
| GET | `/api/tenants/{tenantId}/services` | List services |
| PUT | `/api/services/{id}` | Update service |
| PUT | `/api/services/{id}/schedule` | Set schedule (Direct mode) |
| **Staff** (Optional) |
| POST | `/api/tenants/{tenantId}/staff` | Create staff |
| GET | `/api/tenants/{tenantId}/staff` | List staff |
| GET | `/api/services/{serviceId}/staff` | Get staff for service |
| PUT | `/api/staff/{id}/schedule` | Set staff schedule |
| POST | `/api/staff/{id}/services/{serviceId}` | Assign service |
| **Customers** |
| POST | `/api/customers` | Create customer |
| GET | `/api/customers/{id}` | Get customer |
| **Bookings** |
| POST | `/api/bookings` | Create booking |
| GET | `/api/bookings/{id}` | Get booking |
| POST | `/api/bookings/{id}/confirm` | Confirm |
| POST | `/api/bookings/{id}/cancel` | Cancel |
| **Availability** |
| GET | `/api/services/{serviceId}/availability?date=` | Get slots (Direct) |
| GET | `/api/staff/{staffId}/availability?serviceId=&date=` | Get slots (Staff) |

---

## 12. Summary: Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| **Staff is optional** | Supports courts, rooms, solo consultants without staff |
| **BookingMode on Service** | Determines if service needs staff selection |
| **Schedule on Service (Direct)** | Court/room has operating hours |
| **Schedule on Staff (StaffBased)** | Each therapist has their own hours |
| **MaxConcurrentBookings** | Allows group classes, multiple courts |
| **StaffId nullable on Booking** | Same Booking entity for both modes |

---

*MVP Domain Model v2 for Bookity*  
*Now supports: Courts, Rooms, Studios, Spas, Salons, Clinics, Solo consultants*