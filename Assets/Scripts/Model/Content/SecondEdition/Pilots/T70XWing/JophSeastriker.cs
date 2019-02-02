using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class JophSeastriker : T70XWing
        {
            public JophSeastriker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Joph Seastriker",
                    3,
                    52,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JophSeastrikerAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                //seImageNumber: 93
                );

                //ModelInfo.SkinName = "Black One";

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
            HostShip.Tokens.AssignToken(typeof(Tokens.EvadeToken), Triggers.FinishTrigger);
        }
    }
}