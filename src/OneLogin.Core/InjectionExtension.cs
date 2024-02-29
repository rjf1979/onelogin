using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OneLogin.Core
{
    public static class InjectionExtension
    {
        public static bool IsEnableLogSql => false;
        public static int EnableSqlTimeout => 0;


        public static IServiceCollection AddSingletonService(this IServiceCollection services, Assembly assembly, Type baseType)
        {
            foreach (Type item in assembly.ExportedTypes.Where(a => baseType.IsAssignableFrom(a) && a != baseType).ToList())
            {
                Type[] interfaces = item.GetInterfaces();
                foreach (Type serviceType in interfaces)
                {
                    services.AddSingleton(serviceType, item);
                }
            }
            return services;
        }

        public static IServiceCollection AddScopedService(this IServiceCollection services, Type baseType, params string[] assemblyNames)
        {
            foreach (var assemblyName in assemblyNames)
            {
                services.AddScopedService(baseType, Assembly.Load(assemblyName));
            }

            return services;
        }

        public static IServiceCollection AddScopedService(this IServiceCollection services, Type baseType, Assembly assembly)
        {
            foreach (var item in assembly.ExportedTypes.Where(a => baseType.IsAssignableFrom(a) && a != baseType && !a.IsInterface).ToList())
            {
                var interfaces = item.GetInterfaces().Where(x => x != baseType);
                foreach (var serviceType in interfaces)
                {
                    services.AddScoped(serviceType, item);
                }
            }
            return services;
        }


        public enum SqlSugarInjectType
        {
            Scope, Client
        }

        static List<ConnectionConfig> GetConnectionConfigs(IConfiguration configuration, DbType dbType, params string[] dataSettingNames)
        {
            var configs = new List<ConnectionConfig>();
            foreach (var dataSettingName in dataSettingNames)
            {
                configs.Add(new ConnectionConfig
                {
                    ConnectionString = configuration.GetConnectionString(dataSettingName),
                    DbType = dbType,
                    ConfigId = dataSettingName,
                    IsAutoCloseConnection = true
                });
            }
            return configs;
        }

        public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration, DbType dbType, SqlSugarInjectType sqlSugarInjectType = SqlSugarInjectType.Scope, params string[] dataSettingNames)
        {
            if (sqlSugarInjectType == SqlSugarInjectType.Scope)
            {
                return services.AddSqlSugarScope(configuration, dbType, dataSettingNames);
            }

            return services.AddSqlSugarClient(configuration, dbType, dataSettingNames);
        }

        static IServiceCollection AddSqlSugarClient(this IServiceCollection services, IConfiguration configuration, DbType dbType, params string[] dataSettingNames)
        {
            var client = CreateSqlSugarClient(configuration, dbType, dataSettingNames);
            return services.AddScoped<ISqlSugarClient>(_ => client);
        }

        public static IServiceCollection AddSqlSugarScope(this IServiceCollection services, IConfiguration configuration, DbType dbType, params string[] dataSettingNames)
        {
            var scope = CreateSqlSugarScope(configuration, dbType, dataSettingNames);
            return services.AddSingleton<ISqlSugarClient>(scope);
        }

        public static SqlSugarScope CreateSqlSugarScope(IConfiguration configuration, DbType dbType, params string[] dataSettingNames)
        {
            var connectionConfigs = GetConnectionConfigs(configuration, dbType, dataSettingNames);
            return CreateSqlSugarScope(connectionConfigs);
        }

        public static SqlSugarScope CreateSqlSugarScope(List<ConnectionConfig> connectionConfigs)
        {
            var sqlSugarScope = new SqlSugarScope(connectionConfigs, db =>
            {
                db.Aop.OnLogExecuted = (sql, parameters) =>
                {
                    var ms = (int)db.Ado.SqlExecutionTime.TotalMilliseconds;
                    var kvList = parameters.Select(x => new { x.ParameterName, ParameterValue = x.Value })
                        .ToList();
                    var parameter = kvList.ToJson();
                    LogSql(sql, parameter, ms);
                };
                db.Aop.OnError = sqlException =>
                {
                    var parameter = sqlException.Parametres.ToJson();
                    var sql = sqlException.Sql;
                    LogSql(sql, parameter, 0, sqlException);
                };
            });
            return sqlSugarScope;
        }

        static SqlSugarClient CreateSqlSugarClient(IConfiguration configuration, DbType dbType, params string[] dataSettingNames)
        {
            var connectionConfigs = GetConnectionConfigs(configuration, dbType, dataSettingNames);
            var sqlSugarClient = new SqlSugarClient(connectionConfigs);
            foreach (var name in dataSettingNames)
            {
                sqlSugarClient.GetConnection(name).Aop.OnLogExecuted = (sql, parameters) =>
                {
                    var ms = (int)sqlSugarClient.Ado.SqlExecutionTime.TotalMilliseconds;
                    var kvList = parameters.Select(x => new { x.ParameterName, ParameterValue = x.Value })
                        .ToList();
                    var parameter = kvList.ToJson();
                    LogSql(sql, parameter, ms);
                };
                sqlSugarClient.GetConnection(name).Aop.OnError = (sqlException) =>
                {
                    var parameter = sqlException.Parametres.ToJson();
                    var sql = sqlException.Sql;
                    LogSql(sql, parameter, 0, sqlException);
                };
            }
            return sqlSugarClient;
        }

        static void LogSql(string sql, string parameters, int elapsedTime, Exception exception = null)
        {
            if (exception == null)
            {
                var model = new SqlModel
                {
                    SqlText = sql,
                    Parameter = parameters,
                    ElapsedTime = elapsedTime
                };
                if (IsEnableLogSql)
                {
                    if (EnableSqlTimeout > 0 && EnableSqlTimeout >= elapsedTime)
                    {
                        //ConstObject.Logger.Info(model.ToJson());
                    }
                    else
                    {
                        //ConstObject.Logger.Info(model.ToJson());
                    }
                }
            }
            else
            {
                var model = new SqlModel
                {
                    SqlText = sql,
                    Parameter = parameters
                };
                //ConstObject.Logger.Error(exception, model.ToJson());
            }
        }
    }

    class SqlModel
    {
        public string SqlText { get; set; }
        public string Parameter { get; set; }
        public int ElapsedTime { get; set; }
    }
}