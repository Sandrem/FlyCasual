using Abilities;
using Board;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Wingman : GenericUpgrade
    {
        public Wingman() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Wingman";
            Cost = 2;

            UpgradeAbilities.Add(new WingmanAbility());
        }
    }
}

namespace Abilities
{
    //At the start of the Combat phase, remove 1 stress token from another friendly ship at Range 1.
    public class WingmanAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, CheckTrigger);
        }

        private void CheckTrigger(object sender, EventArgs e)
        {
            List<GenericShip> shipsInRange = BoardManager.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Friendly);
            shipsInRange.Remove(HostShip);

            int count = 0;
            //If the ship doesn't have stress we shouldn't ask to remove a token.
            foreach (GenericShip ship in shipsInRange)
            {
                if (ship.Tokens.HasToken(typeof(StressToken)))
                {
                    count++;
                }
            }

            if (count <= 0)
            {
                Triggers.FinishTrigger();
                return;
            }

            AskToRemoveStress();
        }

        private void AskToRemoveStress()
        {
            Messages.ShowInfoToHuman("Wingman: Select a friendly ship that has a stress token");

            SelectTargetForAbility(RemoveFriendlyStress, new List<TargetTypes> { TargetTypes.OtherFriendly }, new Vector2(1, 1), Triggers.FinishTrigger);
        }

        private void RemoveFriendlyStress()
        {
            if (TargetShip.Tokens.HasToken(typeof(StressToken)))
            {
                //The ship with Wingman equipped is the ship that is removing the stress token. 
                //If a ship with Wingman removes a stress token from another 
                //friendly ship that is equipped with Kyle Katarn (crew), that ship is not assigned a focus token from Kyle's ability. 
                //(X-Wing FAQ, Version 4.2.3, Updated 10/22/2016)
                Messages.ShowInfoToHuman(string.Format("Stress token has been removed from {0}", TargetShip.PilotName));

                TargetShip.Tokens.RemoveToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
            }
            else
            {
                Messages.ShowErrorToHuman("Select a ship with a Stress token");
            }
        }
    }
}
