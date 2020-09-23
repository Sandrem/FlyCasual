using Upgrade;
using ActionsList;
using Actions;
using System.Collections.Generic;
using Ship;
using System;
using Tokens;
using Remote;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class TargetAssistMGK300 : GenericUpgrade
    {
        public TargetAssistMGK300() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Target-Assist MGK-300",
                UpgradeType.Configuration,
                cost: 0,
                addActions: new List<ActionInfo>()
                {
                    new ActionInfo(typeof(CalculateAction)),
                    new ActionInfo(typeof(RotateArcAction)) 
                },
                addActionLink: new LinkedActionInfo(typeof(RotateArcAction), typeof(CalculateAction), ActionColor.White),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.TIERbHeavy.TIERbHeavy)),
                abilityType: typeof(Abilities.SecondEdition.TargetAssistMGK300Ability)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c1/d7/c1d74e73-d6e7-47c5-9d2f-707d1c88cb67/swz67_target-assist_mgk300.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TargetAssistMGK300Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, CheckConditions);
        }

        private void CheckConditions(object sender, EventArgs e)
        {
            if (!HostShip.IsStressed && !HostShip.Tokens.HasGreenTokens)
            {
                int shipsToGetTokens = CountShipsToGetTokens();
                int tokensToAssign = Math.Min(shipsToGetTokens, 2);
                if (tokensToAssign > 0) Messages.ShowInfo("Target-Assist MGK-300: " + HostShip.PilotInfo.PilotName + " gains " + tokensToAssign + " Calculate token(s))");
                HostShip.Tokens.AssignTokens(CreateCalculateToken, tokensToAssign, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private int CountShipsToGetTokens()
        {
            int result = 0;

            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (enemyShip is GenericRemote) continue;

                ShotInfo shotInfo = new ShotInfo(HostShip, enemyShip, HostShip.PrimaryWeapons);
                if (shotInfo.InArc && shotInfo.Range >= 2 && shotInfo.Range <= 3)
                {
                    result++;
                }
            }

            return result;
        }

        private GenericToken CreateCalculateToken()
        {
            return new CalculateToken(HostShip);
        }
    }
}