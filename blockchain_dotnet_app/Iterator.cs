using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace blockchain_dotnet_app
{
    class ChainIterator
    {
        List<Block> chain;
        private List<Transaction> transactions;

        public ChainIterator(Block firstBlock)
        {
            this.chain = new List<Block>();
            this.addBlock(firstBlock);
            this.transactions = new List<Transaction>();
        }

        public void addTransaction(Transaction transaction)
        {
            if (this.enoughTransactions())
            {
                this.addBlock(new Block(this.transactions, this.chain.Last<Block>().getHash()));
                Console.WriteLine("new block was made");
                Console.WriteLine(" ");
                this.transactions.Clear();
            }
            this.transactions.Add(transaction);
        }

        private bool enoughTransactions()
        {
            return this.transactions.Count < 3 ? false : true;
        }

        private bool addBlock(Block block)
        {
            this.chain.Add(block);
            return true;
        }
    }
}
