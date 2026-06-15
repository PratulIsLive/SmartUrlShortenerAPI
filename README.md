# Smart URL Shortener API

A URL Shortener API built using ASP.NET Core and PostgreSQL. This project allows users to create short URLs, generate QR codes, track visits, and manage their links securely using JWT authentication.

## Features

* JWT Authentication
* Password Hashing with BCrypt
* URL Shortening
* Custom Short Codes
* URL Expiration
* Redirect to Original URL
* QR Code Generation
* QR Code Download
* Dashboard Analytics
* Visit Tracking
* Pagination
* Search and Sorting
* Update and Delete URLs
* Global Exception Handling Middleware
* Request Logging Middleware
* PostgreSQL Integration
* Swagger Documentation
* Docker Support
* Deployment on Render with NeonDB

## Tech Stack

* ASP.NET Core (.NET 10)
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* BCrypt
* QRCoder
* Swagger
* Docker
* Render
* NeonDB

## Main Components

**Controllers**

* AuthController
* UrlController
* RedirectController

**Services**

* JwtService
* UserService
* UrlStorageService
* ShortCodeGeneratorService
* QrCodeService

**Other Folders**

* Models
* DTOs
* Data
* Middleware
* Migrations

## API Endpoints

### Authentication

* POST `/api/auth/register`
* POST `/api/auth/login`

### URL Management

* POST `/api/url/shorten`
* GET `/api/url/all`
* GET `/api/url/dashboard`
* GET `/api/url/stats/{shortCode}`
* PUT `/api/url/{shortCode}`
* DELETE `/api/url/{shortCode}`

### QR Code

* GET `/api/url/qrcode/{shortCode}`
* GET `/api/url/qrcode/{shortCode}/download`

### Redirect

* GET `/{shortCode}`

## How It Works

* Controllers receive requests from clients.
* Services contain the business logic.
* Entity Framework Core communicates with PostgreSQL.
* NeonDB stores all data.
* Render hosts the application in the cloud.

## Running the Project

Clone the repository:

```bash
git clone <repository-url>
```

Restore packages:

```bash
dotnet restore
```

Apply migrations:

```bash
dotnet ef database update
```

Run the application:

```bash
dotnet run
```

Open Swagger:

```text
http://localhost:5004/swagger
```

## Future Improvements

* Unit Testing with xUnit
* CI/CD using GitHub Actions
* Bitly-like Frontend
* Redis Caching
* Rate Limiting 

## Author

Pratul Kumar
