using System.Data;

namespace Calculator.Service.Services.Database
{
    public interface ISQLiteDataService
    {
        DataTable GetUserData();
        void InsertUserData(string category, string type, string name, int serialNum, string pulseName);
    }
}
