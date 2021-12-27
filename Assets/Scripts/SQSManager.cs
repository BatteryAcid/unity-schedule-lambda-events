using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.CognitoIdentity;

public class SQSManager : MonoBehaviour
{
    private AmazonSQSClient sqsClient;
    private static string SQSURL = "https://sqs.us-east-1.amazonaws.com/654368844800/unity-schedule-lambda-events-queue";
    private const int MaxMessages = 1;
    private const int WaitTime = 20;
    private bool messageReceived = false;
    private CredentialManager _credentialManager;
    private Dictionary<string, IBuildableObject> queuesSubscribed = new Dictionary<string, IBuildableObject>();

    public void Start()
    {
        Debug.Log("SQSManager start");

        _credentialManager = FindObjectOfType<CredentialManager>();
    }

    // NOTE: All active clients listening to the SQS topic will receive messages from other clients whoes builds
    // complete while subscribed.  However, they will be filtered out in the client code and ignored.  This is
    // not ideal at scale as large amounts of unncessary messages will be sent out, unless, of course, your game has
    // requirements that would want those clients to receive those messages.
    //
    // NOTE:  Should we set a local timer first to the end time minus like 20 seconds?
    // Then kick off the subscription at the end?  That would cut down on the large amount of unwarrented messages received...
    // 
    //
    // TODO: name?
    public async Task<PlayerPlacementFulfillmentInfo> SubscribeToSQSMessages(string requestId, IBuildableObject buildableObject)
    {
        Debug.Log("SubscribeToEventNotifications...");

        if (sqsClient == null)
        {
            // place here to prevent race condition with Credential Manager Start function
            sqsClient = new AmazonSQSClient(_credentialManager.GetCredentials(), RegionEndpoint.USEast1);
        }

        // this may have to be populated from a dynamodb table, as we save every action to track start, end times and status
        // TODO: is this needed? Maybe we don't need to capture multiple builds on client?
        queuesSubscribed.Add(requestId, buildableObject);

        //TODO: do this? fulfillmentFailsafeCoroutine = StartCoroutine(FailsafeTimer());
        PlayerPlacementFulfillmentInfo playerPlacementFulfillmentInfo = null;

        do
        {
            var msg = await GetMessage(sqsClient, SQSURL, WaitTime);
            if (msg.Messages.Count != 0)
            {
                Debug.Log("SubscribeToEventNotifications received message: " + msg.Messages[0].Body.ToString());

                // TODO: add requestId conditional here to filter out other messages
                // In order for that to work we have to create an object that represents the SQS response object
                // If we get a message that's not for us, remove visibility timeout: // https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/SQS/TReceiveMessageRequest.html

                // we don't break loop here because the message received wasn't for this player

                messageReceived = true; // break loop

                buildableObject.EndBuild();

                await DeleteMessage(sqsClient, msg.Messages[0], SQSURL);

                //playerPlacementFulfillmentInfo = ConvertMessage(msg.Messages[0].Body);

                // make sure this notification was for our player
                //if (playerPlacementFulfillmentInfo != null && playerPlacementFulfillmentInfo.placementId == placementId)
                //{
                //    Debug.Log("Placement fulfilled, break loop...");
                //    messageReceived = true; // break loop

                //    // Delete consumed message 
                //    await DeleteMessage(sqsClient, msg.Messages[0], SQSURL);

                //    if (fulfillmentFailsafeCoroutine != null)
                //    {
                //        // kill failsafe coroutine
                //        StopCoroutine(fulfillmentFailsafeCoroutine);
                //    }
                //}

            }
        } while (!messageReceived);

        messageReceived = false;

        return playerPlacementFulfillmentInfo;
    }




    // Method to read a message from the given queue
    private static async Task<ReceiveMessageResponse> GetMessage(IAmazonSQS sqsClient, string qUrl, int waitTime = 0)
    {
        Debug.Log("GetMessage...");
        return await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = qUrl,
            MaxNumberOfMessages = MaxMessages,
            WaitTimeSeconds = waitTime
        });
    }


    private static PlayerPlacementFulfillmentInfo ConvertMessage(string convertMessage)
    {
        Debug.Log("ConvertMessage...");

        string cleanedMessage = CleanupMessage(convertMessage);

        SQSMessage networkMessage = JsonConvert.DeserializeObject<SQSMessage>(cleanedMessage);
        if (networkMessage != null)
        {
            //Debug.Log("networkMessage.Message: " + networkMessage.Message);
            Debug.Log("networkMessage.TopicArn: " + networkMessage.TopicArn);
            Debug.Log("networkMessage.Type: " + networkMessage.Type);

        }
        else
        {
            Debug.Log("NetworkMessage was null");
        }
        return null;
    }

    private static string CleanupMessage(string messageToClean)
    {
        // The Message is JSON inside a string, this removes the quotes around the braces so it can be serialized as an object
        string cleanedMessage = messageToClean.Replace("\"{", "{");
        cleanedMessage = cleanedMessage.Replace("}\"", "}");
        //Debug.Log("cleanedMessage1 " + cleanedMessage);

        // remove escape slashes from message string so it can be properly read as object
        cleanedMessage = cleanedMessage.Replace("\\", "");
        //Debug.Log("cleanedMessage2 " + cleanedMessage);

        return cleanedMessage;
    }

    // Method to delete a message from a queue
    private async Task<bool> DeleteMessage(IAmazonSQS sqsClient, Message message, string qUrl)
    {
        Debug.Log($"Deleting message {message.MessageId} from queue...");
        try
        {
            await sqsClient.DeleteMessageAsync(qUrl, message.ReceiptHandle);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Failed to delete SQS queue message: " + qUrl + ", " + message.MessageId + ", exception: " + ex);
            return false;
        }
    }

}

// TODO: move to own class
[System.Serializable]
public class SQSMessage
{
    public string Type;
    public string MessageId;
    public string TopicArn;
    public string Timestamp;
}

public class PlayerPlacementFulfillmentInfo
{
    public string ipAddress;
    public int port;
    public string placementId;
    public string gameSessionId;

    public PlayerPlacementFulfillmentInfo() { }
}