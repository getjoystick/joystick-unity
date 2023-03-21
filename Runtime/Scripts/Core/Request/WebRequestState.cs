namespace JoystickRemoteConfig.Core.Web
{
    public enum WebRequestState : byte
    {
        None,
        Pending,
        Timeout,
        Completed
    }
}