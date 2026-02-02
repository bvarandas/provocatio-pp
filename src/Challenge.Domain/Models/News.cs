namespace Challenge.Domain.Models;

public class News
{
    public int Id { get; set; }
    public string By { get; set; } = string.Empty;
    public int Descendants { get; set; }
    public List<int> Kids { get; set; } = null!;
    public int Score { get; set; }
    public long Time { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public News() { }

    public News(string by, int descendants, List<int> kids, int score, string title, string type, string url)
    {
        By = by;
        Descendants = descendants;
        Kids = kids;
        Score = score;
        Title = title;
        Type = type;
        Url = url;
    }

    public News(int id, string by, int descendants, List<int> kids, int score, long time, string title, string type, string url)
    {
        Id = id;
        By = by;
        Descendants = descendants;
        Kids = kids;
        Score = score;
        Time = time;
        Title = title;
        Type = type;
        Url = url;
    }
}
