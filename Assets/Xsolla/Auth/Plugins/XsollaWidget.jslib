mergeInto(LibraryManager.library, {
    OpenXsollaLoginWidgetPopup: function (projectIdPtr, localePtr) {
        var projectId = UTF8ToString(projectIdPtr);
        var locale = UTF8ToString(localePtr);
        var flowId = Date.now().toString() + "-" + Math.random().toString(36).substring(2, 11);
        
        var basePath = window.location.pathname.replace(/\/[^\/]*$/, '/');
        var popupUrl = window.location.origin + basePath + "xl-widget.html?project_id=" + projectId + "&sdk_flow_id=" + encodeURIComponent(flowId);
        
        if (locale != null && locale !== "") {
            popupUrl += "&locale=" + locale;
        }
                
        var popup = window.open(
            popupUrl,
            'Xsolla Login Widget',
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

        var completeAuth = function (token) {
            if (isCompleted || !token) {
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

            console.log("[Xsolla SDK] Auth token received from widget proxy page:", token);
            SendMessage('XsollaWebCallbacks', 'PublishWidgetAuthSuccess', token);
        };

        var messageHandler = function (event) {
            if (!event.data || event.data.type !== 'xsolla-widget-auth') {
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
            if (!payload || payload.type !== "xsolla-widget-auth") {
                return;
            }
            completeAuth(payload.data);
        };

        window.addEventListener("message", messageHandler, false);
        window.addEventListener("storage", storageHandler, false);

        if (typeof BroadcastChannel !== "undefined") {
            channel = new BroadcastChannel("xsolla-auth-" + flowId);
            channel.onmessage = function (event) {
                if (!event.data || event.data.type !== "xsolla-widget-auth") {
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
            if (payload && payload.type === "xsolla-widget-auth") {
                completeAuth(payload.data);
            }
        }, 500);
    },
    
    OpenXsollaLoginWidgetPopupWithConfirmation: function (projectIdPtr, localePtr, popupMessageTextPtr, continueButtonTextPtr, cancelButtonTextPtr) {
        var projectId = UTF8ToString(projectIdPtr);
        var locale = UTF8ToString(localePtr);
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
        
            var basePath = window.location.pathname.replace(/\/[^\/]*$/, '/');
            var popupUrl = window.location.origin + basePath + "xl-widget.html?project_id=" + projectId + "&sdk_flow_id=" + encodeURIComponent(flowId);
            
            if (locale != null && locale !== "") {
                popupUrl += "&locale=" + locale;
            }
            
            console.log("[Xsolla SDK] PopupUrl: " + popupUrl);
            var popup = window.open(
                popupUrl,
                'Xsolla Login Widget',
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

            var completeAuth = function (token) {
                if (isCompleted || !token) {
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

                console.log("[Xsolla SDK] Auth token received from widget proxy page: " + token);
                SendMessage('XsollaWebCallbacks', 'PublishWidgetAuthSuccess', token);
            };

            var messageHandler = function (event) {
                if (!event.data || event.data.type !== 'xsolla-widget-auth') {
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
                if (!payload || payload.type !== "xsolla-widget-auth") {
                    return;
                }
                completeAuth(payload.data);
            };

            window.addEventListener("message", messageHandler, false);
            window.addEventListener("storage", storageHandler, false);

            if (typeof BroadcastChannel !== "undefined") {
                channel = new BroadcastChannel("xsolla-auth-" + flowId);
                channel.onmessage = function (event) {
                    if (!event.data || event.data.type !== "xsolla-widget-auth") {
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
                if (payload && payload.type === "xsolla-widget-auth") {
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
});
