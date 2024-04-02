# RabbitMQ Client-Server Project

This project demonstrates a client-server architecture using RabbitMQ messaging middleware, with two ASP.NET Web API applications.

## Table of Contents

1. [Overview](#overview)
2. [Usage](#usage)
3. [Integration](#integration)
4. [License](#license)

## Overview

This project consists of two ASP.NET Web API applications:

- **Listener**: Sends requests to the server application via RabbitMQ (Client).
- **Consumer**: Listens for requests from the client, processes them, and sends responses back (Server).
- **RabbitMQLib**: A library for easier use of RabbitMQ.
- **CommonLibrary**: Provides common services or classes across assemblies.


### Prerequisites

- [.NET Core 8 SDK](https://dotnet.microsoft.com/download)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [RabbitMQ](https://www.rabbitmq.com/download.html)

**Note:** RabbitMQ must be installed locally or running from Docker. Both client and server applications use `localhost` as the connection string by default.

### Setup

1. Clone the repository:

    ```bash
    $ git clone https://github.com/AlexKons9/RabbitMQLib.git
    ```

2. Navigate to the respective directories:

    ```bash
    $ cd RabbitMQLib/Consumer.WebApi
    $ cd RabbitMQLib/RabbitMQLib
    ```

3. Install dependencies:

    ```bash
    $ dotnet restore
    ```

4. Update RabbitMQ connection settings in `appsettings.json` files for both the client and server applications.

## Usage

1. Set multiple startup projects: `Consumer.WebApi` and `Listener.WebApi`.

2. Use your preferred API testing tool (e.g., Postman) to send requests to the client API, which will forward them to the server API via RabbitMQ.

3. **Description:** This project sends a `Person` object to the server and returns an edited name.

    - **HTTP Method:** POST
    - **Endpoint:** `https://localhost:7074/api/Some`
    - **Payload JSON:**
      ```json
      {
        "serviceName": "SomeService",
        "methodName": "EditPerson",
        "parameters": [
          {
            "name": "person",
            "value": {
              "Id": 1,
              "Name": "John Doe",
              "Description": "Some description",
              "Title": "Some title",
              "Author": "Some author"
            }
          }
        ],
        "hasError": false,
        "errorDescription": null
      }
      ``` 

## License

This project is licensed under the [MIT License](LICENSE).
