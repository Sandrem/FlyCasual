using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class KathScarletEmpire : Firespray31
        {
            public KathScarletEmpire() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kath Scarlet",
                    7,
                    38,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KathScarletEmpireAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    factionOverride: Faction.Imperial
                );

                ModelInfo.SkinName = "Kath Scarlet";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KathScarletEmpireAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAtLeastOneCritWasCancelledByDefender += RegisterKathScarletPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAtLeastOneCritWasCancelledByDefender -= RegisterKathScarletPilotAbility;
        }

        private void RegisterKathScarletPilotAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAtLeastOneCritWasCancelledByDefender, KathScarletPilotAbility);
        }

        private void KathScarletPilotAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Critical hit was cancelled - stress token is assigned to the defender");
            Combat.Defender.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }
    }
}
