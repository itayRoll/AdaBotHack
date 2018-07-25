using System;

namespace DataProvider
{
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

			var mandatoryMatch = entities.Where(e =>
				e.Level.ToString().ToLower() == query.Level.ToLower() &&
				e.MediumType.ToString().ToLower() == query.MediumType.ToLower()).ToList();

			// Filter out by domain only if query is not anything
	        if (!string.Equals(query.Domain, "anything", StringComparison.OrdinalIgnoreCase))
	        {
		        mandatoryMatch = mandatoryMatch.Where(e =>
			        string.Equals(e.Domain.ToString(), query.Domain, StringComparison.OrdinalIgnoreCase)).ToList();
	        }

            // Filter optional fields
            List<RepositoryEntity> optionalMatch = mandatoryMatch;
            if (!string.IsNullOrEmpty(query.Language))
            {
                optionalMatch = optionalMatch.Where(e => e.Language.ToLower() == query.Language.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(query.ProgrammingLanguage))
            {
                optionalMatch = optionalMatch.Where(e => e.ProgrammingLanguage.ToLower() == query.ProgrammingLanguage.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(query.Duration))
            {
                optionalMatch = optionalMatch.Where(e => e.Duration.ToLower() == query.Duration.ToLower()).ToList();
            }

            return optionalMatch.Any() ? optionalMatch.Select(a => a.BuildResult()).ToList() :
	            mandatoryMatch.Any() ? mandatoryMatch.Select(a => a.BuildResult()).ToList() : new List<Result>();
        }
    }
}