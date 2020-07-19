using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ConsoleAppReadAzureSql
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString;
            // Check whether the environment variable exists.
            connectionString = Environment.GetEnvironmentVariable("connectionstring");
            // If necessary, create it.
            if (connectionString != null)
            {
                Console.WriteLine("Env String "+connectionString);
            }
            else
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot Configuration = builder.Build();


                connectionString = Configuration.GetSection("sqlConnectionString").Value;
                Console.WriteLine(connectionString);

                
                }
            SqlDataReader rdr = null;
            string json = string.Empty;
            List<object> records = new List<object>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[GetWeatherDetails]", conn);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (rdr.Read())
                {
                    IDictionary<string, object> record = new Dictionary<string, object>();
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        record.Add(rdr.GetName(i), rdr[i]);
                    }
                    records.Add(record);
                }
            }
            json = JsonConvert.SerializeObject(records);
            Console.WriteLine(json);
            Console.ReadLine();
        }
    }
}
