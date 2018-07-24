namespace Provider
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json;

    public class FileProvider : IProvider
    {
        public List<Result> GetResults(Query query)
        {
            List<RepositoryEntity> entities;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DataProvider.repository.json";

            using (Stream sr = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader r = new StreamReader(sr))
            {
                var json = r.ReadToEnd();
                entities = JsonConvert.DeserializeObject<List<RepositoryEntity>>(json);
            }

            // Filter mendatory fields
            List<RepositoryEntity> mendatoryMatch = entities.Where(e =>
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

            return optionalMatch.Any() ? optionalMatch.Select(a => a.BuildResult()).ToList() :
                mendatoryMatch.Any() ? mendatoryMatch.Select(a => a.BuildResult()).ToList() : new List<Result>();
        }
    }
}