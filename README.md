# Communere Microservices

Here we have a simple blog project that has microservice architecture

There are two microservices called `UserService` and `PostService` that both are completely independent asp.net core web api projects with their own sql server databae, and are implemented on its own Docker Images.

We also have a project called `APIGateway` that works as a gateway to handle requests that are coming from different devices. This gateway is using `ocelot` framework.

For the `Event Bus` system, this project is using `RabbitMQ`.

Although we're using `Generic Repository` and `Generic UnitOfWork` patterns, the architecture of the projects is intended to be very simple for simplicity, and it's better to have [Clean Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture) for each project.

***

## Process

After running the solution, three projects run simultaneously. and we can work with the api getway. Microservices have their own `swagger` so we can test each project independently or through the gateway.

`UserService` project is a producer, and `PostService` project is a consumer. at first we add a `User` by `UserService`, then it produce a message for this purpose. After that, `PostService` consumes this message to add the newly created user into its own database. This process also happens when a user entity gets updated.

***

Some of the database functions are created using `stored procedures` and are placed in migration files, so running update command for db will create stored procedures.