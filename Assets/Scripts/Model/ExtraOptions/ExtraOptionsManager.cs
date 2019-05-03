using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ExtraOptions
{
    public abstract class ExtraOption
    {
        public string Name;
        public string Description;

        private bool isOn;
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                if (value) Activate(); else Deactivate();
            }
        }

        protected abstract void Activate();
        protected abstract void Deactivate();
    }

    public static class ExtraOptionsManager
    {
        public static Dictionary<Type, ExtraOption> ExtraOptions;
        public static ModsUI UI;

        public static void Initialize()
        {
            IEnumerable<Type> namespaceIEnum =
                from types in Assembly.GetExecutingAssembly().GetTypes()
                where types.IsClass && types.Namespace == "ExtraOptions.ExtraOptionsList"
                select types;

            ExtraOptions = new Dictionary<Type, ExtraOption>();
            foreach (Type type in namespaceIEnum)
            {
                ExtraOption newExtraOption = (ExtraOption)System.Activator.CreateInstance(type);
                ExtraOptions.Add(type, newExtraOption);
                ExtraOptionToggleIsActive(newExtraOption.ToString(), PlayerPrefs.GetInt("extraOptions/" + newExtraOption.ToString(), 0) == 1);
            }
        }

        public static void InitializePanel()
        {
            UI.InitializePanel();
        }

        public static void ExtraOptionToggleIsActive(string extraOptionTypeName, bool value)
        {
            Type extraOptionType = Type.GetType(extraOptionTypeName);
            ExtraOptions[extraOptionType].IsOn = value;
            PlayerPrefs.SetInt("extraOptions/" + extraOptionTypeName, (value == true) ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
