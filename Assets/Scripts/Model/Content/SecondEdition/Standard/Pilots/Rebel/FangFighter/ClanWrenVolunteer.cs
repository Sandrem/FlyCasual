using BoardTools;
using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class ClanWrenVolunteer : FangFighter
        {
            public ClanWrenVolunteer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Clan Wren Volunteer",
                    "Unlikely Ally",
                    Faction.Rebel,
                    3,
                    5,
                    10,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.ClanWrenVolunteerAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Clan Wren Volunteers"
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/clanwrenvolunteer.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ClanWrenVolunteerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (Combat.ShotInfo.Range != 1) return false;

            if (!Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(1, 1), Team.Type.Friendly).Any(s => s.RevealedManeuver.Speed == HostShip.RevealedManeuver.Speed)) return false;

            return result;
        }

        private int GetDiceModificationAiPriority()
        {
            return 90;
        }
    }
}