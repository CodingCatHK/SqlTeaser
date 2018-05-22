using System;
using System.Collections.Generic;
using System.Text;
using CodingCat.SqlTeaser;

namespace CodingCat.MsSqlTeaser
{
    public class DbRow : SqlTeaser.IDbRow
    {
        public IDbTable OwnerTable { get; private set; }
        private System.Data.DataRow DataRow { get; set; }

        public object this[string columnName]
        {
            get { return this.DataRow[columnName]; }
            set { this.DataRow[columnName] = value; }
        }

        #region Constructor(s)
        public DbRow(IDbTable ownerTable, System.Data.DataRow datarow)
        {
            this.OwnerTable = ownerTable;
            this.DataRow = datarow;
        }
        #endregion

        public IDbRow AttachValue(string key, object value)
        {
            if (this.OwnerTable.IsColumnExists(key))
                this[key] = value;
            return this;
        }

        public DateTime GetDateTimeHandleDBNull(string columnName, DateTime ifIsNull)
        {
            try
            {
                return !this.OwnerTable.IsColumnExists(columnName) ||
                    Convert.IsDBNull(this.DataRow[columnName]) ?
                    ifIsNull : DateTime.Parse(this.DataRow[columnName].ToString());
            }
            catch { }

            return ifIsNull;
        }

        public decimal GetDecimalHandleDBNull(string columnName, decimal ifIsNull)
        {
            try
            {
                return !this.OwnerTable.IsColumnExists(columnName) ||
                    Convert.IsDBNull(this.DataRow[columnName]) ?
                    ifIsNull : decimal.Parse(this.DataRow[columnName].ToString());
            }
            catch { }

            return ifIsNull;
        }

        public int GetIntegerHandleDBNull(string columnName, int ifIsNull)
        {
            try
            {
                return !this.OwnerTable.IsColumnExists(columnName) ||
                    Convert.IsDBNull(this.DataRow[columnName]) ?
                    ifIsNull : int.Parse(this.DataRow[columnName].ToString());
            }
            catch { }

            return ifIsNull;
        }

        public long GetLongHandleDBNull(string columnName, long ifIsNull)
        {
            try
            {
                return !this.OwnerTable.IsColumnExists(columnName) ||
                    Convert.IsDBNull(this.DataRow[columnName]) ?
                    ifIsNull : long.Parse(this.DataRow[columnName].ToString());
            }
            catch { }

            return ifIsNull;
        }

        public string GetStringHandleDbNull(string columnName)
        {
            try
            {
                return !this.OwnerTable.IsColumnExists(columnName) ||
                    Convert.IsDBNull(this.DataRow[columnName]) ?
                    null : this.DataRow[columnName].ToString();
            }
            catch { }

            return null;
        }

        public System.Data.DataRow GetDataRow() { return this.DataRow; }
    }
}
