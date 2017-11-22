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

        public bool IsActive;
    }

    public static class ModsManager
    {
        public static Dictionary<Type, Mod> Mods;
        public static ModsUI UI;

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
    }
}
