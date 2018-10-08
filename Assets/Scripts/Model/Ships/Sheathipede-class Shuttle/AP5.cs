﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Tokens;
using System;
using RuleSets;
using ActionsList;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class AP5 : SheathipedeShuttle, ISecondEditionPilot
        {
            public AP5() : base()
            {
                PilotName = "AP-5";
                PilotSkill = 1;
                Cost = 15;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.AP5PilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 30;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                ActionBar.RemovePrintedAction(typeof(FocusAction));
                ActionBar.AddPrintedAction(new CalculateAction());

                PilotAbilities.RemoveAll(a => a is Abilities.AP5PilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.AP5PilotAbilitySE());

                SEImageNumber = 41;
            }
        }
    }
}

namespace Abilities
{
    public class AP5PilotAbility : GenericAbility
    {
        private class TwoShipsArguments: System.EventArgs
        {
            public GenericShip Host;
            public GenericShip Target;
        }

        public override void ActivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected += RegisterAP5PilotAbilty;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected -= RegisterAP5PilotAbilty;
        }

        private void RegisterAP5PilotAbilty(GenericShip targetShip)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "AP-5's effect",
                TriggerType = TriggerTypes.OnCoordinateTargetIsSelected,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = AskAP5PilotAbility,
                EventArgs = new TwoShipsArguments()
                {
                    Host = this.HostShip,
                    Target = targetShip
                }
            });
        }

        private void AskAP5PilotAbility(object sender, System.EventArgs e)
        {
            TwoShipsArguments twoShipsArguments = (TwoShipsArguments) e;

            AskToUseAbility(
                delegate { return IsUseAP5PilotAbility(twoShipsArguments.Target); },
                delegate { UsePilotAbility(twoShipsArguments); }
            );
            
        }

        private bool IsUseAP5PilotAbility(GenericShip targetShip)
        {
            bool result = false;
            StressToken stressTokens = (StressToken)targetShip.Tokens.GetToken(typeof(StressToken));

            if (stressTokens != null)
            {
                result = true;
            }

            return result;
        }

        private void UsePilotAbility(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Host.Tokens.AssignToken(typeof(StressToken), delegate { AssignSecondStressToken(twoShipsArguments); });
        }

        private void AssignSecondStressToken(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Host.Tokens.AssignToken(typeof(StressToken), delegate { RemoveStressTokenFromTarget(twoShipsArguments); });
        }

        private void RemoveStressTokenFromTarget(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Target.Tokens.RemoveToken(
                typeof(StressToken),
                SubPhases.DecisionSubPhase.ConfirmDecision
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AP5PilotAbilitySE : GenericAbility
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
            if (ship.Tokens.CountTokensByType(typeof(StressToken)) == 1 && !ship.CanPerformActionsWhileStressed)
            {
                SelectedShip = ship;
                ship.CanPerformActionsWhileStressed = true;
                SelectedShip.OnActionIsPerformed += RemoveEffect;
            }
        }

        private void RemoveEffect(GenericAction action)
        {
            SelectedShip.OnActionIsPerformed -= RemoveEffect;
            SelectedShip.CanPerformActionsWhileStressed = false;
            SelectedShip = null;
        }
    }
}
