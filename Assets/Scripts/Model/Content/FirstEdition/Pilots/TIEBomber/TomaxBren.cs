using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEBomber
    {
        public class TomaxBren : TIEBomber
        {
            public TomaxBren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tomax Bren",
                    8,
                    24,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TomaxBrenAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TomaxBrenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAfterDiscardUpgrade += CheckTomaxBrenAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAfterDiscardUpgrade -= CheckTomaxBrenAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckTomaxBrenAbility(GenericUpgrade upgrade)
        {
            if (!IsAbilityUsed && upgrade.HasType(UpgradeType.Talent))
            {
                IsAbilityUsed = true;
                Messages.ShowInfo(string.Format("{0} flips {1} face up.", HostShip.PilotInfo.PilotName, upgrade.UpgradeInfo.Name));
                RegisterAbilityTrigger(TriggerTypes.OnAfterDiscard, (s, e) => upgrade.TryFlipFaceUp(Triggers.FinishTrigger));
            }
        }
    }
}