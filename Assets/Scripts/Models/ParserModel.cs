using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [System.Serializable]
    public class ParserModel
    {
        [System.Serializable]
        public class Class
        {
            public List<int> color;
            public int id;
            public string name;
            public List<int> severities;
        }

        [System.Serializable]
        public class DetectionList
        {
            public int brx;
            public int bry;
            public int classId;
            public string className;
            public bool confidence;
            public int id;
            public List<double> oldProbs;
            public List<double> prob;
            public int severe;
            public List<List<double>> severityProbs;
            public int tlx;
            public int tly;
            public Vector3 rotation;
        }

        [System.Serializable]
        public class Root
        {
            public List<Class> classes;
            public List<DetectionList> detection_list;
            public string labeler;
            public string source_name;
        }
    }
}