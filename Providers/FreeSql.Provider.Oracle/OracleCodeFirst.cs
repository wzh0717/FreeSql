﻿using FreeSql.DataAnnotations;
using FreeSql.DatabaseModel;
using FreeSql.Internal;
using FreeSql.Internal.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FreeSql.Oracle
{

    class OracleCodeFirst : Internal.CommonProvider.CodeFirstProvider
    {

        public OracleCodeFirst(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression) : base(orm, commonUtils, commonExpression) { }

        static object _dicCsToDbLock = new object();
        static Dictionary<string, (OracleDbType type, string dbtype, string dbtypeFull, bool? isUnsigned, bool? isnullable, object defaultValue)> _dicCsToDb = new Dictionary<string, (OracleDbType type, string dbtype, string dbtypeFull, bool? isUnsigned, bool? isnullable, object defaultValue)>() {
                { typeof(bool).FullName,  (OracleDbType.Boolean, "number","number(1) NOT NULL", null, false, false) },{ typeof(bool?).FullName,  (OracleDbType.Boolean, "number","number(1) NULL", null, true, null) },

                { typeof(sbyte).FullName,  (OracleDbType.Decimal, "number", "number(4) NOT NULL", false, false, 0) },{ typeof(sbyte?).FullName,  (OracleDbType.Decimal, "number", "number(4) NULL", false, true, null) },
                { typeof(short).FullName,  (OracleDbType.Int16, "number","number(6) NOT NULL", false, false, 0) },{ typeof(short?).FullName,  (OracleDbType.Int16, "number", "number(6) NULL", false, true, null) },
                { typeof(int).FullName,  (OracleDbType.Int32, "number", "number(11) NOT NULL", false, false, 0) },{ typeof(int?).FullName,  (OracleDbType.Int32, "number", "number(11) NULL", false, true, null) },
                { typeof(long).FullName,  (OracleDbType.Int64, "number","number(21) NOT NULL", false, false, 0) },{ typeof(long?).FullName,  (OracleDbType.Int64, "number","number(21) NULL", false, true, null) },

                { typeof(byte).FullName,  (OracleDbType.Byte, "number","number(3) NOT NULL", true, false, 0) },{ typeof(byte?).FullName,  (OracleDbType.Byte, "number","number(3) NULL", true, true, null) },
                { typeof(ushort).FullName,  (OracleDbType.Decimal, "number","number(5) NOT NULL", true, false, 0) },{ typeof(ushort?).FullName,  (OracleDbType.Decimal, "number", "number(5) NULL", true, true, null) },
                { typeof(uint).FullName,  (OracleDbType.Decimal, "number", "number(10) NOT NULL", true, false, 0) },{ typeof(uint?).FullName,  (OracleDbType.Decimal, "number", "number(10) NULL", true, true, null) },
                { typeof(ulong).FullName,  (OracleDbType.Decimal, "number", "number(20) NOT NULL", true, false, 0) },{ typeof(ulong?).FullName,  (OracleDbType.Decimal, "number", "number(20) NULL", true, true, null) },

                { typeof(double).FullName,  (OracleDbType.Double, "float", "float(126) NOT NULL", false, false, 0) },{ typeof(double?).FullName,  (OracleDbType.Double, "float", "float(126) NULL", false, true, null) },
                { typeof(float).FullName,  (OracleDbType.Single, "float","float(63) NOT NULL", false, false, 0) },{ typeof(float?).FullName,  (OracleDbType.Single, "float","float(63) NULL", false, true, null) },
                { typeof(decimal).FullName,  (OracleDbType.Decimal, "number", "number(10,2) NOT NULL", false, false, 0) },{ typeof(decimal?).FullName,  (OracleDbType.Decimal, "number", "number(10,2) NULL", false, true, null) },

                { typeof(TimeSpan).FullName,  (OracleDbType.IntervalDS, "interval day to second","interval day(2) to second(6) NOT NULL", false, false, 0) },{ typeof(TimeSpan?).FullName,  (OracleDbType.IntervalDS, "interval day to second", "interval day(2) to second(6) NULL",false, true, null) },
                { typeof(DateTime).FullName,  (OracleDbType.TimeStamp, "timestamp", "timestamp(6) NOT NULL", false, false, new DateTime(1970,1,1)) },{ typeof(DateTime?).FullName,  (OracleDbType.TimeStamp, "timestamp", "timestamp(6) NULL", false, true, null) },
                { typeof(DateTimeOffset).FullName,  (OracleDbType.TimeStampLTZ, "timestamp with local time zone", "timestamp(6) with local time zone NOT NULL", false, false, new DateTime(1970,1,1)) },{ typeof(DateTimeOffset?).FullName,  (OracleDbType.TimeStampLTZ, "timestamp with local time zone", "timestamp(6) with local time zone NULL", false, true, null) },

                { typeof(byte[]).FullName,  (OracleDbType.Blob, "blob", "blob NULL", false, null, new byte[0]) },
                { typeof(string).FullName,  (OracleDbType.NVarchar2, "nvarchar2", "nvarchar2(255) NULL", false, null, "") },

                { typeof(Guid).FullName,  (OracleDbType.Char, "char", "char(36 CHAR) NOT NULL", false, false, Guid.Empty) },{ typeof(Guid?).FullName,  (OracleDbType.Char, "char", "char(36 CHAR) NULL", false, true, null) },
            };

        public override (int type, string dbtype, string dbtypeFull, bool? isnullable, object defaultValue)? GetDbInfo(Type type)
        {
            if (_dicCsToDb.TryGetValue(type.FullName, out var trydc)) return new (int, string, string, bool?, object)?(((int)trydc.type, trydc.dbtype, trydc.dbtypeFull, trydc.isnullable, trydc.defaultValue));
            if (type.IsArray) return null;
            var enumType = type.IsEnum ? type : null;
            if (enumType == null && type.IsNullableType())
            {
                var genericTypes = type.GetGenericArguments();
                if (genericTypes.Length == 1 && genericTypes.First().IsEnum) enumType = genericTypes.First();
            }
            if (enumType != null)
            {
                var newItem = enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Any() ?
                    (OracleDbType.Int32, "number", $"number(16){(type.IsEnum ? " NOT NULL" : "")}", false, type.IsEnum ? false : true, Enum.GetValues(enumType).GetValue(0)) :
                    (OracleDbType.Int64, "number", $"number(32){(type.IsEnum ? " NOT NULL" : "")}", false, type.IsEnum ? false : true, Enum.GetValues(enumType).GetValue(0));
                if (_dicCsToDb.ContainsKey(type.FullName) == false)
                {
                    lock (_dicCsToDbLock)
                    {
                        if (_dicCsToDb.ContainsKey(type.FullName) == false)
                            _dicCsToDb.Add(type.FullName, newItem);
                    }
                }
                return ((int)newItem.Item1, newItem.Item2, newItem.Item3, newItem.Item5, newItem.Item6);
            }
            return null;
        }

        public override string GetComparisonDDLStatements(params Type[] entityTypes)
        {
            var userId = (_orm.Ado.MasterPool as OracleConnectionPool).UserId;
            var seqcols = new List<(ColumnInfo, string[], bool)>(); //序列：列，表，自增
            var seqnameDel = new List<string>(); //要删除的序列+触发器

            var sb = new StringBuilder();
            var sbDeclare = new StringBuilder();
            foreach (var entityType in entityTypes)
            {
                if (sb.Length > 0) sb.Append("\r\n");
                var tb = _commonUtils.GetTableByEntity(entityType);
                if (tb == null) throw new Exception($"类型 {entityType.FullName} 不可迁移");
                if (tb.Columns.Any() == false) throw new Exception($"类型 {entityType.FullName} 不可迁移，可迁移属性0个");
                var tbname = tb.DbName.Split(new[] { '.' }, 2);
                if (tbname?.Length == 1) tbname = new[] { userId, tbname[0] };

                var tboldname = tb.DbOldName?.Split(new[] { '.' }, 2); //旧表名
                if (tboldname?.Length == 1) tboldname = new[] { userId, tboldname[0] };
                var primaryKeyName = (entityType.GetCustomAttributes(typeof(OraclePrimaryKeyNameAttribute), false)?.FirstOrDefault() as OraclePrimaryKeyNameAttribute)?.Name;

                if (string.Compare(tbname[0], userId) != 0 && _orm.Ado.ExecuteScalar(CommandType.Text, _commonUtils.FormatSql(" select 1 from sys.dba_users where username={0}", tbname[0])) == null) //创建数据库
                    throw new NotImplementedException($"Oracle CodeFirst 不支持代码创建 tablespace 与 schemas {tbname[0]}");

                var sbalter = new StringBuilder();
                var istmpatler = false; //创建临时表，导入数据，删除旧表，修改
                if (_orm.Ado.ExecuteScalar(CommandType.Text, _commonUtils.FormatSql(" select 1 from all_tab_comments where owner={0} and table_name={1}", tbname)) == null)
                { //表不存在
                    if (tboldname != null)
                    {
                        if (_orm.Ado.ExecuteScalar(CommandType.Text, _commonUtils.FormatSql(" select 1 from all_tab_comments where owner={0} and table_name={1}", tboldname)) == null)
                            //模式或表不存在
                            tboldname = null;
                    }
                    if (tboldname == null)
                    {
                        //创建表
                        var createTableName = _commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}");
                        sb.Append("execute immediate 'CREATE TABLE ").Append(createTableName).Append(" ( ");
                        foreach (var tbcol in tb.ColumnsByPosition)
                        {
                            sb.Append(" \r\n  ").Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append(" ").Append(tbcol.Attribute.DbType).Append(",");
                            if (tbcol.Attribute.IsIdentity == true) seqcols.Add((tbcol, tbname, true));
                        }
                        if (tb.Primarys.Any())
                        {
                            var pkname = primaryKeyName ?? $"{tbname[0]}_{tbname[1]}_pk1";
                            sb.Append(" \r\n  CONSTRAINT ").Append(pkname).Append(" PRIMARY KEY (");
                            foreach (var tbcol in tb.Primarys) sb.Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append(", ");
                            sb.Remove(sb.Length - 2, 2).Append("),");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("\r\n) \r\nLOGGING \r\nNOCOMPRESS \r\nNOCACHE\r\n';\r\n");
                        //创建表的索引
                        foreach (var uk in tb.Indexes)
                        {
                            sb.Append("execute immediate 'CREATE ");
                            if (uk.IsUnique) sb.Append("UNIQUE ");
                            sb.Append("INDEX ").Append(_commonUtils.QuoteSqlName(uk.Name)).Append(" ON ").Append(createTableName).Append("(");
                            foreach (var tbcol in uk.Columns)
                            {
                                sb.Append(_commonUtils.QuoteSqlName(tbcol.Column.Attribute.Name));
                                if (tbcol.IsDesc) sb.Append(" DESC");
                                sb.Append(", ");
                            }
                            sb.Remove(sb.Length - 2, 2).Append(")';\r\n");
                        }
                        //备注
                        foreach (var tbcol in tb.ColumnsByPosition)
                        {
                            if (string.IsNullOrEmpty(tbcol.Comment) == false)
                                sb.Append("execute immediate 'COMMENT ON COLUMN ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}.{tbcol.Attribute.Name}")).Append(" IS ").Append(_commonUtils.FormatSql("{0}", tbcol.Comment).Replace("'", "''")).Append("';\r\n");
                        }
                        continue;
                    }
                    //如果新表，旧表在一个模式下，直接修改表名
                    if (string.Compare(tbname[0], tboldname[0], true) == 0)
                        sbalter.Append("execute immediate 'ALTER TABLE ").Append(_commonUtils.QuoteSqlName($"{tboldname[0]}.{tboldname[1]}")).Append(" RENAME TO ").Append(_commonUtils.QuoteSqlName($"{tbname[1]}")).Append("';\r\n");
                    else
                    {
                        //如果新表，旧表不在一起，创建新表，导入数据，删除旧表
                        istmpatler = true;
                    }
                }
                else
                    tboldname = null; //如果新表已经存在，不走改表名逻辑

                //对比字段，只可以修改类型、增加字段、有限的修改字段名；保证安全不删除字段
                var sql = _commonUtils.FormatSql($@"
select 
a.column_name,
a.data_type,
a.data_length,
a.data_precision,
a.data_scale,
a.char_used,
case when a.nullable = 'Y' then 1 else 0 end,
nvl((select 1 from user_sequences where sequence_name='{Utils.GetCsName((tboldname ?? tbname).Last())}_seq_'||a.column_name), 0),
nvl((select 1 from user_triggers where trigger_name='{Utils.GetCsName((tboldname ?? tbname).Last())}_seq_'||a.column_name||'TI'), 0),
b.comments
from all_tab_columns a
left join all_col_comments b on b.owner = a.owner and b.table_name = a.table_name and b.column_name = a.column_name
where a.owner={{0}} and a.table_name={{1}}", tboldname ?? tbname);
                var ds = _orm.Ado.ExecuteArray(CommandType.Text, sql);
                var tbstruct = ds.ToDictionary(a => string.Concat(a[0]), a =>
                {
                    var sqlType = GetOracleSqlTypeFullName(a);
                    return new
                    {
                        column = string.Concat(a[0]),
                        sqlType,
                        is_nullable = string.Concat(a[6]) == "1",
                        is_identity = string.Concat(a[7]) == "1" && string.Concat(a[8]) == "1",
                        comment = string.Concat(a[9])
                    };
                }, StringComparer.CurrentCultureIgnoreCase);

                if (istmpatler == false)
                {
                    foreach (var tbcol in tb.ColumnsByPosition)
                    {
                        var dbtypeNoneNotNull = Regex.Replace(tbcol.Attribute.DbType, @"NOT\s+NULL", "NULL");
                        if (tbstruct.TryGetValue(tbcol.Attribute.Name, out var tbstructcol) ||
                            string.IsNullOrEmpty(tbcol.Attribute.OldName) == false && tbstruct.TryGetValue(tbcol.Attribute.OldName, out tbstructcol))
                        {
                            var isCommentChanged = tbstructcol.comment != (tbcol.Comment ?? "");
                            if (tbcol.Attribute.DbType.StartsWith(tbstructcol.sqlType, StringComparison.CurrentCultureIgnoreCase) == false)
                                istmpatler = true;
                            //sbalter.Append("execute immediate 'ALTER TABLE ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append(" MODIFY (").Append(_commonUtils.QuoteSqlName(tbstructcol.column)).Append(" ").Append(dbtypeNoneNotNull).Append(")';\r\n");
                            if (tbcol.Attribute.IsNullable != tbstructcol.is_nullable)
                            {
                                if (tbcol.Attribute.IsNullable == false)
                                    sbalter.Append("execute immediate 'UPDATE ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append(" SET ").Append(_commonUtils.QuoteSqlName(tbstructcol.column)).Append(" = ").Append(_commonUtils.FormatSql("{0}", tbcol.Attribute.DbDefautValue).Replace("'", "''")).Append(" WHERE ").Append(_commonUtils.QuoteSqlName(tbstructcol.column)).Append(" IS NULL';\r\n");
                                sbalter.Append("execute immediate 'ALTER TABLE ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append(" MODIFY ").Append(_commonUtils.QuoteSqlName(tbstructcol.column)).Append(" ").Append(tbcol.Attribute.IsNullable == true ? "" : "NOT").Append(" NULL';\r\n");
                            }
                            if (string.Compare(tbstructcol.column, tbcol.Attribute.OldName, true) == 0)
                            {
                                if (tbstructcol.is_identity)
                                    seqnameDel.Add(Utils.GetCsName($"{tbname[1]}_seq_{tbstructcol.column}"));
                                //修改列名
                                sbalter.Append("execute immediate 'ALTER TABLE ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append(" RENAME COLUMN ").Append(_commonUtils.QuoteSqlName(tbstructcol.column)).Append(" TO ").Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append("';\r\n");
                                if (tbcol.Attribute.IsIdentity)
                                    seqcols.Add((tbcol, tbname, tbcol.Attribute.IsIdentity == true));
                            }
                            else if (tbcol.Attribute.IsIdentity != tbstructcol.is_identity)
                                seqcols.Add((tbcol, tbname, tbcol.Attribute.IsIdentity == true));
                            if (isCommentChanged)
                                sbalter.Append("execute immediate 'COMMENT ON COLUMN ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}.{tbcol.Attribute.Name}")).Append(" IS ").Append(_commonUtils.FormatSql("{0}", tbcol.Comment ?? "").Replace("'", "''")).Append("';\r\n");
                            continue;
                        }
                        //添加列
                        sbalter.Append("execute immediate 'ALTER TABLE ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append(" ADD (").Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append(" ").Append(dbtypeNoneNotNull).Append(")';\r\n");
                        if (tbcol.Attribute.IsNullable == false)
                        {
                            sbalter.Append("execute immediate 'UPDATE ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append(" SET ").Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append(" = ").Append(_commonUtils.FormatSql("{0}", tbcol.Attribute.DbDefautValue).Replace("'", "''")).Append("';\r\n");
                            sbalter.Append("execute immediate 'ALTER TABLE ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append(" MODIFY ").Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append(" NOT NULL';\r\n");
                        }
                        if (tbcol.Attribute.IsIdentity == true) seqcols.Add((tbcol, tbname, tbcol.Attribute.IsIdentity == true));
                        if (string.IsNullOrEmpty(tbcol.Comment) == false) sbalter.Append("execute immediate 'COMMENT ON COLUMN ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}.{tbcol.Attribute.Name}")).Append(" IS ").Append(_commonUtils.FormatSql("{0}", tbcol.Comment ?? "").Replace("'", "''")).Append("';\r\n");
                    }

                    CreateOracleFunction(_orm);
                    var dsuksql = _commonUtils.FormatSql(@"
select
nvl(freesql_long_2_varchar(a.index_name, c.table_name, c.column_position), c.column_name),
a.index_name,
case when c.descend = 'DESC' then 1 else 0 end,
case when a.uniqueness = 'UNIQUE' then 1 else 0 end
from all_indexes a,
all_ind_columns c 
where a.index_name = c.index_name
and a.table_owner = c.table_owner
and a.table_name = c.table_name
and a.owner in ({0}) and a.table_name in ({1})
and not exists(select 1 from all_constraints where constraint_name = a.index_name and constraint_type = 'P')", tboldname ?? tbname);
                    var dsuk = _orm.Ado.ExecuteArray(CommandType.Text, dsuksql).Select(a => new[] { string.Concat(a[0]).Trim('"'), string.Concat(a[1]), string.Concat(a[2]), string.Concat(a[3]) }).ToArray();
                    foreach (var uk in tb.Indexes)
                    {
                        if (string.IsNullOrEmpty(uk.Name) || uk.Columns.Any() == false) continue;
                        var dsukfind1 = dsuk.Where(a => string.Compare(a[1], uk.Name, true) == 0).ToArray();
                        if (dsukfind1.Any() == false || dsukfind1.Length != uk.Columns.Length || dsukfind1.Where(a => uk.Columns.Where(b => (a[3] == "1") == uk.IsUnique && string.Compare(b.Column.Attribute.Name, a[0], true) == 0 && (a[2] == "1") == b.IsDesc).Any()).Count() != uk.Columns.Length)
                        {
                            if (dsukfind1.Any()) sbalter.Append("execute immediate 'DROP INDEX ").Append(_commonUtils.QuoteSqlName(uk.Name)).Append("';\r\n");
                            sbalter.Append("execute immediate 'CREATE ");
                            if (uk.IsUnique) sbalter.Append("UNIQUE ");
                            sbalter.Append("INDEX ").Append(_commonUtils.QuoteSqlName(uk.Name)).Append(" ON ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append("(");
                            foreach (var tbcol in uk.Columns)
                            {
                                sbalter.Append(_commonUtils.QuoteSqlName(tbcol.Column.Attribute.Name));
                                if (tbcol.IsDesc) sbalter.Append(" DESC");
                                sbalter.Append(", ");
                            }
                            sbalter.Remove(sbalter.Length - 2, 2).Append(")';\r\n");
                        }
                    }
                }
                if (istmpatler == false)
                {
                    sb.Append(sbalter);
                    continue;
                }
                var oldpk = _orm.Ado.ExecuteScalar(CommandType.Text, _commonUtils.FormatSql(@"select constraint_name from user_constraints where owner={0} and table_name={1} and constraint_type='P'", tbname))?.ToString();
                if (string.IsNullOrEmpty(oldpk) == false)
                    sb.Append("execute immediate 'ALTER TABLE ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}")).Append(" DROP CONSTRAINT ").Append(oldpk).Append("';\r\n");

                //创建临时表，数据导进临时表，然后删除原表，将临时表改名为原表名
                var tablename = tboldname == null ? _commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}") : _commonUtils.QuoteSqlName($"{tboldname[0]}.{tboldname[1]}");
                var tmptablename = _commonUtils.QuoteSqlName($"{tbname[0]}.FTmp_{tbname[1]}");
                //创建临时表
                sb.Append("execute immediate 'CREATE TABLE ").Append(tmptablename).Append(" ( ");
                foreach (var tbcol in tb.ColumnsByPosition)
                {
                    sb.Append(" \r\n  ").Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append(" ").Append(tbcol.Attribute.DbType).Append(",");
                    if (tbcol.Attribute.IsIdentity == true) seqcols.Add((tbcol, tbname, true));
                }
                if (tb.Primarys.Any())
                {
                    var pkname = primaryKeyName ?? $"{tbname[0]}_{tbname[1]}_pk2";
                    sb.Append(" \r\n  CONSTRAINT ").Append(pkname).Append(" PRIMARY KEY (");
                    foreach (var tbcol in tb.Primarys) sb.Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append(", ");
                    sb.Remove(sb.Length - 2, 2).Append("),");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("\r\n) LOGGING \r\nNOCOMPRESS \r\nNOCACHE\r\n';\r\n");
                //备注
                foreach (var tbcol in tb.ColumnsByPosition)
                {
                    if (string.IsNullOrEmpty(tbcol.Comment) == false)
                        sb.Append("execute immediate 'COMMENT ON COLUMN ").Append(_commonUtils.QuoteSqlName($"{tbname[0]}.FTmp_{tbname[1]}.{tbcol.Attribute.Name}")).Append(" IS ").Append(_commonUtils.FormatSql("{0}", tbcol.Comment).Replace("'", "''")).Append("';\r\n");
                }
                sb.Append("execute immediate 'INSERT INTO ").Append(tmptablename).Append(" (");
                foreach (var tbcol in tb.ColumnsByPosition)
                    sb.Append(_commonUtils.QuoteSqlName(tbcol.Attribute.Name)).Append(", ");
                sb.Remove(sb.Length - 2, 2).Append(")\r\nSELECT ");
                foreach (var tbcol in tb.ColumnsByPosition)
                {
                    var insertvalue = "NULL";
                    if (tbstruct.TryGetValue(tbcol.Attribute.Name, out var tbstructcol) ||
                        string.IsNullOrEmpty(tbcol.Attribute.OldName) == false && tbstruct.TryGetValue(tbcol.Attribute.OldName, out tbstructcol))
                    {
                        insertvalue = _commonUtils.QuoteSqlName(tbstructcol.column);
                        if (tbcol.Attribute.DbType.StartsWith(tbstructcol.sqlType, StringComparison.CurrentCultureIgnoreCase) == false)
                        {
                            var dbtypeNoneNotNull = Regex.Replace(tbcol.Attribute.DbType, @"(NOT\s+)?NULL", "");
                            insertvalue = $"cast({insertvalue} as {dbtypeNoneNotNull})";
                        }
                        if (tbcol.Attribute.IsNullable != tbstructcol.is_nullable)
                            insertvalue = $"nvl({insertvalue},{_commonUtils.FormatSql("{0}", tbcol.Attribute.DbDefautValue)})";
                    }
                    else if (tbcol.Attribute.IsNullable == false)
                        insertvalue = _commonUtils.FormatSql("{0}", tbcol.Attribute.DbDefautValue);
                    sb.Append(insertvalue.Replace("'", "''")).Append(", ");
                }
                sb.Remove(sb.Length - 2, 2).Append(" FROM ").Append(tablename).Append("';\r\n");
                sb.Append("execute immediate 'DROP TABLE ").Append(tablename).Append("';\r\n");
                sb.Append("execute immediate 'ALTER TABLE ").Append(tmptablename).Append(" RENAME TO ").Append(_commonUtils.QuoteSqlName($"{tbname[1]}")).Append("';\r\n");
                //创建表的索引
                foreach (var uk in tb.Indexes)
                {
                    sb.Append("execute immediate 'CREATE ");
                    if (uk.IsUnique) sb.Append("UNIQUE ");
                    sb.Append("INDEX ").Append(_commonUtils.QuoteSqlName(uk.Name)).Append(" ON ").Append(tablename).Append("(");
                    foreach (var tbcol in uk.Columns)
                    {
                        sb.Append(_commonUtils.QuoteSqlName(tbcol.Column.Attribute.Name));
                        if (tbcol.IsDesc) sb.Append(" DESC");
                        sb.Append(", ");
                    }
                    sb.Remove(sb.Length - 2, 2).Append(")';\r\n");
                }
            }
            Dictionary<string, bool> dicDeclare = new Dictionary<string, bool>();
            Action<string> dropSequence = seqname =>
            {
                if (dicDeclare.ContainsKey(seqname) == false)
                {
                    sbDeclare.Append("\r\n").Append(seqname).Append("IS NUMBER; \r\n");
                    dicDeclare.Add(seqname, true);
                }
                sb.Append(seqname).Append("IS := 0; \r\n")
                    .Append(" select count(1) into ").Append(seqname).Append(_commonUtils.FormatSql("IS from user_sequences where sequence_name={0}; \r\n", seqname))
                    .Append("if ").Append(seqname).Append("IS > 0 then \r\n")
                    .Append("  execute immediate 'DROP SEQUENCE ").Append(_commonUtils.QuoteSqlName(seqname)).Append("';\r\n")
                    .Append("end if; \r\n");
            };
            Action<string> dropTrigger = tiggerName =>
            {
                if (dicDeclare.ContainsKey(tiggerName) == false)
                {
                    sbDeclare.Append("\r\n").Append(tiggerName).Append("IS NUMBER; \r\n");
                    dicDeclare.Add(tiggerName, true);
                }
                sb.Append(tiggerName).Append("IS := 0; \r\n")
                    .Append(" select count(1) into ").Append(tiggerName).Append(_commonUtils.FormatSql("IS from user_triggers where trigger_name={0}; \r\n", tiggerName))
                    .Append("if ").Append(tiggerName).Append("IS > 0 then \r\n")
                    .Append("  execute immediate 'DROP TRIGGER ").Append(_commonUtils.QuoteSqlName(tiggerName)).Append("';\r\n")
                    .Append("end if; \r\n");
            };
            foreach (var seqname in seqnameDel)
            {
                dropSequence(seqname);
                dropTrigger(seqname + "TI");
            }
            foreach (var seqcol in seqcols)
            {
                var tbname = seqcol.Item2;
                var seqname = Utils.GetCsName($"{tbname[1]}_seq_{seqcol.Item1.Attribute.Name}");
                var tiggerName = seqname + "TI";
                var tbname2 = _commonUtils.QuoteSqlName($"{tbname[0]}.{tbname[1]}");
                var colname2 = _commonUtils.QuoteSqlName(seqcol.Item1.Attribute.Name);
                dropSequence(seqname);
                if (seqcol.Item3)
                {
                    var startWith = _orm.Ado.ExecuteScalar(CommandType.Text, _commonUtils.FormatSql(" select 1 from all_tab_columns where owner={0} and table_name={1} and column_name={2}", tbname[0], tbname[1], colname2)) == null ? 1 :
                        _orm.Ado.ExecuteScalar(CommandType.Text, $" select nvl(max({colname2})+1,1) from {tbname2}");
                    sb.Append("execute immediate 'CREATE SEQUENCE ").Append(_commonUtils.QuoteSqlName(seqname)).Append(" start with ").Append(startWith).Append("';\r\n");
                    sb.Append("execute immediate 'CREATE OR REPLACE TRIGGER ").Append(_commonUtils.QuoteSqlName(tiggerName))
                        .Append(" \r\nbefore insert on ").Append(tbname2)
                        .Append(" \r\nfor each row \r\nbegin\r\nselect ").Append(_commonUtils.QuoteSqlName(seqname))
                        .Append(".nextval into :new.").Append(colname2).Append(" from dual;\r\nend;';\r\n");
                }
                else
                    dropTrigger(tiggerName);
            }
            if (sbDeclare.Length > 0) sbDeclare.Insert(0, "declare ");
            return sb.Length == 0 ? null : sb.Insert(0, "BEGIN \r\n").Insert(0, sbDeclare.ToString()).Append("END;").ToString();
        }

        internal static string GetOracleSqlTypeFullName(object[] row)
        {
            var a = row;
            var sqlType = string.Concat(a[1]).ToUpper();
            var data_length = long.Parse(string.Concat(a[2]));
            long.TryParse(string.Concat(a[3]), out var data_precision);
            long.TryParse(string.Concat(a[4]), out var data_scale);
            var char_used = string.Concat(a[5]);
            if (Regex.IsMatch(sqlType, @"INTERVAL DAY\(\d+\) TO SECOND\(\d+\)", RegexOptions.IgnoreCase))
            {
            }
            else if (Regex.IsMatch(sqlType, @"INTERVAL YEAR\(\d+\) TO MONTH", RegexOptions.IgnoreCase))
            {
            }
            else if (sqlType.StartsWith("TIMESTAMP", StringComparison.CurrentCultureIgnoreCase))
            {
            }
            else if (sqlType.StartsWith("BLOB"))
            {
            }
            else if (char_used.ToLower() == "c")
                sqlType += sqlType.StartsWith("N") ? $"({data_length / 2})" : $"({data_length / 4} CHAR)";
            else if (char_used.ToLower() == "b")
                sqlType += $"({data_length} BYTE)";
            else if (sqlType.ToLower() == "float")
                sqlType += $"({data_precision})";
            else if (data_precision > 0 && data_scale > 0)
                sqlType += $"({data_precision},{data_scale})";
            else if (data_precision > 0)
                sqlType += $"({data_precision})";
            else
                sqlType += $"({data_length})";
            return sqlType;
        }
        internal static void CreateOracleFunction(IFreeSql fsql)
        {
            fsql.Ado.ExecuteNonQuery(CommandType.Text, @"
CREATE OR REPLACE FUNCTION freesql_long_2_varchar (
   p_index_name        IN user_ind_expressions.index_name%TYPE,
   p_table_name        IN user_ind_expressions.table_name%TYPE,
   p_COLUMN_POSITION   IN user_ind_expressions.table_name%TYPE)
   RETURN VARCHAR2
AS
   l_COLUMN_EXPRESSION   LONG;
BEGIN
   SELECT COLUMN_EXPRESSION
     INTO l_COLUMN_EXPRESSION
     FROM user_ind_expressions  
    WHERE     index_name = p_index_name
          AND table_name = p_table_name
          AND COLUMN_POSITION = p_COLUMN_POSITION;
  
   RETURN SUBSTR (l_COLUMN_EXPRESSION, 1, 4000);
END;");
        }
    }
}