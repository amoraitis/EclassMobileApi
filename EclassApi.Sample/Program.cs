using System;
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

                await eclassUser.Start("p3140138", "noveplef97");

                eclassUser.FillDetails();

                eclassUser.UserCourses.ForEach(course => Console.WriteLine(course.Name + " " + course.ID));

            });

            Console.ReadLine();

        }
    }
}
