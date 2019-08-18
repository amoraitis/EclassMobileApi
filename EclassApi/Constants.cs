using System;
using System.Collections.Generic;
using System.Text;

namespace EclassApi
{
    public static class Constants
    {
        #region endpoints
        public static readonly string Login = "/modules/mobile/mlogin.php";
        public static readonly string Logout = "/modules/mobile/mlogin.php?logout";
        public static readonly string Portfolio = "/modules/mobile/mportfolio.php";
        public static readonly string PortfolioPage = "/main/portfolio.php";
        public static readonly string ToolsEndpoint = "/modules/mobile/mtools.php?course=";
        public static readonly string UserCourses = "/modules/mobile/mcourses.php";
        #endregion
    }
}
