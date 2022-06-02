using Conditions;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class WedgeAntilles : RZ1AWing
        {
            public WedgeAntilles() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Wedge Antilles",
                    "Promising Pilot",
                    Faction.Rebel,
                    4,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WedgeAntillesAWingAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    abilityText: "While you perform a primary attack, if the defender is your front arc. The defender rolls 1 fewer defense die.",
                    skinName: "Blue"
                );

                PilotNameCanonical = "wedgeantilles-rz1awing";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/3f/b7/3fb7e02c-21fc-4f85-bbcd-2fcf2c5efcab/swz83_pilot_wedgeantilles.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WedgeAntillesAWingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += TryAddWedgeAntillesAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= TryAddWedgeAntillesAbility;
        }

        public void TryAddWedgeAntillesAbility()
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Front))
                {
                    WedgeAntillesCondition condition = new WedgeAntillesCondition(Combat.Defender, HostShip);
                    Combat.Defender.Tokens.AssignCondition(condition);
                }
            }
        }
    }
}