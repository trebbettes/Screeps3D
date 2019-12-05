using System.Collections.Generic;
using Common;
using Screeps_API;
using UnityEngine;
using Screeps3D.RoomObjects.Views;
using Screeps3D.RoomObjects;

namespace Screeps3D.Rooms.Views
{
    public class RoomGridView : MonoBehaviour, IRoomViewComponent
    {
        public Room Room { get; private set; }

        [SerializeField] private TerrainView _terrainView; // TODO: in case we want to deform the grid based on terrain
        [SerializeField] private GameObject _grid;

        public void Init(Room room)
        {
            Room = room;
            Room.OnShowObjects += OnShowObjects;
        }

        private void OnShowObjects(bool show)
        {
            if (show)
            {
                // Only render grid in subscribed room where we are showing objects.
                _grid.SetActive(true);
            }
            else
            {
                _grid.SetActive(false);
            }
        }
    }
}