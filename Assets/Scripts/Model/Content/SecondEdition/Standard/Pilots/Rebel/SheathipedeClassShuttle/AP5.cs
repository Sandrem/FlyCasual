using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class AP5 : SheathipedeClassShuttle
        {
            public AP5() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "AP-5",
                    "Escaped Analyst Droid",
                    Faction.Rebel,
                    1,
                    4,
                    4,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AP5PilotAbility),
                    tags: new List<Tags>
                    {
                        Tags.Spectre,
                        Tags.Droid
                    },
                    seImageNumber: 41
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AP5PilotAbility : GenericAbility
    {
        private GenericShip SelectedShip;

        public override void ActivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected += CheckUseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected -= CheckUseAbility;
        }

        private void CheckUseAbility(GenericShip ship)
        {
            SelectedShip = ship;

            SelectedShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            SelectedShip.OnCanPerformActionWhileStressed += AllowIfOneOrLessStressTokens;

            SelectedShip.OnActionIsPerformed += RemoveEffect;
        }

        private void RemoveEffect(GenericAction action)
        {
            SelectedShip.OnActionIsPerformed -= RemoveEffect;

            SelectedShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;
            SelectedShip.OnCanPerformActionWhileStressed -= AllowIfOneOrLessStressTokens;

            SelectedShip = null;
        }

        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            AllowIfOneOrLessStressTokens(null, ref isAllowed);
        }

        private void AllowIfOneOrLessStressTokens(GenericAction action, ref bool isAllowed)
        {
            isAllowed = SelectedShip.Tokens.CountTokensByType(typeof(StressToken)) <= 1;
        }
    }
}
