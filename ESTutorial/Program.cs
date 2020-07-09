using CsvHelper;
using ESTutorial.Datamodels;
using ESTutorial.ElasticsearchModels;
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
            var c = services.BuildServiceProvider().GetService<IElasticClient>();
            //
            Console.WriteLine("Start indexing");
            // pull in dataset 
            string userFileName = "data-1.csv";
            string enrollmentFileName = "Enrollments.csv";
            //get user data and enrollment data
            var users = FileReader<User>.GetFile(userFileName).ToList();
            
            var enrollments = FileReader<Enrollment>.GetFile(enrollmentFileName).ToList();
            var randomCourseCount = new Random();
            //create the data to be index
            List<UserES> indexData = new List<UserES>();
            foreach(var user in users)
            {
                var student = new UserES
                {
                    District = user.District, 
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    GradeLevel = user.GradeLevel, 
                    ID = user.ID, 
                    School = user.School
                    
                };
                
                //randomly add courses based on that school's list
                var availableCourses = enrollments.Where(x => x.School == user.School).Select(e => e.CourseName).ToList();
                var ranNum = randomCourseCount.Next(noMagicNumberHere);
                for( int i = 0; i< ranNum;i++)
                {
                    student.Enrollments.Add(availableCourses[randomCourseCount.Next(availableCourses.Count)]);
                }
                indexData.Add(student);
            }
            //now index it

            c.IndexMany<UserES>(indexData);
            

            
        }
        
   
    }
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(
            this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex)
                ;
            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
    public static class FileReader<T>
    { 
        public static IEnumerable<T> GetFile(string fileName)
        {
            TextReader reader = new StreamReader($"{fileName}");
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csvReader.GetRecords<T>();

            return records;
        }
    }

}
