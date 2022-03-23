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
