using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Malice : GenericUpgrade
    {
        public Malice() : base()
        {            
            UpgradeInfo = new UpgradeCardInfo
            (
                "Malice",
                UpgradeType.ForcePower,
                cost: 4,
                restriction: new TagRestriction(Tags.DarkSide),
                abilityType: typeof(Abilities.SecondEdition.MaliceAbility)       
            );

            ImageUrl = "https://i.imgur.com/mRyM1s0.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MaliceAbility : GenericAbility
    {
        
        // track whether we've used malice during this attack, so that when if we deal a crit in the damage phase, we know it was
        // through our ability, not through a natural crit. 
        private bool MaliceActive = false;

        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Focus, DieSide.Success },
                DieSide.Crit,
                payAbilityCost: SpendForce
            );
            
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal += MaliceForceAbility;
            GenericShip.OnAttackFinishGlobal += ResetMaliceActive;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal -= MaliceForceAbility;
            GenericShip.OnAttackFinishGlobal -= ResetMaliceActive;
        }

        private void ResetMaliceActive(GenericShip ship)
        {
            MaliceActive = false;
        }

        private void MaliceForceAbility(GenericShip ship, GenericDamageCard crit, EventArgs e)
        {

            if ((e as DamageSourceEventArgs) == null) return;

            GenericShip damageSourceShip = (e as DamageSourceEventArgs).Source as GenericShip;
            if (damageSourceShip == null) return;

            int forceToRecover = Math.Min(2, HostShip.State.MaxForce - HostShip.State.Force);            

            // check that malice was used, that we are missing some force, that the card type was a pilot crit, and that we are the source of the damage
            if (MaliceActive &&
                forceToRecover > 0 &&                
                crit.Type == CriticalCardType.Pilot && 
                damageSourceShip.ShipId == HostShip.ShipId &&
                (e as DamageSourceEventArgs).DamageType == DamageTypes.ShipAttack)                 
            {                
                Messages.ShowInfo("Malice causes " + ship.PilotInfo.PilotName + " to recover " + forceToRecover + " force since crit was of type \"Pilot\"");
                HostShip.State.RestoreForce(forceToRecover);               
                // need to set this here to handle the edge case of dealing 2 or more faceup pilot cards in the same attack
                MaliceActive = false;
            }                        
        }
        
        private void SpendForce(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                MaliceActive = true;
                HostShip.State.SpendForce(1, delegate { callback(true); });                
            }
            else
            {
                callback(false);
            }
        }
        private int AiPriority()
        {
            return 45;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack &&
                (Combat.DiceRollAttack.Focuses > 0 || Combat.DiceRollAttack.Successes > 0) &&
                HostShip.State.Force > 0;            
        }

    }
}