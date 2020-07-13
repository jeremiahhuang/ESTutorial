using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESTutorial.API.models;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ESTutorial.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private static readonly ConnectionSettings connSettings =
            new ConnectionSettings(new Uri("http://127.0.0.1:9200/")).DefaultIndex("amazonsample");
        private static readonly ElasticClient elasticClient =
            new ElasticClient(connSettings);
    
       
        [HttpGet("findbyname")]
        public IEnumerable<Amazon_Commerce_Sample> FindByName(string term)
        {
            
            var res = elasticClient.Search<Amazon_Commerce_Sample>(x => x
                .TypedKeys(false)
                .Query(q => q.
                    Match(m => m.Field(f => f.Name).Query(term))));
                   
            if (!res.IsValid)
            {
                throw new InvalidOperationException(res.DebugInformation);
            }

            return res.Documents;
        }
        [HttpGet("GetCategories")]
        public Dictionary<string, long?> GetCategories()
        {
            Dictionary<string, long?> t = new Dictionary<string, long?>();
            var res = elasticClient.Search<Amazon_Commerce_Sample>(s => s
                    .Size(0)
                    .Aggregations(c=>c
                        .Terms("categories",t=>t
                            .Field("category")
                        )
                    )
            );
            if (!res.IsValid)
            {
                throw new InvalidOperationException(res.DebugInformation);
            }
            var myaggs = res.Aggregations.Terms("categories");
            foreach(var agg in myaggs.Buckets)
            {
                t.Add(agg.Key, agg.DocCount);
            }
            return t;
        }
    }
}
