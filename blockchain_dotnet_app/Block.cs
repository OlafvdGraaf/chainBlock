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


        public Block(List<Transaction> transactions, string previousHash)
        {
            Interlocked.Increment(ref counter);
            this.id = counter;
            this.transactions = transactions;
            this.previousHash = previousHash;

            this.updateBlock();
        }

        public int getAmountOfTransactions()
        {
            return this.transactions.Count<Transaction>();
        }

        public string getHash()
        {
            return hash;
        }

        public void updateBlock()
        {
            List<string> combinedValues = new List<string>{ this.generateHash<Transaction>(this.transactions), this.previousHash };
            this.hash = this.generateHash<string>(combinedValues);
            Console.WriteLine("A new Hash was generated for the Block: {0}", this.hash);
            Console.WriteLine(" ");
        }

        private string generateHash<T>(List<T> transactions)
        {
            SHA256 sha256 = SHA256Managed.Create();

            string result = "";
            foreach (var t in transactions)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(t.ToString());
                byte[] hash = sha256.ComputeHash(bytes);

                result += BitConverter.ToString(hash, 0).Replace("-", "");
            }
            return result;
        }
    }
}
