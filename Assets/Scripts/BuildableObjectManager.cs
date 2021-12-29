using System.Threading.Tasks;
using System;

public class BuildableObjectManager
{
    private const string Api = "YOUR_API_GATEWAY_ENDPOINT";
    private ApiManager _apiManager;

    public BuildableObjectManager() { }

    public BuildableObjectManager(ApiManager apiManager)
    {
        _apiManager = apiManager;
    }

    public async Task<bool> BuildObject(IBuildableObject buildableObject)
    {
        var postBody = buildableObject.GetMessageAsJson();

        // call to kick off build action
        bool isSuccess = await _apiManager.Post(Api, postBody);

        if (isSuccess)
        {
            // kick off visual build progress
            buildableObject.StartBuild();

            // mock "build time"
            await Task.Delay(TimeSpan.FromSeconds(5));

            // In practice you'd call this after receiving a push notification, SQS message, or a message over websocket
            // indicating the build has finished.  Then you can perform any client side wrap-up of the build.
            buildableObject.EndBuild();

            return true;
        }
        return false;
    }
}
