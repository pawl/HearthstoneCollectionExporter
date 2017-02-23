using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using CsvHelper;
using HearthMirror;
using HearthDb;

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
            if (status == HearthMirror.Enums.MirrorStatus.Ok)
            {
                // Found Hearthstone process, attempting to get cards and write them to CSV.
                var goldenCollection = Reflection.GetCollection().Where(x => x.Premium);
                var commonCollection = Reflection.GetCollection().Where(x => !x.Premium);

                using (var textWriter = new StreamWriter("cards.csv"))
                {
                    var csv = new CsvWriter(textWriter);

                    // Write headers
                    csv.WriteField("Id");
                    csv.WriteField("Name");
                    csv.WriteField("Class");
                    csv.WriteField("Rarity");
                    csv.WriteField("Mana");
                    csv.WriteField("Type");
                    csv.WriteField("Race");
                    csv.WriteField("Health");
                    csv.WriteField("Attack");
                    csv.WriteField("Card Count - Normal");
                    csv.WriteField("Card Count - Golden");
                    csv.WriteField("Set");
                    csv.NextRecord();

                    foreach (var dbCard in Cards.Collectible)
                    {
                        csv.WriteField(dbCard.Key); // Id
                        csv.WriteField(dbCard.Value.Name);
                        csv.WriteField(dbCard.Value.Class);
                        csv.WriteField(dbCard.Value.Rarity);
                        csv.WriteField(dbCard.Value.Cost); // Mana
                        csv.WriteField(dbCard.Value.Type);

                        if (dbCard.Value.Race == HearthDb.Enums.Race.INVALID)
                        {
                            csv.WriteField(null);
                        }
                        else
                        { 
                            csv.WriteField(dbCard.Value.Race);

                        }

                        csv.WriteField(dbCard.Value.Health);
                        csv.WriteField(dbCard.Value.Attack);

                        var amountNormal =
                            commonCollection.Where(x => x.Id.Equals(dbCard.Key)).Select(x => x.Count).FirstOrDefault();
                        var amountGolden =
                            goldenCollection.Where(x => x.Id.Equals(dbCard.Key)).Select(x => x.Count).FirstOrDefault();
                        csv.WriteField(amountNormal);
                        csv.WriteField(amountGolden);

                        csv.WriteField(dbCard.Value.Set);

                        csv.NextRecord();
                    }
                }
                MessageBox.Show("Finished exporting cards to cards.csv.", "Success");
            }
            else if (status == HearthMirror.Enums.MirrorStatus.ProcNotFound)
            {
                MessageBox.Show("Unable to find Hearthstone the process.", "Error");
            }
            else if (status == HearthMirror.Enums.MirrorStatus.Error)
            {
                MessageBox.Show("There was a problem finding the Hearthstone process.", "Error");
            }
        }
    }
}
