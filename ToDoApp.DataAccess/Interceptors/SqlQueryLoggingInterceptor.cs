using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ToDoApp.Infrastructures.Interceptors
{
    // log lại những câu query có time thực thi > 1 giây
    public class SqlQueryLoggingInterceptor : DbCommandInterceptor
    {
        private Stopwatch stopwatch = new Stopwatch();
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            stopwatch.Start();

            using StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sqllog.txt"));

            return base.ReaderExecuting(command, eventData, result);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            stopwatch.Stop();
            var miliseconds = stopwatch.ElapsedMilliseconds;
            if (miliseconds >= 10)
            {
                using StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sqllog.txt"), append: true);
                writer.WriteLine(command.CommandText);
            }
            return base.ReaderExecuted(command, eventData, result);
        }
    }
}
