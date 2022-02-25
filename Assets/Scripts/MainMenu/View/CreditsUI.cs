using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CreditsUI {

    private static readonly float LEGAL_INFO_SIZE = 300f;
    private static readonly float FREE_SPACE_MEDIUM = 25f;
    private static readonly float FREE_SPACE_SMALL = 10f;

    private static readonly Dictionary<string, string> CreditsData = new Dictionary<string, string>()
    {
        { "Programming", "Sandrem, Rune-b, Azrapse, Matthew Blanchard, GeneralVryth, bwakefield27, thordurk91, Conzar, deakolt, Galaxy613, TheChiefMoose, YannickBgn, Evan Lundell, Fabio Macal, SirHaplo, Nick Buonarota (NickyDaB), rats3g, tetrarchangel, Andrew Matheny, Jimmy5910, jamorbal, pappnase99, Stephen Wangner, jychuah, jtg15, vladamex, xNyer, Ryan Praska, savagerose, Krumok, Tron, mikethetike, tinydantser, belk, XPav, DejaFu, bitsai, joshhightower, Andrew Nickell, barraudl, randy-ja, simonthezealot, ManuM, ersk, Aaron Chapin" },
        { "3D Art", "TerranCmdr, Izaak94, Darksaber, DTM, Warb Null, Rob, Gambler, Nim the Netrunner, Gremlyn, Morvana, FabioOtto, Grediel, Cherrybomb67, Smight, Voiceoftheforce, flipdark95, JonnieLP, Atherin, Captain Tributon, JackJohn2942, danielnavax90s, abdyla, gregkash, Kes, The Client, worstcoastchild, Sinslith101" },
        { "2D Art", "xwingtmgphotography, Matthew Cohen, Brimhorn Battalion, Andreas Bazylewsi, Andrew McCarthy, Odanan, Andrew Lippens, T1TAN, Davish_Krail, edartu, BuffaloChicken, mephiston_x" },
        { "Music", "Daniel (Affordable Audio 4 Everyone), Robson Cozendey" },
        { "Fonts", "Pixel Sagas \"Rebellion\", \"Strike Fighter\" (Daniel Zadorozny, Iconian Fonts), xwing-miniatures-font (geordanr)" },
        { "Additional assets", "Skybox Volume 2: Nebula (Hedgehog Team), Simple Table Glass (Quadrante Studio), sFuture Modules PRO Space and Ground (Sami Lehtonen), Sci Fi Doors (MASH Virtual)" },
        { "Testing", "djeknemesis2012, LiquidLogic, doji" },
        { "Thanks for support", "Andy Selby, Emmanuel Broto, Phillip Manwaring, Arik Roshanzamir, Andrew Ballentine, Kent, Scott Milam," +
            " John T. Harrison IV, Rick Servello, Spencer Loper, Landon, Dual Torpedoes, Vittorio Rosa, GregFromParis#42, Charlie King," +
            " Fabian Falconett, Julian Munoz Bermejo, Benny Tsai, Ignatius Bug, John Snape, Bryan Froh, Ross Kushnereit, David Gausebeck," +
            " Alan Michael, Brian Hall, Byron Harder, shaun p kelleher, James Campbell, Willy Fortin, Spqan x, Shawn Mason," +
            " X-Wing Tavern Wars, Phillip Manwaring, Andrew Ballentine, Kevin Vu, Mark Dyson, Ryan Fisher, Blarghle Hargle," +
            " indianajonesey, Maxwell Hathaway, Jozeph, Ted Savage, KEN0Bl, Matthew Roesener" },
    };

    public static void InitializePanel()
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/MainMenu/CreditsBlock", typeof(GameObject));
        GameObject CreditsPanel = GameObject.Find("UI/Panels").transform.Find("CreditsPanel").Find("Scroll View/Viewport/Content").gameObject;

        float verticalSpace = -LEGAL_INFO_SIZE;

        foreach (Transform transform in CreditsPanel.transform)
        {
            if (transform.gameObject.name == "CreditsBlock") MonoBehaviour.Destroy(transform.gameObject);
        }

        GameObject.Find("UI/Panels").transform.Find("ModsPanel").Find("Scroll View").GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;

        foreach (var creditsRecord in CreditsData)
        {
            GameObject CreditsBlock;

            CreditsBlock = MonoBehaviour.Instantiate(prefab, CreditsPanel.transform);
            CreditsBlock.transform.localPosition = new Vector3(CreditsBlock.transform.localPosition.x, verticalSpace, CreditsBlock.transform.localPosition.z);
            CreditsBlock.name = "CreditsBlock";

            CreditsBlock.transform.Find("CreditsCategory").GetComponent<Text>().text = creditsRecord.Key;

            Text description = CreditsBlock.transform.Find("CreditsText").GetComponent<Text>();
            description.text = creditsRecord.Value;
            RectTransform descriptionRectTransform = description.GetComponent<RectTransform>();
            descriptionRectTransform.sizeDelta = new Vector2(descriptionRectTransform.sizeDelta.x, description.preferredHeight);

            RectTransform creditsBlockTransform = CreditsBlock.GetComponent<RectTransform>();
            creditsBlockTransform.sizeDelta = new Vector2(creditsBlockTransform.sizeDelta.x, 2 * FREE_SPACE_MEDIUM + description.preferredHeight);

            verticalSpace -= CreditsBlock.GetComponent<RectTransform>().sizeDelta.y + FREE_SPACE_SMALL;
        }

        CreditsPanel.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Mathf.Abs(verticalSpace));
    }

}
