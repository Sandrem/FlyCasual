using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class LieutenantKarsabi : AlphaClassStarWing
        {
            public LieutenantKarsabi() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Karsabi",
                    3,
                    39,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.LieutenantKarsabiAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 136;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class LieutenantKarsabiAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterLieutenantKarsabiAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterLieutenantKarsabiAbility;
        }

        private void RegisterLieutenantKarsabiAbility(Ship.GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(WeaponsDisabledToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, CheckStress);
            }
        }

        private void CheckStress(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(AlwaysUseByDefault, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            HostShip.Tokens.RemoveToken(
                typeof(WeaponsDisabledToken),
                delegate
                {
                    HostShip.Tokens.AssignToken(typeof(StressToken), DecisionSubPhase.ConfirmDecision);
                }
            );
        }
    }
}
