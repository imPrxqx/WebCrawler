using System;
using System.Collections.Generic;


namespace WebCrawler.Models
{

	class TagsData
	{
		protected Dictionary<string, int> allTags = new Dictionary<string, int>();

		public void AddTags(string tagsInArticle)
		{
			string[] tagsNotSorted = tagsInArticle.Split('-');
			foreach (string tag in tagsNotSorted)
			{
				if (!string.IsNullOrEmpty(tag))
				{
					string trimmedTag = tag.Trim();

					if (allTags.ContainsKey(trimmedTag))
					{
						allTags[trimmedTag]++;
						Console.WriteLine(trimmedTag);
					}
					else
					{
						allTags[trimmedTag] = 1;
						Console.WriteLine("-" + trimmedTag + "-");
					}
				}
			}
		}

		public int OccurrenceTags(string aTag)
		{
			string tag = aTag.Trim();
			if (allTags.ContainsKey(tag))
			{
				return allTags[tag];
			}
			else
			{
				return 0;
			}
		}

		public Dictionary<string, int> GetAllTags()
		{
			return allTags;
		}
	}

}
