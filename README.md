# ATM System

A C# console application that simulates an ATM (Automated Teller Machine) system with both customer and administrative functionalities.

## Features

- Customer Operations:
  - Account login with PIN
  - Check balance
  - Withdraw money
  - Deposit money
  - Exit

- Administrative Operations:
  - Create new accounts
  - Delete existing accounts
  - Update account information
  - Search for accounts
  - Exit

## Prerequisites

- .NET 8.0 SDK or later
- MySQL Server 8.0 or later
- Visual Studio 2022 or Visual Studio Code with C# extensions

## Project Structure

## Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone [repository-url]
   cd ATMSystem
   ```

2. **Database Setup**
   - Install MySQL Server if not already installed
   - Open MySQL command line or MySQL Workbench
   - Run the database script:
     ```bash
     mysql -u root -p < database/atm_database.sql
     ```

3. **Configure Connection String**
   - Open `src/ATMSystem/appsettings.json`
   - Update the connection string with your MySQL credentials:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=localhost;Database=atm_system;User=your_username;Password=your_password;"
       }
     }
     ```

4. **Build the Project**
   ```bash
   dotnet build
   ```

## Running the Application

1. **Run the Main Application**
   ```bash
   cd src/ATMSystem
   dotnet run
   ```

2. **Default Admin Account**
   - Login: Javed123
   - PIN: 12345

3. **Default Customer Account**
   - Login: Adnan123
   - PIN: 12345

## Running Tests

1. **Run All Tests**
   ```bash
   dotnet test
   ```

2. **Run Tests with Coverage**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```
   Coverage reports will be generated in the `coverage` directory.

## Development

### Project Structure

- `Models/`: Contains the domain models (Account, etc.)
- `Services/`: Contains business logic (AccountService, etc.)
- `Repositories/`: Contains data access logic (MySqlAccountRepository, etc.)
- `Tests/`: Contains unit tests

### Adding New Features

1. Create appropriate models in the `Models` directory
2. Add business logic in the `Services` directory
3. Implement data access in the `Repositories` directory
4. Write unit tests in the `Tests` directory

### Code Style

The project uses StyleCop for code style enforcement. Make sure to follow the established coding standards:
- Use XML documentation comments for public members
- Follow C# naming conventions
- Keep methods focused and small
- Write unit tests for new features

## Documentation

### API Documentation

The project uses DocFX for API documentation. To generate documentation:

1. Install DocFX:
   ```bash
   dotnet tool install -g docfx
   ```

2. Generate documentation:
   ```bash
   docfx build
   ```

3. View documentation locally:
   ```bash
   docfx serve _site
   ```

### Database Schema

The database consists of a single table:

```sql
CREATE TABLE accounts (
    account_number INT AUTO_INCREMENT PRIMARY KEY,
    holder_name VARCHAR(255) NOT NULL,
    balance DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    status VARCHAR(50) NOT NULL,
    login VARCHAR(50) NOT NULL UNIQUE,
    pin_code VARCHAR(5) NOT NULL
);
```

## Troubleshooting

1. **Database Connection Issues**
   - Verify MySQL service is running
   - Check connection string in appsettings.json
   - Ensure database and user permissions are correct

2. **Build Errors**
   - Ensure .NET 8.0 SDK is installed
   - Run `dotnet restore` to restore packages
   - Check for any missing dependencies

3. **Test Failures**
   - Check database connection
   - Verify test data setup
   - Check for any environment-specific issues

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request


 