[System.Serializable]
public class AccessDeniedException : System.Exception
{
    public AccessDeniedException() { }
    public AccessDeniedException(string message) : base(message) { }
    public AccessDeniedException(string message, Exception inner) : base(message, inner) { }
}