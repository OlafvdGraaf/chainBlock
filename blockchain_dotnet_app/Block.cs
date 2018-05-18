using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace blockchain_dotnet_app
{
    class Block
    {
        /*hash of previous transactions which may not be changed*/
        private string previousHash;
        private string[] transactions;
        private string hash;


        public Block(string[] transactions, string previousHash)
        {
            this.transactions = transactions;
            this.previousHash = previousHash;

            this.updateBlock();
        }

        public int getAmountOfTransactions()
        {
            return this.transactions.Count<string>();
        }

        public string getHash()
        {
            return hash;
        }

        public void updateBlock()
        {
            string[] combinedValues = { this.generateHash<string>(this.transactions), this.previousHash };
            this.hash = this.generateHash<string>(combinedValues);
            Console.WriteLine("A new Hash was generated for the Block: {0}", this.hash);
            Console.WriteLine(" ");
        }

        private string generateHash<T>(T[] transactions)
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
