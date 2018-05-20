using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace blockchain_dotnet_app
{
    class Transaction
    {
        //sender
        //recipient
        static int counter;
        private int id;
        private List<Product> products;
        private double chainTokens;
        private double transactionFee;

        public Transaction(List<Product> products, double chainTokens, double transactionFee)
        {
            Interlocked.Increment(ref counter);
            this.id = counter;
            this.products = products;
            this.chainTokens = chainTokens;
            this.transactionFee = transactionFee;
        }



    }
}
