using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
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
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.BazeMalbusCrewAbility),
                seImageNumber: 79
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
            HostShip.OnActionIsPerformed += CheckBazeAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckBazeAbility;
        }

        private void CheckBazeAbility(GenericAction action)
        {
            if(action is FocusAction)
            {
                HostShip.OnActionDecisionSubphaseEnd += RegisterBazeAbility;
            }
        }

        private void RegisterBazeAbility(GenericShip ship)
        {
            ship.OnActionDecisionSubphaseEnd -= RegisterBazeAbility;
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToUseBazeAbility);
        }

        private void AskToUseBazeAbility(object sender, EventArgs e)
        {
            enemyShipsWithinRange = GetEnemyShipsWithinRange();
            Func<bool> useAbility = NeverUseByDefault;
            if(enemyShipsWithinRange > 0)
            {
                useAbility = AlwaysUseByDefault;
            }
            AskToUseAbility(useAbility, UseBazeMalbusAbility);
        }

        private void UseBazeMalbusAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            var additionalFocusTokens = (enemyShipsWithinRange > UpgradesList.SecondEdition.BazeMalbus.MaximumAdditionalFocusTokens ?
                UpgradesList.SecondEdition.BazeMalbus.MaximumAdditionalFocusTokens : enemyShipsWithinRange);

            if (enemyShipsWithinRange > 0) {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " caused " + HostShip.PilotInfo.PilotName + " to gain " + additionalFocusTokens + " additional Focus token(s).");
            }

            HostShip.Tokens.AssignTokens(
                () => new FocusToken(HostShip),
                additionalFocusTokens,
                delegate { HostShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger); }
            );
        }

        private int GetEnemyShipsWithinRange()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, 
                new UnityEngine.Vector2(0, UpgradesList.SecondEdition.BazeMalbus.MaxAbilityRange), 
                Team.Type.Enemy).Count;
        }
    }
}
