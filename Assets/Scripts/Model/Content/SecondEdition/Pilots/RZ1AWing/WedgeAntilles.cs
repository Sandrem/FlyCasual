using Conditions;
using Mods.ModsList;
using Ship;
using System;
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
                PilotInfo = new PilotCardInfo(
                    "Wedge Antilles",
                    4,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WedgeAntillesAWingAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Talent },
                    abilityText: "While you perform a primary attack, if the defender is your front arc. The defender rolls 1 fewer defense die."
                );

                RequiredMods = new List<Type>() { typeof(UnreleasedContentMod) };
                PilotNameCanonical = "wedgeantilles-rz1awing";

                ModelInfo.SkinName = "Phoenix Squadron";
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