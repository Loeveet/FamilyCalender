var notificationService = function () {

    function _urlBase64ToUint8Array(base64String) {

        var padding = "=".repeat((4 - (base64String.length % 4)) % 4);
        var base64 = (base64String + padding).replace(/\-/g, "+").replace(/_/g, "/");
        var rawData = atob(base64);
        var outputArray = new Uint8Array(rawData.length);
        for (var i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    }

    function registerServiceWorker() {
        console.log("trying to register");
        
        navigator.serviceWorker.register("push_service_0007.js", { scope: '/' }).then(function (reg) {
            
            window.Notification.requestPermission().then(function (perm) {
                if (perm !== "granted") {
                    console.log("Permission not granted for Notification");
                }
            });
        });
    }

    var firstAttempt = 1;
    function registerDevice(vapidPublicKey) {
        try {
            //navigator.serviceWorker.register("push_service_0007.js", { scope: '/' }).then(function (reg) {
            //    window.Notification.requestPermission().then(function (perm) {
            //        if (perm !== "granted") {
            //            console.log("Permission not granted for Notification");
            //        }
            //    });

            

                navigator.serviceWorker.ready.then(function (serviceWorkerRegistration) {
                    console.log("ServiceWorkerRegistrátion ready");
                    var applicationServerKey = _urlBase64ToUint8Array(
                        vapidPublicKey
                    );

                    var options = {
                        applicationServerKey: applicationServerKey,
                        userVisibleOnly: true
                    };
                    serviceWorkerRegistration.pushManager.subscribe(options).then(function (subscription) {
                        console.log("Subscribe to push - saving to backend");

                        var body = {
                            payload: subscription
                        };

                        fetch(`/Notification/Register`, {
                            method: 'POST',
                            headers: {
                                'Accept': 'application/json, text/plain, */*',
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify(body)
                        }).then(function (res) {
                            if (res.status === 200) {
                                Swal.fire({
                                    title: 'Push notiser aktiverade!',
                                    text: 'Sidan laddas om för ytterligare inställningar',
                                    icon: 'success',
                                    confirmButtonText: 'OK'
                                }).then((result) => {
                                    location.reload();

                                });
                            } else {
                                if (firstAttempt == 1) {
                                    setTimeout(function () {
                                        firstAttempt = 2;
                                        registerDevice(vapidPublicKey);
                                    }, 500);
                                }
                                else
                                    alert('Det gick inte att registrera dig för pushnotiser, vänligen försöka kadda om sidan igen')
                            }

                        }).catch(function (err) {
                            if (firstAttempt == 1) {
                                setTimeout(function () {
                                    firstAttempt = 2;
                                    registerDevice(vapidPublicKey);
                                }, 500);
                            }
                        });

                    });
                });
           // });

        }
        catch (err) {

        }
    }

    function unregisterPush() {
        if ('serviceWorker' in navigator) {
            
            navigator.serviceWorker.ready
                .then(registration => {
                    return registration.pushManager.getSubscription();
                })
                .then(subscription => {
                    if (subscription) {
                        return subscription.unsubscribe();
                    }
                })
                .then(() => {
                    
                    localStorage.removeItem("userPushSettings");
                    location.href = "/UserSettings"
                    
                })
                .catch(error => {
                    
                    location.href = "/UserSettings"
                });
        }
    }




    return {
        RegisterDevice: registerDevice,
        UnregisterPush: unregisterPush,
        RegisterServiceWorker: registerServiceWorker
    };
}();