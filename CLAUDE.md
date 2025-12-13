# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Bookity is a marketplace platform for bookable services built with .NET 10, following Clean Architecture and Domain-Driven Design (DDD) principles. The system supports two distinct booking modes:

1. **Direct Mode**: Resource-based bookings (badminton courts, meeting rooms, solo consultants) without staff assignment
2. **Staff-Based Mode**: Service bookings requiring specific staff selection (spa, salon, clinic services)

## Architecture

### Clean Architecture Layers

```
Booking.Domain/          → Core domain logic, aggregates, value objects, domain events
Booking.Application/     → Use cases, CQRS command/query handlers, DTOs
Booking.Infrastructure/  → Domain services implementation (availability, number generation)
Booking.Persistence/     → Database context, repository implementations, EF configurations
Booking.API/            → REST API controllers, dependency injection setup
```

### Dependency Flow
- API → Application → Domain
- Infrastructure → Domain
- Persistence → Domain
- Application depends on Domain only
- Domain has no dependencies (pure domain logic)

## Core Domain Concepts

### Aggregates (5 Total)

All aggregates extend `AggregateRoot<TId>` which provides domain event management. Each aggregate uses strongly-typed IDs (readonly record structs).

1. **Tenant** - Business owner (badminton yard, spa, consultant)
   - Has `TimeZone` (IANA format, e.g., "Asia/Ho_Chi_Minh")
   - Has unique `Slug` for URL-friendly identification

2. **Service** - Bookable offering (court, massage, consultation)
   - `BookingMode` enum: Direct | StaffBased
   - **Direct mode**: Service has its own `WeeklySchedule`
   - **Staff-based mode**: Schedule lives on Staff, not Service
   - `MaxConcurrentBookings`: Supports group classes or multiple resources (default: 1)

3. **Staff** - Service provider (OPTIONAL, only for StaffBased services)
   - Has `WeeklySchedule` for working hours
   - Can be assigned to multiple services via `ServiceIds` list
   - Only created when service uses `BookingMode.StaffBased`

4. **Customer** - End user making bookings
   - Simple profile: name, email, phone

5. **Booking** - Core transaction entity
   - `StaffId` is nullable (null for Direct mode, required for StaffBased)
   - Captures service/staff snapshots (name, price) for historical accuracy
   - Has `BookingStatus`: Pending | Confirmed | Cancelled | Completed | NoShow
   - Two factory methods: `CreateDirect()` and `CreateWithStaff()`

### Key Design Patterns

- **Dual Booking Modes**: Single Booking aggregate handles both direct and staff-based bookings via nullable `StaffId`
- **Schedule Ownership**:
  - Direct mode → Schedule on Service
  - StaffBased mode → Schedule on Staff
- **Snapshot Pattern**: Booking stores service name/price at booking time for immutability
- **Domain Events**: Aggregates raise events (BookingCreated, BookingConfirmed, etc.) via protected `AddDomainEvent()`

### Value Objects

- **Strongly-Typed IDs**: `TenantId`, `ServiceId`, `StaffId`, `CustomerId`, `BookingId` (all `readonly record struct` wrapping `Guid`)
- **WeeklySchedule**: Dictionary<DayOfWeek, WorkingHours?>
- **WorkingHours**: StartTime, EndTime (TimeOnly)

### Base Classes

- `Entity<TId>`: Provides Id, CreatedAt, UpdatedAt, equality by ID
- `AggregateRoot<TId>`: Extends Entity, adds domain event collection
- All IDs use `TId : struct` constraint for value type safety

## Common Development Commands

### Build and Run
```bash
# Build entire solution
dotnet build Booking.slnx

# Run API project
dotnet run --project src/Booking.API

# Restore dependencies
dotnet restore
```

### Project References
When adding dependencies, respect the layer architecture:
```bash
# Example: Add Domain reference to Application
dotnet add src/Booking.Application reference src/Booking.Domain

# Example: Add Application reference to API
dotnet add src/Booking.API reference src/Booking.Application
```

### Testing
```bash
# Run all tests (when test project exists)
dotnet test

# Run specific test project
dotnet test tests/Booking.Domain.Tests

# Run with detailed output
dotnet test --verbosity detailed
```

## Domain Implementation Guidelines

### Creating Aggregates
- Use private parameterless constructors for EF Core compatibility
- Use static `Create()` factory methods for instantiation
- Validate invariants in factory methods, throw `DomainException` on violation
- Set `Id` using `[AggregateType]Id.New()` for new Guid
- Raise domain events via `AddDomainEvent()` after state changes
- Always set `CreatedAt` and `UpdatedAt` timestamps

### Repository Pattern
- Define interfaces in Domain layer (e.g., `IBookingRepository`)
- Implement in Persistence layer
- All repositories extend `IRepository<TAggregate, TId>`
- Key repository methods:
  - `GetByIdAsync(TId, CancellationToken)`
  - `AddAsync(TAggregate, CancellationToken)`
  - Custom queries specific to aggregate (e.g., `GetByBookingNumberAsync`)

### Domain Services
Located in `Booking.Domain/Services/`:
- `IBookingAvailabilityService`: Handles slot availability logic for both booking modes
  - `GetAvailableSlotsForServiceAsync()` - Direct mode
  - `GetAvailableSlotsForStaffAsync()` - StaffBased mode
  - `IsSlotAvailableAsync()` - Unified availability check
- `IBookingNumberGenerator`: Generates unique booking identifiers

### Booking Mode Logic
When implementing booking features:
```csharp
// Check if service requires staff
if (service.RequiresStaff() && !request.StaffId.HasValue)
    return Error("Staff required for this service");

// Use appropriate factory method
if (service.RequiresStaff())
    booking = Booking.CreateWithStaff(...);
else
    booking = Booking.CreateDirect(...);
```

## Current State

The repository structure and base domain classes are established. Key files exist but many aggregates are placeholder stubs (marked with TODO comments). The MVP design document ([MVP_DESIGN.md](MVP_DESIGN.md)) contains the complete domain model specification with detailed examples and code samples.

## Critical Business Rules

1. **Staff Optionality**: Never assume staff exists - always check `StaffId.HasValue` on Booking
2. **Schedule Location**: Direct mode services have schedules; StaffBased services don't (staff have them)
3. **MaxConcurrentBookings**: Only applies to Direct mode (courts, rooms, group classes)
4. **Timezone Handling**: All tenants have timezone - use it for date/time calculations
5. **Snapshot Immutability**: Booking snapshots (ServiceName, ServicePrice, StaffName) should never update after creation

## Technology Stack

- **.NET 10.0** (target framework)
- **C# 13** with nullable reference types enabled
- **Clean Architecture** + **DDD tactical patterns**
- No database/ORM configured yet (Persistence layer empty)
- No CQRS library added yet (Application layer empty)
