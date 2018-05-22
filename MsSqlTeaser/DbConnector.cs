using System;
using System.Collections.Generic;
using System.Text;
using CodingCat.SqlTeaser;

namespace CodingCat.MsSqlTeaser
{
    public class DbConnector : SqlTeaser.IDbConnector, System.IDisposable
    {
        private System.Data.SqlClient.SqlConnection Connection = null;
        private System.Data.SqlClient.SqlDataAdapter DataAdapter = null;
        private System.Data.SqlClient.SqlCommandBuilder CommandBuilder = null;

        public string ConnectionString { get; private set; }
        public string LatestID { get; private set; } = null;
        public long AffectedRowCount { get; private set; } = 0;

        #region Constructor(s)
        public DbConnector(string connectionString) { this.ConnectionString = connectionString; }
        #endregion

        public IDbConnector Execute(
            ISqlCommand sqlCommand
        )
        {
            this.ResetStateProperties();
            this.Close();

            using (var Connection = this.OpenSqlConnection())
            {
                using (var cmd = sqlCommand.GenerateSqlCommandInstance<System.Data.SqlClient.SqlCommand>())
                {
                    cmd.Connection = Connection;
                    this.AffectedRowCount = cmd.ExecuteNonQuery();
                }
                this.GetAndAssignLatestID(Connection);
                Connection.Close();
            }

            return this;
        }

        public IDbConnector Insert(
            IDbTable dbTable
        )
        {
            return this.Update(dbTable);
        }

        public IDbConnector Read(
            ISqlCommand sqlCommand,
            out IDbTable dbTable
        )
        {
            this.ResetStateProperties();
            this.Connection = this.OpenSqlConnection();
            using (var cmd = sqlCommand.GenerateSqlCommandInstance<System.Data.SqlClient.SqlCommand>())
            {
                cmd.Connection = this.Connection;
                this.DataAdapter = this.GetDataAdapter(cmd);
                this.CommandBuilder = this.GetCommandBuilder(this.DataAdapter);

                var DataTable = new System.Data.DataTable();
                this.DataAdapter.Fill(DataTable);
                dbTable = new DbTable(DataTable);
            }
            this.Connection.Close();

            return this;
        }

        public IDbConnector Update(
            IDbTable dbTable
        )
        {
            this.ResetStateProperties();
            this.Connection = this.OpenSqlConnection(this.Connection);
            this.AffectedRowCount = this.DataAdapter.Update(dbTable.GetSourceTable<System.Data.DataTable>());
            this.GetAndAssignLatestID(this.Connection);
            this.Connection.Close();

            return this;
        }

        public IDbConnector Delete(
            ISqlCommand sqlCommand
        )
        {
            return this.Execute(sqlCommand);
        }

        public void Dispose() { this.Close(); }

        private void ResetStateProperties()
        {
            this.AffectedRowCount = 0;
            this.LatestID = null;
        }

        private System.Data.SqlClient.SqlConnection OpenSqlConnection()
        {
            var Connection = new System.Data.SqlClient.SqlConnection(this.ConnectionString);
            Connection.Open();
            return Connection;
        }

        private System.Data.SqlClient.SqlConnection OpenSqlConnection(
            System.Data.SqlClient.SqlConnection connection
        )
        {
            if (connection == null) return this.OpenSqlConnection();
            try { connection.Open(); } catch { return this.OpenSqlConnection(); }
            return connection;
        }

        private System.Data.SqlClient.SqlDataAdapter GetDataAdapter(System.Data.SqlClient.SqlCommand sqlCommand)
        {
            return new System.Data.SqlClient.SqlDataAdapter(sqlCommand);
        }

        private System.Data.SqlClient.SqlCommandBuilder GetCommandBuilder(System.Data.SqlClient.SqlDataAdapter dataAdapter)
        {
            return new System.Data.SqlClient.SqlCommandBuilder(dataAdapter);
        }

        private void GetAndAssignLatestID(System.Data.SqlClient.SqlConnection Connection)
        {
            using (var cmd = new System.Data.SqlClient.SqlCommand("Select @@IDENTITY", Connection))
            {
                var ID = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(ID))
                    if (ID != null)
                        this.LatestID = ID.ToString();

                if (string.IsNullOrEmpty(this.LatestID))
                    this.LatestID = null;
            }
        }

        public void Close()
        {
            if (this.DataAdapter != null)
            {
                try { using (this.DataAdapter) { } }
                catch { }
            }

            if (this.CommandBuilder != null)
            {
                try { using (this.CommandBuilder) { } }
                catch { }
            }

            if (this.Connection != null)
            {
                try
                {
                    using (this.Connection)
                        this.Connection.Close();
                }
                catch { }
            }

            this.DataAdapter = null;
            this.CommandBuilder = null;
            this.Connection = null;
        }
    }
}
