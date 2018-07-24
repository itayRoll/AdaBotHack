namespace Provider
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RepositoryEntity
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("mediumType")]
        public MediumType MediumType { get; set; }

        [JsonProperty("domain")]
        public DomainType Domain { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("level")]
        public LevelType Level { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("programmingLanguage")]
        public string ProgrammingLanguage { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("link")]
        public string ContentUrl { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("additionalInfo")]
        public HashSet<string> AdditionalInfo { get; set; }

        /// <summary>
        /// Builds the result object based on the repository entity
        /// </summary>
        /// <returns>The result object</returns>
        public Result BuildResult()
        {
            Result ret = new Result()
            {
                Description = this.Description,
                DisplayName = this.DisplayName,
                Id = this.Id,
                Image = this.ImageUrl,
                Link = this.ContentUrl
            };

            return ret;
        }
    }
}
