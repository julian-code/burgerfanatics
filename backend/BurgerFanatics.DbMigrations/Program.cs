using System.Reflection;

using DbUp;
// DbUp is a library used to create migrations. This will run through all the SQL scripts
// located in ./Scripts and execute them. It also tracks what the schema is, by
// creating a table in our Postgres database called schemaversions. I like this tool
// because it is lightweight and quite easy to use. You're also fully in control compared to
// EF Core migrations tool.
var connectionString =
    args.FirstOrDefault()
    ?? Environment.GetEnvironmentVariable("POSTGRES");

Console.WriteLine(connectionString);

var upgrader =
    DeployChanges.To
        .PostgresqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogToConsole()
        .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif                
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;