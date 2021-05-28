using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebAPIClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
            
        static List<string> myList;

        static async Task Main(string[] args)
        {
            // Create new list to hold the titles of all linked articles 
            myList = new List<string>();

            Console.WriteLine("Enter the title of an article: ");

            // Read user input 
            string userInput = Console.ReadLine();

            //WriteOutWikiInfo(wikiPage, myList);

            //Console.WriteLine(await GetPages(userInput, 1));
            await GetPages(userInput, 1, 15);
        }

        private static async Task GetPages(string userInput, int nPages, int maxWidth)
        {
            // Iterate over ProcessWikiPage
            // return List<WikiPage>
            
            for (int i = 0; i < maxWidth; i++)
            {
                var wikiPage = await ProcessWikiPage(userInput);
                var wikiPage2 = await ProcessWikiPage(wikiPage.Parse.Linked[i].Link);
            }
            
            //return List<Wikipage> blankList;
        }

        private static async Task<WikiPage> ProcessWikiPage(string article)
        {
            // Replace whitespace in input with "_" 
            article = article.Replace(" ", "_");
            
            // Fetch the data from the requested article
            string url = $"https://en.wikipedia.org/w/api.php?action=parse&format=json&page={article}&prop=links";

            var streamTask = client.GetStreamAsync(url);
        
            // Deserialize the returned json file and store the data as wikiPage objects
            var wikiPage = await JsonSerializer.DeserializeAsync<WikiPage>(await streamTask);
            
            // Add links to myList
            foreach (var link in wikiPage.Parse.Linked)
            {
                //Console.WriteLine(link.Ns);
                //Console.WriteLine(link.Link);
                //Console.WriteLine();
                if (myList.Contains(link.Link))
                {
                    continue;
                }
                else
                {
                    myList.Add(link.Link);
                }
            }
            Console.WriteLine();
            Console.WriteLine($"Root Page Title: {wikiPage.Parse.Title}");
            Console.WriteLine($"Root Page ID: {wikiPage.Parse.PageId}");
            Console.WriteLine($"Total links on page: {myList.Count}");
            return wikiPage;

        }
    }


    public class WikiPage
    {
        [JsonPropertyName("parse")]
        public ParseData Parse { get; set; }
    }


    public class ParseData
    {
        [JsonPropertyName("title")]
        public string Title {get; set;}

        [JsonPropertyName("pageid")]
        public int PageId {get; set;}

        [JsonPropertyName("links")]
        public Links[] Linked {get; set;}
    }


    public class Links
    {
        [JsonPropertyName("ns")]
        public int Ns {get; set;}

        [JsonPropertyName("exists")]
        public string Exists {get; set;}

        [JsonPropertyName("*")]
        public string Link {get; set;}    
    }


    /*
    class PageProcessor
    {
        // 
    }
    */
}
