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
            _editFlagPopup.OnFlagCreated += FlagCreated;
            _editFlagPopup.OnFlagColorChanged += FlagColorChanged;
            _editFlagPopup.OnCancel += CancelFlagEdit;
        }

        private void OnDestroy()
        {
            _editFlagPopup.OnFlagCreated -= FlagCreated;
            _editFlagPopup.OnFlagColorChanged -= FlagColorChanged;
            _editFlagPopup.OnCancel -= CancelFlagEdit;
        }

        private void CancelFlagEdit()
        {
            ////Debug.Log("CancelFlagEdit");
            ////_isPlacing = false;
            ToggleEditFlagPopup(false);
        }
        private void FlagColorChanged()
        {
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

        public void DeleteFlag()
        {

            //  POST https://screeps.com/api/game/remove-flag
            // request: {"room":"E19S38","shard":"shard3","name":"Flag2"}
            // response: {"ok":1,"result":{"n":1,"nModified":1,"ok":1},"connection":{"id":8,"host":"dhost3.srv.screeps.com","port":25270},"message":{"parsed":true,"index":75,"raw":{"type":"Buffer","data":[75,0,0,0,142,100,226,151,150,40,22,2,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"data":{"type":"Buffer","data":[75,0,0,0,142,100,226,151,150,40,22,2,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"bson":{},"opts":{"promoteLongs":true,"promoteValues":true,"promoteBuffers":false},"length":75,"requestId":-1746770802,"responseTo":35006614,"responseFlags":8,"cursorId":"0","startingFrom":0,"numberReturned":1,"documents":[{"n":1,"nModified":1,"ok":1}],"cursorNotFound":false,"queryFailure":false,"shardConfigStale":false,"awaitCapable":true,"promoteLongs":true,"promoteValues":true,"promoteBuffers":false,"hashedName":"8cf87ebd96d4f56356284e048c6646c112baf617"}}

        }

        private void ToggleEditFlagPopup(bool? active = null)
        {
            if (!active.HasValue)
            {
                active = _editFlagPopup.gameObject.activeSelf;
            }

            this.enabled = active.Value;

            _editFlagPopup?.gameObject?.SetActive(active.Value);
        }

        private void OnDisable()
        {
            ToggleEditFlagPopup(false);
        }
    }
}