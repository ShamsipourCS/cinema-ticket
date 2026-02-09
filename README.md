# CinemaTicket - وظایف تیمی

> **Movie Ticket Booking System** built with ASP.NET Core 7, CLEAN Architecture, and CQRS pattern.

## 👥 اعضای تیم

### 🎯 علی شامنصوری
**تمرکز**: Core Architecture, Security, Complex Business Logic

**وظایف اصلی**:
- Project initialization and CLEAN architecture setup
- MediatR and CQRS pattern implementation (pipeline behaviors, command/query structure)
- Authentication & authorization system (JWT, Identity, Refresh Tokens)
- Payment integration (Stripe test mode) with webhook handling
- Global middleware (exception handling, logging behaviors)
- Auth-related CQRS handlers (Login, Register, RefreshToken, Logout)
- Payment CQRS handlers (CreatePaymentIntent, ConfirmPayment)
- Stripe webhook endpoint implementation and signature verification
- API controllers implementation (`AuthController`, `PaymentsController`)
- Data seeding infrastructure and implementation (Users, Showtimes, Tickets, Payments)
- Code reviews
- PR reviews and merge approvals

**خروجی‌های کلیدی**:
- `BaseEntity`, `IRepository`, `IUnitOfWork` interfaces
- MediatR setup with `ValidationBehavior` and `LoggingBehavior` pipelines
- CQRS pattern structure (Commands, Queries, Handlers, Validators)
- `User`, `RefreshToken` entities with repository
- `JwtService` and auth CQRS handlers (Login, Register, RefreshToken, Logout)
- `StripePaymentService` and payment CQRS handlers (CreatePaymentIntent, ConfirmPayment)
- Stripe webhook handler (`/api/payments/webhook`) with signature verification
- `ExceptionHandlingMiddleware`
- Dependency injection configuration across all layers
- Unit tests for authentication and payment handlers
- API Controllers: `AuthController`, `PaymentsController`
- Data Seeders: Base infrastructure + Users (Admin/Customers) + Booking domain (Showtimes, Tickets, Payments)

---

### 📚 ابوالفضل اسمعیل بیگی (Catalog Management)
**تمرکز**: Standard CRUD Operations - Movies & Cinemas

**وظایف اصلی**:
- Catalog domain entities (`Movie`, `Cinema`, `Hall`, `Seat`)
- Entity configurations (Fluent API) for catalog domain
- Repository implementations (`MovieRepository`, `CinemaRepository`)
- CQRS handlers for Movies (Create, Update, GetList, GetById)
- CQRS handlers for Cinemas/Halls (Create, Update, GetList, GetById)
- FluentValidation validators for catalog commands
- API controllers implementation (`MoviesController`, `CinemasController`)
- Unit tests for catalog handlers
- Catalog data seeders (movies, cinemas, halls, seats)

**خروجی‌های کلیدی**:
- All Movie and Cinema CRUD operations (Commands/Queries/Handlers)
- Validation logic for catalog operations
- API Controllers: `MoviesController`, `CinemasController`
- Test coverage for catalog features
- Data Seeders: Sample movies, cinemas, halls, and seats for testing

---

### 🎟️ امیرمهدی علیپور تاجانی (Booking Logic)
**تمرکز**: Complex CRUD Operations - Showtimes & Tickets

**وظایف اصلی**:
- Booking domain entities (`Showtime`, `Ticket`, `Payment`)
- Entity configurations for booking domain
- Repository implementations (`ShowtimeRepository`, `TicketRepository`)
- CQRS handlers for Showtimes (Create, Update, GetList, GetAvailableSeats)
- `CreateBookingCommand` (multi-entity transaction with seat locking)
- `ReservationCleanupService` (background job for expired reservations)
- FluentValidation validators for booking commands
- API controllers implementation (`ShowtimesController`, `TicketsController`, `BookingController`, `SeatsController`)

**خروجی‌های کلیدی**:
- All Showtime operations with seat availability queries
- Booking creation with 10-minute reservation timeout
- Background service for cleaning expired reservations
- API Controllers: `ShowtimesController`, `TicketsController`, `BookingController`, `SeatsController`

---

## 📋 فاز های توسعه

### Phase 1: Foundation
- **علی شامنصوری**: Project setup, base classes, DbContext skeleton → **BLOCKS** other developers work
- **ابوالفضل اسمعیل بیگی**: Catalog domain entities + configurations (after Ali Shamansouri merge)
- **امیرمهدی علیپور تاجانی**: Booking domain entities + configurations (after Ali Shamansouri merge)

### Phase 2: Application Core
- **علی شامنصوری**: MediatR behaviors, Stripe service, seeder infrastructure + user/booking seeders, review PRs
- **ابوالفضل اسمعیل بیگی**: Movie/Cinema CQRS handlers + validators, catalog seeders (movies, cinemas, halls, seats)
- **امیرمهدی علیپور تاجانی**: Showtime CQRS handlers + seat availability queries

### Phase 3: Advanced Logic & API
- **علی شامنصوری**: Auth handlers, payment intent handler, webhook implementation, DI wiring
- **ابوالفضل اسمعیل بیگی**: Controllers, unit tests for catalog
- **امیرمهدی علیپور تاجانی**: Booking transaction logic, background jobs

### Phase 4: Polish
- **علی شامنصوری**: Security audit and project polish

---

## 🔄 Git Workflow

### Branch Naming
پیروی از روش نامگذاری مبتنی بر

- `feature/*`: ویژگی یا تغیر جدید
- `fix/*`: باگ فیکس
- `docs/*`: تغیرات در مستندات
- `chore/*` وظایفی که ویژگی جدید اضافه نمی‌کنند ولی در ساختار مهم هستند

### PR Rules
1. هیچ وقت مستقیم به برنچ اصلی کامیت نزنید
2. همیشه "علی شامنصوری" را به عنوان ناظر تگ کنید
3. دسکریپشن باید از الگو کامیت ها و برنچ ها پیروی کند
4. بیلد و تست گرفتن پروژه قبل از مرج

---

## 📁 Project Structure

```
Domain/           → Pure business entities, interfaces
Application/      → CQRS handlers, DTOs, validators
Persistence/      → EF Core, repositories
Infrastructure/   → External services: JWT, Stripe
API/              → Controllers, middleware
```

---

## 🚀 Quick Commands

```bash
# Build entire solution
dotnet build

# Run API
dotnet run --project src/CinemaTicket.API/CinemaTicket.API.csproj

# Add migration (after entity changes)
dotnet ef migrations add MigrationName --project src/CinemaTicket.Persistence --startup-project src/CinemaTicket.API

# Update database
dotnet ef database update --project src/CinemaTicket.Persistence --startup-project src/CinemaTicket.API

# Run tests
dotnet test
```

### Stripe CLI (Testing Payments)
```bash
# Login to Stripe CLI
stripe login

# Forward webhooks to local API
stripe listen --forward-to http://localhost:5001/api/payments/webhook

# Confirm payment intent (testing)
stripe payment_intents confirm pi_... --payment-method pm_card_visa --return-url http://localhost:5001/success

# List recent payment intents
stripe payment_intents list --limit 10
```
