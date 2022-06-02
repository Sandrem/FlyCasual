using ActionsList;
using SubPhases;
using System;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BazeMalbus : GenericUpgrade
    {
        public static readonly int MaximumAdditionalFocusTokens = 2;
        public static readonly int MaxAbilityRange = 1;

        public BazeMalbus() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Baze Malbus",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.BazeMalbusCrewAbility),
                seImageNumber: 79
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(427, 1),
                new Vector2(150, 150)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BazeMalbusCrewAbility : GenericAbility
    {
        public int enemyShipsWithinRange;

        public override void ActivateAbility()
        {
            HostShip.BeforeActionIsPerformed += RegisterBazeAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeActionIsPerformed -= RegisterBazeAbility;
        }

        private void RegisterBazeAbility(GenericAction action, ref bool isFree)
        {
            if (action is FocusAction)
                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskToUseBazeAbility);
        }

        private void AskToUseBazeAbility(object sender, EventArgs e)
        {
            enemyShipsWithinRange = GetEnemyShipsWithinRange();
            Func<bool> useAbility = NeverUseByDefault;
            if (enemyShipsWithinRange > 0)
            {
                useAbility = AlwaysUseByDefault;
            }

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                useAbility,
                UseBazeMalbusAbility,
                descriptionLong: "Do you want to treat action as red? If you do, gain 1 additional focus token for each enemy ship at range 0-1, to a maximum of 2.",
                imageHolder: HostUpgrade
            );
        }

        private void UseBazeMalbusAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            var additionalFocusTokens = (enemyShipsWithinRange > UpgradesList.SecondEdition.BazeMalbus.MaximumAdditionalFocusTokens ?
                UpgradesList.SecondEdition.BazeMalbus.MaximumAdditionalFocusTokens : enemyShipsWithinRange);

            if (enemyShipsWithinRange > 0) {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " caused " + HostShip.PilotInfo.PilotName + " to gain " + additionalFocusTokens + " additional Focus token(s)");
            }

            HostShip.OnCheckActionComplexity += TreatActionAsRed;

            HostShip.Tokens.AssignTokens(
                () => new FocusToken(HostShip),
                additionalFocusTokens,
                Triggers.FinishTrigger
            );
        }

        private void TreatActionAsRed(GenericAction action, ref Actions.ActionColor color)
        {
            if (action is FocusAction)
            {
                HostShip.OnCheckActionComplexity -= TreatActionAsRed;
                color = Actions.ActionColor.Red;
            }
        }

        private int GetEnemyShipsWithinRange()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, 
                new UnityEngine.Vector2(0, UpgradesList.SecondEdition.BazeMalbus.MaxAbilityRange), 
                Team.Type.Enemy).Count;
        }
    }
}
