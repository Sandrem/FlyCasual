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

                PilotAbilities.Add(new PilotAbilitiesNamespace.TychoCelchuAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class TychoCelchuAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.CanPerformActionsWhileStressed = true;
        }
    }
}
