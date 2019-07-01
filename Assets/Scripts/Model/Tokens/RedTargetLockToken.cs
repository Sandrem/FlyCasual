using Obstacles;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class RedTargetLockToken : GenericTargetLockToken
    {
        public ITargetLockable HostTargetable { get; private set; }

        public RedTargetLockToken(ITargetLockable host) : base(null)
        {
            Name = ImageName = "Red Target Lock Token";
            TokenColor = TokenColors.Red;
            PriorityUI = 20;

            HostTargetable = host;
            if (host is GenericShip) Host = host as GenericShip;
        }

        public override void RemoveFromHost(Action callback)
        {
            if (HostTargetable is GenericShip)
            {
                base.RemoveFromHost(callback);
            }
            else if (HostTargetable is GenericObstacle)
            {
                (HostTargetable as GenericObstacle).Tokens.Remove(this);
                callback();
            }
        }
    }

}
