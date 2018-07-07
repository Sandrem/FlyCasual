using Upgrade;
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
                HostShip.AfterGenerateAvailableActionEffectsList += AddDiceModification;
            }

            public override void DeactivateAbility()
            {
                HostShip.AfterGenerateAvailableActionEffectsList -= AddDiceModification;
            }

            private void AddDiceModification(GenericShip host)
            {
                GenericAction newAction = new RuthlessDiceModification
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    Host = host,
                    Source = HostUpgrade
                };
                host.AddAvailableActionEffect(newAction);
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
                    "" +
                    "Choose another friendly ship near the defender. That ship will suffer 1 damage and you will change 1 die result to Hit.",
                    HostUpgrade.ImageUrl
                );
            }

            private void ShipIsSelected()
            {
                TargetShip.AssignedDamageDiceroll.AddDice(DieSide.Success);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = TargetShip.Owner.PlayerNo,
                    EventHandler = TargetShip.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = HostShip,
                        DamageType = DamageTypes.CardAbility
                    }
                });

                Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, SelectShipSubPhase.FinishSelection);
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
            Name = EffectName = "Ruthless";
        }

        public override void ActionEffect(Action callBack)
        {
            (Source.UpgradeAbilities[0] as Abilities.SecondEdition.RuthlessAbility).StartAbility(callBack);
        }

        public override bool IsActionEffectAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;

            List<GenericShip> friendlyShipsAtRange1FromTarget = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Friendly);
            return friendlyShipsAtRange1FromTarget.Any(n => n.ShipId != Host.ShipId);
        }

        public override int GetActionEffectPriority()
        {
            return 0;
        }
    }
}
