using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using BoardTools;
using RuleSets;

namespace Ship
{
    namespace YWing
    {
        public class HortonSalm : YWing, ISecondEditionPilot
        {
            public HortonSalm() : base()
            {
                PilotName = "Horton Salm";
                PilotSkill = 8;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                faction = Faction.Rebel;

                SkinName = "Gray";

                PilotAbilities.Add(new Abilities.HortonSalmAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 38;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.RemoveAll(ability => ability is Abilities.HortonSalmAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.HortonSalmAbilitySE());

                SEImageNumber = 15;
            }
        }
    }
}

namespace Abilities
{
    public class HortonSalmAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += HortonSalmPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= HortonSalmPilotAbility;
        }

        public void HortonSalmPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new HortonSalmAction());
        }

        private class HortonSalmAction : ActionsList.GenericAction
        {
            public HortonSalmAction()
            {
                Name = DiceModificationName = "Horton Salm's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = new List<DieSide> { DieSide.Blank },
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                ShotInfo shotInfo = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                if ((Combat.AttackStep == CombatStep.Attack) && (shotInfo.Range > 1))
                {
                    result = true;
                }
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    if (Combat.DiceRollAttack.Blanks > 0) result = 95;
                }

                return result;
            }
        }

    }
}

namespace Abilities.SecondEdition
{
    public class HortonSalmAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += HortonSalmPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= HortonSalmPilotAbility;
        }

        public void HortonSalmPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new HortonSalmActionSE() { Host = HostShip });
        }

        private class HortonSalmActionSE : ActionsList.GenericAction
        {
            int numFriendlyShips = 0;

            public HortonSalmActionSE()
            {
                Name = DiceModificationName = "Horton Salm's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                int tempFriendlyShips = numFriendlyShips;
                numFriendlyShips = 0;

                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = tempFriendlyShips,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsDiceModificationAvailable()
            {
                if (Combat.AttackStep != CombatStep.Attack)
                    return false;

                List<GenericShip> friendlyShipsAtRange = Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Enemy);

                foreach (GenericShip friendlyShip in friendlyShipsAtRange)
                {
                    if (friendlyShip != Host)
                    {
                        numFriendlyShips++;
                    }
                }

                if (numFriendlyShips > 0)
                {
                    return true;
                }

                return false;
            }

            public override int GetDiceModificationPriority()
            {
                return 90;
            }
        }
    }
}
