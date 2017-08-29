﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models;

namespace JasperSiteCore.Models.Database
{
   
        public  class DbInitializer
        {

        public DbInitializer(DatabaseContext context)
        {
            _databaseContext = context;
           
        }

        private DatabaseContext _databaseContext;  
        public  DatabaseContext DatabaseContext
        {
            get { return _databaseContext; }
           
        }


      public void Initialize()
            {
           
            Configuration.DbHelper = new DbHelper(DatabaseContext);

            DatabaseContext.Database.EnsureCreated();

                // Look for any articles
                if (DatabaseContext.Articles.Any())
                {
                    return;   // DB has been seeded
                }


            Article[] articles = new Article[]
            {
                new Article {Name="První článek", HtmlContent="<b>tučný text</b>", PublishDate = DateTime.Now},
                 new Article {Name="Druhý článek", HtmlContent="<b>kurzíva</b>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(60)},
                   new Article {Name="Třetí článek", HtmlContent="<h2>nadpis text</h2>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(120)},
                    new Article {Name="Čtvrtý článek", HtmlContent="test", PublishDate = DateTime.Now + TimeSpan.FromMinutes(180)},
            };

            foreach(Article a in articles)
            {
                DatabaseContext.Articles.Add(a);
            }

            Category[] categories = new Category[]
            {
                new Category { Name="Category1"},
                 new Category { Name="Category2"},
                  new Category { Name="Category3"},
                   new Category { Name="Category4"}
            };

            foreach(Category c in categories)
            {
                DatabaseContext.Categories.Add(c);
            }

            //    var students = new Student[]
            //    {
            //new Student{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2005-09-01")},
            //new Student{FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2002-09-01")},
            //new Student{FirstMidName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2003-09-01")},
            //new Student{FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2002-09-01")},
            //new Student{FirstMidName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2002-09-01")},
            //new Student{FirstMidName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2001-09-01")},
            //new Student{FirstMidName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2003-09-01")},
            //new Student{FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2005-09-01")}
            //    };
            //    foreach (Student s in students)
            //    {
            //        context.Students.Add(s);
            //    }
            //    context.SaveChanges();

            //    var courses = new Course[]
            //    {
            //new Course{CourseID=1050,Title="Chemistry",Credits=3},
            //new Course{CourseID=4022,Title="Microeconomics",Credits=3},
            //new Course{CourseID=4041,Title="Macroeconomics",Credits=3},
            //new Course{CourseID=1045,Title="Calculus",Credits=4},
            //new Course{CourseID=3141,Title="Trigonometry",Credits=4},
            //new Course{CourseID=2021,Title="Composition",Credits=3},
            //new Course{CourseID=2042,Title="Literature",Credits=4}
            //    };
            //    foreach (Course c in courses)
            //    {
            //        context.Courses.Add(c);
            //    }
            //    context.SaveChanges();

            //    var enrollments = new Enrollment[]
            //    {
            //new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
            //new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
            //new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
            //new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
            //new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
            //new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
            //new Enrollment{StudentID=3,CourseID=1050},
            //new Enrollment{StudentID=4,CourseID=1050},
            //new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
            //new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
            //new Enrollment{StudentID=6,CourseID=1045},
            //new Enrollment{StudentID=7,CourseID=3141,Grade=Grade.A},
            //    };
            //    foreach (Enrollment e in enrollments)
            //    {
            //        context.Enrollments.Add(e);
            //    }
               DatabaseContext.SaveChanges();
            }
        }
    
}


