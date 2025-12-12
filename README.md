# Library Management System - Web API

## Problem Statement
Model a basic library management system API with CRUD operations in .NET Core using Entity Framework.

## Technologies
- .NET Core 8.0 Web API
- Entity Framework Core
- SQL Server
- Repository Pattern with OOP Principles
- AutoMapper
- Structured Logging

## Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## Project Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd UnisontechAssessment
```

### 2. Configure Database Connection
Update the connection string in `Library.API/appsettings.json` and `Library.API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<Your Server>;Database=Library;User Id=<Your User>;Password=<Your Password>;TrustServerCertificate=true"
  }
}
```
Replace:
- `<Your Server>` - Your SQL Server instance (e.g., `localhost`, `localhost\\SQLEXPRESS`, or `localhost,1433`)
- `<Your User>` - Your SQL Server username (e.g., `sa`)
- `<Your Password>` - Your SQL Server password

### 3. Apply Database Migrations
Navigate to the API project directory and apply migrations:
```bash
cd Library.API
dotnet ef database update
```

If you need to create migrations (for any schema changes):
```bash
dotnet ef migrations add MigrationName --project ../Library.Repository
```

### 4. Build the Solution
```bash
cd ..
dotnet build
```

### 5. Run the Application
```bash
cd Library.API
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5052`
- **HTTPS**: `https://localhost:7206`
- **Swagger UI**: `http://localhost:5052/swagger`

## Architecture & Design Patterns

### Project Structure
- **Library.API** - Controllers, Middleware, API Configuration
- **Library.Core** - Entities, DTOs, Interfaces, Domain Models
- **Library.Repository** - EF Core DbContext, Repositories, Unit of Work
- **Library.Service** - Business Logic, AutoMapper Profiles

### Key Features
- ✅ **Repository Pattern** with Unit of Work
- ✅ **Dependency Injection** throughout
- ✅ **Global Exception Handling** via middleware
- ✅ **Structured Logging** with ILogger
- ✅ **AutoMapper** for DTO mapping
- ✅ **Pagination** support
- ✅ **Data Validation** with DataAnnotations
- ✅ **Business Rules** validation

## C# Models / Entities

### 1. Book
- BookId (PK), Title, ISBN, Author, Publisher, PublicationYear, Genre, TotalCopies, AvailableCopies, LibraryId (FK)

### 2. Member
- MemberId (PK), FirstName, LastName, Email, PhoneNumber, Address, MembershipDate, MembershipType, IsActive

### 3. Borrow
- BorrowId (PK), BookId (FK), MemberId (FK), BorrowDate, DueDate, ReturnDate, Status, Fine

### 4. Library
- LibraryId (PK), Name, Address, PhoneNumber, Email, OpeningTime, ClosingTime

## Database Design
**Relationships:**
- Library → Books (1:N)
- Member → Borrows (1:N)
- Book → Borrows (1:N)

## API Endpoints

### Books
- `GET /api/books?page=1&pageSize=10` - Get all books (with pagination)
- `GET /api/books/{id}` - Get book by ID
- `GET /api/books/available` - Get available books
- `GET /api/books/library/{libraryId}` - Get books by library
- `POST /api/books` - Create book
- `PUT /api/books/{id}` - Update book
- `DELETE /api/books/{id}` - Delete book

### Members
- `GET /api/members` - Get all members
- `GET /api/members/{id}` - Get member by ID
- `GET /api/members/{id}/borrows` - Get member with borrow history
- `GET /api/members/active` - Get active members
- `GET /api/members/type/{membershipType}` - Get members by type
- `POST /api/members` - Create member
- `PUT /api/members/{id}` - Update member
- `DELETE /api/members/{id}` - Delete member

### Borrows
- `GET /api/borrows` - Get all borrows
- `GET /api/borrows/{id}` - Get borrow by ID
- `GET /api/borrows/overdue` - Get overdue borrows
- `GET /api/borrows/active` - Get active borrows
- `GET /api/borrows/member/{memberId}` - Get borrows by member
- `POST /api/borrows` - Borrow book
- `PUT /api/borrows/{id}/return` - Return book

### Libraries
- `GET /api/libraries` - Get all libraries
- `GET /api/libraries/{id}` - Get library by ID
- `GET /api/libraries/{id}/books` - Get library with books
- `POST /api/libraries` - Create library
- `PUT /api/libraries/{id}` - Update library
- `DELETE /api/libraries/{id}` - Delete library

## Testing the API

Use **Swagger UI** at `http://localhost:5052/swagger` or tools like Postman/Insomnia.

### Sample Request: Create a Library
```json
POST /api/libraries
{
  "name": "Central Library",
  "address": "123 Main St",
  "phoneNumber": "+1234567890",
  "email": "central@library.com",
  "openingTime": "09:00:00",
  "closingTime": "18:00:00"
}
```

### Sample Request: Create a Book
```json
POST /api/books
{
  "title": "The Great Gatsby",
  "isbn": "978-0-7432-7356-5",
  "author": "F. Scott Fitzgerald",
  "publisher": "Scribner",
  "publicationYear": 1925,
  "genre": "Fiction",
  "totalCopies": 5,
  "availableCopies": 5,
  "libraryId": 1
}
```

### Sample Request: Borrow a Book
```json
POST /api/borrows
{
  "bookId": 1,
  "memberId": 1,
  "borrowDurationDays": 14
}
```

## Business Rules
- Books cannot be deleted if they have active borrows
- Members cannot be deleted if they have active borrows
- Members with overdue books cannot borrow new books
- Available copies are automatically managed during borrow/return
- Fine calculation: $1 per day for overdue books
- Validation for closing time must be after opening time

## Troubleshooting

### Connection Issues
If you encounter database connection errors:
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure database user has proper permissions
4. For Windows Authentication, use: `Server=localhost;Database=Library;Integrated Security=true;TrustServerCertificate=true`

### Migration Issues
If migrations fail:
```bash
# Remove existing database
dotnet ef database drop --project Library.API

# Reapply migrations
dotnet ef database update --project Library.API
```

