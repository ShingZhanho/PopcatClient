namespace PopcatClient
{
    public class PopcatClient
    {
        public PopcatClient(int waitTime)
        {
            WaitTime = waitTime;
        }
        public string Token { get; private set; }
        public int WaitTime { get; }
    }
}