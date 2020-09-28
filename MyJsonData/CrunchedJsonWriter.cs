using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MyJsonData.Models;
using Newtonsoft.Json;

namespace MyJsonData
{

    [ContentTypeWriter]
    class CrunchedJsonWriter : ContentTypeWriter<Atlas>
    {
        protected override void Write(ContentWriter output, Atlas value)
        {
            output.Write(JsonConvert.SerializeObject(value));
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Atlas).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Chess.ContentReaders.CrunchedJsonReader, Chess";
        }
    }
}
