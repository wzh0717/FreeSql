﻿using FreeSql.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FreeSql.Odbc.SqlServer
{

    public class OdbcSqlServerUtils : CommonUtils
    {
        public OdbcSqlServerUtils(IFreeSql orm) : base(orm)
        {
        }

        public bool IsSelectRowNumber = true;

        public override DbParameter AppendParamter(List<DbParameter> _params, string parameterName, Type type, object value)
        {
            if (string.IsNullOrEmpty(parameterName)) parameterName = $"p_{_params?.Count}";
            if (value?.Equals(DateTime.MinValue) == true) value = new DateTime(1970, 1, 1);
            var ret = new OdbcParameter { ParameterName = QuoteParamterName(parameterName), Value = value };
            var tp = _orm.CodeFirst.GetDbInfo(type)?.type;
            if (tp != null) ret.OdbcType = (OdbcType)tp.Value;
            _params?.Add(ret);
            return ret;
        }

        public override DbParameter[] GetDbParamtersByObject(string sql, object obj) =>
            Utils.GetDbParamtersByObject<OdbcParameter>(sql, obj, null, (name, type, value) =>
            {
                if (value?.Equals(DateTime.MinValue) == true) value = new DateTime(1970, 1, 1);
                var ret = new OdbcParameter { ParameterName = $"@{name}", Value = value };
                var tp = _orm.CodeFirst.GetDbInfo(type)?.type;
                if (tp != null) ret.OdbcType = (OdbcType)tp.Value;
                return ret;
            });

        public override string FormatSql(string sql, params object[] args) => sql?.FormatOdbcSqlServer(args);
        public override string QuoteSqlName(string name)
        {
            var nametrim = name.Trim();
            if (nametrim.StartsWith("(") && nametrim.EndsWith(")"))
                return nametrim; //原生SQL
            return $"[{nametrim.TrimStart('[').TrimEnd(']').Replace(".", "].[")}]";
        }
        public override string TrimQuoteSqlName(string name)
        {
            var nametrim = name.Trim();
            if (nametrim.StartsWith("(") && nametrim.EndsWith(")"))
                return nametrim; //原生SQL
            return $"{nametrim.TrimStart('[').TrimEnd(']').Replace("].[", ".").Replace(".[", ".")}";
        }
        public override string QuoteParamterName(string name) => $"@{(_orm.CodeFirst.IsSyncStructureToLower ? name.ToLower() : name)}";
        public override string IsNull(string sql, object value) => $"isnull({sql}, {value})";
        public override string StringConcat(string[] objs, Type[] types)
        {
            var sb = new StringBuilder();
            var news = new string[objs.Length];
            for (var a = 0; a < objs.Length; a++)
            {
                if (types[a] == typeof(string)) news[a] = objs[a];
                else if (types[a].NullableTypeOrThis() == typeof(Guid)) news[a] = $"cast({objs[a]} as char(36))";
                else news[a] = $"cast({objs[a]} as nvarchar)";
            }
            return string.Join(" + ", news);
        }
        public override string Mod(string left, string right, Type leftType, Type rightType) => $"{left} % {right}";
        public override string Div(string left, string right, Type leftType, Type rightType) => $"{left} / {right}";

        public override string QuoteWriteParamter(Type type, string paramterName) => paramterName;
        public override string QuoteReadColumn(Type type, string columnName) => columnName;

        public override string GetNoneParamaterSqlValue(List<DbParameter> specialParams, Type type, object value)
        {
            if (value == null) return "NULL";
            if (type == typeof(byte[]))
            {
                var bytes = value as byte[];
                var sb = new StringBuilder().Append("0x");
                foreach (var vc in bytes)
                {
                    if (vc < 10) sb.Append("0");
                    sb.Append(vc.ToString("X"));
                }
                return sb.ToString();
            }
            else if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                var ts = (TimeSpan)value;
                value = $"{ts.Hours}:{ts.Minutes}:{ts.Seconds}.{ts.Milliseconds}";
            }
            return FormatSql("{0}", value, 1);
        }
    }
}
