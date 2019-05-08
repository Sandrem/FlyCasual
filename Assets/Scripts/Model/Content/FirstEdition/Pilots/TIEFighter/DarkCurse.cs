using Ship;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class DarkCurse : TIEFighter
        {
            public DarkCurse() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dark Curse",
                    6,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.DarkCurseAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class DarkCurseAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsDefender += AddDarkCursePilotAbility;
            HostShip.OnDefenceStartAsDefender += RemoveDarkCursePilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= AddDarkCursePilotAbility;
            HostShip.OnDefenceStartAsDefender -= RemoveDarkCursePilotAbility;
        }

        private void AddDarkCursePilotAbility()
        {
            if (Combat.AttackStep == CombatStep.Attack)
            {
                Combat.Attacker.OnTryAddAvailableDiceModification += UseDarkCurseRestriction;
                Combat.Attacker.Tokens.AssignCondition(typeof(Conditions.DarkCurseCondition));
            }
        }

        private void UseDarkCurseRestriction(GenericShip ship, ActionsList.GenericAction action, ref bool canBeUsed)
        {
            if (action.TokensSpend.Contains(typeof(Tokens.FocusToken)))
            {
                Messages.ShowErrorToHuman("Dark Curse's Ability: The target cannot spend focus");
                canBeUsed = false;
            }
            if (action.IsReroll)
            {
                Messages.ShowErrorToHuman("Dark Curse's Ability: The target cannot reroll");
                canBeUsed = false;
            }
        }

        private void RemoveDarkCursePilotAbility()
        {
            if ((Combat.AttackStep == CombatStep.Defence) && (Combat.Defender.ShipId == HostShip.ShipId))
            {
                Combat.Attacker.OnTryAddAvailableDiceModification -= UseDarkCurseRestriction;
                Combat.Attacker.Tokens.RemoveCondition(typeof(Conditions.DarkCurseCondition));
            }
        }
    }
}

namespace Conditions
{
    public class DarkCurseCondition : Tokens.GenericToken
    {
        public DarkCurseCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new Ship.FirstEdition.TIEFighter.DarkCurse().ImageUrl;
        }
    }
}