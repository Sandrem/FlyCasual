using Upgrade;

namespace Ship
{
    namespace TIEBomber
    {
        public class TomaxBren : TIEBomber
        {
            public TomaxBren() : base()
            {
                PilotName = "Tomax Bren";
                PilotSkill = 8;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.TomaxBrenAbility());
            }
        }
    }
}

namespace Abilities
{
    public class TomaxBrenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAfterDiscardUpgrade += CheckTomaxBrenAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAfterDiscardUpgrade -= CheckTomaxBrenAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckTomaxBrenAbility(GenericUpgrade upgrade)
        {            
            if (!IsAbilityUsed && upgrade.hasType(UpgradeType.Elite))
            {
                IsAbilityUsed = true;
                Messages.ShowInfo(string.Format("{0} flips {1} face up.", HostShip.PilotName, upgrade.Name));
                RegisterAbilityTrigger(TriggerTypes.OnAfterDiscard, (s, e) => upgrade.TryFlipFaceUp(Triggers.FinishTrigger));
            }
        }                 
    }
}
