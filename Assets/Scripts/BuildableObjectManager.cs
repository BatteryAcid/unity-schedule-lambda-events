using System.Threading.Tasks;
using System;

public class BuildableObjectManager
{
    private const string Api = "https://doqrfhpyh4.execute-api.us-east-1.amazonaws.com/demo";
    private ApiManager _apiManager;
    private SQSManager _sqsManager;

    public BuildableObjectManager() { }

    public BuildableObjectManager(ApiManager apiManager, SQSManager sqsManager)
    {
        _apiManager = apiManager;
        _sqsManager = sqsManager;
    }

    public async Task<bool> BuildObject(IBuildableObject buildableObject)
    {
        // Call API Gateway with schedule data
        // Lambda saves to dynamodb the start and end times
        // Lambda calls to schedule step function
        // Upon completion SQS notification is received here
        // Updates all local graphics and statuses
        //
        // Anytime the app refreshes data during an active build, we get the current builds' end time
        // and apply the current animation.
        //
        // Consider using websockets for instant updates on build? Not sure we need that for this example
        // I can always just talk to it in the video and say your game will likely have an active websocket
        // connection so if the app is open maybe skip sending out SQS to that player? or maybe send it then ignore it
        // if update is already received.

        // TODO: call to schedule build on step function through API Gateway
        // use unauthenticated identity pools

        // if websocket connected, send update there
        // also, push out SQS notification

        var postBody = buildableObject.GetJsonData();

        //TODO: pass in the uuid as part of the post request, move requestuuid up here

        // call to kick off build action
        bool isSuccess = await _apiManager.Post(Api, postBody);

        if (isSuccess)
        {
            // kick off visual build progress
            buildableObject.StartBuild();

            // subscribe to receive notification once complete
            var requestUuid = Guid.NewGuid().ToString();
            await _sqsManager.SubscribeToSQSMessages(requestUuid, buildableObject);
            return true;
        }
        return false;
    }
}
