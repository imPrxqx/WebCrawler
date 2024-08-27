public class WebsiteRecordModel
{
    public int Id { get; set; }

    public string Url { get; set; }
    public string BoundaryRegExp { get; set; }

    public int Days { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }

    public string Label { get; set; }
    public bool IsActive { get; set; }
    public string Tags { get; set; }

    public TimeSpan GetPeriodicity()
    {
        return new TimeSpan(Days, Hours, Minutes, 0);
    }
}
