namespace Ship
{
    namespace AWing
    {
        public class TychoCelchu : AWing
        {
            public TychoCelchu() : base()
            {
                PilotName = "Tycho Celchu";
                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/a/a7/Tycho_Celchu.png";
                PilotSkill = 8;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilitiesList.Add(new PilotAbilities.TychoCelchuAbility());
            }
        }
    }
}

namespace PilotAbilities
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
