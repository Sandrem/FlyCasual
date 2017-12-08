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
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            HostShip.CanPerformActionsWhileStressed = true;
        }
    }
}
