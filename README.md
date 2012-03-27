# What? #
This a REST application inspired in the excellent example of the book Rest in Practice.

It is based on the Restbucks on .NET sample, but uses Nancy, where the original sample uese WebAPI.

# How? #
To run this sample on your local box follow these steps:

  * Clone the codeplex repository with mercurial
  * Create an empty database in a Sql Server instance
  * Open the Visual Studio solution
  * Edit the connection string named “Restbucks” in the Web.Config (web project) and App.config (test project) files.
  * Run the test in RestBucks.Tests.DataInitializer, named InitializeData; this will create the database schema and populate some data.

If you want to run the full suite of tests, you need to follow these two additional steps:
  * Create another empty database
  * Modify the connection string named “Restbucks_Tests” in the App.Config (test project) to point to the database that you just created.