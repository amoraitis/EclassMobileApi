using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EclassApi.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            EclassUser eclassUser = new EclassUser("aueb");

            Task.Run(async () =>

            {
                Console.WriteLine(DateTime.Now);
                await eclassUser.Start("Username", "Password");

                eclassUser.FillDetails();

                eclassUser.UserCourses.ForEach(course =>
                {
                    Console.WriteLine(course.Name + " " + course.ID);
                    course.Tools.ForEach(tool => Console.WriteLine(tool.Name));
                });
                Console.WriteLine(DateTime.Now);
            });

            Console.ReadLine();

        }
    }
}
