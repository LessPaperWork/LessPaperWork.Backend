using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LessPaper.GuardService.Options
{
    public class AppSettings
    {
        private string test;
        public DatabaseSettings DatabaseSettings { get; set; }

        public string Test
        {
            get { return test; }
            set
            {
                test = value;

            }
        }
    }
}
