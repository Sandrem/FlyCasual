using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class CommanderMalarus : GenericDualUpgrade
    {
        public CommanderMalarus() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Commander Malarus",
                UpgradeType.Crew,
                cost: 2,
                restriction: new FactionRestriction(Faction.FirstOrder),
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CommanderMalarusCrewAbility)
            );

            SelectSideOnSetup = false;
            AnotherSide = typeof(CommanderMalarusPerfected);

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(239, 1)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6d/2a/6d2a8204-2de1-4b1e-8980-456501628a24/swz69_malarus_card.png";
        }
    }

    public class CommanderMalarusPerfected : GenericDualUpgrade
    {
        public CommanderMalarusPerfected() : base()
        {
            IsHidden = true; // Hidden in Squad Builder only

            UpgradeInfo = new UpgradeCardInfo(
                "Commander Malarus (Perfected)",
                UpgradeType.Crew,
                cost: 7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CommanderMalarusPerfectedAbility)
            );

            AnotherSide = typeof(CommanderMalarus);
            IsSecondSide = true;

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/42/ed/42edc96c-938a-4bd4-88de-7be1e449d96b/swz69_malarus-perfected_card.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CommanderMalarusCrewAbility : GenericAbility
    {
        private GenericShip ShipUsedAbility;

        public override void ActivateAbility()
        {
            AddDiceModification(
                "Commander Malarus",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                isGlobal: true,
                payAbilityPostCost: MarkAsUsed
            );

            GenericShip.OnAttackMissedAsAttackerGlobal += CheckFlip;
        }

        private void MarkAsUsed()
        {
            ShipUsedAbility = Combat.Attacker;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && Board.CheckInRange(HostShip, Combat.Attacker, 0, 1, RangeCheckReason.UpgradeCard)
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.DiceRollAttack.HasResult(DieSide.Blank)
                && !Combat.Attacker.PilotInfo.IsLimited;
        }

        private int GetAiPriority()
        {
            return 95;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();

            GenericShip.OnAttackMissedAsAttackerGlobal -= CheckFlip;
        }

        private void CheckFlip()
        {
            if (ShipUsedAbility != null && Combat.Attacker.ShipId == ShipUsedAbility.ShipId)
            {
                Messages.ShowInfo("Commander Malarus: Attack is missed");
                (HostUpgrade as GenericDualUpgrade).Flip();
            }
        }
    }

    public class CommanderMalarusPerfectedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Commander Malarus (Perfected)",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success,
                payAbilityCost: GetStress,
                isForcedModification: true
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye)
                && Combat.DiceRollAttack.HasResult(DieSide.Focus);
        }

        private int GetAiPriority()
        {
            return 55;
        }

        private void GetStress(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(
                typeof(StressToken),
                delegate{ CheckDamage(delegate { callback(true); }); }
            );
        }

        private void CheckDamage(Action callBack)
        {
            if (HostShip.Tokens.CountTokensByType<StressToken>() >= 2)
            {
                HostShip.Damage.TryResolveDamage(
                    damage: 1,
                    new DamageSourceEventArgs() {
                        DamageType = DamageTypes.CardAbility,
                        Source = HostUpgrade
                    },
                    callBack
                );
            }
            else
            {
                callBack();
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}