mergeInto(LibraryManager.library, {

	OpenPaystationWidget: function(token, sandbox) {
		var jsToken = Pointer_stringify(token);
		var isSandbox = sandbox > 0 ? true : false;
		var options = {
			access_token: jsToken,
			sandbox: isSandbox,
			lightbox: {
				width: '740px',
				height: '760px',
				spinner: 'round',
				spinnerColor: '#cccccc',
			}
		};

		var s = document.createElement('script');
		s.type = "text/javascript";
		s.async = true;
		s.src = "https://static.xsolla.com/embed/paystation/1.0.7/widget.min.js";

		s.addEventListener('load', function(e) {
			XPayStationWidget.on(XPayStationWidget.eventTypes.STATUS, function(event, data) {
			    var paymentInfo = data.paymentInfo;
			    console.log('payment status update: ' + paymentInfo.status);
				unityInstance.SendMessage('XsollaWebCallbacks', 'PublishPaymentStatusUpdate', JSON.stringify(paymentInfo));
			});

			XPayStationWidget.on(XPayStationWidget.eventTypes.CLOSE, function(event, data) {
				if (data == null) {
				    console.log('payment close');
					unityInstance.SendMessage('XsollaWebCallbacks', 'PublishPaymentCancel');
				}
			});

			XPayStationWidget.init(options);
			XPayStationWidget.open();
		}, false);

		var head = document.getElementsByTagName('head')[0];
		head.appendChild(s);
	},

	ClosePaystationWidget: function() {
		if (typeof XPayStationWidget !== 'undefined') {
			XPayStationWidget.off();
		}

		var elements = document.getElementsByClassName('xpaystation-widget-lightbox');
		while (elements.length > 0) {
			elements[0].parentNode.removeChild(elements[0]);
		}
	},
});