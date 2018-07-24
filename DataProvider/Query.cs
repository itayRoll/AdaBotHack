namespace Provider
{
    public class Query
    {
	    public Query(int age, string level, string domain, string mediumType, string language = "", string programingLanguage = "", string duration = "", string price = "")
	    {
		    this.Age = age;
		    this.Level = level;
			this.Domain = domain;
		    this.MediumType = mediumType;
		    this.Language = language;
		    this.ProgrammingLanguage = programingLanguage;
		    this.Duration = duration;
		    this.Price = price;
	    }

        public int Age { get; set; }
        public string Level { get; set; }
        public string Domain { get; set; }
        public string MediumType { get; set; }
        public string Language { get; set; }
        public string ProgrammingLanguage { get; set; }
        public string Duration { get; set; }
        public string Price { get; set; }
    }
}
