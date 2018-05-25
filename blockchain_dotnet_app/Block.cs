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
    class Block
    {
        /*hash of previous transactions which may not be changed*/
        static int counter;
        private int id;
        private string previousHash;
        private List<Transaction> transactions;
        private string hash;
        private int proof;


        public Block(List<Transaction> transactions, string previousHash, int proof)
        {
            Interlocked.Increment(ref counter);
            this.id = counter;
            this.transactions = transactions;
            this.previousHash = previousHash;
            this.proof = proof;
        }

        public int getAmountOfTransactions()
        {
            return this.transactions.Count<Transaction>();
        }

        public int getId()
        {
            return this.id;
        }

        public string getHash()
        {
            return hash;
        }

    }
}
