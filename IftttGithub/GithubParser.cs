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
            SyndicationFeed feed = new SyndicationFeed("Watched " + username, "Github watched " + username, null, username, DateTime.Now);

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
                    UpdateDate = UpdateDate.AddHours(-3); //fix date

                    string full_name = project["full_name"].ToString();
                    string name = project["name"].ToString();
                    string html_url = project["html_url"].ToString();
                    string DownloadUrl = string.Format("https://github.com/{0}/zipball/master", full_name);

                    SyndicationItem item = new SyndicationItem(name, DownloadUrl, new Uri(html_url));
                    item.Id=string.Format("{0} {1}",name,UpdateDate.ToString("ddMMyyyy-hh:mm:ss"));
                    item.PublishDate = UpdateDate;
                    item.LastUpdatedTime = item.PublishDate;
                    item.Links.Add(new SyndicationLink(new Uri(html_url),"html url",string.Empty,"text/html",0));
                    item.Links.Add(new SyndicationLink(new Uri(DownloadUrl), "download", string.Empty, string.Empty, 0));
                    FeedItems.Add(item);
                }

                feed.Items = FeedItems;

            }

            
            return feed;
        }


    }
}
