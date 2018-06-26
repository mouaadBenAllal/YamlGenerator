using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace fireflycaesar.Controllers
{
    /// <summary>
    /// data for each json parameter
    /// </summary>
    public class JsonParametersData
    {
        public bool required { get; set; }
        public bool list { get; set; }
        public List<string> listItems { get; set; }
        public List<string> options { get; set; }

    }

    public class WebscrapeController : Controller
    {
        /// <summary>
        /// starts webscraping
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var latestModulesHtml = @"http://docs.ansible.com/ansible/latest/modules/";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(latestModulesHtml);

            HtmlNode[] nodes = htmlDoc.DocumentNode.SelectNodes("//a").ToArray();
            
            int i = 0;
            var jsonData = new List<Dictionary<string, Dictionary<string, JsonParametersData>>>();
            //foreach through all modules
            foreach (HtmlNode item in nodes)
            {

                var moduleParameters      = new List<string>();
                var parameterOptionsArray = new List<List<string>>();
                var listOptionsArray      = new List<List<string>>();
                var requiredArray         = new List<bool>();
                var listArray             = new List<bool>();
                string moduleName         = item.Attributes["href"].Value.Replace("_module.html", "");

                //skip the first 4 html links
                if (i > 4)
                { 
                    var moduleHtml = @"http://docs.ansible.com/ansible/latest/modules/" + item.Attributes["href"].Value;
                    var moduleHtmlDoc = web.Load(moduleHtml);

                    HtmlNodeCollection moduleNode  = moduleHtmlDoc.DocumentNode.SelectNodes("//tr");
                    bool arrivedAtParameterRow     = false;
                    bool startScrapingParameters   = false;
                    bool arrivedAtReturnRow        = false;
                    if (moduleNode != null)
                    { 
                        //foreach table row data
                        foreach (HtmlNode tableRow in moduleNode)
                        {
                            var parameterOptions = new List<string>();
                            var listOptions      = new List<string>();

                            HtmlNodeCollection checkIfReturnRowExistCollection = tableRow.SelectNodes(".//th[@class='head']");
                            if(checkIfReturnRowExistCollection != null) { 
                            foreach (var checkIfReturnRowExist in checkIfReturnRowExistCollection)
                                {
                                    var returnRowExist = checkIfReturnRowExist.SelectSingleNode(".//div[@class='cell-border']");
                                    if (returnRowExist != null && returnRowExist.InnerHtml == "Key")
                                    {
                                        // arrived at return parameters, stopping loop.
                                        arrivedAtReturnRow = true;
                                        break;
                                    }
                                    if (returnRowExist != null && returnRowExist.InnerHtml == "Parameter")
                                    {
                                        arrivedAtParameterRow = true;
                                        break;
                                    }
                                    if (returnRowExist != null && returnRowExist.InnerHtml == "Fact")
                                    {
                                        arrivedAtReturnRow = true;
                                        break;
                                    }
                                }
                            }

                            if (arrivedAtReturnRow)
                                break;

                            if (arrivedAtParameterRow)
                            {
                                arrivedAtParameterRow = false;
                                startScrapingParameters = true;
                                continue; 
                            }
                            //getting ansible parameters
                            if (startScrapingParameters)
                            {
                                HtmlNodeCollection attributeArray = tableRow.SelectNodes(".//div[@class='elbow-key']");
                                foreach (var attributecol in attributeArray)
                                {
                                    HtmlNodeCollection parametersCollection = attributecol.SelectNodes(".//b");
                                    foreach (var parameter in parametersCollection)
                                    {
                                        moduleParameters.Add(parameter.InnerHtml);
                                    }
                                    //getting ansible required parameter
                                    HtmlNodeCollection requiredCollection = attributecol.SelectNodes(".//div[@style='font-size: small; color: red']");
                                    if (requiredCollection != null) { 
                                        requiredArray.Add(true);
                                        //save parameter as required in json here
                                    }
                                    else
                                    {
                                        requiredArray.Add(false);
                                    }
                                }
                                
                                HtmlNode[] secondRowsAndThirdRows = tableRow.SelectNodes(".//div[@class='cell-border']").ToArray();
                                foreach (var secondThirdRow in secondRowsAndThirdRows)
                                {
                                    HtmlNodeCollection secondRowinformation = secondThirdRow.SelectNodes(".//b");
                                    if(secondRowinformation != null) { 
                                        foreach (var secondrowinfo in secondRowinformation)
                                        {
                                            switch (secondrowinfo.InnerHtml)
                                            {
                                                case "Default:":
                                                    HtmlNodeCollection defaultCollection = secondrowinfo.SelectNodes(".//div[@style='color: blue']");
                                                    if(defaultCollection != null) { 
                                                        foreach ( var defaultItem in defaultCollection)
                                                            {
                                                                //save defaultitems in json here.
                                                            }
                                                    }
                                                    break;
                                                case "Choices:":
                                                    HtmlNodeCollection choicesCollection = tableRow.SelectNodes(".//li");
                                                    if(choicesCollection != null)
                                                    { 
                                                        foreach (var choicesItems in choicesCollection)
                                                        {
                                                            var choiceItem = choicesItems.SelectSingleNode(".//b");
                                                            ///save choice in json here
                                                            if (choiceItem != null)
                                                            {
                                                                parameterOptions.Add(choiceItem.InnerHtml);
                                                            }
                                                            else
                                                            {
                                                                parameterOptions.Add(choicesItems.InnerHtml);

                                                            }
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    Console.WriteLine("something went wrong");
                                                    break;
                                            }
                                        }
                                    }

                                    HtmlNodeCollection thirdRows = secondThirdRow.SelectNodes(".//div");
                                    if(thirdRows != null)
                                    { 
                                        foreach(var thirdRow in thirdRows)
                                        {
                                            if (thirdRow.InnerHtml.Contains("list") || thirdRow.InnerHtml.Contains("set of commands"))
                                            {
                                                HtmlNodeCollection thirdRowsItems = thirdRow.SelectNodes(".//code");
                                                if(thirdRowsItems != null) { 
                                                    foreach (var thirdRowsItem in thirdRowsItems)
                                                    {
                                                        if(thirdRowsItem.InnerHtml.Contains(":"))
                                                            listOptions.Add(thirdRowsItem.InnerHtml);
                                                    }
                                                }
                                                //list parameter in json true (parameter contains list)
                                                listArray.Add(false);
                                                int requiredArrayCount = requiredArray.Count - 1;
                                                listArray[requiredArrayCount] = true;
                                            }
                                            else
                                            {
                                                //list parameter in json false
                                                listArray.Add(false);
                                            }
                                        }
                                    }
                                }
                            }
                            /// saves row data in arrays
                            parameterOptionsArray.Add(parameterOptions);
                            listOptionsArray.Add(listOptions);
                        }
                    }

                    int index = 0;
                    var innerDict = new Dictionary<string, JsonParametersData>();
                    //filling array with moduleData
                    foreach (var name in moduleParameters)
                    {
                        innerDict[name] = new JsonParametersData
                        {
                            required = requiredArray[index],
                            list = listArray[index],
                            listItems = new List<string>(),
                            options = new List<string>(),
                        };
                        innerDict[name].options.AddRange(parameterOptionsArray[index]);
                        innerDict[name].listItems.AddRange(listOptionsArray[index]);
                        index++;
                    }

                    var dict = new Dictionary<string, Dictionary<string, JsonParametersData>>();
                    dict[moduleName] = innerDict;
                    jsonData.Add(dict);
                }
                i++;
            }
            // filling json file with moduelData
            string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            System.IO.File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Scripts\\projectscripts\\csharpmodules.json", json);
            return View();
        }
    }
}