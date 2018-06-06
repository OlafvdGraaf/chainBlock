using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace blockchain_dotnet_app
{
    public class Transaction
    {
        public int id;
        static int counter;
        public List<Product> products;
        public string sender;
        public string recipient;
        public double chainTokens;
        public double transactionFee;

        public Transaction(List<Product> products, string sender, string recipient, double chainTokens, double transactionFee)
        {
            Interlocked.Increment(ref counter);
            this.id = counter;
            this.products = products;
            this.sender = sender;
            this.recipient = recipient;
            this.chainTokens = chainTokens;
            this.transactionFee = transactionFee;
        }
    }
}
