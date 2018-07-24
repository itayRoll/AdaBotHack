namespace Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class FileProvider : IProvider
    {
        public List<Result> GetResults(Query query)
        {
            RepositoryEntitys entities = JsonConvert.DeserializeObject<RepositoryEntitys>(Resource.Repository);

            // Filter mendatory fields
            List<RepositoryEntity> match = entities.Entities.Where(e =>
                e.Level.ToString() == query.Level && 
                e.Domain.ToString() == query.Domain &&
                e.MediumType.ToString() == query.MediumType).ToList();

            // Filter optional fields
            List<RepositoryEntity> optionalMatch = match.Where(e =>
                e.Language == query.Language &&
                e.ProgrammingLanguage == query.ProgrammingLanguage &&
                e.Duration == query.Duration &&
                e.ProgrammingLanguage == query.Price).ToList();

            return optionalMatch.Any() ? optionalMatch.Select(a => a.BuildResult()).ToList() : 
                match.Any() ? match.Select(a => a.BuildResult()).ToList() : new List<Result>();
        }
    }
}