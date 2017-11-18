﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Screeps3D {
    public class EntityView : MonoBehaviour {
        
        [SerializeField] private ScreepsAPI api;
        [SerializeField] private ObjectManager manager;
        
        private Dictionary<string, RoomObject> roomObjects = new Dictionary<string, RoomObject>();
        private Queue<JSONObject> roomData = new Queue<JSONObject>();
        private WorldCoord coord;
        private string path;
        
        public void Load(WorldCoord coord) {
            this.coord = coord;
            
            if (api.Address.hostName.ToLowerInvariant() == "screeps.com") {
                path = string.Format("room:{0}/{1}", coord.shardName, coord.roomName);
            } else {
                path = string.Format("room:{0}", coord.roomName);
            }
            
            api.Socket.Subscribe(path, OnRoomData);
        }

        private void OnDestroy() {
            if (api.Socket != null && coord != null) {
                api.Socket.Unsub(path);
            }
        }

        private void OnRoomData(JSONObject data) {
            roomData.Enqueue(data);
        }

        private void Update() {
            if (roomData.Count == 0)
                return;
            RenderEntities(roomData.Dequeue());
        }

        private void RenderEntities(JSONObject data) {
            var objects = data["objects"];
            foreach (var id in objects.keys) {
                var datum = objects[id];
                
                if (datum["type"] && datum["type"].str == "wall") {
                    Debug.Log(datum);
                }

                RoomObject roomObject;
                if (roomObjects.ContainsKey(id)) {
                    roomObject = roomObjects[id];
                } else {
                    roomObject = manager.Get(id, datum);
                    roomObjects[id] = roomObject;
                    if (roomObject.View) {
                        roomObject.View.transform.SetParent(transform, false);
                    }
                }
                
                if (datum.IsNull) {
                    manager.Remove(id);
                    this.roomObjects.Remove(id);
                } else {
                    roomObject.Delta(datum);   
                }
            }
        }
    }
}