using NUnit.Framework;
using System.Threading;

public class FailedAutorizationTest
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
        login.SetText("WrongUsername");
        AltUnityObject pass = AltUnityDriver.FindElement("LoginPassword");
        pass.ClickEvent();
        pass.SetText("WrongPassword");
        AltUnityDriver.FindElement("LoginButton").ClickEvent();
        Thread.Sleep(1000);
        AltUnityDriver.FindElement("ErrorPopUpTryAgainButton");
    }

}