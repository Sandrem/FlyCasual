using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Mods
{
    public abstract class Mod
    {
        public string Name;
        public string Description;
        public Type EditionType = typeof(Editions.FirstEdition);

        public bool IsOn;

        public bool IsAvailable()
        {
            return IsOn && EditionType == Editions.Edition.Current.GetType();
        }
    }

    public static class ModsManager
    {
        public static Dictionary<Type, Mod> Mods;
        public static ModsUI UI;

        public static Dictionary<Type, Mod> GetAllMods()
        {
            return Mods.Where(m => m.Value.EditionType == Editions.Edition.Current.GetType()).ToDictionary(n => n.Key, n => n.Value);
        }

        public static bool IsAnyModOn
        {
            get { return Mods.Any(n => n.Value.IsOn); }
        }

        public static void Initialize()
        {
            IEnumerable<Type> namespaceIEnum =
                from types in Assembly.GetExecutingAssembly().GetTypes()
                where types.IsClass && types.Namespace == "Mods.ModsList"
                select types;

            Mods = new Dictionary<Type, Mod>();
            foreach (Type type in namespaceIEnum)
            {
                Mod newMod = (Mod)System.Activator.CreateInstance(type);
                Mods.Add(type, newMod);
            }
        }

        public static void InitializePanel()
        {
            UI.InitializePanel();
        }

        public static void ModToggleIsActive(string modTypeName, bool value)
        {
            Type modType = Type.GetType(modTypeName);
            Mods[modType].IsOn = value;
            PlayerPrefs.SetInt("mods/" + modTypeName, (value == true) ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
