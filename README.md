
## 🏗️ LeverX Course Project - Equipment Rental API

REST API for equipment rental management with booking system, reviews, and role-based authentication.

[![.NET](https://img.shields.io/badge/.NET-9.0-%23512BD4)](https://dotnet.microsoft.com/)
[![MS SQL Server](https://img.shields.io/badge/MS%20SQL%20Server-2022-%23CC2927)](https://www.microsoft.com/sql-server)
[![JWT Auth](https://img.shields.io/badge/Auth-JWT-%23FF6F00)](https://jwt.io/)
[![Swagger](https://img.shields.io/badge/Docs-Swagger-%2385EA2D)](https://swagger.io/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

## 📖 Table of Contents
- [Features](#-features)  
- [Technologies](#-technologies)
- [Run Locally](#-run-locally)
- [API Documentation](#-api-documentation)
- [Contribution Guide](#-contribution-guide)
- [License](#-license)
- [Authors](#-authors)
## 🌟 Features
- **Equipment Management**: Full CRUD operations for equipment and categories
- **Booking System**: Create, cancel, and track equipment rentals
- **Reviews & Ratings**: Users can leave feedback on rented equipment
- **Role-Based Auth**: Separate permissions for admins and regular users  
- **Pagination & Filtering**: Advanced search with pagination and filters
 
## 🛠️ Technologies
- **Backend Framework**: ASP.NET Core 9.0 
- **Database**: MS SQL Server 2022
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger / OpenAPI
- **Containerization**: Docker Compose

## 🚀 Run Locally

1. Clone the project:

```bash
  git clone https://github.com/Old-Butt-Gold/LeverX-course-project
```

2. Go to the project directory:

```bash
  cd LeverX-course-project
```

3. Create and run containers:
```bash
  docker-compose up -d --build
```

4. The application will be available at:
`http://localhost:5000` or `https://localhost:5001`

5. To stop the containers:
```bash
  docker-compose down
```

## 📚 API Documentation (Swagger)

Detailed documentation and interactive testing of each endpoint is available through the Swagger UI.
1. Open your browser and go to:
- `http://localhost:5000/swagger`
- or `https://localhost:5001/swagger`
2. In the `Authorize` tab, enter your JWT token to access the protected methods:
```bash
Bearer <Your_Token>
```
3. Swagger will automatically generate query and response patterns and allow you to run queries directly from your browser.

## 📂 How to Contribute

 To contribute to this project, follow these steps:

1. Fork repository:
2. Create feature branch:
```bash
git checkout -b feature/my-new-feature
```
3. Make changes and test your modifications.
4. Commit changes.  
5. Push to your fork:
```bash
git push origin feature/my-new-feature
```
6. Create Pull Request with:
- Description of changes
- Screenshots if applicable
- Related issue numbers



## 📄 License

This project is licensed under the [MIT License](https://choosealicense.com/licenses/mit/).

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)


## 📧 Authors

- [Andrey Krutsko](https://www.github.com/Old-Butt-Gold)