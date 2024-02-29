using SqlSugar;

namespace OneLogin.Core
{
    public static class SqlSugarExtension
    {
        public static ISqlSugarClient GetPayConnection(this ISqlSugarClient sqlSugarClient)
        {
            if (sqlSugarClient is SqlSugarScope sqlSugarScope)
                return sqlSugarScope.GetConnectionScope(DataSourceSettings.PayDb);
            return sqlSugarClient.AsTenant().GetConnectionScope(DataSourceSettings.PayDb);
        }

        public static ISqlSugarClient GetMasterDbConnection(this ISqlSugarClient sqlSugarClient)
        {
            return sqlSugarClient.AsTenant().GetConnectionScope(DataSourceSettings.MasterDb);
        }

        public static ISqlSugarClient GetLogDataConnection(this ISqlSugarClient sqlSugarClient)
        {
            var sqlClient = sqlSugarClient.AsTenant().GetConnection(DataSourceSettings.LogData);
            return sqlClient;
        }

        public static ISugarQueryable<TEntity> QueryableWithSplitTable<TEntity>(this ISqlSugarClient sqlSugarClient, object objectValue) where TEntity : class, new()
        {
            var tableName = sqlSugarClient.SplitHelper<TEntity>().GetTableName(objectValue);
            return sqlSugarClient.Queryable<TEntity>().AS(tableName);
        }

        public static IUpdateable<TEntity> UpdateableWithSplitTable<TEntity>(this ISqlSugarClient sqlSugarClient, object objectValue) where TEntity : class, new()
        {
            var tableName = sqlSugarClient.SplitHelper<TEntity>().GetTableName(objectValue);
            return sqlSugarClient.Updateable<TEntity>().AS(tableName);
        }

        public static IUpdateable<TEntity> UpdateableWithSplitTable<TEntity>(this ISqlSugarClient sqlSugarClient, TEntity entity, object objectValue) where TEntity : class, new()
        {
            var tableName = sqlSugarClient.SplitHelper<TEntity>().GetTableName(objectValue);
            return sqlSugarClient.Updateable(entity).AS(tableName);
        }
    }
}
