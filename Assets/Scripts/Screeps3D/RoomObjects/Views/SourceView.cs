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
            var visibility = _source.Energy / _source.EnergyCapacity + 0.01f/*to keep it visible and selectable, also allows the resource to regen*/;
            // https://www.thoughtco.com/exponential-decay-definition-2312215
            // https://en.wikipedia.org/wiki/Logarithmic_scale
            // TODO: scale the visibility in such a way that a lot of the model is still rendered when 50% energy is left
            _Visibility.SetVisibility(visibility);
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}