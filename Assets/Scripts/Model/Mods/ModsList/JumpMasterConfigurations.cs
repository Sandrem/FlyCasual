namespace Mods
{
    namespace ModsList
    {
        public class JumpMasterConfigurationsMod : Mod
        {
            public JumpMasterConfigurationsMod()
            {
                Name = "JumpMaster Configurations";
                Description = "Combat Configuration that adds front-facing primary weapon and decreases difficulty of right side maneuvers." +
                    "\nTransport Configuration that adds white barrel roll and decreases difficulty of left side maneuvers." +
                    "\n(Idea by 5050saint)";

                EditionType = typeof(Editions.SecondEdition);
            }
        }
    }
}
