namespace Mods
{
    namespace ModsList
    {
        public class FirstEditionPilotsMod : Mod
        {
            public FirstEditionPilotsMod()
            {
                Name = "First Edition Pilots";
                Description = "Tycho Celchu as Second Edition pilot";
                EditionType = typeof(Editions.SecondEdition);
            }
        }
    }
}
