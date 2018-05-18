using System;

namespace blockchain_dotnet_app
{
    class Program
    {
        static void Main(string[] args)
        {
            
            String[] transActions = {
                "Diamant was won at Canada",
                "Diamant arrived at Walmart",
                "Diamant arrived in New York"
            };

            ChainIterator iter = new ChainIterator(new Block(transActions, "nothing"));

            iter.addTransaction("Diamant was won in south America");
            iter.addTransaction("Diamant arrived at Amsterdam");
            iter.addTransaction("Diamant was sold");

            Console.ReadLine();
        }
    }
}
