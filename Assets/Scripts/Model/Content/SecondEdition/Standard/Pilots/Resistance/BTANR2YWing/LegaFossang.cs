using BoardTools;
using Bombs;
using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class LegaFossang : BTANR2YWing
        {
            public LegaFossang() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lega Fossang",
                    "Hero of Humbarine",
                    Faction.Resistance,
                    3,
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LegaFossangAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    skinName: "Blue"
                );

                ImageUrl = "https://i.imgur.com/SIFbsBi.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LegaFossangAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                GetRerollCount
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack &&
                (Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.Turret || Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.PrimaryWeapon) &&
                GetRerollCount() > 0;
        }

        private int GetDiceModificationPriority()
        {
            return 90; // Free rerolls
        }

        private int GetRerollCount()
        {
            int friendlies = 0;
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (ship.Tokens.HasToken(typeof(CalculateToken)))
                {
                    ShotInfo shotInfo = new ShotInfo(HostShip, ship, Combat.ChosenWeapon);
                    if (shotInfo.InArc) friendlies++;
                }
            }

            foreach (var bombHolder in BombsManager.GetBombsOnBoard())
            {
                if (Tools.IsSameTeam(bombHolder.Value.HostShip, HostShip))
                {
                    if (BombsManager.IsDeviceInArc(HostShip, bombHolder.Key, Combat.ArcForShot, Combat.ChosenWeapon))
                    {
                        friendlies++;
                    }
                }
            }
            return friendlies;
        }
    }
}
