using ActionsList;
using Actions;
using Arcs;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class AttackShuttle : FirstEdition.AttackShuttle.AttackShuttle
        {
            public AttackShuttle() : base()
            {
                ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.FullFront, 3);
                ShipInfo.Hull = 3;
                ShipInfo.Shields = 1;

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)));

                IconicPilots[Faction.Rebel] = typeof(HeraSyndulla);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/46/Maneuver_attack_shuttle.png";
            }
        }
    }
}
