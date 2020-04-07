# Dessert Backend

This is the backend powering [Dessert](https://dessert.ovh/) web interface. It
is a Dotnet Core application written in C# that exposes a GraphQL endpoint.

**Summary**

* Installation
  * [Development](#installation-for-development)
  * [Docker](#installation-with-docker)
* [Configuration](#configuration)
* [Tests](#tests)


## Installation for development

First, Make sure that Dotnet Core is installed:

```
dotnet --info
```

**Install the dependencies**

While at the root of the repository, run:

```
dotnet restore
```

**Create and populate the database**

While at the root of the repository, run:

```
dotnet run -p Dessert/ migrate --with-fake
```

If you want to create a database without the fake data, you can ommit
the `--with-fake` option. Fake data is generated
using [Bogus](https://github.com/bchavez/Bogus).

**Run the server**

Before running the server, make sure you have edited
the [configuration](#configuration) file according to your needs.

```
dotnet watch -p Dessert/ run start
```

If you want to have access to the development tools, set
the `ASPNETCORE_ENVIRONMENT=Development` environement variable.

*WARNING: The GraphQL development interfaces will NOT be available if the
server is not launched in development mode.*

The application is now available on the route and port indicated on the
standard outut.

While in development mode, you can access 3 handy routes for GraphQL:

* `/voyager` Is a visual representation of the GraphQL schema
* `/graphiql` Is a friendly GraphQL editor
* `/playground` Is a more advanced version of `/graphiql`


## Installation with Docker

First, build the image. While at the root of the repository run:

```
docker build -t dessert-backend .
```

Then, run a container:

```
docker run \
  --name back \
  -p 127.0.0.1:5000:80 \
  -d dessert-backend
```

The server is now available on port `1337`.


## Configuration

The app can be configured through multiple files in the Dessert/ directory:

- `settings.yaml` always read
- `settings.development.yaml` for developement only
- `settings.production.yaml` for production only

The two environment specific files will override any key present in the `settings.yaml` file.

You can also use the application argument and environment to configure it.

### Database
The `Database` section describes the database connection
The server is compatible with either SQLite or PostgreSQL.
Example for SQLite:

```
Database:
  Type: SQLite
  SQLite:
    Source: 'db.db'
```

Example for Postgres:

```
Database:
  Type: Postgres
  Postgres:
    User: back
    Password: "put_that_in_a_scret_you_foock"
    Database: "dessert-back"
    Host: "myhost.com"
    Port: 5432
```

### AllowedOrigins
The `AllowedOrigins` section should accept an array of url, which will describe the allowed origins for the CORS
Example:

```
AllowedOrigins:
  - "http://localhost:3000"
  - "http://localhost:3001"
```

## Tests

While at the root of the repository, run:

```
dotnet test
```

The tests are located in `Dessert.Tests/`.
