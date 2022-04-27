
# SDK composition

The SDK is a set of classes and methods for making requests to the Xsolla REST APIs. It includes the following namespaces:
*   The `Auth` namespace that contains methods and classes for working with user authentication, the friend system, and for obtaining user information, etc.
*   The `Catalog` namespace that contains methods and classes for working with virtual items and virtual currencies, making purchases, etc.
*   The `Orders` namespace that contains methods and classes for payment processing and opening the payment UI.
*   The`Cart` namespace that contains methods and classes for managing a shopping cart.
*   The `Core` namespace that contains lower-level classes that are common to the rest of the SDK. For example, common methods for calling APIs, built-in browser, SDK settings, methods for integrating with third-party solutions, etc.


## The main SDK module


### SDK methods for calling the Xsolla APIs

To send requests to Xsolla servers and receive responses, the SDK provides the `XsollaAuth`, `XsollaCatalog`, `XsollaCart`, and `XsollaOrders` classes. If you want to implement your own logic for buying items or inventory management, and don’t want to write boilerplate code for API calls, these classes are a good place to start.

The `XsollaAuth`, `XsollaCatalog`, `XsollaCart`, and `XsollaOrders` classes contain methods to refer to the `WebRequestHelper` class, which uses the standard [UnityWebRequest](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html) class. Since the methods for API calls use coroutines for requests to Xsolla servers, [delegates](https://learn.unity.com/tutorial/delegates#5c894658edbc2a0d28f48aee) act for them as parameters. Delegates are called when a response is received from the server.

<div style="background-color: #d9d9d9">
	<p><strong>Example:</strong></p>
	<code>public void GetUserInfo(string token, Action &lt;UserInfo&gt; onSuccess, Action &lt;Error&gt; onError = null)</code>
	<p>The <code>GetUserInfo</code> method for getting user information has two delegate parameters:
		<ul>
		<li>The <code>OnSuccess</code> delegate is called if the server responds successfully. The <code>UserInfo</code> parameter contains information about the user (nickname, email, etc.).</li>
		<li>The <code>OnError</code> delegate is called if an error arises from the server. The <code>Error</code> parameter contains the error code and its description.</li>
		</ul>
	</p>
</div>


### Data and classes for working with APIs

In addition to methods for calling the APIs, the SDK contains data and classes for making requests to the API and receiving a response — [data transfer objects](https://en.wikipedia.org/wiki/Data_transfer_object).

<div style="background-color: #d9d9d9">
	<p><strong>Example:</strong>
	 <ul>
		 <li>The <code>CartItem</code> class describes a cart item (name, type, and other attributes).</li>
		 <li>The <code>UserSocialFriends</code> is data that describes a user's list of friends.</li>
	 </ul>
  </p>
	</div>

### Built-in browser

The built-in browser is a separate module designed to open web pages inside the application. It is usually used to open a payment UI and to log a user in via third-party services.

The `BrowserHelper` class is a convenient entry point for interacting with the built-in browser. That class contains the necessary methods for using the browser.

Use the `Open` method to open a webpage, passing the required URL as a parameter.

To open the payment UI via the built-in browser, use the `BrowserHelper` class and the `OpenPurchase` method, passing as parameters:

*   `Url` — Pay Station API endpoint.
*   `Token` — Pay Station token.
*   `IsSandbox` — payment processing mode.


### Utils classes

The SDK provides utility classes to work with Unity or third-party modules and solutions.

<div style="background-color: #d9d9d9">
	<p><strong>Example:</strong>
	<ul>
    <li>The <code>SteamManager</code> class manages authentication via Steam.</li>
		<li>A set of methods for working with native code on mobile devices.</li>
  </ul>
</p>
</div>


