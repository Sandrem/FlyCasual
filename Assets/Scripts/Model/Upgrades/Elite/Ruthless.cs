﻿using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using System;
using Abilities;
using RuleSets;
using ActionsList;
using System.Linq;

namespace UpgradesList
{
    public class Ruthless : GenericUpgrade
    {
        public Ruthless() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Ruthless";
            Cost = 1;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.RuthlessAbility());

            SEImageNumber = 13;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
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
                    Host = host,
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
                    true,
                    null,
                    HostUpgrade.Name,
                    "Choose another friendly ship near the defender. That ship will suffer 1 damage and you will change 1 die result to Hit.",
                    HostUpgrade.ImageUrl
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
                BoardTools.DistanceInfo distInfo = new BoardTools.DistanceInfo(targetShip, rangeFromShip);
                return distInfo.Range >= minRange && distInfo.Range <= maxRange;
            }

            private int GetAiPriority(GenericShip ship)
            {
                if (ship.Hull == 1) return 0;
                return ship.Hull + ship.Shields;
            }
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

            List<GenericShip> friendlyShipsAtRange1FromTarget = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Friendly);
            return friendlyShipsAtRange1FromTarget.Any(n => n.ShipId != Host.ShipId);
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if ((Combat.DiceRollAttack.Blanks > 0) || (Combat.DiceRollAttack.Focuses > 0))
            {
                List<GenericShip> friendlyShipsAtRange1FromTarget = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Friendly);
                if (friendlyShipsAtRange1FromTarget.Any(n => n.ShipId != Host.ShipId && n.Hull > 0)) result = 33;
            }

            return result;
        }
    }
}
