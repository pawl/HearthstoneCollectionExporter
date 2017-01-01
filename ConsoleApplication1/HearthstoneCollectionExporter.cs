using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HearthMirror;
using static HearthMirror.Enums.MirrorStatus;

namespace HearthstoneCollectionExporter
{
    class MainClass
    {
        static void Main(string[] args)
        {
            var status = HearthMirror.Status.GetStatus().MirrorStatus;
            if (status == Ok)
            {
                Console.WriteLine("Found Hearthstone process.");
            }
            else if (status == ProcNotFound)
            {
                Console.WriteLine("Unable to find Hearthstone process.");
                Environment.Exit(0);
            }
            else if (status == Error)
            {
                Console.WriteLine("There was a problem finding the Hearthstone process.");
                Environment.Exit(0);
            }

            var collection = Reflection.GetCollection();

            using (var w = new StreamWriter("cards.csv"))
            {
                var header = "cardId,cardCount";
                w.WriteLine(header);

                foreach (var card in collection)
                {
                    var cardId = card.Id;
                    var cardCount = card.Count;
                    var newLine = string.Format("{0},{1}", cardId, cardCount);
                    w.WriteLine(newLine);
                    w.Flush();
                }
            }

            Console.WriteLine("Finished exporting cards to cards.csv.");
        }
    }
}
