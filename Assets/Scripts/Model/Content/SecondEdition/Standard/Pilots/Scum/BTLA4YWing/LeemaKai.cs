using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTLA4YWing
{
    public class LeemaKai : BTLA4YWing
    {
        public LeemaKai() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Leema Kai",
                "Opportunity Knocks",
                Faction.Scum,
                5,
                4,
                14,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.LeemaKaiAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Turret,
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
                    UpgradeType.Astromech,
                    UpgradeType.Device
                },
                tags: new List<Tags>
                {
                    Tags.YWing
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f5/ce/f5ce6adb-194d-458f-a6df-f84a6cc57d33/swz85_pilot_leemakai.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LeemaKaiAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (AbilityConditionsAreMet())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, StarExecutionOfAbility);
            }
        }

        private bool AbilityConditionsAreMet()
        {
            bool result = true;

            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (enemyShip.SectorsInfo.IsShipInSector(HostShip, Arcs.ArcType.Front)) return false;
            }

            return result;
        }

        private void StarExecutionOfAbility(object sender, EventArgs e)
        {
            if (HasTargersForAbility())
            {
                SelectTargetForAbility
                (
                    TryAcquireLock,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: HostShip.PilotInfo.PilotName,
                    description: "You may acquire a lock on an enemy ship in your full front arc",
                    imageSource: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool HasTargersForAbility()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (FilterTargets(enemyShip)) return true;
            }

            return false;
        }

        private void TryAcquireLock()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Acquires lock on {TargetShip.PilotInfo.PilotName}");

            ActionsHolder.AcquireTargetLock(HostShip, TargetShip, Triggers.FinishTrigger, Triggers.FinishTrigger);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.FullFront);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = ship.PilotInfo.Cost;
            
            bool canAttackShip = false;
            bool isInRangeButRequiresLock = false;

            foreach (IShipWeapon weapon in HostShip.GetAllWeapons())
            {
                ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons);

                if (shotInfo.IsShotAvailable) canAttackShip = true;

                if (shotInfo.Range >= weapon.WeaponInfo.MinRange
                    && shotInfo.Range <= weapon.WeaponInfo.MaxRange
                    && weapon.WeaponInfo.RequiresTokens.Contains(typeof(Tokens.BlueTargetLockToken))
                )
                {
                    isInRangeButRequiresLock = true;
                }
            }

            if (canAttackShip) priority += 100;
            if (isInRangeButRequiresLock) priority += 200;

            return priority;
        }
    }
}