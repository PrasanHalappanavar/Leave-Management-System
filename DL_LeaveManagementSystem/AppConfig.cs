using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem
{
    public class AppConfig
    {
        private static AppConfig _instance;

        private static readonly object _lock = new object();

        public string ConnectionString { get; private set; }

        private AppConfig(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static AppConfig GetInstance(string connectionString = null)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppConfig(connectionString);
                    }
                }
            }
            return _instance;
        }
    }
}
