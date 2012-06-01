using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Syndication;
using Newtonsoft.Json.Linq;

namespace IftttGithub
{
    public class GithubParser
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">github account username</param>
        /// <returns></returns>
        public SyndicationFeed GetRssWathFromUserName(string username)
        {
            SyndicationFeed feed = new SyndicationFeed("Watched " + username, "Github watched" + username, null, username, DateTime.Now);

            string url = string.Format("https://api.github.com/users/{0}/watched", username);
            var json = string.Empty;
            try
            {
                json = new WebClient().DownloadString(url);
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    json = string.Empty;
                }
                else
                {
                    throw;
                }
            }


            if (!string.IsNullOrWhiteSpace(json))
            {
                JArray Projects = JArray.Parse(json);

                List<SyndicationItem> FeedItems = new List<SyndicationItem>();

                foreach (var project in Projects)
                {
                    DateTime UpdateDate = DateTime.Parse(project["updated_at"].ToString());
                    DateTime PushedAt = DateTime.Parse(project["pushed_at"].ToString());
                    string id = project["id"].ToString();
                    string full_name = project["full_name"].ToString();
                    string name = project["name"].ToString();
                    string Description = project["description"].ToString();
                    string DownloadUrl = string.Format("https://github.com/{0}/zipball/master", full_name);

                    SyndicationItem item = new SyndicationItem(name, Description, new Uri(DownloadUrl), id, UpdateDate);
                    item.PublishDate = UpdateDate;
                    FeedItems.Add(item);
                }

                feed.Items = FeedItems;

            }

            
            return feed;
        }


    }
}
