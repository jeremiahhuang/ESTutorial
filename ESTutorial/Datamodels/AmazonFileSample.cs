using System;
using System.Collections.Generic;
using System.Text;

namespace ESTutorial.Datamodels
{
    public class AmazonFileSample
    {
        public string uniq_id { get; set; }
        public string product_name { get; set; }
        public string manufacturer { get; set; }
        public string price { get; set; }
        public string number_available_in_stock { get; set; }
        public string number_of_reviews { get; set; }
        public string number_of_answered_questions { get; set; }
        public string average_review_rating { get; set; }
        public string amazon_category_and_sub_category { get; set; }
        public string customers_who_bought_this_item_also_bought { get; set; }
        public string description { get; set; }
        public string product_information { get; set; }

    }
}
