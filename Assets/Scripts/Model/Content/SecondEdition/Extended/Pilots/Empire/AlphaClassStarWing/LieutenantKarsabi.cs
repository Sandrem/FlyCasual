using Content;
using SubPhases;
using System.Collections.Generic;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Karsabi",
                    "Brash Noble",
                    Faction.Imperial,
                    3,
                    5,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantKarsabiAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Missile,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    seImageNumber: 136,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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

        private void RegisterLieutenantKarsabiAbility(Ship.GenericShip ship, GenericToken token)
        {
            if (token is WeaponsDisabledToken)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, CheckStress);
            }
        }

        private void CheckStress(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want to receive 1 Stress Token to remove Disarm Token?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " recieved Stress token instead of Disarm Token");

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
