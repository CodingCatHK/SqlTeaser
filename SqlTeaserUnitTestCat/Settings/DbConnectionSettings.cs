using System.Collections.Generic;
using System.ComponentModel;

namespace SqlTeaserUnitTestCat.Settings
{
    public class DbConnectionSettings : SettingCat.Base
    {
        private static readonly Dictionary<string, string> _SETTINGS_FILENAME = new Dictionary<string, string>()
        {
            {SettingCat.Environments.DEFAULT.ToString(), "db_connection.json" },
            {SettingCat.Environments.UAT.ToString(), "db_connection.uat.json" },
            {SettingCat.Environments.PRODUCTION.ToString(), "db_connection.prod.json" }
        };
        public override Dictionary<string, string> SettingsFilename {
            get{ return new Dictionary<string, string>(_SETTINGS_FILENAME); }
            protected set { }
        }

        [DefaultValue(null)] public string DatabaseServerNetworkAddress;
        [DefaultValue(null)] public string NameOfProjectDatabase;
        [DefaultValue(null)] public string DatabaseUsername;
        [DefaultValue(null)] public string DatabasePassword;

        public DbConnectionSettings() : base() { }

        public string GetConnectionString(bool isIntegratedSecurity)
        {
            return string.Format(
                @"server={0};database={1};integrated security={2};uid={3};pwd={4};",
                this.DatabaseServerNetworkAddress,
                this.NameOfProjectDatabase,
                isIntegratedSecurity ? "true" : "false",
                this.DatabaseUsername,
                this.DatabasePassword
            );
        }
    }
}