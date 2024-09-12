using System.Collections.Concurrent;
using DotNetEnv;
using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class BackgroundObserver : IHostedService, IDisposable
    {
        private int GetNumberThreads()
        {
            if (int.TryParse(Environment.GetEnvironmentVariable("NUMBER_THREADS"), out int threads))
            {
                return threads;
            }
            return 1;
        }

        private string GetConnectionString()
        {
            string databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
            string databasePass = Environment.GetEnvironmentVariable("DATABASE_PASS");
            string databaseUser = Environment.GetEnvironmentVariable("DATABASE_USER");
            string databaseServer = Environment.GetEnvironmentVariable("DATABASE_SERVER");
            string databasePort = Environment.GetEnvironmentVariable("DATABASE_PORT");

            return $"Host={databaseServer};Port={databasePort};Username={databaseUser};Password={databasePass};Database={databaseName};";
        }

        private readonly string _connectionString;
        private readonly int _numberThreads;
        private Timer? _timer;
        private readonly RecordsQueue _DataQueue;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ConcurrentBag<Task> _workerTasks = new();

        public BackgroundObserver(RecordsQueue data)
        {
            _DataQueue = data;
            Env.Load();
            _connectionString = GetConnectionString();
            _numberThreads = GetNumberThreads();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(StartObserving, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            StartWorkerThreads();
            return Task.CompletedTask;
        }

        private void StartObserving(object? state)
        {
            string sql =
                @"
        SELECT ""Id"", ""Url"", ""BoundaryRegExp"", ""Days"", ""Hours"", ""Minutes"", ""Label"", ""IsActive"", ""Tags"", ""LastChange""
        FROM public.""WebsiteRecord"";
    ";

            try
            {
                var websiteRecords = DataAccess.LoadData<WebsiteRecordModel, dynamic>(
                    sql,
                    new { },
                    _connectionString
                );

                foreach (WebsiteRecordModel record in websiteRecords)
                {
                    if (record.LastChange.HasValue)
                    {
                        DateTime nextChangeTime = record
                            .LastChange.Value.AddDays(record.Days)
                            .AddHours(record.Hours)
                            .AddMinutes(record.Minutes);

                        if (nextChangeTime <= DateTime.Now)
                        {
                            record.LastChange = null;
                            _DataQueue.Add(record);
                            Console.WriteLine($"Queued WebsiteRecord for action: {record.Url}");
                            updateRecord(record, _connectionString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking website records: {ex.Message}");
            }
        }

        private void updateRecord(WebsiteRecordModel record, string connectionString)
        {
            string updateSql =
                @"
        UPDATE public.""WebsiteRecord""
        SET ""LastChange"" = @LastChange
        WHERE ""Id"" = @Id;
    ";

            var parameters = new { LastChange = record.LastChange, Id = record.Id };

            try
            {
                DataAccess.SaveData(updateSql, parameters, connectionString);
                Console.WriteLine(
                    $"Updated LastChange for WebsiteRecord ID: {record.Id} with {record.LastChange}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error updating WebsiteRecord ID: {record.Id}, Error: {ex.Message}"
                );
                throw;
            }
        }

        private void StartWorkerThreads()
        {
            for (int i = 0; i < _numberThreads; i++)
            {
                var workerTask = Task.Run(() => WorkerThread(_cancellationTokenSource.Token));
                _workerTasks.Add(workerTask);
            }
        }

        private async Task WorkerThread(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_DataQueue.WorkQueue.TryTake(out var record))
                {
                    WebsiteRecordCleaner(record.Id, _connectionString);
                    FrontNodeManager manager = new();
                    foreach (
                        var node in manager.ManageUrls(
                            new Uri(record.Url),
                            record.BoundaryRegExp,
                            record.Id
                        )
                    )
                    {
                        NodeSniffer(node);
                    }
                    if (record.IsActive)
                    {
                        record.LastChange = DateTime.Now;
                        updateRecord(record, _connectionString);
                    }
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), token);
                }
            }
        }

        private void WebsiteRecordCleaner(int id, string connectionString)
        {
            string deleteNodeNeighboursSql =
                @"
        DELETE FROM public.""NodeNeighbour""
        WHERE ""NodeId"" IN (SELECT ""Id"" FROM public.""Node"" WHERE ""WebsiteRecordId"" = @Id)
        OR ""NeighbourNodeId"" IN (SELECT ""Id"" FROM public.""Node"" WHERE ""WebsiteRecordId"" = @Id);
    ";

            string deleteNodesSql =
                @"
        DELETE FROM public.""Node""
        WHERE ""WebsiteRecordId"" = @Id;
    ";

            try
            {
                DataAccess.SaveData(deleteNodeNeighboursSql, new { Id = id }, connectionString);
                Console.WriteLine($"Deleted NodeNeighbour for WebsiteRecord ID: {id}");

                DataAccess.SaveData(deleteNodesSql, new { Id = id }, connectionString);
                Console.WriteLine($"Deleted Nodes for WebsiteRecord ID: {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error cleaning nodes and neighbours for WebsiteRecord ID: {id}, Error: {ex.Message}"
                );
                throw;
            }
        }

        private void NodeSniffer(Node? node)
        {
            if (node.HasValue)
            {
                List<int> ids = new List<int>();
                foreach (var nextUrl in node.Value.nextUrls)
                {
                    int? nodeId = DataVerifier.GetNodeIdInWebsiteRecord(
                        nextUrl.OriginalString,
                        node.Value.WebsiteRecordId,
                        _connectionString
                    );
                    if (nodeId.HasValue)
                    {
                        ids.Add(nodeId.Value);
                    }
                    else
                    {
                        int newNodeId = AddNode(
                            nextUrl.OriginalString,
                            node.Value.WebsiteRecordId,
                            _connectionString
                        );
                        ids.Add(newNodeId);
                    }
                }
                int? currentNodeId = DataVerifier.GetNodeIdInWebsiteRecord(
                    node.Value.url.OriginalString,
                    node.Value.WebsiteRecordId,
                    _connectionString
                );
                if (!currentNodeId.HasValue)
                {
                    currentNodeId = AddNode(
                        node.Value.url.OriginalString,
                        node.Value.WebsiteRecordId,
                        _connectionString
                    );
                }
                UpdateNodeWithTitle(currentNodeId, node.Value.Title, _connectionString);
                foreach (int NeighbourNodeId in ids)
                {
                    AddNeighbour(NeighbourNodeId, currentNodeId, _connectionString);
                }
            }
        }

        private void UpdateNodeWithTitle(int? currentNodeId, string title, string connectionString)
        {
            if (!currentNodeId.HasValue)
            {
                Console.WriteLine("missing current node id");
                return;
            }

            string sql =
                @"
        UPDATE public.""Node""
        SET ""Title"" = @Title
        WHERE ""Id"" = @Id;
    ";

            var parameters = new { Title = title, Id = currentNodeId.Value };

            try
            {
                DataAccess.SaveData(sql, parameters, connectionString);
                Console.WriteLine($"Updated Node ID: {currentNodeId.Value} with Title: {title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating node title: {ex.Message}");
                throw;
            }
        }

        private void AddNeighbour(int neighbourNodeId, int? nodeId, string connectionString)
        {
            if (!nodeId.HasValue)
            {
                Console.WriteLine("Invalid node ID, unable to add neighbour.");
                return;
            }

            string sql =
                @"
        INSERT INTO public.""NodeNeighbour"" (""NodeId"", ""NeighbourNodeId"")
        VALUES (@NodeId, @NeighbourNodeId)
        ON CONFLICT DO NOTHING;
    ";

            var parameters = new { NodeId = nodeId.Value, NeighbourNodeId = neighbourNodeId };

            try
            {
                DataAccess.SaveData(sql, parameters, connectionString);
                Console.WriteLine(
                    $"Added neighbour with NodeId: {nodeId.Value} and NeighbourNodeId: {neighbourNodeId}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding neighbour: {ex.Message}");
                throw;
            }
        }

        private int AddNode(string url, int websiteRecordId, string connectionString)
        {
            string sql =
                @"
        INSERT INTO public.""Node"" (""Title"", ""CrawlTime"", ""UrlMain"", ""WebsiteRecordId"")
        VALUES (@Title, @CrawlTime, @UrlMain, @WebsiteRecordId)
        RETURNING ""Id"";
    ";

            var parameters = new
            {
                Title = url,
                CrawlTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                UrlMain = url,
                WebsiteRecordId = websiteRecordId,
            };

            try
            {
                var newNodeId = DataAccess
                    .LoadData<int, dynamic>(sql, parameters, connectionString)
                    .FirstOrDefault();
                Console.WriteLine($"Added new node with ID: {newNodeId} for URL: {url}");
                return newNodeId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding node: {ex.Message}");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
