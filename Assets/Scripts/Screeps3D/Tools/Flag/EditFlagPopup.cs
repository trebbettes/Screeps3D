using Screeps_API;
using Screeps3D.RoomObjects;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Tools.Selection
{
    public class EditFlagPopup : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _flagName;
        [SerializeField] private FlagColorToggle _primaryFlagColor;
        [SerializeField] private FlagColorToggle _secondaryFlagColor;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _okButton;

        private bool _newFlag;
        private Flag _flag;

        public Action OnFlagCreated;

        public Action OnCancel;

        private void Start()
        {
            _cancelButton.onClick.AddListener(CancelClicked);
            _okButton.onClick.AddListener(OkClicked);
            _flagName.onDeselect.AddListener(FlagNameDeselect);

            _primaryFlagColor.OnColorChange += PrimaryColorChange;
            _secondaryFlagColor.OnColorChange += SecondaryColorChange;
        }
        private void OnDestroy()
        {
            _cancelButton.onClick.RemoveListener(CancelClicked);
            _okButton.onClick.RemoveListener(OkClicked);
            _flagName.onDeselect.RemoveListener(FlagNameDeselect);

            _primaryFlagColor.OnColorChange -= PrimaryColorChange;
            _secondaryFlagColor.OnColorChange -= SecondaryColorChange;
        }

        public void Load(Flag flag, bool newFlag = false)
        {
            ////Debug.Log($"flag loaded create {newFlag}=> {_flag?.Room?.ShardName}/{_flag?.Room?.RoomName}");
            _flag = flag;
            _newFlag = newFlag;

            // TODO: store initial values so we can reset the flag on cancel.

            if (newFlag)
            {
                GenerateUniqueFlagName();
            }
            else
            {

                _flagName.text = flag.Name;
                _primaryFlagColor.SetColor((Constants.FlagColor)_flag.PrimaryColor);
                _secondaryFlagColor.SetColor((Constants.FlagColor)_flag.SecondaryColor);
            }
        }

        private void GenerateUniqueFlagName()
        {
            ScreepsAPI.Http.GenerateUniqueFlagName(_flag.Room.ShardName, onSuccess: jsonString =>
            {
                var result = new JSONObject(jsonString);

                var ok = result["ok"];

                if (ok.n == 1)
                {
                    var flagName = result["name"];
                    _flagName.text = flagName.str;
                    _flag.Name = _flagName.text;
                }
            });
        }

        public void Save()
        {
            if (_newFlag)
            {
                ScreepsAPI.Http.CreateFlag(
                    _flag.Room.ShardName,
                    _flag.Room.RoomName,
                    _flag.X,
                    _flag.Y,
                    _flag.Name,
                    _flag.PrimaryColor,
                    _flag.SecondaryColor,
                    onSuccess: jsonString =>
                {
                    var result = new JSONObject(jsonString);

                    var ok = result["ok"];

                    if (ok.n == 1)
                    {
                        OnFlagCreated?.Invoke();
                    }
                    else
                    {
                        // error
                    }
                });
            }
            else
            {
                // Update flag?
                // TODO: we can change flag position as well, how do we trigger flag movement when we edit an existing flag?

                // POST https://screeps.com/api/game/change-flag-color
                // Request: {"room":"E19S38","shard":"shard3","name":"Flag1","color":10,"secondaryColor":7}
                // Response: {"ok":1,"result":{"n":1,"nModified":1,"ok":1},"connection":{"id":4,"host":"dhost3.srv.screeps.com","port":25270},"message":{"parsed":true,"index":75,"raw":{"type":"Buffer","data":[75,0,0,0,186,216,193,77,139,130,92,0,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"data":{"type":"Buffer","data":[75,0,0,0,186,216,193,77,139,130,92,0,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"bson":{},"opts":{"promoteLongs":true,"promoteValues":true,"promoteBuffers":false},"length":75,"requestId":1304549562,"responseTo":6062731,"responseFlags":8,"cursorId":"0","startingFrom":0,"numberReturned":1,"documents":[{"n":1,"nModified":1,"ok":1}],"cursorNotFound":false,"queryFailure":false,"shardConfigStale":false,"awaitCapable":true,"promoteLongs":true,"promoteValues":true,"promoteBuffers":false,"hashedName":"8cf87ebd96d4f56356284e048c6646c112baf617"}}

                // moving flag seems to just call create flag again
                //  POST https://screeps.com/api/game/create-flag
                // request: {"x":38,"y":23,"name":"Flag1","color":10,"secondaryColor":7,"room":"E19S38","shard":"shard3"}
                // response: {"ok":1,"result":{"n":1,"nModified":1,"ok":1},"connection":{"id":16,"host":"dhost3.srv.screeps.com","port":25270},"message":{"parsed":true,"index":75,"raw":{"type":"Buffer","data":[75,0,0,0,210,79,229,77,233,142,92,0,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"data":{"type":"Buffer","data":[75,0,0,0,210,79,229,77,233,142,92,0,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"bson":{},"opts":{"promoteLongs":true,"promoteValues":true,"promoteBuffers":false},"length":75,"requestId":1306873810,"responseTo":6065897,"responseFlags":8,"cursorId":"0","startingFrom":0,"numberReturned":1,"documents":[{"n":1,"nModified":1,"ok":1}],"cursorNotFound":false,"queryFailure":false,"shardConfigStale":false,"awaitCapable":true,"promoteLongs":true,"promoteValues":true,"promoteBuffers":false,"hashedName":"8cf87ebd96d4f56356284e048c6646c112baf617"}}
            }
        }

        private void PrimaryColorChange(Constants.FlagColor flagColor)
        {
            _flag.PrimaryColor = (int)flagColor;
        }
        private void SecondaryColorChange(Constants.FlagColor flagColor)
        {
            _flag.SecondaryColor = (int)flagColor;
        }

        private void FlagNameDeselect(string text)
        {
            Debug.Log("FlagNameDeselect");
            // POST https://screeps.com/api/game/check-unique-flag-name
            // Request: {"name":"Flag1","shard":"shard3"}
            // Response: {"error":"name exists"} || {"ok":1}
        }

        private void CancelClicked()
        {
            OnCancel?.Invoke();
        }

        private void OkClicked()
        {
            Save();
        }
    }
}