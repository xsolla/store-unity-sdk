mergeInto(LibraryManager.library, {
    OpenXsollaSocialAuthPopup: function (socialNetworkAuthUrlPtr) {
        var socialNetworkAuthUrl = UTF8ToString(socialNetworkAuthUrlPtr);
        
        // base popup url (pointer → string)
        var baseUrl = new URL(".", window.location.href);
        var popupUrl = baseUrl.href + "xl-social.html";
        
        // append social network auth url as query parameter
        popupUrl += (popupUrl.indexOf("?") === -1 ? "?" : "&");
        popupUrl += "social_network_url=" + encodeURIComponent(socialNetworkAuthUrl);
        
        console.log("[Xsolla SDK] PopupUrl:", popupUrl);
        console.log("[Xsolla SDK] SocialNetworkAuthUrl:", socialNetworkAuthUrl);
                
        const popupName = 'Xsolla Social Auth ' + Date.now();
        var popup = window.open(
            popupUrl,
            popupName,
            'width=600, height=700, resizable=yes, scrollbars=yes'
        );
        
        if (!popup || popup.closed || typeof popup.closed === 'undefined') {
            alert("The popup window was blocked by the browser. Please allow popups for this site.");
            return;
        }
    
        var messageHandler = function (event) {
            if (!event.data || event.data.type !== "xsolla-social-auth") {
                return;
            }
        
            var code = event.data.data;
            console.log("[Xsolla SDK] Auth code received:", code);
        
            SendMessage("XsollaWebCallbacks", "PublishWidgetAuthSuccess", code);
            window.removeEventListener("message", messageHandler);
        };
        
        window.addEventListener("message", messageHandler, false);
    },
    
    OpenXsollaSocialAuthPopupWithConfirmation: function (socialNetworkAuthUrlPtr, popupMessageTextPtr, continueButtonTextPtr, cancelButtonTextPtr) {
        var socialNetworkAuthUrl = UTF8ToString(socialNetworkAuthUrlPtr);
        var popupMessageText = UTF8ToString(popupMessageTextPtr);
        var continueButtonText = UTF8ToString(continueButtonTextPtr);
        var cancelButtonText = UTF8ToString(cancelButtonTextPtr);
        
        var overlay = document.createElement('div');
        overlay.style.position = 'fixed';
        overlay.style.top = '0';
        overlay.style.left = '0';
        overlay.style.width = '100%';
        overlay.style.height = '100%';
        overlay.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
        overlay.style.zIndex = '1000';
        overlay.style.display = 'flex';
        overlay.style.alignItems = 'center';
        overlay.style.justifyContent = 'center';
    
        var popup = document.createElement('div');
        popup.style.backgroundColor = '#ffffff';
        popup.style.padding = '20px';
        popup.style.borderRadius = '10px';
        popup.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.2)';
        popup.style.textAlign = 'center';
    
        var messageElement = document.createElement('p');
        messageElement.textContent = popupMessageText;
        messageElement.style.margin = '20px';
        messageElement.style.fontSize = '16px';
        popup.appendChild(messageElement);
    
        var cancelButton = document.createElement('button');
        cancelButton.textContent = cancelButtonText;
        cancelButton.style.padding = '10px 20px';
        cancelButton.style.margin = '0 5px';
        cancelButton.style.border = 'none';
        cancelButton.style.borderRadius = '5px';
        cancelButton.style.borderWidth = '1px';
        cancelButton.style.borderColor = '#6939F9';
        cancelButton.style.borderStyle = 'solid';
        cancelButton.style.backgroundColor = '#ffffff';
        cancelButton.style.color = '#6939F9';
        cancelButton.style.cursor = 'pointer';
        
        cancelButton.addEventListener('click', function () {
            document.body.removeChild(overlay);
        });
        
        var continueButton = document.createElement('button');
        continueButton.textContent = continueButtonText;
        continueButton.style.padding = '10px 20px';
        continueButton.style.margin = '0 5px';
        continueButton.style.border = 'none';
        continueButton.style.borderRadius = '5px';
        continueButton.style.borderWidth = '1px';
        continueButton.style.borderColor = '#6939F9';
        continueButton.style.borderStyle = 'solid';
        continueButton.style.backgroundColor = '#6939F9';
        continueButton.style.color = '#ffffff';
        continueButton.style.cursor = 'pointer';
        
        continueButton.addEventListener('click', function () {
        
            // base popup url (pointer → string)
            var baseUrl = new URL(".", window.location.href);
            var popupUrl = baseUrl.href + "xl-social.html";
            
            // append social network auth url as query parameter
            popupUrl += (popupUrl.indexOf("?") === -1 ? "?" : "&");
            popupUrl += "social_network_url=" + encodeURIComponent(socialNetworkAuthUrl);
            
            console.log("[Xsolla SDK] PopupUrl:", popupUrl);
            console.log("[Xsolla SDK] SocialNetworkAuthUrl:", socialNetworkAuthUrl);
                    
            const popupName = 'Xsolla Social Auth ' + Date.now();
            var popup = window.open(
                popupUrl,
                popupName,
                'width=600, height=700, resizable=yes, scrollbars=yes'
            );
            
            if (!popup || popup.closed || typeof popup.closed === 'undefined') {
                alert("The popup window was blocked by the browser. Please allow popups for this site.");
                return;
            }
        
            var messageHandler = function (event) {
                if (!event.data || event.data.type !== "xsolla-social-auth") {
                    return;
                }
            
                var code = event.data.data;
                console.log("[Xsolla SDK] Auth code received:", code);
            
                SendMessage("XsollaWebCallbacks", "PublishWidgetAuthSuccess", code);
                window.removeEventListener("message", messageHandler);
            };
            
            window.addEventListener("message", messageHandler, false);
            document.body.removeChild(overlay);
        });
        
        popup.appendChild(cancelButton);
        popup.appendChild(continueButton);
        overlay.appendChild(popup);
        document.body.appendChild(overlay);
    },
    
    GetSocialAuthPopupUrl: function () {
        var baseUrl = new URL(".", window.location.href);
        var popupUrl = baseUrl.href + "xl-social.html";
        
        var bufferSize = lengthBytesUTF8(popupUrl) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(popupUrl, buffer, bufferSize);
        return buffer;
    },
});
