using Abilities.Parameters;
using Arcs;
using Content;
using Movement;
using System.Collections.Generic;
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
                PilotInfo = new PilotCardInfo25
                (
                    "\"Rampage\"",
                    "Implacable Pursuer",
                    Faction.Imperial,
                    4,
                    5,
                    16,
                    abilityType: typeof(Abilities.SecondEdition.RampagePilotAbility),
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
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
            abilityDescription: new AbilityDescription
            (
                name: "\"Rampage\"",
                description: "Choose a ship to assign it 1 strain token (2 if you are damaged)",
                imageSource: HostShip
            ),
            conditions: new ConditionsBlock
            (
                new RangeToHostCondition(0, 1),
                new InArcCondition(ArcType.SingleTurret)
            ),
            action: new AssignTokenAction
            (
                tokenType: typeof(StrainToken),
                targetShipRole: ShipRole.TargetShip,
                getCount: GetCountOfStrainTokens,
                showMessage: GetMessageToShow
            ),
            aiSelectShipPlan: new AiSelectShipPlan
            (
                aiSelectShipTeamPriority: AiSelectShipTeamPriority.Enemy,
                aiSelectShipSpecial: AiSelectShipSpecial.Agile
            )
        );

        private string GetMessageToShow()
        {
            string message = "\"Rampage\": ";
            if (GetCountOfStrainTokens() == 1)
            {
                message += "Strain token is assigned to " + TargetShip.PilotInfo.PilotName;
            }
            else
            {
                message += "2 strain tokens are assigned to " + TargetShip.PilotInfo.PilotName;
            }
            return message;
        }

        public int GetCountOfStrainTokens()
        {
            return HostShip.Damage.IsDamaged ? 2 : 1;
        }
    }
}