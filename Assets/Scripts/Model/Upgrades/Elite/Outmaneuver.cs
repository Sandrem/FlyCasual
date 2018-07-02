using Ship;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class Outmaneuver : GenericUpgrade, ISecondEditionUpgrade
    {
        public Outmaneuver() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Outmaneuver";
            Cost = 10;

            UpgradeAbilities.Add(new OutmaneuverAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            ImageUrl = "https://i.imgur.com/LtA5WV3.png";

            UpgradeAbilities.RemoveAll(a => a is OutmaneuverAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.OutmaneuverAbilitySE());
        }
    }
}

namespace Abilities
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
            if (Combat.Defender.Agility != 0)
            {
                Messages.ShowError("Outmaneuver: Agility is decreased");
                Combat.Defender.Tokens.AssignCondition(typeof(Conditions.OutmaneuverCondition));
                Combat.Defender.ChangeAgilityBy(-1);
                Combat.Defender.OnAttackFinish += RemoveOutmaneuverAbility;
            }
        }

        public void RemoveOutmaneuverAbility(GenericShip ship)
        {
            Messages.ShowInfo("Agility is restored");
            Combat.Defender.Tokens.RemoveCondition(typeof(Conditions.OutmaneuverCondition));
            ship.ChangeAgilityBy(+1);
            ship.OnAttackFinish -= RemoveOutmaneuverAbility;
        }

        protected virtual bool AbilityCanBeUsed()
        {
            if (!Combat.ShotInfo.InArc) return false;

            BoardTools.ShotInfo reverseShotInfo = new BoardTools.ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
            if (reverseShotInfo.InArc) return false;

            return true;
        }
    }

    namespace SecondEdition
    {
        public class OutmaneuverAbilitySE : OutmaneuverAbility
        {
            protected override bool AbilityCanBeUsed()
            {
                if (!Combat.ShotInfo.InPrimaryArc) return false;

                BoardTools.ShotInfo reverseShotInfo = new BoardTools.ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                if (reverseShotInfo.InArc) return false;

                return true;
            }
        }
    }
}

namespace Conditions
{
    public class OutmaneuverCondition : Tokens.GenericToken
    {
        public OutmaneuverCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
            GenericUpgrade upgrade = new UpgradesList.Outmaneuver();
            RuleSet.Instance.AdaptUpgradeToRules(upgrade);
            Tooltip = upgrade.ImageUrl;
        }
    }
}