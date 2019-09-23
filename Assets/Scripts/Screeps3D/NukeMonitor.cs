using Common;
using Screeps_API;
using Screeps3D;
using Screeps3D.Player;
using Screeps3D.Rooms;
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
        private const int NUKE_ROOM_RANGE = 10;
        private const int NUKE_TRAVEL_TICKS = 50000;

        private const string Path = "Prefabs/WorldView/";

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
                var shardNukes = nukes[nukesShardName].list;
                NotifyText.Message($"{nukesShardName} has {shardNukes.Count} nukes!", Color.red);
                Debug.LogWarning(shardNukes.ToString());

                var time = ScreepsAPI.Time;
                ScreepsAPI.Http.Request("GET", $"/api/game/time?shard={nukesShardName}", null, (jsonTime) => {
                    var timeData = new JSONObject(jsonTime)["time"];
                    if (timeData != null)
                    {
                        time = (long)timeData.n;
                    }

                    // TODO: WorldViewFactory like RoomViewFactory?
                    foreach (var nuke in shardNukes)
                    {
                        var go = PrefabLoader.Load(string.Format("{0}{1}", Path, "nukeMissile"));
                        var arcRenderer = go.GetComponentInChildren<NukeMissileArchRenderer>();

                        var launcRoom = RoomManager.Instance.Get(nuke["launchRoomName"].str, nukesShardName);
                        arcRenderer.point1.transform.position = launcRoom.Position + new Vector3(25, 0, 25);
                        var point1Text = arcRenderer.point1.GetComponentInChildren<TMP_Text>();
                        point1Text.text = launcRoom.Name;

                        var impactRoom = RoomManager.Instance.Get(nuke["room"].str, nukesShardName);
                        arcRenderer.point2.transform.position = PosUtility.Convert(nuke, impactRoom);
                        var point2Text = arcRenderer.point2.GetComponentInChildren<TMP_Text>();
                        

                        var landingTime = (long)nuke["landTime"].n;
                        var initialLaunchTick = landingTime - NUKE_TRAVEL_TICKS;
                        var progress = (float)(time - initialLaunchTick) / NUKE_TRAVEL_TICKS;
                        arcRenderer.Progress(progress);

                        point2Text.text = $"{progress*100}%";

                        go.name = $"nukeMissile:{launcRoom.Name}->{impactRoom.Name} {progress*100}%";
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

        //protected internal virtual void AssignView()
        //{
        //    if (Shown && View == null)
        //    {
        //        View = ObjectViewFactory.Instance.NewView(this);
        //        if (View)
        //            View.Load(this);
        //    }
        //}
    }
}
