using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LineNotifyPractice.Models;

namespace LineNotifyPractice
{
    /// <summary>
    /// 共用環境變數
    /// </summary>
    public class AppSettings
    {
        public static IConfigurationBuilder builder = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


        public static IConfiguration Configuration = builder.Build();
        public static readonly LineLoginConfig LineLoginConfig = Configuration.GetSection("LineLogIn").Get<LineLoginConfig>();
        public static readonly LineNotifyConfig LineNotifyConfig = Configuration.GetSection("LineNotify").Get<LineNotifyConfig>();
        public static readonly string ConnctionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");


        public static readonly string stop = "";

    }
}