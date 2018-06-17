using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace blockchain_dotnet_app
{
    public class Transaction
    {
        public string id;
        public List<Product> products;
        public string sender;
        public string recipient;
        public double chainTokens;
        public double transactionFee;
        public DateTime time;

        public Transaction(List<Product> products, string sender, string recipient, double chainTokens, double transactionFee, DateTime time)
        {
            this.products = products;
            this.sender = sender;
            this.recipient = recipient;
            this.chainTokens = chainTokens;
            this.transactionFee = transactionFee;
            this.time = time;

            this.setHash();
        }

        private void setHash()
        {
            this.id = Program.blockchain.hash(this);
        }
    }
}
