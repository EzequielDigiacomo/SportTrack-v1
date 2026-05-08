using System;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true);

var configuration = builder.Build();
var connectionString = configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"Using Connection String: {connectionString}");

var optionsBuilder = new DbContextOptionsBuilder<SportTrackDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using (var context = new SportTrackDbContext(optionsBuilder.Options))
{
    try 
    {
        Console.WriteLine("Manually adding columns if they don't exist...");
        context.Database.ExecuteSqlRaw(@"
            ALTER TABLE regatas.""Eventos"" ADD COLUMN IF NOT EXISTS ""BotesHabilitados"" text;
            ALTER TABLE regatas.""Eventos"" ADD COLUMN IF NOT EXISTS ""CategoriasHabilitadas"" text;
            ALTER TABLE regatas.""Eventos"" ADD COLUMN IF NOT EXISTS ""DistanciasHabilitadas"" text;
        ");
        Console.WriteLine("Columns added successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
