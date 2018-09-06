using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;
using BoardTools;
using SubPhases;

namespace Ship
{
    namespace XWing
    {
        public class BiggsDarklighter : XWing, ISecondEditionPilot
        {
            public BiggsDarklighter() : base()
            {
                PilotName = "Biggs Darklighter";
                PilotSkill = 5;
                Cost = 25;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.BiggsDarklighterAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 48;

                PilotAbilities.RemoveAll(ability => ability is Abilities.BiggsDarklighterAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.BiggsDarklighterAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class BiggsDarklighterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAskBiggsAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAskBiggsAbility;
        }

        private void RegisterAskBiggsAbility()
        {
            if (!IsAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, ActivateBiggsAbility);
        }

        private void ActivateBiggsAbility(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.Tokens.AssignCondition(typeof(Conditions.BiggsDarklighterCondition));

            GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;

            HostShip.OnShipIsDestroyed += RemoveBiggsDarklighterAbility;
            Phases.Events.OnCombatPhaseEnd_NoTriggers += RemoveBiggsDarklighterAbility;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            bool shipIsProtected = false;
            if (Selection.AnotherShip.ShipId != HostShip.ShipId)
            {
                if (Selection.AnotherShip.Owner.PlayerNo == HostShip.Owner.PlayerNo)
                {
                    BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(Selection.AnotherShip, HostShip);
                    if (positionInfo.Range <= 1)
                    {
                        if (!Selection.ThisShip.ShipsBumped.Contains(HostShip))
                        {
                            if (Combat.ChosenWeapon.IsShotAvailable(HostShip)) shipIsProtected = true;
                        }
                    }
                }
            }

            if (shipIsProtected)
            {
                if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                {
                    stringList.Add("Biggs DarkLighter: You cannot attack target ship");
                }
                result = false;
            }
        }

        private void RemoveBiggsDarklighterAbility(GenericShip ship, bool isFled)
        {
            RemoveBiggsDarklighterAbility();
        }

        private void RemoveBiggsDarklighterAbility(object sender, System.EventArgs e)
        {
            RemoveBiggsDarklighterAbility();
        }

        private void RemoveBiggsDarklighterAbility()
        {
            HostShip.Tokens.RemoveCondition(typeof(Conditions.BiggsDarklighterCondition));

            GenericShip.OnTryPerformAttackGlobal -= CanPerformAttack;

            HostShip.OnShipIsDestroyed -= RemoveBiggsDarklighterAbility;
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= RemoveBiggsDarklighterAbility;

            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAskBiggsAbility;
        }
    }
}

namespace Conditions
{
    public class BiggsDarklighterCondition : Tokens.GenericToken
    {
        public BiggsDarklighterCondition(GenericShip host) : base(host)
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new Ship.XWing.BiggsDarklighter().ImageUrl;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BiggsDarklighterAbilitySE : GenericAbility
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

            if (HostShip.Hull > 1)
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
