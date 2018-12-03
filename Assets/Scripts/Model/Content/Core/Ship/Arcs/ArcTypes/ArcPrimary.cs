using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcPrimary : GenericArc
    {
        public ArcPrimary(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcType.Front;
            Facing = ArcFacing.Forward;

            Limits = new Dictionary<Vector3, float>()
            {
                { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0), -40f },
                { new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),  40f }
            };

            Edges = new List<Vector3>()
            {
                new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
                new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
            };

            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
                CanShootCannon = true,
                CanShootMissiles = true,
                CanShootTorpedoes = true,
                CanShootTurret = true
            };
        }
    }
}
