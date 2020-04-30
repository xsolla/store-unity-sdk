using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Tests
{
    public class Case5_SignOut
    {
        const string SIGN_OUT_BUTTON = "Button_SignOut";

        [UnityTest]
        public IEnumerator SuccessSignOut()
        {
            TestHelper helper = TestHelper.Instance;
            // Load Store scene
            yield return helper.LoadScene(TestHelper.Scenes.Store);
            // Click SignOut button
            helper.ClickSimpleTextButton(SIGN_OUT_BUTTON);
            // Wait one of: Login scene loaded or Timeout is expired
            yield return helper.WaitScene(TestHelper.Scenes.Login, 4.0F);
            // Assert expected result
            Assert.True(helper.IsScene(TestHelper.Scenes.Login));
        }
    }
}
