# Library Management System - Web API

## Problem Statement
Model a basic library management system API with CRUD operations in .NET Core using Entity Framework.

## Technologies
- .NET Core 8.0 Web API
- Entity Framework Core
- SQL Server
- Repository Pattern with OOP Principles

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
- `GET /api/books` - Get all books
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Create book
- `PUT /api/books/{id}` - Update book
- `DELETE /api/books/{id}` - Delete book

### Members
- `GET /api/members` - Get all members
- `GET /api/members/{id}` - Get member by ID
- `POST /api/members` - Create member
- `PUT /api/members/{id}` - Update member
- `DELETE /api/members/{id}` - Delete member

### Borrows
- `GET /api/borrows` - Get all borrows
- `GET /api/borrows/{id}` - Get borrow by ID
- `POST /api/borrows` - Borrow book
- `PUT /api/borrows/{id}/return` - Return book
- `GET /api/borrows/overdue` - Get overdue borrows

### Library
- `GET /api/library` - Get library info
- `PUT /api/library/{id}` - Update library

