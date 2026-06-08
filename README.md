# SmartUrlShortenerAPI

Enterprise-grade URL Shortener API built with ASP.NET Core and PostgreSQL.

## Features

- JWT Authentication
- URL Shortening
- Custom Short Codes
- URL Expiration
- QR Code Generation
- QR Code Download
- Dashboard Analytics
- Visit Tracking
- Pagination
- Search
- Sorting
- Global Exception Middleware
- Request Logging Middleware
- PostgreSQL Database
- Swagger Documentation

## Tech Stack

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Swagger
- QRCode Generator

## API Endpoints

### Authentication

- POST /api/auth/register
- POST /api/auth/login

### URL Management

- POST /api/url/shorten
- GET /api/url/all
- GET /api/url/dashboard
- GET /api/url/stats/{shortCode}
- PUT /api/url/{shortCode}
- DELETE /api/url/{shortCode}

### QR Code

- GET /api/url/qrcode/{shortCode}
- GET /api/url/qrcode/{shortCode}/download

## Author

Pratul Kumar
