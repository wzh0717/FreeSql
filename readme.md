<p align="center">
  <img height="210" src="https://github.com/2881099/FreeSql/blob/master/logo.png?raw=true"/>
</p>

FreeSql 是一个功能强大的对象关系映射程序(O/RM)，支持 .NETCore 2.1+ 或 .NETFramework 4.0+（QQ群：4336577）

扶摇直上，至强ORM只为自由编码；鹏程万里，至简Linq可使保留黑发；横批：FreeSql（诗人：Coder）

# Features

- [x] 支持 CodeFirst 迁移；
- [x] 支持 DbFirst 从数据库导入实体类；
- [x] 大量采用 ExpressionTree 提升性能；
- [x] 支持 深入的类型映射，比如pgsql的数组类型；
- [x] 支持 丰富的表达式函数；
- [x] 支持 导航属性一对多、多对多贪婪加载，以及延时加载；
- [x] 支持 同步/异步数据库操作方法；
- [x] 支持 读写分离、分表分库，租户设计，过滤器，乐观锁；
- [x] 支持 多种数据库，MySql/SqlServer/PostgreSQL/Oracle/Sqlite；

| | |
| - | - |
| <img src="https://user-images.githubusercontent.com/16286519/55138232-f5e19e80-516d-11e9-9144-173cc7e52845.png" width="40" height="59"/> | [《新人学习指引》](https://www.cnblogs.com/FreeSql/p/11531300.html) \| [《Select》](https://github.com/2881099/FreeSql/wiki/%e6%9f%a5%e8%af%a2) \| [《Update》](https://github.com/2881099/FreeSql/wiki/%e4%bf%ae%e6%94%b9) \| [《Insert》](https://github.com/2881099/FreeSql/wiki/%e6%b7%bb%e5%8a%a0) \| [《Delete》](https://github.com/2881099/FreeSql/wiki/%e5%88%a0%e9%99%a4) |
| <img src="https://user-images.githubusercontent.com/16286519/55138241-faa65280-516d-11e9-8b27-139dea46e4df.png" width="40" height="59"/> | [《表达式函数》](https://github.com/2881099/FreeSql/wiki/%e8%a1%a8%e8%be%be%e5%bc%8f%e5%87%bd%e6%95%b0) \| [《CodeFirst》](https://github.com/2881099/FreeSql/wiki/CodeFirst) \| [《DbFirst》](https://github.com/2881099/FreeSql/wiki/DbFirst) \| [《BaseEntity》](https://github.com/2881099/FreeSql/tree/master/Examples/base_entity) |
| <img src="https://user-images.githubusercontent.com/16286519/55138263-06921480-516e-11e9-8da9-81f18a18b694.png" width="40" height="59"/> | [《Repository》](https://github.com/2881099/FreeSql/wiki/Repository) \| [《UnitOfWork》](https://github.com/2881099/FreeSql/wiki/%e5%b7%a5%e4%bd%9c%e5%8d%95%e5%85%83) \| [《过滤器》](https://github.com/2881099/FreeSql/wiki/%e8%bf%87%e6%bb%a4%e5%99%a8) \| [《乐观锁》](https://github.com/2881099/FreeSql/wiki/%e4%bf%ae%e6%94%b9) \| [《DbContext》](https://github.com/2881099/FreeSql/wiki/DbContext) |
| <img src="https://user-images.githubusercontent.com/16286519/55138284-0eea4f80-516e-11e9-8764-29264807f402.png" width="40" height="59"/> | [《读写分离》](https://github.com/2881099/FreeSql/wiki/%e8%af%bb%e5%86%99%e5%88%86%e7%a6%bb) \| [《分区分表》](https://github.com/2881099/FreeSql/wiki/%e5%88%86%e5%8c%ba%e5%88%86%e8%a1%a8) \| [《租户》](https://github.com/2881099/FreeSql/wiki/%e7%a7%9f%e6%88%b7) \| [《AOP》](https://github.com/2881099/FreeSql/wiki/AOP) \| [*更新日志*](https://github.com/2881099/FreeSql/wiki/%e6%9b%b4%e6%96%b0%e6%97%a5%e5%bf%97) |

# Packages

| Package Name |  NuGet | Downloads |
|--------------|  ------- |  ---- |
| FreeSql | [![nuget](https://img.shields.io/nuget/v/FreeSql.svg?style=flat-square)](https://www.nuget.org/packages/FreeSql) | [![stats](https://img.shields.io/nuget/dt/FreeSql.svg?style=flat-square)](https://www.nuget.org/stats/packages/FreeSql?groupby=Version) |
| FreeSql.Repository | [![nuget](https://img.shields.io/nuget/v/FreeSql.Repository.svg?style=flat-square)](https://www.nuget.org/packages/FreeSql.Repository) | [![stats](https://img.shields.io/nuget/dt/FreeSql.Repository.svg?style=flat-square)](https://www.nuget.org/stats/packages/FreeSql.Repository?groupby=Version) |
| FreeSql.DbContext | [![nuget](https://img.shields.io/nuget/v/FreeSql.DbContext.svg?style=flat-square)](https://www.nuget.org/packages/FreeSql.DbContext) | [![stats](https://img.shields.io/nuget/dt/FreeSql.DbContext.svg?style=flat-square)](https://www.nuget.org/stats/packages/FreeSql.DbContext?groupby=Version) |
| [FreeSql.AdminLTE](https://github.com/2881099/FreeSql.AdminLTE) | [![nuget](https://img.shields.io/nuget/v/FreeSql.AdminLTE.svg?style=flat-square)](https://www.nuget.org/packages/FreeSql.AdminLTE) | [![stats](https://img.shields.io/nuget/dt/FreeSql.AdminLTE.svg?style=flat-square)](https://www.nuget.org/stats/packages/FreeSql.AdminLTE?groupby=Version) |
| [FreeSql.Connection.Extensions](https://github.com/2881099/FreeSql.Connection.Extensions) | [![nuget](https://img.shields.io/nuget/v/FreeSql.Connection.Extensions.svg?style=flat-square)](https://www.nuget.org/packages/FreeSql.Connection.Extensions) | [![stats](https://img.shields.io/nuget/dt/FreeSql.Connection.Extensions.svg?style=flat-square)](https://www.nuget.org/stats/packages/FreeSql.Connection.Extensions?groupby=Version) |

> FreeSql 提供了五种使用习惯，请根据实际情况选择团队合适的一种：

- 要么FreeSql，原始用法；
- 要么[FreeSql.Repository](https://github.com/2881099/FreeSql/wiki/Repository)，仓储+工作单元习惯；
- 要么[FreeSql.DbContext](https://github.com/2881099/FreeSql/wiki/DbContext)，有点像efcore的使用习惯；
- 要么[FreeSql.Connection.Extensions](https://github.com/2881099/FreeSql.Connection.Extensions)，有点像Dapper的使用习惯；
- 要么[BaseEntity](https://github.com/2881099/FreeSql/tree/master/Examples/base_entity)，我求简单现在使用的这个；

> 其他下载

- [FreeSql.Tools 生成器](https://github.com/2881099/FreeSql.Tools)，基于 razor 模板的生成器；
- [Abp 中使用 FreeSql](https://github.com/gnsilence/JPGZService)，测试中．．．；
- [FreeSql 优势.pptx](https://github.com/2881099/FreeSql/files/3305852/FreeSql.pptx)；

# Providers

| Package Name | Version |
|--------------|  ------- |
| FreeSql.Provider.MySql | NETStandard2.0、net45、net40 |
| FreeSql.Provider.MySqlConnector | NETStandard2.0、net45 |
| FreeSql.Provider.PostgreSQL | NETStandard2.0、net45 |
| FreeSql.Provider.SqlServer | NETStandard2.0、net45、net40 |
| FreeSql.Provider.Sqlite | NETStandard2.0、net45、net40 |
| FreeSql.Provider.Oracle | NETStandard2.0、net45、net40 |
| [FreeSql.Provider.Odbc](https://github.com/2881099/FreeSql/tree/master/Providers/FreeSql.Provider.Odbc) | NETStandard2.0、net45、net40 |
| FreeSql.Extensions.LazyLoading | NETStandard2.0、net45、net40 |
| FreeSql.Extensions.JsonMap | NETStandard2.0、net45、net40 |
| FreeSql.Extensions.BaseEntity | NETStandard2.0 |

<p align="center">
  <img src="https://images.cnblogs.com/cnblogs_com/kellynic/133561/o_functions06.png"/>
</p>

# Quick start

> dotnet add package FreeSql.Provider.Sqlite

```csharp
IFreeSql fsql = new FreeSql.FreeSqlBuilder()
    .UseConnectionString(FreeSql.DataType.Sqlite, 
        @"Data Source=|DataDirectory|\document.db;Pooling=true;Max Pool Size=10")
    .UseAutoSyncStructure(true) //自动同步实体结构到数据库
    .Build(); //请务必定义成 Singleton 单例模式

class Song {
    [Column(IsIdentity = true)]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public DateTime CreateTime { get; set; }

    public virtual ICollection<Tag> Tags { get; set; }
}
class Song_tag {
    public int Song_id { get; set; }
    public virtual Song Song { get; set; }

    public int Tag_id { get; set; }
    public virtual Tag Tag { get; set; }
}
class Tag {
    [Column(IsIdentity = true)]
    public int Id { get; set; }
    public string Name { get; set; }

    public int? Parent_id { get; set; }
    public virtual Tag Parent { get; set; }

    public virtual ICollection<Song> Songs { get; set; }
    public virtual ICollection<Tag> Tags { get; set; }
}
```

# Query
```csharp
//OneToOne、ManyToOne
var t0 = fsql.Select<Tag>()
    .Where(a => a.Parent.Parent.Name == "粤语")
    .IncludeMany(a => a.Tags, then => then.Where(sub => sub.Name == "xxx"))
    .ToList();

//OneToMany
var t1 = fsql.Select<Tag>()
    .Where(a => a.Tags.AsSelect().Any(t => t.Parent.Id == 10))
    .ToList();

//ManyToMany
var t2 = fsql.Select<Song>()
    .Where(s => s.Tags.AsSelect().Any(t => t.Name == "国语"))
    .IncludeMany(a => a.Tags, then => then.Where(sub => sub.Name == "xxx"))
    .ToList();

//Other
var t3 = fsql.Select<Xxx>()
    .Where(a => a.IsDelete == 0)
    .WhereIf(keyword != null, a => a.UserName.Contains(keyword))
    .WhereIf(role_id > 0, a => a.RoleId == role_id)
    .Where(a => a.Nodes.AsSelect().Any(t => t.Parent.Id == t.UserId))
    .Count(out var total)
    .Page(page, size)
    .OrderByDescending(a => a.Id)
    .ToList()
```
更多前往Wiki：[《Select 查询数据文档》](https://github.com/2881099/FreeSql/wiki/%e6%9f%a5%e8%af%a2)

```csharp
var t3 = fsql.Select<Song>()
    .Where(a => new[] { 1, 2, 3 }.Contains(a.Id))
    .ToList();
```
```csharp
var t4 = fsql.Select<Song>()
    .Where(a => a.CreateTime.Date == DateTime.Now.Date)
    .ToList();
```
```csharp
var t5 = fsql.Select<Song>()
    .OrderBy(a => Guid.NewGuid())
    .Limit(1)
    .ToList();
```
更多前往Wiki：[《表达式函数》](https://github.com/2881099/FreeSql/wiki/%e8%a1%a8%e8%be%be%e5%bc%8f%e5%87%bd%e6%95%b0) 

# Repository & UnitOfWork
> dotnet add package FreeSql.Repository

```csharp
using (var uow = fsql.CreateUnitOfWork()) {
    var repo1 = uow.GetRepository<Song, int>();
    var repo2 = uow.GetRepository<Tag, int>();

    await repo1.InsertAsync(new Song());
    await repo2.InsertAsync(new Tag());
    uow.Commit();
}
```

# DbContext & DbSet
> dotnet add package FreeSql.DbContext

```csharp
using (var ctx = new fsql.CreateDbContext()) {
    var songs = ctx.Set<Song>();
    var tags = ctx.Set<Tag>();

    var tag = new Tag {
        Name = "testaddsublist",
        Tags = new[] {
            new Tag { Name = "sub1" },
            new Tag { Name = "sub2" },
            new Tag {
                Name = "sub3",
                Tags = new[] {
                    new Tag { Name = "sub3_01" }
                }
            }
        }
    };
    //tags.Add(tag);
    ctx.Add(tag);
    await ctx.SaveChangesAsync();
}
```

# DataFilter & Tenant

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddSingleton<IFreeSql>(Fsql);
    services.AddFreeRepository(filter => filter
        .Apply<ISoftDelete>("SoftDelete", a => a.IsDeleted == false)
        .Apply<ITenant>("Tenant", a => a.TenantId == 1)
        ,
        this.GetType().Assembly
    );
}
```

Temporary disable:
```csharp
var repoq = fsql.GetRepository<Song, int>();

using (repo1.DataFilter.Disable("Tenant")) {
    //Tenant Invalid
}
//Tenant restore
```

# Performance

FreeSql Query & Dapper Query
```shell
Elapsed: 00:00:00.6733199; Query Entity Counts: 131072; ORM: Dapper

Elapsed: 00:00:00.4554230; Query Tuple Counts: 131072; ORM: Dapper

Elapsed: 00:00:00.6846146; Query Dynamic Counts: 131072; ORM: Dapper

Elapsed: 00:00:00.6818111; Query Entity Counts: 131072; ORM: FreeSql*

Elapsed: 00:00:00.6060042; Query Tuple Counts: 131072; ORM: FreeSql*

Elapsed: 00:00:00.4211323; Query ToList<Tuple> Counts: 131072; ORM: FreeSql*

Elapsed: 00:00:01.0236285; Query Dynamic Counts: 131072; ORM: FreeSql*
```

FreeSql ToList & Dapper Query
```shell
Elapsed: 00:00:00.6707125; ToList Entity Counts: 131072; ORM: FreeSql*

Elapsed: 00:00:00.6495301; Query Entity Counts: 131072; ORM: Dapper
```

[Test code](FreeSql.Tests.PerformanceTests/MySqlAdoTest.cs)

# Contributors

[systemhejiyong](https://github.com/systemhejiyong)、
[LambertW](https://github.com/LambertW)、
[mypeng1985](https://github.com/mypeng1985)、
[stulzq](https://github.com/stulzq)、
[movingsam](https://github.com/movingsam)、
[ALer-R](https://github.com/ALer-R)、
[zouql](https://github.com/zouql)、
深圳|凉茶
