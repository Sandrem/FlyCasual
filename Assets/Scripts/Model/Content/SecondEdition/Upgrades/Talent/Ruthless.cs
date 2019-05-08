using Upgrade;
using System.Collections.Generic;
using Ship;
using BoardTools;
using ActionsList;
using System;
using SubPhases;
using System.Linq;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class Ruthless : GenericUpgrade
    {
        public Ruthless() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ruthless",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.RuthlessAbility),
                restriction: new FactionRestriction(Faction.Imperial),
                seImageNumber: 13
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class RuthlessAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddDiceModification;
        }

        private void AddDiceModification(GenericShip host)
        {
            GenericAction newAction = new RuthlessDiceModification
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host,
                Source = HostUpgrade
            };
            host.AddAvailableDiceModification(newAction);
        }

        public void StartAbility(Action callback)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, StartSelectShipForAbility);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, delegate
            {
                Combat.DiceRollAttack.ChangeWorstResultTo(DieSide.Success);
                callback();
            });
        }

        private void StartSelectShipForAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                ShipIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "Choose another friendly ship near the defender: that ship will suffer 1 damage and you will change 1 die result to Hit",
                HostUpgrade,
                showSkipButton: false
            );
        }

        private void ShipIsSelected()
        {
            DamageSourceEventArgs ruthlessDamage = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.CardAbility
            };

            TargetShip.Damage.TryResolveDamage(1, ruthlessDamage, SelectShipSubPhase.FinishSelection);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterTargetsByShipRange(ship, Combat.Defender, 0, 1) && FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly });
        }

        private bool FilterTargetsByShipRange(GenericShip targetShip, GenericShip rangeFromShip, int minRange, int maxRange)
        {
            DistanceInfo distInfo = new DistanceInfo(targetShip, rangeFromShip);
            return distInfo.Range >= minRange && distInfo.Range <= maxRange;
        }

        private int GetAiPriority(GenericShip ship)
        {
            if (ship.State.HullCurrent == 1) return 0;
            return ship.State.HullCurrent + ship.State.ShieldsCurrent;
        }
    }
}

namespace ActionsList
{
    public class RuthlessDiceModification : GenericAction
    {
        public RuthlessDiceModification()
        {
            Name = DiceModificationName = "Ruthless";
        }

        public override void ActionEffect(Action callBack)
        {
            (Source.UpgradeAbilities[0] as Abilities.SecondEdition.RuthlessAbility).StartAbility(callBack);
        }

        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;

            // The team argument is relative to the ship it's measuring from, not the host of this ability, so we need to query for enemies of the defender
            List<GenericShip> friendlyShipsAtRange1FromTarget = Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Enemy);
            return friendlyShipsAtRange1FromTarget.Any(n => n.ShipId != HostShip.ShipId);
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if ((Combat.DiceRollAttack.Blanks > 0) || (Combat.DiceRollAttack.Focuses > 0))
            {
                // The team argument is relative to the ship it's measuring from, not the host of this ability, so we need to query for enemies of the defender
                List<GenericShip> friendlyShipsAtRange1FromTarget = Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Enemy);
                if (friendlyShipsAtRange1FromTarget.Any(n => n.ShipId != HostShip.ShipId && n.State.HullCurrent > 0)) result = 33;
            }

            return result;
        }
    }
}