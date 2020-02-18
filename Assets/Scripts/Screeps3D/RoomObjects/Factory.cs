using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /**
      {
	        "_id": "5dd07883350edf2a4ac16d06",
	        "type": "factory",
	        "x": 26,
	        "y": 16,
	        "room": "E18S38",
	        "notifyWhenAttacked": true,
	        "user": "5d9a06a66623cbcc5ede74fb",
	        "store": {
		        "energy": 5459,
		        "U": 21,
		        "utrium_bar": 0
	        },
	        "storeCapacity": 50000,
	        "hits": 1000,
	        "hitsMax": 1000,
	        "cooldown": 0,
	        "actionLog": {
		        "produce": null
	        },
	        "cooldownTime": 1,
	        327286E+07
        }
     */
    public class Factory : OwnedStoreStructure, ICooldownObject
    {
        public float Cooldown { get; set; }
        public float Level { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Cooldown(this, data);
        }
    }
}