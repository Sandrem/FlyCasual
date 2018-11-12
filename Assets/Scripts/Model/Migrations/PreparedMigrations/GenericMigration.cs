using RuleSets;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Migrations
{
    public class GenericMigration 
    {
        public int Version { get; protected set; }
        public List<CardInfo> PilotsToDeleteFromImageCache { get; protected set; }

        public void DoMigration()
        {
            Console.Write("Migration for " + Version + " is started");

            DeleteOldImages();

            CustomMigration();

            PlayerPrefs.SetInt("LastMigrationVersion", Version);
            Console.Write("Migration for " + Version + " is finished");
        }

        private void DeleteOldImages()
        {
            if (PilotsToDeleteFromImageCache == null) return;

            foreach (var pilot in PilotsToDeleteFromImageCache)
            {
                RuleSet.Instance = (RuleSet) Activator.CreateInstance(pilot.RuleType);

                GenericShip ship = (GenericShip)Activator.CreateInstance(pilot.CardType);
                if (pilot.RuleType == typeof(SecondEdition)) (ship as ISecondEditionPilot).AdaptPilotToSecondEdition();

                ImageManager.DeleteCachedImage(ship.ImageUrl, pilot.RuleType);
                Console.Write(ship.PilotInfo.PilotName + "'s image is deleted from image cache");
            }
        }

        protected virtual void CustomMigration() { }
    }
}