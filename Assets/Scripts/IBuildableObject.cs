using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildableObject
{
    // TODO: actions that happen to the object being built...
    public void StartBuild();
    public void EndBuild();
    public string GetComponentName();
    public float GetBuildTimeInSeconds();
    public string GetJsonData(); // TODO: rename
    public bool GetIsBuilding(); // TODO: probably remove these
    public void SetIsBuilding(bool isBuildingIn);
}
