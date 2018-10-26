using Algorithmia;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TopicModelingAPP
{
    public partial class GetContentBySearch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string GetDocumentWithTopicWeight()
        {

            string searchWord = TextBox1.Text;

            string responseText = "";
            try
            {

                int? topicIndex = null;
                StringBuilder searchresult = new StringBuilder();


                string docsList = "[";
                foreach (string file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/TestFiles/Files")))
                {

                    var fileData = File.ReadAllText(file, System.Text.Encoding.Default);

                    if (fileData.IndexOf(searchWord) >= 0)
                    {
                        docsList = docsList + "\"" + fileData + "|" + Path.GetFileName(file) + "\"" + ",";
                    }
                }
                docsList = docsList + "]";


                var input = "{"
                + "  \"docsList\":" + docsList + ","
                + " \"customSettings\": {"
                + "         \"numTopics\":" + 10 + ","
                + "         \"numIterations\":" + 300 + ","
                + "         \"numWords\":" + 10
                + "         }"
                + "}";


                var client = new Client("simF0eQKLdDbLZQSe6FMv0fjiES1");
                var algorithm = client.algo("nlp/LDA/1.0.0");
                var topicsResponse = algorithm.pipeJson<object>(input);

                var data = topicsResponse.result.ToString().Replace("{[", "[").Replace("}]", "]");

                var topicsList = JArray.Parse(data).Children().ToList();


                for (int i = 0; i < topicsList.Count; i++)
                {
                    if (topicsList[i][searchWord] != null)
                    {
                        topicIndex = i;
                        break;
                    }
                }

                if (topicIndex != null)
                {

                    var secondinput = "{"
                   + " \"topics\":" + topicsResponse.result + ","
                   + " \"docsList\":" + docsList
                   + "}";

                    var client1 = new Client("simF0eQKLdDbLZQSe6FMv0fjiES1");
                    var algorithm1 = client1.algo("nlp/LDAMapper/0.1.1");
                    var response1 = algorithm1.pipeJson<object>(secondinput);

                    var data1 = response1.result.ToString().Replace("{[", "[").Replace("}]", "]");

                    var topicDistributionList = JObject.Parse(data1)["topic_distribution"].Children().ToList();


                    Dictionary<string, decimal> fileProbabilitybySearch = new Dictionary<string, decimal>();

                    for (int i = 0; i < topicDistributionList.Count; i++)
                    {
                        if (Convert.ToDecimal(topicDistributionList[i]["freq"][Convert.ToString(topicIndex)]) != 0)
                        {
                            var docfile = ((Newtonsoft.Json.Linq.JValue)topicDistributionList[i]["doc"]).Value.ToString().Split('|')[1];

                            fileProbabilitybySearch.Add(docfile, Convert.ToDecimal(topicDistributionList[i]["freq"][Convert.ToString(topicIndex)]));
                        }
                    }

                    var sortedDict = from entry in fileProbabilitybySearch orderby entry.Value ascending select entry;

                    System.Data.DataTable table = new System.Data.DataTable();
                    //columns  
                    //  table.Columns.Add("S.No", typeof(int));
                    table.Columns.Add("SearchWord", typeof(string));
                    table.Columns.Add("FileName", typeof(string));
                    table.Columns.Add("WordCount", typeof(string));
                    table.Columns.Add("Content", typeof(string));

                    var rownumber = 0;

                    foreach (var entry in sortedDict)
                    {
                        var filepath = HttpContext.Current.Server.MapPath("~/TestFiles/Files/") + entry.Key;

                        var fileParagraphs = Regex.Split(File.ReadAllText(filepath, System.Text.Encoding.Default), ("" + ("\n\n")));

                        for (int i = 0; i < fileParagraphs.Length; i++)
                        {
                            int wordCount = Regex.Matches(fileParagraphs[i], "\\b" + Regex.Escape(searchWord) + "\\b", RegexOptions.IgnoreCase).Count;

                            if (wordCount != 0)
                            {
                                rownumber = rownumber + 1;
                                table.Rows.Add(searchWord, entry.Key, wordCount, fileParagraphs[i]);
                            }
                        }
                    }

                    DataTable dtOut = null;
                    table.DefaultView.Sort = "WordCount DESC";
                    dtOut = table.DefaultView.ToTable();


                    //   SaveToCSV(dtOut, @"C:\Users\52063202\Desktop\TopicModelingOutput\resultContent.csv");

                    responseText = "The list of documents for " + searchWord + " is listed below :";

                    Label1.Text = responseText;

                    TextBox1.Text = "";

                    this.GridView1.Visible = true;

                    GridView1.DataSource = dtOut;

                    GridView1.DataBind();

                }
                else
                {
                    responseText = "Sorry! No results are found";
                }

                return responseText;
            }
            catch (Exception ex)
            {
                responseText = ex.Message;

                return responseText;
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            GetDocumentWithTopicWeight();
        }


        public static void SaveToCSV(System.Data.DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();

        }
    }
}