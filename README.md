# assessment
Assessment Web API Project
================================

Overview:
---------
This project is an ASP.NET Core Web API (.NET 8) for managing user information with MS SQL Server. It demonstrates local caching: user data is retrieved from the database if available, otherwise fetched from a 3rd party public API, saved to the database, and returned to the client.

Features:
---------
- RESTful API endpoints:
  - GET /api/Assessment/{id}: Retrieve a single user record by ID (local or 3rd party API).
  - PUT /api/Assessment/{id}: Update a user record in the database.
- Uses raw SQL queries (no ORM).
- Local caching logic for efficient data retrieval.
- Error handling and clear code comments.
- Secure API key usage via environment variable.

Technologies Used:
------------------
- .NET 8
- ASP.NET Core Web API
- MS SQL Server
- SQL queries via SqlConnection/SqlCommand
- 3rd party API: https://jsonplaceholder.typicode.com/users/{id}
- API key (if required) stored in environment variable: THIRD_PARTY_API_KEY
