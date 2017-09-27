using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

static class DamageNumbers
{
    private struct HpInfo
    {
        public int Hull;
        public int Shields;

        public HpInfo(int hull, int shields)
        {
            Hull = hull;
            Shields = shields;
        }
    }

    private static Dictionary<string, HpInfo> SavedHP;

    public static void UpdateSavedHP()
    {
        Debug.Log("Updated");
        SavedHP = new Dictionary<string, HpInfo>();

        foreach (var shipHolder in Roster.AllShips)
        {
            SavedHP.Add(shipHolder.Key, new HpInfo(shipHolder.Value.Hull, shipHolder.Value.Shields));
        }
    }

    public static void ShowChangedHP()
    {
        Debug.Log("Show");
        foreach (var shipHolder in Roster.AllShips)
        {
            int hullChanged = SavedHP[shipHolder.Key].Hull - shipHolder.Value.Hull;
            int shieldsChanged = SavedHP[shipHolder.Key].Shields - shipHolder.Value.Shields;

            if ((hullChanged != 0) || (shieldsChanged != 0))
            {
                Debug.Log(shipHolder.Value.PilotName + " lost " + hullChanged + " hull and " + shieldsChanged + " shields");
            }
        }
    }
}
