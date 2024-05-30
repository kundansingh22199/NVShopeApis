using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using NVShopeApi.ClassFiles;

namespace MetaMinesAPI.ClassFiles
{
    public class DataUtility:ClsConnection
    {
        #region All Constructor
        /// <summary>
        /// Default or no argument constructor
        /// </summary>
        public DataUtility()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        public DataUtility(string ConnectionString)
        {
            _mCon = new SqlConnection(ConnectionString);
        }
        #endregion
        #region All Properties
        private SqlConnection _mCon;

        public SqlConnection mCon
        {
            get { return _mCon; }
            set { _mCon = value; }
        }
        private SqlCommand _mDatacom;

        public SqlCommand mDatacom
        {
            get { return _mDatacom; }
            set { _mDatacom = value; }
        }
        private SqlDataAdapter _mDa;

        public SqlDataAdapter MyProperty
        {
            get { return _mDa; }
            set { _mDa = value; }
        }



        #endregion
        #region All Private Methods
        /// <summary>
        /// This is to open connection with SQL DB
        /// initlize command object and set active connection with it
        /// </summary>
        private void OpenConnection()
        {
            if (_mCon == null)
            {
                _mCon = new SqlConnection(str);
            }
            if (_mCon.State == ConnectionState.Closed)
            {
                _mCon.Open();
                _mDatacom = new SqlCommand();
                _mDatacom.Connection = _mCon;
            }
        }
        /// <summary>
        /// This is to close an active connection
        /// </summary>
        private void CloseConnection()
        {
            if (_mCon.State == ConnectionState.Open)
            {
                _mCon.Close();
            }
        }
        /// <summary>
        /// This is to relese connection
        /// </summary>
        private void DisposeConnection()
        {
            if (_mCon != null)
            {
                _mCon.Dispose();
                _mCon = null;
            }
        }

        #endregion
        #region All Public Methods
        /// <summary>
        /// This is to execute all DML using SQL as text
        /// </summary>
        /// <param name="strsql"> String, SQL query </param>
        /// <returns> int, No of rows affected </returns>
        public int ExecuteSql(String strsql)
        {
            int Result = 0;
            try
            {
                OpenConnection();
                // Set command object properties
                _mDatacom.CommandType = CommandType.Text;
                _mDatacom.CommandText = strsql;
                _mDatacom.CommandTimeout = 10000;

                // Call the methods
                Result = _mDatacom.ExecuteNonQuery();
                CloseConnection();
                DisposeConnection();
                return Result;
            }
            catch (Exception ex)
            {
                return Result;
            }
        }

        /// <summary>
        /// This is to execute all DML using SQL as text
        /// </summary>
        /// <param name="strsql"> String, SQL Procedure </param>
        /// <returns> int, No of rows affected </returns>
        public int ExecuteSqlProc(String strsql, SqlParameter[] pram)
        {
            int Result = 0;
            try
            {
                OpenConnection();
                // Set command object properties
                _mDatacom.CommandType = CommandType.StoredProcedure;
                _mDatacom.CommandText = strsql;
                _mDatacom.Parameters.AddRange(pram);
                _mDatacom.CommandTimeout = 10000;

                // Call the methods
                Result = _mDatacom.ExecuteNonQuery();
                CloseConnection();
                DisposeConnection();
                return Result;
            }
            catch (Exception ex)
            {
                return Result;
            }
        }
        /// <summary>
        /// This is to check if  values is present in table
        /// </summary>
        /// <param name="strsql"> String, SQL query </param>
        /// <returns> bool </returns>
        public bool IsExit(String strsql)
        {
            try
            {
                OpenConnection();
                // Set command object properties
                _mDatacom.CommandType = CommandType.Text;
                _mDatacom.CommandText = strsql;
                _mDatacom.CommandTimeout = 10000;

                // Call the methods
                int Result = (int)_mDatacom.ExecuteScalar();
                CloseConnection();
                DisposeConnection();
                if (Result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// This is to check if  values is present in table
        /// </summary>
        /// <param name="strsql"> String, Store Procedure </param>
        /// <returns> 
        /// bool </returns>
        public bool IsExitProc(String strsql, SqlParameter[] pram)
        {
            try
            {
                OpenConnection();
                // Set command object properties
                _mDatacom.CommandType = CommandType.StoredProcedure;
                _mDatacom.CommandText = strsql;
                _mDatacom.Parameters.AddRange(pram);
                _mDatacom.CommandTimeout = 10000;

                // Call the methods
                int Result = (int)_mDatacom.ExecuteScalar();
                CloseConnection();
                DisposeConnection();
                if (Result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// This is return First value present in table
        /// </summary>
        /// <param name="strsql"> String, Store Procedure </param>
        /// <returns> bool </returns>
        public string SqlScalerProc(String strsql, SqlParameter[] pram)
        {
            try
            {
                OpenConnection();
                // Set command object properties
                _mDatacom.CommandType = CommandType.StoredProcedure;
                _mDatacom.CommandText = strsql;
                _mDatacom.Parameters.AddRange(pram);
                _mDatacom.CommandTimeout = 10000;

                // Call the methods
                string Result = Convert.ToString(_mDatacom.ExecuteScalar());
                CloseConnection();
                DisposeConnection();
                return Result;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                vircon();
            }
        }

        public string SqlScaler(String strsql)
        {
            try
            {
                OpenConnection();
                // Set command object properties
                _mDatacom.CommandType = CommandType.Text;
                _mDatacom.CommandText = strsql;
                _mDatacom.CommandTimeout = 10000;

                // Call the methods
                string Result = Convert.ToString(_mDatacom.ExecuteScalar());
                CloseConnection();
                DisposeConnection();
                return Result;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                vircon();
            }
        }



        /// <summary>
        /// This is to create DQL statement in disconnected mode
        /// using SQL as Text
        /// </summary>
        /// <param name="strsql"> String </param>
        /// <returns> DataTable </returns>
        public DataTable GetDataTable(String strsql)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
                _mDatacom.CommandType = CommandType.Text;
                _mDatacom.CommandText = strsql;
                _mDatacom.CommandTimeout = 10000;
                _mDa = new SqlDataAdapter();

                // Asigns command object into DataAdapter
                _mDa.SelectCommand = _mDatacom;

                // Trasnfer the data into database
                _mDa.Fill(dt);
                CloseConnection();
                DisposeConnection();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// This is to create DQL statement in disconnected mode
        /// using SQL as Store Procedure
        /// </summary>
        /// <param name="strsql"> String </param>
        /// <returns> DataTable </returns>
        /// 



        public DataTable GetDataTableProc(String strsql, SqlParameter[] pram)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
                _mDatacom.CommandType = CommandType.StoredProcedure;
                _mDatacom.CommandText = strsql;
                _mDatacom.Parameters.AddRange(pram);
                _mDatacom.CommandTimeout = 10000;
                _mDa = new SqlDataAdapter();

                // Asigns command object into DataAdapter
                _mDa.SelectCommand = _mDatacom;

                // Trasnfer the data into database
                _mDa.Fill(dt);
                CloseConnection();
                DisposeConnection();
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// This is to create DQL statement in disconnected mode
        /// using SQL as Text
        /// </summary>
        /// <param name="strsql"> String </param>
        /// <returns> DataSet </returns>
        public DataSet GetDataSet(string strsql)
        {
            DataSet ds = new DataSet();
            try
            {
                OpenConnection();
                _mDatacom.CommandType = CommandType.Text;
                _mDatacom.CommandText = strsql;
                _mDatacom.CommandTimeout = 10000;
                _mDa = new SqlDataAdapter();

                // Asigns command object into DataAdapter
                _mDa.SelectCommand = _mDatacom;

                // Trasnfer the data into database
                _mDa.Fill(ds);
                CloseConnection();
                DisposeConnection();
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// This is to create DQL statement in disconnected mode
        /// using SQL as Store Procedure
        /// </summary>
        /// <param name="strsql"> String </param>
        /// <returns> DataSet </returns>
        public DataSet GetDataSetProc(string strsql, SqlParameter[] pram)
        {
            DataSet ds = new DataSet();
            try
            {
                OpenConnection();
                _mDatacom.CommandType = CommandType.StoredProcedure;
                _mDatacom.CommandText = strsql;
                _mDatacom.Parameters.AddRange(pram);
                _mDatacom.CommandTimeout = 10000;
                _mDa = new SqlDataAdapter();

                // Asigns command object into DataAdapter
                _mDa.SelectCommand = _mDatacom;

                // Trasnfer the data into database
                _mDa.Fill(ds);
                CloseConnection();
                DisposeConnection();
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// This is to create DML statement in Connected mode
        /// using SQL as Text
        /// </summary>
        /// <param name="strsql"> String </param>
        /// <returns> DataReader </returns>
        public SqlDataReader GetDataReader(String strsql)
        {
            try
            {
                OpenConnection();
                SqlDataReader dReader;
                _mDatacom.CommandType = CommandType.Text;
                _mDatacom.CommandText = strsql;
                dReader = _mDatacom.ExecuteReader(CommandBehavior.CloseConnection);
                return dReader;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// This is to create DML statement in Connected mode
        /// using SQL as Store Procedure
        /// </summary>
        /// <param name="strsql"> String </param>
        /// <returns> DataReader </returns>
        public SqlDataReader GetDataReaderProc(String strsql, SqlParameter[] pram)
        {
            try
            {
                OpenConnection();
                SqlDataReader dReader;
                _mDatacom.CommandType = CommandType.StoredProcedure;
                _mDatacom.CommandText = strsql;
                _mDatacom.Parameters.AddRange(pram);
                dReader = _mDatacom.ExecuteReader(CommandBehavior.CloseConnection);
                return dReader;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //public string ShowMsg(GlobalEnume.MsgType msg)
        //{
        //    switch (msg)
        //    {
        //        case  GlobalEnume.MsgType.INSERT: return (ConfigurationManager.AppSettings["INSERT"].ToString());
        //        default: return (ConfigurationManager.AppSettings["ERROR"].ToString());

        //    }
        //}

        public void vircon()
        {
            if (_mCon != null)
            {
                _mCon.Close();
            }
        }

        #endregion
    }
}