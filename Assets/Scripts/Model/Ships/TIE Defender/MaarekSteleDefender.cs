namespace Ship
{
    namespace TIEDefender
    {
        public class MaarekSteleDefender : TIEDefender
        {
            public MaarekSteleDefender() : base()
            {
                PilotName = "Maarek Stele";
                PilotSkill = 7;
                Cost = 35;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.MaarekSteleAbility());
            }
        }
    }
}