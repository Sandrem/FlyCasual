namespace Ship
{
    namespace AWing
    {
        public class TychoCelchu : AWing
        {
            public TychoCelchu() : base()
            {
                PilotName = "Tycho Celchu";
                PilotSkill = 8;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.TychoCelchuAbility());
            }
        }
    }
}

namespace Abilities
{
    public class TychoCelchuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanPerformActionsWhileStressed = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanPerformActionsWhileStressed = false;
        }
    }
}
