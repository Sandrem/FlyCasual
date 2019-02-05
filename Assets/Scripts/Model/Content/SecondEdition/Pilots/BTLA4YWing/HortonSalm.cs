using Abilities.SecondEdition;
using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HortonSalm : BTLA4YWing
        {
            public HortonSalm() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Horton Salm",
                    4,
                    37,
                    isLimited: true,
                    abilityType: typeof(HortonSalmAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 15
                );

                ModelInfo.SkinName = "Gray";
            }
        }
    }
}

namespace Abilities.SecondEdition
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
            ship.AddAvailableDiceModification(new HortonSalmActionSE() { HostShip = HostShip });
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
                    if (friendlyShip != HostShip)
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
