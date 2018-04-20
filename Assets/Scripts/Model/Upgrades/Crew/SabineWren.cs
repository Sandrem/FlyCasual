using Upgrade;
using Ship;
using Abilities;
using System;
using System.Collections.Generic;
using Bombs;
using UnityEngine;
using System.Linq;
using SubPhases;

namespace UpgradesList
{
    public class SabineWren : GenericUpgradeSlotUpgrade
    {
        public SabineWren() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Sabine Wren";
            Cost = 2;

            isUnique = true;

            AvatarOffset = new Vector2(47, 1);

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Bomb),
            };

            UpgradeAbilities.Add(new SabineWrenCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class SabineWrenCrewAbility : GenericAbility
    {
        private GameObject detonatedBombObject;

        public override void ActivateAbility()
        {
            BombsManager.OnBombIsRemoved += CheckAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            BombsManager.OnBombIsRemoved -= CheckAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility(GenericBomb bomb, GameObject bombObject)
        {
            if (!IsAbilityUsed && IsBombFriendly(bomb) && IsAtLeastOneEnemyInRange(bomb, bombObject))
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnBombIsRemoved,
                    AskToUseSabine,
                    new BombDetonationEventArgs() { BombObject = bombObject }
               );
            }
        }

        private bool IsBombFriendly(GenericBomb bomb)
        {
            return bomb.Host.Owner.PlayerNo == HostShip.Owner.PlayerNo;
        }

        private bool IsAtLeastOneEnemyInRange(GenericBomb bomb, GameObject bombObject)
        {
            List<GenericShip> shipsInRange = BombsManager.GetShipsInRange(bombObject);

            // Count only enemies
            shipsInRange = shipsInRange.Where(n => n.Owner.PlayerNo != bomb.Host.Owner.PlayerNo).ToList();

            return shipsInRange.Count > 0;
        }

        private void AskToUseSabine(object sender, EventArgs e)
        {
            if (!IsAbilityUsed)
            {
                Messages.ShowInfoToHuman("Sabine Wren: Select ship to deal 1 damage");

                detonatedBombObject = (e as BombDetonationEventArgs).BombObject;

                SelectTargetForAbility(
                    DealDamageToShip,
                    FilterTargetsForDealDamage,
                    GetAiPriorityForDealDamage,
                    HostShip.Owner.PlayerNo,
                    true,
                    null,
                    HostUpgrade.Name,
                    "Choose enemy ship at range 1 of detonated bomb token.\nThat ship suffers 1 damage.",
                    HostUpgrade.ImageUrl
                );
            }
            else
            {
                Messages.ShowErrorToHuman("Ability of Sabine Wren was already used!");
                Triggers.FinishTrigger();
            }
        }

        private void DealDamageToShip()
        {
            IsAbilityUsed = true;

            DealOneDamage(TargetShip, SelectShipSubPhase.FinishSelection);
        }

        private bool FilterTargetsForDealDamage(GenericShip ship)
        {
            // Only enemies in range 1 (range of bomb detonation is range 1)
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && BombsManager.GetShipsInRange(detonatedBombObject).Contains(ship);
        }

        private int GetAiPriorityForDealDamage(GenericShip ship)
        {
            // Highest priority to ships with lowest HP
            return 50 - (ship.Hull + ship.Shields);
        }

        private void DealOneDamage(GenericShip ship, Action callback)
        {
            ship.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from Sabiine Wren",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = ship.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = this.HostUpgrade,
                    DamageType = DamageTypes.CardAbility
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callback);
        }
    }
}