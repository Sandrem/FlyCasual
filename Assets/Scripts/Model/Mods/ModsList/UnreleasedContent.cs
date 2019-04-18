namespace Mods
{
    namespace ModsList
    {
        public class UnreleasedContentMod : Mod
        {
            public UnreleasedContentMod()
            {
                Name = "Unreleased Content";
                Description = "Unreleased content from future releases.\nWARNING: Cost of cards and available upgrade slots may be based on speculation and guesses.";
                EditionType = typeof(Editions.SecondEdition);
            }
        }
    }
}
