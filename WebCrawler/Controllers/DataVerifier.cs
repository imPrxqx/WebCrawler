using Npgsql;
using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class DataVerifier
    {
        public static bool WebsiteRecordVerifier(WebsiteRecordModel model)
        {
            if (model == null)
                return false;
            if (!bool.TryParse(model.IsActive.ToString(), out bool isActive))
            {
                model.IsActive = false;
            }

            if (!int.TryParse(model.Minutes.ToString(), out int minutes))
            {
                model.Minutes = 0;
            }

            if (!int.TryParse(model.Hours.ToString(), out int hours))
            {
                model.Hours = 0;
            }

            if (!int.TryParse(model.Days.ToString(), out int days))
            {
                model.Days = 0;
            }

            if (
                model == null
                && model.Minutes <= 0
                && model.Minutes > 60
                && model.Hours <= 0
                && model.Hours > 24
                && model.Days <= 0
                && model.Days > 31
                && string.IsNullOrWhiteSpace(model.Url)
                && string.IsNullOrWhiteSpace(model.BoundaryRegExp)
            )
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Label))
            {
                model.Label = model.Url;
            }

            if (string.IsNullOrWhiteSpace(model.Tags))
            {
                model.Tags = "";
            }
            if (model.LastChange == null)
            {
                model.LastChange = DateTime
                    .Now.AddMinutes(-model.Minutes)
                    .AddHours(-model.Hours)
                    .AddDays(-model.Days);
            }
            return true;
        }

        public static bool DoesWebsiteRecordExist(int websiteRecordId, string connectionString)
        {
            string sql = @"SELECT ""Id"" FROM ""WebsiteRecord"" WHERE ""Id"" = @Id;";
            var result = DataAccess.LoadData<WebsiteRecordModel, dynamic>(
                sql,
                new { Id = websiteRecordId },
                connectionString
            );
            return result.Any();
        }

        public static int? GetNodeIdInWebsiteRecord(
            string url,
            int websiteRecordId,
            string connectionString
        )
        {
            string sql =
                @"
    SELECT ""Id""
    FROM public.""Node""
    WHERE ""UrlMain"" = @Url AND ""WebsiteRecordId"" = @WebsiteRecordId;
    ";

            var result = DataAccess.LoadData<int?, dynamic>(
                sql,
                new { Url = url, WebsiteRecordId = websiteRecordId },
                connectionString
            );

            return result.FirstOrDefault();
        }

        public static bool AreNodesFromSameWebsiteRecord(
            int nodeId,
            int neighbourNodeId,
            string connectionString
        )
        {
            string sql =
                @"
                SELECT n1.""WebsiteRecordId"", n2.""WebsiteRecordId""
                FROM ""Node"" n1
                INNER JOIN ""Node"" n2 ON n1.""WebsiteRecordId"" = n2.""WebsiteRecordId""
                WHERE n1.""Id"" = @NodeId AND n2.""Id"" = @NeighbourNodeId;
            ";

            var result = DataAccess.LoadData<dynamic, dynamic>(
                sql,
                new { NodeId = nodeId, NeighbourNodeId = neighbourNodeId },
                connectionString
            );

            return result.Any();
        }
    }
}
