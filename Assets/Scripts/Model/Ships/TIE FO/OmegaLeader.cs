using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Abilities;
using System;

namespace Ship
{
    namespace TIEFO
    {
        public class OmegaLeader : TIEFO
        {
            public OmegaLeader() : base()
            {
                PilotName = "\"Omega Leader\"";
                PilotSkill = 8;
                Cost = 21;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new OmegaLeaderAbility());
            }
        }
    }
}

namespace Abilities
{
    public class OmegaLeaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender += AddOmegaLeaderPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender -= AddOmegaLeaderPilotAbility;
        }

        private void AddOmegaLeaderPilotAbility()
        {
            GenericShip enemyship;
            if (Combat.Defender.ShipId == HostShip.ShipId)
            {
                enemyship = Combat.Attacker;
            }
            else
            {
                enemyship = Combat.Defender;
            }

            char targetLock = HostShip.Tokens.GetTargetLockLetterPair(enemyship);
            if (targetLock != ' ')
            {
                enemyship.OnTryAddAvailableActionEffect += UseOmegaLeaderRestriction;
                enemyship.OnTryAddAvailableOppositeActionEffect += UseOmegaLeaderRestriction;
                enemyship.OnAttackFinish += RemoveOmegaLeaderPilotAbility;
            }
        }

        private void UseOmegaLeaderRestriction(ActionsList.GenericAction action, ref bool canBeUsed)
        {
            Messages.ShowErrorToHuman("Omega Leader: Unable to modify dice.");
            canBeUsed = false;
        }

        private void RemoveOmegaLeaderPilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableActionEffect -= UseOmegaLeaderRestriction;
            ship.OnAttackFinish -= RemoveOmegaLeaderPilotAbility;
        }
    }
}