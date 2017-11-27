using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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

                PilotAbilities.Add(new PilotAbilitiesNamespace.KananJarrusPilotAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class KananJarrusPilotAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            GenericShip.OnAttackStartAsAttackerGlobal += CheckPilotAbility;
        }

        private void CheckPilotAbility()
        {
            bool IsDifferentPlayer = (Host.Owner.PlayerNo != Combat.Attacker.Owner.PlayerNo);
            bool HasFocusTokens = Host.HasToken(typeof(Tokens.FocusToken));
            Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Host, Combat.Attacker);

            if (IsDifferentPlayer && HasFocusTokens && distanceInfo.Range < 3)
            {
                Debug.Log("Ability is registered");
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        private void AskDecreaseAttack(object sender, System.EventArgs e)
        {
            Debug.Log("Ask decision");
            AskToUseAbility(AlwaysUseByDefault, DecreaseAttack, null, true);
        }

        private void DecreaseAttack(object sender, System.EventArgs e)
        {
            Host.SpendToken(typeof(Tokens.FocusToken), RegisterDecreaseNumberOfAttackDice);
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
