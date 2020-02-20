using Actions;
using ActionsList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class AP5 : SheathipedeClassShuttle
        {
            public AP5() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "AP-5",
                    1,
                    32,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AP5PilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
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
