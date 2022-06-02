using BoardTools;
using Ship;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class LtBlount : Z95AF4Headhunter
        {
            public LtBlount() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Blount",
                    "Team Player",
                    Faction.Rebel,
                    4,
                    3,
                    5,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LtBlountAbiliity),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    seImageNumber: 28
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LtBlountAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterLtBlountAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterLtBlountAbility;
        }

        private void RegisterLtBlountAbility()
        {
            if (Combat.Attacker == HostShip)
            {
                bool isFriendlyShips = false;

                foreach (GenericShip ship in Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Enemy))
                {
                    if (ship != HostShip)
                        isFriendlyShips = true;
                }

                if (isFriendlyShips)
                {
                    HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += LtBlountAddAttackDice;
                }
            }
        }

        private void LtBlountAddAttackDice(ref int value)
        {
            value++;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= LtBlountAddAttackDice;
        }
    }
}
