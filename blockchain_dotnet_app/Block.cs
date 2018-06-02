using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security;

namespace blockchain_dotnet_app
{
    public class Block
    {
        /*hash of previous transactions which may not be changed*/
        static int counter;
        public int id;
        public string hash;
        public int proof;
        public List<Transaction> transactions;

        public Block(List<Transaction> transactions, string hash, int proof)
        {
            Interlocked.Increment(ref counter);
            this.id = counter;
            this.transactions = transactions;
            this.hash = hash;
            this.proof = proof;
        }

        public int getAmountOfTransactions()
        {
            return this.transactions.Count<Transaction>();
        }

    }
}
