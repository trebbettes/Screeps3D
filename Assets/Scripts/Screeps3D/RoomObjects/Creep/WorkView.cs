﻿using UnityEngine;

namespace Screeps3D
{
    public class WorkView : CreepPartView
    {
        [SerializeField] private Animator _anim;

        public override void Init(RoomObject roomObject)
        {
            base.Init(roomObject);
            AdjustSize("work", .2f, .8f);
        }

        public override void Delta(JSONObject data)
        {
            base.Delta(data);
            AdjustSize("work", .2f, .8f);

            if (creep.Actions.ContainsKey("harvest") && !creep.Actions["harvest"].IsNull)
            {
                var rotation = Quaternion.LookRotation(GetActionVector(creep.Actions["harvest"]));
                view.rotTarget = rotation;
                _anim.SetTrigger("activate");
            }
        }
    }
}