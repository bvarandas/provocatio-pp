namespace Challenge.Application.Commands;

public class InsertNewsCache : NewsCommand
{
    public InsertNewsCache(string by, int descendants, List<int> kids, int score, long time, string title, string tyoe, string url)
    {
        By = by;
        Descendants = descendants;
        Kids = kids;
        Score = score;
        Time = time;
        Title = title;
        Type = tyoe;
        Url = url;
    }

    public InsertNewsCache(int id, string by, int descendants, List<int> kids, int score, long time, string title, string tyoe, string url)
    {
        Id = id;
        By = by;
        Descendants = descendants;
        Kids = kids;
        Score = score;
        Time = time;
        Title = title;
        Type = tyoe;
        Url = url;
    }
}