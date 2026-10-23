using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace CuahangNongduoc
{

    public class DataService : DataTable
    {

        // The connection to a database of this data service.
        private static OleDbConnection m_Connection;

        // Chuoi ket noi SQL Server. Kiem tra ten may chu bang lenh:  SELECT @@SERVERNAME  trong SSMS
        // (vi du ".\\SQLEXPRESS", "localhost", "TENMAY\\SQLEXPRESS").
        public static String m_ConnectString =
     "Provider=SQLOLEDB;Data Source=.\\SQLEXPRESS;Initial Catalog=CuahangNongDuoc;Integrated Security=SSPI;";

        // Loi ket noi gan nhat (de bao ra thay vi nuot im lang)
        private static String m_LoiKetNoi = "";

        // The command to execute query or non-query command on a database of this data service.
        private OleDbCommand m_Command;

        // The data adapter to execute query on a database of this data service.
        private OleDbDataAdapter m_DataAdapter;

        // Loi cuoi cung cua Load / ExecuteNoneQuery() tren bang nay (rong neu thanh cong)
        private String m_LastError = "";
        public String LastError
        {
            get { return m_LastError; }
        }

        public DataService() { }


        public OleDbCommand Command
        {
            get { return m_Command; }
            set { m_Command = value; }
        }

        // ------------------------------------------------------------------
        // Ghi loi vao file loi_csdl.log (canh file .exe) vi cac ham ben duoi nuot loi:
        // mo file nay se biet cau SQL nao bi loi va vi sao.
        // ------------------------------------------------------------------
        private static void GhiLog(String noiXayRa, String sql, String loi)
        {
            try
            {
                String duongDan = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loi_csdl.log");
                String dong = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + noiXayRa + "] " + loi
                    + (String.IsNullOrEmpty(sql) ? "" : "\r\n    SQL: " + sql) + "\r\n";
                File.AppendAllText(duongDan, dong, Encoding.UTF8);
            }
            catch (Exception) { }
        }

        // ------------------------------------------------------------------
        // Doi cau lenh viet kieu Access/Jet sang kieu chay duoc tren SQL Server qua OleDb:
        //   - tham so ten @id  ->  dau ?  (OleDb chi nhan tham so theo vi tri; SQL Server hieu @id la bien T-SQL)
        //     cac tham so duoc sap lai theo thu tu xuat hien trong cau lenh
        //   - hang True / False -> 1 / 0
        //   - Now() -> GETDATE()
        // Chuoi nam trong dau nhay don duoc giu nguyen. Goi lai nhieu lan van an toan.
        // ------------------------------------------------------------------
        private static void ChuanHoaLenh(OleDbCommand cmd)
        {
            if (cmd == null || cmd.CommandType != CommandType.Text) return;
            String sql = cmd.CommandText;
            if (sql == null || sql.Length == 0) return;

            StringBuilder sb = new StringBuilder();
            List<String> thuTu = new List<String>();
            int i = 0;
            while (i < sql.Length)
            {
                char c = sql[i];

                if (c == '\'')
                {
                    sb.Append(c);
                    i++;
                    while (i < sql.Length)
                    {
                        sb.Append(sql[i]);
                        if (sql[i] == '\'')
                        {
                            if (i + 1 < sql.Length && sql[i + 1] == '\'')
                            {
                                sb.Append('\'');
                                i += 2;
                                continue;
                            }
                            i++;
                            break;
                        }
                        i++;
                    }
                    continue;
                }

                if (c == '@')
                {
                    if (i + 1 < sql.Length && sql[i + 1] == '@')
                    {
                        sb.Append("@@");
                        i += 2;
                        continue;
                    }
                    int j = i + 1;
                    while (j < sql.Length && (Char.IsLetterOrDigit(sql[j]) || sql[j] == '_')) j++;
                    if (j > i + 1)
                    {
                        thuTu.Add(sql.Substring(i + 1, j - i - 1));
                        sb.Append('?');
                        i = j;
                        continue;
                    }
                }

                if (Char.IsLetter(c) || c == '_')
                {
                    int j = i;
                    while (j < sql.Length && (Char.IsLetterOrDigit(sql[j]) || sql[j] == '_')) j++;
                    String w = sql.Substring(i, j - i);
                    String up = w.ToUpperInvariant();
                    if (up == "TRUE") sb.Append('1');
                    else if (up == "FALSE") sb.Append('0');
                    else if (up == "NOW" && j + 1 < sql.Length && sql[j] == '(' && sql[j + 1] == ')')
                    {
                        sb.Append("GETDATE()");
                        j += 2;
                    }
                    else sb.Append(w);
                    i = j;
                    continue;
                }

                sb.Append(c);
                i++;
            }

            if (thuTu.Count > 0)
            {
                List<OleDbParameter> moi = new List<OleDbParameter>();
                foreach (String ten in thuTu)
                {
                    OleDbParameter goc = null;
                    foreach (OleDbParameter p in cmd.Parameters)
                    {
                        String pn = (p.ParameterName == null) ? "" : p.ParameterName.TrimStart('@');
                        if (String.Equals(pn, ten, StringComparison.OrdinalIgnoreCase)) { goc = p; break; }
                    }
                    if (goc == null)
                        throw new InvalidOperationException("Cau lenh dung tham so @" + ten + " nhung chua khai bao tham so nay.");

                    OleDbParameter np = new OleDbParameter(goc.ParameterName, goc.OleDbType, goc.Size);
                    np.Precision = goc.Precision;
                    np.Scale = goc.Scale;
                    np.Direction = goc.Direction;
                    np.Value = goc.Value;
                    moi.Add(np);
                }
                cmd.Parameters.Clear();
                foreach (OleDbParameter p in moi) cmd.Parameters.Add(p);
            }

            cmd.CommandText = sb.ToString();
        }

        public void Load(OleDbCommand command)
        {
            m_LastError = "";
            if (!OpenConnection())
            {
                // Khong ket noi duoc CSDL: bao ro nguyen nhan thay vi de luoi trong
                m_LastError = "Khong ket noi duoc SQL Server: " + m_LoiKetNoi;
                throw new Exception(m_LastError + "\r\nChuoi ket noi: " + m_ConnectString);
            }

            m_Command = command;
            try
            {
                ChuanHoaLenh(command);

                m_Command.Connection = m_Connection;

                m_DataAdapter = new OleDbDataAdapter();
                m_DataAdapter.SelectCommand = m_Command;

                this.Clear();
                m_DataAdapter.Fill(this);

            }
            catch (Exception e)
            {
                String str = e.Message;
                m_LastError = str;
                GhiLog("Load", (command == null) ? "" : command.CommandText, str);
            }
        }


        public static bool OpenConnection()
        {
            try
            {
                if (m_Connection == null)
                    m_Connection = new OleDbConnection(m_ConnectString);

                if (m_Connection.State == ConnectionState.Broken)
                    m_Connection.Close();

                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.Open();
                    // Ghi may chu / CSDL dang dung de doi chieu voi SSMS (moi lan mo ket noi)
                    GhiLog("KetNoi", "", "May chu: " + m_Connection.DataSource + " | CSDL: " + m_Connection.Database);
                }

                m_LoiKetNoi = "";
                return true;
            }
            catch (Exception e)
            {
                m_LoiKetNoi = e.Message;
                GhiLog("OpenConnection", m_ConnectString, e.Message);
                try { if (m_Connection != null) m_Connection.Close(); }
                catch (Exception) { }
                return false;
            }

        }
        /// <summary>
        /// Closes the connection of this data service.
        /// </summary>
        public void CloseConnection()
        {
            m_Connection.Close();
        }

        /// <summary>
        /// Update DataTable
        /// </summary>
        /// <returns></returns>
        public int ExecuteNoneQuery()
        {
            int result = 0;
            OleDbTransaction tr = null;
            m_LastError = "";
            try
            {
                tr = m_Connection.BeginTransaction();

                ChuanHoaLenh(m_Command);
                m_Command.Connection = m_Connection;
                m_Command.Transaction = tr;

                m_DataAdapter = new OleDbDataAdapter();
                m_DataAdapter.SelectCommand = m_Command;

                OleDbCommandBuilder builder = new OleDbCommandBuilder(m_DataAdapter);

                result = m_DataAdapter.Update(this);


                tr.Commit();

            }
            catch (Exception e)
            {
                if (tr != null) tr.Rollback();
                m_LastError = e.Message;
                GhiLog("ExecuteNoneQuery(Update)", (m_Command == null) ? "" : m_Command.CommandText, e.Message);
            }
            return result;
        }
        /// <summary>
        /// Thuc thi mot command
        /// </summary>
        /// <param name="command">OleDb hay Store Procedure</param>
        /// <returns></returns>
        public int ExecuteNoneQuery(OleDbCommand cmd)
        {

            int result = 0;
            OleDbTransaction tr = null;

            try
            {
                tr = m_Connection.BeginTransaction();

                ChuanHoaLenh(cmd);
                cmd.Connection = m_Connection;

                cmd.Transaction = tr;

                result = cmd.ExecuteNonQuery();

                this.AcceptChanges();

                tr.Commit();

            }
            catch (Exception e)
            {
                if (tr != null) tr.Rollback();
                GhiLog("ExecuteNoneQuery(cmd)", (cmd == null) ? "" : cmd.CommandText, e.Message);
                throw;
            }
            return result;

        }

        public object ExecuteScalar(OleDbCommand cmd)
        {
            object result = null;
            OleDbTransaction tr = null;

            try
            {
                tr = m_Connection.BeginTransaction();

                ChuanHoaLenh(cmd);
                cmd.Connection = m_Connection;

                cmd.Transaction = tr;

                result = cmd.ExecuteScalar();

                this.AcceptChanges();

                tr.Commit();

                if (result == DBNull.Value)
                {
                    result = null;
                }
            }
            catch (Exception e)
            {
                if (tr != null) tr.Rollback();
                GhiLog("ExecuteScalar", (cmd == null) ? "" : cmd.CommandText, e.Message);
                throw;
            }
            return result;
        }

    }
}