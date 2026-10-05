<div align="center">

# 🏫 Smart Teacher ERP & Center Management System
### *The Next-Generation, Zero-Hardware Educational Center & Tutor Enterprise Solution*

<p align="center">
  <img src="https://img.shields.io/badge/.NET%2010-512BD4?style=for-the-badge&logo=.net&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Flutter-02569B?style=for-the-badge&logo=flutter&logoColor=white" alt="Flutter" />
  <img src="https://img.shields.io/badge/Architecture-Clean%20%2F%2520Onion-blue?style=for-the-badge" alt="Clean Architecture" />
</p>

*Built with enterprise-grade architecture to eliminate manual data entry, enforce strict financial privacy, and automate hybrid anti-cheat attendance with zero hardware costs.*

</div>

---

## 🎯 Architectural Vision & Objective

Managing private educational centers (سناتر) usually suffers from massive data entry friction, financial leaks, and complex attendance tracking. **Smart Teacher ERP** is engineered from scratch to solve these exact enterprise pain points through robust backend design, strict security policies, and high scalability.

---

## 🏛️ System Architecture (Clean / Onion Architecture)

The solution strictly adheres to **Clean Architecture** principles, maintaining absolute separation of concerns across layers:

```text
 📂 TeacherERP.sln
 ┣━ 📂 TeacherERP.Domain         # Core Entities, Enums, Value Objects, & Factory Methods (Zero Dependencies)
 ┣━ 📂 TeacherERP.Application    # Business Use Cases, CQRS, DTOs, & Repository Interfaces
 ┣━ 📂 TeacherERP.Infrastructure # EF Core 10, SQL Server, ASP.NET Identity, & External Integrations
 ┗━ 📂 TeacherERP.API            # Presentation Layer (Web API Endpoints & Middlewares)

🛡️ Core Features & Technical Highlights
1. 🔒 Advanced Security & Session Control
ASP.NET Core Identity Integration: Uses custom ApplicationUser inheriting from IdentityUser<Guid> to manage students, assistants, and super admins seamlessly.

JWT & Secure Refresh Token Rotation: Features device-informed session tracking, allowing instant token revocation (e.g., forcefully logging out a terminated assistant in real-time).

2. 📍 Hybrid Anti-Cheat Attendance System
Self-Scan via GPS Geofencing: Students scan the center's QR code, and the backend validates their geographic coordinates against the center's latitude/longitude with strict Mock Location (Fake GPS) rejection.

Offline/Alternative Fallbacks: Assistants can scan static student QR cards or log manual entries for emergency cases.

Smart Compensation Flow: Handles student makeup classes via pending requests requiring assistant approval.

Auto-Suspension Engine: Automatically penalizes students after consecutive absences.

3. 💰 Strict Financial Ledger & Privacy
Granular Financial Delegation: Assistants have restricted visibility over funds and debts unless explicit financial delegation (HasFinancialDelegation == true) is granted by the Super Admin.

Flexible Payment Types: Supports per-session, monthly subscriptions, and debt settlements.

Digital Receipts: Manages InstaPay/E-wallet receipts requiring Super Admin approval.

🗄️ Database ER Schema Overview
Managed via Entity Framework Core 10 with optimized Fluent API configurations to prevent multiple cascade path anomalies in SQL Server:

AspNetUsers / ApplicationUser (Unified user base with role traits)

RefreshTokens (Secure session management)

TenantSettings (Geofencing configuration and attendance modes)

Groups (Class structures with recurring schedule days)

GroupAdmins (Many-to-many relationship mapping assistants to strict group boundaries)

Sessions (Auto-generated class instances tracking board images and homework timelines)

AttendanceRecords (Hybrid verification logs)

Transactions (Smart financial ledger)

🚀 Getting Started Locally
Prerequisites
.NET 10 SDK

SQL Server

Git

Installation & Run
Clone the repository:

Bash
git clone [https://github.com/Ahmed-Al-Desouki/TeacherERP.git](https://github.com/Ahmed-Al-Desouki/TeacherERP.git)
cd TeacherERP
Configure your connection string in appsettings.json under TeacherERP.API.

Apply database migrations:

Bash
dotnet ef database update --project TeacherERP.Infrastructure --startup-project TeacherERP.API
Run the API:

Bash
dotnet run --project TeacherERP.API/TeacherERP.API.csproj
👨‍💻 Author
Ahmed Al-Desouki
    
.NET Developer & Software Architect

GitHub Profile
