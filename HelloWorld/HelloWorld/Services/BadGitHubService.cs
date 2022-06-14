namespace HelloWorld.Services
{
    public class BadGitHubService
    {
        private string _token = "4a9a0b1aec1a34201b3c5658855e8b7";

        public string GetMeSomeInfo()
        {
            // do something with the token
            return _token.ToLower();
        }
    }
}
