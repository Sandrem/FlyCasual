using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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
                PilotAbilities.Add(new PilotAbilitiesNamespace.OmegaLeaderAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class OmegaLeaderAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.OnAttackStartAsAttacker += AddOmegaLeaderPilotAbility;
            Host.OnAttackStartAsDefender += AddOmegaLeaderPilotAbility;
        }

        private void AddOmegaLeaderPilotAbility()
        {
            Messages.ShowErrorToHuman("Test.");
            GenericShip enemyship;
            if (Combat.Defender.ShipId == Host.ShipId)
            {
                enemyship = Combat.Attacker;
            }
            else
            {
                enemyship = Combat.Defender;
            }

            char targetLock = Host.GetTargetLockLetterPair(enemyship);
            if (targetLock != ' ')
            {
                enemyship.OnTryAddAvailableActionEffect += UseOmegaLeaderRestriction;
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