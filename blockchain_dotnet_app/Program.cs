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

            String[] transActions2 =
            {
                "nothing"
            };

            Block block1 = new Block(transActions, 0);

            Block block2 = new Block(transActions, block1.getHash());

            Block block3 = new Block(transActions, block2.getHash());

            Block block4 = new Block(transActions, block3.getHash());

            Block block5 = new Block(transActions, block4.getHash());

            Block block6 = new Block(transActions, block5.getHash());
            
            Console.WriteLine("hello world");
        }
    }
}
