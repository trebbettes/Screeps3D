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
            var percentage = _source.Energy / _source.EnergyCapacity;

            // http://james-ramsden.com/map-a-value-from-one-number-scale-to-another-formula-and-c-code/
            float minimum = Mathf.Exp(0);
            float maximum = Mathf.Exp(1);

            // Scale the visibility in such a way that a lot of the model is rendered above 50% energy
            float current = Mathf.Exp(percentage);

            // Map exponential range to visibility range
            var minVisibility = 0.001f; /*to keep it visible and selectable, also allows the resource to render again when regen hits*/
            var maxVisibility = 1f;

            var visibility = minVisibility + (maxVisibility - minVisibility) * ((current - minimum) / (maximum - minimum));

            _Visibility.SetVisibility(visibility);
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}