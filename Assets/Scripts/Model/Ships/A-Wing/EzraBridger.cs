namespace Ship
{
    namespace AWing
    {
        public class EzraBridger : AWing
        {
            public EzraBridger() : base()
            {
                PilotName = "Ezra Bridger";
                PilotSkill = 4;
                Cost = 22;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Blue";

                PilotAbilities.Add(new PilotAbilitiesNamespace.EzraBridgerPilotAbility());
            }
        }
    }
}
