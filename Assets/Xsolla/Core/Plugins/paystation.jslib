mergeInto(LibraryManager.library, {

	OpenPayStationWidget: function (token, sandbox, engineVersion, sdkVersion, applePayMerchantDomain) {
		var jsToken = UTF8ToString(token);
		var isSandbox = sandbox > 0 ? true : false;
		var options = {
			access_token: jsToken,
			sandbox: isSandbox,
			lightbox: {
				width: '740px',
				height: '760px',
				spinner: 'round',
				spinnerColor: '#cccccc',
			},
			queryParams: {
                sdk: 'store',
                sdk_v: UTF8ToString(sdkVersion),
                engine: 'unity',
                engine_v: UTF8ToString(engineVersion),
                browser_type: 'iframe',
                build_platform: 'webglplayer',
            }
		};
		
		var jsParentDomain = UTF8ToString(applePayMerchantDomain);
		if (jsParentDomain !== "") {
            options.queryParams.parent_domain = jsParentDomain;
        }

		var s = document.createElement('script');
		s.type = "text/javascript";
		s.async = true;
		s.src = "https://cdn.xsolla.net/payments-bucket-prod/embed/1.5.0/widget.min.js";

		s.addEventListener('load', function (e) {
			XPayStationWidget.on(XPayStationWidget.eventTypes.STATUS, function (event, data) {
				Module.SendMessage('XsollaWebCallbacks', 'PublishPaymentStatusUpdate');
			});

            XPayStationWidget.on(XPayStationWidget.eventTypes.CLOSE, function (event, data) {
				if (data === undefined) {
					Module.SendMessage('XsollaWebCallbacks', 'PublishPaymentCancel');
				}
				else {
					Module.SendMessage('XsollaWebCallbacks', 'PublishPaymentStatusUpdate');
				}
			});

			XPayStationWidget.init(options);
			XPayStationWidget.open();
		}, false);

		var head = document.getElementsByTagName('head')[0];
		head.appendChild(s);
	},

	ClosePayStationWidget: function () {
		if (typeof XPayStationWidget !== undefined) {
			XPayStationWidget.off();
		}

		var elements = document.getElementsByClassName('xpaystation-widget-lightbox');
		for (var i = 0; i < elements.length; i++) {
			elements[i].style.display = 'none';
			elements[i].remove();
		}
	},
	
	ShowPopupAndOpenPayStation: function (url, popupMessage, continueButtonText, cancelButtonText) {
        var jsUrl = UTF8ToString(url);
        var jsPopupMessage = UTF8ToString(popupMessage);
        var jsContinueButtonText = UTF8ToString(continueButtonText);
        var jsCancelButtonText = UTF8ToString(cancelButtonText);

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
        messageElement.textContent = jsPopupMessage;
        messageElement.style.margin = '20px';
        messageElement.style.fontSize = '16px';
        popup.appendChild(messageElement);

        var cancelButton = document.createElement('button');
        cancelButton.textContent = jsCancelButtonText;
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
        continueButton.textContent = jsContinueButtonText;
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
            window.open(jsUrl, '_blank');
        });

        popup.appendChild(cancelButton);
        popup.appendChild(continueButton);
        overlay.appendChild(popup);
        document.body.appendChild(overlay);
    },
    
    GetUserAgent: function () {
        var str = navigator.userAgent;
        var lengthBytes = lengthBytesUTF8(str) + 1;
        var stringOnWasmHeap = _malloc(lengthBytes);
        stringToUTF8(str, stringOnWasmHeap, lengthBytes);
        return stringOnWasmHeap;
    },
    
    GetBrowserLanguage: function () {
        var language = navigator.language || navigator.userLanguage;
        var length = lengthBytesUTF8(language) + 1;
        var buffer = _malloc(length);
        stringToUTF8(language, buffer, length);
        return buffer;
    },
});