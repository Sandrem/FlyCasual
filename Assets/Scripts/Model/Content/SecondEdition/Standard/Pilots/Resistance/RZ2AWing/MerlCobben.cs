using Arcs;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class MerlCobben : RZ2AWing
        {
            public MerlCobben() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Merl Cobben",
                    "Distracting Daredevil",
                    Faction.Resistance,
                    1,
                    4,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MerlCobbenAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    }
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/c0/Merl_cobben.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MerlCobbenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal -= CheckAbility;
        }

        private void CheckAbility(ref int defenseDiceCount)
        {
            if (AreConditionsMet())
            {
                Messages.ShowInfo("Merl Cobben: Defender rolls 1 fewer defense die");
                defenseDiceCount--;
            }
        }

        private bool AreConditionsMet()
        {
            bool result = false;

            if (Tools.IsSameTeam(Combat.Attacker, HostShip))
            {
                if (new DistanceInfo(Combat.Attacker, HostShip).Range < 3)
                {
                    if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
                    {
                        if (Combat.Defender.SectorsInfo.IsShipInSector(HostShip, ArcType.Bullseye))
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }
    }
}