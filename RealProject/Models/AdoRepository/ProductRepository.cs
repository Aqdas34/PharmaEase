using Microsoft.Data.SqlClient;

namespace RealProject.Models.AdoRepository
{
    public class ProductRepository
    {
        //private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PharmaCompany;Integrated Security=True";

        public void AddProduct(Product product)
        {

            string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO PRODUCTS(name, price, company, dosagetype, description,imagepath) VALUES(@n, @p, @c, @d, @r,@i)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@n", product.Name));
                    cmd.Parameters.Add(new SqlParameter("@p", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@c", product.Company));
                    cmd.Parameters.Add(new SqlParameter("@d", product.Dosage ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@r", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@i", product.ImagePath));

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} row(s) affected.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }


            }
        }


        public List<Product> GetProducts()
        {
            string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
            List<Product> products = new List<Product>();



            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();


                string query = "SELECT * FROM PRODUCT";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {


                    //SqlDataReader dr = cmd.ExecuteReader();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {



                        while (dr.Read())
                        {
                            Product product = new Product
                            {
                                Id = dr.GetInt32(0),
                                Name = dr.IsDBNull(1) ? null : dr.GetString(1),
                                Price = dr.IsDBNull(2) ? 0 : dr.GetDecimal(2), // Assuming Price is a decimal
                                Company = dr.IsDBNull(3) ? null : dr.GetString(3),
                                Dosage = dr.IsDBNull(4) ? null : dr.GetString(4),
                                Description = dr.IsDBNull(5) ? null : dr.GetString(5),
                                ImagePath = dr.IsDBNull(6) ? null : dr.GetString(6)

                            };

                            products.Add(product);


                        }
                    }


                }

            }

            return products;
        }




        public void DeleteProduct(int id)
        {

            string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();


                string query = "DELETE FROM PRODUCTS WHERE  ID = @id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int rows = cmd.ExecuteNonQuery();

                }

            }


        }

        public Product GetProductByID(int id)
        {
            string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
            Product product = new Product();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();



                string query = "SELECT * FROM PRODUCT WHERE ID = @id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())  // Changed to dr.Read() to fetch the row
                    {
                        product = new Product
                        {
                            Id = dr.GetInt32(0),
                            Name = dr.IsDBNull(1) ? null : dr.GetString(1),
                            Price = dr.IsDBNull(2) ? 0 : dr.GetDecimal(2), // Assuming Price is a decimal
                            Company = dr.IsDBNull(3) ? null : dr.GetString(3),
                            Dosage = dr.IsDBNull(4) ? null : dr.GetString(4),
                            Description = dr.IsDBNull(5) ? null : dr.GetString(5),
                            ImagePath = dr.IsDBNull(6) ? null : dr.GetString(6)
                        };
                    }




                }

            }

            return product;



        }

    }





}

