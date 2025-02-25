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
});