using Microsoft.Xna.Framework.Content.Pipeline;
using MyJsonData.Models;

namespace MyJsonData
{

    [ContentProcessor(DisplayName = "Crunched JSON Processor (-j -p -t -u)")]
    public class CrunchedJsonProcessor : ContentProcessor<Atlas, Atlas>
    {
        public override Atlas Process(Atlas input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing JSON");

            return input;
        }
    }
}
