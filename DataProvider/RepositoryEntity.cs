namespace Provider
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RepositoryEntity
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        [JsonProperty("MediumType")]
        public MediumType MediumType { get; set; }

        [JsonProperty("Domain")]
        public DomainType Domain { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }

        [JsonProperty("Level")]
        public LevelType Level { get; set; }

        [JsonProperty("Language")]
        public string Language { get; set; }

        [JsonProperty("ProgrammingLanguage")]
        public string ProgrammingLanguage { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Link")]
        public string ContentUrl { get; set; }

        [JsonProperty("ImageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("AdditionalInfo")]
        public string AdditionalInfo { get; set; }

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
