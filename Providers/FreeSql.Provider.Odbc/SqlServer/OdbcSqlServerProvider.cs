﻿using FreeSql.Internal;
using FreeSql.Internal.CommonProvider;
using System;
using System.Collections.Generic;

namespace FreeSql.Odbc.SqlServer
{

    public class OdbcSqlServerProvider<TMark> : IFreeSql<TMark>
    {

        public ISelect<T1> Select<T1>() where T1 : class => new OdbcSqlServerSelect<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public ISelect<T1> Select<T1>(object dywhere) where T1 : class => new OdbcSqlServerSelect<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);
        public IInsert<T1> Insert<T1>() where T1 : class => new OdbcSqlServerInsert<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression);
        public IInsert<T1> Insert<T1>(T1 source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(T1[] source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(List<T1> source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IInsert<T1> Insert<T1>(IEnumerable<T1> source) where T1 : class => this.Insert<T1>().AppendData(source);
        public IUpdate<T1> Update<T1>() where T1 : class => new OdbcSqlServerUpdate<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public IUpdate<T1> Update<T1>(object dywhere) where T1 : class => new OdbcSqlServerUpdate<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);
        public IDelete<T1> Delete<T1>() where T1 : class => new OdbcSqlServerDelete<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, null);
        public IDelete<T1> Delete<T1>(object dywhere) where T1 : class => new OdbcSqlServerDelete<T1>(this, this.InternalCommonUtils, this.InternalCommonExpression, dywhere);

        public IAdo Ado { get; }
        public IAop Aop { get; }
        public ICodeFirst CodeFirst { get; }
        public IDbFirst DbFirst { get; }
        public OdbcSqlServerProvider(string masterConnectionString, string[] slaveConnectionString)
        {
            this.InternalCommonUtils = new OdbcSqlServerUtils(this);
            this.InternalCommonExpression = new OdbcSqlServerExpression(this.InternalCommonUtils);

            this.Ado = new OdbcSqlServerAdo(this.InternalCommonUtils, masterConnectionString, slaveConnectionString);
            this.Aop = new AopProvider();

            this.DbFirst = new OdbcSqlServerDbFirst(this, this.InternalCommonUtils, this.InternalCommonExpression);
            this.CodeFirst = new OdbcSqlServerCodeFirst(this, this.InternalCommonUtils, this.InternalCommonExpression);

            if (this.Ado.MasterPool != null)
                using (var conn = this.Ado.MasterPool.Get())
                {
                    try
                    {
                        (this.InternalCommonUtils as OdbcSqlServerUtils).IsSelectRowNumber = int.Parse(conn.Value.ServerVersion.Split('.')[0]) <= 10;
                    }
                    catch
                    {
                    }
                }
        }

        internal CommonUtils InternalCommonUtils { get; }
        internal CommonExpression InternalCommonExpression { get; }

        public void Transaction(Action handler) => Ado.Transaction(handler);
        public void Transaction(Action handler, TimeSpan timeout) => Ado.Transaction(handler, timeout);

        public GlobalFilter GlobalFilter { get; } = new GlobalFilter();

        ~OdbcSqlServerProvider()
        {
            this.Dispose();
        }
        bool _isdisposed = false;
        public void Dispose()
        {
            if (_isdisposed) return;
            (this.Ado as AdoProvider)?.Dispose();
        }
    }
}
