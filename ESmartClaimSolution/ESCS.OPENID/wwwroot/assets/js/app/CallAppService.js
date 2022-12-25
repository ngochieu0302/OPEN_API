function CallAppService() {
    var _service = new Service();
    this.callVideo = function () {
        var _instance = this;
        _service.postData("/callapp/videocall", {}).then(access_token => {
            console.log(access_token);
            console.log('StringeeUtil.isWebRTCSupported: ' + StringeeUtil.isWebRTCSupported());
            var client = new StringeeClient();
            client.connect(access_token);

            client.on('connect', function () {
                console.log('connected');
            });
            client.on('authen', function (res) {
                console.log('authen', res); 
            });
            client.on('disconnect', function () {
                console.log('disconnected');
            });
            client.on('incomingcall', function (incomingcall) {
                incomingcall.ringing(function (res) { });
                console.log('++++++++++++++ incomingcall', incomingcall);
            });
            client.on('requestnewtoken', function () {
                console.log('++++++++++++++ requestnewtoken; please get new access_token from YourServer and call client.connect(new_access_token)+++++++++');
            });

            var call = new StringeeCall(client, "842477788802", "0972878870", true);
            console.log(call);
            call.ringing(function (res) {
                console.log(res);
            });
        });
    },
    this.settingCallEvent = function (call) {
        var _instance = this; 
        call.on('addremotestream', function (stream) {
            // reset srcObject to work around minor bugs in Chrome and Edge.
            console.log('addremotestream');
            remoteVideo.srcObject = null;
            remoteVideo.srcObject = stream;
        });

        call.on('addlocalstream', function (stream) {
            // reset srcObject to work around minor bugs in Chrome and Edge.
            console.log('addlocalstream');
            localVideo.srcObject = null;
            localVideo.srcObject = stream;
        });

        call.on('signalingstate', function (state) {
            console.log('signalingstate ', state);
            var reason = state.reason;
            $('#callStatus').html(reason);
        });

        call.on('mediastate', function (state) {
            console.log('mediastate ', state);
        });

        call.on('info', function (info) {
            console.log('on info:' + JSON.stringify(info));
        });
    }
}