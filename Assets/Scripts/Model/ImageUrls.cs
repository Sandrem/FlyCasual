using System;

public static class ImageUrls
{
    private const string RootURL = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/";
    private const string UpgradesPath = "upgrades/";
    private const string DamageDeckPath = "damage-decks/core-tfa/";
    private const string PilotsPath = "pilots/";

    public static string GetImageUrl(Upgrade.GenericUpgrade upgrade, string filename = null)
    {
        return GetImageUrl(UpgradesPath + upgrade.Type.ToString(), upgrade.Name, filename);
    }

    public static string GetImageUrl(CriticalHitCard.GenericCriticalHit crit, string filename = null)
    {
        return GetImageUrl(DamageDeckPath, crit.Name, filename);
    }

    public static string GetImageUrl(Ship.GenericShip ship, string filename = null)
    {
        return GetImageUrl(PilotsPath + FormatFaction(ship.faction) + "/" + ship.Type, ship.PilotName, filename);
    }

    private static string GetImageUrl(string subpath, string cardName, string filename)
    {
        return RootURL + subpath + "/" + (filename ?? FormatName(cardName) + ".png");
    }

    private static string FormatFaction(Faction faction)
    {
        switch (faction)
        {
            case Faction.Rebels:
                return "Rebel Alliance";
            case Faction.Empire:
                return "Galactic Empire";
            default:
                throw new NotImplementedException();
        }
    }

    private static string FormatName(string name)
    {
        return name
            .ToLower()
            .Replace(' ', '-')
            .Replace('/', '-');
    }
}

