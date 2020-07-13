using CsvHelper;
using ESTutorial.Datamodels;
using ESTutorial.ElasticsearchModels;
using ESTutorial.Extensions;
using ESTutorial.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;

namespace ESTutorial
{
    class Program
    {
        //https://miroslavpopovic.com/posts/2018/07/elasticsearch-with-aspnet-core-and-docker
        //https://www.elastic.co/guide/en/elastic-stack-get-started/current/get-started-docker.html
        private static IConfigurationRoot config;
        private static int noMagicNumberHere = 10;
        static void Main(string[] args)
        {
            //setup

            var builder = new ConfigurationBuilder().SetBasePath(Path.GetFullPath(AppContext.BaseDirectory)).AddJsonFile("appsettings.json");
            config = builder.Build(); 
            var services = new ServiceCollection();
            services.AddElasticsearch(config);
            var client = services.BuildServiceProvider().GetService<IElasticClient>();
            //
            Console.WriteLine("Start indexing");

            //create index
            //https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/auto-map.html
            client.Indices.Create(config["elasticsearch:index"], c=>c
                .Map<Amazon_Commerce_Sample>(m=>m
                        .AutoMap<Amazon_Commerce_Sample>()
                    )
            );

            // pull in dataset 
            string fileName = "amazonexample.csv";
            //get user data and enrollment data
            var dataFromFile = FileReader<AmazonFileSample>.GetFile(fileName).ToList();
           
            //create the data to be index
            List<Amazon_Commerce_Sample> indexData = new List<Amazon_Commerce_Sample>();
            int id = 0;
            foreach(var user in dataFromFile)
            {
                var stocktoken = user.number_available_in_stock.Split(' ');
                var categories = user.amazon_category_and_sub_category.Split('>');
                var itemsalsobought = user.customers_who_bought_this_item_also_bought.Split('|');
                double price = 0.0;
                int numberOfreviews = 0;
                int numberOfAnsweredQuestions = 0;
                Int32.TryParse(user.number_of_reviews,out numberOfreviews);
                Int32.TryParse(user.number_of_answered_questions,out numberOfAnsweredQuestions);
                Double.TryParse(user.price, out price);

                if (user.uniq_id.Length < 2)
                    continue;
                var sample = new Amazon_Commerce_Sample()
                {
                    unique_id = user.uniq_id,
                    ID = ++id,
                    Name = user.product_name,
                    Manufacturer = user.manufacturer,
                    Price = price,
                    InStock = stocktoken.Count() > 1 ? Int32.Parse(stocktoken[0]) : 0,
                    IsStockUsed = stocktoken.Count() >1 ? (stocktoken[1].ToLower() == "new" ? false : true) : false,
                    NumberOfReviews = numberOfreviews,
                    NumberOfAnsweredQuestions = numberOfAnsweredQuestions,
                    Average_review_rating = user.average_review_rating,
                    Category = categories.Count() > 0 ? categories[0] : "",
                    Description = user.description,
                    ListOfItemsBoughtTogether = itemsalsobought.ToList(),
                    ProductInfo = user.product_information,
                    SubCategory = categories.Count() > 0 ? categories.Skip(1).ToList() : new List<string>()
                };

                indexData.Add(sample);
            }
            //now index it
            // indexData.Count();
            
            client.IndexMany<Amazon_Commerce_Sample>(indexData);           

            
        }      
     

   
    }
    
   
}
