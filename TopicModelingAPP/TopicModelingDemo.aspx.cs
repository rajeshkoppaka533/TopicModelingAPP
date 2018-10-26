using Algorithmia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TopicModelingAPP
{
    public partial class TopicModelingDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var docsList = "["
+ "    \"Machine Learning is Fun Part 5: Language Translation with Deep Learning and the Magic of Sequences\","
+ "    \"Paddle: Baidu's open source deep learning framework\","
+ "    \"An overview of gradient descent optimization algorithms\","
+ "    \"Create a Chatbot for Telegram in Python to Summarize Text\","
+ "    \"Image super-resolution through deep learning\","
+ "    \"World's first self-driving taxis debut in Singapore\","
        + "    \"Minds and machines: The art of forecasting in the age of artificial intelligence\""
+ "  ]";

            Label1.Text = docsList;


            var input = "{"
 + "  \"docsList\":" + docsList + ","
 + "  \"mode\": \"quality\""
 + "}";

            var client = new Client("simF0eQKLdDbLZQSe6FMv0fjiES1");
            var algorithm = client.algo("nlp/LDA/1.0.0");
            var response = algorithm.pipeJson<object>(input);

            Label2.Text = response.result.ToString();



            var secondinput = "{"
 + " \"topics\":" + response.result + ","
 + " \"docsList\":" + docsList
 + "}";

            var client1 = new Client("simF0eQKLdDbLZQSe6FMv0fjiES1");
            var algorithm1 = client1.algo("nlp/LDAMapper/0.1.1");
            var response1 = algorithm1.pipeJson<object>(secondinput);


            Label3.Text = response1.result.ToString();
        }
    }
}