using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcBullseye : GenericArc
    {
        public ArcBullseye(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.Bullseye;
            Facing = ArcFacing.Bullseye;

            Limits = new Dictionary<Vector3, float>()
            {
                { new Vector3(-shipBase.HALF_OF_BULLSEYEARC_SIZE, 0, 0), 0f },
                { new Vector3( shipBase.HALF_OF_BULLSEYEARC_SIZE,  0, 0), 0f }
            };

            Edges = new List<Vector3>()
            {
                new Vector3(-shipBase.HALF_OF_BULLSEYEARC_SIZE, 0, 0),
                new Vector3( shipBase.HALF_OF_BULLSEYEARC_SIZE, 0, 0),
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
