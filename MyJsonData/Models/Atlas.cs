using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyJsonData.Models
{
    public partial class Atlas
    {
        [JsonProperty("textures")]
        public List<Texture> Textures { get; set; }
    }

    public partial class Texture
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("images")]
        public List<Image> Images { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("w")]
        public int W { get; set; }

        [JsonProperty("h")]
        public int H { get; set; }

        [JsonProperty("fx")]
        public int Fx { get; set; }

        [JsonProperty("fy")]
        public int Fy { get; set; }

        [JsonProperty("fw")]
        public int Fw { get; set; }

        [JsonProperty("fh")]
        public int Fh { get; set; }
    }
}