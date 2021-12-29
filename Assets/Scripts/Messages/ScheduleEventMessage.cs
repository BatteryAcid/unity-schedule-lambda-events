[System.Serializable]
public class ScheduleEventMessage
{
    public string buildId;
    public ScheduleEventMessage() { }
    public ScheduleEventMessage(string buildIdIn)
    {
        this.buildId = buildIdIn;
    }
}
