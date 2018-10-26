using Algorithmia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopicModelingAPP.Models
{
    public class FileModel
    {
        public List<string> FileNames { get; set; }
        public string DocsList { get; set; }
    }

    public class Algorithm1Model
    {
        public AlgorithmResponse TopicsResponse { get; set; }
        public string Data { get; set; }
    }

}
