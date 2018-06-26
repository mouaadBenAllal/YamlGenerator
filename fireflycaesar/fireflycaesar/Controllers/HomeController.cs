using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AngleSharp;
using AngleSharp.Parser.Html;
using AngleSharp.Extensions;
using System.Net.Http;
using System.Net;
using HtmlAgilityPack;

namespace fireflycaesar.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WebScraper(String mymodule)
        {
            
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("http://docs.ansible.com/ansible/latest/modules/list_of_windows_modules.html");
            Dictionary<int, string> dict = new Dictionary<int, string>();

            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//span[@class='std std-ref']").ToArray();
            
            int i = 0;
            foreach (var item in nodes)
            {
                string module = item.InnerHtml.Substring(0, item.InnerHtml.LastIndexOf("- ")-1);
                module += "_module";
                dict.Add(i, module);
                i++;
            }

            ViewBag.ModuleList = dict;
            return View();
            
        }

        [HttpPost]
        public string postWebScrape(string mymodule)
        {

            if (mymodule.Contains('.'))
            {
                return null;
            }
            else
            {
                HtmlWeb web = new HtmlWeb();

                HtmlDocument document = web.Load("http://docs.ansible.com/ansible/latest/modules/" + mymodule + ".html");

                HtmlNode[] description = document.DocumentNode.SelectNodes("//div[@id='synopsis']//ul[@class='simple']").ToArray();

                HtmlNode[] parameter = document.DocumentNode.SelectNodes("//div[@id='parameters']").ToArray();

                string moduleParameters = "";
                foreach (var item in parameter)
                {
                    moduleParameters += item.InnerHtml;
                }

                string descriptions = "";
                foreach (var item in description)
                {
                    descriptions += item.InnerHtml;
                }

                return "description = " + descriptions + "parameters = " + moduleParameters;

            }


        }
    }
}
