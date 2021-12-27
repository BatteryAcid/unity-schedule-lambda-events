using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using System.Threading.Tasks;

// Implement authorization strategy required for your project
public class CredentialManager : MonoBehaviour
{
    // Set identity pool id and region
    private const string CognitoIdentityPool = "us-east-1:5367c0bb-6993-40a9-9058-2282492791c6"; // TODO: YOUR_COGNITO_IDENITY_POOL_ID
    private RegionEndpoint Region = RegionEndpoint.USEast1;

    private CognitoAWSCredentials credentials;

    // Start is called before the first frame update
    void Start()
    {
        credentials = new CognitoAWSCredentials(
            CognitoIdentityPool,
            Region
        );

        //string identityId = credentials.GetIdentityId();
        //Debug.Log("GetIdentityId : " + identityId);
    }

    public CognitoAWSCredentials GetCredentials()
    {
        return credentials;
    }
}
