using Arcs;
using Movement;
using Tokens;
using Upgrade;

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
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ee/88/ee888bad-fa08-42e2-a558-cbf9a6f2da62/swz67_rampage.png";
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