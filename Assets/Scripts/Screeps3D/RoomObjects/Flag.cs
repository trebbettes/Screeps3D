using System.Collections.Generic;
using Assets.Scripts.Screeps3D.Tools.Selection;
using Screeps3D.Rooms;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    public class Flag: RoomObject, INamedObject, IButtons
    {
        public string Name { get; set; }

        public int SecondaryColor { get; set; }
        public int PrimaryColor { get; set; }

        public Flag(string name)
        {
            Id = name;
            Name = name;
            Type = "flag";
        }

        public void FlagDelta(string[] dataArray, Room room)
        {
            PrimaryColor = int.Parse(dataArray[1]);
            SecondaryColor = int.Parse(dataArray[2]);
            X = int.Parse(dataArray[3]);
            Y = int.Parse(dataArray[4]);
            Room = room;
            RoomName = room.RoomName;
        }

        public List<IRoomObjectPanelButton> GetButtonActions()
        {
            return new List<IRoomObjectPanelButton>
            {
                new SelectionRoomObjectButton<Flag>("Change position", (flag) => Debug.Log($"{flag.Name} change position clicked")),
                new SelectionRoomObjectButton<Flag>("Change color", (flag) => Debug.Log($"{flag.Name} change color clicked")),
                new SelectionRoomObjectButton<Flag>("Remove flag", (flag) => Debug.Log($"{flag.Name} remove flag clicked")),
                //new SelectionRoomObjectButton<Flag>("view memory", (flag) => Debug.Log($"{flag.Name} remove flag clicked"))
            };
        }
    }
}