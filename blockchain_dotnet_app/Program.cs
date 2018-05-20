using System;
using System.Collections.Generic;

namespace blockchain_dotnet_app
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //String[] transActions = {
            //    "Diamant was won at Canada",
            //    "Diamant arrived at Walmart",
            //    "Diamant arrived in New York"
            //};

           /* List if 3 transactions
            Transaction(List<Product>, chainTokenAmount : double, transactionFee : double) 
            Product(Weight of diamond in grams : int)
           */
            List<Transaction> transactions = new List<Transaction>{
                new Transaction(new List<Product>{new Product(100)},0,0.5),
                new Transaction(new List<Product>{new Product(105), new Product(170) },0,0.9),
                new Transaction(new List<Product>{new Product(210), new Product(136) },0,1.2),
            };



            ChainIterator iter = new ChainIterator(new Block(transactions, "nothing"));

            iter.addTransaction(new Transaction(new List<Product> { new Product(100) }, 0, 0.5));
            iter.addTransaction(new Transaction(new List<Product> { new Product(100) }, 0, 0.5));
            iter.addTransaction(new Transaction(new List<Product> { new Product(100) }, 0, 0.5));
            iter.addTransaction(new Transaction(new List<Product> { new Product(100) }, 0, 0.5));
            iter.addTransaction(new Transaction(new List<Product> { new Product(100) }, 0, 0.5));
            iter.addTransaction(new Transaction(new List<Product> { new Product(100) }, 0, 0.5));

            Console.ReadLine();
        }
    }
}
