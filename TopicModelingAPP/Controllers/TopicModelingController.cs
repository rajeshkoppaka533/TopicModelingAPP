using TopicModelingAPP.BusinessLogic;
using TopicModelingAPP.Models;
using System;
using System.Web.Http;

namespace TopicModelingAPP.Controllers
{
    public class TopicModelingController : ApiController
    {

        /// <summary>
        /// Searching string in document, based on intent from DialogFlow
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseModel Post(RequestModel request)
        {
            string responseText = string.Empty;

            try
            {
                if (request.QueryResult != null)
                {
                    //Checking the Intent Name
                    switch (request.QueryResult.Intent.DisplayName.ToLower())
                    {
                        case "searchtext":
                            // "any" is the entity which holds the search value
                            if (request.QueryResult.Parameters["any"].ToString() != string.Empty)
                            {                       
                                string userRequest = request.QueryResult.Parameters["any"].ToString();
                                responseText = AnalyzeDocumentBussinessLogic.GetDocumentWithTopicWeight(userRequest);
                            }
                            else {
                                responseText = "Input is invalid";
                            }                            
                            break;
                    }
                }
                return new ResponseModel() { fulfillmentText = responseText, source = $"API.AI" };
            }
            catch (Exception)
            {
                return new ResponseModel() { fulfillmentText = "Api Error", source = $"API.AI" };
            }
        } 

    }
}

