using Common;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public static class WorldViewFactory
    {
        private static Transform _parent;

        private const string Path = "Prefabs/WorldView/";

        public static WorldView GetInstance(WorldOverlay overlay)
        {
            if (_parent == null)
            {
                _parent = new GameObject("WorldViews").transform;
            }

            var go = PrefabLoader.Load(string.Format("{0}{1}", Path, overlay.Type), _parent);
            var view = go.GetComponent<WorldView>();
            //view.gameObject.name = overlay.Name;
            //view.transform.localPosition = overlay.Position;
            view.Init(overlay);
            return view;
        }
    }
}