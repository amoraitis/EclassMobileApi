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
            EclassUser eclassUser = new EclassUser("uni");
            DateTime started = DateTime.Now;
            Task.Run(async () =>

            {
                
                await eclassUser.Start("username", "password");

                eclassUser.FillDetails();

                eclassUser.UserCourses.ForEach(course =>
                {
                    Console.WriteLine(course.Name + " " + course.ID);
                    course.Tools.ForEach(tool => Console.WriteLine(tool.Name));
                });
                var end = DateTime.Now;
                Console.WriteLine(end - started);
            });
            
            Console.ReadLine();

        }
    }
}
