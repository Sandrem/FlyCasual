using Abilities.SecondEdition;
using Ship;
using SubPhases;
using System;
using Content;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class DutchVanderBoY : BTLA4YWing
        {
            public DutchVanderBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Dutch\" Vander",
                    "Battle of Yavin",
                    Faction.Rebel,
                    4,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(DutchVanderBoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    isStandardLayout: true
                );

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.IonCannonTurret));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AdvProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.TargetingAstromech));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/d4/Dutchvander-battleofyavin.png";

                PilotNameCanonical = "dutchvander-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DutchVanderBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsSpent += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsSpent -= CheckConditions;
        }

        private void CheckConditions(GenericShip ship, GenericToken token)
        {
            if (token is Tokens.BlueTargetLockToken && HasPossibleTargets())
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, StartAbility);
            }
        }

        private bool HasPossibleTargets()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 3), Team.Type.Friendly).Count > 0;
        }

        private void StartAbility(object sender, EventArgs e)
        {
            if (Combat.Defender == null)
            {
                Messages.ShowError(HostShip.PilotInfo.PilotName + " doesn't have any locked targets!");
                Triggers.FinishTrigger();
                return;
            }

            SelectTargetForAbility(
                GetTargetLockOnSameTarget,
                AnotherFriendlyShipInRange,
                AiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship, that ship will acquire a lock on the defender",
                HostShip
            );
        }

        private void GetTargetLockOnSameTarget()
        {
            if (Combat.Defender is GenericShip)
            {
                Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " acquired a Target Lock on " + (Combat.Defender as GenericShip).PilotInfo.PilotName);
            }
            else
            {
                Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " acquired a Target Lock on obstacle");
            }

            ActionsHolder.AcquireTargetLock(TargetShip, Combat.Defender, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection, ignoreRange: true);
        }

        private bool AnotherFriendlyShipInRange(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int AiPriority(GenericShip ship)
        {
            int priority = 0;

            if (!ship.Tokens.HasToken(typeof(BlueTargetLockToken))) priority += 50;

            if (Combat.Defender is GenericShip)
            {
                BoardTools.ShotInfo shotInfo = new BoardTools.ShotInfo(ship, Combat.Defender as GenericShip, ship.PrimaryWeapons);
                if (shotInfo.IsShotAvailable) priority += 40;
            }

            priority += ship.State.Firepower * 5;

            return priority;
        }
    }
}