using Upgrade;
using System.Collections.Generic;
using Ship;

namespace UpgradesList.FirstEdition
{
    public class Outmaneuver : GenericUpgrade
    {
        public Outmaneuver() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Outmaneuver",
                UpgradeType.Talent,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.OutmaneuverAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class OutmaneuverAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (AbilityCanBeUsed()) ApplyAbility();
        }

        private void ApplyAbility()
        {
            if (Combat.Defender.State.Agility != 0)
            {
                Messages.ShowInfo(Combat.Attacker.PilotInfo.PilotName + " Outmaneuvered " + Combat.Defender.PilotInfo.PilotName + ", decreasing their agility by 1");
                Conditions.OutmaneuverCondition condition = new Conditions.OutmaneuverCondition(HostShip, HostUpgrade);
                //condition.Upgrade = HostUpgrade;
                Combat.Defender.Tokens.AssignCondition(condition);
                Combat.Defender.ChangeAgilityBy(-1);
                Combat.Defender.OnAttackFinish += RemoveOutmaneuverAbility;
            }
        }

        public void RemoveOutmaneuverAbility(GenericShip ship)
        {
            Messages.ShowInfo("Outmaneuver: " + Combat.Defender.PilotInfo.PilotName + "'s agility is restored");
            Combat.Defender.Tokens.RemoveCondition(typeof(Conditions.OutmaneuverCondition));
            ship.ChangeAgilityBy(+1);
            ship.OnAttackFinish -= RemoveOutmaneuverAbility;
        }

        protected virtual bool AbilityCanBeUsed()
        {
            if (!Combat.ShotInfo.InArc) return false;

            BoardTools.ShotInfo reverseShotInfo = new BoardTools.ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapons);
            if (reverseShotInfo.InArc) return false;

            return true;
        }
    }
}

namespace Conditions
{
    public class OutmaneuverCondition : Tokens.GenericToken
    {
        public GenericUpgrade Upgrade;

        public OutmaneuverCondition(GenericShip host, GenericUpgrade upgrade) : base(host)
        {
            Upgrade = upgrade;
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = Upgrade.ImageUrl;
        }
    }
}