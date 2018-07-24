namespace Provider
{
    using System.Collections.Generic;

    public class RepositoryEntity
    {        
        private long Id { get; set; }

        private string DisplayName { get; set; }

        private MediumType MediumType { get; set; }

        private DomainType Domain { get; set; }

        private string Duration { get; set; }

        private LevelType Level { get; set; }

        private string Language { get; set; }

        private string CodeLanguage { get; set; }

        private string Description { get; set; }

        private string ContentUrl { get; set; }

        private string ImageUrl { get; set; }

        private HashSet<string> AdditionalInfo { get; set; }
    }
}
