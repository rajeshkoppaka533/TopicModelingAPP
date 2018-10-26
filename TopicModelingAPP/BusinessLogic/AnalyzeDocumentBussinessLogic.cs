using Algorithmia;
using TopicModelingAPP.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic; 
using System.IO;
using System.Linq; 
using System.Text;
using System.Web; 

namespace TopicModelingAPP.BusinessLogic
{
    public class AnalyzeDocumentBussinessLogic
    {
        /// <summary>
        /// NLP/LDA algorithm configuration settings
        /// </summary>
        public static string _clientID = "simF0eQKLdDbLZQSe6FMv0fjiES1";
        public static string _apiVersion = "nlp/LDA/1.0.0";
        public static string _apiVersion2 = "nlp/LDAMapper/0.1.1";

        /// <summary>
        /// LDA is a generative topic model extractor from collection of documents.
        /// This Algorithmia Platform provides API for LDA Topic Modeling. 
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public static string GetDocumentWithTopicWeight(string userRequest)
        {
            string responseText = string.Empty;
            try
            {
                int? topicIndex = null;
                StringBuilder searchresult = new StringBuilder();
                FileModel files = GetFileNames();

                // 1. This API used for generating List of Topics(collection of words) from all documents.
                // LDA Input custom settings 
                var allDocsAlgorithm = GetTopicsFromAllDocsAlgorithm(files);                 
                var topicsList = JArray.Parse(allDocsAlgorithm.Data).Children().ToList();

                // Access the Topic index by user search keyword.
                for (int i = 0; i < topicsList.Count; i++)
                {
                    if (topicsList[i][userRequest] != null)
                    {
                        topicIndex = i;
                        break;
                    }
                }

                if (topicIndex != null)
                {
                    // 2. This API used for mapping LDA topics to documents.
                    // It gives the response as distrubution of topics according to input documents
                    searchresult = GetLDATopicsToDocsAlgorithm(allDocsAlgorithm.TopicsResponse, files, userRequest, topicIndex);
                    responseText = searchresult.ToString();
                }
                else
                {
                    responseText = "Sorry! No results are found";
                }

                //Returning the response
                return responseText;
            }
            catch (Exception ex)
            {
                //Returning the exception
                responseText = ex.Message;
                return responseText;
            }
        }

        /// <summary>
        /// Get file details
        /// </summary>
        /// <returns></returns>
        public static FileModel GetFileNames()
        {
            FileModel item = new FileModel();
            List<string> filenames = new List<string>();

            string docsList = "[";
            foreach (string file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/TestFiles/Txt_Files")))
            {                
                filenames.Add(Path.GetFileName(file));
                docsList = docsList + "\"" + File.ReadAllText(file, System.Text.Encoding.Default) + "\"" + ",";                
            }
            docsList = docsList + "]";
            item.DocsList = docsList; ;
            item.FileNames = filenames;
            return item;
        }


        /// <summary>
        /// This API used for generating List of Topics(collection of words) from all documents.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Algorithm1Model GetTopicsFromAllDocsAlgorithm(FileModel result)
        {
            Algorithm1Model response = new Algorithm1Model();
            var input = "{"
                + "  \"docsList\":" + result.DocsList + ","
                + " \"customSettings\": {"
                + "         \"numTopics\":" + 3 + ","
                + "         \"numIterations\":" + 300 + ","
                + "         \"numWords\":" + 3
                + "         }"
                + "}";

            // Generate & use client API Key from https://algorithmia.com/ to access NLP/LDA API's
            var client = new Client(_clientID);
            var algorithm = client.algo(_apiVersion);
            var topicsResponse = algorithm.pipeJson<object>(input);
            var data = topicsResponse.result.ToString().Replace("{[", "[").Replace("}]", "]");
            response.Data = data;
            response.TopicsResponse = topicsResponse;
            return response;
        }

        /// <summary>
        /// 2. This API used for mapping LDA topics to documents.
        /// </summary>
        /// <param name="topicsResponse"></param>
        /// <param name="result"></param>
        /// <param name="userRequest"></param>
        /// <param name="topicIndex"></param>
        /// <returns></returns>
        public static StringBuilder GetLDATopicsToDocsAlgorithm(AlgorithmResponse topicsResponse, FileModel result, string userRequest, int? topicIndex = 0)
        {
            StringBuilder searchresult = new StringBuilder();
            var secondinput = "{"
                   + " \"topics\":" + topicsResponse.result + ","
                   + " \"docsList\":" + result.DocsList
                   + "}";

            var client1 = new Client(_clientID);
            var algorithm1 = client1.algo(_apiVersion2);
            var response1 = algorithm1.pipeJson<object>(secondinput);
            var data1 = response1.result.ToString().Replace("{[", "[").Replace("}]", "]");
            var topicDistributionList = JObject.Parse(data1)["topic_distribution"].Children().ToList();
            searchresult.Append(" This " + userRequest + " keyword weightage, distributed among the documents are : \n\n");

            //It will result the search keyword topic index weightage in each document of corpus.
            for (int i = 0; i < topicDistributionList.Count; i++)
            {
                searchresult.Append(result.FileNames[i] + " : " + Math.Round((Convert.ToDecimal(topicDistributionList[i]["freq"][Convert.ToString(topicIndex)]) * 100), 2) + "%" + ", \n");
            }
            return searchresult;
        }
    }
}