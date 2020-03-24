mergeInto(LibraryManager.library, {
  Purchase: function (token, sandbox) {
       var jsToken = Pointer_stringify(token);

       var isSandbox = sandbox > 0 ? true : false;
       var options = {
            access_token: jsToken,
            sandbox: isSandbox,
            lightbox: {
              width: '740px',
              height: '760px',
              spinner: 'round',
              spinnerColor: '#cccccc'
            }
        };
        var s = document.createElement('script');
        s.type = "text/javascript";
        s.async = true;
        s.src = "https://static.xsolla.com/embed/paystation/1.0.7/widget.min.js";
        s.addEventListener('load', function (e) {
            XPayStationWidget.init(options);
            XPayStationWidget.open();
        }, false);
        var head = document.getElementsByTagName('head')[0];
        head.appendChild(s);
  }
});
