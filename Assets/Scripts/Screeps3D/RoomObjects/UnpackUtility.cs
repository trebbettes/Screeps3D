﻿using System.Collections.Generic;
using System.Linq;
using Screeps_API;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    public static class UnpackUtility
    {

        internal static void Id(RoomObject roomObject, JSONObject data)
        {
            var idObj = data["_id"];
            if (idObj != null)
                roomObject.Id = idObj.str;
        }

        internal static void Regeneration(IRegenerationObject roomObject, JSONObject data)
        {
            var regenObj = data["nextRegenerationTime"];
            if (regenObj != null)
                roomObject.NextRegenerationTime = regenObj.n;
        }

        internal static void Type(RoomObject roomObject, JSONObject data)
        {
            var typeObj = data["type"];
            if (typeObj != null)
                roomObject.Type = typeObj.str;
        }

        internal static void Position(RoomObject roomObject, JSONObject data)
        {
            var xObj = data["x"];
            if (xObj != null)
                roomObject.X = (int)xObj.n;

            var yObj = data["y"];
            if (yObj != null)
                roomObject.Y = (int)yObj.n;

            var roomNameObj = data["room"];
            if (roomNameObj != null)
                roomObject.RoomName = roomNameObj.str;
        }

        internal static void Energy(IEnergyObject energyObj, JSONObject data)
        {
            var energyCapData = data["energyCapacity"];
            if (energyCapData)
            {
                energyObj.EnergyCapacity = energyCapData.n;
            }

            var energyData = data["energy"];
            if (energyData != null)
            {
                energyObj.Energy = energyData.n > energyObj.EnergyCapacity ? energyObj.EnergyCapacity : energyData.n;
            }
        }

        internal static void Name(INamedObject obj, JSONObject data)
        {
            var nameData = data["name"];
            if (nameData != null)
            {
                obj.Name = nameData.str;
            }
        }

        internal static void HitPoints(IHitpointsObject obj, JSONObject data)
        {
            var hitsData = data["hits"];
            if (hitsData != null)
            {
                obj.Hits = hitsData.n;
            }

            var hitsMaxData = data["hitsMax"];
            if (hitsMaxData != null)
            {
                obj.HitsMax = hitsMaxData.n;
            }
        }

        internal static void Owner(IOwnedObject obj, JSONObject data)
        {
            var userData = data["user"];
            if (userData != null)
            {
                obj.UserId = userData.str;
                obj.Owner = ScreepsAPI.UserManager.GetUser(userData.str);
            }
        }

        internal static void Cooldown(ICooldownObject obj, JSONObject data)
        {
            var coolDownData = data["cooldown"];
            if (coolDownData != null)
            {
                obj.Cooldown = coolDownData.n;
            }
        }

        internal static void Decay(IDecay obj, JSONObject data)
        {
            var decayData = data["nextDecayTime"];

            if (decayData == null)
            {
                decayData = data["decayTime"];
            }

            if (decayData == null)
            {
                decayData = data["ticksToDecay"]; // Portals: The amount of game ticks when the portal disappears, or undefined when the portal is stable.
            }

            if (decayData != null)
            {
                obj.NextDecayTime = decayData.n;
            }
        }

        internal static void Progress(IProgress progressObj, JSONObject data)
        {
            var progressData = data["progress"];
            if (progressData != null)
            {
                progressObj.Progress = progressData.n;
            }
        }

        internal static void Level(ILevel obj, JSONObject data)
        {
            var levelData = data["level"];
            if (levelData != null)
            {
                obj.Level = (int)levelData.n;
            }
        }

        internal static void Store(IStoreObject obj, JSONObject data)
        {

            try
            {
                if (data != null && data.IsObject && data.keys.Count == 0)
                {
                    // bail out early of "empty" updates
                    return;
                }

                // TODO: Convert existing energy data structure to new data structure?

                // ---- PRE STORE UPDATE
                // TODO: convert energy

                if (data.HasField("energyCapacity"))
                {
                    obj.TotalCapacity = data["energyCapacity"].n;
                }

                // ----- POST STORE UPDATE 

                var store = data.HasField("store") ? data["store"] : data; // this supports both PRE and POST store update
                if (store != null && !store.IsNull)
                {
                    foreach (var resourceType in store.keys)
                    {
                        if (!Constants.ResourcesAll.Contains(resourceType)) continue; // Early

                        if (obj.Store.ContainsKey(resourceType))
                        {
                            obj.Store[resourceType] = store[resourceType].n;
                        }
                        else
                        {
                            obj.Store.Add(resourceType, store[resourceType].n);
                        }
                    }
                }

                obj.TotalResources = obj.Store.Sum(a => a.Value);

                if (data.HasField("storeCapacity")) // Labs seems to have this and not storeCapacityResource when they do not contain a mineral type?, atleast according to this https://github.com/screeps/storage/blob/b045531aca745f0942293bd32e0bdb5813bc12e2/lib/db.js#L123-L131
                {
                    obj.TotalCapacity = data["storeCapacity"].n;
                }

                if (data.HasField("storeCapacityResource"))
                {
                    // TODO: store capacity resource is actually an array just like store with a capacity for each store, should probably add a TotalResourceCapacity
                    var storeCapacityResource = data["storeCapacityResource"];
                    if (storeCapacityResource != null && !storeCapacityResource.IsNull)
                    {
                        obj.TotalCapacity = 0;

                        foreach (var resourceType in storeCapacityResource.keys)
                        {
                            if (!Constants.ResourcesAll.Contains(resourceType)) continue; // Early

                            obj.TotalCapacity += storeCapacityResource[resourceType].n;

                            if (obj.Capacity.ContainsKey(resourceType))
                            {
                                obj.Capacity[resourceType] = storeCapacityResource[resourceType].n;
                            }
                            else
                            {
                                obj.Capacity.Add(resourceType, storeCapacityResource[resourceType].n);
                            }
                        } 
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }

        internal static void ActionLog(IActionObject actionObject, JSONObject data)
        {
            var actionLog = data["actionLog"];
            if (actionLog != null)
            {
                actionObject.Actions.Clear();
                foreach (var key in actionLog.keys)
                {
                    var actionData = actionLog[key];
                    actionObject.Actions[key] = actionData;
                }
            }
        }
    }
}