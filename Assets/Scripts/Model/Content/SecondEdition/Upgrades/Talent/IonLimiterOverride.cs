using Upgrade;
using ActionsList;
using Tokens;
using Ship;
using Movement;
using System;

namespace UpgradesList.SecondEdition
{
    public class IonLimiterOverride : GenericUpgrade
    {
        public IonLimiterOverride() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Limiter Override",
                UpgradeType.Talent,
                cost: 2,
                restriction: new TagRestriction(Content.Tags.Tie),
                abilityType: typeof(Abilities.SecondEdition.IonLimiterOverrideAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a5/d9/a5d987e9-ec4c-4a99-828b-c498799a7d6c/swz67_ion-limiter-overdrive.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IonLimiterOverrideAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterManeuver
        (
            complexity: MovementComplexity.Complex,
            onlyIfFullyExecuted: true
        );

        public override AbilityPart Action => new AskToPerformAction
        (
            description: new Parameters.AbilityDescription
            (
                name: HostUpgrade.UpgradeInfo.Name,
                description: "You may perform a Barrel Roll action, even while stressed. If you do, roll an attack die; on a Hit result gain 1 strain token, and on a Critical Hit result gain 1 ion token",
                HostUpgrade
            ),
            actionInfo: new Parameters.ActionInfo
            (
                actionType: typeof(BarrelRollAction),
                canBePerformedWhileStressed: true
            ),
            afterAction: new RollDiceAction
            (
                diceType: DiceKind.Attack,
                onHit: new AssignTokenAction
                (
                    tokenType: typeof(StrainToken),
                    targetShipRole: ShipRole.HostShip,
                    showMessage: ShowStrainTokenMessage
                ),
                onCrit: new AssignTokenAction
                (
                    tokenType: typeof(IonToken),
                    targetShipRole: ShipRole.HostShip,
                    showMessage: ShowIonTokenMessage
                )
            )
        );

        private string ShowStrainTokenMessage()
        {
            return $"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} gains 1 Strain token";
        }

        private string ShowIonTokenMessage()
        {
            return $"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} gains 1 Ion token";
        }
    }
}