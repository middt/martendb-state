# Account Opening State Machine with Marten DB

This project demonstrates an account opening process using a state machine implemented with Marten DB, a .NET Transactional Document DB and Event Store on PostgreSQL.

## Project Overview

This application showcases a simple account opening workflow using a state machine approach. It utilizes Marten DB for persistence and event sourcing, allowing for a robust and scalable solution.

### Key Components

1. **AccountOpeningStateMachine**: Manages the state transitions of the account opening process.
2. **Account Model**: Represents the account entity with its properties and status.
3. **API Endpoints**: Provides HTTP endpoints to interact with the account opening process.
4. **Docker Setup**: Includes a Docker Compose file for easy setup of the PostgreSQL database.

## Getting Started

### Prerequisites

- .NET 6.0 or later
- Docker and Docker Compose

### Setup

1. Clone the repository
2. Navigate to the project directory

3. Start the PostgreSQL database:

   ```
   docker-compose up -d
   ```

4. Run the application:
   ```
   dotnet run
   ```

## API Endpoints

The application exposes the following endpoints:

- `POST /account`: Create a new account
- `GET /account/{id}`: Retrieve account details
- `GET /account/{id}/state`: Get the current state of an account
- `GET /account/{id}/event/{eventName}`: Trigger a state transition event (Submit, Approve, Activate)
- `GET /account/{id}/events`: Get the event stream for an account

### Testing API Endpoints

You can use the provided `AccountOpeningApi.http` file to test these endpoints. This file contains pre-configured HTTP requests that you can execute directly from compatible IDEs like Visual Studio Code (with the REST Client extension) or JetBrains Rider.

To use the `AccountOpeningApi.http` file:

1. Open the file in your IDE
2. Ensure your application is running on `http://localhost:5290`
3. Click on the "Send Request" link above each request
4. View the response in the output window

This file includes requests for:

- Creating a new account
- Retrieving account details
- Getting the current state of an account
- Submitting an account for review
- Approving an account
- Activating an account
- Retrieving the event stream for an account

It also demonstrates handling of invalid state transitions.

This allows for quick and easy testing of the API without the need for external tools like Postman.

## Project Structure

- `Program.cs`: Entry point of the application, sets up the web application and Marten DB
- `Services/AccountOpeningStateMachine.cs`: Implements the state machine logic
- `Models/Account.cs`: Defines the Account model
- `docker-compose.yml`: Configures the PostgreSQL database for local development
- `AccountOpeningApi.http`: Contains sample API requests for testing

## Technologies Used

- ASP.NET Core
- Marten DB
- PostgreSQL
- Docker

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is open-source and available under the [MIT License](LICENSE).
