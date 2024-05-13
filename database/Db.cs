using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DtoModels;
using Models;
using MySql.Data.MySqlClient;

namespace database
{
    public class Db
    {
        string connectionString = "server=77.237.238.253;user=nico622f;database=master;port=3306;password=!Xrq22baw";


        public async Task<List<DtoProductInventory>> GetAll()
        {
            List<DtoProductInventory> products = new List<DtoProductInventory>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM master.Products;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DtoProductInventory product = new DtoProductInventory
                            {
                                Id = reader.GetInt32("productId"),
                                EanNr = reader.GetInt32("eanNr"),
                                Name = reader.GetString("name"),
                                Description = reader.GetString("description"),
                                Price = reader.GetDouble("price"),
                                Amount = reader.GetInt32("amount")
                            };

                            // Convert byte[] image to Base64 string
                            byte[] imageBytes = (byte[])reader["image"];
                            if (imageBytes != null)
                            {
                                product.Image = Encoding.Default.GetString(imageBytes);
                            }

                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        public async Task<DtoProductInventory> GetProductByIdAsync(int id)
        {
            DtoProductInventory product = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM master.Products WHERE productId = @Id;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new DtoProductInventory
                            {
                                Id = reader.GetInt32("productId"),
                                EanNr = reader.GetInt32("eanNr"),
                                Name = reader.GetString("name"),
                                Description = reader.GetString("description"),
                                Price = reader.GetDouble("price"),
                                Amount = reader.GetInt32("amount")
                            };

                            // Convert byte[] image to Base64 string
                            byte[] imageBytes = (byte[])reader["image"];
                            if (imageBytes != null)
                            {
                                product.Image = Encoding.Default.GetString(imageBytes);
                            }
                        }
                    }
                }
            }

            return product;
        }

        public async Task<int> CreateCustomerAsync(Customer customer)
            {
                int rowsAffected = 0;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"INSERT INTO Customers (FirstName, LastName, Age, Email, Mobile, Address, Postcode, City, Password)
                         VALUES (@FirstName, @LastName, @Age, @Email, @Mobile, @Address, @Postcode, @City, @Password);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                        command.Parameters.AddWithValue("@LastName", customer.LastName);
                        command.Parameters.AddWithValue("@Age", customer.Age);
                        command.Parameters.AddWithValue("@Email", customer.Email);
                        command.Parameters.AddWithValue("@Mobile", customer.Mobile);
                        command.Parameters.AddWithValue("@Address", customer.Address);
                        command.Parameters.AddWithValue("@Postcode", customer.Postcode);
                        command.Parameters.AddWithValue("@City", customer.City);
                        command.Parameters.AddWithValue("@Password", customer.Password);

                        rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                }

                return rowsAffected;
            }
        }
    }
