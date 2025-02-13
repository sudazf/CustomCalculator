using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Calculator.Service.Services.Database
{
    internal class SQLiteDataService : ISQLiteDataService
    {
        private readonly SQLiteDatabase _currentDb = new SQLiteDatabase();
        public DataTable GetUserData()
        {
            var sql = @"select distinct Category,Type,Name,TypeSerialNum from r_pulse_testPartsList_t ";
            var result = _currentDb.ExecuteSelect(sql);
            return result;
        }

        public void InsertUserData(string category, string type, string name, int serialnum, string pulseName)
        {
            var sql = @"insert into r_pulse_testPartsList_t (PulseName,Category,Type,Name,TypeSerialNum)
                                  values (:pulsename,:category,:type,:name,:serialnum)";
            var paras = new List<SQLiteParameter> { 
                new SQLiteParameter("category", category),
                new SQLiteParameter("type", type), 
                new SQLiteParameter("pulsename", pulseName),
                new SQLiteParameter("name",name),
                new SQLiteParameter("serialnum",serialnum) };

            _currentDb.ExecuteNonQuery(sql, paras);
        }
    }
}
