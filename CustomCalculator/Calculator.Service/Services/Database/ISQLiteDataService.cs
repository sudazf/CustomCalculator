using System;
using System.Data;

namespace Calculator.Service.Services.Database
{
    public interface ISQLiteDataService
    {
        DataTable GetPatients();
        void AddPatient(string name, DateTime birthday, double weight);
    }
}
