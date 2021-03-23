using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.TIEDDefender
{
    public class CaptainDobbs : TIEDDefender
    {
        public CaptainDobbs() : base()
        {
            PilotInfo = new PilotCardInfo(
                "Captain Dobbs",
                3,
                75,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CaptainDobbsAbility),
                extraUpgradeIcons: new List<UpgradeType>(){ UpgradeType.Talent, UpgradeType.Sensor },
                abilityText: "When another friendly ship defends, before the Neutralize Results step, if you are in the attack arc and you are not ionized, you may gain 1 ion token to cancel 1 hit or crit result"
            );

            ImageUrl = "https://i.imgur.com/RfgdAPL.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainDobbsAbility : GenericAbility
    {
        public override string Name => HostShip.PilotInfo.PilotName;

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

            if (HostShip.Tokens.CountTokensByType<IonToken>() > 0)
                return false;

            return true;
        }

        protected class CaptainDobbsDecisionSubPhase : DecisionSubPhase { }

        protected virtual void StartQuestionSubphase(object sender, EventArgs e)
        {
            CaptainDobbsDecisionSubPhase selectBiggsDarklighterSubPhase = (CaptainDobbsDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(CaptainDobbsDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectBiggsDarklighterSubPhase.DescriptionShort = Name;
            selectBiggsDarklighterSubPhase.DescriptionLong = "You may gain 1 Ion token to cancel 1 die result";
            selectBiggsDarklighterSubPhase.ImageSource = HostShip;

            if (curToDamage.AssignedDamageDiceroll.RegularSuccesses > 0)
            {
                selectBiggsDarklighterSubPhase.AddDecision("Cancel Hit result", delegate { PreventDamage(DieSide.Success); });
                selectBiggsDarklighterSubPhase.AddTooltip("Cancel Hit result", HostShip.ImageUrl);
            }

            if (curToDamage.AssignedDamageDiceroll.CriticalSuccesses > 0)
            {
                selectBiggsDarklighterSubPhase.AddDecision("Cancel Crit result", delegate { PreventDamage(DieSide.Crit); });
                selectBiggsDarklighterSubPhase.AddTooltip("Cancel Crit result", HostShip.ImageUrl);
            }

            selectBiggsDarklighterSubPhase.AddDecision("Don't cancel anything", delegate { DecisionSubPhase.ConfirmDecision(); });
            selectBiggsDarklighterSubPhase.DefaultDecisionName = GetDefaultDecision();
            selectBiggsDarklighterSubPhase.ShowSkipButton = true;
            selectBiggsDarklighterSubPhase.DecisionOwner = HostShip.Owner;
            selectBiggsDarklighterSubPhase.Start();
        }

        protected string GetDefaultDecision()
        {
            string result = "Don't cancel anything";

            if (HostShip.State.HullCurrent > 1)
            {
                if (curToDamage.AssignedDamageDiceroll.RegularSuccesses > 0)
                {
                    result = "Cancel Hit result";
                }

                if (curToDamage.AssignedDamageDiceroll.CriticalSuccesses > 0)
                {
                    result = "Cancel Crit result";
                }

            }

            return result;
        }

        private void PreventDamage(DieSide type)
        {
            // Find a crit and remove it from the ship we're protecting's assigned damage.
            Die dieToRemove = curToDamage.AssignedDamageDiceroll.DiceList.Find(n => n.Side == type);
            curToDamage.AssignedDamageDiceroll.DiceList.Remove(dieToRemove);

            HostShip.Tokens.AssignToken(typeof(IonToken), DecisionSubPhase.ConfirmDecision);
        }
    }
}