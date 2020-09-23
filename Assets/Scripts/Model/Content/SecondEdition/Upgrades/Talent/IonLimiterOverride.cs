using Upgrade;
using ActionsList;
using Tokens;
using Ship;
using Movement;
using Actions;
using System;

namespace UpgradesList.SecondEdition
{
    public class IonLimiterOverride : GenericUpgrade
    {
        public IonLimiterOverride() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Ion Limiter Override",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.IonLimiterOverrideAbility)
            );

            ImageUrl = "https://i.imgur.com/hZ329OH.png";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIE;
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
                name: "",
                description: "",
                HostUpgrade
            ),
            actionInfo: new Parameters.ActionInfo
            (
                actionType: typeof(BarrelRollAction),
                actionColor: ActionColor.Red,
                canBePerformedWhileStressed: true
            ),
            afterAction: new RollDiceAction
            (
                diceType: DiceKind.Attack,
                onHit: new AssignTokenAction
                (
                    tokenType: typeof(StrainToken),
                    targetShip: GetThisShip
                ),
                onCrit: new AssignTokenAction
                (
                    tokenType: typeof(IonToken),
                    targetShip: GetThisShip
                )
            )
        );

        private GenericShip GetThisShip()
        {
            return HostShip;
        }
    }
}