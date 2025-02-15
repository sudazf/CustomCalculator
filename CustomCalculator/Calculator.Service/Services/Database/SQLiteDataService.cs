using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Calculator.Service.Services.Database
{
    internal class SQLiteDataService : ISQLiteDataService
    {
        private readonly SQLiteDatabase _currentDb = new SQLiteDatabase();
        public DataTable GetPatients()
        {
            var sql = @"select * from patients";
            var result = _currentDb.ExecuteSelect(sql);
            return result;
        }

        public void AddPatient(string name, DateTime birthday, double weight)
        {
            try
            {
                var sql = @"insert into patients (name,birthday,weight)
                    values (:name,:birthday,:weight)";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("name", name),
                    new SQLiteParameter("birthday", birthday),
                    new SQLiteParameter("weight", weight),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
