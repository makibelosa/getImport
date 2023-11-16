using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;

public class Root
{
    // Properties representing JSON fields
    public string name { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string address { get; set; }
    public string postalZip { get; set; }
    public string region { get; set; }
    public string country { get; set; }
    public int list { get; set; }
    public object numberrange { get; set; }
    public string currency { get; set; }
    public string alphanumeric { get; set; }

}

class Program
{
    static void Main(string[] args)
    {
        string jsonFilePath = @"C:\Users\samuel\Downloads\Work\Task\data.json"; // The file path that contains the JSON data

        try
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath); //jsonFilePath is the path to the file that contains the JSON data.

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    List<Root> dataList = JsonConvert.DeserializeObject<List<Root>>(jsonContent);

                    if (dataList != null)
                    {
                        string connString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;  // This line provides the SQL connection string
                        string tableName = "Customer"; // This is the actual name from my table in the database

                        using (SqlConnection connection = new SqlConnection(connString))
                        {
                            connection.Open();

                            foreach (var item in dataList)
                            {
                                try
                                {
                                    //This code insert data into SQL table
                                    string insertQuery = $@"INSERT INTO {tableName} 
                                                            (name, phone, email, address, postalZip, region, country, list, numberrange, currency, alphanumeric) 
                                                            VALUES 
                                                            (@Name, @Phone, @Email, @Address, @PostalZip, @Region, @Country, @List, @NumberRange, @Currency, @Alphanumeric)";

                                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                                    {
                                        //Add parameters to the SQL command
                                        command.Parameters.AddWithValue("@Name", item.name);
                                        command.Parameters.AddWithValue("@Phone", item.phone);
                                        command.Parameters.AddWithValue("@Email", item.email);
                                        command.Parameters.AddWithValue("@Address", item.address);
                                        command.Parameters.AddWithValue("@PostalZip", item.postalZip);
                                        command.Parameters.AddWithValue("@Region", item.region);
                                        command.Parameters.AddWithValue("@Country", item.country);
                                        command.Parameters.AddWithValue("@List", item.list);
                                        command.Parameters.AddWithValue("@NumberRange", item.numberrange);
                                        command.Parameters.AddWithValue("@Currency", item.currency);
                                        command.Parameters.AddWithValue("@Alphanumeric", item.alphanumeric);



                                        command.ExecuteNonQuery();  //Execute SQL command
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error inserting data: {ex.Message}");
                                }
                            }
                        }

                        Console.WriteLine("Data successfully imported into the SQL table.");

                        // Delete the original JSON file
                        File.Delete(jsonFilePath);
                        Console.WriteLine("Original JSON file deleted.");
                    }
                    else
                    {
                        Console.WriteLine("Deserialization resulted in a null list.");
                    }
                }
                else
                {
                    Console.WriteLine("The JSON content is empty.");
                }
            }
            else
            {
                Console.WriteLine("The file does not exist.");
            }
        }
        catch (JsonSerializationException ex)
        {
            Console.WriteLine("Error deserializing JSON: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);  // This handles exceptions
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
