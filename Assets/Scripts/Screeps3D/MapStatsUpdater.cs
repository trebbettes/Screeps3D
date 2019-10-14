using System;
using System.Collections.Generic;
using Common;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;
using System.Linq;
using Screeps3D.Player;
using System.Collections;

namespace Screeps3D
{
    // This seems more like "room info" in regards to status of the room
    public class MapStatsUpdater : BaseSingleton<MapStatsUpdater>
    {
        private Dictionary<string, RoomInfo> _roomInfo = new Dictionary<string, RoomInfo>();

        // like room chooser, should start a coroutine

        // for starters, lets just fetch data for all loaded rooms.

        // trigger an update for mapstats

        private void Start()
        {
            StartCoroutine(Scan());
        }

        public RoomInfo GetRoomInfo(string roomName) => _roomInfo.ContainsKey(roomName) ? _roomInfo[roomName] : null; // Do we need shardname support?
        public IEnumerator Scan()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);

                try
                {
                    Action<string> serverCallback = jsonString =>
                    {
                        var result = new JSONObject(jsonString);

                        UnpackUsers(result["users"]);

                        var stats = result["stats"];
                        foreach (var roomName in stats.keys)
                        {
                            // should we store this info on the room so it is available for others?
                            var roomInfo = _roomInfo.ContainsKey(roomName) ? _roomInfo[roomName] : null;
                            if (roomInfo == null)
                            {
                                roomInfo = new RoomInfo(stats[roomName]);
                                _roomInfo[roomName] = roomInfo;
                            }

                            roomInfo.Unpack(stats[roomName]);

                            roomInfo.Time = (long)result["gameTime"].n;
                        }
                    };

                    var rooms = RoomManager.Instance.Rooms.Select(r => r.RoomName).ToList();
                    // what shard should we be getting info from? had the same issue in the nuker
                    // We might have an issue if people use custom shard names, so we can't use shardName, because playerposition shardname is shardX
                    var shardName = PlayerPosition.Instance.ShardName;

                    ScreepsAPI.Http.GetMapStats(rooms, shardName, "owner0", serverCallback);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                yield return new WaitForSeconds(30);
            }
        }

        private void UnpackUsers(JSONObject data)
        {
            var usersData = data["users"];
            if (usersData == null)
            {
                return;
            }

            foreach (var id in usersData.keys)
            {
                var userData = usersData[id];
                ScreepsAPI.UserManager.CacheUser(userData);
            }
        }
    }

    public class RoomInfo
    {
        /// <summary>
        /// Amount of ticks in gameTime
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// status can at least be "normal" or "out of borders" 
        /// </summary>
        public string Status { get; set; }

        public bool IsNoviceZone { get; set; }
        public string NoviceTime { get; set; }

        public bool IsRespawnArea { get; set; }
        public string RespawnAreaTime { get; set; }

        public JSONObject Sign { get; set; } // adjust at later date

        public ScreepsUser User { get; set; }

        /// <summary>
        /// the actual time on the safemode is on the controller, so we only know if safemode is up or not.
        /// </summary>
        public bool HasSafeMode { get; set; }

        /// <summary>
        /// Unix timestamp for when it opens if a sector is pending opening
        /// </summary>
        public string OpenTime { get; set; } // TODO: convert to datetime

        public bool IsReserved { get; set; }

        public RoomInfo(JSONObject roomStats)
        {
            this.Unpack(roomStats);
        }

        internal void Unpack(JSONObject roomStats)
        {
            var status = roomStats["status"];
            var own = roomStats["own"];
            if (own != null && !own.IsNull)
            {
                var user = own["user"];
                if (user != null && !user.IsNull)
                {
                    var userId = user.str;
                    this.User = ScreepsAPI.UserManager.GetUser(userId);
                }

                var level = (int)own["level"].n;

                this.IsReserved = level == 0;
            }

            this.Status = status.str;

            var openTime = roomStats["openTime"];
            this.OpenTime = openTime != null && !openTime.IsNull ? openTime.str : null;

            var novice = roomStats["novice"];
            this.IsNoviceZone = novice != null && !novice.IsNull;
            this.NoviceTime = this.IsNoviceZone ? novice.n.ToString() : null;

            var respawnArea = roomStats["respawnArea"];
            this.IsRespawnArea = respawnArea != null && !respawnArea.IsNull;
            this.RespawnAreaTime = this.IsRespawnArea ? respawnArea.n.ToString() : null;

            var safeMode = roomStats["safeMode"];
            this.HasSafeMode = safeMode != null && !safeMode.IsNull && safeMode.b;

            // could also store mineral type and density for future use in map overview
        }
    }
}