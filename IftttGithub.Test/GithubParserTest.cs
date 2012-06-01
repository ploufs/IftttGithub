using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel.Syndication;
using System.Linq;
using System.Net;
using System.Xml;

namespace IftttGithub.Test
{
    [TestClass]
    public class GithubParserTest
    {
        [TestMethod]
        public void GetRssWathFromUserName_EmptyXmlWhenUsernameNotFound()
        {
            var parser = new GithubParser();
            var result = parser.GetRssWathFromUserName("dddddfewr32r4");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Items.Count() == 0);
        }

        [TestMethod]
        public void GetRssWathFromUserName_ReturnValue()
        {
            var parser = new GithubParser();
           var result= parser.GetRssWathFromUserName("ploufs");

            Assert.IsTrue(result.Items.Count() > 0);
        }

        [TestMethod]
        public void GetRssWathFromUserName_SaveToHd()
        {
            var parser = new GithubParser();
            var result = parser.GetRssWathFromUserName("ploufs");

            var filename=@"c:\TestRSSFile.xml";

            System.IO.File.Delete(filename);

            XmlWriter xmlWriter = XmlWriter.Create(filename);
            result.SaveAsRss20(xmlWriter);
            xmlWriter.Close();
            xmlWriter.Dispose();

            Assert.IsTrue(System.IO.File.Exists(filename));

        }

        [TestMethod]
        public void GetRssWathFromUserName_DownloadUrlIsFound()
        {
            var parser = new GithubParser();
            var result = parser.GetRssWathFromUserName("ploufs");

            if (result.Items.Count() > 0)
            {
                /*foreach (var item in result.Items)
                {
                    foreach (var link in item.Links)
                    {
                        Assert.IsTrue(this.RemoteFileExists(link.Uri.ToString())); 
                    }    
                }*/
                Assert.IsTrue(this.RemoteFileExists(result.Items.FirstOrDefault().Links.FirstOrDefault().Uri.ToString())); 
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/924679/c-sharp-how-can-i-check-if-a-url-exists-is-valid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool RemoteFileExists(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TURE if the Status code == 200
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }
    }
}
