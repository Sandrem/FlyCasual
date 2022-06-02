using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class JophSeastriker : T70XWing
        {
            public JophSeastriker() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Joph Seastriker",
                    "Reckless Bodyguard",
                    Faction.Resistance,
                    3,
                    5,
                    9,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JophSeastrikerAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/90/fc/90fc30fb-db99-46cb-8761-89b6536286eb/swz25_joph_a1.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JophSeastrikerAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnShieldLost += RegisterJophSeastrikerAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShieldLost -= RegisterJophSeastrikerAbility;
        }

        private void RegisterJophSeastrikerAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnShieldIsLost, GetEvadeToken);
        }

        private void GetEvadeToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " gains Evade token");
            HostShip.Tokens.AssignToken(typeof(Tokens.EvadeToken), Triggers.FinishTrigger);
        }
    }
}