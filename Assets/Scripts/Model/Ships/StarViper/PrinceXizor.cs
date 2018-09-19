using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ship;
using RuleSets;
using SubPhases;
using Arcs;
using BoardTools;

namespace Ship
{
    namespace StarViper
    {
        public class PrinceXizor : StarViper, ISecondEditionPilot
        {
            public PrinceXizor() : base()
            {
                PilotName = "Prince Xizor";
                PilotSkill = 4;
                Cost = 54;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.PrinceXizorAbility());

                SEImageNumber = 180;
            }

            public void AdaptPilotToSecondEdition()
            {

            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PrinceXizorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAfterNeutralizeResults += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAfterNeutralizeResults -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (Combat.DiceRollAttack.Successes > 0 && IsAnyRedirectTargetPresent())
            {
                RegisterAbilityTrigger(TriggerTypes.OnAfterNeutralizeResults, AskToRedirect);
            }
        }

        private bool IsAnyRedirectTargetPresent()
        {
            bool result = false;

            foreach (GenericShip ship in HostShip.Owner.Ships.Values)
            {
                if (IsPossibleRedirectTarget(ship)) return true;
            }

            return result;
        }

        private bool IsPossibleRedirectTarget(GenericShip ship)
        {
            bool result = false;

            if (ship.ShipId == HostShip.ShipId) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range > 1) return false;

            foreach (GenericArc arc in Combat.Attacker.ArcInfo.Arcs)
            {
                ShotInfoArc shotInfoArcDefender = new ShotInfoArc(Combat.Attacker, HostShip, arc);
                if (shotInfoArcDefender.InArc && shotInfoArcDefender.IsShotAvailable)
                {
                    ShotInfoArc shotInfoArcRedirect = new ShotInfoArc(Combat.Attacker, ship, arc);
                    if (shotInfoArcRedirect.InArc && shotInfoArcDefender.IsShotAvailable) return true;
                }
            }

            return result;
        }

        private void AskToRedirect(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                RedirectTargetIsSelected,
                IsPossibleRedirectTarget,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Select another friendly ship to redirect one damage result",
                HostShip.ImageUrl
            );
        }

        private void RedirectTargetIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            if (Combat.DiceRollAttack.CriticalSuccesses == 0) // && Combat.DiceRollAttack.RegularSuccesses == 0
            {
                DealDamageToTargetShip(DieSide.Success);
            }
            else if (Combat.DiceRollAttack.RegularSuccesses == 0) // && Combat.DiceRollAttack.CriticalSuccesses > 0
            {
                DealDamageToTargetShip(DieSide.Crit);
            }
            else // Combat.DiceRollAttack.CriticalSuccesses > 0 && && Combat.DiceRollAttack.RegularSuccesses > 0
            {
                StartHitCritDecisionSubphase();
            }
        }

        private void StartHitCritDecisionSubphase()
        {
            var subphase = Phases.StartTemporarySubPhaseNew<HitOrCritDecisionSubphase>("Prince Xizor", Triggers.FinishTrigger);

            subphase.InfoText = "Suffer Hit or Crit result?";

            subphase.AddDecision(
                "Hit",
                delegate {
                    DecisionSubPhase.ConfirmDecisionNoCallback();
                    DealDamageToTargetShip(DieSide.Success);
                }
            );

            subphase.AddDecision(
                "Crit",
                delegate {
                    DecisionSubPhase.ConfirmDecisionNoCallback();
                    DealDamageToTargetShip(DieSide.Crit);
                }
            );

            subphase.DefaultDecisionName = "Crit";

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = false;

            subphase.Start();
        }

        private void DealDamageToTargetShip(DieSide dieSide)
        {
            Combat.DiceRollAttack.DiceList.Remove(Combat.DiceRollAttack.DiceList.First(d => d.Side == dieSide));

            int regularDamage = 0;
            int criticalDamage = 0;
            if (dieSide == DieSide.Success)
            {
                regularDamage = 1;
            }
            else if (dieSide == DieSide.Crit)
            {
                criticalDamage = 1;
            }

            DamageSourceEventArgs redirectedDamageArgs = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.CardAbility
            };

            TargetShip.Damage.TryResolveDamage(regularDamage, redirectedDamageArgs, Triggers.FinishTrigger, criticalDamage);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;

            priority = 100 - ship.Cost;

            priority += (ship.Hull + ship.Shields);

            if (ship.Hull + ship.Shields == 1) priority -= 50;

            return priority;
        }

        public class HitOrCritDecisionSubphase : DecisionSubPhase { }
    }
}

