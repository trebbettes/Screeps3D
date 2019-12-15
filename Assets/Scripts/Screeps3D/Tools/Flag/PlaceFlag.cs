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
    public class PlaceFlag : BaseSingleton<PlaceFlag>
    {
        [SerializeField] private EditFlagPopup _editFlagPopup;

        private bool _isPlacing;
        private Vector2Int _position;


        private Flag _flag;

        private void Start()
        {
            Debug.Log("PlaceFlag Start");
            _flag = new Flag("PlaceFlag");
            _flag.PrimaryColor = (int)Constants.FlagColor.White;
            _flag.SecondaryColor = (int)Constants.FlagColor.White;

            _editFlagPopup.OnFlagCreated += FlagCreated;
            _editFlagPopup.OnCancel += CancelFlagCreation;
        }

        private void OnDestroy()
        {
            _editFlagPopup.OnFlagCreated -= FlagCreated;
            _editFlagPopup.OnCancel -= CancelFlagCreation;
        }

        private void CancelFlagCreation()
        {
            _isPlacing = false;
            ToggleEditFlagPopup(false);
        }

        private void FlagCreated()
        {
            _isPlacing = false;
            ToggleEditFlagPopup(false);
        }

        private void ToggleEditFlagPopup(bool? active = null)
        {
            if (!active.HasValue)
            {
                active = _editFlagPopup.gameObject.activeSelf;
            }

            if (active.Value)
            {
                _editFlagPopup.Load(_flag, true);
            }

            _editFlagPopup.gameObject.SetActive(active.Value);
        }

        private void OnDisable()
        {
            if (_flag.View != null)
            {
                _flag.HideObject(_flag.Room);
            }

            ToggleEditFlagPopup(false);
        }

        ////public void Highlight()
        ////{
        ////    if (!_rend)
        ////    {
        ////        _rend = GetComponent<Renderer>();
        ////        _original = _rend.material.color.r;
        ////    }
        ////    _target = _original + .1f;
        ////    enabled = true;
        ////}

        ////public void Dim()
        ////{
        ////    _target = _original;
        ////    enabled = true;
        ////}

        ////public void Update()
        ////{
        ////    if (!_rend || Mathf.Abs(_current - _target) < .001f)
        ////    {
        ////        enabled = false;
        ////        return;
        ////    }
        ////    _current = Mathf.SmoothDamp(_rend.material.color.r, _target, ref _targetRef, 1);
        ////    _rend.material.color = new Color(_current, _current, _current);
        ////}

        private void Update()
        {
            if (!InputMonitor.OverUI && !_isPlacing)
            {
                var rayTarget = Rayprobe();
                //SelectionOutline.DrawOutline(rayTarget);
                // move flage location, flag should be alphablended 
                // TODO: center flag on tile position
                // we need to spawn / show a flagview prefab and position it. we should only load it when enabled though? and remove it when disabled?
                if (rayTarget.HasValue)
                {
                    // What room are we in? perhaps we should look at PlayerGaze
                    var roomView = rayTarget.Value.collider.GetComponent<RoomView>();
                    if (roomView == null)
                    {
                        return;
                    }

                    var room = roomView.Room;

                    ////Debug.Log(room.Position);
                    ////Debug.Log(room.Position - rayTarget.Value.point);
                    var roomPosition = PosUtility.ConvertToXY(rayTarget.Value.point, room);
                    if (roomPosition.x != _flag.X || roomPosition.y != _flag.Y)
                    {
                        ////Debug.Log(roomPosition);
                        ////Debug.Log($"flag: {_flag.X}, {_flag.Y} => {_flag.Position} == {PosUtility.Convert(_flag.X, _flag.Y, room)}");
                        ////Debug.Log("placeflag delta");
                        _flag.Delta(new JSONObject($"{{\"x\":{roomPosition.x},\"y\":{roomPosition.y}}}"), room);

                        // Move flag, flags normally don't move.
                        if (_flag.View != null)
                        {
                            ////Debug.Log($"{_flag?.Room?.ShardName}/{_flag?.Room?.RoomName}");
                            _flag.View.transform.localPosition = _flag.Position;
                        }
                    }
                }
            }

            ////if (Input.GetMouseButtonDown(0) && !InputMonitor.OverUI)
            ////{
            ////    _isPlacing = true;
                
            ////}

            if (!_isPlacing && Input.GetMouseButtonUp(0) && !InputMonitor.OverUI)
            {
                _isPlacing = true;
                ToggleEditFlagPopup(true);
            }
        }

        private RaycastHit? Rayprobe()
        {
            RaycastHit hitInfo;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics.Raycast(ray, out hitInfo, 1000f, 1 << 10 /* roomView */);
            if (!hit) return null; // Early

            return hitInfo;
            //return hitInfo.transform.gameObject.GetComponent<ObjectView>();
        }
    }
}