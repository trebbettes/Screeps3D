using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Screeps3D.Menus.Options;
using Screeps3D.RoomObjects;
using Screeps3D.RoomObjects.Views;
using Screeps3D.Rooms.Views;
using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public class EditFlag : BaseSingleton<EditFlag>
    {
        [SerializeField] private EditFlagPopup _editFlagPopup;

        private void Start()
        {
            _editFlagPopup.OnFlagCreated += FlagCreated; // propbably want a FlagUpdated event here
            _editFlagPopup.OnCancel += CancelFlagEdit;
        }

        private void OnDestroy()
        {
            _editFlagPopup.OnFlagCreated -= FlagCreated;
            _editFlagPopup.OnCancel -= CancelFlagEdit;
        }

        private void CancelFlagEdit()
        {
            ////Debug.Log("CancelFlagEdit");
            ////_isPlacing = false;
            ToggleEditFlagPopup(false);
        }

        private void FlagCreated()
        {
            ////_isPlacing = false;
            ToggleEditFlagPopup(false);
        }
        public void ShowEditDialog(Flag flag)
        {
            this.enabled = true;
            ToggleEditFlagPopup(true);
            _editFlagPopup.Load(flag);
        }
        private void ToggleEditFlagPopup(bool? active = null)
        {
            if (!active.HasValue)
            {
                active = _editFlagPopup.gameObject.activeSelf;
            }

            _editFlagPopup?.gameObject?.SetActive(active.Value);
        }

        private void OnDisable()
        {
            ToggleEditFlagPopup(false);
        }
    }
}