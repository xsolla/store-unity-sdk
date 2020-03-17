using System.Threading;
using NUnit.Framework;

public class AutorizationTest
{
    public AltUnityDriver AltUnityDriver;
    //Before any test it connects with the socket
    [OneTimeSetUp]
    public void SetUp()
    {
        AltUnityDriver =new AltUnityDriver();
    }

    //At the end of the test closes the connection with the socket
    [OneTimeTearDown]
    public void TearDown()
    {
        AltUnityDriver.Stop();
    }

    [Test]
    public void Test()
    {

        AltUnityDriver.LoadScene("Login");
        AltUnityObject login = AltUnityDriver.FindElement("LoginUsername");
        login.ClickEvent();
        login.SetText("test123");
        AltUnityObject pass = AltUnityDriver.FindElement("LoginPassword");
        pass.ClickEvent();
        pass.SetText("232323");
        AltUnityDriver.FindElement("LoginButton").ClickEvent();
        Thread.Sleep(4000);
        AltUnityDriver.FindElement("CartGroup(Clone)");




    }

}