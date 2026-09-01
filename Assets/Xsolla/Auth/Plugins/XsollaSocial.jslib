mergeInto(LibraryManager.library, {
    OpenXsollaSocialAuthPopup: function (socialNetworkAuthUrlPtr) {
        var socialNetworkAuthUrl = UTF8ToString(socialNetworkAuthUrlPtr);
        var flowId = Date.now().toString() + "-" + Math.random().toString(36).substring(2, 11);
        
        // base popup url (pointer → string)
        var baseUrl = new URL(".", window.location.href);
        var popupUrl = baseUrl.href + "xl-social.html";
        
        // append social network auth url as query parameter
        popupUrl += (popupUrl.indexOf("?") === -1 ? "?" : "&");
        popupUrl += "social_network_url=" + encodeURIComponent(socialNetworkAuthUrl);
        popupUrl += "&sdk_flow_id=" + encodeURIComponent(flowId);
        
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

        var storageKey = "xsolla-auth-" + flowId;
        var isCompleted = false;
        var pollTimer = null;
        var channel = null;
        var authStartTime = Date.now();

        var completeAuth = function (code) {
            if (isCompleted || !code) {
                return;
            }

            isCompleted = true;
            window.removeEventListener("message", messageHandler);
            window.removeEventListener("storage", storageHandler);
            if (pollTimer) {
                clearInterval(pollTimer);
                pollTimer = null;
            }
            if (channel) {
                channel.close();
                channel = null;
            }
            try {
                localStorage.removeItem(storageKey);
            } catch (error) {
                console.warn("[Xsolla SDK] Failed to clear localStorage auth key:", error);
            }

            console.log("[Xsolla SDK] Auth code received:", code);
            SendMessage("XsollaWebCallbacks", "PublishWidgetAuthSuccess", code);
        };
    
        var messageHandler = function (event) {
            if (!event.data || event.data.type !== "xsolla-social-auth") {
                return;
            }
            if (event.data.flowId && event.data.flowId !== flowId) {
                return;
            }
            completeAuth(event.data.data);
        };

        var parseStoragePayload = function (rawValue) {
            if (!rawValue) {
                return null;
            }
            try {
                return JSON.parse(rawValue);
            } catch (error) {
                console.warn("[Xsolla SDK] Failed to parse localStorage auth payload:", error);
                return null;
            }
        };

        var storageHandler = function (event) {
            if (event.key !== storageKey || !event.newValue) {
                return;
            }
            var payload = parseStoragePayload(event.newValue);
            if (!payload || payload.type !== "xsolla-social-auth") {
                return;
            }
            completeAuth(payload.data);
        };

        window.addEventListener("message", messageHandler, false);
        window.addEventListener("storage", storageHandler, false);

        if (typeof BroadcastChannel !== "undefined") {
            channel = new BroadcastChannel("xsolla-auth-" + flowId);
            channel.onmessage = function (event) {
                if (!event.data || event.data.type !== "xsolla-social-auth") {
                    return;
                }
                completeAuth(event.data.data);
            };
        }

        pollTimer = setInterval(function () {
            if (isCompleted) {
                return;
            }
            if ((popup && popup.closed) || Date.now() - authStartTime > 10 * 60 * 1000) {
                window.removeEventListener("message", messageHandler);
                window.removeEventListener("storage", storageHandler);
                if (pollTimer) {
                    clearInterval(pollTimer);
                    pollTimer = null;
                }
                if (channel) {
                    channel.close();
                    channel = null;
                }
                return;
            }
            var payload = null;
            try {
                payload = parseStoragePayload(localStorage.getItem(storageKey));
            } catch (error) {
                console.warn("[Xsolla SDK] Failed to read localStorage auth payload:", error);
            }
            if (payload && payload.type === "xsolla-social-auth") {
                completeAuth(payload.data);
            }
        }, 500);
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
            var flowId = Date.now().toString() + "-" + Math.random().toString(36).substring(2, 11);
        
            // base popup url (pointer → string)
            var baseUrl = new URL(".", window.location.href);
            var popupUrl = baseUrl.href + "xl-social.html";
            
            // append social network auth url as query parameter
            popupUrl += (popupUrl.indexOf("?") === -1 ? "?" : "&");
            popupUrl += "social_network_url=" + encodeURIComponent(socialNetworkAuthUrl);
            popupUrl += "&sdk_flow_id=" + encodeURIComponent(flowId);
            
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

            var storageKey = "xsolla-auth-" + flowId;
            var isCompleted = false;
            var pollTimer = null;
            var channel = null;
            var authStartTime = Date.now();

            var completeAuth = function (code) {
                if (isCompleted || !code) {
                    return;
                }

                isCompleted = true;
                window.removeEventListener("message", messageHandler);
                window.removeEventListener("storage", storageHandler);
                if (pollTimer) {
                    clearInterval(pollTimer);
                    pollTimer = null;
                }
                if (channel) {
                    channel.close();
                    channel = null;
                }
                try {
                    localStorage.removeItem(storageKey);
                } catch (error) {
                    console.warn("[Xsolla SDK] Failed to clear localStorage auth key:", error);
                }

                console.log("[Xsolla SDK] Auth code received:", code);
                SendMessage("XsollaWebCallbacks", "PublishWidgetAuthSuccess", code);
            };
        
            var messageHandler = function (event) {
                if (!event.data || event.data.type !== "xsolla-social-auth") {
                    return;
                }
                if (event.data.flowId && event.data.flowId !== flowId) {
                    return;
                }
                completeAuth(event.data.data);
            };

            var parseStoragePayload = function (rawValue) {
                if (!rawValue) {
                    return null;
                }
                try {
                    return JSON.parse(rawValue);
                } catch (error) {
                    console.warn("[Xsolla SDK] Failed to parse localStorage auth payload:", error);
                    return null;
                }
            };

            var storageHandler = function (event) {
                if (event.key !== storageKey || !event.newValue) {
                    return;
                }
                var payload = parseStoragePayload(event.newValue);
                if (!payload || payload.type !== "xsolla-social-auth") {
                    return;
                }
                completeAuth(payload.data);
            };
            
            window.addEventListener("message", messageHandler, false);
            window.addEventListener("storage", storageHandler, false);

            if (typeof BroadcastChannel !== "undefined") {
                channel = new BroadcastChannel("xsolla-auth-" + flowId);
                channel.onmessage = function (event) {
                    if (!event.data || event.data.type !== "xsolla-social-auth") {
                        return;
                    }
                    completeAuth(event.data.data);
                };
            }

            pollTimer = setInterval(function () {
                if (isCompleted) {
                    return;
                }
                if ((popup && popup.closed) || Date.now() - authStartTime > 10 * 60 * 1000) {
                    window.removeEventListener("message", messageHandler);
                    window.removeEventListener("storage", storageHandler);
                    if (pollTimer) {
                        clearInterval(pollTimer);
                        pollTimer = null;
                    }
                    if (channel) {
                        channel.close();
                        channel = null;
                    }
                    return;
                }
                var payload = null;
                try {
                    payload = parseStoragePayload(localStorage.getItem(storageKey));
                } catch (error) {
                    console.warn("[Xsolla SDK] Failed to read localStorage auth payload:", error);
                }
                if (payload && payload.type === "xsolla-social-auth") {
                    completeAuth(payload.data);
                }
            }, 500);
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

    OpenXsollaAdditionalInfoAuthPopup: function (loginUrlPtr) {
        var loginUrl = UTF8ToString(loginUrlPtr);
        var flowId = Date.now().toString() + "-" + Math.random().toString(36).substring(2, 11);

        var baseUrl = new URL(".", window.location.href);
        var popupUrl = baseUrl.href + "xl-additional-info.html";
        popupUrl += (popupUrl.indexOf("?") === -1 ? "?" : "&");
        popupUrl += "login_url=" + encodeURIComponent(loginUrl);
        popupUrl += "&sdk_flow_id=" + encodeURIComponent(flowId);

        const popupName = 'Xsolla Additional Info Auth ' + Date.now();
        var popup = window.open(
            popupUrl,
            popupName,
            'width=600, height=700, resizable=yes, scrollbars=yes'
        );

        if (!popup || popup.closed || typeof popup.closed === 'undefined') {
            SendMessage("XsollaWebCallbacks", "PublishAdditionalInfoAuthCancel", "popup_blocked");
            alert("The popup window was blocked by the browser. Please allow popups for this site.");
            return;
        }

        var storageKey = "xsolla-auth-" + flowId;
        var isCompleted = false;
        var pollTimer = null;
        var channel = null;
        var authStartTime = Date.now();

        var cleanup = function () {
            window.removeEventListener("message", messageHandler);
            window.removeEventListener("storage", storageHandler);
            if (pollTimer) {
                clearInterval(pollTimer);
                pollTimer = null;
            }
            if (channel) {
                channel.close();
                channel = null;
            }
            try {
                localStorage.removeItem(storageKey);
            } catch (error) {
                console.warn("[Xsolla SDK] Failed to clear localStorage auth key:", error);
            }
        };

        var completeAuth = function (authData) {
            if (isCompleted || !authData || (!authData.code && !authData.token && !authData.error)) {
                return;
            }

            isCompleted = true;
            cleanup();
            SendMessage("XsollaWebCallbacks", "PublishAdditionalInfoAuthSuccess", JSON.stringify(authData));
        };

        var cancelAuth = function (reason) {
            if (isCompleted) {
                return;
            }

            isCompleted = true;
            cleanup();
            SendMessage("XsollaWebCallbacks", "PublishAdditionalInfoAuthCancel", reason || "cancelled");
        };

        var messageHandler = function (event) {
            if (event.origin !== window.location.origin) {
                return;
            }
            if (!event.data || event.data.type !== "xsolla-additional-info-auth") {
                return;
            }
            if (event.data.flowId && event.data.flowId !== flowId) {
                return;
            }
            completeAuth(event.data.data);
        };

        var parseStoragePayload = function (rawValue) {
            if (!rawValue) {
                return null;
            }
            try {
                return JSON.parse(rawValue);
            } catch (error) {
                console.warn("[Xsolla SDK] Failed to parse localStorage auth payload:", error);
                return null;
            }
        };

        var storageHandler = function (event) {
            if (event.key !== storageKey || !event.newValue) {
                return;
            }
            var payload = parseStoragePayload(event.newValue);
            if (!payload || payload.type !== "xsolla-additional-info-auth") {
                return;
            }
            completeAuth(payload.data);
        };

        window.addEventListener("message", messageHandler, false);
        window.addEventListener("storage", storageHandler, false);

        if (typeof BroadcastChannel !== "undefined") {
            channel = new BroadcastChannel("xsolla-auth-" + flowId);
            channel.onmessage = function (event) {
                if (!event.data || event.data.type !== "xsolla-additional-info-auth") {
                    return;
                }
                completeAuth(event.data.data);
            };
        }

        pollTimer = setInterval(function () {
            if (isCompleted) {
                return;
            }
            if ((popup && popup.closed)) {
                cancelAuth("popup_closed");
                return;
            }
            if (Date.now() - authStartTime > 10 * 60 * 1000) {
                cancelAuth("timeout");
                return;
            }

            var payload = null;
            try {
                payload = parseStoragePayload(localStorage.getItem(storageKey));
            } catch (error) {
                console.warn("[Xsolla SDK] Failed to read localStorage auth payload:", error);
            }
            if (payload && payload.type === "xsolla-additional-info-auth") {
                completeAuth(payload.data);
            }
        }, 500);
    },

    OpenXsollaAdditionalInfoAuthPopupWithConfirmation: function (loginUrlPtr, popupMessageTextPtr, continueButtonTextPtr, cancelButtonTextPtr) {
        var loginUrl = UTF8ToString(loginUrlPtr);
        var popupMessageText = UTF8ToString(popupMessageTextPtr);
        var continueButtonText = UTF8ToString(continueButtonTextPtr);
        var cancelButtonText = UTF8ToString(cancelButtonTextPtr);
        
        var openPopup = function () {
            var flowId = Date.now().toString() + "-" + Math.random().toString(36).substring(2, 11);
            var baseUrl = new URL(".", window.location.href);
            var popupUrl = baseUrl.href + "xl-additional-info.html";
            popupUrl += (popupUrl.indexOf("?") === -1 ? "?" : "&");
            popupUrl += "login_url=" + encodeURIComponent(loginUrl);
            popupUrl += "&sdk_flow_id=" + encodeURIComponent(flowId);
    
            const popupName = 'Xsolla Additional Info Auth ' + Date.now();
            var popup = window.open(
                popupUrl,
                popupName,
                'width=600, height=700, resizable=yes, scrollbars=yes'
            );
    
            if (!popup || popup.closed || typeof popup.closed === 'undefined') {
                SendMessage("XsollaWebCallbacks", "PublishAdditionalInfoAuthCancel", "popup_blocked");
                alert("The popup window was blocked by the browser. Please allow popups for this site.");
                return;
            }
    
            var storageKey = "xsolla-auth-" + flowId;
            var isCompleted = false;
            var pollTimer = null;
            var channel = null;
            var authStartTime = Date.now();
    
            var cleanup = function () {
                window.removeEventListener("message", messageHandler);
                window.removeEventListener("storage", storageHandler);
                if (pollTimer) {
                    clearInterval(pollTimer);
                    pollTimer = null;
                }
                if (channel) {
                    channel.close();
                    channel = null;
                }
                try {
                    localStorage.removeItem(storageKey);
                } catch (error) {
                    console.warn("[Xsolla SDK] Failed to clear localStorage auth key:", error);
                }
            };
    
            var completeAuth = function (authData) {
                if (isCompleted || !authData || (!authData.code && !authData.token && !authData.error)) {
                    return;
                }
    
                isCompleted = true;
                cleanup();
                SendMessage("XsollaWebCallbacks", "PublishAdditionalInfoAuthSuccess", JSON.stringify(authData));
            };
    
            var cancelAuth = function (reason) {
                if (isCompleted) {
                    return;
                }
    
                isCompleted = true;
                cleanup();
                SendMessage("XsollaWebCallbacks", "PublishAdditionalInfoAuthCancel", reason || "cancelled");
            };
    
            var messageHandler = function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                if (!event.data || event.data.type !== "xsolla-additional-info-auth") {
                    return;
                }
                if (event.data.flowId && event.data.flowId !== flowId) {
                    return;
                }
                completeAuth(event.data.data);
            };
    
            var parseStoragePayload = function (rawValue) {
                if (!rawValue) {
                    return null;
                }
                try {
                    return JSON.parse(rawValue);
                } catch (error) {
                    console.warn("[Xsolla SDK] Failed to parse localStorage auth payload:", error);
                    return null;
                }
            };
    
            var storageHandler = function (event) {
                if (event.key !== storageKey || !event.newValue) {
                    return;
                }
                var payload = parseStoragePayload(event.newValue);
                if (!payload || payload.type !== "xsolla-additional-info-auth") {
                    return;
                }
                completeAuth(payload.data);
            };
    
            window.addEventListener("message", messageHandler, false);
            window.addEventListener("storage", storageHandler, false);
    
            if (typeof BroadcastChannel !== "undefined") {
                channel = new BroadcastChannel("xsolla-auth-" + flowId);
                channel.onmessage = function (event) {
                    if (!event.data || event.data.type !== "xsolla-additional-info-auth") {
                        return;
                    }
                    completeAuth(event.data.data);
                };
            }
    
            pollTimer = setInterval(function () {
                if (isCompleted) {
                    return;
                }
                if ((popup && popup.closed)) {
                    cancelAuth("popup_closed");
                    return;
                }
                if (Date.now() - authStartTime > 10 * 60 * 1000) {
                    cancelAuth("timeout");
                    return;
                }
    
                var payload = null;
                try {
                    payload = parseStoragePayload(localStorage.getItem(storageKey));
                } catch (error) {
                    console.warn("[Xsolla SDK] Failed to read localStorage auth payload:", error);
                }
                if (payload && payload.type === "xsolla-additional-info-auth") {
                    completeAuth(payload.data);
                }
            }, 500);
        };

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
            SendMessage("XsollaWebCallbacks", "PublishAdditionalInfoAuthCancel", "confirmation_cancelled");
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
            document.body.removeChild(overlay);
            openPopup();
        });

        popup.appendChild(cancelButton);
        popup.appendChild(continueButton);
        overlay.appendChild(popup);
        document.body.appendChild(overlay);
    },
});
