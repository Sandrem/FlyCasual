using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;
using Tokens;
using SubPhases;

namespace Ship
{
    namespace XWing
    {
        public class JekPorkins : XWing, ISecondEditionPilot
        {
            public JekPorkins() : base()
            {
                PilotName = "Jek Porkins";
                PilotSkill = 7;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.JekPorkinsAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 60;

                ImageUrl = "https://i.imgur.com/xoIkP6d.png";
            }
        }
    }
}

namespace Abilities
{
    public class JekPorkinsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += CheckAbilityConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned += CheckAbilityConditions;
        }

        private void CheckAbilityConditions(GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(StressToken)) RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskToUsePilotAbility);
        }

        private void AskToUsePilotAbility(object sender, EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, RemoveStressAndRollDice);
        }

        private void RemoveStressAndRollDice(object sender, EventArgs e)
        {
            HostShip.Tokens.RemoveToken(typeof(StressToken), StartRollDiceSubphase);
        }

        private void StartRollDiceSubphase()
        {
            Phases.CurrentSubPhase.Pause();

            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhaseOld(
                "Jek Porkins: Roll for damage",
                typeof(JekPorkinsCheckSubPhase),
                delegate {
                    Phases.GoBack(typeof(JekPorkinsCheckSubPhase));
                    DecisionSubPhase.ConfirmDecision();
                }
            );
        }
    }
}

namespace SubPhases
{

    public class JekPorkinsCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.RegularSuccesses > 0)
            {
                RegisterSufferHullDamage();
            }
            else
            {
                CallBack();
            }
        }

        private void RegisterSufferHullDamage()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from bomb",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                EventHandler = SufferHullDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = Selection.ActiveShip,
                    DamageType = DamageTypes.CardAbility
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CallBack);
        }

        private void SufferHullDamage(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman(string.Format("{0}: Facedown card is dealt", Selection.ActiveShip.PilotName));
            Selection.ActiveShip.SufferHullDamage(false, e);
        }
    }

}