using System.Collections.Concurrent;
using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class RecordsQueue
    {
        public ConcurrentBag<WebsiteRecordModel> WorkQueue { get; set; } = new();

        public void Add(WebsiteRecordModel record)
        {
            WorkQueue.Add(record);
        }

        public bool TryTake(out WebsiteRecordModel? record)
        {
            return WorkQueue.TryTake(out record);
        }

        public bool IsEmpty => WorkQueue.IsEmpty;
    }
}
