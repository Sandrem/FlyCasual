using Arcs;
using Ship;
using SubPhases;
using System;
using UnityEngine;

namespace Abilities
{
    public class RollDiceAction : AbilityPart
    {
        private GenericAbility Ability;

        public DiceKind DiceType { get; }
        public AbilityPart OnCrit { get; }
        public AbilityPart OnHit { get; }

        public RollDiceAction(DiceKind diceType, AbilityPart onCrit, AbilityPart onHit)
        {
            DiceType = diceType;
            OnCrit = onCrit;
            OnHit = onHit;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            AbilityDiceRollCheckSubphase subphase = Phases.StartTemporarySubPhaseNew<AbilityDiceRollCheckSubphase>("Ability Check", Triggers.FinishTrigger);

            subphase.AfterRoll = CheckResults;
            subphase.DiceKind = DiceType;
            subphase.DiceCount = 1;

            subphase.RequiredPlayer = Ability.HostShip.Owner.PlayerNo;
            subphase.Start();            
        }

        private void CheckResults()
        {
            (Phases.CurrentSubPhase as DiceRollCheckSubPhase).HideDiceResultMenu();

            switch ((Phases.CurrentSubPhase as AbilityDiceRollCheckSubphase).CurrentDiceRoll.ResultsArray[0])
            {
                case DieSide.Crit:
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    OnCrit.DoAction(Ability);
                    break;
                case DieSide.Success:
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    OnHit.DoAction(Ability);
                    break;
                default:
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    Triggers.FinishTrigger();
                    break;
            }
        }

        private class AbilityDiceRollCheckSubphase : DiceRollCheckSubPhase { }
    }
}
