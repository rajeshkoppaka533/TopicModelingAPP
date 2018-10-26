using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicModelingAPP.Models
{
    public class LuisJsonCreator
    {
        public string luis_schema_version;
        public string versionId;
        public string name;
        public string desc;
        public string culture;
        public LuisIntents[] intents;
            //entities[]
            //composites[]
            //closedLists[]
            //bing_entities[]
            //actions[]
            //model_features[]
            //regex_features[]
            //utterances[]

    }
}
