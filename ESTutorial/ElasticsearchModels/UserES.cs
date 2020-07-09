using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESTutorial.ElasticsearchModels
{
    [ElasticsearchType(IdProperty = "ID",Name = "userenrollments")]


    public class UserES
    {
        public UserES()
        {
            Enrollments = new List<string>();
        }
        [Keyword]
        public int ID { get; set; }
        //no attr will let elastic pick
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Keyword]
        //keyword will index the entire string as one token. 
        public string School { get; set; }
        public string District { get; set; }
        [Number]
        public int GradeLevel { get; set; }
        [Nested]
        public List<string> Enrollments { get; set; }
    }
}
