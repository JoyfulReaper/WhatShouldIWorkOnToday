using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class SqlDataAccess : IDataAccess, IDisposable
{
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private bool isClosed = false;
    private readonly IConfiguration _config;
    private readonly ILogger<SqlDataAccess> _logger;

    public SqlDataAccess(IConfiguration config,
        ILogger<SqlDataAccess> logger)
    {
        _config = config;
        _logger = logger;
    }

    public string GetConnectionString(string name)
    {
        return _config.GetConnectionString(name);
    }

    public async Task<List<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionStringName)
    {
        string connectionString = GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            List<T> rows = (await connection.QueryAsync<T>(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure))
                .ToList();

            return rows;
        }
    }

    public async Task SaveDataAsync<T>(string storedProcedure, T parameters, string connectionStringName)
    {
        string connectionString = GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            await connection.ExecuteAsync(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure);
        }
    }

    public async Task<int> SaveDataAndGetIdAsync<T>(string storedProcedure, T parameters, string connectionStringName)
    {
        string connectionString = GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            int id = await connection.QuerySingleAsync<int>(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure);

            return id;
        }
    }

    public async Task SaveDataInTransactionAsync<T>(string storedProcedure, T parameters)
    {
        await _connection.ExecuteAsync(storedProcedure, parameters,
            commandType: CommandType.StoredProcedure, transaction: _transaction);
    }

    public async Task<List<T>> LoadDataInTransactionAsync<T, U>(string storedProcedure, U parameters)
    {
        List<T> rows = (await _connection.QueryAsync<T>(storedProcedure, parameters,
            commandType: CommandType.StoredProcedure, transaction: _transaction))
            .ToList();

        return rows;
    }

    public void StartTransaction(string connectionStringName)
    {
        string connectionString = GetConnectionString(connectionStringName);
        _connection = new SqlConnection(connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();

        isClosed = false;
    }

    public void CommitTransaction()
    {
        _transaction?.Commit();
        _connection?.Close();

        isClosed = true;
    }

    public void RollbackTransaction()
    {
        _transaction?.Rollback();
        _connection?.Close();

        isClosed = true;
    }

    public void Dispose()
    {
        if (isClosed == false)
        {
            try
            {
                CommitTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Commit transaction failed in the Dispose() method.");
            }
        }

        _transaction = null;
        _connection = null;
    }

    public async Task<List<T>> QueryRawSql<T, U>(string sql, U parameters, string connectionStringName)
    {
        string connectionString = GetConnectionString(connectionStringName);
        using IDbConnection connection = new SqlConnection(connectionString);

        return (await connection.QueryAsync<T>(sql, parameters,
            commandType: CommandType.Text))
            .ToList();
    }

    public async Task ExecuteRawSql<T>(string sql, T parameters, string connectionStringName)
    {
        string connectionString = GetConnectionString(connectionStringName);
        using IDbConnection connection = new SqlConnection(connectionString);

        await connection.ExecuteAsync(sql, parameters,
            commandType: CommandType.Text);
    }
}
