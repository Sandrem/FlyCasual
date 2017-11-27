using Mods.ModsList;

namespace Ship
{
    namespace AWing
    {
        public class EzraBridger : AWing
        {
            public EzraBridger() : base()
            {
                FromMod = typeof(PhoenixSquadronMod);

                PilotName = "Ezra Bridger";
                PilotSkill = 4;
                Cost = 22;

                ImageUrl = "https://i.imgur.com/xPe8HQo.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Blue";

                PilotAbilities.Add(new PilotAbilitiesNamespace.EzraBridgerPilotAbility());
            }
        }
    }
}
