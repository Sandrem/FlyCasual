namespace Mods
{
    namespace ModsList
    {
        public class UnreleasedContentMod : Mod
        {
            public UnreleasedContentMod()
            {
                Name = "Unreleased Content";
                Description = "Unreleased content from Wave 3.\nWARNING: Cost of cards and available upgrade slots are not spoiled by FFG yet - so approximate values are set.";
                EditionType = typeof(Editions.SecondEdition);
            }
        }
    }
}
