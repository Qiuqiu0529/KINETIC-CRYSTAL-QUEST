using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIController : Singleton<TutorialUIController>
{
    public List<GameObject> tutorialPanels;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayHelpPanel()
    {

        GameMode gameMode = ModeMgr.Instance.gamename;
        int panelIndex = (int)gameMode-1;
        if (panelIndex<0)
        {
            Debug.LogWarning("panelIndex < 0");
            return;
        }
        int numOfPanels = tutorialPanels.Count;
        if (panelIndex>numOfPanels-1)
        {
            Debug.LogWarning("panelIndex out of range");
            return;
        }

        GameObject tutorialPanel = tutorialPanels[panelIndex];
        MainUIMgr.Instance.SetParticleDisActive();

        tutorialPanel.SetActive(true);
    }

    public bool IsTutorialPanelActive()
    {
        foreach (var panel in tutorialPanels)
        {
            if (panel.activeSelf)
            {
                return true;
            }
        }
        return false;
    }


}
