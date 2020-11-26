using Editions;
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
            DeleteOldImages();
            CustomMigration();

            PlayerPrefs.SetInt("LastMigrationVersion", Version);
        }

        private void DeleteOldImages()
        {
            if (PilotsToDeleteFromImageCache == null) return;

            foreach (var pilot in PilotsToDeleteFromImageCache)
            {
                Edition.Current = (Edition) Activator.CreateInstance(pilot.RuleType);

                GenericShip ship = (GenericShip)Activator.CreateInstance(pilot.CardType);

                ImageManager.DeleteCachedImage(ship.ImageUrl, pilot.RuleType);
            }
        }

        protected virtual void CustomMigration() { }
    }
}