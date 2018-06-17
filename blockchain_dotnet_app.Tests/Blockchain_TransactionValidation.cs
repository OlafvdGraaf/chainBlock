using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using blockchain_dotnet_app;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace blockchain_dotnet_app.Tests
{
    [TestClass]
    public class Blockchain_TransactionValidationTests
    {

        [TestMethod]
        public void validateTransaction_MissingJsonValues_ReturnsMissingValues()
        {
            // Arrange
            var blockchain = new Blockchain();

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", new List<Product>() },
                {"sender", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15" },
                {"recipient", "75793F029A398702E99ED71F5D1BF070E27DA689D1892F46C7C57CC4CB9CFAE2" },
                {"chainTokens", 0 },
                //{"transactionFee" , 0 }
                //{"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.AreEqual("Missing values", result["response"].ToString());
            
        }

        [TestMethod]
        public void validateTansaction_NewTransactionWithoutProducs_ReturnsValidationTrue()
        {
            // Arrange
            var blockchain = new Blockchain();

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", new List<Product>() },
                {"sender", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15" },
                {"recipient", "75793F029A398702E99ED71F5D1BF070E27DA689D1892F46C7C57CC4CB9CFAE2" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.IsTrue(result["validation"].ToObject<bool>());
        }

        [TestMethod]
        public void validateTansaction_ExcistingTransactionWithoutProducs_ReturnsValidationTrue()
        {
            // Arrange
            var blockchain = new Blockchain();

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", new List<Product>() },
                {"sender", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15" },
                {"recipient", "75793F029A398702E99ED71F5D1BF070E27DA689D1892F46C7C57CC4CB9CFAE2" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            var new_transaction = new Transaction(json["products"].ToObject<List<Product>>(), json["sender"].ToString(), json["recipient"].ToString(), json["chainTokens"].ToObject<double>(), json["transactionFee"].ToObject<double>(),json["time"].ToObject<DateTime>());
            blockchain.newTransaction(new_transaction);

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.AreEqual("The transaction passed validation, but is already in present in the list of current transactions", result["response"].ToString());
        }

        [TestMethod]
        public void validateTansaction_TransactionWithProductNoInfo_ReturnsNoInfo()
        {
            // Arrange
            var blockchain = new Blockchain();

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", new List<Product>(){new Product(200) } },
                {"sender", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15" },
                {"recipient", "75793F029A398702E99ED71F5D1BF070E27DA689D1892F46C7C57CC4CB9CFAE2" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.AreEqual("No information about sender available, cannot validate", result["response"].ToString());
        }

        [TestMethod]
        public void validateTansaction_TransactionWithProductWrongNodeType_ReturnsWrongType()
        {
            // Arrange
            var blockchain = new Blockchain();

            // add node
            blockchain.registerNode(new NeighborNode("127.0.0.1:80", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15", "normal"));

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", new List<Product>(){new Product(200) } },
                {"sender", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15" },
                {"recipient", "75793F029A398702E99ED71F5D1BF070E27DA689D1892F46C7C57CC4CB9CFAE2" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.AreEqual("The sender does is not a member of the supplychain, and thus cannot send products", result["response"].ToString());
        }

        [TestMethod]
        public void validateTansaction_TransactionWithNewProductWrongNodeType_ReturnsWrongType()
        {
            // Arrange
            var blockchain = new Blockchain();

            // add node
            blockchain.registerNode(new NeighborNode("127.0.0.1:80", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15", "supplychain"));

            // make a list of product dictionary's
            var products = new List<Dictionary<string, dynamic>>()
            {
                new Dictionary<string, dynamic>()
                {
                    {"id", "new"},
                    {"weight", 200 }
                }
            };

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", products },
                {"sender", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15" },
                {"recipient", "75793F029A398702E99ED71F5D1BF070E27DA689D1892F46C7C57CC4CB9CFAE2" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.AreEqual("Product creator is not a verified diamondminer of the supplychain", result["response"].ToString());
        }

        [TestMethod]
        public void validateTansaction_TransactionWithNewProductCorrectNodeType_ReturnsValidationTrue()
        {
            // Arrange
            var blockchain = new Blockchain();

            // add node
            blockchain.registerNode(new NeighborNode("127.0.0.1:80", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15", "supplychain_miner"));

            // make a list of product dictionary's
            var products = new List<Dictionary<string, dynamic>>()
            {
                new Dictionary<string, dynamic>()
                {
                    {"id", "new"},
                    {"weight", 200 }
                }
            };

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", products },
                {"sender", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15" },
                {"recipient", "75793F029A398702E99ED71F5D1BF070E27DA689D1892F46C7C57CC4CB9CFAE2" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.IsTrue(result["validation"].ToObject<bool>());
        }

        [TestMethod]
        public void validateTansaction_TransactionWithExcistingProductNotPresent_ReturnsNotPresent()
        {
            // Arrange
            var blockchain = new Blockchain();

            // add node
            blockchain.registerNode(new NeighborNode("127.0.0.1:80", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15", "supplychain"));

            // make a list of product dictionary's
            var products = new List<Dictionary<string, dynamic>>()
            {
                new Dictionary<string, dynamic>()
                {
                    {"id", "SOME_INCORRECT_ID"},
                    {"weight", 200 }
                }
            };

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", products },
                {"sender", "EEF2732E3C84E230708F593C6E309CE48448B2E841ADBD9B3B506A3C3289EC15" },
                {"recipient", "75793F029A398702E99ED71F5D1BF070E27DA689D1892F46C7C57CC4CB9CFAE2" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.AreEqual("A product in the transaction is not present on the blockchain", result["response"].ToString());
        }

        [TestMethod]
        public void validateTansaction_TransactionWithExcistingProductIsPresentWrongOwner_ReturnsWrongOwner()
        {
            // Arrange
            var blockchain = new Blockchain();

            // add node
            blockchain.registerNode(new NeighborNode("127.0.0.1:80", "Node1", "supplychain_miner"));

            // make new transaction to be on the blockchain
            blockchain.newTransaction(new Transaction(new List<Product>() { new Product(200) }, "Node1", "Node2", 0, 0, DateTime.Now));

            string product_id = blockchain.getProducts().First().id;

            // make a list of product dictionary's
            var products = new List<Dictionary<string, dynamic>>()
            {
                new Dictionary<string, dynamic>()
                {
                    {"id", product_id},
                    {"weight", 200 }
                }
            };

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", products },
                {"sender", "Node1" },
                {"recipient", "Node2" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.AreEqual("A product in the transaction does not belong to sender", result["response"].ToString());
        }

        [TestMethod]
        public void validateTansaction_TransactionWithExcistingProductIsPresentCorrectOwner_ReturnsValidationTrue()
        {
            // Arrange
            var blockchain = new Blockchain();

            // add node
            blockchain.registerNode(new NeighborNode("127.0.0.1:80", "Node2", "supplychain"));

            // make new transaction to be on the blockchain
            blockchain.newTransaction(new Transaction(new List<Product>() { new Product(200) }, "Node1", "Node2", 0, 0, DateTime.Now));

            string product_id = blockchain.getProducts().First().id;

            // make a list of product dictionary's
            var products = new List<Dictionary<string, dynamic>>()
            {
                new Dictionary<string, dynamic>()
                {
                    {"id", product_id},
                    {"weight", 200 }
                }
            };

            // make input for method
            var input = new Dictionary<string, dynamic>()
            {
                {"products", products },
                {"sender", "Node2" },
                {"recipient", "Node1" },
                {"chainTokens", 0 },
                {"transactionFee" , 0 },
                {"time", DateTime.Now }
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.IsTrue(result["validation"].ToObject<bool>());
        }

    }
}
