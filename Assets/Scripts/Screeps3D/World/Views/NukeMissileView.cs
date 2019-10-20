using TMPro;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public class NukeMissileView : MonoBehaviour, IWorldOverlayViewComponent
    {
        public NukeMissileOverlay Overlay { get; private set; }

        private NukeMissileArchRenderer arcRenderer;

        public void Init(WorldOverlay overlay)
        {
            Overlay = overlay as NukeMissileOverlay;

            this.arcRenderer = this.gameObject.GetComponentInChildren<NukeMissileArchRenderer>();

            // do we have a launchroom? what if we first acquire the launchroom later?, should this be in update?
            if (Overlay.LaunchRoom != null)
            {
                arcRenderer.point1.transform.position = Overlay.LaunchRoom.Position + new Vector3(25, 0, 25); // Center of the room, because we do not know where the nuke is, could perhaps scan for it and correct it?
                
            }
            var launchRoomText = arcRenderer.point1.GetComponentInChildren<TMP_Text>();
            launchRoomText.text = "";//launcRoom.Name;

            if (Overlay.ImpactRoom != null)
            {
                arcRenderer.point2.transform.position = Overlay.ImpactPosition;
            }

            var point2Text = arcRenderer.point2.GetComponentInChildren<TMP_Text>();
            point2Text.text = ""; //$"{progress*100}%";

            gameObject.name = $"nukeMissile:{Overlay?.LaunchRoom?.Name}->{Overlay?.ImpactRoom?.Name} {Overlay?.Progress * 100}%";
        }

        private void Update()
        {
            // TODO: should we simulate movement / progress in between nukemonitor updates so the misile moves "smoothly"? this neeeds to be in update then. and not sure calling arcRenderer.Progress works, we then need a "targetProgress" or something like that
            arcRenderer.Progress(Overlay.Progress); // TODO: render progress on selection panel when you select the missile.
        }
    }
}