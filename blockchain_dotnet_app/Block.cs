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
        private int previousHash;
        private String[] transactions;
        private int hash;


        public Block(String[] transactions, int previousHash)
        {
            this.transactions = transactions;
            this.previousHash = previousHash;

            this.updateBlock();
        }

        public int getHash()
        {
            return hash;
        }

        public void addTransaction(String transaction)
        {
            List<String> transList = this.transactions.ToList<String>();
            transList.Add(transaction);
            this.transactions = transList.ToArray<String>();

            this.updateBlock();
        }

        private void updateBlock()
        {
            int[] combinedValues = { this.generateHash<String>(this.transactions), this.previousHash };
            this.hash = this.generateHash<int>(combinedValues);
            Console.WriteLine("A new Hash was generated for the Block: {0}", this.hash);
        }

        private int generateHash<T>(T[] transactions)
        {
            SHA256 sha256 = SHA256Managed.Create();

            int result = 9;
            foreach (var t in transactions)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(t.ToString());
                byte[] hash = sha256.ComputeHash(bytes);

                result = result * BitConverter.ToInt32(hash, 0);
            }
            return result;
        }
    }
}
