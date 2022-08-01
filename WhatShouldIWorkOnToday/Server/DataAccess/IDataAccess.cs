namespace WhatShouldIWorkOnToday.Server.DataAccess;

public interface IDataAccess
{
    void CommitTransaction();
    void Dispose();
    Task ExecuteRawSql<T>(string sql, T parameters, string connectionStringName);
    string GetConnectionString(string name);
    Task<List<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionStringName);
    Task<List<T>> LoadDataInTransactionAsync<T, U>(string storedProcedure, U parameters);
    Task<List<T>> QueryRawSql<T, U>(string sql, U parameters, string connectionStringName);
    void RollbackTransaction();
    Task<int> SaveDataAndGetIdAsync<T>(string storedProcedure, T parameters, string connectionStringName);
    Task SaveDataAsync<T>(string storedProcedure, T parameters, string connectionStringName);
    Task SaveDataInTransactionAsync<T>(string storedProcedure, T parameters);
    void StartTransaction(string connectionStringName);
}