using System;
using System.Collections.Generic;
using System.Text;

namespace ESTutorial.Datamodels
{
    public class User
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string School { get; set; }
        public string District { get; set; }
        public int GradeLevel { get; set; }
    }
}
