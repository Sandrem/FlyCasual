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
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                CanPerformActionsWhileStressed = true;
            }
        }
    }
}
