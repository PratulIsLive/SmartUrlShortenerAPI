# Smart URL Shortener

This project started as a backend API built with ASP.NET Core and PostgreSQL and was later extended with a React frontend. Users can register, log in, create short URLs, generate QR codes, and track basic statistics from the dashboard.

The project helped me understand JWT authentication, Entity Framework Core, PostgreSQL, middleware, and how a frontend communicates with a backend API.

## Features

- User registration and login
- JWT authentication with BCrypt password hashing
- Create short URLs with optional custom codes
- Redirect short URLs to original links
- QR code generation and download
- Visit tracking and dashboard analytics
- Search, sorting, and pagination
- Update and delete URLs
- Global exception handling and request logging
- Swagger documentation
- Docker support
- Deployment using Render and NeonDB

## Tech Stack

### Backend
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT
- BCrypt
- QRCoder

### Frontend
- React
- Vite
- Axios

### Deployment
- Render
- NeonDB
- Docker

## API Endpoints

### Authentication
- POST `/api/auth/register`
- POST `/api/auth/login`

### URL Management
- POST `/api/url/shorten`
- GET `/api/url/all`
- GET `/api/url/dashboard`
- GET `/api/url/stats/{shortCode}`
- PUT `/api/url/{shortCode}`
- DELETE `/api/url/{shortCode}`

### QR Code
- GET `/api/url/qrcode/{shortCode}`
- GET `/api/url/qrcode/{shortCode}/download`

### Redirect
- GET `/{shortCode}`

## Running the Project

```bash
dotnet restore
dotnet ef database update
dotnet run
