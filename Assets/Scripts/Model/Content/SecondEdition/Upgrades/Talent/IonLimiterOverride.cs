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
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.IonLimiterOverrideAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a5/d9/a5d987e9-ec4c-4a99-828b-c498799a7d6c/swz67_ion-limiter-overdrive.png";
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