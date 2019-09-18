using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    // TODO: do we actually have anything that uses energyview anymore? should this script be replaced by storeview on most gameobjects?
    public class EnergyView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleVisibility _energyDisplay;

        private IEnergyObject _energyObject;
        private IStoreObject _storeObject;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _energyObject = roomObject as IEnergyObject;
            _storeObject = roomObject as IStoreObject;

            AdjustScale();
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_energyObject != null)
            {
                _energyDisplay.SetVisibility(_energyObject.Energy / _energyObject.EnergyCapacity);
            }

            if (_storeObject != null)
            {
                float energy;
                _storeObject.Store.TryGetValue(Constants.TypeResource, out energy);

                _energyDisplay.SetVisibility(energy / _storeObject.TotalCapacity);
            }
        }
    }
}