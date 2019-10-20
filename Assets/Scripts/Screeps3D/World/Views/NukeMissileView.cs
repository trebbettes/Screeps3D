using Screeps_API;
using TMPro;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public class NukeMissileView : MonoBehaviour, IWorldOverlayViewComponent
    {
        public NukeMissileOverlay Overlay { get; private set; }

        private NukeMissileArchRenderer arcRenderer;

        private bool initialized = false;

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
            arcRenderer.Progress(Overlay.Progress - 0.3f); // give a little smoke trail when initialized
            //arcRenderer.Progress(Overlay.Progress); // TODO: render progress on selection panel when you select the missile.

            initialized = true;
        }

        private void Update()
        {
            if (Overlay == null)
            {
                return;
            }

            if (!initialized)
            {
                return;
            }

            // TODO: should we simulate movement / progress in between nukemonitor updates so the misile moves "smoothly"? this neeeds to be in update then. and not sure calling arcRenderer.Progress works, we then need a "targetProgress" or something like that, could let us inspire by creep movement between ticks
            // TODO: should perhaps move this calculation so progress is updated on each tick? and not each rendering?
            float progress = (float)(ScreepsAPI.Time - Overlay.InitialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;

            Debug.Log(this.Overlay.Id);
            arcRenderer.Progress(progress);

            gameObject.name = $"nukeMissile:{this.Overlay.Id}:{Overlay?.LaunchRoom?.Name}->{Overlay?.ImpactRoom?.Name} {progress * 100}%";

        }
    }
}