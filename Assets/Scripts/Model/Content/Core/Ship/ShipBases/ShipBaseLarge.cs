using BoardTools;
using Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public class ShipBaseLarge : GenericShipBase
    {

        public ShipBaseLarge(GenericShip host) : base(host)
        {
            Size = BaseSize.Large;
            PrefabPath = "Prefabs/ShipModel/ShipBase/ShipBaseLarge";
            TemporaryPrefabPath = "Prefabs/ShipModel/ShipBase/TemporaryShipBaseLarge";

            HALF_OF_SHIPSTAND_SIZE = 1f;
            SHIPSTAND_SIZE = 2f;
            SHIPSTAND_SIZE_CM = 8f;

            HALF_OF_FIRINGARC_SIZE = 0.425f * SHIPSTAND_SIZE;

            CreateShipBase();
        }

        public override List<ManeuverTemplate> BoostTemplatesAvailable => new List<ManeuverTemplate>() {
            new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1),
            new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed1),
            new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed1)
        };

        public override List<ManeuverTemplate> BarrelRollTemplatesAvailable => new List<ManeuverTemplate>() {
            new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1, isSideTemplate:true)
        };

        public override List<ManeuverTemplate> DecloakBoostTemplatesAvailable => new List<ManeuverTemplate>() {
            new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1)
        };

        public override List<ManeuverTemplate> DecloakBarrelRollTemplatesAvailable => new List<ManeuverTemplate>() {
            new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1, isSideTemplate:true)
        };

    }
}
