using System;
using System.Data;
using Npgsql;

namespace Infrastructure.Context;

public class DataContext
{


    private readonly string ConnectionString =
                            @"Host = localhost ; Database = workspace ; Username = postgres ; Port = 5432 ; Password = mai2026";
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }


}
