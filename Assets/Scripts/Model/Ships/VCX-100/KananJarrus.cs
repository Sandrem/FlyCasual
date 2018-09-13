using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using BoardTools;
using RuleSets;

namespace Ship
{
    namespace Vcx100
    {
        public class KananJarrus : Vcx100, ISecondEditionPilot
        {
            public KananJarrus() : base()
            {
                PilotName = "Kanan Jarrus";
                PilotSkill = 5;
                Cost = 38;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.KananJarrusPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 90;
                MaxForce = 2;

                PilotAbilities.RemoveAll(ability => ability is Abilities.KananJarrusPilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.KananJarrusAbilitySE());

                SEImageNumber = 74;
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

        protected virtual void CheckPilotAbility()
        {
            bool IsDifferentPlayer = (HostShip.Owner.PlayerNo != Combat.Attacker.Owner.PlayerNo);
            bool HasFocusTokens = HostShip.Tokens.HasToken(typeof(Tokens.FocusToken));
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, Combat.Attacker);

            if (IsDifferentPlayer && HasFocusTokens && distanceInfo.Range < 3)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        protected void AskDecreaseAttack(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, DecreaseAttack, null, null, false);
        }

        protected virtual void DecreaseAttack(object sender, System.EventArgs e)
        {
            HostShip.Tokens.SpendToken(typeof(Tokens.FocusToken), RegisterDecreaseNumberOfAttackDice);
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        protected void RegisterDecreaseNumberOfAttackDice()
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

namespace Abilities.SecondEdition
{
    public class KananJarrusAbilitySE : KananJarrusPilotAbility
    {
        protected override void CheckPilotAbility()
        {
            bool enemy = HostShip.Owner.PlayerNo != Combat.Attacker.Owner.PlayerNo;
            bool hasForceTokens = HostShip.Force > 0;
            bool inArc = Board.IsShipInArc(HostShip, Combat.Attacker);

            if (enemy && hasForceTokens && inArc)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        protected override void DecreaseAttack(object sender, System.EventArgs e)
        {
            HostShip.Force--;
            RegisterDecreaseNumberOfAttackDice();
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}