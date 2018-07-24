namespace Provider
{
    public class Query
    {
	    public Query(int age, string level, string domain, string mediumType, string language = "english", string programingLanguage = "", string duration = "", string price = "")
	    {
		    this.Age = age;
		    this.Level = level;
			this.Domain = domain;
		    this.MediumType = mediumType;
		    this.Language = language;
		    this.ProgramingLanguage = programingLanguage;
		    this.Duration = duration;
		    this.Price = price;
	    }

        private int Age { get; set; }

        private string Level { get; set; }

	    private string Domain { get; set; }

	    private string MediumType { get; set; }

	    private string Language { get; set; }

	    private string ProgramingLanguage { get; set; }

	    private string Duration { get; set; }

	    private string Price { get; set; }
    }
}
