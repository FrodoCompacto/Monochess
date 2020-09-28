using Microsoft.Xna.Framework.Content;
using MyJsonData.Models;
using Newtonsoft.Json;

namespace Chess.ContentReaders
{

    public class CrunchedJsonReader : ContentTypeReader<Atlas>
    {
        protected override Atlas Read(ContentReader input, Atlas existingInstance)
        {
            var json = input.ReadString();
            Atlas atlas = JsonConvert.DeserializeObject<Atlas>(json);
            return atlas;
        }
    }
}
