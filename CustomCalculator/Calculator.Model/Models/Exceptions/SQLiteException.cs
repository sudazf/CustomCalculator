using System;

namespace Calculator.Model.Models.Exceptions
{
    public class SQLiteException : Exception
    {
        public Exception Exception { get; }

        public SQLiteException(Exception exception)
        {
            Exception = exception;
        }
    }
}
