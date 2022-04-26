# University Management System

This is University Management System, my own personal page created to manage the university. On this page, you can manage different universities, faculties, courses, and transactions.
Soon I will add students and instructors models, and many more. This page is similar to the page called USOS which is used around whole Poland's universities.
Those who know usos, know about how unfriendly and difficult a page it is to use, so my plan is to create a better page from the beginning. Of course, this project
focuses on the backend so it won't look like Facebook or Twitter. Page is created for free and it's not created for business purposes!

## Installation

Make sure you have installed [.NET 6.0](https://dotnet.microsoft.com/en-us/download)

Check it using this command in cmd or PowerShell
```bash
dotnet --info
```
Download the project from git repository (Always download version from the main branch, other versions can not be stable!)

Install EntityFramework(recommended version is 6.0.2) in project file using PowerShell
```bash
dotnet add package EntityFramework
```

After installing EF Core type this command to initialize database
```bash
dotnet ef database update
```
After initializing database type run command
```bash
dotnet run
```
Your app is now running, on PowerShell window, you should have displayed URL
## Create Admin User
To create an admin user, with permission to access "Admin Page", first make sure you updated database (more in Installation section)

In Program.cs go to line 64 and in if statement change "false" to "true". Your code should looks like this:
```csharp
// Set this value to true to initialize admin user and Admin/User Roles
if (true)
{
    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        Thread.Sleep(3000);
        await AdminDefaultCreate.Initializer(services);
        await RolesDefaultCreate.Initializer(services);

    }
}
```
Now run the program and in the login page type:

Login: admin@example.com

Password: AdminPassword

Now you have access to admin page

## Policy Settings

You can set to use default policy, or policy based on Json config file named AuthorizationConfig.json.
To add/remove or reset json file go to Admin page and from there to Permission page.

To enable/disable json based policy go to Program.cs and follow the rules:
```csharp
// Chose between: DefaultPolicy or PolicyLoaded from Json Config File
// Comment/Uncomment line below:

AddAuthorizationPolicyFromJson(builder.Services);
//AddAuthorizationPolicy(builder.Services);
```
Default policy is loaded from json file

To add new policy follow this rules:

1. Go to AdminController.cs
2. Add new policy name to _policyArray, make sure you change the size of array
3. Add attributes to method or whole class in controller you want 
```csharp
[Authorize(Policy = "Your Policy name")]
public Task<IActionResult> Foo()
{
    // Controller Code
}
```
4. Add role to policy using admin page, or do it by your self in AuthorizationConfig.Json