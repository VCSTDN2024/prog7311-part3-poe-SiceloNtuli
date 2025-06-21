# Agri-Energy Connect Platform

The Agri-Energy Connect platform is a web application designed to bridge the gap between the agricultural sector and green energy technology providers. This platform facilitates collaboration, sharing of resources, and innovation in sustainable agriculture and renewable energy.

## Setup Instructions

### Prerequisites
- .NET 9.0 SDK or later
- SQL Server (LocalDB or higher)
- Visual Studio 2022 or later (recommended)

### Database Setup
1. The application uses Entity Framework Core with SQL Server.
2. The connection string is configured in `appsettings.json` to use LocalDB.
3. The database will be automatically created and seeded with sample data when the application starts.

### Building and Running the Application
1. Clone the repository to your local machine.
2. Open the solution file `Prog7311_Part2.sln` in Visual Studio.
3. Build the solution (Ctrl+Shift+B).
4. Run the application (F5 or Ctrl+F5).

### Alternative: Using the Command Line
1. Navigate to the project directory.
2. Run `dotnet build` to build the application.
3. Run `dotnet run` to start the application.

## User Roles and Functionality

### Predefined Users
The application comes with two predefined user accounts for testing:

1. **Employee Account**
   - Username: admin
   - Password: admin123

2. **Farmer Account**
   - Username: farmer1/farmer2/farmer3/farmer4/farmer5
   - Password: farmer123

### Farmer Role
As a farmer, you can:
- Add products to your profile
- View and edit your product listings
- Update your farm information

### Employee Role
As an employee, you can:
- Add new farmer profiles
- View all products from specific farmers
- Filter products by date range, product type, and farmer

## Technologies Used
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Font Awesome

## Architecture and Design Patterns
The application implements:
- MVC architectural pattern
- Repository pattern for data access
- Factory pattern for user creation
- Observer pattern for notifications

Git

## Notes
- This is a prototype application developed for educational purposes.
- For a production environment, additional security measures would be implemented. 