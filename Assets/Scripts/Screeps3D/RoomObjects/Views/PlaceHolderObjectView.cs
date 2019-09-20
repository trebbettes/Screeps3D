using Common;
using TMPro;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    [ExecuteAlways]
    public class PlaceHolderObjectView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleVisibility _vis;
        [SerializeField] private GameObject _rotation;
        [SerializeField] private TextMeshPro _label;
        [SerializeField] private LineRenderer _line;

        private PlaceHolderRoomObject _roomObject;

        public void Init()
        {
            
        }

        public void Load(RoomObject roomObject)
        {
            _roomObject = roomObject as PlaceHolderRoomObject;
            _label.text = _roomObject.Type;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 3, transform.localPosition.z);
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void Update()
        {
            if (Application.IsPlaying(gameObject))
            {
                // Play logic
                _rotation.transform.rotation = Camera.main.transform.rotation;
            }
        }

        private void OnRenderObject()
        {
            if (!Application.IsPlaying(gameObject))
            {
                // Editor logic
                if (UnityEditor.SceneView.lastActiveSceneView.camera != null)
                {
                    _rotation.transform.rotation = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
                }
            }
        }
    }
}