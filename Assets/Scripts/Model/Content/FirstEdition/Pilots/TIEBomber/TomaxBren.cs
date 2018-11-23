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
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.TomaxBrenAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
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
            if (!IsAbilityUsed && upgrade.HasType(UpgradeType.Elite))
            {
                IsAbilityUsed = true;
                Messages.ShowInfo(string.Format("{0} flips {1} face up.", HostShip.PilotName, upgrade.UpgradeInfo.Name));
                RegisterAbilityTrigger(TriggerTypes.OnAfterDiscard, (s, e) => upgrade.TryFlipFaceUp(Triggers.FinishTrigger));
            }
        }
    }
}