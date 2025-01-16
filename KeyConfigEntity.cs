using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace ScreenTranslate
{

    [SugarTable("key_config")]
    public class KeyConfigEntity
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]//数据库与实体不一样设置列名 
        public int Id { get; set; }

        [SugarColumn(ColumnName = "access_key")]//数据库与实体不一样设置列名 
        public string AccessKey { get; set; }

        [SugarColumn(ColumnName = "secret_key")]//数据库与实体不一样设置列名 
        public string SecretKey { get; set; }

        [SugarColumn(ColumnName = "qwen_token")]//数据库与实体不一样设置列名 
        public string QwenToken { get; set; }

        [SugarColumn(ColumnName = "create_time", InsertServerTime=true)]//数据库与实体不一样设置列名 
        public string CreateTime { get; set; }

        [SugarColumn(ColumnName = "update_time", UpdateServerTime = true)]//数据库与实体不一样设置列名 
        public string UpdateTime { get; set; }
    }
}
