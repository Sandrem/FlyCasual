using Abilities;
using ActionsList;
using BoardTools;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace Aggressor
    {
        class IG88A : Aggressor, ISecondEditionPilot
        {
            public IG88A() : base()
            {
                PilotName = "IG-88A";
                PilotSkill = 4;
                Cost = 70;

                IsUnique = true;
                PilotRuleType = typeof(SecondEdition);
                SkinName = "Green";

                PilotAbilities.Add(new IG88AAbility());

                SEImageNumber = 197;
            }

            public void AdaptPilotToSecondEdition()
            {
                //Not required
            }
        }
    }
}

namespace Abilities
{
    public class IG88AAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (HostShip.Tokens.HasToken(typeof(CalculateToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
            }
        }

        private void Ability(object sender, EventArgs e)
        {
            if (TargetsForAbilityExist(FilterAbilityTarget))
            {
                Messages.ShowInfoToHuman("IG-88A Ability: Select a ship to transfer a calculate token to");

                SelectTargetForAbility(
                    TransferCalculate,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotName,
                    "Choose a ship to transfer one of your calculate tokens to",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void TransferCalculate()
        {
            TargetShip.Tokens.AssignToken(
                typeof(CalculateToken),
                delegate {
                    HostShip.Tokens.RemoveToken(
                        typeof(CalculateToken),
                        delegate {
                            SelectShipSubPhase.FinishSelection();
                        }
                    );
                }
            );
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            //This AI decision takes a defensive approach, could be written to determine offensive power of a selected ship
            List<GenericShip> myShipsAtRangeOne = Board.GetShipsAtRange(HostShip, new Vector2(1, 1), Team.Type.Enemy);
            List<GenericShip> myShipsAtRangeTwo = Board.GetShipsAtRange(HostShip, new Vector2(2, 2), Team.Type.Enemy);
            List<GenericShip> myShipsAtRangeThree = Board.GetShipsAtRange(HostShip, new Vector2(3, 3), Team.Type.Enemy);

            List<GenericShip> targetsShipsAtRangeOne = Board.GetShipsAtRange(ship, new Vector2(1, 1), Team.Type.Enemy);
            List<GenericShip> targetsShipsAtRangeTwo = Board.GetShipsAtRange(ship, new Vector2(2, 2), Team.Type.Enemy);
            List<GenericShip> targetsShipsAtRangeThree = Board.GetShipsAtRange(ship, new Vector2(3, 3), Team.Type.Enemy);

            if (ship.Agility == 0 || (targetsShipsAtRangeOne.Count == 0 && targetsShipsAtRangeTwo.Count == 0 && targetsShipsAtRangeThree.Count == 0))
            {
                return 0;
            }

            int myThreatLevel = GetThreatLevelForShip(HostShip, myShipsAtRangeOne, myShipsAtRangeTwo, myShipsAtRangeThree);
            int targetThreatLevel = GetThreatLevelForShip(ship, targetsShipsAtRangeOne, targetsShipsAtRangeTwo, targetsShipsAtRangeThree);

            myThreatLevel *= HostShip.Agility;
            targetThreatLevel *= ship.Agility;
            if (targetThreatLevel == 0)
            {
                return 0;
            }
            if (!ship.Tokens.HasToken(typeof(FocusToken)) && !ship.Tokens.HasToken(typeof(CalculateToken)))
            {
                targetThreatLevel += 75;
            }
            if (ship.Tokens.HasToken(typeof(FocusToken)))
            {
                targetThreatLevel -= 75;
            }
            if (ship.Tokens.HasToken(typeof(CalculateToken)))
            {
                targetThreatLevel -= 55;
            }
            if (myThreatLevel > targetThreatLevel)
            {
                return 0;
            }
            else
            {
                return targetThreatLevel;
            }
        }

        private int GetThreatLevelForShip(GenericShip ship, List<GenericShip> enemiesAtRangeOne, List<GenericShip> enemiesAtRangeTwo, List<GenericShip> enemiesAtRangeThree)
        {
            int shipThreatLevel = 0;
            foreach (GenericShip enemyShip in enemiesAtRangeOne)
            {
                ShotInfo shotInfo = new ShotInfo(enemyShip, HostShip, enemyShip.PrimaryWeapon);
                if (shotInfo.IsShotAvailable)
                {
                    shipThreatLevel += (enemyShip.Firepower + 1) * 10;
                }
            }

            foreach (GenericShip enemyShip in enemiesAtRangeTwo)
            {
                ShotInfo shotInfo = new ShotInfo(enemyShip, HostShip, enemyShip.PrimaryWeapon);
                if (shotInfo.IsShotAvailable)
                {
                    shipThreatLevel += enemyShip.Firepower * 10;
                }
            }

            foreach (GenericShip enemyShip in enemiesAtRangeThree)
            {
                ShotInfo shotInfo = new ShotInfo(enemyShip, HostShip, enemyShip.PrimaryWeapon);
                if (shotInfo.IsShotAvailable)
                {
                    shipThreatLevel += enemyShip.Firepower * 7;
                }
            }
            return shipThreatLevel;
        }

        private bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3) && FilterTargetWithCalculateAction(ship);
        }

        private bool FilterTargetWithCalculateAction(GenericShip ship)
        {
            return ship.ActionBar.HasAction(typeof(CalculateAction));
        }
    }
}
