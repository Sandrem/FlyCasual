using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace Ship
{
    namespace Vcx100
    {
        public class KananJarrus : Vcx100
        {
            public KananJarrus() : base()
            {
                PilotName = "Kanan Jarrus";
                PilotSkill = 5;
                Cost = 38;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.KananJarrusPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class KananJarrusPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckPilotAbility;
        }

        private void CheckPilotAbility()
        {
            bool IsDifferentPlayer = (HostShip.Owner.PlayerNo != Combat.Attacker.Owner.PlayerNo);
            bool HasFocusTokens = HostShip.HasToken(typeof(Tokens.FocusToken));
            Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(HostShip, Combat.Attacker);

            if (IsDifferentPlayer && HasFocusTokens && distanceInfo.Range < 3)
            {
                Debug.Log("Ability is registered");
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        private void AskDecreaseAttack(object sender, System.EventArgs e)
        {
            Debug.Log("Ask decision");
            AskToUseAbility(AlwaysUseByDefault, DecreaseAttack, null, null, true);
        }

        private void DecreaseAttack(object sender, System.EventArgs e)
        {
            HostShip.SpendToken(typeof(Tokens.FocusToken), RegisterDecreaseNumberOfAttackDice);
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void RegisterDecreaseNumberOfAttackDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += DecreaseNumberOfAttackDice;
        }

        private void DecreaseNumberOfAttackDice(ref int diceCount)
        {
            diceCount--;
            Combat.Attacker.AfterGotNumberOfAttackDice -= DecreaseNumberOfAttackDice;
        }
    }
}
