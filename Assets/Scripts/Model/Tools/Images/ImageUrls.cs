using System;
using Upgrade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuleSets;
using Ship;

public static class ImageUrls
{
    //Old path, will be changed to value from RuleSet
    private const string RootURL = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/";

    private const string UpgradesPath = "upgrades/";
    private const string DamageDeckPath = "damage-decks/core-tfa/";
    private const string PilotsPath = "pilots/";

    public static string GetImageUrl(GenericUpgrade upgrade, string filename = null)
    {
        return Edition.Instance.GetUpgradeImageUrl(upgrade);
    }

    public static string GetImageUrlOld(GenericUpgrade upgrade, string filename = null)
    {
        return GetImageUrlOld(UpgradesPath + FormatUpgradeTypes(upgrade.UpgradeInfo.UpgradeTypes), FormatUpgradeName(upgrade.UpgradeInfo.Name), filename);
    }

    public static string GetImageUrl(GenericDamageCard crit, string filename = null)
    {
        return GetImageUrlOld(DamageDeckPath, crit.Name, filename);
    }

    public static string GetImageUrl(GenericShip ship, string filename = null)
    {
        return Edition.Instance.GetPilotImageUrl(ship, filename);
    }

    private static string GetImageUrl(string subpath, string cardName, string filename)
    {
        return Edition.Instance.RootUrlForImages + subpath + "/" + (filename ?? FormatName(cardName) + ".png");
    }

    private static string GetImageUrlOld(string subpath, string cardName, string filename)
    {
        return RootURL + subpath + "/" + (filename ?? FormatName(cardName) + ".png");
    }

    public static string FormatShipType(string type)
    {
        return type
            .Replace("-Wing", "-wing")
            .Replace("/FO", "/fo")
            .Replace("TIE/SF", "TIE/sf")
            .Replace('/', '-');
    }

    public static string FormatUpgradeTypes(List<UpgradeType> types)
    {
        string name = "";
        UpgradeType type = types [0];
        switch (type)
        {
            case UpgradeType.SalvagedAstromech:
                name += "Salvaged Astromech";
                break;
            default:
                name += type.ToString ();
                break;
        }
        return name;
    }

    public static string FormatUpgradeName(string upgradeName)
    {
        return upgradeName.Replace('.', ' ');
    }

    public static string FormatFaction(SubFaction faction)
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

    public static string FormatName(string name)
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

