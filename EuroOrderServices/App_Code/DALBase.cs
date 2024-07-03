using System;
using System.Data;
using System.Configuration;

using System.Data.SqlClient;

using System.Collections;

/// <summary>
/// Summary description for DALBase
/// </summary>
public class DALBase
{
    /*** PRIVATE FIELDS ***/

    private string _connectionString;
    private SqlTransaction sqlTransaction =null;
    private SqlConnection conn = null;
    private SqlCommand sqlcommand = null;
    
       

    /*** PROPERTIES ***/
    

    public string ConnectionString
    {
        get
        {
            string _connectionString =  Convert.ToString(ConfigurationManager.AppSettings["ConnectionString"]);
            //string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString().ToString();
            return _connectionString;

        }
        set { _connectionString = value; }
    }

    private SqlCommand CreateCommand()
    {
        conn = new SqlConnection(ConnectionString);
        sqlcommand = new SqlCommand();
        sqlcommand.Connection = conn;
        sqlcommand.CommandTimeout = 0;
        return sqlcommand;    
    }

	public DALBase()
	{
       
	}
    private void RemoveEscpe(ref SqlParameter[] parmas)
    {
        try
        {
            if (parmas.Length > 0)
            {
                for (int i = 0; i < parmas.Length; i++)
                {
                    if (parmas[i].DbType == DbType.String && parmas[i].Value != null && !parmas[i].Value.ToString().Equals(""))
                        parmas[i].Value = parmas[i].Value.ToString().Trim().Replace("'", "''");
                    else if(parmas[i].DbType == DbType.String && (parmas[i].Value == null || parmas[i].Value.ToString().Equals("")))
                        parmas[i].Value = "";
                }
            }
        }
        catch
        { }
    
    }
    private void ReverseEscape(ref DataTable dt)
    {
        try
        {
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j< dt.Columns.Count; j++)
                    {
                        if (!dt.Rows[i][j].ToString().Equals("") && dt.Rows[i][j] != null)
                        {
                            dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace("''", "'").Trim();
                        }

                    }
                }
            }
        }
        catch
        {
        }
    }

    protected bool Execute_NonQuery(string spname, SqlParameter[] parmas)
    {
        SqlCommand command = this.CreateCommand();
        RemoveEscpe(ref parmas);
        command.CommandType = CommandType.StoredProcedure;
        command.Transaction = sqlTransaction;
        try
        {
            command.Connection.Open();
            command.Connection = conn;
            command.CommandText = spname;
            command.Parameters.AddRange(parmas);
            command.ExecuteNonQuery();
            return true;
        }
        catch(Exception ex)
        {
            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.INSERT_ErrorLog(ex, "Execute_NonQuery");

            this.sqlTransaction.Rollback();
            return false;
        }
        finally
        {
            if (command.Connection.State == ConnectionState.Open)
                command.Connection.Close();
            command = null;
        }
    }
    protected bool Execute_NonQuery(string spname, SqlParameter[] parmas, ref string ReturnID, string ReturnParameter)
    {
        SqlCommand command = this.CreateCommand();
        RemoveEscpe(ref parmas);
        command.CommandType = CommandType.StoredProcedure;
        command.Transaction = sqlTransaction;
        try
        {
            command.Connection.Open();
            command.Connection = conn;
            command.CommandText = spname;
            command.Parameters.AddRange(parmas);
            command.ExecuteNonQuery();
            ReturnID = command.Parameters[ReturnParameter].Value.ToString();
            return true;
        }
        catch (Exception ex)
        {
            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.INSERT_ErrorLog(ex, "Execute_NonQuery 2");
            this.sqlTransaction.Rollback();
            throw;

        }
        finally
        {
            if (command.Connection.State == ConnectionState.Open)
                command.Connection.Close();
            command = null;
        }
    }
    protected bool Execute_NonQuery(string sql)
    {
        SqlCommand cmd = this.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.Transaction = sqlTransaction;
        try
        {            
            cmd.Connection.Open();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.INSERT_ErrorLog(ex, "Execute_NonQuery 3");

            this.sqlTransaction.Rollback();   
            throw;

        }
        finally
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            cmd = null;
            conn = null;
        }
    }


    protected bool Get_Records_SQL(string sql,ref DataTable dt)
    {
        SqlCommand cmd = this.CreateCommand();
        SqlDataAdapter sqlDataAdap = new SqlDataAdapter();
        cmd.CommandType = CommandType.Text;
        cmd.Transaction = sqlTransaction;
        try
        {
            cmd.Connection.Open();
            cmd.CommandText = sql;
            sqlDataAdap.SelectCommand = cmd;
            sqlDataAdap.Fill(dt);

            if ( dt != null && dt.Rows.Count > 0)
            {
                ReverseEscape(ref dt);
            }
            else
            {
                dt = null;
            }
            return true;
        }
        catch (Exception ex)
        {
            this.sqlTransaction.Rollback();
            throw;
        }
        finally
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            cmd = null;
            conn = null;
        }
    }

    protected bool Get_RecordsDataset(string spname, SqlParameter[] parmas, ref DataSet ds)
    {
        SqlCommand cmd = this.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter adpter = new SqlDataAdapter();
        try
        {
            cmd.Connection.Open();
            cmd.CommandText = spname;
            cmd.Parameters.AddRange(parmas);
            adpter.SelectCommand = cmd;
            adpter.Fill(ds);

            return true;
        }
        catch (Exception ex)
        {
            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.INSERT_ErrorLog(ex, "Get_RecordsDataset");
            return false;
        }
        finally
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            cmd = null;
            conn = null;
        }
    }


    protected  bool Get_Records(string spname, SqlParameter[] parmas, ref DataTable dt)
    {
        SqlCommand cmd = this.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter adpter = new SqlDataAdapter();
        DataSet ds = new DataSet();
        try
        {
            cmd.Connection.Open();
            cmd.CommandText = spname;
            cmd.Parameters.AddRange(parmas);
            adpter.SelectCommand = cmd;
            adpter.Fill(ds);

            if (ds.Tables.Count != 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
                ReverseEscape(ref dt);
            }
            else
            {
                dt = null;
            }

            return true;
        }
        catch (Exception ex)
        {
            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.INSERT_ErrorLog(ex, "Get_Records");
            return false;
        }
        finally
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            cmd = null;
            conn = null;
        }
    }

    protected  bool Get_RecordsExisting(string spname, SqlParameter[] parmas, ref DataTable dt)
    {
        SqlCommand cmd = this.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter adpter = new SqlDataAdapter();
        DataSet ds = new DataSet();
        try
        {
            cmd.Connection.Open();
            cmd.CommandText = spname;
            cmd.Parameters.AddRange(parmas);
            adpter.SelectCommand = cmd;
            adpter.Fill(ds);

            if (ds.Tables.Count != 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
                ReverseEscape(ref dt);
            }
            else
            {
                dt = null;
            }

            return true;
        }
        catch (Exception ex)
        {
            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.INSERT_ErrorLog(ex, "Get_RecordsExisting");
            return false;
        }
        finally
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            cmd = null;
            conn = null;
        }
    }
    protected bool Get_Records(string spname, ref DataTable dt)
    {
        SqlCommand cmd = this.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter adpter = new SqlDataAdapter();
        DataSet ds = new DataSet();
        try
        {
            cmd.Connection.Open();
            cmd.CommandText = spname;
            adpter.SelectCommand = cmd;
            adpter.Fill(ds);
            if (ds.Tables.Count != 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
                ReverseEscape(ref dt);
            }
            else
                dt = null;
            return true;
        }
        catch(Exception ex)
        {
            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.INSERT_ErrorLog(ex, "Get_Records 2");
            return false;
        }
        finally
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            cmd = null;
            conn = null;
        }
    }
    protected object Execute_Scalar(string spname, SqlParameter[] parmas)
    {
        SqlCommand cmd = this.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            cmd.Connection.Open();
            cmd.CommandText = spname;
            cmd.Parameters.AddRange(parmas);
            return cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.INSERT_ErrorLog(ex, "Execute_Scalar");

            return (object)"";

        }
        finally
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            cmd = null;
            conn = null;
        }
    }
  
   

}
