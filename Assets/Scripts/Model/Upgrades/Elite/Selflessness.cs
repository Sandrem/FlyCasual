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

            AvatarOffset = new Vector2(9, 0);

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

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckSelflessnessAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckSelflessnessAbility;
        }

        private void CheckSelflessnessAbility()
        {
            if (Combat.Defender.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Defender.ShipId != HostShip.ShipId)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Combat.Defender, HostShip);
                if (distanceInfo.Range == 1 && Combat.DiceRollAttack.RegularSuccesses > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseSelflessnessAbility);
                }
            }
        }

        private void UseSelflessnessAbility(object sender, System.EventArgs e)
        {
            if (Combat.DiceRollAttack.RegularSuccesses > 0)
            {
                AskToUseAbility(AlwaysUseByDefault, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            DiscardUpgrade(SufferRegularHits);
        }

        private void DiscardUpgrade(System.Action callBack)
        {
            HostUpgrade.Discard(callBack);
        }

        private void SufferRegularHits()
        {
            List<Die> regularHitsDice = new List<Die>();

            List<Die> diceRollAttackCopy = new List<Die>(Combat.DiceRollAttack.DiceList);

            foreach (var die in diceRollAttackCopy)
            {
                if (die.Side == DieSide.Success)
                {
                    regularHitsDice.Add(die);
                    Combat.DiceRollAttack.DiceList.Remove(die);
                }
            }

            HostShip.AssignedDamageDiceroll.DiceList.AddRange(regularHitsDice);

            foreach (var die in HostShip.AssignedDamageDiceroll.DiceList)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer damage from Selflessness",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = HostShip.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Selflessness",
                        DamageType = DamageTypes.CardAbility
                    },
                    Skippable = true
                });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, DecisionSubPhase.ConfirmDecision);
        }


    }
}