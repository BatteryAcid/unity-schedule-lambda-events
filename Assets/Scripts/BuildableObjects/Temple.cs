using UnityEngine;

// This class shouldn't handle any networking, just local actions on the game object
public class Temple : MonoBehaviour, IBuildableObject
{
    private const string GameObjectName = "PressurePad";
    private const string InnerPlatformName = "PressurePadInner";
    private const string BuildId = "temple";

    private GameObject _templeObject;
    private Transform _innerPlatform;
    private Vector3 _innerPlatformStartPosition;
    private Vector3 _innerPlatformBuildCompletePosition;
    private float _elapsedBuildTime = 0f;
    private string _platformMovementDirection = "down";
    private bool isBuilding = false;
    private bool _endBuildAnimation = false;

    // kickoff build animations, etc
    public void StartBuild()
    {
        _templeObject.SetActive(true);

        _templeObject.transform.localPosition = new Vector3(1.2f, -2.08f, -4.4f); // just some random height above the bottom for nice effect
        _innerPlatformBuildCompletePosition = _innerPlatform.transform.position;

        _innerPlatformStartPosition = new Vector3(_innerPlatform.transform.position.x,
            _innerPlatform.transform.position.y + 5f,
            _innerPlatform.transform.position.z);

        _innerPlatform.transform.position = _innerPlatformStartPosition;

        isBuilding = true;
    }

    // end build animation and perform any after build cleanup
    public void EndBuild()
    {
        Debug.Log("Ending build...");

        // setting to true allows the animation to end at the bottom
        _endBuildAnimation = true;
    }

    public string GetMessageAsJson()
    {
        ScheduleEventMessage scheduleEventMessage = new ScheduleEventMessage(BuildId);
        string jsonPostData = JsonUtility.ToJson(scheduleEventMessage);
        return jsonPostData;
    }
  
    void Start()
    {
        _templeObject = GameObject.Find(GameObjectName);
        _templeObject.SetActive(false);
        _innerPlatform = _templeObject.transform.Find(InnerPlatformName);
    }

    void Update()
    {
        if (isBuilding)
        {
            _elapsedBuildTime += Time.deltaTime;

            if (_platformMovementDirection == "down" && _innerPlatform.transform.position.y <= _innerPlatformBuildCompletePosition.y)
            {
                CheckAnimationEnd(); // only end at the bottom, so finish downward animation then stop

                // got to bottom, going up
                _platformMovementDirection = "up";
                _elapsedBuildTime = 0f;
            } else if (_platformMovementDirection == "up" && _innerPlatform.transform.position.y >= _innerPlatformStartPosition.y)
            {
                // got to top, going down
                _platformMovementDirection = "down";
                _elapsedBuildTime = 0f;
            }

            if (_platformMovementDirection == "down")
            {
                _innerPlatform.transform.position
                    = Vector3.Lerp(_innerPlatformStartPosition, _innerPlatformBuildCompletePosition, _elapsedBuildTime / GetAnimationTimeInSeconds());
            }
            else
            {
                _innerPlatform.transform.position
                    = Vector3.Lerp(_innerPlatformBuildCompletePosition, _innerPlatformStartPosition, _elapsedBuildTime / GetAnimationTimeInSeconds());
            }
        }
    }

    private void CheckAnimationEnd()
    {
        if (_endBuildAnimation)
        {
            isBuilding = false;
            _endBuildAnimation = false;
        }
    }

    public float GetAnimationTimeInSeconds()
    {
        return 3f;
    }
}
