using System;
using System.Windows.Forms;
using System.IO;
using HearthMirror;
using static HearthMirror.Enums.MirrorStatus;

namespace HearthstoneCollectionExporter
{
    static class HearthstoneCollectionExporter
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var status = HearthMirror.Status.GetStatus().MirrorStatus;
            if (status == Ok)
            {
                // Found Hearthstone process, attempting to get cards and write them to CSV.
                var collection = Reflection.GetCollection();

                using (var w = new StreamWriter("cards.csv"))
                {
                    var header = "cardId,isGolden,cardCount";
                    w.WriteLine(header);

                    foreach (var card in collection)
                    {
                        var cardId = card.Id;
                        var cardCount = card.Count;
                        var isGolden = card.Premium;
                        var newLine = string.Format("{0},{1},{2}", cardId, isGolden, cardCount);
                        w.WriteLine(newLine);
                        w.Flush();
                    }
                }
                MessageBox.Show("Finished exporting cards to cards.csv.", "Success");

            }
            else if (status == ProcNotFound)
            {
                MessageBox.Show("Unable to find Hearthstone the process.", "Error");
            }
            else if (status == Error)
            {
                MessageBox.Show("There was a problem finding the Hearthstone process.", "Error");
            }
        }
    }
}
