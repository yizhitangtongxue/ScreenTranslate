using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace ScreenTranslate
{
    internal class SqlSugarHelper
    {
        

        public static SqlSugarClient GetSugar()
        {
            //创建数据库对象 (用法和EF Dappper一样通过new保证线程安全)
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = $"datasource=st.db",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
            },
            db => {

                db.Aop.OnLogExecuting = (sql, pars) =>
                {

                    //获取原生SQL推荐 5.1.4.63  性能OK
                    Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

                    //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                    //Console.WriteLine(UtilMethods.GetSqlString(DbType.SqlServer,sql,pars))


                };

                //注意多租户 有几个设置几个
                //db.GetConnection(i).Aop

            });

            return Db;

        }
    }
}
