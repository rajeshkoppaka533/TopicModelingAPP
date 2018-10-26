using Algorithmia;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TopicModelingAPP.Models;

namespace TopicModelingAPP
{
    public partial class TopicWtCSV : System.Web.UI.Page
    {
        public static string _clientID = "simF0eQKLdDbLZQSe6FMv0fjiES1";
        public static string _apiVersion = "nlp/LDA/1.0.0";

        public static List<string> wordList;

        public static List<string> extensions;

        protected void Page_Load(object sender, EventArgs e)
        {

            Dictionary<string, List<JToken>> typeObjectList = new Dictionary<string, List<JToken>>();

            extensions = new List<string>()
            {
               "PDF",
               "DOC",
               "TXT"
            };

            for (int i = 0; i < extensions.Count; i++)
            {

                FileModel files = GetFiles(extensions[i]);


                //int? topicIndex = null;

                var input = "{"
               + "  \"docsList\":" + files.DocsList + ","
               + " \"customSettings\": {"
               + "         \"numTopics\":" + 5 + ","
               + "         \"numIterations\":" + 600 + ","
               + "         \"numWords\":" + 5
               + "         }"
               + "}";


                var client = new Client(_clientID);
                var algorithm = client.algo(_apiVersion);
                var topicsResponse = algorithm.pipeJson<object>(input);
                var data = topicsResponse.result.ToString().Replace("{[", "[").Replace("}]", "]");

                var topicsList = JArray.Parse(data).Children().ToList();

                typeObjectList.Add(extensions[i], topicsList);

            }

            MatrixTable(extensions, typeObjectList);



            //   Console.WriteLine(typeObjectList);
        }

        public FileModel GetFiles(string extension)
        {
            FileModel item = new FileModel();

            string docsList = "[";

            if (extension == "PDF")
            {
                foreach (string file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/TestFiles/Pdf_Files")))
                {

                    //Pdf Files
                    StringBuilder text = new StringBuilder();
                    using (PdfReader reader = new PdfReader(file))
                    {
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                        }
                    }

                    docsList = docsList + "\"" + text + "\"" + ",";

                }
            }
            else if (extension == "DOC")
            {
                foreach (string file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/TestFiles/Doc_Files")))
                {
                    //Doc Files
                    StringBuilder text = new StringBuilder();
                    Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                    object miss = System.Reflection.Missing.Value;
                    object path = file;
                    object readOnly = true;
                    Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

                    for (int i = 0; i < docs.Paragraphs.Count; i++)
                    {
                        text.Append(" \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString());
                    }

                    docsList = docsList + "\"" + text.ToString() + "\"" + ",";
                }
            }
            else
            {
                foreach (string file in Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath("~/TestFiles/Txt_Files")))
                {


                    docsList = docsList + "\"" + File.ReadAllText(file, System.Text.Encoding.Default) + "\"" + ",";

                }


            }


            docsList = docsList + "]";
            item.DocsList = docsList;
            return item;
        }

        public void MatrixTable(List<string> extensions, Dictionary<string, List<JToken>> typeObjectList)
        {

            wordList = new List<string>();

            for (int i = 0; i < typeObjectList.Count; i++)
            {

                var typeTopicList = typeObjectList[extensions[i]];

                for (int j = 0; j < typeTopicList.Count; j++)
                {
                    var topicRow = typeTopicList[j].ToList();
                    for (int k = 0; k < topicRow.Count; k++)
                    {
                        var word = ((Newtonsoft.Json.Linq.JProperty)topicRow[k]).Name;
                        if (!wordList.Contains(word))
                        {
                            wordList.Add(word);
                        }
                    }
                }
            }

            System.Data.DataTable table = new System.Data.DataTable();
            //columns  
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Word", typeof(string));
            table.Columns.Add("PDF", typeof(string));
            table.Columns.Add("DOC", typeof(string));
            table.Columns.Add("TXT", typeof(string));

            for (int i = 0; i < wordList.Count; i++)
            {

                var word = wordList[i];

                Dictionary<string, int> wordFreq = new Dictionary<string, int>();

                for (int j = 0; j < extensions.Count; j++)
                {

                    var typeTopicList = typeObjectList[extensions[j]];

                    for (int k = 0; k < typeTopicList.Count; k++)
                    {
                        if (typeTopicList[k][word] != null)
                        {
                            int value = Convert.ToInt32(((Newtonsoft.Json.Linq.JValue)typeTopicList[k][word]).Value);

                            if (wordFreq.ContainsKey(extensions[j]))
                            {
                                var wordCount = wordFreq[extensions[j]];
                                wordFreq[extensions[j]] = wordCount + value;
                            }
                            else
                            {
                                wordFreq.Add(extensions[j], value);
                            }
                        }
                    }

                    if (!wordFreq.ContainsKey(extensions[j]))
                    {
                        wordFreq.Add(extensions[j], 0);
                    }

                }

                table.Rows.Add(i + 1, word, wordFreq["PDF"], wordFreq["DOC"], wordFreq["TXT"]);
            }

            LuisJsonGeneration(table);

            SaveToCSV(table, @"C:\Users\52063202\Desktop\TopicModelingOutput\TopicWeightResult.csv");

            lblResult.InnerText = "CSV File and Luis Json intents is generated";
            lblResultpath.InnerText = @"C:\Users\52063202\Desktop\TopicModelingOutput";

         
        }

        public void LuisJsonGeneration(DataTable table)
        {

            string path = @"C:\Users\52063202\Desktop\TopicModelingOutput\Luis.json";
            // var Excelpath = @"D:\json\result.xlsx";
            // ReadExcel r = new ReadExcel();
            // ArrayList IntentNames = r.convertdata(Excelpath, 1);



            LuisIntents[] li = new LuisIntents[table.Rows.Count];
            int i = 0;
            foreach (DataRow row in table.Rows)
            {
                li[i] = new LuisIntents();
                li[i].name = row["Word"].ToString();
                i++;
            }


            LuisJsonCreator ljc = new LuisJsonCreator();
            ljc.luis_schema_version = "2.1.0";
            ljc.versionId = "0.1";
            ljc.name = "TestLuisApp";
            ljc.desc = "We are Testing a luis app by json";
            ljc.culture = "en-us";
            ljc.intents = li;

            string JSONresult = JsonConvert.SerializeObject(ljc);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var tw = new StreamWriter(path, true))
            {
                tw.WriteLine(JSONresult.ToString());
                tw.Close();
            }


        }


        public void SaveToCSV(System.Data.DataTable dtDataTable, string strFilePath)
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