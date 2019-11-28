using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace Service.Services
{
    public class DapperHelper
    {

        public static string ConnectionString { get; set; }

        private IDbConnection _dbConnection = null;
        private IDbConnection DbConnection
        {
            get
            {
                if (_dbConnection == null)
                {
                    _dbConnection = new MySqlConnection(ConnectionString);
                }
                //判断连接状态
                if (_dbConnection.State == ConnectionState.Closed)
                {
                    _dbConnection.Open();
                }
                return _dbConnection;
            }
        }

        public void CloseDbConnection()
        {
            if (_dbConnection?.State == ConnectionState.Open) _dbConnection.Close();
        }

        public void DisposeObj()
        {
            DbConnection?.Close();
            DbConnection?.Dispose();
        }


        public void Insert<T>(T entity) where T : class
        {
            using (var dbconnection = new MySqlConnection(ConnectionString))
            {
                dbconnection.Open();
                dbconnection.Insert(entity);
            }
        }

        public void Insert<T>(List<T> entitys) where T : class
        {
            using (var dbconnection = new MySqlConnection(ConnectionString))
            {
                dbconnection.Open();
                dbconnection.Insert(entitys);
            }
        }

        public void Update<T>(T entity) where T : class
        {
            using (var dbconnection = new MySqlConnection(ConnectionString))
            {
                dbconnection.Open();
                dbconnection.Update(entity);
            }
        }

        public T Get<T>(string sql, object param = null) where T : class
        {
            using (var dbconnection = new MySqlConnection(ConnectionString))
            {
                dbconnection.Open();
                var entity = dbconnection.QueryFirstOrDefault<T>(sql, param);
                return entity;
            }
        }

        public List<T> GetList<T>(string sql, object param = null) where T : class
        {
            using (var dbconnection = new MySqlConnection(ConnectionString))
            {
                dbconnection.Open();
                var list = dbconnection.Query<T>(sql, param);
                return list.ToList();
            }
        }

        public void QueryMultiple(string sql, Action<SqlMapper.GridReader> action, object param = null)
        {
            using (var dbconnection = new MySqlConnection(ConnectionString))
            {
                dbconnection.Open();
                var reader = dbconnection.QueryMultiple(sql, param);
                action(reader);
                reader.Dispose();
            }
        }
    }
}
