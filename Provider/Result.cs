namespace Provider
{
    using Newtonsoft.Json;

    public class Result
    {
        [JsonProperty("id")]
        private string Id { get; set; }

        [JsonProperty("displayName")]
        private string DisplayName { get; set; }

        [JsonProperty("description")]
        private string Description { get; set; }

        [JsonProperty("link")]
        private string Link { get; set; }

        [JsonProperty("image")]
        private string Image { get; set; }

    }
}
