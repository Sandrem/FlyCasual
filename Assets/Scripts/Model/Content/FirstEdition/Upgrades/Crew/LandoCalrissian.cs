using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class LandoCalrissian : GenericUpgrade
    {
        public LandoCalrissian() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lando Calrissian",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.LandoCalrissianCrewAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    //ACTION: Roll 2 defense dice. For each focus result, assign 1 focus token to your ship. For each evade result, assign 1 evade token to your ship.
    public class LandoCalrissianCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddAction;
        }

        protected void AddAction(Ship.GenericShip ship)
        {
            ship.AddAvailableAction(new ActionsList.GenericAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip,
                Source = HostUpgrade,
                DoAction = DoAction,
                Name = HostName
            });
        }

        protected void DoAction()
        {
            PerformDiceCheck(
                HostName,
                DiceKind.Defence,
                2,
                DiceCheckFinished,
                Phases.CurrentSubPhase.CallBack);
        }

        protected virtual void DiceCheckFinished()
        {
            HostShip.Tokens.AssignTokens(() => new Tokens.FocusToken(HostShip), DiceCheckRoll.Focuses, () => {
                HostShip.Tokens.AssignTokens(() => new Tokens.EvadeToken(HostShip), DiceCheckRoll.Successes, () =>
                {
                    AbilityDiceCheck.ConfirmCheck();
                });
            });
        }
    }
}