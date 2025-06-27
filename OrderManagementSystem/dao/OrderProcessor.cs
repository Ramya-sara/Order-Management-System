// dao/OrderProcessor.cs
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OrderManagementSystem.entity;
using OrderManagementSystem.exception;
using OrderManagementSystem.util;

namespace OrderManagementSystem.dao
{
    public class OrderProcessor : IOrderManagementRepository
    {
        SqlConnection conn = DBConnUtil.GetConnection();

        public void CreateUser(User user)
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO Users (UserId, Username, Password, Role) VALUES (@UserId, @Username, @Password, @Role)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.ExecuteNonQuery();
                Console.WriteLine("User created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public void CreateProduct(User user, Product product)
        {
            if (user.Role != "Admin")
            {
                Console.WriteLine("Only Admin can create products.");
                return;
            }

            try
            {
                conn.Open();
                string query = "INSERT INTO Products (ProductId, ProductName, Description, Price, QuantityInStock, Type) VALUES (@ProductId, @ProductName, @Description, @Price, @QuantityInStock, @Type)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);
                cmd.Parameters.AddWithValue("@Type", product.Type);
                cmd.ExecuteNonQuery();

                if (product.Type == "Electronics" && product is Electronics electronics)
                {
                    string query2 = "INSERT INTO Electronics (ProductId, Brand, WarrantyPeriod) VALUES (@ProductId, @Brand, @WarrantyPeriod)";
                    SqlCommand cmd2 = new SqlCommand(query2, conn);
                    cmd2.Parameters.AddWithValue("@ProductId", electronics.ProductId);
                    cmd2.Parameters.AddWithValue("@Brand", electronics.Brand);
                    cmd2.Parameters.AddWithValue("@WarrantyPeriod", electronics.WarrantyPeriod);
                    cmd2.ExecuteNonQuery();
                }
                else if (product.Type == "Clothing" && product is Clothing clothing)
                {
                    string query3 = "INSERT INTO Clothing (ProductId, Size, Color) VALUES (@ProductId, @Size, @Color)";
                    SqlCommand cmd3 = new SqlCommand(query3, conn);
                    cmd3.Parameters.AddWithValue("@ProductId", clothing.ProductId);
                    cmd3.Parameters.AddWithValue("@Size", clothing.Size);
                    cmd3.Parameters.AddWithValue("@Color", clothing.Color);
                    cmd3.ExecuteNonQuery();
                }

                Console.WriteLine("Product created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public List<Product> GetAllProducts()
        {
            List<Product> productList = new List<Product>();
            try
            {
                conn.Open();
                string query = "SELECT * FROM Products";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product p = new Product
                    {
                        ProductId = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Description = reader.GetString(2),
                        Price = reader.GetDouble(3),
                        QuantityInStock = reader.GetInt32(4),
                        Type = reader.GetString(5)
                    };
                    productList.Add(p);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return productList;
        }

        public void CreateOrder(User user, List<Product> products)
        {
            try
            {
                conn.Open();

                // Check if user exists
                SqlCommand checkUser = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserId=@UserId", conn);
                checkUser.Parameters.AddWithValue("@UserId", user.UserId);
                int count = (int)checkUser.ExecuteScalar();
                if (count == 0)
                {
                    CreateUser(user); // Create user if not exists
                }

                // Create Order
                SqlCommand insertOrder = new SqlCommand("INSERT INTO Orders (UserId) OUTPUT INSERTED.OrderId VALUES (@UserId)", conn);
                insertOrder.Parameters.AddWithValue("@UserId", user.UserId);
                int orderId = (int)insertOrder.ExecuteScalar();

                foreach (var product in products)
                {
                    SqlCommand insertItem = new SqlCommand("INSERT INTO OrderItems (OrderId, ProductId) VALUES (@OrderId, @ProductId)", conn);
                    insertItem.Parameters.AddWithValue("@OrderId", orderId);
                    insertItem.Parameters.AddWithValue("@ProductId", product.ProductId);
                    insertItem.ExecuteNonQuery();
                }

                Console.WriteLine("Order created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public void CancelOrder(int userId, int orderId)
        {
            try
            {
                conn.Open();
                SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM Orders WHERE OrderId=@OrderId AND UserId=@UserId", conn);
                check.Parameters.AddWithValue("@OrderId", orderId);
                check.Parameters.AddWithValue("@UserId", userId);
                int found = (int)check.ExecuteScalar();

                if (found == 0)
                    throw new OrderNotFoundException("Order not found for this user.");

                SqlCommand deleteItems = new SqlCommand("DELETE FROM OrderItems WHERE OrderId=@OrderId", conn);
                deleteItems.Parameters.AddWithValue("@OrderId", orderId);
                deleteItems.ExecuteNonQuery();

                SqlCommand deleteOrder = new SqlCommand("DELETE FROM Orders WHERE OrderId=@OrderId", conn);
                deleteOrder.Parameters.AddWithValue("@OrderId", orderId);
                deleteOrder.ExecuteNonQuery();

                Console.WriteLine("Order cancelled successfully.");
            }
            catch (OrderNotFoundException ex)
            {
                Console.WriteLine("Custom Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public List<Product> GetOrderByUser(User user)
        {
            List<Product> list = new List<Product>();
            try
            {
                conn.Open();
                string query = @"
                    SELECT p.ProductId, p.ProductName, p.Description, p.Price, p.QuantityInStock, p.Type
                    FROM Products p
                    INNER JOIN OrderItems oi ON p.ProductId = oi.ProductId
                    INNER JOIN Orders o ON oi.OrderId = o.OrderId
                    WHERE o.UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product p = new Product
                    {
                        ProductId = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Description = reader.GetString(2),
                        Price = reader.GetDouble(3),
                        QuantityInStock = reader.GetInt32(4),
                        Type = reader.GetString(5)
                    };
                    list.Add(p);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return list;
        }
    }
}
