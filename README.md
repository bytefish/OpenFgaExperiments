# Experimenting with Relationship-based Access Control and ASP.NET Core OData #

[written an article about the Google Zanzibar Data Model]: https://www.bytefish.de/blog/relationship_based_acl_with_google_zanzibar.html

The Google Drive app starts and a moment later *your files* appear. It's magic. But have you 
ever wondered what's *your files* actually? How do these services actually know, which files 
*you are allowed* to see?

Are you part of an *Organization* and you are allowed to *view* all their files? Have you been 
assigned to a *Team*, that's allowed to *view* or *edit* files? Has someone shared *their files* 
with *you* as a *User*?

So in 2019 Google has lifted the curtain and has published a paper on *Google Zanzibar*, which 
is Google's central solution for providing authorization among its many services:

* [https://research.google/pubs/pub48190/](https://research.google/pubs/pub48190/)

The keyword here is *Relationship-based Access Control*, which is ...

> [...] an authorization paradigm where a subject's permission to access a resource is defined by the 
> presence of relationships between those subjects and resources.

I have previously [written an article about the Google Zanzibar Data Model], and also wrote some 
pretty nice SQL statements to make sense of the it. This repository implements Relationship-based 
Access Control using ASP.NET Core, EntityFramework Core and Microsoft SQL Server.

## About this Repository ##

This repository is an OData-enabled implementation of the RESTful API implementing ReBAC in ASP.NET Core:

* [https://www.bytefish.de/blog/aspnetcore_rebac.html](https://www.bytefish.de/blog/aspnetcore_rebac.html)

The blog article for this repository can be found at:

* [https://www.bytefish.de/blog/aspnetcore_rebac_odata.html](https://www.bytefish.de/blog/aspnetcore_rebac_odata.html)

## OData API Example using a .http File ##

We got everything in place. We can now start the application and use Swagger to query it. But Visual Studio 2022 
now comes with the "Endpoints Explorer" to execute HTTP Requests against HTTP endpoints. Though it's not fully-fledged 
yet, I think it'll improve with time and it already covers a lot of use cases.

You can find the Endpoints Explorer at:

* `View -> Other Windows -> Endpoints Explorer`

By clicking on `RebacExperiments.Server.Api.http` the HTTP script with the sample requests comes up.

### The Example Setup ###

We have got 2 Tasks:

* `task_152`: "Sign Document"
* `task 323`: "Call Back Philipp Wagner"

And we have got two users: 

* `user_philipp`: "Philipp Wagner"
* `user_max`: "Max Mustermann"

Both users are permitted to login, so they are allowed to query for data, given a permitted role and permissions.

There are two Organizations:

* Organization 1: "Organization #1"
* Organization 2: "Organization #2"

And 2 Roles:

* `role_user`: "User" (Allowed to Query for UserTasks)
* `role_admin`: "Administrator" (Allowed to Delete a UserTask)

The Relationships between the entities are the following:

```
The Relationship-Table is given below.

ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
--------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
:task_323  :        |   UserTask        |       viewer      |   :organization_1:    |       Organization    |   member
:task_152  :        |   UserTask        |       viewer      |   :organization_1:    |       Organization    |   member
:task_152  :        |   UserTask        |       viewer      |   :organization_2:    |       Organization    |   member
:organization_1:    |   Organization    |       member      |   :user_philipp:      |       User            |   NULL
:organization_2:    |   Organization    |       member      |   :user_max:          |       User            |   NULL
:role_user:         |   Role            |       member      |   :user_philipp:      |       User            |   NULL
:role_admin:        |   Role            |       member      |   :user_philipp:      |       User            |   NULL
:role_user:         |   Role            |       member      |   :user_max:          |       User            |   NULL
:task_323:          |   UserTask        |       owner       |   :user_2:            |       User            |   member
```

We can draw the following conclusions here: A `member` of `organization_1` is `viewer` of `task_152` and `task_323`. A `member` 
of `organization_2` is a `viewer` of `task_152` only. `user_philipp` is member of `organization_1`, so the user is able to see 
both tasks as `viewer`. `user_max` is member of `organization_2`, so he is a `viewer` of `task_152` only. `user_philipp` has the 
`User` and `Administrator` roles assigned, so he can create, query and delete a `UserTask`. `user_max` only has the `User` role 
assigned, so he is not authorized to delete a `UserTask`. Finally `user_philipp` is also the `owner` of `task_323` so he is 
permitted to update the data of the `UserTask`.

### HTTP Endpoints Explorer Script ###

We start by defining the Host Address:

```
@RebacExperiments.Server.Api_HostAddress = https://localhost:5000/odata
```

Then we signin `philipp@bytefish.de` using the `SignInUser` Action:

```
### Sign In "philipp@bytefish.de"

POST {{RebacExperiments.Server.Api_HostAddress}}/SignInUser
Content-Type: application/json

{
  "username": "philipp@bytefish.de",
  "password": "5!F25GbKwU3P",
  "rememberMe": true
}
```

And then we get all `UserTask` entities for the current user:

```
### Get all UserTasks for "philipp@bytefish.de"

GET {{RebacExperiments.Server.Api_HostAddress}}/UserTasks
```

The response is going to contain two entities:

```json
{
  "@odata.context": "https://localhost:5000/odata/$metadata#UserTasks",
  "value": [
    {
      "title": "Call Back",
      "description": "Call Back Philipp Wagner",
      "dueDateTime": null,
      "reminderDateTime": null,
      "completedDateTime": null,
      "assignedTo": null,
      "userTaskPriority": "Low",
      "userTaskStatus": "NotStarted",
      "id": 152,
      "rowVersion": "AAAAAAAAB\u002Bw=",
      "lastEditedBy": 1,
      "validFrom": "2013-01-01T00:00:00\u002B01:00",
      "validTo": "9999-12-31T23:59:59.9999999\u002B01:00"
    },
    {
      "title": "Sign Document",
      "description": "You need to Sign a Document",
      "dueDateTime": null,
      "reminderDateTime": null,
      "completedDateTime": null,
      "assignedTo": null,
      "userTaskPriority": "Normal",
      "userTaskStatus": "InProgress",
      "id": 323,
      "rowVersion": "AAAAAAAAB\u002B0=",
      "lastEditedBy": 1,
      "validFrom": "2013-01-01T00:00:00\u002B01:00",
      "validTo": "9999-12-31T23:59:59.9999999\u002B01:00"
    }
  ]
}
```

We can then introduce some OData Goodies and say we want only `1` entity, the results should be 
ordered by the `id` property and the response should contain the total number of entities the user 
is authorized to acces.

```
### Get the first task and return the total count of Entities visible to "philipp@bytefish.de"

GET {{RebacExperiments.Server.Api_HostAddress}}/UserTasks?$top=1&$orderby=id&$count=true
```

The result is going to contain the `@odata.count` property and have `1` task only.

```
{
  "@odata.context": "https://localhost:5000/odata/$metadata#UserTasks",
  "@odata.count": 2,
  "value": [
    {
      "title": "Call Back",
      "description": "Call Back Philipp Wagner",
      "dueDateTime": null,
      "reminderDateTime": null,
      "completedDateTime": null,
      "assignedTo": null,
      "userTaskPriority": "Low",
      "userTaskStatus": "NotStarted",
      "id": 152,
      "rowVersion": "AAAAAAAAB\u002Bw=",
      "lastEditedBy": 1,
      "validFrom": "2013-01-01T00:00:00\u002B01:00",
      "validTo": "9999-12-31T23:59:59.9999999\u002B01:00"
    }
  ]
}
```

We can then sign in as `max@mustermann.local`.

```
### Sign In as "max@mustermann.local"

POST {{RebacExperiments.Server.Api_HostAddress}}/SignInUser
Content-Type: application/json

{
  "username": "max@mustermann.local",
  "password": "5!F25GbKwU3P",
  "rememberMe": true
}
```

If you try to get all `UserTask` entities of `max@mustermann.local`:

```
### Get all UserTasks for "max@mustermann.local"

GET {{RebacExperiments.Server.Api_HostAddress}}/UserTasks
```

There will be `1` task only.

```json
{
  "@odata.context": "https://localhost:5000/odata/$metadata#UserTasks",
  "value": [
    {
      "title": "Call Back",
      "description": "Call Back Philipp Wagner",
      "dueDateTime": null,
      "reminderDateTime": null,
      "completedDateTime": null,
      "assignedTo": null,
      "userTaskPriority": "Low",
      "userTaskStatus": "NotStarted",
      "id": 152,
      "rowVersion": "AAAAAAAAB\u002Bw=",
      "lastEditedBy": 1,
      "validFrom": "2013-01-01T00:00:00\u002B01:00",
      "validTo": "9999-12-31T23:59:59.9999999\u002B01:00"
    }
  ]
}
```

Now we'll create a new `UserTask` "API HTTP File Example":

```
### Create a new UserTask "API HTTP File Example" as "max@mustermann.local"

POST {{RebacExperiments.Server.Api_HostAddress}}/UserTasks
Content-Type: application/json

{
    "title": "API HTTP File Example",
    "description": "API HTTP File Example",
    "dueDateTime": null,
    "reminderDateTime": null,
    "completedDateTime": null,
    "assignedTo": null,
    "userTaskPriority": "Normal",
    "userTaskStatus": "NotStarted"
}
```

And we can see, that `max@mustermann.local` now sees both `UserTask` entities:

```json
{
  "@odata.context": "https://localhost:5000/odata/$metadata#UserTasks",
  "value": [
    {
      "title": "Call Back",
      "description": "Call Back Philipp Wagner",
      "dueDateTime": null,
      "reminderDateTime": null,
      "completedDateTime": null,
      "assignedTo": null,
      "userTaskPriority": "Low",
      "userTaskStatus": "NotStarted",
      "id": 152,
      "rowVersion": "AAAAAAAAB\u002Bw=",
      "lastEditedBy": 1,
      "validFrom": "2013-01-01T00:00:00\u002B01:00",
      "validTo": "9999-12-31T23:59:59.9999999\u002B01:00"
    },
    {
      "title": "API HTTP File Example",
      "description": "API HTTP File Example",
      "dueDateTime": null,
      "reminderDateTime": null,
      "completedDateTime": null,
      "assignedTo": null,
      "userTaskPriority": "Normal",
      "userTaskStatus": "NotStarted",
      "id": 38191,
      "rowVersion": "AAAAAAAACAY=",
      "lastEditedBy": 7,
      "validFrom": "2023-10-25T19:58:44.8007138\u002B02:00",
      "validTo": "9999-12-31T23:59:59.9999999\u002B01:00"
    }
  ]
}
```

If we sign in as `philipp@bytefish.de` again:

```
### Sign In "philipp@bytefish.de"

POST {{RebacExperiments.Server.Api_HostAddress}}/SignInUser
Content-Type: application/json

{
  "username": "philipp@bytefish.de",
  "password": "5!F25GbKwU3P",
  "rememberMe": true
}
```

We can see with a call to `/UserTasks`, that he doesn't see the new `UserTask` at all.

```json
{
  "@odata.context": "https://localhost:5000/odata/$metadata#UserTasks",
  "value": [
    {
      "title": "Call Back",
      "description": "Call Back Philipp Wagner",
      "dueDateTime": null,
      "reminderDateTime": null,
      "completedDateTime": null,
      "assignedTo": null,
      "userTaskPriority": "Low",
      "userTaskStatus": "NotStarted",
      "id": 152,
      "rowVersion": "AAAAAAAAB\u002Bw=",
      "lastEditedBy": 1,
      "validFrom": "2013-01-01T00:00:00\u002B01:00",
      "validTo": "9999-12-31T23:59:59.9999999\u002B01:00"
    },
    {
      "title": "Sign Document",
      "description": "You need to Sign a Document",
      "dueDateTime": null,
      "reminderDateTime": null,
      "completedDateTime": null,
      "assignedTo": null,
      "userTaskPriority": "Normal",
      "userTaskStatus": "InProgress",
      "id": 323,
      "rowVersion": "AAAAAAAAB\u002B0=",
      "lastEditedBy": 1,
      "validFrom": "2013-01-01T00:00:00\u002B01:00",
      "validTo": "9999-12-31T23:59:59.9999999\u002B01:00"
    }
  ]
}
```


## Further Reading ##

* [Exploring Relationship-based Access Control (ReBAC) with Google Zanzibar](https://www.bytefish.de/blog/relationship_based_acl_with_google_zanzibar.html)