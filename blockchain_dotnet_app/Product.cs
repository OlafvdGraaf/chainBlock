using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace blockchain_dotnet_app
{
    class Product
    {
        static int counter;
        private int id;
        //private node origin
        private int weight;

        public Product(int weight)
        {
            Interlocked.Increment(ref counter);
            this.id = counter;
            this.weight = weight;
        }
    }
}
