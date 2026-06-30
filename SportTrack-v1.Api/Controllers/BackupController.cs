using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class BackupController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BackupController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadBackup()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, "Connection string not found.");
            }

            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                var host = builder.Host;
                var port = builder.Port > 0 ? builder.Port : 5432;
                var database = builder.Database;
                var username = builder.Username;
                var password = builder.Password;

                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                
                // Si es Windows, asumimos que puede estar en las rutas habituales de PostgreSQL o en el PATH
                // En Linux (Render), al haber instalado postgresql-client, pg_dump estará en el PATH
                var pgDumpCommand = isWindows ? "pg_dump" : "pg_dump";
                
                if (isWindows)
                {
                    // Intentar buscar en C:\Program Files\PostgreSQL si no se encuentra en el PATH
                    var defaultPgPath = @"C:\Program Files\PostgreSQL\16\bin\pg_dump.exe"; // Versión común, puede ser 15, 16, 17...
                    if (!System.IO.File.Exists(defaultPgPath))
                    {
                        defaultPgPath = @"C:\Program Files\PostgreSQL\15\bin\pg_dump.exe";
                    }
                    if (!System.IO.File.Exists(defaultPgPath))
                    {
                        defaultPgPath = @"C:\Program Files\PostgreSQL\14\bin\pg_dump.exe";
                    }
                    
                    if (System.IO.File.Exists(defaultPgPath))
                    {
                        pgDumpCommand = defaultPgPath;
                    }
                }

                var arguments = $"--host={host} --port={port} --username={username} --format=custom --no-owner --no-privileges {database}";
                // Para simplificar la salida y que sea texto plano SQL en lugar de 'custom', cambiamos a SQL plano
                arguments = $"--host={host} --port={port} --username={username} --format=plain --no-owner --no-privileges --clean --if-exists {database}";

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = pgDumpCommand,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Pasar la contraseña por variable de entorno es la forma segura en PostgreSQL
                processStartInfo.EnvironmentVariables["PGPASSWORD"] = password;

                var process = new Process { StartInfo = processStartInfo };
                
                process.Start();

                var memoryStream = new MemoryStream();
                await process.StandardOutput.BaseStream.CopyToAsync(memoryStream);
                var errorOutput = await process.StandardError.ReadToEndAsync();
                
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    return StatusCode(500, $"Error executing pg_dump: {errorOutput}");
                }

                memoryStream.Position = 0;

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"backup_{database}_{timestamp}.sql";

                return File(memoryStream, "application/sql", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while generating backup: {ex.Message}");
            }
        }
    }
}
