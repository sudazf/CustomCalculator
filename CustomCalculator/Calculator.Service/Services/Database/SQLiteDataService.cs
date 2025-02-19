using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Xml.Linq;
using Calculator.Model.Models;

namespace Calculator.Service.Services.Database
{
    internal class SQLiteDataService : ISQLiteDataService
    {
        private readonly SQLiteDatabase _currentDb = new SQLiteDatabase();
        public DataTable GetPatients()
        {
            try
            {
                var sql = @"select * from patients";
                var result = _currentDb.ExecuteSelect(sql);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AddPatient(string id, string name, DateTime birthday, double weight)
        {
            try
            {
                var sql = @"insert into patients (id,name,birthday,weight)
                    values (:id,:name,:birthday,:weight)";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", id),
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
        public void UpdatePatientInfo(string patientId, string patientName, DateTime patientBirthday, double patientWeight)
        {
            try
            {
                var sql = @"update patients
                    set name=:name, birthday=:birthday, weight=:weight, 
                        update_time=datetime(CURRENT_TIMESTAMP, 'localtime')
                    where id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("name", patientName),
                    new SQLiteParameter("birthday", patientBirthday),
                    new SQLiteParameter("weight", patientWeight),
                    new SQLiteParameter("patientId", patientId),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void DeletePatient(string id)
        {
            throw new NotImplementedException();
        }

        public DataTable GetPatientVariables(string id)
        {
            try
            {
                var sql = @"select * from patients_variables where patient_id=:id";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", id),
                };
                var result = _currentDb.ExecuteSelect(sql, paras);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void DeletePatientVariables(string patientId)
        {
            try
            {
                var sql = @"delete from patients_variables
                where patient_id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patientId", patientId),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void InsertPatientVariable(string patientId, Variable variable)
        {
            try
            {
                var sql = @"insert into patients_variables 
                    (id,patient_id,variable_name,variable_value,
                    variable_min,variable_max,variable_unit,variable_expression)
                    values (:id, :patient_id, :variable_name, :variable_value,
                    :variable_min, :variable_max, :variable_unit, :variable_expression)";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", variable.Id),
                    new SQLiteParameter("patient_id", patientId),
                    new SQLiteParameter("variable_name", variable.Name),
                    new SQLiteParameter("variable_value", variable.Value),
                    new SQLiteParameter("variable_min", variable.Min),
                    new SQLiteParameter("variable_max", variable.Max),
                    new SQLiteParameter("variable_unit", variable.Unit),
                    new SQLiteParameter("variable_expression", variable.Formula.MetaExpression)
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
