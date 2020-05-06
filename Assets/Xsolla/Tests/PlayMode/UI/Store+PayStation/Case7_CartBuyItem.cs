using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;
using NUnit.Framework;
using UnityEngine.EventSystems;

namespace Tests
{
    public class Case7_CartBuyItem
    {

        const string CART_BUTTON = "pref_CartMenuButton";



        const string USERNAME_FIELD = "LoginUsername";
        const string USERPASSWORD_FIELD = "LoginPassword";
        const string USERLOGIN_BUTTON = "LoginButton";

        [UnityTest]
        public IEnumerator SuccessBuyCart()
        {

            TestHelper helper = TestHelper.Instance;


            yield return helper.LoadScene(TestHelper.Scenes.Login);
            yield return helper.WaitFor(1.0F);
            helper.SetInputField(USERNAME_FIELD, "test123");
            helper.SetInputField(USERPASSWORD_FIELD, "232323");
            yield return helper.WaitFor(0.01F);
            helper.ClickButton(USERLOGIN_BUTTON);
            yield return helper.WaitScene(TestHelper.Scenes.Store, 4.0F);
            yield return helper.WaitFor(5.0F);



            yield return helper.WaitFor(1.0F);
           

            GameObject bodyArmor = TestHelper.Instance.Find("Item_Test_item");
            AddToCartButton cartButton = TestHelper.Instance.FindIn<AddToCartButton>(bodyArmor, "CartButton");

            cartButton.OnPointerDown(new PointerEventData(EventSystem.current));
            cartButton.OnPointerUp(new PointerEventData(EventSystem.current));
            yield return TestHelper.Instance.WaitFor(3F);
          //  Assert.True(UserCart.Instance.GetItems().Count == 1);
            yield return helper.WaitFor(2.0F);

            GameObject section = TestHelper.Instance.Find("Group_Cart");
            CartMenuButton section2 = TestHelper.Instance.FindIn<CartMenuButton>(section, "pref_CartMenuButton");
            section2.OnPointerDown(new PointerEventData(EventSystem.current));
            section2.OnPointerUp(new PointerEventData(EventSystem.current));

            yield return helper.WaitFor(5.0F);
            Assert.True(helper.IsScene(TestHelper.Scenes.Store));










            //cartButton.OnPointerDown(new PointerEventData(EventSystem.current));
            //cartButton.OnPointerUp(new PointerEventData(EventSystem.current));
            //yield return TestHelper.Instance.WaitFor(3F);
            //Assert.True(UserCart.Instance.GetItems().Count == 0);
            //SimpleTextButton buyButton = TestHelper.Instance.FindIn<SimpleTextButton>(bodyArmor, "BuyButton");
            //buyButton.onClick?.Invoke();
            //yield return new WaitUntil(() => BrowserHelper.Instance.GetLastBrowser() != null);
        }

    }
}
