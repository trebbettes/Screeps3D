using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class SourceView : MonoBehaviour, IObjectViewComponent
    {
        //[SerializeField] private ScaleAxes _energyDisplay;
        [SerializeField] private ScaleVisibility _Visibility;
        private Source _source;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _source = roomObject as Source;
        }

        public void Delta(JSONObject data)
        {
            var visibility = _source.Energy / _source.EnergyCapacity;
            _Visibility.SetVisibility(visibility);
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}