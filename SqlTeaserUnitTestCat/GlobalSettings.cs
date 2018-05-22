using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SqlTeaserUnitTestCat
{
    public class GlobalSettings
    {
        private static object obj = new object();
        private static GlobalSettings _INSTANCE;
        public static GlobalSettings Instance
        {
            get
            {
                lock (obj)
                    if (_INSTANCE == null)
                        _INSTANCE = new GlobalSettings();

                return _INSTANCE;
            }
        }
        
        public Settings.DbConnectionSettings DbConnection { get; private set; }

        private GlobalSettings()
        {
            this.DbConnection = new Settings.DbConnectionSettings();
            this.DbConnection.LoadSettingFromDefaultFiles(@"App_Data\Settings\", SettingCat.FileFormats.JSON);
        }
    }
}