namespace Challenge.Front.ViewModels;

public class HackNewsViewModel
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

    public DateTime ToDate
    {
        get
        {
            var dateOffSet = DateTimeOffset.FromUnixTimeSeconds(Time);

            return dateOffSet.UtcDateTime;
        }
    }
}

