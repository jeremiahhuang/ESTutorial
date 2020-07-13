using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESTutorial.API.models
{
    [ElasticsearchType(IdProperty = "ID",RelationName = "amazonsample")]
    public class Amazon_Commerce_Sample
    {
        public Amazon_Commerce_Sample()
        {
            SubCategory = new List<string>();
            ListOfItemsBoughtTogether = new List<string>();
        }
        public string unique_id { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        [Keyword]
        public string Manufacturer { get; set; }
        [Number]
        public double Price { get; set; }
        [Number]
        public int InStock { get; set; }
        public bool IsStockUsed { get; set; }
        public int NumberOfReviews { get; set; }
        public int NumberOfAnsweredQuestions { get; set; }
        public string Average_review_rating { get; set; }
        [Keyword]
        public string Category { get; set; }
        [Keyword]
        public List<string> SubCategory { get; set; }
        public List<string> ListOfItemsBoughtTogether { get; set; }
        public string Description { get; set; }
        public string ProductInfo { get; set; }
    }
}
