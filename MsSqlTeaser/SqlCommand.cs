using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using CodingCat.SqlTeaser;

namespace CodingCat.MsSqlTeaser
{
    public class SqlCommand : SqlTeaser.ISqlCommand
    {
        private StringBuilder _SqlBuilder = new StringBuilder();
        private Dictionary<string, object> _Parameters = new Dictionary<string, object>();

        public string Command { get { return this._SqlBuilder.ToString(); } }

        public void AddParameter(string key, object value)
        {
            if (!this._Parameters.ContainsKey(key)) this._Parameters.Add(key, null);
            this._Parameters[key] = value;
        }

        public ISqlCommand AppendCommand(string statement)
        {
            this._SqlBuilder.Append(statement);
            return this;
        }

        public ISqlCommand AppendFormatCommand(
            string statement,
            params object[] objs
        )
        {
            this._SqlBuilder.AppendFormat(statement, objs);
            return this;
        }

        T ISqlCommand.GenerateSqlCommandInstance<T>()
        {
            var cmd = new System.Data.SqlClient.SqlCommand(this._SqlBuilder.ToString());
            foreach (var key in this._Parameters.Keys)
                cmd.Parameters.AddWithValue(key, this._Parameters[key]);

            return (T)Convert.ChangeType(cmd, typeof(T));
        }

        public P PopParameter<P>(string key)
        {
            if (!this._Parameters.ContainsKey(key)) return default(P);
            var Value = (P)this._Parameters[key];
            this._Parameters.Remove(key);

            return Value;
        }
    }
}
