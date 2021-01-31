using Editions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SquadBuilderNS
{
    public interface IRosterViewPanelInfo
    {

    }

    public class AddShipPanelInfo : IRosterViewPanelInfo
    {
        private const float PILOT_CARD_WIDTH = 300;
        private const float PILOT_CARD_HEIGHT = 418;
        public Vector2 Size = new Vector2(PILOT_CARD_WIDTH, PILOT_CARD_HEIGHT);

        public GameObject Panel;

        public AddShipPanelInfo()
        {
            GameObject addShipButton = SpawnButton();
            AddOnClickHandler(addShipButton);
        }

        private GameObject SpawnButton()
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
            Panel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel").transform);

            prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/AddShipButton", typeof(GameObject));
            GameObject addShipButton = MonoBehaviour.Instantiate(prefab, Panel.transform);

            Sprite factionSprite = GameObject.Find("UI/Panels").transform.Find("SelectFactionPanel").Find("Panel").Find("FactionPanels" + Edition.Current.NameShort).Find(Global.SquadBuilder.CurrentSquad.SquadFaction.ToString()).GetComponent<Image>().sprite;
            addShipButton.GetComponent<Image>().sprite = factionSprite;
            return addShipButton;
        }

        private static void AddOnClickHandler(GameObject addShipButton)
        {
            EventTrigger trigger = addShipButton.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { Global.SquadBuilder.View.OpenSelectShip(); });
            trigger.triggers.Add(entry);
        }
    }
}
