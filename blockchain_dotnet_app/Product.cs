using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace blockchain_dotnet_app
{
    public class Product
    {
        static int counter;
        public int id;
        //private node origin
        public int weight;

        public Product(int weight)
        {
            Interlocked.Increment(ref counter);
            this.id = counter;
            this.weight = weight;
        }
    }
}
