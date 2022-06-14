namespace HelloWorld.Services
{
    public interface IBadEncryptionService
    {
        string CreateMD5(string input);
        byte[] Encrypt(string plainText);
    }
}