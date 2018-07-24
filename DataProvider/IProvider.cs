namespace Provider
{
    using System.Collections.Generic;

    public interface IProvider
    {
        List<Result> GetResults(Query query);
    }
}
