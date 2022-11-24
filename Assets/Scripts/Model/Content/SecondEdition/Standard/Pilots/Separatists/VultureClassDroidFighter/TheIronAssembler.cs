using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class TheIronAssembler : VultureClassDroidFighter
    {
        public TheIronAssembler()
        {
            IsWIP = true;

            PilotInfo = new PilotCardInfo25
            (
                "The Iron Assembler",
                "Scintilla Scavenger",
                Faction.Separatists,
                1,
                2,
                5,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.TheIronAssemblerAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/b03bf8e3-1596-4e31-9220-ea5511043a11/SWZ97_TheIronAssemblerlegal.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TheIronAssemblerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
