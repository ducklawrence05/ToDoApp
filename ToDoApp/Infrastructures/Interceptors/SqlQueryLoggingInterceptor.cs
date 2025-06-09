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
            return base.ReaderExecuting(command, eventData, result);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            stopwatch.Stop();
            var miliseconds = stopwatch.ElapsedMilliseconds;
            if (miliseconds >= 10)
            {
                using StreamWriter writer = new StreamWriter("C:\\0LamViec\\ELCA_6months\\ASP.NET\\ToDoApp\\ToDoApp\\sqllog.txt", append: true);
                writer.WriteLine(command.CommandText);
            }
            return base.ReaderExecuted(command, eventData, result);
        }
    }
}
