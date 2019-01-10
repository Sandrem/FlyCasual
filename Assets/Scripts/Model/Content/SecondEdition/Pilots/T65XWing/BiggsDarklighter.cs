using System.Collections;
using System.Collections.Generic;
using Ship;
using SubPhases;
using BoardTools;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class BiggsDarklighter : T65XWing
        {
            public BiggsDarklighter() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Biggs Darklighter",
                    3,
                    48,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BiggsDarklighterAbility),
                    seImageNumber: 7
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BiggsDarklighterAbility : GenericAbility
    {
        private GenericShip curToDamage;
        private DamageSourceEventArgs curDamageInfo;

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckDrawTheirFireAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckDrawTheirFireAbility;
        }

        private void CheckDrawTheirFireAbility(GenericShip ship, DamageSourceEventArgs e)
        {
            curToDamage = ship;
            curDamageInfo = e;

            if (AbilityCanBeUsed())
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, StartQuestionSubphase);
        }

        protected virtual bool AbilityCanBeUsed()
        {
            // Is the damage type a ship attack?
            if (curDamageInfo.DamageType != DamageTypes.ShipAttack)
                return false;

            // Is the defender on our team and not us? If not return.
            if (curToDamage.Owner.PlayerNo != HostShip.Owner.PlayerNo || curToDamage.ShipId == HostShip.ShipId)
                return false;

            // Is the defender at range 1 and is there a hit/crit result?
            if (!Board.IsShipAtRange(HostShip, curToDamage, 1) || curToDamage.AssignedDamageDiceroll.Successes < 1)
                return false;

            return true;
        }

        private void PreventDamage(DieSide type)
        {
            // Find a crit and remove it from the ship we're protecting's assigned damage.
            Die dieToRemove = curToDamage.AssignedDamageDiceroll.DiceList.Find(n => n.Side == type);
            curToDamage.AssignedDamageDiceroll.DiceList.Remove(dieToRemove);

            DamageSourceEventArgs drawtheirfireDamage = new DamageSourceEventArgs()
            {
                Source = "Biggs Darklighter's Ability",
                DamageType = DamageTypes.CardAbility
            };

            int hits = type == DieSide.Success ? 1 : 0;
            int crits = type == DieSide.Crit ? 1 : 0;
            HostShip.Damage.TryResolveDamage(hits, crits, drawtheirfireDamage, DecisionSubPhase.ConfirmDecision);
        }

        protected class BiggsDarklighterDecisionSubPhase : DecisionSubPhase { }

        protected virtual void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            BiggsDarklighterDecisionSubPhase selectBiggsDarklighterSubPhase = (BiggsDarklighterDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(BiggsDarklighterDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectBiggsDarklighterSubPhase.InfoText = "Use " + Name + "?";

            if (curToDamage.AssignedDamageDiceroll.RegularSuccesses > 0)
            {
                selectBiggsDarklighterSubPhase.AddDecision("Suffer one damage to cancel one hit.", delegate { PreventDamage(DieSide.Success); });
                selectBiggsDarklighterSubPhase.AddTooltip("Suffer one damage to cancel one hit.", HostShip.ImageUrl);
            }

            if (curToDamage.AssignedDamageDiceroll.CriticalSuccesses > 0)
            {
                selectBiggsDarklighterSubPhase.AddDecision("Suffer one critical damage to cancel one crit.", delegate { PreventDamage(DieSide.Crit); });
                selectBiggsDarklighterSubPhase.AddTooltip("Suffer one critical damage to cancel one crit.", HostShip.ImageUrl);
            }

            selectBiggsDarklighterSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });
            selectBiggsDarklighterSubPhase.DefaultDecisionName = GetDefaultDecision();
            selectBiggsDarklighterSubPhase.ShowSkipButton = true;
            selectBiggsDarklighterSubPhase.DecisionOwner = HostShip.Owner;
            selectBiggsDarklighterSubPhase.Start();
        }

        protected string GetDefaultDecision()
        {
            string result = "No";

            if (HostShip.State.HullCurrent > 1)
            {
                if (curToDamage.AssignedDamageDiceroll.RegularSuccesses > 0)
                {
                    result = "Suffer one damage to cancel one hit.";
                }

                if (curToDamage.AssignedDamageDiceroll.CriticalSuccesses > 0)
                {
                    result = "Suffer one critical damage to cancel one crit.";
                }

            }

            return result;
        }

    }
}
