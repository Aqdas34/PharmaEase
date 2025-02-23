using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace RealProject.Models.DapperGenericRepo
{
  
    public class GenericRepository<TEntity> : IRepository<TEntity>
    {
        private readonly string _connectionString;

        public GenericRepository()
        {
            // Retrieve the connection string from appsettings.json
            _connectionString = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json")
                                    .Build()
                                    .GetConnectionString("DefaultConnection");
        }



        // Dapper

        public void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var tableName = typeof(TEntity).Name;   
            var properties = typeof(TEntity).GetProperties()
                                            .Where(p => p.Name != "Id" && p.PropertyType != typeof(IFormFile))
                                            .ToList();
            var columnNames = string.Join(",", properties.Select(x => x.Name));
            var parameterNames = string.Join(",", properties.Select(x => "@" + x.Name));

            var query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(entity) ?? DBNull.Value;
                        parameters.Add("@" + property.Name, value);
                    }

                    connection.Execute(query, parameters);
                }
            }
            catch (SqlException ex)
            {
                // Log exception (using your preferred logging framework)
                Console.Error.WriteLine("An error occurred while adding the entity to the database: " + ex.Message);
                throw new Exception("An error occurred while adding the entity to the database.", ex);
            }
        }
        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var tableName = typeof(TEntity).Name;
            var properties = typeof(TEntity).GetProperties()
                                             .Where(p => p.Name != "Id" && p.PropertyType != typeof(IFormFile))
                                             .ToList();

            if (!properties.Any())
                throw new InvalidOperationException("No properties to update.");

            var setClause = string.Join(",", properties.Select(x => $"{x.Name} = @{x.Name}"));

            var query = $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Creating a dynamic parameters object for Dapper
                    var parameters = new DynamicParameters();
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(entity) ?? DBNull.Value;
                        parameters.Add("@" + property.Name, value);
                    }

                    var idProperty = typeof(TEntity).GetProperty("Id");
                    if (idProperty == null)
                        throw new InvalidOperationException("The entity does not have an 'Id' property.");

                    var idValue = idProperty.GetValue(entity);
                    if (idValue == null)
                        throw new InvalidOperationException("The 'Id' property value cannot be null.");

                    parameters.Add("@Id", idValue);

                    connection.Execute(query, parameters);
                }
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"An error occurred while updating the entity in the database: {ex.Message}");
                throw new Exception("An error occurred while updating the entity in the database.", ex);
            }
        }

        public void Delete(int id)
        {
            var tableName = typeof(TEntity).Name;
            var query = $"DELETE FROM {tableName} WHERE Id = @Id";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    connection.Execute(query, new { Id = id });
                }
            }
            catch (SqlException ex)
            {
                // Log exception (using your preferred logging framework)
                Console.Error.WriteLine("An error occurred while deleting the entity from the database: " + ex.Message);
                throw new Exception("An error occurred while deleting the entity from the database.", ex);
            }
        }

        public List<TEntity> GetAll()
        {
            var tableName = typeof(TEntity).Name;
            var query = $"SELECT * FROM {tableName}";
            var filteredEntities = new List<TEntity>();

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    var entities = connection.Query<TEntity>(query).AsList();

                    // Filter out properties of type IFormFile

                    foreach (var entity in entities)
                    {
                        var properties = typeof(TEntity).GetProperties();
                        foreach (var property in properties)
                        {
                            if (property.PropertyType == typeof(IFormFile))
                            {
                                property.SetValue(entity, null);
                            }
                        }
                        filteredEntities.Add(entity);
                    }


                }
                catch (SqlException ex)
                {
                    // Log exception (using your preferred logging framework)
                    Console.Error.WriteLine("An error occurred while retrieving the entities from the database: " + ex.Message);
                    throw new Exception("An error occurred while retrieving the entities from the database.", ex);
                }
            }
            return filteredEntities;
        }
        public TEntity? GetById(int id)
        {

            var tableName = typeof(TEntity).Name;
            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty == null)
                throw new Exception("Entity does not have an Id property");

            var query = $"SELECT * FROM {tableName} WHERE Id = @Id";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var entity = connection.QuerySingleOrDefault<TEntity>(query, new { Id = id });

                    if (entity != null)
                    {
                        // Set properties of type IFormFile to null
                        foreach (var property in typeof(TEntity).GetProperties())
                        {
                            if (property.PropertyType == typeof(IFormFile))
                            {
                                property.SetValue(entity, null);
                            }
                        }
                    }

                    return entity;
                }
            }
            catch (SqlException ex)
            {
                // Log exception (using your preferred logging framework)
                Console.Error.WriteLine("An error occurred while retrieving the entity from the database: " + ex.Message);
                throw new Exception("An error occurred while retrieving the entity from the database.", ex);
            }
        }

        //public void Delete(int id)
        //{


        //        var tableName = typeof(TEntity).Name;

        //        try
        //        {
        //        using (SqlConnection connection = new SqlConnection(_connectionString))
        //        {


        //            connection.Open();

        //            var querry = $"DELETE FROM {tableName} WHERE id =  @id";

        //            var command = new SqlCommand(querry, connection);
        //            command.Parameters.AddWithValue("@id", id);

        //            command.ExecuteNonQuery();

        //        }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex);
        //        }



        //}

        //public List<TEntity> GetAll()
        //{
        //    var tableName = typeof(TEntity).Name;
        //    var query = $"SELECT * FROM {tableName}";

        //    var entities = new List<TEntity>();

        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();

        //            using (var command = new SqlCommand(query, connection))
        //            {
        //                using (var reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var entity = Activator.CreateInstance<TEntity>();
        //                        foreach (var property in typeof(TEntity).GetProperties())
        //                        {
        //                            // Exclude properties of type IFormFile
        //                            if (property.PropertyType == typeof(IFormFile))
        //                                continue;

        //                            if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
        //                            {
        //                                var value = reader[property.Name];
        //                                property.SetValue(entity, value);
        //                            }
        //                        }
        //                        entities.Add(entity);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        // Log exception (using your preferred logging framework)
        //        Console.Error.WriteLine("An error occurred while retrieving the entities from the database: " + ex.Message);
        //        throw new Exception("An error occurred while retrieving the entities from the database.", ex);
        //    }

        //    return entities;
        //}



        //public TEntity GetById(int id)
        //{
        //    var tableName = typeof(TEntity).Name;
        //    var idProperty = typeof(TEntity).GetProperty("Id");
        //    if (idProperty == null)
        //        throw new Exception("Entity does not have an Id property");

        //    var query = $"SELECT * FROM {tableName} WHERE Id = @Id";

        //    TEntity entity = default;

        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();

        //            using (var command = new SqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@Id", id);

        //                using (var reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        entity = Activator.CreateInstance<TEntity>();
        //                        foreach (var property in typeof(TEntity).GetProperties())
        //                        {
        //                            // Exclude properties of type IFormFile
        //                            if (property.PropertyType == typeof(IFormFile))
        //                                continue;

        //                            if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
        //                            {
        //                                var value = reader[property.Name];
        //                                property.SetValue(entity, value);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        // Log exception (using your preferred logging framework)
        //        Console.Error.WriteLine("An error occurred while retrieving the entity from the database: " + ex.Message);
        //        throw new Exception("An error occurred while retrieving the entity from the database.", ex);
        //    }

        //    return entity;
        //}



    }
}
