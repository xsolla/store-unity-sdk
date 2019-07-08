### JWT Validation

A [JWT](https://jwt.io/introduction/) is generated for each successfully authenticated user. This value is signed by the secret key encrypted according to the SHA-256 algorithm. You can set up JWT validation using Firebase. Follow the instructions below:
1. Create the [Firebase](https://firebase.google.com/) project.
2. Install [Node.JS](https://nodejs.org/en/).
3. Install the **firebase-tools** package:
```
$ npm install -g firebase-tools
```
4. Go to the Unity project > **Assets > Xsolla > Server** and unpack the *xsolla-jwt.7z* archive.
5. Open console, go to the Unity project > **Assets > Xsolla > Server > xsolla-jwt** and run the following command: 
```
$ firebase login
```
6. Specify your Firebase authentication data in a browser.
7. Go to the Unity project > **Assets > Xsolla > Server > xsolla-jwt**, open the *.firebaserc* file, and check that the Firebase Project ID is correct. **Note:** If you could not find the *.firebaserc* file, set up the display of hidden files on your PC.
8. Go to the Unity project > **Assets > Xsolla > Server > xsolla-jwt > functions**.
1. Open the *config.json* file and paste your secret key. You can find it in your **Publisher Account > Login settings > General settings**.  
2. Install the xsolla-jwt script to the Firebase project:
```
$ npm install
$ npm run deploy
```
3. Copy the URL from the console.
9. Open *XsollaAuthentication.prefab* in **Inspector** and paste the copied URL to the **JWT validation URL** field.

**Note:** Login SDK automatically validates and updates the JWT value on the Xsolla’s server. 
