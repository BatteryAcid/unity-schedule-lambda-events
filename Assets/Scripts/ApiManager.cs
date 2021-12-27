using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class ApiManager : MonoBehaviour
{
    public async Task<bool> Post(string api, string postData)
    {
        bool isuccess = false;
        var formData = System.Text.Encoding.UTF8.GetBytes(postData);

        UnityWebRequest webRequest = UnityWebRequest.Post(api, "");

        // add body
        webRequest.uploadHandler = new UploadHandlerRaw(formData);
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // add header so we know where the reqest came from, if reusing same lambda function for both API GW and Step Functions
        // TODO: not sure if this is necessary, but if so come up with better header name
        webRequest.SetRequestHeader("source", "unity");

        await webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
            
        {
            Debug.Log("Success, API call complete!");
            Debug.Log(webRequest.downloadHandler.text);
            isuccess = true;
        }
        else
        {
            Debug.Log("API call failed: " + webRequest.error + "\n" + webRequest.result + "\n" + webRequest.responseCode);
        }

        webRequest.Dispose();

        return isuccess;
    }
}

// TODO: move this to its own class
[System.Serializable]
public class BuildId
{
    public string buildId;
    public BuildId() { }
    public BuildId(string buildIdIn)
    {
        this.buildId = buildIdIn;
    }
}