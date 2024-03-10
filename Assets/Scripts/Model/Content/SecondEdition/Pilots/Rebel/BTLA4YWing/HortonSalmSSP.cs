using Abilities.SecondEdition;
using BoardTools;
using Content;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HortonSalmSSP : BTLA4YWing
        {
            public HortonSalmSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Horton Salm",
                    "Gray Leader",
                    Faction.Rebel,
                    4,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(HortonSalmSSPAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Turret,
                        UpgradeType.Device
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    skinName: "Gray",
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(IonCannonTurret));
                MustHaveUpgrades.Add(typeof(ProximityMines));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/hortonsalm-swz106.png";

                PilotNameCanonical = "hortonsalm-swz106";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HortonSalmSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += HortonSalmSSPPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= HortonSalmSSPPilotAbility;
        }

        public void HortonSalmSSPPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new HortonSalmSSPActionSE());
        }

        private class HortonSalmSSPActionSE : ActionsList.GenericAction
        {
            public override string Name => HostShip.PilotInfo.PilotName;
            public override string DiceModificationName => HostShip.PilotInfo.PilotName;
            public override string ImageUrl => HostShip.ImageUrl;

            int numFriendlyShips = 0;

            public HortonSalmSSPActionSE()
            {
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
