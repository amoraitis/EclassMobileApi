﻿using System;
using Flurl.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using EclassApi;

namespace EclassMobileApi
{
    class Program
    {
        static string _auebEclass = "https://eclass.aueb.gr",
             _Login = "/modules/mobile/mlogin.php",
            _portfolio = "/modules/mobile/mportfolio.php",
            _UserCourses = "/modules/mobile/mcourses.php",
            _OpenCourses = "/modules/mobile/mcourses.php",
            _tools = "/modules/mobile/mtools.php?course=",
            _PortfolioPage = "/main/portfolio.php";
        static Model.EclassUser EclassUser = new Model.EclassUser();
        static string LoginResult { get; set; }
        static string login = "", portfolio = "", opencourses = "", userCourses = "", loginurl = _auebEclass + _Login, rss = "";
        
        static string tools = "", portfolioPage = "";
        public static void Main(string[] args)
        {
            EclassUser.Username = "p3140138"; //Console.ReadLine();
            EclassUser.Password = "noveplef97"; //Console.ReadLine();

            
            Task.Run(async () =>
            {
                login = await (loginurl).PostUrlEncodedAsync(new { uname = EclassUser.Username, pass = EclassUser.Password }).ReceiveString();
            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                portfolio = await (_auebEclass + _portfolio).PostUrlEncodedAsync(new { token = login }).ReceiveString();
            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                userCourses = await (_auebEclass + _UserCourses).PostUrlEncodedAsync(new { token = login }).ReceiveString();

            }).GetAwaiter().GetResult();
            
            ViewModel.ToolViewModel toolViewModel = new ViewModel.ToolViewModel(login);
            Task.Run(async () =>
            {
                tools = await (_auebEclass + _tools + "INF261").PostUrlEncodedAsync(new { token = login }).ReceiveString();
                
                
            }).GetAwaiter().GetResult();
            toolViewModel.SetTools(tools);
            Task.Run(async () => {portfolioPage= await (_auebEclass+_PortfolioPage).PostUrlEncodedAsync(new { token = login }).ReceiveString(); }).GetAwaiter().GetResult();
            Model.AnnouncementToken AnnouncementToken = new Model.AnnouncementToken("INF261", login);
            Console.WriteLine("Login: "+loginurl + Environment.NewLine + LoginResult);
            //Console.WriteLine("Portfolio: " + Environment.NewLine + portfolio);
            //Console.WriteLine("User Courses:" + Environment.NewLine + userCourses);
            //Console.WriteLine("Open courses:" + Environment.NewLine + opencourses);
            //Console.WriteLine("Tools:" + Environment.NewLine + toolViewModel.ToString());
            
            Console.WriteLine(tools);
            Console.WriteLine("Portfolio:" + Environment.NewLine + EclassUser.GetUID(LoginResult, _auebEclass + _PortfolioPage));
            Console.WriteLine("Token for course with code INF261:" + AnnouncementToken.Token);
            System.Console.ReadLine();
            
        }
        
        

        static Dictionary<string, string> GetDirectories(string courseUID, string loginToken)
        {
            Dictionary<string, string> listDictionary = null;

            return listDictionary;
        }
        
    }
}
