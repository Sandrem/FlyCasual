using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using System;
using Abilities;

namespace UpgradesList
{
    public class Selflessness : GenericUpgrade
    {
        public Selflessness() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Selflessness";
            Cost = 1;

            isUnique = true;

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(9, 0));

            UpgradeAbilities.Add(new SelflessnessAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            bool result = true;

            if (ship.ShipBaseSize != BaseSize.Small) result = false;
            else if (ship.faction != Faction.Rebel) result = false;

            return result;
        }
    }
}

namespace Abilities
{
    public class SelflessnessAbility : GenericAbility
    {
        private GenericShip curToDamage;
        private DamageSourceEventArgs curDamageInfo;

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckSelflessnessAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckSelflessnessAbility;
        }

        private void CheckSelflessnessAbility(GenericShip toDamage, DamageSourceEventArgs e)
        {
            curToDamage = toDamage;
            curDamageInfo = e;

            // Is this ship the defender in combat?
            if (Combat.Defender != curToDamage)
                return;

            // Is the damage type a ship attack?
            if (curDamageInfo.DamageType != DamageTypes.ShipAttack)
                return;

            // Is the defender on our team and not us? If not return.
            if (Combat.Defender.Owner.PlayerNo != HostShip.Owner.PlayerNo || Combat.Defender.ShipId == HostShip.ShipId)
                return;

            // If the defender is at range one of us we register our trigger to prevent damage.
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Combat.Defender, HostShip);
            if (distanceInfo.Range == 1 && Combat.DiceRollAttack.RegularSuccesses > 0)
            {
                    RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseSelflessnessAbility);
            }
        }

        private void UseSelflessnessAbility(object sender, System.EventArgs e)
        {
            // Are there any non-crit damage results in the damage queue?
            if (Combat.Defender.AssignedDamageDiceroll.RegularSuccesses > 0)
            {
                // If there are we prompt to see if they want to use the ability.
                AskToUseAbility(AlwaysUseByDefault, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            HostUpgrade.Discard(SufferRegularHits);
        }

        private void SufferRegularHits()
        {
            // Get the number of hits assigned to the ship we're protecting.
            int hits = curToDamage.AssignedDamageDiceroll.RegularSuccesses;

            // Remove the hits from the assigned damage dice of the ship we're preventing damage on.
            for (int canceledHit = 0; canceledHit < hits; canceledHit++)
            {
                curToDamage.AssignedDamageDiceroll.RemoveType(DieSide.Success);
            }

            DamageSourceEventArgs selflessDamage = new DamageSourceEventArgs()
            {
                Source = "Selflesness",
                DamageType = DamageTypes.CardAbility
            };

            // Deal that damage to us instead.
            HostShip.Damage.TryResolveDamage(hits, selflessDamage, DecisionSubPhase.ConfirmDecision);
        }


    }
}