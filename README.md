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
