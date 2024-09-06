using Npgsql;
using WebCrawler.Models;

namespace WebCrawler.Controllers
{
    public class DataVerifier
    {
        public static bool WebsiteRecordVerifier(WebsiteRecordModel model)
        {
            if (
                model == null
                || model.Minutes < 0
                || model.Minutes > 60
                || model.Hours < 0
                || model.Hours > 24
                || model.Days < 0
                || model.Days > 31
                || string.IsNullOrWhiteSpace(model.Url)
                || string.IsNullOrWhiteSpace(model.BoundaryRegExp)
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
    }
}
