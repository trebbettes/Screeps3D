using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
     * initial payload
        "body":[
            {
                "type":"work",
                "hits":100,
                "boost": "XGH2O"
            },
            {
                "type":"work",
                "hits":100
            },
            {
                "type":"carry",
                "hits":100
            },
            {
                "type":"carry",
                "hits":100
            },
            {
                "type":"move",
                "hits":100
            }
        ],
        
        delta payloads
	        "body": {
		        "9": {
			        "type": "move",
			        "hits": 0,
                    "boost": "XGH2O"
		        },
		        "10": {
			        "type": "carry",
			        "hits": 60
		        }
	        },
	        "hits": 1360,
	        "actionLog": {
		        "attacked": {
			        "x": 17,
			        "y": 23
		        }
	        }
        
         
         
         */

    public class CreepBody
    {
        public List<CreepPart> Parts { get; private set; }

        public CreepBody()
        {
            Parts = new List<CreepPart>();
        }

        internal void Unpack(JSONObject data, bool initial)
        {
            var bodyObj = data["body"];
            if (bodyObj == null)
                return;

            ////Debug.Log(initial);
            ////Debug.Log(data.ToString());

            if (initial)
            {
                Parts.Clear();
                foreach (var partObj in bodyObj.list)
                {
                    var bodyPart = new CreepPart();
                    bodyPart.Unpack(partObj, initial);
                    Parts.Add(bodyPart);
                }
            }
            else
            {
                foreach (var key in bodyObj.keys)
                {
                    if (int.TryParse(key, out int index))
                    {
                        var partObject = bodyObj[key];
                        var bodyPart = Parts.ElementAt(index);

                        bodyPart.Unpack(partObject, initial);
                    }
                }
            }
        }
    }

    public class CreepPart
    {
        public string Type;
        public float Hits;
        public string Boost;

        internal void Unpack(JSONObject data, bool initial)
        {
            this.Hits = data["hits"].n;
            this.Type = data["type"].str;

            // Boost is optional
            var boostObj = data["boost"];
            if (boostObj != null)
            {
                this.Boost = boostObj.str;
            }
        }
    }
}