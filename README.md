# Working with OpenFGA in ASP.NET Core #

This repository is an experiment for working with OpenFGA in .NET. I basically want to develop an end-to-end example for using OpenFGA in .NET and applying it to ASP.NET Core OData Queries, so we can apply fine grained authorizations down to a datasets entity. 

We are building a Task Management application, that allows a `User`, `Team` and `Organization` to manage `TaskItems`. A `User` has a `Role`, such as `Administrator` or `User`, so we can also allow for Role-based Access Control. By using OpenFGA we add a Relationship-based Access Control, which makes it easy to model relations between objects.

As of now the application has a `User`, `Role`, `Organization`, `Team` and `TaskItem`. 

The Model in the FGA Syntax is given as: 

```
model
  schema 1.1

type User

type Role
    relations
        define member: [User]

type Organization
    relations
        define member: [User] or owner
        define owner: [User]

type Team
  relations
    define member: [User]

type TaskItem
    relations
        define can_change_owner: owner
        define can_read: viewer or owner
        define can_share: owner
        define can_write: owner
        define owner: [User, Team#member]
        define viewer: [User, User:*, Organization#member, Team#member]
```

## Getting Started ##

Switch into the `/docker` folder and run:

```
./docker compose up
```

This will start OpenFGA and a Postgres instance, which is going to hold the OpenFGA data.

### Creating the OpenFGA Store ###

To get started, we need to create a Store. This is done using the FGA CLI, which has been added to the repository at `tools/fga_0.2.1_windows_amd64.tar.gz`. 

You can create the Store by running the Powershellscript, which will output the Store ID, Authorization Model ID and the full JSON response:

```
PS > .\createFgaStore.ps1
OpenFGA StoreId:                  01HJ8S5C3R7TKXPSP9N5HTDPTP
OpenFGA AuthorizationModelId:     01HJ8S5C46YMQRCC7Z1MHHJMFR
JSON Response:
{
  "store": {
    "created_at": "2023-12-22T12:49:18.968564Z",
    "id": "01HJ8S5C3R7TKXPSP9N5HTDPTP",
    "name": "Task Management Application",
    "updated_at": "2023-12-22T12:49:18.968564Z"
  },
  "model": {
    "authorization_model_id": "01HJ8S5C46YMQRCC7Z1MHHJMFR"
  }
}
```

We can see the StoreID being written to the Environment variable:

```
PS C:\Users\philipp\source\repos\bytefish\OpenFgaExperiments> $env:OpenFGA__StoreId
01HJ8S5C3R7TKXPSP9N5HTDPTP
PS C:\Users\philipp\source\repos\bytefish\OpenFgaExperiments> $env:OpenFGA__AuthorizationModelId
01HJ8S5C46YMQRCC7Z1MHHJMFR
```

If you want to create the Store without Powershell run:

```
./fga store create --name "Task Management Application" --model "src\Server\RebacExperiments.Server.Api\Resources\task-management-model.fga"
```

Set the `OpenFGA:StoreId` and `OpenFga:AuthorizationModelId` in the `appsettings.json`, or set the Environment Variables `OpenFGA__StoreId` and `OpenFGA__AuthorizationModelId`.