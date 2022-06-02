using System;
using System.Collections.Generic;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class WilhuffTarkin : NimbusClassVWing
        {
            public WilhuffTarkin() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Wilhuff Tarkin",
                    "Aspiring Admiral",
                    Faction.Republic,
                    3,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WilhuffTarkinAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/bf/0e/bf0e3b50-3f36-4940-953b-f0a2d9f2b9b9/swz80_ship_tarkin.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WilhuffTarkinAbility : GenericAbility
    {
        private GenericShip LockedShip;

        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        // REGISTER SYSTEM ACTIVATION

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            if (HasLockAtObjectInRange1to3()) flag = true;
        }

        private bool HasLockAtObjectInRange1to3()
        {
            foreach (BlueTargetLockToken blueLock in HostShip.Tokens.GetTokens<BlueTargetLockToken>('*'))
            {
                GenericShip lockedShip = blueLock.OtherTargetLockTokenOwner as GenericShip;
                if (lockedShip != null)
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, lockedShip);
                    if (distInfo.Range >= 1 && distInfo.Range <= 3)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // SELECT A LOCKED TARGET

        private void RegisterAbility(GenericShip ship)
        {
            if (HasLockAtObjectInRange1to3())
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToChooseLockedTarget);
            }
        }

        private void AskToChooseLockedTarget(object sender, EventArgs e)
        {
            if (HasLockAtObjectInRange1to3())
            {
                SelectTargetForAbility(
                    AskToSelectAnotherFriendlyShip,
                    FilterLockedTargets,
                    GetLockedTargetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: HostShip.PilotInfo.PilotName,
                    description: "You may choose an object you have a locked at range 1-3...",
                    imageSource: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterLockedTargets(GenericShip ship)
        {
            if (!ActionsHolder.HasTargetLockOn(HostShip, ship)) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return (distInfo.Range >= 1 && distInfo.Range <= 3);
        }

        private int GetLockedTargetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        // SELECT ANOTHER FRIENDLY SHIP

        private void AskToSelectAnotherFriendlyShip()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            LockedShip = TargetShip;

            if (HasFriendlyShipAtRange1to3())
            {
                SelectTargetForAbility(
                    FriendlyShipIsSelected,
                    FilterFriendlyTargets,
                    GetFriednlyShipAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: HostShip.PilotInfo.PilotName,
                    description: "Choose another friendly ship at range 1-3 - it may acquire a lock on that object",
                    imageSource: HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman($"{HostShip.PilotInfo.PilotName}: No another friendly ship in range");
            }
        }

        private bool HasFriendlyShipAtRange1to3()
        {
            return Board.GetShipsAtRange(HostShip, new Vector2(1, 3), Team.Type.Friendly).Count > 0;
        }

        private bool FilterFriendlyTargets(GenericShip ship)
        {
            if (!Tools.CheckShipsTeam(HostShip, ship, TargetTypes.OtherFriendly)) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return (distInfo.Range >= 1 && distInfo.Range <= 3);
        }

        private int GetFriednlyShipAiPriority(GenericShip ship)
        {
            int priority = ship.PilotInfo.Cost;

            DistanceInfo distInfo = new DistanceInfo(ship, LockedShip);
            if (distInfo.Range < 4) priority += 100;

            ShotInfo shotInfo = new ShotInfo(ship, LockedShip, ship.PrimaryWeapons);
            if (shotInfo.IsShotAvailable) priority += 50;

            if (!ship.Tokens.HasToken<BlueTargetLockToken>('*')) priority += 100;

            return priority;
        }

        // ABILITY IS RESOLVED

        private void FriendlyShipIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            ActionsHolder.AcquireTargetLock(TargetShip, LockedShip, Triggers.FinishTrigger, Triggers.FinishTrigger);
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }
    }
}