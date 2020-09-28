using Microsoft.Xna.Framework.Content.Pipeline;
using MyJsonData.Models;
using Newtonsoft.Json;
using System.IO;

namespace MyJsonData
{

    [ContentImporter(".json", DefaultProcessor = "CrunchedJsonProcessor",
    DisplayName = "Crunched JSON Importer (-j -p -t -u)")]
    public class CrunchedJsonImporter : ContentImporter<Atlas>
    {
        public override Atlas Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing JSON file: {0}", filename);

            using (var streamReader = new StreamReader(filename))
            {
                var deserializer = new JsonSerializer();
                return (Atlas)deserializer.Deserialize(streamReader, typeof(Atlas));
            }
        }
    }
}
