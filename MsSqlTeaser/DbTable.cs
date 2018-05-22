using System;
using System.Collections.Generic;
using System.Text;
using CodingCat.SqlTeaser;

namespace CodingCat.MsSqlTeaser
{
    public class DbTable : SqlTeaser.IDbTable, System.IDisposable
    {
        private System.Data.DataTable DataTable { get; set; }

        public List<string> ColumnsName {
            get {
                var Names = new List<string>();
                if (this.DataTable == null || this.DataTable.Columns == null) return Names;
                foreach (System.Data.DataColumn Column in this.DataTable.Columns)
                    Names.Add(Column.ColumnName);
                return Names;
            }
        }

        public List<IDbRow> Rows
        {
            get
            {
                var RowList = new List<IDbRow>();

                if (this.DataTable.Rows.Count > 0)
                    foreach (System.Data.DataRow Row in this.DataTable.Rows)
                        RowList.Add(new DbRow(this, Row));

                return RowList;
            }
        }

        public IDbRow this[int index] { get { return this.Rows[index]; } }

        public IDbRow FirstRow { get { return this[0]; } }

        public IDbRow LastRow { get { return this[this.DataTable.Rows.Count - 1]; } }

        #region Constructor(s)
        public DbTable(System.Data.DataTable dataTable)
        {
            this.DataTable = dataTable;
        }
        #endregion

        public T GetSourceTable<T>()
        {
            return (T)Convert.ChangeType(this.DataTable, typeof(T));
        }

        public bool IsColumnExists(string columnName)
        {
            return this.DataTable.Columns.Contains(columnName);
        }

        public IDbRow RowAt(int index)
        {
            return this.Rows[index];
        }

        public IDbRow GetEmptyRow()
        {
            return new DbRow(this, this.DataTable.NewRow());
        }

        public void AppendRow(IDbRow dbRow)
        {
            if (dbRow.GetType() != typeof(DbRow)) throw new NotImplementedException();
            this.DataTable.Rows.Add((dbRow as DbRow).GetDataRow());
        }

        public void Dispose()
        {
            if (this.DataTable != null)
                try { using (this.DataTable) { } }
                catch { }
            this.DataTable = null;
        }
    }
}
