using Ship;
using Upgrade;
using UnityEngine;
using Bombs;
using System.Collections.Generic;
using System.Linq;
using System;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class SabineWren : GenericUpgrade
    {
        public SabineWren() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sabine Wren",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                addSlot: new UpgradeSlot(UpgradeType.Bomb),
                abilityType: typeof(Abilities.FirstEdition.SabineWrenCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(47, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SabineWrenCrewAbility : GenericAbility
    {
        private GameObject detonatedBombObject;

        public override void ActivateAbility()
        {
            BombsManager.OnBombIsRemoved += CheckAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            BombsManager.OnBombIsRemoved -= CheckAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
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
            return bomb.HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo;
        }

        private bool IsAtLeastOneEnemyInRange(GenericBomb bomb, GameObject bombObject)
        {
            List<GenericShip> shipsInRange = BombsManager.GetShipsInRange(bombObject);

            // Count only enemies
            shipsInRange = shipsInRange.Where(n => n.Owner.PlayerNo != bomb.HostShip.Owner.PlayerNo).ToList();

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
                    HostUpgrade.UpgradeInfo.Name,
                    "Choose enemy ship at range 1 of detonated bomb token,\nthat ship suffers 1 damage",
                    HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman("Sabine Wren's ability has already been used!");
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
            return 50 - (ship.State.HullCurrent + ship.State.HullCurrent);
        }

        private void DealOneDamage(GenericShip ship, Action callback)
        {
            DamageSourceEventArgs sabineDamage = new DamageSourceEventArgs()
            {
                Source = this.HostUpgrade,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(1, sabineDamage, callback);
        }
    }
}