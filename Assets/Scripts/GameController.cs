using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private Button buildTempleButton;
    private bool buildInProgress = false;
    private BuildableObjectManager buildableObjectManager;

    // Start is called before the first frame update
    void Start()
    {
        buildTempleButton = GameObject.Find("BuildTemple").GetComponent<Button>();
        buildTempleButton.onClick.AddListener(OnBuildTemplePressed);

        buildableObjectManager = new BuildableObjectManager(FindObjectOfType<ApiManager>());
    }

    // Update is called once per frame
    void Update()
    {
        if (buildInProgress)
        {
            buildTempleButton.enabled = false;
        }
    }

    private void BuildComplete()
    {
        Debug.Log("Game controller build complete");
        buildInProgress = false;
        buildTempleButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Build Temple";
        buildTempleButton.enabled = true;
    }

    public async void OnBuildTemplePressed()
    {
        Debug.Log("Build temple pressed");
        buildInProgress = true;
        buildTempleButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Building...";

        
        bool isSuccess = await buildableObjectManager.BuildObject(FindObjectOfType<Temple>());
        BuildComplete();
    }
}
