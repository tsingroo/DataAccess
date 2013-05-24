using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess {
    /// <summary>
    /// 实体类
    /// </summary>
    public class daEntity : Dictionary<string, string> { }
    
    /// <summary>
    /// 表逻辑
    /// </summary>
    public class daTable {
        private string _ConnectionString = null;
        private string _CurrentTable = null;
        private string _PrimaryKey = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="conn">链接字串</param>
        /// <param name="tableName">表名</param>
        /// <param name="pkColumn">主键名</param>
        public daTable( string conn,string tableName,string pkColumn) {
            _ConnectionString = conn;
            _CurrentTable = tableName;
            _PrimaryKey = pkColumn;
        }

        /// <summary>
        /// 插入(MsSql2005及以下版本使用此方法)
        /// </summary>
        /// <param name="enList"></param>
        /// <returns></returns>
        public int MsSql2005Insert(List<daEntity> enList) {

            return -1;
        }

        /// <summary>
        /// 插入(MsSql2008及以上版本使用此方法)
        /// </summary>
        /// <param name="enList"></param>
        /// <returns></returns>
        public int Insert(List<daEntity> enList) {
            StringBuilder insertSqlBuilder ;
            int enCount = enList.Count;
            if (null == enList || enCount <= 0) {
                throw new Exception("泛型集合不能为空!");
            }
            string[] columnNames = _GetColumns(enList[0]);
            insertSqlBuilder = new StringBuilder(" INSERT INTO " + _CurrentTable + " (");
            int columnCount = columnNames.Length;
            for (int i = 0; i < columnCount; ++i) {
                insertSqlBuilder.Append(" "+columnNames[i]+",");
            }
            insertSqlBuilder.Remove(insertSqlBuilder.Length - 1, 1).Append(") VALUES ");
            for (int i = 0; i < enCount; ++i) {
                string[] columnValues = _GetValues(enList[i]);
                int valuesCount = columnValues.Length;
                insertSqlBuilder.Append(" (");
                for (int j = 0; j < valuesCount; ++j) {
                    insertSqlBuilder.Append(" '"+columnValues[j]+"',");
                }
                insertSqlBuilder.Remove(insertSqlBuilder.Length - 1, 1).AppendLine("),");
            }
            insertSqlBuilder.Remove(insertSqlBuilder.Length - 1, 1).AppendLine(";");

            int insertedRows = SqlHelper.ExecuteNonQuery(_ConnectionString, CommandType.Text, insertSqlBuilder.ToString());
            
            return insertedRows;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(daEntity entity) {
            if (null == entity || 0 == entity.Count) {
                throw new Exception("实体类不能为空！");
            }
            string[] columnNames = _GetColumns(entity);
            string[] columnValiues = _GetValues(entity);
            StringBuilder deleteSql = new StringBuilder(" DELETE " + _CurrentTable +" WHERE ");
            int columnCount = columnNames.Length;
            for (int i = 0; i < columnCount; ++i) {
                deleteSql.Append(" " + columnNames[i] + "='" + columnValiues[i] + "',");
            }
            deleteSql.Remove(deleteSql.Length - 1, 1);

            int deletedRows = SqlHelper.ExecuteNonQuery(_ConnectionString, CommandType.Text, deleteSql.ToString());

            return deletedRows;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(daEntity entity) {
            if (null == entity || 0 == entity.Count) {
                throw new Exception("实体类不能为空！");
            }
            string[] columnNames = _GetColumns(entity);
            string[] columnValiues = _GetValues(entity);
            int columnCount = columnNames.Length;
            StringBuilder updateSql = new StringBuilder(" UPDATE " + _CurrentTable + " SET ");
            for (int i = 0; i < columnCount; ++i) {
                if (_PrimaryKey == columnNames[i]) {
                    continue;
                } else {
                    updateSql.Append(" " + columnNames[i] + "='" + columnValiues[i] + "',");
                }
            }
            updateSql.Remove(updateSql.Length - 1, 1);
            updateSql.Append(" WHERE " + _PrimaryKey + "='" + entity[_PrimaryKey] + "' ");

            int updatedRows = SqlHelper.ExecuteNonQuery(_ConnectionString, CommandType.Text, updateSql.ToString());

            return updatedRows;
        }

        /// <summary>
        /// 获取全表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTable() {
            string getSql = "SELECT * FROM " + _CurrentTable;
            DataTable dt = SqlHelper.ExecuteDataTable(_ConnectionString, CommandType.Text, getSql);
            
            return dt;
        }

        /// <summary>
        /// 根据条件获取指定的列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public SqlDataReader GetRows(daEntity entity,string[] columns) {
            if (null == entity || 0 == entity.Count) {
                throw new Exception("实体类不能为空！");
            }
            string[] columnNames = _GetColumns(entity);
            string[] columnValiues = _GetValues(entity);
            int columnCount = columnNames.Length;
            StringBuilder queryColumns = new StringBuilder("");
            int queryColumnCount = columns.Length;
            for (int i = 0; i < queryColumnCount; ++i) {
                queryColumns.Append(" "+columns[i]+",");
            }
            queryColumns.Remove(queryColumns.Length - 1, 1);
            StringBuilder getSql = new StringBuilder("SELECT " + queryColumns.ToString() + " FROM ");
            getSql.Append(_CurrentTable+" WHERE ");
            for (int i = 0; i < columnCount; ++i) {
                getSql.Append(" " + columnNames[i] + "='" + columnValiues[i] + "' AND");
            }
            getSql.Remove(getSql.Length - 3, 3);


            return SqlHelper.ExecuteReader(_ConnectionString, CommandType.Text, getSql.ToString());
        }

        public int GetMaxPK() {

            return -1;
        }

        /// <summary>
        /// 统计符合条件的有多少行
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int CountRows(daEntity entity) {
            if (null == entity || 0 == entity.Count) {
                throw new Exception("实体类不能为空！");
            }
            string[] columnNames = _GetColumns(entity);
            string[] columnValues = _GetValues(entity);
            int columnCount = columnNames.Length;
            StringBuilder countSql = new StringBuilder("SELECT COUNT(1) FROM "+ _CurrentTable +" WHERE ");
            for (int i = 0; i < columnCount; ++i) {
                countSql.Append(" " + columnNames[i] + "='" + columnValues[i] + "' AND");
            }
            countSql.Remove(columnValues.Length - 3, 3);

            return (int)SqlHelper.ExecuteScalar(_ConnectionString, CommandType.Text, countSql.ToString());
        }


        #region 类内私有公共方法

        /// <summary>
        /// 获取实体类的列名
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        private string[] _GetColumns(daEntity entity) {
            StringBuilder keysbuilder = new StringBuilder("");
            foreach (string key in entity.Keys) {
                keysbuilder.Append(key + ",");
            }

            return keysbuilder.Remove(keysbuilder.Length - 1, 1).ToString().Split(',');
        }

        /// <summary>
        /// 获取实体对象的列值
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        private string[] _GetValues(daEntity entity) {
            StringBuilder valuesbuilder = new StringBuilder("");
            foreach (string key in entity.Keys) {
                valuesbuilder.Append(entity[key] + ",");
            }

            return valuesbuilder.Remove(valuesbuilder.Length - 1, 1).ToString().Split(',');
        }


        #endregion


    }
}
