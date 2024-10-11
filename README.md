# DennerBanx API

## Description

The **DennerBanx API** is a simple banking service implemented with ASP.NET Core 8. The API allows basic banking operations such as creating accounts, making deposits, withdrawals, transfers, and checking balances. The project architecture follows the Clean Architecture pattern, separating responsibilities across different layers.

## Project Structure

The project is organized into multiple projects to ensure separation of concerns and facilitate maintenance and scalability:

- **DennerBanx.API**: ASP.NET Core Web API project responsible for exposing the endpoints and handling HTTP requests.
- **DennerBanx.Application**: Contains the use cases and business logic.
- **DennerBanx.Communication**: Defines the Data Transfer Objects (DTOs) or Json used for communication between layers.
- **DennerBanx.Domain**: Contains the main entities and interfaces of the application.
- **DennerBanx.Infrastructure**: Implements data persistence logic (in this case, using an in-memory repository).

## Endpoints

### Reset state
- **POST** `/reset`
- **Response**: `200 OK`

### Get balance for a non-existent account
- **GET** `/balance?account_id={account_id}`
- **Response**: `404 0`

### Get balance for an existing account
- **GET** `/balance?account_id={account_id}`
- **Response**: `200 {balance}`

### Create account with initial balance (deposit)
- **POST** `/event`
  - Request body:
    ```json
    {
      "type": "deposit",
      "destination": "{account_id}",
      "amount": {amount}
    }
    ```
- **Response**: `201 {"destination": {"id":"{account_id}", "balance":{balance}}}`

### Deposit into an existing account
- **POST** `/event`
  - Request body:
    ```json
    {
      "type": "deposit",
      "destination": "{account_id}",
      "amount": {amount}
    }
    ```
- **Response**: `201 {"destination": {"id":"{account_id}", "balance":{balance}}}`

### Withdraw from an existing account
- **POST** `/event`
  - Request body:
    ```json
    {
      "type": "withdraw",
      "origin": "{account_id}",
      "amount": {amount}
    }
    ```
- **Response**: `201 {"origin": {"id":"{account_id}", "balance":{balance}}}`

### Withdraw from a non-existent account
- **POST** `/event`
  - Request body:
    ```json
    {
      "type": "withdraw",
      "origin": "{account_id}",
      "amount": {amount}
    }
    ```
- **Response**: `404 0`

### Transfer between accounts
- **POST** `/event`
  - Request body:
    ```json
    {
      "type": "transfer",
      "origin": "{origin_account_id}",
      "amount": {amount},
      "destination": "{destination_account_id}"
    }
    ```
- **Response**: `201 {"origin": {"id":"{origin_account_id}", "balance":{balance}}, "destination": {"id":"{destination_account_id}", "balance":{balance}}}`

### Transfer from a non-existent account
- **POST** `/event`
  - Request body:
    ```json
    {
      "type": "transfer",
      "origin": "{origin_account_id}",
      "amount": {amount},
      "destination": "{destination_account_id}"
    }
    ```
- **Response**: `404 0`

## Running the Application

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Cloning the repository

```bash
git clone https://github.com/dennersam/DennerBanx.git
cd DennerBanx/src
