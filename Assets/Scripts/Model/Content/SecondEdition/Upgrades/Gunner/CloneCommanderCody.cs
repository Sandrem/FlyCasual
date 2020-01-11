using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class CloneCommanderCody : GenericUpgrade
    {
        public CloneCommanderCody() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
				"Clone Commander Cody",
                UpgradeType.Gunner,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                abilityType: typeof(Abilities.SecondEdition.CloneCommanderCodyAbility)
            );

            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/d7/Swz33_cody-upgrade.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an attack that missed, if 1 or more hit/crit results were neutralized, the defender gains 1 strain token.
    public class CloneCommanderCodyAbility : GenericAbility
    {
        int hitOrCritResults;

        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += SaveHitOrCritResults;
            HostShip.OnAttackMissedAsAttacker += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= SaveHitOrCritResults;
            HostShip.OnAttackMissedAsAttacker -= RegisterAbility;
        }

        private void SaveHitOrCritResults()
        {
            hitOrCritResults = Combat.DiceRollAttack.Successes;
        }

        private void RegisterAbility()
        {
            if (hitOrCritResults > 0)
            {
                HostShip.OnAttackFinish += RegisterTrigger;
            }
        }

        private void RegisterTrigger(GenericShip ship)
        {
            HostShip.OnAttackFinish -= RegisterTrigger;
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AssignStrainToDefender);
        }

        private void AssignStrainToDefender(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " assigned a strain token to " + Combat.Defender.PilotInfo.PilotName);
            Combat.Defender.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
        }
    }
}