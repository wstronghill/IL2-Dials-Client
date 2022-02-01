using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


public class LoadManager : MonoBehaviour
{
    public static void LoadLayout(AirplaneData airplaneData, DialsManager dialsManager)
    {
        MenuHandler menuHandler = GameObject.FindGameObjectWithTag("MenuObject").GetComponent<MenuHandler>();

        //first of all empty trays
        ButtonManager.EmptyTrays(menuHandler);

        //are we master client or slave 

        if(dialsManager.slaveManager.slave)
        {
            Slave();
        }
        else
        {
            MasterClient(airplaneData,dialsManager,menuHandler);
        }      
    }

    private static void MasterClient(AirplaneData airplaneData, DialsManager dialsManager, MenuHandler menuHandler)
    {
        //master client spawns other slave windows

        //get all registry keys/ where user profiles are saved
        string[] keys = GetKeys(airplaneData);
        //spawn window at position in key for all slaves

        //build layout for each key we have found
        foreach (string key in keys)
        {
            //grab layout data if available from player prefs
            string jsonFoo = PlayerPrefs.GetString(key);
            if (System.String.IsNullOrEmpty(jsonFoo))
            {
                //no save data, no extra slaves
                //set dials to default on master client and return
                DefaultLayouts(dialsManager.countryDialBoard);
                return;
            }

            //continue if there is a pref file

            //rebuild json
            Layout layout = JsonUtility.FromJson<Layout>(jsonFoo);

            //check for version change?
            if (layout.version != airplaneData.clientVersion)
            {
            }

            //check if slave
            if(layout.slave)
            {
                //spawn client window with arguments so it knows it is slave
                //on startup the slave will check for layout info itself
                UnityEngine.Debug.Log("Spawning Slave");
                SpawnSlaves(layout);
                //we are finished with this key, move to next element in loop
                continue;
            }

            //if we are the master - load the layout for this window ( and position of window)
            ScaleAndPositions(dialsManager, menuHandler, layout);
        }
    }

    private static void SpawnSlaves(Layout layout)
    {
        //craete id and pass as arg
        string args = "Slave " + System.DateTime.Now.ToString("hh.mm.ss.ffffff");

        var process = Process.GetCurrentProcess();
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();

        //window size and position

        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();
    }

    private static string[] GetKeys(AirplaneData airplaneData)
    {
        //get all player prefs that start with this plane type
        string[] keys = PlayerPrefsHelper.GetRegistryValues();

        List<string> planeKeys = new List<string>();
        foreach (string key in keys)
        {
            //look for matching plane type
            //matching keys can be in form "planeType" or "planeType xxxxx" (if slave)
            //we want all of these
            string[] subs = key.Split(' ');

            if (subs[0] == airplaneData.planeType)
            {
                //this is one of the keys we want
                planeKeys.Add(key);
            }
        }

        return keys;
    }

    private static void Slave()
    {

    }

    private static void ScaleAndPositions(DialsManager dialsManager, MenuHandler menuHandler, Layout layout)
    {
        //first apply window positin from save layout

        int width = layout.windowWidth;
        int height = layout.windowHeight;
        // Screen.SetResolution(width, height, false);
        Display.displays[1].SetParams(width, height, 1, 1);
        //save window scale

        //apply to dials/positions        
        dialsManager.speedometer.GetComponent<RectTransform>().anchoredPosition = layout.speedoPos;
        dialsManager.speedometer.GetComponent<RectTransform>().localScale = new Vector3(layout.speedoScale, layout.speedoScale, 1f);

        if (layout.speedoInTray)
            AddToTrayOnLoad(dialsManager.speedometer, menuHandler);

        GameObject altimeter = dialsManager.countryDialBoard.transform.Find("Altimeter").gameObject;
        altimeter.GetComponent<RectTransform>().anchoredPosition = layout.altPos;
        altimeter.GetComponent<RectTransform>().localScale = new Vector3(layout.altScale, layout.altScale, 1f);

        if (layout.altimeterInTray)
            AddToTrayOnLoad(altimeter, menuHandler);

        if (dialsManager.countryDialBoard.transform.Find("Heading Indicator") != null)
        {
            GameObject headingIndicator = dialsManager.countryDialBoard.transform.Find("Heading Indicator").gameObject;
            headingIndicator.GetComponent<RectTransform>().anchoredPosition = layout.headingPos;
            headingIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.headingScale, layout.headingScale, 1f);

            if (layout.headingIndicatorInTray)
                AddToTrayOnLoad(headingIndicator, menuHandler);
        }

        if (dialsManager.countryDialBoard.transform.Find("Turn And Bank") != null)
        {
            GameObject turnAndBank = dialsManager.countryDialBoard.transform.Find("Turn And Bank").gameObject;
            turnAndBank.GetComponent<RectTransform>().anchoredPosition = layout.turnAndBankPos;
            turnAndBank.GetComponent<RectTransform>().localScale = new Vector3(layout.turnAndBankScale, layout.turnAndBankScale, 1f);

            if (layout.turnAndBankInTray)
                AddToTrayOnLoad(turnAndBank, menuHandler);

        }

        if (dialsManager.countryDialBoard.transform.Find("Turn Coordinator") != null)
        {

            GameObject turnIndicator = dialsManager.countryDialBoard.transform.Find("Turn Coordinator").gameObject;
            turnIndicator.GetComponent<RectTransform>().anchoredPosition = layout.turnIndicatorPos;
            turnIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.turnIndicatorScale, layout.turnIndicatorScale, 1f);

            if (layout.turnIndicatorInTray)
                AddToTrayOnLoad(turnIndicator, menuHandler);
        }

        //both vsi share the same variable - only one vsi per plane

        if (dialsManager.countryDialBoard.transform.Find("VSI Smallest") != null)
        {

            GameObject vsi = dialsManager.countryDialBoard.transform.Find("VSI Smallest").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiSmallestPos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiSmallestScale, layout.vsiSmallestScale, 1f);

            if (layout.vsiSmallestInTray)
                AddToTrayOnLoad(vsi, menuHandler);
        }

        if (dialsManager.countryDialBoard.transform.Find("VSI Small") != null)
        {

            GameObject vsi = dialsManager.countryDialBoard.transform.Find("VSI Small").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiSmallPos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiSmallScale, layout.vsiSmallScale, 1f);

            if (layout.vsiSmallInTray)
                AddToTrayOnLoad(vsi, menuHandler);
        }

        //both vsi share the same variable - only one vsi per plane
        if (dialsManager.countryDialBoard.transform.Find("VSI Large") != null)
        {

            GameObject vsi = dialsManager.countryDialBoard.transform.Find("VSI Large").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiLargePos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiLargeScale, layout.vsiLargeScale, 1f);

            if (layout.vsiLargeInTray)
                AddToTrayOnLoad(vsi, menuHandler);
        }

        if (dialsManager.countryDialBoard.transform.Find("Artificial Horizon") != null)
        {

            GameObject artificialHorizon = dialsManager.countryDialBoard.transform.Find("Artificial Horizon").gameObject;
            artificialHorizon.GetComponent<RectTransform>().anchoredPosition = layout.artificialHorizonPos;
            artificialHorizon.GetComponent<RectTransform>().localScale = new Vector3(layout.artificialHorizonScale, layout.artificialHorizonScale, 1f);

            if (layout.artificialHorizonInTray)
                AddToTrayOnLoad(artificialHorizon, menuHandler);
        }

        if (dialsManager.countryDialBoard.transform.Find("Repeater Compass") != null)
        {

            GameObject repeaterCompass = dialsManager.countryDialBoard.transform.Find("Repeater Compass").gameObject;
            repeaterCompass.GetComponent<RectTransform>().anchoredPosition = layout.repeaterCompassPos;
            repeaterCompass.GetComponent<RectTransform>().localScale = new Vector3(layout.repeaterCompassScale, layout.repeaterCompassScale, 1f);

            if (layout.repeaterCompassInTray)
                AddToTrayOnLoad(repeaterCompass, menuHandler);
        }

        if (dialsManager.countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
        {
            GameObject repeaterCompassAlternate = dialsManager.countryDialBoard.transform.Find("Repeater Compass Alternate").gameObject;
            //using non alternate variables because we won't have two compasses 
            repeaterCompassAlternate.GetComponent<RectTransform>().anchoredPosition = layout.repeaterCompassAlternatePos;
            repeaterCompassAlternate.GetComponent<RectTransform>().localScale = new Vector3(layout.repeaterCompassAlternateScale, layout.repeaterCompassAlternateScale, 1f);

            if (layout.repeaterCompassAlternateInTray)
                AddToTrayOnLoad(repeaterCompassAlternate, menuHandler);
        }


        for (int i = 0; i < dialsManager.rpmObjects.Count; i++)
        {
            dialsManager.rpmObjects[i].GetComponent<RectTransform>().anchoredPosition = layout.rpmPos[i];
            dialsManager.rpmObjects[i].GetComponent<RectTransform>().localScale = new Vector3(layout.rpmScale[i], layout.rpmScale[i], 1f);

            if (layout.rpmInTray[i])
                AddToTrayOnLoad(dialsManager.rpmObjects[i], menuHandler);
        }
    }

    static void AddToTrayOnLoad(GameObject dial, MenuHandler menuHandler)
    {
        //USe button manager class to store dial in tray
        ButtonManager.PutDialInTray(dial, menuHandler);
    }

    public static List<GameObject> ActiveDials(GameObject dialsPrefab)
    {
        List<GameObject> activeDials = new List<GameObject>();
        for (int i = 0; i < dialsPrefab.transform.childCount; i++)
            if (dialsPrefab.transform.GetChild(i).gameObject.activeSelf)
                activeDials.Add(dialsPrefab.transform.GetChild(i).gameObject);

        return activeDials;
    }



    public static float DefaultDialScale(List<GameObject> activeDials)
    { //find out if we ned to scale dials to fit them all in the screen (happens if 7 or more dials)
        //length of top will be the longest
        float f = activeDials.Count;
        //round half of count upwards and convert to int. Mathf.Ceil rounds up. If on a whole number, it doesn't round up //https://docs.unity3d.com/ScriptReference/Mathf.Ceil.html
        //half of count because there are two rows
        int longestRow = (int)Mathf.Ceil(f / 2);
        longestRow *= 300;//300 default step between dials

        GameObject canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        //if longer than the canvas width
        //UnityEngine.Debug.Log("longest row = " + longestRow);
        //UnityEngine.Debug.Log("canvas X = " + canvasObject.GetComponent<RectTransform>().rect.width);

        float scale = 1f;
        if (longestRow > canvasObject.GetComponent<RectTransform>().rect.width)
        {
            //UnityEngine.Debug.Log("row longer than canvas");

            //use this ratio for all positional calculations
            scale = canvasObject.GetComponent<RectTransform>().rect.width / longestRow;

        }

        return scale;
    }



    public static void DefaultLayouts(GameObject dialsPrefab)
    {
        //Programtically sort default layouts, so if there is an update, i don't need to create a prefab layout

        //organise dials depending on how many are available
        //we need to know the total amount of active dials before we continue
        List<GameObject> activeDials = ActiveDials(dialsPrefab);

        float scale = DefaultDialScale(activeDials);

        //split in to two rows, if odd number, put more on the top
        for (int i = 0; i < activeDials.Count; i++)
        {
            //ternary statement            
            int odd = activeDials.Count % 2 != 0 ? 1 : 0;

            //if odd, we will add one extra to the top row
            if (i < activeDials.Count / 2 + odd)
            {
                //0 0
                //150 1
                //300 2

                int x = ((int)((activeDials.Count - 1) / 2)) * -150;
                //then add step
                int step = 300 * (i);
                x += step;

                int y = 150;

                //scale and round and convert to int for position
                float xFloat = x * scale;
                x = (int)(Mathf.Round(xFloat));
                float yFloat = y * scale;
                y = (int)(Mathf.Round(yFloat));

                activeDials[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);


            }
            else
            {
                //starting point //from whats left 
                //use "odd" to nudge in to position
                int diff = activeDials.Count - 1 + odd - (activeDials.Count / 2);
                int x = ((int)(diff));
                x *= -150;
                //then add step
                int step = 300 * (i - (activeDials.Count / 2));
                x += step;

                int y = -150;

                //scale and round and convert to int 
                float xFloat = x * scale;
                x = (int)(Mathf.Round(xFloat));
                float yFloat = y * scale;
                y = (int)(Mathf.Round(yFloat));

                activeDials[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }

            //scale dial            
            activeDials[i].transform.localScale = new Vector3(scale * 0.35f, scale * 0.35f, scale * 0.35f);
        }
    }
}
