using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardTools;
using Movement;
using UnityEngine;

namespace Ship
{
    public class ShipBaseSmall : GenericShipBase
    {

        public ShipBaseSmall(GenericShip host) : base(host)
        {
            Size = BaseSize.Small;
            PrefabPath = "Prefabs/ShipModel/ShipBase/ShipBaseSmall";
            TemporaryPrefabPath = "Prefabs/ShipModel/ShipBase/TemporaryShipBaseSmall";

            HALF_OF_SHIPSTAND_SIZE = 0.5f;
            SHIPSTAND_SIZE = 1f;
            SHIPSTAND_SIZE_CM = 4f;

            HALF_OF_FIRINGARC_SIZE = 0.425f;

            CreateShipBase();
        }

        public override List<ManeuverTemplate> BoostTemplatesAvailable => new List<ManeuverTemplate>() {
            new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1),
            new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed1),
            new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed1)
        };

        public override List<ManeuverTemplate> BarrelRollTemplatesAvailable => new List<ManeuverTemplate>() {
            new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1)
        };

        public override List<ManeuverTemplate> DecloakBoostTemplatesAvailable => new List<ManeuverTemplate>() {
            new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed2)
        };

        public override List<ManeuverTemplate> DecloakBarrelRollTemplatesAvailable => new List<ManeuverTemplate>() {
            new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed2)
        };
    }
}
