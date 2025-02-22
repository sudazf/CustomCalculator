using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography;
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
                var sql = @"select * from patients
                    WHERE update_time >= date('now', '-6 months');";
                var result = _currentDb.ExecuteSelect(sql);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatients(string patientName)
        {
            try
            {
                var searchKeyword = $"%{patientName}%";
                var sql = @"SELECT * from patients t 
                    WHERE t.name LIKE :searchKeyword limit 0,5";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter(":searchKeyword", searchKeyword),
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

        public void AddPatient(string id, string bedNumber, string name, DateTime birthday, double weight)
        {
            try
            {
                var sql = @"insert into patients (id, bed_number,name,birthday,weight)
                    values (:id,:bed_number,:name,:birthday,:weight)";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", id),
                    new SQLiteParameter("bed_number", bedNumber),
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
        public void UpdatePatientInfo(string patientId, string bedNumber, string patientName, 
            DateTime patientBirthday, double patientWeight, string diagnosis)
        {
            try
            {
                var sql = @"update patients
                    set bed_number=:bedNumber, name=:name, birthday=:birthday, weight=:weight, diagnosis=:diagnosis,
                        update_time=datetime(CURRENT_TIMESTAMP, 'localtime')
                    where id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("bedNumber", bedNumber),
                    new SQLiteParameter("name", patientName),
                    new SQLiteParameter("birthday", patientBirthday),
                    new SQLiteParameter("weight", patientWeight),
                    new SQLiteParameter("patientId", patientId),
                    new SQLiteParameter("diagnosis", diagnosis),
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
            try
            {
                var sql = @"delete from patients
                    where id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patientId", id),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatientDays(string id)
        {
            try
            {
                var sql = @"select * from patients_variables 
                            where patient_id=:patient_id
                            Order by create_day";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patient_id", id),
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

        public void DeletePatientDailyVariables(string patientId, string day)
        {
            try
            {
                var sql = @"delete from patients_variables
                where patient_id=:patientId AND create_day=:day";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patientId", patientId),
                    new SQLiteParameter("day", day),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void InsertPatientDailyVariable(string patientId, string day, Variable variable)
        {
            try
            {
                var sql = @"insert into patients_variables 
                    (id,isChecked,patient_id,variable_name,variable_value,
                    variable_min,variable_max,variable_unit,variable_expression,create_day)
                    values (:id, :isChecked, :patient_id, :variable_name, :variable_value,
                    :variable_min, :variable_max, :variable_unit, :variable_expression, :day)";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", variable.Id),
                    new SQLiteParameter("isChecked", variable.IsChecked ? 1 : 0),
                    new SQLiteParameter("patient_id", patientId),
                    new SQLiteParameter("variable_name", variable.Name),
                    new SQLiteParameter("variable_value", variable.Value),
                    new SQLiteParameter("variable_min", variable.Min),
                    new SQLiteParameter("variable_max", variable.Max),
                    new SQLiteParameter("variable_unit", variable.Unit),
                    new SQLiteParameter("variable_expression", variable.Formula.MetaExpression),
                    new SQLiteParameter("day", day)
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void InsertVariableTemplate(VariableTemplate template)
        {
            try
            {
                var sql = @"insert into template_variables 
                    (id,template_name,isChecked,variable_name,variable_value,
                    variable_min,variable_max,variable_unit,variable_expression)
                    values (:id, :template_name, :isChecked, :variable_name, :variable_value,
                    :variable_min, :variable_max, :variable_unit, :variable_expression)";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", template.Id),
                    new SQLiteParameter("template_name", template.Name),
                    new SQLiteParameter("isChecked", template.Variable.IsChecked),
                    new SQLiteParameter("variable_name", template.Variable.Name),
                    new SQLiteParameter("variable_value",  template.Variable.Value),
                    new SQLiteParameter("variable_min",  template.Variable.Min),
                    new SQLiteParameter("variable_max",  template.Variable.Max),
                    new SQLiteParameter("variable_unit",  template.Variable.Unit),
                    new SQLiteParameter("variable_expression",  template.Variable.Formula.MetaExpression)
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetTemplateNames()
        {
            try
            {
                var sql = @"select distinct(template_name) template_name 
                        from template_variables t;";
                var result = _currentDb.ExecuteSelect(sql);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetVariableTemplates(string templateName)
        {
            try
            {
                var sql = @"select * from template_variables 
                            where template_name=:templateName";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("templateName", templateName),
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


        public void DeletePatientDays(string id)
        {
            try
            {
                var sql = @"delete from patients_variables
                    where patient_id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patientId", id),
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
