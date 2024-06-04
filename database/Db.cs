using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DtoModels;
using Google.Protobuf.WellKnownTypes;
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


        public async Task<bool> getCostumerAsynce(string mail)
        {
            bool found = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT CustomerId FROM master.Customer WHERE email = @email";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@email", mail);

                    // Execute the query asynchronously
                    var result = await command.ExecuteScalarAsync();

                    // Check if the result is null (no customer found) or not null (customer found)
                    if (result != null)
                    {
                        found = true; // Customer found
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return found;
        }




        public async Task<int> CreateCustomerAsync(Customer customer)
        {
            int rowsAffected = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"INSERT INTO Customer (FirstName, LastName, Age, Email, Mobil, Address, Postcode, City, Password)
                         VALUES (@FirstName, @LastName, @Age, @Email, @Mobil, @Address, @Postcode, @City, @Password);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    command.Parameters.AddWithValue("@LastName", customer.LastName);
                    command.Parameters.AddWithValue("@Age", customer.Age);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@Mobil", customer.Mobile);
                    command.Parameters.AddWithValue("@Address", customer.Address);
                    command.Parameters.AddWithValue("@Postcode", customer.Postcode);
                    command.Parameters.AddWithValue("@City", customer.City);
                    command.Parameters.AddWithValue("@Password", customer.Password);

                    rowsAffected = await command.ExecuteNonQueryAsync();
                }
            }

            return rowsAffected;
        }

        public async Task<int> CreateInvoice(List<int> productsId, string mail)
        {
            int rowsAffected = 0;
            int id = await getMail(mail);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"INSERT INTO Invoice (customerId, timestamp, isCompleted)
                         VALUES (@customerId, @timestamp, @isCompleted);";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customerId", id);
                    command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                    command.Parameters.AddWithValue("@isCompleted", 0);

                    rowsAffected = await command.ExecuteNonQueryAsync();
                }
            }
            await Createinvoiceproduct(productsId, id);

            return rowsAffected;

        }
        private async Task<bool> Createinvoiceproduct(List<int> productsId, int customerId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                int invoiceId;
                string query = @"SELECT invoiceId
                                FROM master.Invoice
                                WHERE customerId = @customerId
                                ORDER BY timestamp DESC
                                LIMIT 1;"; //skal tage den nyeste
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customerId", customerId);
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        if (reader.Read()) // Check if there is a row returned
                        {
                            invoiceId = reader.GetInt32("invoiceId"); // Assuming invoiceId is an integer

                        }
                        else
                        {
                            throw new Exception("Customer not found");
                        }

                    }
                }

                string query2 = @"INSERT INTO InvoiceProduct (invoiceId, productId)
                                VALUES";

                // Assuming productsId is a List<int>, ICollection<int>, or another collection type that supports indexed access
                int lastProductId = productsId[productsId.Count - 1];

                // Start building the query string without the last item
                foreach (int id in productsId.Take(productsId.Count - 1))
                {
                    query2 += $"({invoiceId}, {id}), ";
                }

                // Append the last item with a semicolon
                query2 += $"({invoiceId}, {lastProductId});";

                using (MySqlCommand command = new MySqlCommand(query2, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            return true;
        }

        private async Task<int> getMail(string mail)
        {
            int id;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"SELECT customerId FROM master.Customer WHERE email = @email";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@email", mail);
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        if (reader.Read()) // Check if there is a row returned
                        {
                            id = reader.GetInt32("customerId"); // Assuming CustomerId is an integer

                        }
                        else
                        {
                            throw new Exception("Customer not found");
                        }

                    }
                }
            }
            return id;
        }

    }
}
