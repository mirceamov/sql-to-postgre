using Microsoft.SqlServer.Types;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Z.BulkOperations;

namespace SqlToPostgre
{
    /* The code is the first version and clearly needs refactoring.
     * Uses multiple threads to copy the tables from a sql server db to a postgresql db.
     * Tried it on a 65Gb database and it took 4 hours and 30 min to move it.
     * IMPORTANT NOTE : because I use the bulk-operations library, you need to update the nuget package Z.BulkOperations once a month. 
     * After the update, the free use period will be extended until next month.  
     * */
    public partial class Main : Form
    {
        private readonly BackgroundWorker _bw = new BackgroundWorker();
        private CancellationTokenSource tokenSource;

        public string SQLServerAddress = "192.168.0.1";
        public string PostgresServerAddress = "";

        public string SqlDbName = "dbName";
        public string PostgreDbName = "dbname";

        public string SqlUser = "sa";
        public string PostgreUser = "postgres";

        public string SqlPass = "sa";
        public string PostgrePass = "postgres";

        public string SqlProvider = @"System.Data.SqlClient";
        public string PostgreProvider = @"Npgsql";

        public int SqlPort = 1433;
        public int PostgrePort = 5432;      
        

        public Main()
        {
            InitializeComponent();

            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            _bw.DoWork += DoConvert;
            _bw.RunWorkerCompleted += BwRunWorkerCompleted;
        }

        #region events

        private void btnCheckSqlConnectionString_Click(object sender, EventArgs e)
        {
            ReadSettings();
            var connectionStringSqlServer = GetConnectionString(SQLServerAddress, SqlDbName, SqlUser, SqlPass);

            try
            {
                CheckSqlServerConnection(connectionStringSqlServer);
            }
            catch(Exception ex)
            {
                StatusWriteLine("Eroare la verificarea conexiunii SQL : " + ex.Message);
            }            
        }
        
        private void btnCheckPostgreConnection_Click(object sender, EventArgs e)
        {
            ReadSettings();
            var connectionStringPostgreSql = GetConnectionString(PostgresServerAddress, PostgreDbName, PostgreUser, PostgrePass, PostgreProvider, PostgrePort);

            try
            {
                CheckPostgreSqlConnection(connectionStringPostgreSql);
            }
            catch (Exception ex)
            {
                StatusWriteLine("Eroare la verificarea conexiunii Postgre : " + ex.Message);
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            tokenSource = new CancellationTokenSource();
            btnConvert.Enabled = false;
            btnCancelConvertion.Visible = true;
            progressBar.Show();
            txtStatus.Text = "";
            ReadSettings();

            _bw.WorkerSupportsCancellation = true;
            _bw.RunWorkerAsync();
        }

        private void DoConvert(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            Stopwatch stopWatch1 = new Stopwatch();

            var connectionStringSqlServer = GetConnectionString(SQLServerAddress, SqlDbName, SqlUser, SqlPass);
            var connectionStringPostgresql = GetConnectionString(PostgresServerAddress, PostgreDbName, PostgreUser, PostgrePass, PostgreProvider, PostgrePort);

            // Check SQL Server connection.
            if (CheckSqlServer(ref connectionStringSqlServer))
            {
                StatusWriteLine(connectionStringSqlServer);
                StatusWriteLine(connectionStringPostgresql);
                StatusWriteLine("-----------------------------");

                stopWatch1.Start();

                // Try to open PostgreSQL connection first, create if it does not exist.
                string result = OpenPostgreSql(connectionStringPostgresql);

                if (string.IsNullOrEmpty(result))
                {
                    // Copy to PostgreSQL database.
                    StatusWriteLine("-----------------------------");
                    result = CopyToPostgreSql(connectionStringSqlServer, connectionStringPostgresql);
                    StatusWriteLine("-----------------------------");

                    if (result != "Aborted")
                    {
                        //CreateViews(connectionStringSqlServer, connectionStringPostgresql);
                        //StatusWriteLine("-----------------------------");
                        CreateForeignKeys(connectionStringSqlServer, connectionStringPostgresql);
                        StatusWriteLine("-----------------------------");
                    }
                }

                stopWatch1.Stop();
                StatusWriteLine(result + string.Format(" in {0} seconds", stopWatch1.ElapsedMilliseconds / 1000));
            }
        }
        
        private void btnCancelConvertion_Click(object sender, EventArgs e)
        {
            _bw.CancelAsync();
            tokenSource.Cancel();
        }

        private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnConvert.Enabled = true;
            btnCancelConvertion.Visible = false;
            progressBar.Hide();
        }
        
        private void btnClearStatus_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Sigur vreti sa stergeti status-ul?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                txtStatus.Text = "";
            }
        }

        #endregion

        #region functions

        /// <summary>
        /// Try to connect to SQLEXPRESS, if this fails try standard SQL Server.
        /// https://blogs.msdn.microsoft.com/sql_protocols/2008/09/19/understanding-data-sourcelocal-in-sql-server-connection-strings/
        /// </summary>
        /// <returns>True on success.</returns>
        public bool CheckSqlServer(ref string connectionStringSqlServer)
        {
            bool result;

            using (var msConnection = new SqlConnection(connectionStringSqlServer))
            {
                msConnection.Open();
                result = msConnection.State == System.Data.ConnectionState.Open;

                if (result)
                {
                    StatusWriteLine("Connection with SQL Server OK. SQL Server Express version : " + msConnection.ServerVersion);
                }
                else
                {
                    // Not SQLEXPRESS, try standard SQL Server.
                    SQLServerAddress = @"(local)";
                    connectionStringSqlServer = GetConnectionString(SQLServerAddress, SqlDbName, SqlUser, SqlPass);

                    using (var msConnection2 = new SqlConnection(connectionStringSqlServer))
                    {
                        msConnection2.Open();
                        result = msConnection2.State == System.Data.ConnectionState.Open;

                        if (result)
                        {
                            StatusWriteLine("SQL Server " + msConnection.ServerVersion);
                        }
                        else
                        {
                            StatusWriteLine("Could not connect to SQL Server.");
                        }
                    }
                }
            }
            return result;
        }
        
        /// <summary>
        /// Try to open PostgreSQL connection, if it fails create a new database.
        /// </summary>
        /// <param name="connectionStringPostgresql">The Postgresql connectionString.</param>
        /// <returns>Empty string on success.</returns>
        public string OpenPostgreSql(string connectionStringPostgresql)
        {
            bool catalogExists = false;

            using (var pgConnection1 = new NpgsqlConnection(connectionStringPostgresql))
            {
                try
                {
                    pgConnection1.Open();
                    catalogExists = pgConnection1.State == ConnectionState.Open;
                }
                catch (Exception ex)
                {
                    StatusWriteLine(ex.Message);

                    if (!ex.Message.Contains("does not exist"))
                    {
                        // Other error than "database does not exist"
                        return ex.Message;
                    }
                }
            }

            if (catalogExists)
            {
                if (MessageBox.Show(string.Format("Vrei sa stergi baza de date '{0}' si sa o recreezi?", PostgreDbName), "Sterge DB?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    StatusWriteLine(@"DROP DATABASE " + PostgreDbName);

                    try
                    {
                        // DROP DATABASE.
                        // Connection to main database "postgres", typically with: username "postgres", password "postgres".
                        var connWithoutCatalog2 = GetConnectionString(PostgresServerAddress, string.Empty, PostgreUser, PostgrePass, PostgreProvider, PostgrePort);
                        DeleteDatabase(connWithoutCatalog2, PostgreDbName);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            
                // Create new database.
                string pgSql = string.Format("CREATE DATABASE \"{0}\";", PostgreDbName);
                StatusWriteLine(pgSql);

                // Connection to main database "postgres", typically with: username "postgres", password "postgres".
                var connWithoutCatalog = GetConnectionString(PostgresServerAddress, string.Empty, PostgreUser, PostgrePass, PostgreProvider, PostgrePort);
                var resultCreate = ExecuteSqlScript(connWithoutCatalog, pgSql);

                using (var pgConnection = new NpgsqlConnection(connectionStringPostgresql))
                {
                    if (resultCreate)
                    {
                        pgConnection.Open();
                    }

                    if (pgConnection.State != ConnectionState.Open)
                    {
                        return @"Error could not open connection: " + connectionStringPostgresql;
                    }
                }

                //-- Needed for: uuid PRIMARY KEY DEFAULT uuid_generate_v1()
                ExecuteSqlScript(connectionStringPostgresql, "CREATE EXTENSION \"uuid-ossp\";");

                //-- Postgis extension for geography
                ExecuteSqlScript(connectionStringPostgresql, "CREATE EXTENSION postgis;");
            
            return string.Empty;
        }



        string CopyTable(string connectionStringSqlServer, string connectionStringPostgresql, DataRow tableInfo)
        {
            List<Tuple<string, string, string>> tablePKs = new List<Tuple<string, string, string>>();            
            string[] indexes = new string[10];
            string[] defaults = new string[10];

            int fetchRows = 100000;
            fetchRows = int.TryParse(txtFetchRows.Text, out fetchRows) ? fetchRows : 100000;

            var msTables = new List<string>();
            var msColumns = new List<string>();
            var msColumnsB = new List<string>();

            string msSchema = (string)tableInfo[1];
            string msTablename = (string)tableInfo[2];
            string pgTablename = msTablename.Replace(' ', '_').ToLower();

            if (pgTablename == "sysdiagrams")
            {
                return "sysdiagrams table not to be copied";
            }

            using (var pgConnection = new NpgsqlConnection(connectionStringPostgresql))
            {
                pgConnection.Open();

                using (var msConnection = new SqlConnection(connectionStringSqlServer))
                {
                    msConnection.Open();

                    if (msConnection.State == ConnectionState.Open)
                    {
                        string pgCreateFields = GetFieldInformation(msConnection, msTablename, ref msColumns, ref msSchema, ref indexes, ref defaults, ref tablePKs);

                        var pgSchema = msSchema.Replace(' ', '_');
                        pgTablename = pgSchema + "." + pgTablename;
                        bool result1 = ExecuteSqlScript(connectionStringPostgresql, "create schema if not exists " + pgSchema + ";");

                        var pgSql1 = string.Format("create table {0} ({1});", pgTablename, pgCreateFields);
                        StatusWriteLine(string.Format("CREATE POSTGRE TABLE : {0}", pgSql1));
                        bool result2 = ExecuteSqlScript(connectionStringPostgresql, pgSql1);

                        //COPY DATA FROM SQL TO POSTGRE
                        for (int i = 0; i < msColumns.Count; i++)
                        {
                            msColumnsB.Add("[" + msColumns[i] + "]");
                        }

                        string msColumnsJoin = string.Join(",", msColumnsB);

                        // Read values from source db.
                        var command = msConnection.CreateCommand();
                        command.CommandTimeout = 0;

                        var noMoreSqlData = false;
                        var offsetRows = 0; //skip rows
                        long minID = 0;
                        long maxID = 0;

                        if (tablePKs != null && tablePKs.Count > 0 && (msTablename == "Positions" || msTablename == "PositionExtendedData"))
                        {
                            var commandMinMax = msConnection.CreateCommand();
                            commandMinMax.CommandTimeout = 0;
                            commandMinMax.CommandText = string.Format("SELECT min({0}) AS minPK, max({0}) AS maxPK FROM [{1}].[{2}].[{3}]", tablePKs[0].Item3, SqlDbName, msSchema, msTablename);
                            object[] minMaxObjects = new object[2];
                            using (var dr = commandMinMax.ExecuteReader(CommandBehavior.Default))
                            {
                                while (dr.Read())
                                {
                                    dr.GetValues(minMaxObjects);
                                }
                            }

                            minID = long.TryParse(minMaxObjects[0].ToString(), out minID) ? minID : 0;
                            maxID = long.TryParse(minMaxObjects[1].ToString(), out maxID) ? maxID : 0;
                            StatusWriteLine(string.Format("minID: {0} / maxID: {1}", minID, maxID));
                        }

                        while (!noMoreSqlData)
                        {
                            string msSql = "";

                            //min and max ids are used only for Positions and PositionExtendedData tables
                            if (tablePKs != null && tablePKs.Count > 0 && (msTablename == "Positions" || msTablename == "PositionExtendedData"))
                            {
                                var nextID = minID + fetchRows;
                                msSql = string.Format(@"SELECT {0} FROM [{1}].[{2}].[{3}] WHERE {4} >= {5} AND {4} < {6}", msColumnsJoin, SqlDbName, msSchema, msTablename, tablePKs[0].Item3, minID, nextID);
                                minID = nextID;
                            }
                            else if (tablePKs != null && tablePKs.Count > 0)
                            {
                                msSql = string.Format(@"SELECT {0} FROM [{1}].[{2}].[{3}] ORDER BY {4} OFFSET {5} ROWS FETCH NEXT {6} ROWS ONLY", msColumnsJoin, SqlDbName, msSchema, msTablename, tablePKs[0].Item3, offsetRows, fetchRows);
                            }
                            else
                            {
                                msSql = string.Format(@"SELECT {0} FROM [{1}].[{2}].[{3}]", msColumnsJoin, SqlDbName, msSchema, msTablename);
                                noMoreSqlData = true;
                            }

                            command.CommandText = msSql;
                            StatusWriteLine(string.Format("COPY DATA FROM SQL : {0}", msSql));

                            // Insert values into target db.
                            DataTable dataTable = new DataTable();

                            //5 retries
                            for (int i = 0; i < 5; i++)
                            {
                                try
                                {
                                    using (var dr = command.ExecuteReader(CommandBehavior.Default))
                                    {
                                        dataTable.Clear();
                                        dataTable.Load(dr);

                                        ReplaceNullCharacter(msTablename, ref dataTable);

                                        if (minID > maxID && (msTablename == "Positions" || msTablename == "PositionExtendedData"))
                                        {
                                            noMoreSqlData = true;
                                        }
                                        else
                                        {
                                            if (dataTable.Rows.Count < fetchRows && msTablename != "Positions" && msTablename != "PositionExtendedData")
                                            {
                                                noMoreSqlData = true;
                                            }
                                            else
                                            {
                                                offsetRows += fetchRows;
                                            }
                                        }
                                    }
                                    break;
                                }
                                catch (SqlException e)
                                {                                    
                                    StatusWriteLine(string.Format("EROARE : {0}", e.Message));
                                    Thread.Sleep(1000);
                                    if (msConnection != null && msConnection.State != ConnectionState.Open)
                                    {
                                        msConnection.Open();
                                    }
                                    StatusWriteLine(string.Format("Retry {0}", i));

                                    continue;
                                }
                            }

                            var bulk = new BulkOperation(pgConnection);
                            bulk.BatchSize = dataTable.Rows.Count;
                            bulk.DestinationTableName = pgTablename;
                            bulk.CaseSensitive = CaseSensitiveType.Insensitive;
                            bulk.AllowUpdatePrimaryKeys = true;

                            try
                            {
                                bulk.BulkInsert(dataTable);
                            }
                            catch (Exception ex)
                            {
                                StatusWriteLine(string.Format("ERROR ON BULK INSERT IN TABLE '{0}' : {1}", pgTablename, ex.Message));
                            }

                            if (_bw.CancellationPending)
                            {
                                return "Aborted";
                            }
                        }

                        StatusWriteLine(string.Format("Data for table '{0}' was copied.", pgTablename.ToLower()));

                        // Create DEFAULT values.
                        if (defaults != null)
                        {
                            foreach (string pgDefault in defaults)
                            {
                                ExecuteSqlScript(connectionStringPostgresql, pgDefault);
                                StatusWriteLine(string.Format("Default : {0}", pgDefault));
                            }
                        }

                        // Index is created after the bulk insert operation.
                        if (indexes != null)
                        {
                            foreach (string pgIndex in indexes)
                            {
                                if (!pgIndex.StartsWith("constraint"))
                                {
                                    ExecuteSqlScript(connectionStringPostgresql, pgIndex);
                                    StatusWriteLine(string.Format("Index '{0}' for table '{1}' was created.", pgIndex, pgTablename.ToLower()));
                                }
                            }
                        }

                        //Set primary key to autoincrement from max value
                        if (tablePKs.Count > 0)
                        {
                            foreach (var tableColIndex in tablePKs)
                            {
                                if (tableColIndex.Item2.Split(',').Count() == 1)
                                {
                                    var serialSeq = string.Format("SELECT setval(pg_get_serial_sequence('{0}', '{1}'), coalesce(max({1}), 0) + 1, false) FROM {0};", tableColIndex.Item1, tableColIndex.Item2);
                                    ExecuteSqlScript(connectionStringPostgresql, serialSeq);
                                    StatusWriteLine(string.Format("Serial sequence set to max primary key '{0}' for table '{1}' was created.", tableColIndex.Item2, tableColIndex.Item1));
                                }
                            }
                        }
                    }
                }
            }
            return string.Format("TABLE '{0}' WAS MOVED TO POSTGRESQL", pgTablename.ToLower());
        }


        /// <summary>
        /// Copy data from MSSQL directly to PostgreSQL.
        /// If the target database does not exist, create it.
        /// </summary>
        public string CopyToPostgreSql(string connectionStringSqlServer, string connectionStringPostgresql)
        {            
            var token = tokenSource.Token;
            List<Task> tasks = new List<Task>();
            var tableList = new ConcurrentBag<string>();

            using (var pgConnection = new NpgsqlConnection(connectionStringPostgresql))
            {
                pgConnection.Open();
                
                using (var msConnection = new SqlConnection(connectionStringSqlServer))
                {
                    msConnection.Open();

                    if (msConnection.State == ConnectionState.Open)
                    {
                        DataTable dt = msConnection.GetSchema("Tables", new string[] { null, null, null, "BASE TABLE" });

                        foreach (DataRow rowTable in dt.Rows)
                        {
                            Task t = Task.Run(() => {
                                CopyTable(connectionStringSqlServer, connectionStringPostgresql, rowTable);
                                tableList.Add((string)rowTable[2]);
                            }, token);
                            tasks.Add(t);                            
                        }

                        try
                        {
                            Task.WaitAll(tasks.ToArray());
                        }
                        catch (AggregateException e)
                        {
                            StatusWriteLine("Exception messages:");
                            foreach (var ie in e.InnerExceptions)
                                StatusWriteLine(string.Format("{0}: {1}", ie.GetType().Name, ie.Message));
                        }
                        finally
                        {
                            tokenSource.Dispose();
                        }
                    }
                }
            }

            return "FINISHED";
        }
        
        /// <summary>
        /// Replace null characters from string column
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        void ReplaceNullCharacter(string tableName, ref DataTable dataTable)
        {
            if (tableName.Contains("TCPLogBackUp"))
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataTable.Rows[i]["DataString"] = dataTable.Rows[i]["DataString"].ToString().Replace("\0", string.Empty);
                    //Base64Encode(dataTable.Rows[i]["DataString"].ToString());
                }
            }
            else if(tableName.Contains("CommandsIn"))
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataTable.Rows[i]["Command"] = dataTable.Rows[i]["Command"].ToString().Replace("\0", string.Empty);
                    //Base64Encode(dataTable.Rows[i]["Command"].ToString());
                }
            }
            else if (tableName.Contains("CommandsOut"))
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataTable.Rows[i]["Command"] = dataTable.Rows[i]["Command"].ToString().Replace("\0", string.Empty);
                }
            }
        }
        
        /// <summary>
        /// Get the indexes (including primary key) formatted PostgreSQL style: 
        /// "constraint pk_name primary key(field1,field2,field3 ...)".
        /// "create index index_name on tableName (field1,field2,field3 ...)"
        /// </summary>
        public string[] GetIndexes(SqlConnection msConnection, string tableName, string schema, ref List<Tuple<string, string, string>> tablePKs)
        {
            string indexName = string.Empty;
            string indexColumns = string.Empty;
            string pgTableName = schema + "." + tableName;
            pgTableName = pgTableName.ToLower();
            var indexes = new List<string>();

            try
            {
                var command = msConnection.CreateCommand();

                // Find indexes, including the primary key.
                // EXEC sys.sp_helpindex @objname = N'dbo.tablename'
                // EXEC sys.sp_helpconstraint @objname = N'dbo.tablename', @nomsg = N'nomsg'
                var msSql = string.Format("EXEC sys.sp_helpindex @objname = N'{0}.{1}' ", schema, tableName);
                command.CommandText = msSql;

                using (var dr = command.ExecuteReader(CommandBehavior.Default))
                {
                    while (dr.Read())
                    {
                        // 0 = index_name, 1 = index_description, 2 = index_keys.
                        object[] rowObjects = new object[3];
                        int result = dr.GetValues(rowObjects);
                        indexName = rowObjects[0].ToString().ToLower();
                        var description = rowObjects[1].ToString().ToLower();
                        indexColumns = rowObjects[2].ToString().ToLower();
                        ////StatusWriteLine(indexName + " / " + description + " / " + indexColumns);
                        indexName = indexName.Replace(' ', '_');
                        indexName = indexName.Replace('-', '_');
                        indexName = indexName.Replace("<", String.Empty);
                        indexName = indexName.Replace(">", String.Empty);
                        indexName = indexName.Replace(",", String.Empty);

                        indexName += DateTime.Now.ToString("yyyyMMddHHmmss");
                        indexColumns = indexColumns.Replace(", ", ",");
                        indexColumns = indexColumns.Replace(' ', '_');

                        // TODO: descending indexed column with (-) at end ?
                        indexColumns = indexColumns.Replace("(-)", string.Empty);

                        if (description.Contains("primary key"))
                        {
                            //string pk = string.Format("constraint {0} primary key({1})", indexName, indexColumns);
                            string pk = string.Format("ALTER TABLE {0} ADD PRIMARY KEY ({1});", pgTableName, indexColumns); 
                            indexes.Add(pk);
                            tablePKs.Add(new Tuple<string, string, string>(pgTableName, indexColumns, rowObjects[2].ToString()));
                        }
                        else
                        {
                            string ix = string.Format("create index {0} on {1} ({2});", indexName, pgTableName, indexColumns);
                            indexes.Add(ix);
                        }
                    }
                }
            }
            catch
            {
                StatusWriteLine("GetIndexes() error.");
            }

            if (string.IsNullOrEmpty(indexName))
            {
                // No PK or indexes found for this table.
                return null;
            }

            return indexes.ToArray();
        }

        /// <summary>
        /// Get the DEFAULTS formatted PostgreSQL style, example: 
        /// "alter table tablename alter column columnname set default 'false';"
        /// "alter table tablename alter column columnname set default (0);"
        /// Note: ROWGUIDCOL will not be found here.
        /// </summary>
        public List<string> GetDefaults(SqlConnection msConnection, string tableName, List<string> fields, string schema)
        {
            string constraintName = string.Empty;
            string constraintKeys = string.Empty;
            string pgTableName = schema + "." + tableName;
            pgTableName = pgTableName.ToLower();
            var defaultsTemp = new List<string>();

            try
            {
                var command = msConnection.CreateCommand();
                var msSql = string.Format("EXEC sys.sp_helpconstraint @objname = N'{0}.{1}', @nomsg = N'nomsg'", schema, tableName);
                command.CommandText = msSql;

                using (var dr = command.ExecuteReader(CommandBehavior.Default))
                {
                    while (dr.Read())
                    {
                        // 0 = constraint_type, 1 = constraint_name, 6 = constraint_keys.
                        object[] rowObjects = new object[7];
                        int result = dr.GetValues(rowObjects);
                        var constraintType = rowObjects[0].ToString().ToLower();
                        constraintName = rowObjects[1].ToString().ToLower();
                        constraintKeys = rowObjects[6].ToString().ToLower();

                        // string str = constraintName + " / " + constraintType + " / " + constraintKeys;

                        if (constraintType.StartsWith("default on column "))
                        {
                            string columName = constraintType.Replace("default on column ", string.Empty);

                            // Remove the extra hooks ()
                            constraintKeys = constraintKeys.Remove(0, 1);
                            constraintKeys = constraintKeys.Remove(constraintKeys.Length - 1, 1);

                            constraintKeys = constraintKeys.Replace("newid()", "uuid_generate_v1()");

                            constraintKeys = constraintKeys.Replace("getdate()", "now()");

                            // boolean field ?
                            var found = fields.Find(x => x.ToLower().StartsWith(columName));

                            if (!string.IsNullOrEmpty(found))
                            {
                                if (found.Contains(" bit "))
                                {
                                    constraintKeys = constraintKeys.Replace("(0)", "false");
                                    constraintKeys = constraintKeys.Replace("(1)", "true");
                                }
                            }

                            columName = columName.Replace(' ', '_');
                            string df = string.Format("alter table {0} alter column {1} set default {2};", pgTableName, columName, constraintKeys);
                            defaultsTemp.Add(df);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusWriteLine("GetDefaults() " + ex.Message);
            }

            return defaultsTemp;
        }

        /// <summary>
        /// Get the ROWGUIDCOL and IDENTITY Columns for a table formatted in PostgreSQL style, example: 
        /// "alter table tablename alter column columnname set default uuid_generate_v1();"
        /// </summary>
        public List<string> GetDefaults2(SqlConnection msConnection, string tableName, string schema)
        {
            var defaultsTemp = new List<string>();
            string pgTableName = schema + "." + tableName;
            pgTableName = pgTableName.ToLower();

            try
            {
                var command = msConnection.CreateCommand();

                var msSql = string.Format(@"SELECT DISTINCT col.name, is_rowguidcol, is_identity
                                            FROM    sys.indexes ind
                                                    INNER JOIN sys.index_columns ic
                                                        ON ind.object_id = ic.object_id
                                                           AND ind.index_id = ic.index_id
                                                    INNER JOIN sys.columns col
                                                        ON ic.object_id = col.object_id
                                                           AND ic.column_id = col.column_id
                                                    INNER JOIN sys.tables t
                                                        ON ind.object_id = t.object_id
                                            WHERE   t.is_ms_shipped = 0 
                                                    AND (col.is_rowguidcol > 0 OR col.is_identity > 0)
                                                    AND OBJECT_SCHEMA_NAME(ind.object_id) = '{0}'
		                                            AND OBJECT_NAME(ind.object_id) = '{1}'
                                            ", schema, tableName);

                command.CommandText = msSql;

                using (var dr = command.ExecuteReader(CommandBehavior.Default))
                {
                    while (dr.Read())
                    {
                        // 0 = ColumnName, 1 = Rowguid, 2 = Identity.
                        object[] rowObjects = new object[3];
                        int result = dr.GetValues(rowObjects);
                        var columnName = rowObjects[0].ToString().ToLower();
                        var isrowguidcol = rowObjects[1].ToString();
                        var isidentity = rowObjects[2].ToString();

                        if (!string.IsNullOrEmpty(columnName))
                        {
                            columnName = columnName.Replace(' ', '_');

                            if (isrowguidcol == "True")
                            {
                                // ROWGUIDCOL found for this table.
                                string df = string.Format("alter table {0} alter column {1} set default uuid_generate_v1();", pgTableName, columnName);
                                defaultsTemp.Add(df);
                            }
                            else if (isidentity == "True")
                            {
                                // IDENTITY found for this table, convert to "SERIAL" pseudo data type.
                                string seq1 = string.Format("create sequence {0}_{1}_seq;", pgTableName, columnName);
                                string df = string.Format("alter table {0} alter column {1} set default nextval('{0}_{1}_seq');", pgTableName, columnName);
                                string seq2 = string.Format("alter sequence {0}_{1}_seq owned by {0}.{1};", pgTableName, columnName);
                                defaultsTemp.Add(seq1);
                                defaultsTemp.Add(df);
                                defaultsTemp.Add(seq2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusWriteLine("GetDefaults2() " + ex.Message);
            }

            return defaultsTemp;
        }

        /// <summary>
        /// Get the PostgreSQL style fields for CREATE TABLE or VIEW from the SQL Server Schema Collections.
        /// Add primary key (if it exists).
        /// https://msdn.microsoft.com/en-us/library/ms254969(v=vs.110).aspx
        /// http://www.postgresql.org/docs/9.3/static/datatype.html
        /// </summary>
        public string GetFieldInformation(SqlConnection msConnection, string msTablename, ref List<string> msColumns, ref string schema, ref string[] indexes, ref string[] defaults, ref List<Tuple<string, string, string>> tablePKs)
        {
            // 0 = Catalog; 1 = Schema; 2 = Table Name; 3 = Column Name. 
            var columnRestrictions = new string[4];
            // columnRestrictions[1] = schema;     // To filter on schema.
            columnRestrictions[2] = msTablename;
            DataTable dataColumns = msConnection.GetSchema("Columns", columnRestrictions);
            List<string> pgFields = new List<string>();
            string pgCreateFields;

            foreach (DataRow rowColumn in dataColumns.Rows)
            {
                string fieldCreate;
                schema = rowColumn["table_schema"].ToString();
                string columnName = rowColumn["column_name"].ToString();
                string datatype = rowColumn["data_type"].ToString();
                string charlen = rowColumn["character_maximum_length"].ToString();
                string nullable = rowColumn["is_nullable"].ToString();
                // column_default
                // numeric_precision

                msColumns.Add(columnName);

                // Replace spaces by underscores in PostgreSQL field.
                columnName = columnName.Replace(' ', '_');

                // Only for tables.
                if (string.IsNullOrEmpty(charlen) || (charlen == "-1" && datatype.Contains("geography")))
                {
                    fieldCreate = columnName + " " + datatype;
                }
                else if (charlen == "-1")
                {
                    fieldCreate = columnName + " " + datatype + "(max)";

                }
                else if (datatype == "binary" || datatype == "varbinary" || datatype == "image")
                {
                    // Will be converted to bytea, without (length).
                    fieldCreate = columnName + " " + datatype;
                }
                else
                {
                    fieldCreate = columnName + " " + datatype + "(" + charlen + ")";
                }

                if (nullable == "YES")
                {
                    fieldCreate += " NULL";
                }
                else
                {
                    fieldCreate += " NOT NULL";
                }

                // StatusWriteLine(fieldCreate);
                pgFields.Add(fieldCreate);
            }

            // Get the DEFAULTS and ROWGUIDCOL.
            var defaults1 = GetDefaults(msConnection, msTablename, pgFields, schema);

            var defaults2 = GetDefaults2(msConnection, msTablename, schema);

            if (defaults2.Count > 0)
            {
                defaults1.AddRange(defaults2);
            }

            defaults = defaults1.ToArray();

            indexes = GetIndexes(msConnection, msTablename, schema, ref tablePKs);
            
            // Translate to PostgreSQL datatypes.
            pgCreateFields = string.Join(",", pgFields);
            pgCreateFields = pgCreateFields.ToLower();
            pgCreateFields = pgCreateFields.Replace(" uniqueidentifier", " uuid");
            pgCreateFields = pgCreateFields.Replace(" nvarchar(max)", " text");
            pgCreateFields = pgCreateFields.Replace(" varchar(max)", " text");
            pgCreateFields = pgCreateFields.Replace(" nvarchar", " varchar");
            pgCreateFields = pgCreateFields.Replace(" nchar", " char");
            pgCreateFields = pgCreateFields.Replace(" xml(max)", " text");
            pgCreateFields = pgCreateFields.Replace(" xml", " text");
            pgCreateFields = pgCreateFields.Replace(" tinyint", " smallint");
            pgCreateFields = pgCreateFields.Replace(" double", " double precision");
            pgCreateFields = pgCreateFields.Replace(" decimal", " numeric");
            pgCreateFields = pgCreateFields.Replace(" smallmoney", " numeric");
            pgCreateFields = pgCreateFields.Replace(" money", " numeric");
            pgCreateFields = pgCreateFields.Replace(" hierarchyid", " bytea");
            pgCreateFields = pgCreateFields.Replace(" geography", " text");
            pgCreateFields = pgCreateFields.Replace(" varbinary(max)", " bytea");
            pgCreateFields = pgCreateFields.Replace(" varbinary", " bytea");
            pgCreateFields = pgCreateFields.Replace(" binary", " bytea");
            pgCreateFields = pgCreateFields.Replace(" image", " bytea");
            pgCreateFields = pgCreateFields.Replace(" datetime2", " timestamptz");
            pgCreateFields = pgCreateFields.Replace(" datetime", " timestamptz");
            pgCreateFields = pgCreateFields.Replace(" int ", " integer ");
            pgCreateFields = pgCreateFields.Replace(" bit ", " boolean ");

            return pgCreateFields;
        }

        /// <summary>
        /// Create all views for catalog, e.g. AdventureWorksLT2008R2.
        /// Note: for AdventureWorksLT2008R2 only 1 of the 3 views can be converted.
        /// </summary>
        public void CreateViews(string connectionStringSqlServer, string connectionStringPostgresql)
        {
            StatusWriteLine(@"Creating Views ...");

            using (var cn = new SqlConnection(connectionStringSqlServer))
            {
                // SELECT [VIEW_DEFINITION] FROM [AdventureWorksLT2008R2].[INFORMATION_SCHEMA].[VIEWS]
                var msSql = string.Format(@"SELECT [VIEW_DEFINITION] FROM [{0}].[INFORMATION_SCHEMA].[VIEWS]", SqlDbName);
                StatusWriteLine(msSql);
                cn.Open();
                var command = cn.CreateCommand();
                command.CommandText = msSql;

                using (var dr = command.ExecuteReader(CommandBehavior.Default))
                {
                    if (!dr.HasRows)
                    {
                        StatusWriteLine(@"No views.");
                    }

                    object[] rowObjects = new object[1];

                    // Read values from source db, e.g.
                    // CREATE VIEW [SalesLT].[vProductAndDescription]   WITH SCHEMABINDING   AS   SELECT   p.[ProductID]   ,p.[Name]   ,pm.[Name] AS [ProductModel]   ,pmx.[Culture]   ,pd.[Description]   FROM [SalesLT].[Product] p   INNER JOIN [SalesLT].[ProductModel] pm   ON p.[ProductModelID] = pm.[ProductModelID]   INNER JOIN [SalesLT].[ProductModelProductDescription] pmx   ON pm.[ProductModelID] = pmx.[ProductModelID]   INNER JOIN [SalesLT].[ProductDescription] pd   ON pmx.[ProductDescriptionID] = pd.[ProductDescriptionID];  
                    while (dr.Read())
                    {
                        int result = dr.GetValues(rowObjects);
                        var pgSql = rowObjects[0].ToString();
                        StatusWriteLine(string.Empty);
                        StatusWriteLine(pgSql);

                        pgSql = pgSql.Replace("WITH SCHEMABINDING", string.Empty);
                        pgSql = pgSql.Replace("[", string.Empty);
                        pgSql = pgSql.Replace("]", string.Empty);
                        pgSql = pgSql.ToLower();

                        var pgArr = pgSql.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        StatusWriteLine(pgArr[0]);
                        StatusWriteLine(string.Empty);

                        // Try to create view in target db..
                        if (!ExecuteSqlScript(connectionStringPostgresql, pgSql))
                        {
                            StatusWriteLine(string.Empty);
                            StatusWriteLine(@"Error: view could not be created !");
                            StatusWriteLine(string.Empty);
                        }
                    }

                    StatusWriteLine(string.Empty);
                }
            }
        }

        /// <summary>
        /// Create PostgreSQL foreign keys, this needs to be done as a last step to avoid conflicts.
        /// </summary>
        public void CreateForeignKeys(string connectionStringSqlServer, string connectionStringPostgresql)
        {
            StatusWriteLine(@"Creating Foreign Keys ...");
            var fkStatements = GetForeignKeyInformation(connectionStringSqlServer);

            if (fkStatements.Length < 1)
            {
                StatusWriteLine(@"No Foreign Keys.");
            }

            foreach (string fkStatement in fkStatements)
            {
                ExecuteSqlScript(connectionStringPostgresql, fkStatement);
            }
        }

        /// <summary>
        /// Get foreign key information from SQL Server.
        /// Return PostgreSQL ALTER TABLE statements array.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection String.</param>
        /// <returns>string array.</returns>
        public string[] GetForeignKeyInformation(string connectionString)
        {
            var pgStatements = new List<string>();

            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();
                var command = cn.CreateCommand();
                command.CommandText = @"
BEGIN
	DECLARE @tbl_foreign_key_columns TABLE ( constraint_name NVARCHAR(128),
											 base_schema_name NVARCHAR(128),
											 base_table_name NVARCHAR(128),
											 base_column_id INT,
											 base_column_name NVARCHAR(128),
											 unique_schema_name NVARCHAR(128),
											 unique_table_name NVARCHAR(128),
											 unique_column_id INT,
											 unique_column_name NVARCHAR(128),
											 base_object_id INT,
											 unique_object_id INT )
	INSERT  INTO @tbl_foreign_key_columns ( constraint_name, base_schema_name, base_table_name, base_column_id, base_column_name, unique_schema_name, unique_table_name, unique_column_id, unique_column_name, base_object_id, unique_object_id )
			SELECT  FK.name AS constraint_name, S.name AS base_schema_name, T.name AS base_table_name, C.column_id AS base_column_id, C.name AS base_column_name, US.name AS unique_schema_name, UT.name AS unique_table_name, UC.column_id AS unique_column_id, UC.name AS unique_column_name, T.[object_id], UT.[object_id]
			FROM    sys.tables AS T
			INNER JOIN sys.schemas AS S
			ON      T.schema_id = S.schema_id
			INNER JOIN sys.foreign_keys AS FK
			ON      T.object_id = FK.parent_object_id
			INNER JOIN sys.foreign_key_columns AS FKC
			ON      FK.object_id = FKC.constraint_object_id
			INNER JOIN sys.columns AS C
			ON      FKC.parent_object_id = C.object_id
					AND FKC.parent_column_id = C.column_id
			INNER JOIN sys.columns AS UC
			ON      FKC.referenced_object_id = UC.object_id
					AND FKC.referenced_column_id = UC.column_id
			INNER JOIN sys.tables AS UT
			ON      FKC.referenced_object_id = UT.object_id
			INNER JOIN sys.schemas AS US
			ON      UT.schema_id = US.schema_id
			ORDER BY base_schema_name, base_table_name
END

SELECT base_schema_name, base_table_name, constraint_name, base_column_name, unique_table_name, unique_column_name FROM @tbl_foreign_key_columns
";

                var dr = command.ExecuteReader();

                while (dr.Read())
                {
                    var baseSchemaName = dr.GetValue(0).ToString().ToLower();
                    var baseTableName = dr.GetValue(1).ToString().ToLower();
                    var constraintName = dr.GetValue(2).ToString().ToLower();
                    var baseColumnName = dr.GetValue(3).ToString().ToLower();
                    var uniqueTableName = dr.GetValue(4).ToString().ToLower();
                    var uniqueColumnName = dr.GetValue(5).ToString().ToLower();
                    //StatusWriteLine(base_table_name);
                    //StatusWriteLine(constraint_name);

                    var pgTableName = baseSchemaName + "." + baseTableName;
                    pgTableName = pgTableName.Replace(" ", "_");

                    var pguniqueTableName = baseSchemaName + "." + uniqueTableName;
                    pguniqueTableName = pguniqueTableName.Replace(" ", "_");

                    //string fkSkip = @"|fk_xxxxxxxxxxx|fk_yyyyyyyyyyyyyyyyyyyyyy|";

                    //if (fkSkip.Contains("|" + constraintName + "|"))
                    //{
                    //    StatusWriteLine(@"Skipping: " + constraintName);
                    //}
                    //else
                    {
                        var sqlFk = string.Format(@"ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {3} ({4});", pgTableName, constraintName.Replace('.','_'), baseColumnName, pguniqueTableName, uniqueColumnName);
                        StatusWriteLine(sqlFk);
                        pgStatements.Add(sqlFk);
                    }
                }

                StatusWriteLine(string.Empty);
            }

            return pgStatements.ToArray();
        }

        /// <summary>
        /// Execute an SQL Server or PostgreSQL command.
        /// Note: does not execute a script with "GO" statements.
        /// </summary>
        /// <param name="connectionString">The connectionString.</param>
        /// <param name="sqlClause">The sql command.</param>
        /// <returns>True on success.</returns>
        public bool ExecuteSqlScript(string connectionString, string sqlClause)
        {
            bool result = false;

            if (IsPostgresql(connectionString))
            {
                using (var cn = new NpgsqlConnection(connectionString))
                {
                    using (var cmd = new NpgsqlCommand(sqlClause, cn))
                    {
                        try
                        {
                            if (cn.State != System.Data.ConnectionState.Open)
                            {
                                cn.Open();
                            }

                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            StatusWriteLine(string.Format("ExecuteSqlScript() error on query '{0}' : {1}", sqlClause, ex.Message));
                        }
                        finally
                        {
                            cn.Close();
                        }
                    }
                }
            }
            else
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand(sqlClause, cn))
                    {
                        try
                        {
                            if (cn.State != System.Data.ConnectionState.Open)
                            {
                                cn.Open();
                            }

                            cmd.ExecuteNonQuery();
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            StatusWriteLine(string.Format("ExecuteSqlScript() error on query '{0}' : {1}", sqlClause, ex.Message));
                        }
                        finally
                        {
                            cn.Close();
                        }
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// Get SQL Server connection string.
        /// </summary>
        /// <param name="sqlServer">The sql server.</param>
        /// <param name="catalog">The catalog.</param>
        /// <param name="username">The username (empty for Windows Authentication).</param>
        /// <param name="password">The password (empty for Windows Authentication).</param>
        /// <returns> The connection string.</returns>
        public string GetConnectionString(string sqlServer, string catalog, string username, string password)
        {
            string connectionString;

            if (username.Equals(string.Empty))
            {
                connectionString = string.Format(@"Integrated Security=SSPI;Persist Security Info=False;Data Source={0}", sqlServer);
            }
            else if (username.Contains("\\"))
            {
                connectionString = string.Format(@"Password={0};User ID={1};Integrated Security=SSPI;Persist Security Info=True;Data Source={2}", password, username, sqlServer);
            }
            else
            {
                connectionString = string.Format(@"Password={0};Persist Security Info=True;User ID={1};Data Source={2}", password, username, sqlServer);
            }

            connectionString = @"Connection Timeout=30;" + connectionString;

            if (!catalog.Equals(string.Empty))
            {
                // add the catalog
                connectionString += ";Initial Catalog=" + catalog;
            }

            return connectionString;
        }

        /// <summary>
        /// Get connection string with ProviderName and port, e.g. "Npgsql" and 5432 for PostgreSQL.
        /// </summary>
        /// <param name="server">The server, e.g. "(local)" for SQL Server, "127.0.0.1" for PostgreSQL.</param>
        /// <param name="catalog">The catalog.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="providername">ProviderName: "System.Data.SqlClient" or empty for SQL Server, "Npgsql" for PostgreSQL</param>
        /// <param name="port">The port, e.g. 5432 for PostgreSQL.</param>
        /// <returns> The connection string.</returns>
        public string GetConnectionString(string server, string catalog, string username, string password, string providername, int port)
        {
            string connectionString;

            if (string.IsNullOrEmpty(providername))
            {
                providername = "System.Data.SqlClient";
            }

            if (providername == @"System.Data.SqlClient")
            {
                // Get SQL Server string, port is not used.
                connectionString = GetConnectionString(server, catalog, username, password);
            }
            else if (providername == @"Npgsql")
            {
                // PostgreSQL example:
                // Server=127.0.0.1;Port=5432;User Id=postgres;Password=postgres;MinPoolSize=8;MaxPoolSize=80;CommandTimeout=20;Database=postgrestest
                if (string.IsNullOrEmpty(catalog))
                {
                    catalog = @"postgres";
                }

                // Set default CommandTimeout of infinite. 
                connectionString = string.Format(
                    "Server={0};Port={1};User Id={2};Password={3};MinPoolSize={4};MaxPoolSize={5};CommandTimeout={6};Database={7}",
                    server,
                    port,
                    username,
                    password,
                    Environment.ProcessorCount,
                    Environment.ProcessorCount * 10,
                    0,
                    catalog);
            }
            else
            {
                string errorMessage = @"GetConnectionString() failed, unknown providername: " + providername;
                throw new Exception(errorMessage);
            }

            return connectionString;
        }

        public void DeleteDatabase(string connectionStringWithoutCatalog, string catalog)
        {
            if (IsPostgresql(connectionStringWithoutCatalog))
            {
                // PostgreSQL, note that the database name must be in lower case.
                NpgsqlConnection.ClearAllPools();

                using (var sqlConn = new NpgsqlConnection(connectionStringWithoutCatalog))
                {
                    sqlConn.Open();
                    using (var command = new NpgsqlCommand(string.Format("DROP DATABASE \"{0}\";", catalog.ToLower()), sqlConn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return;
            }

            // SQL Server.
            using (var connection = new SqlConnection(connectionStringWithoutCatalog))
            {
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    string sqlString;
                    if (connection.Database.ToLower().Equals("master"))
                    {
                        sqlString = string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{0}];", catalog);
                    }
                    else
                    {
                        sqlString = string.Format("USE Master; ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{0}];", catalog);
                    }

                    StatusWriteLine(sqlString);
                    var command = new SqlCommand(sqlString, connection);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Determine if PostgreSQL is used.
        /// </summary>
        /// <param name="connectionstring">The connectionstring.</param>
        /// <returns>True or false.</returns>
        public bool IsPostgresql(string connectionstring)
        {
            return connectionstring.ToLower().Contains("server=");
        }
        
        private void StatusWriteLine(string _text, bool newLine = true)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string, bool>(StatusWriteLine), new object[] { _text, newLine });
                return;
            }

            int caret = 0;
            if (chkAutoScrollStatus.Checked)
            {
                txtStatus.AppendText(newLine ? (_text + Environment.NewLine) : _text);
                caret = txtStatus.SelectionStart;
            }
            else
            {
                caret = txtStatus.SelectionStart;
                txtStatus.Text += newLine ? (_text + Environment.NewLine) : _text;
                txtStatus.SelectionStart = caret;
                txtStatus.ScrollToCaret();
            }
        }
        
        private void ReadSettings()
        {
            SQLServerAddress = txtSqlAddress.Text;
            SqlDbName = txtSqlCatalog.Text;
            SqlUser = txtSqlUser.Text;
            SqlPass = txtSqlPass.Text;
            SqlProvider = txtSqlProvider.Text;

            int sqlPort = 0;
            SqlPort = int.TryParse(txtSqlPort.Text, out sqlPort) ? sqlPort : 1433;

            PostgresServerAddress = txtPostgreAddress.Text;
            PostgreDbName = txtPostgreCatalog.Text.ToLower();
            PostgreUser = txtPostgreUser.Text;
            PostgrePass = txtPostgrePass.Text;
            PostgreProvider = txtPostgreProvider.Text;

            int postgrePort = 0;
            PostgrePort = int.TryParse(txtPostgrePort.Text, out postgrePort) ? postgrePort : 5432;            
        }

        private bool CheckSqlServerConnection(string connectionStringSqlServer)
        {
            bool result;

            using (var msConnection = new SqlConnection(connectionStringSqlServer))
            {
                try
                {
                    msConnection.Open();
                }
                catch(Exception ex)
                {
                    if (msConnection != null)
                        msConnection.Close();
                    throw new Exception(ex.Message);
                }

                result = msConnection.State == System.Data.ConnectionState.Open;

                if (result)
                {
                    StatusWriteLine("Connection with SQL Server OK. SQL Server Express version : " + msConnection.ServerVersion);
                }
                else
                {
                    StatusWriteLine("Could not connect to SQL Server.");                      
                }
            }

            return result;
        }

        private bool CheckPostgreSqlConnection(string connectionStringPostgre)
        {
            bool result;

            using (var postgreConnection = new NpgsqlConnection(connectionStringPostgre))
            {
                try
                {
                    postgreConnection.Open();
                }
                catch (Exception ex)
                {
                    if (postgreConnection != null)
                        postgreConnection.Close();
                    throw new Exception(ex.Message);
                }

                result = postgreConnection.State == System.Data.ConnectionState.Open;

                if (result)
                {
                    StatusWriteLine("Connection with PostgreSQL OK. PostgreSQL version : " + postgreConnection.ServerVersion);
                }
                else
                {
                    StatusWriteLine("Could not connect to PostgreSQL.");
                }
            }

            return result;
        }

        #endregion

    }
}
