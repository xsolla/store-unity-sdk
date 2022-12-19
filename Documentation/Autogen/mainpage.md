
# SDK composition

The SDK is a set of classes and methods for making requests to the Xsolla REST APIs. It includes the following namespaces:
*   The `Auth` namespace that contains methods and classes for working with user authentication and for obtaining user information.
*   The `Catalog` namespace that contains methods and classes for working with virtual items and virtual currencies, making purchases, etc.
*   The `Subscriptions` namespace that contains methods and classes for purchasing and managing user subscriptions.
*   The `UserAccount` namespace that contains methods and classes for managing user's personal data and public profile data and linking social network accounts, platform accounts, and devices.
*   The `Inventory` namespace that contains methods and classes to display and consume the user’s items that they bought or received according to the game logic.
*   The `Orders` namespace that contains methods and classes for payment processing and opening the payment UI.
*   The `Cart` namespace that contains methods and classes for managing a shopping cart.
*   The `Core` namespace that contains lower-level classes that are common to the rest of the SDK. For example, common methods for calling APIs, built-in browser, SDK settings, methods for integrating with third-party solutions, etc.


## The main SDK module


### SDK methods for calling the Xsolla APIs

To send requests to Xsolla servers and receive responses, the SDK provides the `XsollaAuth`, `XsollaCatalog`, `XsollaCart`, and `XsollaOrders` classes. If you want to implement your own logic for buying items or inventory management, and don’t want to write boilerplate code for API calls, these classes are a good place to start.

The `XsollaAuth`, `XsollaCatalog`, `XsollaSubscriptions`, `XsollaUserAccount`, `XsollaInventory`, `XsollaOrders`, and `XsollaCart` classes contain methods to refer to the `WebRequestHelper` class, which uses the standard [UnityWebRequest](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html) class. Since the methods for API calls use coroutines for requests to Xsolla servers, [delegates](https://learn.unity.com/tutorial/delegates#5c894658edbc2a0d28f48aee) act for them as parameters. Delegates are called when a response is received from the server.

**Example:**

<code>public void GetUserInfo(string token, Action &lt;UserInfo&gt; onSuccess, Action &lt;Error&gt; onError = null)</code>

The `GetUserInfo` method for getting user information has two delegate parameters:

*   The `OnSuccess` delegate is called if the server responds successfully. The `UserInfo` parameter contains information about the user (nickname, email, etc.).
*   The `OnError` delegate is called if an error arises from the server. The `Error` parameter contains the error code and its description.



### Data and classes for working with APIs

In addition to methods for calling the APIs, the SDK contains data and classes for making requests to the API and receiving a response — [data transfer objects](https://en.wikipedia.org/wiki/Data_transfer_object).

**Example:**

*   The `CartItem` class describes a cart item (name, type, and other attributes).
*   The `UserSocialFriends` is data that describes a user's list of friends.

### Built-in browser

The built-in browser is a separate module designed to open web pages inside the application. It is usually used to open a payment UI and to log a user in via third-party services.

Browser limitations:


* One page per browser instance.
* No navigating functions: Back, Forward, etc.
* Only [printable ASCII characters](http://facweb.cs.depaul.edu/sjost/it212/documents/ascii-pr.htm) are supported..
* Works only for desktop builds.
* Compatible only with Mono compiler.

To enable/disable the build-in browser use the **Enable In-App browser** box in the **Inspector** panel.

When the application starts, it checks for the built-in browser. If the built-in browser isn't installed, it is downloaded. The file that needs to be downloaded is 300 MB, so it may take some time when the user first starts your application. You can also pack the browser in your application. In this case, when building the application, the browser is downloaded in advance and placed in the build folder. To do this, check the **Pack In-App Browser in Build** box in the **Inspector** panel.

The `BrowserHelper` class is a convenient entry point for interacting with the built-in browser. That class contains the necessary methods for using the browser.

Use the `Open` method to open a web page, passing the required URL as a parameter.

To open the payment UI via the built-in browser, use the `BrowserHelper` class and the `OpenPurchase` method and pass the following parameters:

* `Url` — Pay Station API endpoint.
* `Token` — Pay Station token.


### Utils classes

The SDK provides utility classes to work with Unity or third-party modules and solutions.

**Example:**

*   The `SteamManager` class manages authentication via Steam.
*   A set of methods for working with native code on mobile devices.
