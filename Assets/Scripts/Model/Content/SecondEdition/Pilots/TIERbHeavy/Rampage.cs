using Arcs;
using Movement;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIERbHeavy
    {
        public class Rampage : TIERbHeavy
        {
            public Rampage() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Rampage\"",
                    4,
                    42,
                    abilityType: typeof(Abilities.SecondEdition.RampagePilotAbility),
                    isLimited: true
                );

                ImageUrl = "https://i.imgur.com/cPVLRxm.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RampagePilotAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterManeuver
        (
            minSpeed: ManeuverSpeed.Speed3,
            maxSpeed: ManeuverSpeed.Speed4
        );

        public override AbilityPart Action => new SelectShipAction
        (
            name: "\"Rampage\"",
            description: "Choose a ship to assign it 1 strain token (2 if you are damaged)",
            imageSource: HostShip,
            filter: new SelectShipFilter
            (
                minRange: 0,
                maxRange: 1,
                inArcType: ArcType.SingleTurret
            ),
            action: new AssignTokenOnTargetAction
            (
                tokenType: typeof(StrainToken),
                getCount: GetCountOfStrainTokens
            ),
            aiPriority: AiSelectShipPriority.Enemy
        );

        public int GetCountOfStrainTokens()
        {
            return HostShip.Damage.IsDamaged ? 2 : 1;
        }
    }
}