namespace JoystickRemote.Core.Web
{
    public enum WebRequestState : byte
    {
        None,
        Pending,
        Timeout,
        Completed
    }
}