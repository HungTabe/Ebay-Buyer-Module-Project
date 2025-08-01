using System;
using System.IO;
using System.Threading.Tasks;

namespace CloneEbay.Services
{
    public class TransactionLogger
    {
        private static readonly string LogFilePath = Path.Combine("wwwroot", "logs", "transaction.log");

        public static async Task LogAsync(string message)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
            using (var writer = new StreamWriter(LogFilePath, true))
            {
                await writer.WriteLineAsync($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
            }
        }
    }
} 