using UnityEngine;

public class CommandsPanelHandler : MonoBehaviour
{
    public MainMenu mainMenu;
    public string worldName = "CowRunner";
    public DataSave.DataKeys dataKey;
    public GameObject[] enable, disable;
    
    public void CommandsOrPlay()
    {
        if(true)//if (DataSave.GetVal(dataKey)==0)
        {
            DataSave.SetVal(dataKey, 1);
            ShowChargeCommands();
        }
        else
        {
            mainMenu.GoToWorld(worldName);
        }
        
    }

    private void ShowChargeCommands()
    {
        foreach (var en in enable)
        {
            en.SetActive(true);
        }

        foreach (var di in disable)
        {
            di.SetActive(false);
        }
    }
}
