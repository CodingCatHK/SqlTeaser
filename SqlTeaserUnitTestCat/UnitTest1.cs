using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodingCat.SqlTeaser;

namespace SqlTeaserUnitTestCat
{
    [TestClass]
    public class UnitTest1
    {
        private string ConnectionString = GlobalSettings.Instance.DbConnection.GetConnectionString(false);

        [TestMethod()]
        public void TestRead()
        {
            ISqlCommand cmd = new CodingCat.MsSqlTeaser.SqlCommand().AppendCommand("Select TOP 100 * From tblTest ");
            IDbTable DataTable = null;

            using (new CodingCat.MsSqlTeaser.DbConnector(this.ConnectionString).Read(cmd, out DataTable))
                if (DataTable != null)
                    System.Diagnostics.Debug.WriteLine("Rows: {0}", DataTable.Rows.Count);
        }

        [TestMethod()]
        public void TestInsert()
        {
            ISqlCommand cmd = new CodingCat.MsSqlTeaser.SqlCommand().AppendCommand("Select TOP 100 * From tblTest ");
            IDbTable DataTable = null;

            using (IDbConnector DbConnector = new CodingCat.MsSqlTeaser.DbConnector(
                this.ConnectionString
            ).Read(cmd, out DataTable))
            {
                if (DataTable != null)
                {
                    DataTable.AppendRow(
                        DataTable.GetEmptyRow()
                            .AttachValue("TextField", "TestTextField")
                            .AttachValue("NVarcharField", "TestNVarcharField")
                            .AttachValue("DateTimeField", System.DateTime.Now)
                    );
                    DbConnector.Insert(DataTable);

                    System.Diagnostics.Debug.WriteLine(
                        "LatestID: {0}, Affected: {1}", DbConnector.LatestID, DbConnector.AffectedRowCount
                    );
                }
            }
        }

        [TestMethod()]
        public void TestUpdate()
        {
            ISqlCommand cmd = new CodingCat.MsSqlTeaser.SqlCommand()
                .AppendCommand("Select TOP 100 * From tblTest ")
                .AppendCommand("Where PK = @id ");
            cmd.AddParameter("@id", 5);
            IDbTable DataTable = null;

            using (IDbConnector DbConnector = new CodingCat.MsSqlTeaser.DbConnector(
                this.ConnectionString
            ).Read(cmd, out DataTable))
            {
                if (DataTable != null && DataTable.Rows.Count > 0)
                {
                    DataTable.FirstRow
                        .AttachValue("TextField", "TestTextField2")
                        .AttachValue("NVarcharField", "TestNVarcharField2")
                        .AttachValue("DateTimeField", System.DateTime.Now);
                    DbConnector.Update(DataTable);

                    System.Diagnostics.Debug.WriteLine(
                        "LatestID: {0}, Affected: {1}", DbConnector.LatestID, DbConnector.AffectedRowCount
                    );
                }
            }
        }

        [TestMethod()]
        public void TestDelete()
        {
            ISqlCommand cmd = new CodingCat.MsSqlTeaser.SqlCommand()
                .AppendCommand("Delete From tblTest Where PK = @id ");
            cmd.AddParameter("@id", 5);
            using (IDbConnector DbConnector = new CodingCat.MsSqlTeaser.DbConnector(this.ConnectionString).Delete(cmd))
                System.Diagnostics.Debug.WriteLine(
                    "LatestID: {0}, Affected: {1}", DbConnector.LatestID, DbConnector.AffectedRowCount
                );
        }
    }
}
