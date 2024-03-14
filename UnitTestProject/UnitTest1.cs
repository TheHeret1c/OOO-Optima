using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        private List<Product> products;
        private List<Order> orders;

        public class Product
        {
            public int ProductArticle { get; set; }
            public string ProductName { get; set; }
            public int CategoryId { get; set; }
            public double ProductCost { get; set; }
        }

        public class Order
        {
            public int OrderID { get; set; }
            public int OrderClient { get; set; }
            public List<OrderProduct> OrderProducts { get; set; }
        }

        public class OrderProduct
        {
            public int ProductArticle { get; set; }
            public string ProductName { get; set; }
            public int ProductCount { get; set; }
            public double Price { get; set; }
        }

        public class Client
        {
            public int ClientID { get; set; }
            public string ClientEmail { get; set; }
        }

        public class ProductTests
        {
            public List<Product> GetTestProducts()
            {
                return new List<Product>
        {
            new Product { ProductArticle = 1, ProductName = "Product 1", CategoryId = 1 },
            new Product { ProductArticle = 2, ProductName = "Product 2", CategoryId = 1 },
            new Product { ProductArticle = 3, ProductName = "Product 3", CategoryId = 2 },
            new Product { ProductArticle = 4, ProductName = "Product 4", CategoryId = 2 }
        };
            }

            public List<Product> GetProductsByCategory(List<Product> products, int categoryId)
            {
                return products.FindAll(p => p.CategoryId == categoryId);
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            // Инициализация списка товаров
            products = new List<Product>
        {
            new Product { ProductName = "Product 1", ProductCost = 10.0 },
            new Product { ProductName = "Product 2", ProductCost = 15.0 },
            new Product { ProductName = "Product 3", ProductCost = 20.0 }
        };

            // Инициализация списка заказов
            orders = new List<Order>
        {
            new Order { OrderID = 1, OrderClient = 1, OrderProducts = new List<OrderProduct> { new OrderProduct { ProductArticle = 1, ProductCount = 2 } } },
            new Order { OrderID = 2, OrderClient = 2, OrderProducts = new List<OrderProduct> { new OrderProduct { ProductArticle = 2, ProductCount = 3 } } },
            new Order { OrderID = 3, OrderClient = 1, OrderProducts = new List<OrderProduct> { new OrderProduct { ProductArticle = 3, ProductCount = 1 } } }
        };
        }

        [TestMethod]
        public void TestSortProductsByCost()
        {
            // Arrange
            var expectedOrder = new List<string> { "Product 1", "Product 2", "Product 3" };

            // Act
            var sortedProducts = products.OrderBy(p => p.ProductCost).ToList();
            var actualOrder = sortedProducts.Select(p => p.ProductName).ToList();

            // Assert
            CollectionAssert.AreEqual(expectedOrder, actualOrder);
        }

        [TestMethod]
        public void TestGetClientsByEmail()
        {
            // Arrange
            string email = "test@example.com";

            var clients = new List<Client>
            {
                new Client { ClientID = 1, ClientEmail = "test@example.com" },
                new Client { ClientID = 2, ClientEmail = "another@example.com" },
                new Client { ClientID = 3, ClientEmail = "test@example.com" }
            };

            // Act
            var clientsWithEmail = clients.Where(c => c.ClientEmail == email);

            // Assert
            Assert.IsNotNull(clientsWithEmail, "No clients found with the specified email address");

            foreach (var client in clientsWithEmail)
            {
                Assert.AreEqual(email, client.ClientEmail, $"Client with ID {client.ClientID} has incorrect email address");
            }
        }

        [TestMethod]
        public void TestGroupOrdersByClient()
        {
            // Arrange
            var expectedGroupCounts = new Dictionary<int, int> { { 1, 2 }, { 2, 1 } };

            // Act
            var groupedOrders = orders.GroupBy(o => o.OrderClient).ToDictionary(g => g.Key, g => g.Count());

            // Assert
            CollectionAssert.AreEqual(expectedGroupCounts, groupedOrders);
        }

        [TestMethod]
        public void GetProductsByCategory_ShouldReturnCorrectProducts()
        {
            // Arrange
            var productTests = new ProductTests();
            var products = productTests.GetTestProducts();
            var categoryId = 1;

            // Act
            var result = productTests.GetProductsByCategory(products, categoryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            foreach (var product in result)
            {
                Assert.AreEqual(categoryId, product.CategoryId);
            }
        }

        [TestMethod]
        public void CalculateTotalQuantity_ReturnsCorrectTotalQuantity()
        {
            // Arrange
            var orderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductArticle = 1, ProductName = "Product1", ProductCount = 2 },
                new OrderProduct { ProductArticle = 2, ProductName = "Product2", ProductCount = 1 },
                new OrderProduct { ProductArticle = 3, ProductName = "Product3", ProductCount = 3 }
            };

            var expectedTotalQuantity = 2 + 1 + 3;

            // Act
            var actualTotalQuantity = CalculateTotalQuantity(orderProducts);

            // Assert
            Assert.AreEqual(expectedTotalQuantity, actualTotalQuantity);
        }

        public int CalculateTotalQuantity(List<OrderProduct> orderProducts)
        {
            int totalQuantity = 0;

            foreach (var orderProduct in orderProducts)
            {
                totalQuantity += orderProduct.ProductCount;
            }

            return totalQuantity;
        }





    }
}
