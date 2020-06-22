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
        const string BUY_BUTTON = "Item.Button";
        const string SUCCESS_BUTTON = "SuccessPopupButton";
        const string INVENTORY_BUTTON = "InventoryButton";


        [UnityTest]
        [Timeout(100000000)]
        public IEnumerator SuccessBuyCart()
        {

            TestHelper helper = TestHelper.Instance;


            yield return helper.LoadScene(TestHelper.Scenes.Login);
            yield return helper.WaitFor(1.0F);
            helper.SetInputField(USERNAME_FIELD, "test123");
            helper.SetInputField(USERPASSWORD_FIELD, "232323");
            yield return helper.WaitFor(0.01F);
            helper.ClickButton(USERLOGIN_BUTTON);
            yield return helper.WaitScene(TestHelper.Scenes.Store, 10.0F);
            yield return helper.WaitFor(5.0F);



            yield return helper.WaitFor(1.0F);


            GameObject bodyArmor = TestHelper.Instance.Find("Item_BodyArmor");
            Assert.False(bodyArmor == null);
            AddToCartButton cartButton = TestHelper.Instance.FindIn<AddToCartButton>(bodyArmor, "CartButton");

            cartButton.OnPointerDown(new PointerEventData(EventSystem.current));
            cartButton.OnPointerUp(new PointerEventData(EventSystem.current));
            yield return helper.WaitFor(2.0F);

            GameObject section = TestHelper.Instance.Find("Group_Cart");
            CartMenuButton section2 = TestHelper.Instance.FindIn<CartMenuButton>(section, "pref_CartMenuButton");
            section2.OnPointerDown(new PointerEventData(EventSystem.current));
            section2.OnPointerUp(new PointerEventData(EventSystem.current));
            yield return helper.WaitFor(2.0F);
            helper.ClickSimpleTextButton(BUY_BUTTON);

            yield return helper.WaitFor(6.5F);

            SinglePageBrowser2D browser2D = BrowserHelper.Instance.GetLastBrowser();
            if (browser2D != null)
            {
                XsollaBrowser xsollaBrowser = browser2D.GetComponent<XsollaBrowser>();

                string str = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str);
                yield return helper.WaitFor(2.0F);
                string str2 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str2);
                yield return helper.WaitFor(0.5F);
                string str3 = KeysConverter.Convert(KeyCode.KeypadEnter);
                xsollaBrowser.Input.Keyboard.PressKey(str3);
                yield return helper.WaitFor(2.0F);
                string str4 = KeysConverter.Convert(KeyCode.Alpha4);
                xsollaBrowser.Input.Keyboard.PressKey(str4);
                string str5 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str5);
                string str6 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str6);
                string str7 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str7);
                string str8 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str8);
                string str9 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str9);
                string str10 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str10);
                string str11 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str11);
                string str12 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str12);
                string str13 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str13);
                string str14 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str14);
                string str15 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str15);
                string str16 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str16);
                string str17 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str17);
                string str18 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str18);
                string str19 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str19);
                yield return helper.WaitFor(0.5F);
                string str20 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str20);
                string str21 = KeysConverter.Convert(KeyCode.Alpha2);
                xsollaBrowser.Input.Keyboard.PressKey(str21);
                string str22 = KeysConverter.Convert(KeyCode.Alpha4);
                xsollaBrowser.Input.Keyboard.PressKey(str22);
                string str23 = KeysConverter.Convert(KeyCode.Alpha0);
                xsollaBrowser.Input.Keyboard.PressKey(str23);
                yield return helper.WaitFor(0.5F);
                string str24 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str24);
                string str25 = KeysConverter.Convert(KeyCode.Alpha2);
                xsollaBrowser.Input.Keyboard.PressKey(str25);
                string str26 = KeysConverter.Convert(KeyCode.Alpha3);
                xsollaBrowser.Input.Keyboard.PressKey(str26);
                yield return helper.WaitFor(1.0F);

                string str28 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str28);
                string str29 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str29);
                string str30 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str30);
                string str31 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str31);
                string str32 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str32);
                string str33 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str33);

                string str27 = KeysConverter.Convert(KeyCode.KeypadEnter);
                xsollaBrowser.Input.Keyboard.PressKey(str27);
                yield return helper.WaitFor(2.0F);

                string str37 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str37);
                string str34 = KeysConverter.Convert(KeyCode.Alpha1);
                xsollaBrowser.Input.Keyboard.PressKey(str34);
                string str35 = KeysConverter.Convert(KeyCode.Alpha2);
                xsollaBrowser.Input.Keyboard.PressKey(str35);
                string str36 = KeysConverter.Convert(KeyCode.Alpha3);
                xsollaBrowser.Input.Keyboard.PressKey(str36);

                yield return helper.WaitFor(1.0F);

                string str38 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str38);
                string str40 = KeysConverter.Convert(KeyCode.Tab);
                xsollaBrowser.Input.Keyboard.PressKey(str40);

                string str39 = KeysConverter.Convert(KeyCode.KeypadEnter);
                xsollaBrowser.Input.Keyboard.PressKey(str39);

                yield return helper.WaitFor(1.5F);
                GameObject close = TestHelper.Instance.Find("pref_SinglePageBrowser2D(Clone)");
                Button close2 = TestHelper.Instance.FindIn<Button>(close, "Button");
                close2?.onClick?.Invoke();


                yield return helper.WaitFor(5.0F);
                helper.ClickSimpleTextButton(SUCCESS_BUTTON);

                yield return helper.WaitFor(1.5F);
                helper.ClickMenuButton(INVENTORY_BUTTON);
                yield return helper.WaitFor(2.0F);

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
}
