using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace blockchain_dotnet_app
{
    class Transaction
    {
        private string sender;
        private string recipient;
        static int counter;
        private int id;
        private List<Product> products;
        private double chainTokens;
        private double transactionFee;

        public Transaction(List<Product> products, string sender, string recipient, double chainTokens, double transactionFee)
        {
            Interlocked.Increment(ref counter);
            this.id = counter;
            this.sender = sender;
            this.recipient = recipient;
            this.products = products;
            this.chainTokens = chainTokens;
            this.transactionFee = transactionFee;
        }
    }
}
