using System;
using Upgrade;

public static class ImageUrls
{
    private const string RootURL = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/";
    private const string UpgradesPath = "upgrades/";
    private const string DamageDeckPath = "damage-decks/core-tfa/";
    private const string PilotsPath = "pilots/";

    public static string GetImageUrl(GenericUpgrade upgrade, string filename = null)
    {
        return GetImageUrl(UpgradesPath + FormatUpgradeType(upgrade.Type), FormatUpgradeName(upgrade.Name), filename);
    }

    public static string GetImageUrl(CriticalHitCard.GenericCriticalHit crit, string filename = null)
    {
        return GetImageUrl(DamageDeckPath, crit.Name, filename);
    }

    public static string GetImageUrl(Ship.GenericShip ship, string filename = null)
    {
        return GetImageUrl(PilotsPath + FormatFaction(ship.SubFaction) + "/" + FormatShipType(ship.Type), ship.PilotName, filename);
    }

    private static string GetImageUrl(string subpath, string cardName, string filename)
    {
        return RootURL + subpath + "/" + (filename ?? FormatName(cardName) + ".png");
    }

    private static string FormatShipType(string type)
    {
        return type
            .Replace("-Wing", "-wing")
            .Replace("/FO", "/fo")
            .Replace("/SF", "/sf")
            .Replace('/', '-');
    }

    private static string FormatUpgradeType(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.SalvagedAstromech:
                return "Salvaged Astromech";
            default:
                return type.ToString();
        }
    }

    private static string FormatUpgradeName(string upgradeName)
    {
        return upgradeName.Replace('.', ' ');
    }

    private static string FormatFaction(SubFaction faction)
    {
        switch (faction)
        {
            case SubFaction.RebelAlliance:
                return "Rebel Alliance";
            case SubFaction.Resistance:
                return "Resistance";
            case SubFaction.GalacticEmpire:
                return "Galactic Empire";
            case SubFaction.FirstOrder:
                return "First Order";
            case SubFaction.ScumAndVillainy:
                return "Scum and Villainy";
            default:
                throw new NotImplementedException();
        }
    }

    private static string FormatName(string name)
    {
        return name
            .ToLower()
            .Replace("(", "")
            .Replace(")", "")
            .Replace(' ', '-')
            .Replace('/', '-')
            .Replace('\'', '-')
            .Replace("\"", "");
    }
}

