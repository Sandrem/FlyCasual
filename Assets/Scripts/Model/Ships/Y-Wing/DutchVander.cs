using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;
using System;
using RuleSets;
using ActionsList;
using Tokens;

namespace Ship
{
    namespace YWing
    {
        public class DutchVander : YWing, ISecondEditionPilot
        {
            public DutchVander() : base()
            {
                PilotName = "\"Dutch\" Vander";
                PilotSkill = 6;
                Cost = 23;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                faction = Faction.Rebel;

                PilotAbilities.Add(new Abilities.DutchVanderAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;

                PilotAbilities.RemoveAll(a => a is Abilities.DutchVanderAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.DutchVanderAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class DutchVanderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += DutchVanderPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= DutchVanderPilotAbility;
        }

        private void DutchVanderPilotAbility(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.BlueTargetLockToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, StartSubphaseForDutchVanderPilotAbility);
            }
        }

        private void StartSubphaseForDutchVanderPilotAbility(object sender, System.EventArgs e)
        {
            Selection.ThisShip = HostShip;
            if (HostShip.Owner.Ships.Count > 1)
            {
                SelectTargetForAbility(
                    GrantFreeTargetLock,
                    FilterAbilityTargets,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    true,
                    null,
                    HostShip.PilotName,
                    "Choose another ship.\nIt may acquire a Target Lock.",
                    HostShip.ImageUrl
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 2);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            if (ship.Tokens.CountTokensByType(typeof(BlueTargetLockToken)) == 0) result += 100;
            if (Actions.HasTarget(ship)) result += 50;

            return result;
        }

        private void GrantFreeTargetLock()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, StartSubphaseForTargetLock);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, SelectShipSubPhase.FinishSelection);
        }

        private void StartSubphaseForTargetLock(object sender, System.EventArgs e)
        {
            Selection.ThisShip = TargetShip;
            Selection.ThisShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                HostShip.PilotName,
                HostShip.ImageUrl
            );
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class DutchVanderAbilitySE : GenericAbility
        {
            private GenericShip LockedShip;

            public override void ActivateAbility()
            {
                HostShip.OnActionIsPerformed += CheckConditions;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnActionIsPerformed -= CheckConditions;
            }

            private void CheckConditions(GenericAction action)
            {
                if (action is TargetLockAction && HasPossibleTargets())
                {
                    RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, StartAbility);
                }
            }

            private bool HasPossibleTargets()
            {
                return BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 3), Team.Type.Friendly).Count > 0;
            }

            private void StartAbility(object sender, EventArgs e)
            {
                LockedShip = GetLockedShip();
                if (LockedShip == null)
                {
                    Messages.ShowError("\"Dutch\" Vander: No Locked Object!");
                    Triggers.FinishTrigger();
                    return;
                }

                SelectTargetForAbility(
                    GetTargetLockOnSameTarget,
                    AnotherFriendlyShipInRange,
                    AiPriority,
                    HostShip.Owner.PlayerNo,
                    true,
                    null,
                    HostShip.PilotName,
                    "Choose ship. That ship will acquire a lock on the object you locked.",
                    HostShip.ImageUrl
                );
            }

            private GenericShip GetLockedShip()
            {
                GenericShip result = null;

                BlueTargetLockToken blueTargetLock = HostShip.Tokens.GetToken<BlueTargetLockToken>(letter: '*');
                if (blueTargetLock != null)
                {
                    result = blueTargetLock.OtherTokenOwner;
                }

                return result;
            }

            private void GetTargetLockOnSameTarget()
            {
                Messages.ShowInfo(TargetShip.PilotName + " acquired Target Lock on " + LockedShip.PilotName);
                Actions.AcquireTargetLock(TargetShip, LockedShip, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection, ignoreRange: true);
            }

            private bool AnotherFriendlyShipInRange(GenericShip ship)
            {
                return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
            }

            private int AiPriority(GenericShip ship)
            {
                int priority = 0;

                if (!ship.Tokens.HasToken(typeof(BlueTargetLockToken))) priority += 50;

                BoardTools.ShotInfo shotInfo = new BoardTools.ShotInfo(ship, LockedShip, ship.PrimaryWeapon);
                if (shotInfo.IsShotAvailable) priority += 40;

                priority += ship.Firepower * 5;

                return priority;
            }
        }
    }
}