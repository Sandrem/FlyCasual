using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Null : TIEFoFighter
        {
            public Null() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Null\"",
                    "Epsilon Ace",
                    Faction.FirstOrder,
                    0,
                    3,
                    5,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NullAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Tech
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/f/f2/TieFO_Null.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NullAbility : GenericAbility, IModifyPilotSkill
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageCardIsDealt += RemoveInitiative;
            HostShip.OnGameStart += ApplyInitiative;
        }

        public override void DeactivateAbility()
        {
            HostShip.State.RemovePilotSkillModifier(this);
        }

        private void ApplyInitiative()
        {
            HostShip.OnGameStart -= ApplyInitiative;
            HostShip.State.AddPilotSkillModifier(this);
        }

        private void RemoveInitiative(GenericShip ship)
        {
            HostShip.OnDamageCardIsDealt -= RemoveInitiative;
            HostShip.State.RemovePilotSkillModifier(this);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 7;
        }
    }
}