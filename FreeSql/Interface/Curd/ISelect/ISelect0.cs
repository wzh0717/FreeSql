﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FreeSql
{
    public partial interface ISelect0<TSelect, T1> where T1 : class
    {

#if net40
#else
        Task<DataTable> ToDataTableAsync(string field = null);
        Task<List<T1>> ToListAsync(bool includeNestedMembers = false);
        Task<List<TTuple>> ToListAsync<TTuple>(string field);

        Task<T1> ToOneAsync();
        Task<T1> FirstAsync();

        Task<bool> AnyAsync();
        Task<long> CountAsync();
#endif

        /// <summary>
        /// 指定事务对象
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        TSelect WithTransaction(DbTransaction transaction);
        /// <summary>
        /// 指定连接对象
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        TSelect WithConnection(DbConnection connection);

        /// <summary>
        /// 审核或跟踪 ToList 即将返回的数据
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        TSelect TrackToList(Action<object> action);

        /// <summary>
        /// 执行SQL查询，返回 DataTable
        /// </summary>
        /// <returns></returns>
        DataTable ToDataTable(string field = null);

        /// <summary>
        /// 执行SQL查询，返回 T1 实体所有字段的记录，记录不存在时返回 Count 为 0 的列表
        /// </summary>
        /// <param name="includeNestedMembers">false: 返回 2级 LeftJoin/InnerJoin/RightJoin 对象；true: 返回所有 LeftJoin/InnerJoin/RightJoin 的导航数据</param>
        /// <returns></returns>
        List<T1> ToList(bool includeNestedMembers = false);
        /// <summary>
        /// 执行SQL查询，分块返回数据，可减少内存开销。比如读取10万条数据，每次返回100条处理。
        /// </summary>
        /// <param name="size">数据块的大小</param>
        /// <param name="done">处理数据块</param>
        /// <param name="includeNestedMembers">false: 返回 2级 LeftJoin/InnerJoin/RightJoin 对象；true: 返回所有 LeftJoin/InnerJoin/RightJoin 的导航数据</param>
        void ToChunk(int size, Action<List<T1>> done, bool includeNestedMembers = false);
        /// <summary>
        /// 执行SQL查询，返回 field 指定字段的记录，并以元组或基础类型(int,string,long)接收，记录不存在时返回 Count 为 0 的列表
        /// </summary>
        /// <typeparam name="TTuple"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        List<TTuple> ToList<TTuple>(string field);
        /// <summary>
        /// 执行SQL查询，返回 T1 实体所有字段的第一条记录，记录不存在时返回 null
        /// </summary>
        /// <returns></returns>
        T1 ToOne();

        /// <summary>
        /// 执行SQL查询，返回 T1 实体所有字段的第一条记录，记录不存在时返回 null
        /// </summary>
        /// <returns></returns>
        T1 First();

        /// <summary>
        /// 将查询转为删除对象，以便支持导航对象或其他查询功能删除数据，如下：<para></para>
        /// fsql.Select&lt;T1&gt;().Where(a => a.Options.xxx == 1).ToDelete().ExecuteAffrows()<para></para>
        /// 注意：此方法不是将数据查询到内存循环删除，上面的代码产生如下 SQL 执行：<para></para>
        /// DELETE FROM `T1` WHERE id in (select a.id from T1 a left join Options b on b.t1id = a.id where b.xxx = 1)<para></para>
        /// 复杂删除使用该方案的好处：<para></para>
        /// 1、删除前可预览测试数据，防止错误删除操作；<para></para>
        /// 2、支持更加复杂的删除操作（IDelete 默认只支持简单的操作）；
        /// </summary>
        /// <returns></returns>
        IDelete<T1> ToDelete();
        /// <summary>
        /// 将查询转为更新对象，以便支持导航对象或其他查询功能更新数据，如下：<para></para>
        /// fsql.Select&lt;T1&gt;().Where(a => a.Options.xxx == 1).ToUpdate().Set(a => a.Title, "111").ExecuteAffrows()<para></para>
        /// 注意：此方法不是将数据查询到内存循环更新，上面的代码产生如下 SQL 执行：<para></para>
        /// UPDATE `T1` SET Title = '111' WHERE id in (select a.id from T1 a left join Options b on b.t1id = a.id where b.xxx = 1)<para></para>
        /// 复杂更新使用该方案的好处：<para></para>
        /// 1、更新前可预览测试数据，防止错误更新操作；<para></para>
        /// 2、支持更加复杂的更新操作（IUpdate 默认只支持简单的操作）；
        /// </summary>
        /// <returns></returns>
        IUpdate<T1> ToUpdate();

        /// <summary>
        /// 设置表名规则，可用于分库/分表，参数1：实体类型；参数2：默认表名；返回值：新表名； <para></para>
        /// 设置多次，可查询分表后的多个子表记录，以 UNION ALL 形式执行。 <para></para>
        /// 如：select.AsTable((type, oldname) => "table_1").AsTable((type, oldname) => "table_2").AsTable((type, oldname) => "table_3").ToSql(a => a.Id); <para></para>
        /// select * from (SELECT a."Id" as1 FROM "table_1" a) ftb <para></para>
        /// UNION ALL select * from (SELECT a."Id" as1 FROM "table_2" a) ftb <para></para>
        /// UNION ALL select * from (SELECT a."Id" as1 FROM "table_3" a) ftb <para></para>
        /// 还可以这样：select.AsTable((a, b) => "(select * from tb_topic where clicks > 10)").Page(1, 10).ToList()
        /// </summary>
        /// <param name="tableRule"></param>
        /// <returns></returns>
        TSelect AsTable(Func<Type, string, string> tableRule);
        /// <summary>
        /// 动态Type，在使用 Select&lt;object&gt; 后使用本方法，指定实体类型
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        TSelect AsType(Type entityType);
        /// <summary>
        /// 返回即将执行的SQL语句
        /// </summary>
        /// <param name="field">指定字段</param>
        /// <returns></returns>
        string ToSql(string field = null);
        /// <summary>
        /// 执行SQL查询，是否有记录
        /// </summary>
        /// <returns></returns>
        bool Any();

        /// <summary>
        /// 查询的记录数量
        /// </summary>
        /// <returns></returns>
        long Count();
        /// <summary>
        /// 查询的记录数量，以参数out形式返回
        /// </summary>
        /// <param name="count">返回的变量</param>
        /// <returns></returns>
        TSelect Count(out long count);

        /// <summary>
        /// 指定从主库查询（默认查询从库）
        /// </summary>
        /// <returns></returns>
        TSelect Master();

        /// <summary>
        /// 左联查询，使用导航属性自动生成SQL
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        TSelect LeftJoin(Expression<Func<T1, bool>> exp);
        /// <summary>
        /// 联接查询，使用导航属性自动生成SQL
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        TSelect InnerJoin(Expression<Func<T1, bool>> exp);
        /// <summary>
        /// 右联查询，使用导航属性自动生成SQL
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        TSelect RightJoin(Expression<Func<T1, bool>> exp);
        /// <summary>
        /// 左联查询，指定关联的实体类型
        /// </summary>
        /// <typeparam name="T2">关联的实体类型</typeparam>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        TSelect LeftJoin<T2>(Expression<Func<T1, T2, bool>> exp);
        /// <summary>
        /// 联接查询，指定关联的实体类型
        /// </summary>
        /// <typeparam name="T2">关联的实体类型</typeparam>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        TSelect InnerJoin<T2>(Expression<Func<T1, T2, bool>> exp);
        /// <summary>
        /// 右联查询，指定关联的实体类型
        /// </summary>
        /// <typeparam name="T2">关联的实体类型</typeparam>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        TSelect RightJoin<T2>(Expression<Func<T1, T2, bool>> exp);

        /// <summary>
        /// 左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id and b.clicks > ?clicks", new { clicks = 1 })
        /// </summary>
        /// <param name="sql">sql语法条件</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect LeftJoin(string sql, object parms = null);
        /// <summary>
        /// 联接查询，使用原生sql语法，InnerJoin("type b on b.id = a.id and b.clicks > ?clicks", new { clicks = 1 })
        /// </summary>
        /// <param name="sql">sql语法条件</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect InnerJoin(string sql, object parms = null);
        /// <summary>
        /// 右联查询，使用原生sql语法，RightJoin("type b on b.id = a.id and b.clicks > ?clicks", new { clicks = 1 })
        /// </summary>
        /// <param name="sql">sql语法条件</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect RightJoin(string sql, object parms = null);

        /// <summary>
        /// 原生sql语法条件，Where("id = ?id", new { id = 1 })
        /// </summary>
        /// <param name="sql">sql语法条件</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect Where(string sql, object parms = null);
        /// <summary>
        /// 原生sql语法条件，WhereIf(true, "id = ?id", new { id = 1 })
        /// </summary>
        /// <param name="condition">true 时生效</param>
        /// <param name="sql">sql语法条件</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect WhereIf(bool condition, string sql, object parms = null);

        /// <summary>
        /// 禁用全局过滤功能，不传参数时将禁用所有
        /// </summary>
        /// <param name="name">零个或多个过滤器名字</param>
        /// <returns></returns>
        TSelect DisableGlobalFilter(params string[] name);

        /// <summary>
        /// 按原生sql语法分组，GroupBy("concat(name, ?cc)", new { cc = 1 })
        /// </summary>
        /// <param name="sql">sql语法</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect GroupBy(string sql, object parms = null);
        /// <summary>
        /// 按原生sql语法聚合条件过滤，Having("count(name) = ?cc", new { cc = 1 })
        /// </summary>
        /// <param name="sql">sql语法条件</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect Having(string sql, object parms = null);

        /// <summary>
        /// 按原生sql语法排序，OrderBy("count(name) + ?cc desc", new { cc = 1 })
        /// </summary>
        /// <param name="sql">sql语法</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect OrderBy(string sql, object parms = null);
        /// <summary>
        /// 按原生sql语法排序，OrderBy(true, "count(name) + ?cc desc", new { cc = 1 })
        /// </summary>
        /// <param name="condition">true 时生效</param>
        /// <param name="sql">sql语法</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TSelect OrderBy(bool condition, string sql, object parms = null);

        /// <summary>
        /// 查询向后偏移行数
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        TSelect Skip(int offset);
        /// <summary>
        /// 查询向后偏移行数
        /// </summary>
        /// <param name="offset">行数</param>
        /// <returns></returns>
        TSelect Offset(int offset);
        /// <summary>
        /// 查询多少条数据
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        TSelect Limit(int limit);
        /// <summary>
        /// 查询多少条数据
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        TSelect Take(int limit);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页多少</param>
        /// <returns></returns>
        TSelect Page(int pageNumber, int pageSize);

        /// <summary>
        /// 查询数据前，去重
        /// <para>
        /// .Distinct().ToList(x => x.GroupName) 对指定字段去重
        /// </para>
        /// <para>
        /// .Distinct().ToList() 对整个查询去重
        /// </para>
        /// </summary>
        /// <returns></returns>
        TSelect Distinct();
    }
}
