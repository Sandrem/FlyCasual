using Upgrade;
using ActionsList;
using Actions;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using System;

namespace UpgradesList.SecondEdition
{
    public class RookKast : GenericUpgrade
    {
        public RookKast() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Rook Kast",
                UpgradeType.Crew,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                addAction: new ActionInfo(typeof(TargetLockAction), ActionColor.Red),
                abilityType: typeof(Abilities.SecondEdition.RookKastCrewAbility)
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/rookkast.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class RookKastCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddRookKastPilotAbility;
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddRookKastPilotAbility;
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void AddRookKastPilotAbility(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == ship.ShipId
                && Combat.Attacker.IsStrained
                && (Combat.DiceRollAttack.Focuses > 0 || Combat.DiceRollAttack.Blanks > 0))
            {
                ship.AddAvailableDiceModificationOwn(new RookKastAction
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    HostShip = HostShip
                });
            }
        }
        protected void CheckConditions(GenericAction action)
        {
            if (action.IsRed)
            {
                HostShip.OnActionDecisionSubphaseEnd += RegisterActionTrigger;
            }
        }

        private void RegisterActionTrigger(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= RegisterActionTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskToUseOwnAbility);
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                ActivateOwnAbility,
                descriptionLong: "Do you want to gain 1 strain token?",
                imageHolder: HostUpgrade
            );
        }

        private void ActivateOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
        }


        private class RookKastAction : ActionsList.GenericAction
        {
            public RookKastAction()
            {
                Name = DiceModificationName = "Rook Kast's ability";

                IsTurnsOneFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                if (Combat.CurrentDiceRoll.Blanks > 0)
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
                }
                else if (Combat.CurrentDiceRoll.Focuses > 0)
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                }
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                if (Combat.AttackStep == CombatStep.Attack &&
                    Combat.Attacker.Tokens.HasToken(typeof(Tokens.StrainToken)))
                {
                    result = true;
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    int attackBlanks = Combat.DiceRollAttack.Blanks;
                    if (attackBlanks > 0)
                    {
                        if ((attackBlanks == 1) && (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0))
                        {
                            result = 100;
                        }
                        else
                        {
                            result = 55;
                        }
                    }
                }

                return result;
            }
        }
    }
}