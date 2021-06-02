using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Numerics;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;


namespace WebAPIClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        
        Random rand = new Random();
        static List<string> myList;
        static Dictionary<string, Node> linkRefs; // store link name and pointer to node (or hash and int of counts)
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
            // Get links and text from root page
            await ProcessWikiPage(userInput);
            // var pageLinks = wikiPage.Item1;
            // var pageText = wikiPage.Item2;
        }

        private static async Task ProcessWikiPage(string article)
        {
            // Replace whitespace in input with "_" 
            article = article.Replace(" ", "_");

            // Fetch the data from the requested article
            var getPageTask = client.GetStreamAsync($"https://en.wikipedia.org/w/api.php?action=parse&page={article}&format=json&prop=links%7Cwikitext");
            //var getTextTask = client.GetStreamAsync($"https://en.wikipedia.org/w/api.php?action=parse&page={article}&format=json&prop=text");
            //https://en.wikipedia.org/w/api.php?action=parse&format=json&prop=text&page=COVID-19_pandemic
            //https://en.wikipedia.org/w/api.php?action=parse&format=json&page=Wikipedia%3AUnusual_articles%2FPlaces_and_infrastructure&prop=links%7Cwikitext

            // Deserialize the returned json file and store the data as wikiPage objects
            var page = await JsonSerializer.DeserializeAsync<WikiPage>(await getPageTask);
            //var pageText = await JsonSerializer.DeserializeAsync<WikiText>(await getTextTask);

            // Add links to myList
            foreach (var link in page.Parse.Linked)
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
            Console.WriteLine($"Root Page Title: {page.Parse.Title}");
            Console.WriteLine($"Root Page ID: {page.Parse.PageId}");
            Console.WriteLine($"Total links on page: {myList.Count}");
            Console.WriteLine($"Length of text: {page.Parse.Text.AllText.Length}");

            // count strings
            //int refs = page.Parse.Text.AllText.Count<string>(st => st == page.Parse.Linked[3].Link);
            int refs = Regex.Matches(page.Parse.Text.AllText, page.Parse.Linked[10].Link).Count;
            Console.WriteLine($"Count: {refs}");
            //(WikiPage, WikiText) t1 = (pageLinks, pageText);
            //return t1;
        }

        // Generate a child node in a random direction from parent node w/distance based on relatedness 
        private Vector3 GeneratePos(Vector3 parent, float distCoefficient, float spacing = 1)
        {
            Vector3 pos = new Vector3(rand.Next(), rand.Next(), rand.Next());  
            Vector3.Normalize(pos);    // Set length to 1
            pos *= distCoefficient;    // Set length to reflect # of related links
            pos *= spacing;            // User-defined variable to globally adjust node distance
            Vector3.Add(pos, parent);  // Set pos relative to parent node

            return pos;
        }
    }



    public class WikiPage
    {
        [JsonPropertyName("parse")]
        public ParseData Parse { get; set; }
    }

//  Nested Objects for holding article links 
    public class ParseData
    {
        [JsonPropertyName("title")]
        public string Title {get; set;}

        [JsonPropertyName("pageid")]
        public int PageId {get; set;}

        [JsonPropertyName("links")]
        public Links[] Linked {get; set;}

        [JsonPropertyName("wikitext")]
        public ArticleText Text {get; set;}
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

    public class ArticleText
    {
        [JsonPropertyName("*")]
        public string AllText {get; set;}
    }

    // Object holding link data
    public class Node
    {
        public string Title { get; set; }
        public Node[] Parents { get; set; }
        public Node[] Childs { get; set; }
        public bool Visited { get; set; } // flag for traversals to prevent looping
        public Vector3 Pos { get; set; }
    }




    /*
    class PageProcessor
    {
        // 
    }
    */
}
