// main/MainModule.cs
using System;
using System.Collections.Generic;
using OrderManagementSystem.entity;
using OrderManagementSystem.dao;

namespace OrderManagementSystem.main
{
    class MainModule
    {
        static void Main(string[] args)
        {
            OrderProcessor orderProcessor = new OrderProcessor();

            while (true)
            {
                Console.WriteLine("\n--- Order Management System Menu ---");
                Console.WriteLine("1. Create User");
                Console.WriteLine("2. Create Product");
                Console.WriteLine("3. Get All Products");
                Console.WriteLine("4. Create Order");
                Console.WriteLine("5. Cancel Order");
                Console.WriteLine("6. Get Orders by User");
                Console.WriteLine("7. Exit");
                Console.Write("Enter your choice: ");

                string input = Console.ReadLine();
                int choice;
                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        CreateUser(orderProcessor);
                        break;
                    case 2:
                        CreateProduct(orderProcessor);
                        break;
                    case 3:
                        GetAllProducts(orderProcessor);
                        break;
                    case 4:
                        CreateOrder(orderProcessor);
                        break;
                    case 5:
                        CancelOrder(orderProcessor);
                        break;
                    case 6:
                        GetOrdersByUser(orderProcessor);
                        break;
                    case 7:
                        Console.WriteLine("Exiting application.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        static void CreateUser(OrderProcessor op)
        {
            Console.Write("Enter User ID: ");
            int userId = int.Parse(Console.ReadLine());
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            Console.Write("Enter Role (Admin/User): ");
            string role = Console.ReadLine();

            User user = new User(userId, username, password, role);
            op.CreateUser(user);
        }

        static void CreateProduct(OrderProcessor op)
        {
            Console.Write("Enter Admin User ID: ");
            int adminUserId = int.Parse(Console.ReadLine());
            Console.Write("Enter Admin Username: ");
            string adminUsername = Console.ReadLine();
            Console.Write("Enter Admin Password: ");
            string adminPassword = Console.ReadLine();
            Console.Write("Enter Admin Role (Admin): ");
            string adminRole = Console.ReadLine();

            User adminUser = new User(adminUserId, adminUsername, adminPassword, adminRole);

            Console.Write("Enter Product ID: ");
            int productId = int.Parse(Console.ReadLine());
            Console.Write("Enter Product Name: ");
            string productName = Console.ReadLine();
            Console.Write("Enter Description: ");
            string description = Console.ReadLine();
            Console.Write("Enter Price: ");
            double price = double.Parse(Console.ReadLine());
            Console.Write("Enter Quantity In Stock: ");
            int quantity = int.Parse(Console.ReadLine());
            Console.Write("Enter Type (Electronics/Clothing): ");
            string type = Console.ReadLine();

            if (type.Equals("Electronics", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Enter Brand: ");
                string brand = Console.ReadLine();
                Console.Write("Enter Warranty Period (months): ");
                int warranty = int.Parse(Console.ReadLine());

                Electronics product = new Electronics(productId, productName, description, price, quantity, type, brand, warranty);
                op.CreateProduct(adminUser, product);
            }
            else if (type.Equals("Clothing", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Enter Size: ");
                string size = Console.ReadLine();
                Console.Write("Enter Color: ");
                string color = Console.ReadLine();

                Clothing product = new Clothing(productId, productName, description, price, quantity, type, size, color);
                op.CreateProduct(adminUser, product);
            }
            else
            {
                Console.WriteLine("Invalid product type.");
            }
        }

        static void GetAllProducts(OrderProcessor op)
        {
            List<Product> products = op.GetAllProducts();
            Console.WriteLine("\n--- Product List ---");
            foreach (var p in products)
            {
                Console.WriteLine($"ID: {p.ProductId}, Name: {p.ProductName}, Type: {p.Type}, Price: {p.Price}");
            }
        }

        static void CreateOrder(OrderProcessor op)
        {
            Console.Write("Enter User ID: ");
            int userId = int.Parse(Console.ReadLine());
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            Console.Write("Enter Role (User/Admin): ");
            string role = Console.ReadLine();

            User user = new User(userId, username, password, role);

            List<Product> productsToOrder = new List<Product>();
            while (true)
            {
                Console.Write("Enter Product ID to add to order (or 'done' to finish): ");
                string input = Console.ReadLine();
                if (input.ToLower() == "done")
                    break;

                if (int.TryParse(input, out int prodId))
                {
                    // Here, for simplicity, we create dummy product with just ID.
                    // You may want to get full product details from DB.
                    Product p = new Product { ProductId = prodId };
                    productsToOrder.Add(p);
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }

            op.CreateOrder(user, productsToOrder);
        }

        static void CancelOrder(OrderProcessor op)
        {
            Console.Write("Enter User ID: ");
            int userId = int.Parse(Console.ReadLine());
            Console.Write("Enter Order ID: ");
            int orderId = int.Parse(Console.ReadLine());

            op.CancelOrder(userId, orderId);
        }

        static void GetOrdersByUser(OrderProcessor op)
        {
            Console.Write("Enter User ID: ");
            int userId = int.Parse(Console.ReadLine());
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            Console.Write("Enter Role (User/Admin): ");
            string role = Console.ReadLine();

            User user = new User(userId, username, password, role);

            var products = op.GetOrderByUser(user);
            Console.WriteLine("\n--- Products Ordered by User ---");
            foreach (var p in products)
            {
                Console.WriteLine($"ID: {p.ProductId}, Name: {p.ProductName}, Type: {p.Type}, Price: {p.Price}");
            }
        }
    }
}
