using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace blockchain_dotnet_app
{
    public class Product
    {
        static int counter;
        public string id;
        //private node origin
        public int weight;

        public Product(int weight)
        {
            Interlocked.Increment(ref counter);
            this.weight = weight;

            this.setHash();
        }

        private void setHash()
        {
            this.id = Program.blockchain.hash(this);
        }
    }
}
