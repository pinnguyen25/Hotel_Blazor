# 🏨 ChillZone - Hotel Booking Management System

> **ChillZone** is a comprehensive, full-stack hotel booking platform built with **.NET 9**, **Blazor Server**, and **Clean Architecture**. It connects Hotel Owners, Customers, and Administrators in a seamless, real-time ecosystem.

![.NET](https://img.shields.io/badge/.NET%209-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Docker](https://img.shields.io/badge/docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![MudBlazor](https://img.shields.io/badge/MudBlazor-7E6EEF?style=for-the-badge&logo=mudblazor&logoColor=white)

## � Tech Stack

*  **Framework:** ASP.NET Core 9
*  **Frontend:** Blazor Server
*  **Architecture:** Clean Architecture
*  **Database:** SQL Server 2019+
*  **ORM:** Entity Framework Core
*  **Cloud Storage:** Cloudinary
*  **Containerization:** Docker


## 🌟 Key Features

### 👤 Customer
* **Smart Search:** Filter hotels by location, amenities, and price.
* **Booking System:** Seamless room reservation with date validation.
* **Wallet & Payments:** Integrated digital wallet for secure transactions.
* **Reviews:** Rate hotels and share experiences.

### 🏨 Hotel Owner
* **Property Management:** specialized dashboard to manage hotels, rooms, and images.
* **Booking Oversight:** Approve or reject incoming booking requests.
* **Revenue Tracking:** Monitor earnings and request withdrawals.
* **Promotions:** Manage events and discount codes.

### 🛡️ Admin
* **Platform Control:** Manage users, verify hotels, and moderate reviews.
* **Analytics:** View system-wide statistics and transaction flows.

## 🛠️ How to Run (Local Development)

### Prerequisites
*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop)
*   [Azure Data Studio](https://azure.microsoft.com/en-us/products/data-studio/)

### 1. Database Setup (Docker & Script)
Instead of Code First migrations, this project uses a specific SQL script initialization.

1.  **Start SQL Server** via Docker:
    ```bash
    docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=db@HotelBooking03" -p 1434:1433 -d mcr.microsoft.com/mssql/server:2019-latest
    ```

2.  **Initialize Database**:
    *   Open **Azure Data Studio**.
    *   Connect to `localhost,1434` (User: `sa`, Pass: `db@HotelBooking03`).
    *   Open the project's SQL script (e.g., `HotelBookingDb.sql`) and **Execute** it to create the schema and seed data.

### 2. Configure Application
Ensure `HotelBooking.infrastructure/appsettings.json` matches your Docker container:
```json
"ConnectionStrings": {
  "connectionStringHotelBooking": "Server=127.0.0.1,1434;Database=ManageHotel;User Id=sa;Password=db@HotelBooking03;TrustServerCertificate=True"
}
```

### 3. Run the Application
```bash
# Restore dependencies
dotnet restore

# Run the Blazor Web App
cd HotelBooking.webapp
dotnet watch run
```
The application will launch typically at `https://localhost:7000` or `http://localhost:5000`.

## 📸 Screenshots

## 📞 Contact

**Nguyễn Ngọc Huỳnh** - Full Stack .NET Developer
*   [LinkedIn](https://linkedin.com)
*   [GitHub](https://github.com)
*   Email: huynhnguyen.250603@gmail.com

