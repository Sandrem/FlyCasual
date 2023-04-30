using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class CaptainPhasma : TIESfFighter
        {
            public CaptainPhasma() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Phasma",
                    "Scyre Survivor",
                    Faction.FirstOrder,
                    4,
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainPhasmaPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you defend, after the Neutralize Results step, another friendly ship at range 0-1 must suffer
    // 1 hit/crit damage to cancel 1 matching result.

    public class CaptainPhasmaPilotAbility : Abilities.SecondEdition.PrinceXizorAbility
    {
        protected override bool CheckInArcRequirements(GenericShip ship)
        {
            // No arc requirement
            return true;
        }

        protected override bool IsShowSkipButton()
        {
            // Cannot be skipped
            return false;
        }
    }
}
