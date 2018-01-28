using System;
using Upgrade;
using Ship;
using Abilities;
using Tokens;

namespace UpgradesList
{
    public class LattsRazzi : GenericUpgrade
    {
        public LattsRazzi() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Latts Razzi";
            Cost = 2;

            isUnique = true;

            UpgradeAbilities.Add(new LattsRazziCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }

    }
}

namespace Abilities
{
    public class LattsRazziCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddLattsRazziDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddLattsRazziDiceModification;
        }

        private void AddLattsRazziDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.LattsRazziDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            host.AddAvailableActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class LattsRazziDiceModification : GenericAction
    {

        public LattsRazziDiceModification()
        {
            Name = EffectName = "Latts Razzi";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Messages.ShowInfo("Latts Razzi: Stress token is removed from the attacker to add 1 Evade result");

            Combat.CurrentDiceRoll.ApplyEvade();

            Combat.Attacker.RemoveToken(
                typeof(StressToken),
                callBack
            );
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.HasToken(typeof(StressToken)))
            {
                result = true;
            }

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.HasToken(typeof(StressToken)))
            {
                int attackSuccesses = Combat.DiceRollAttack.Successes;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccesses > defenceSuccesses)
                {
                    result = (attackSuccesses - defenceSuccesses == 1) ? 65 : 15;
                }
            }

            return result;
        }

    }
}
