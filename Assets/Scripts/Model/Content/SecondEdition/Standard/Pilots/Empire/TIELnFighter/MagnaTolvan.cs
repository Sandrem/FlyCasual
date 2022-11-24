using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class MagnaTolvan : TIELnFighter
        {
            public MagnaTolvan() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Magna Tolvan",
                    "Cold Tyrant",
                    Faction.Imperial,
                    3,
                    3,
                    9,
                    isLimited: true,
                    abilityType: typeof(MagnaTolvanAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/KXRxwN1.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MagnaTolvanAbility : GenericAbility
    {
        private GenericShip PreviousActiveShip;

        public override void ActivateAbility()
        {
            HostShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            HostShip.OnCanPerformActionWhileStressed += CheckTwoOrFewerStress;

            HostShip.OnTokenIsAssigned += TryRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanPerformActionWhileStressed -= CheckTwoOrFewerStress;
            HostShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;

            HostShip.OnTokenIsAssigned -= TryRegisterAbility;
        }

        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            isAllowed = (HostShip.Tokens.CountTokensByType<Tokens.StressToken>() <= 2);
        }

        private void CheckTwoOrFewerStress(GenericAction action, ref bool isAllowed)
        {
            isAllowed = (HostShip.Tokens.CountTokensByType<Tokens.StressToken>() <= 2 && action.Color == Actions.ActionColor.White);
        }

        private void TryRegisterAbility(GenericShip ship, GenericToken token)
        {
            if (token is StressToken)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, UseFreeActionAbility);
            }
        }

        private void UseFreeActionAbility(object sender, EventArgs e)
        {
            PreviousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);

            List<GenericAction> whiteActions = HostShip.GetAvailableActionsWhiteOnly();

            HostShip.AskPerformFreeAction
            (
                whiteActions,
                FinishAbility,
                HostShip.PilotInfo.PilotName,
                "You may perform an action",
                HostShip
            );
        }

        private void FinishAbility()
        {
            Selection.ThisShip = PreviousActiveShip;
            Triggers.FinishTrigger();
        }
    }
}