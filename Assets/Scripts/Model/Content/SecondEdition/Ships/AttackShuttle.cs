using ActionsList;
using Actions;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class AttackShuttle : FirstEdition.AttackShuttle.AttackShuttle
        {
            public AttackShuttle() : base()
            {
                ShipInfo.Hull = 3;
                ShipInfo.Shields = 1;

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)));

                IconicPilots[Faction.Rebel] = typeof(HeraSyndulla);

                //TODO: ManeuversImageUrl
            }
        }
    }
}
