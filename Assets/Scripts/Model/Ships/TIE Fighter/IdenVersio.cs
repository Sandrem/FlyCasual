using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Tokens;
using RuleSets;
using SubPhases;

namespace Ship
{
    namespace TIEFighter
    {
        public class IdenVersio : TIEFighter, ISecondEditionPilot
        {
            public IdenVersio() : base()
            {
                PilotName = "Iden Versio";
                PilotSkill = 4;
                Cost = 40;

                IsUnique = true;

                UsesCharges = true;
                MaxCharges = 1;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.IdenVersioAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IdenVersioAbilitySE : GenericAbility
    {
        private GenericShip curToDamage;

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckIdenVersioAbilitySE;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckIdenVersioAbilitySE;
        }

        private void CheckIdenVersioAbilitySE(GenericShip toDamage, DamageSourceEventArgs e)
        {
            curToDamage = toDamage;

            // Is the defender on our team? If not return.
            if (curToDamage.Owner.PlayerNo != HostShip.Owner.PlayerNo)
                return;

            if (!(curToDamage is Ship.TIEFighter.TIEFighter))
                return;

            // If the defender is at range one of us we register our trigger to prevent damage.
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(curToDamage, HostShip);
            if (distanceInfo.Range == 1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseIdenVersioAbilitySE);
            }
        }

        private void UseIdenVersioAbilitySE(object sender, System.EventArgs e)
        {
            // Are there any non-crit damage results in the damage queue?
            if (HostShip.Charges > 0)
            {
                // If there are we prompt to see if they want to use the ability.
                AskToUseAbility(AlwaysUseByDefault, delegate { HostShip.RemoveCharge(BlankDamage); });
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void BlankDamage()
        {
            curToDamage.AssignedDamageDiceroll.RemoveAll();
            DecisionSubPhase.ConfirmDecision();
        }


    }
}