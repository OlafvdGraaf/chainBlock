using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using blockchain_dotnet_app;
using Newtonsoft.Json.Linq;

namespace blockchain_dotnet_app.Tests
{
    [TestClass]
    public class Blockchain_TransactionValidationTests
    {

        [TestMethod]
        public void validateTransaction_MissingJsonValues_ReturnsValidationFalse()
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
            };
            var json = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(input));

            // Act
            var result = blockchain.validateTransaction(json);

            // Assert
            Assert.AreEqual("Missing values", result["response"].ToString());
            
        }
    }
}
