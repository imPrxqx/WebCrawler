using System.ComponentModel.DataAnnotations.Schema;

namespace WebCrawler.Models
{
	[Table("WebsiteRecord")]
	public class WebsiteRecordModel
	{
		public int Id { get; set; }

		public string Url { get; set; } = string.Empty; 
		public string BoundaryRegExp { get; set; } = string.Empty;

		public int Days { get; set; }
		public int Hours { get; set; }
		public int Minutes { get; set; }

		public string Label { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public string Tags { get; set; } = string.Empty;
	}


}