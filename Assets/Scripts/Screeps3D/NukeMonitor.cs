using Common;
using Screeps_API;
using Screeps3D;
using Screeps3D.Player;
using Screeps3D.Rooms;
using Screeps3D.World.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Screeps3D
{
    /*  https://docs.screeps.com/api/#StructureNuker
     *  A Nuke roomObject is created in the target room, so impact handling can be done by that roomObject
     *  But do we want to see a visual explosion in the distance? perhaps impact should be handled on a general case?
     *  
     *  A nuke has a range of 10 rooms
     *  It takes 50.000 ticks to land
     **/
   
    public class NukeMonitor : BaseSingleton<NukeMonitor>
    {
        private Dictionary<string, NukeMissileOverlay> _nukes = new Dictionary<string, NukeMissileOverlay>();

        private void Start()
        {
            PlayerPosition.Instance.OnRoomChange += OnRoomChange; // this triggers twice, we might need a more reliable way to detect when loaded
        }

        private void OnRoomChange()
        {

            NotifyText.Message("SCANNING FOR NUKES!", Color.red);

            // We might have an issue if people use custom shard names, so we can't use shardName, because playerposition shardname is shardX
            var shardIndex = PlayerPosition.Instance.ShardLevel;

            // Should probably cache this, and refresh it at an interval to detect new nukes.
            ScreepsAPI.Http.GetExperimentalNukes((jsonString) => {

                var obj = new JSONObject(jsonString);
                var status = obj["ok"];
                var nukes = obj["nukes"];
                var nukesShardName = nukes.keys[shardIndex];
                var shardName = ScreepsAPI.Cache.MMO ? nukesShardName : $"shard{shardIndex}";
                var shardNukes = nukes[nukesShardName].list;
                NotifyText.Message($"{nukesShardName} has {shardNukes.Count} nukes!", Color.red);
                Debug.LogWarning(shardNukes.ToString());

                var time = ScreepsAPI.Time;
                // TODO: getting time should be moved to when logging or switching shards
                ScreepsAPI.Http.Request("GET", $"/api/game/time?shard={nukesShardName}", null, (jsonTime) => {
                    var timeData = new JSONObject(jsonTime)["time"];
                    if (timeData != null)
                    {
                        ScreepsAPI.Time = time = (long)timeData.n;
                    }
                    
                    foreach (var nuke in shardNukes)
                    {
                        var id = nuke["_id"].str; // should probably switch to UnPackUtility later.

                        if (!_nukes.TryGetValue(id, out var overlay)) {
                            // TODO: further detection if this was a newly launched nuke. perhaps the progress is at a really low percentage, or between x ticks?
                            overlay = new NukeMissileOverlay(id);
                            _nukes.Add(id, overlay);
                        }

                        // TODO: overlay.Unpack?

                        // TODO: should probably not be doing this everytime we get nuke data, if it is already initialized?
                        overlay.LaunchRoom = RoomManager.Instance.Get(nuke["launchRoomName"].str, shardName);
                        overlay.ImpactRoom = RoomManager.Instance.Get(nuke["room"].str, shardName);
                        overlay.ImpactPosition = PosUtility.Convert(nuke, overlay.ImpactRoom);


                        var nukeLandTime = nuke["landTime"];

                        var landingTime = nukeLandTime.IsNumber ? (long)nukeLandTime.n : long.Parse(nukeLandTime.str.Replace("\"",""));
                        
                        var initialLaunchTick = Math.Max(landingTime - Constants.NUKE_TRAVEL_TICKS,0);
                        var progress = (float)(time - initialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;

                        overlay.LandingTime = landingTime;
                        overlay.InitialLaunchTick = initialLaunchTick;
                        overlay.Progress = progress;
                    }

                    if (shardNukes.Count > 0)
                    {
                        // W need to persist data and detect when changing shard.
                        // should probably also clear rendered nukes from previous shard?
                        PlayerPosition.Instance.OnRoomChange -= OnRoomChange;
                    }
                });


                

                // TODO: launch detected events


                /* Example
                 *  {
	                    "ok": 1,
	                    "nukes": {
		                    "shard0": [],
		                    "shard1": [],
		                    "shard2": [{
				                    "_id": "5d26127173fcd27b55a7ef39",
				                    "type": "nuke",
				                    "room": "W23S15",
				                    "x": 12,
				                    "y": 37,
				                    "landTime": 17300541,
				                    "launchRoomName": "W31S18"
			                    }, {
				                    "_id": "5d26aa11385277180e5c2187",
				                    "type": "nuke",
				                    "room": "W22S22",
				                    "x": 23,
				                    "y": 22,
				                    "landTime": 17311981,
				                    "launchRoomName": "W17S28"
			                    }
		                    ],
		                    "shard3": []
	                    }
                    }
                 */

            });
        }

        // How do we instantiate the object, it's kinda like RoomObjects that has a view attached, we should probably do something like that
        // except this object can be between rooms, and should be using a "global" position, for placement. probably need to use PosUtility to some regards
        // perhaps a raytrace or something like playerpostion to figure out what room it is in check the update method
        // we also need to convert the impact room to a world position to figure out where to draw the arch / missile path to

        private class NukeData
        {
            public NukeData(JSONObject nuke)
            {

            }
        }
        //private ObjectView NewInstance(string type)
        //{
        //    var go = PrefabLoader.Load(string.Format("{0}{1}", _path, type));
        //    if (go == null)
        //        return null;
        //    var view = go.GetComponent<ObjectView>();
        //    view.transform.SetParent(_objectParent);
        //    view.Init();
        //    return view;
        //}
        //protected void EnterRoom(Room room)
        //{
        //    Room = room;

        //    if (View == null)
        //    {
        //        Scheduler.Instance.Add(AssignView);
        //    }

        //    Shown = true;
        //    if (OnShow != null)
        //        OnShow(this, true);
        //}

        
    }
}
