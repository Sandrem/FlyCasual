﻿using UnityEngine;
using Ship;
using UnityEngine.UI;

namespace SubPhases
{

    public class NotificationSubPhase : GenericSubPhase
    {
        public string TextToShow;

        public override void Start()
        {
            Roster.HighlightOfPlayersTurnOff();

            Name = "Notification";
            IsTemporary = true;
            UpdateHelpInfo();

            FinishAfterDelay();
        }

        public void FinishAfterDelay()
        {
            if (!DebugManager.BatchAiSquadTestingModeActive)
            {
                GameObject notificationPanel = GameObject.Find("UI").transform.Find("NotificationPanel").gameObject;
                notificationPanel.GetComponentInChildren<Text>().text = TextToShow;
                notificationPanel.SetActive(true);
                notificationPanel.GetComponent<Animation>().Play();

                GameManagerScript.Wait(1.5f, Next);
            }
            else
            {
                Next();
            }
            
        }

        public override void Next()
        {
            GameObject notificationPanel = GameObject.Find("UI").transform.Find("NotificationPanel").gameObject;
            notificationPanel.SetActive(false);

            CallBack();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            return false;
        }

    }

}
