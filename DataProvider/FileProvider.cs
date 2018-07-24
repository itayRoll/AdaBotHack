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
            List<RepositoryEntity> mendatoryMatch = entities.Entities.Where(e =>
                e.Level.ToString() == query.Level && 
                e.Domain.ToString() == query.Domain &&
                e.MediumType.ToString() == query.MediumType).ToList();

            // Filter optional fields
            List<RepositoryEntity> optionalMatch = mendatoryMatch;
            if (!string.IsNullOrEmpty(query.Language))
            {
                optionalMatch = optionalMatch.Where(e => e.Language == query.Language).ToList();
            }

            if (!string.IsNullOrEmpty(query.ProgrammingLanguage))
            {
                optionalMatch = optionalMatch.Where(e => e.ProgrammingLanguage == query.ProgrammingLanguage).ToList();
            }

            if (!string.IsNullOrEmpty(query.Duration))
            {
                optionalMatch = optionalMatch.Where(e => e.Duration == query.Duration).ToList();
            }

            if (!string.IsNullOrEmpty(query.Price))
            {
                optionalMatch = optionalMatch.Where(e => e.Price == query.Price).ToList();
            }

            return optionalMatch.Any() ? optionalMatch.Select(a => a.BuildResult()).ToList() :
                mendatoryMatch.Any() ? mendatoryMatch.Select(a => a.BuildResult()).ToList() : new List<Result>();
        }
    }
}