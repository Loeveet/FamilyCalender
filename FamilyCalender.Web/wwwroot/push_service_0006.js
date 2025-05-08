  
/* Notiserna registreras av notification.service.js där vi frågar användfaren om behörighet
samt registrera enheten till backend. Denna service har hand om att ta emot notiserna och presentera dom*/

self.addEventListener('install', function (event) {
    console.log('Service Worker installerad.');
    self.skipWaiting();
});

self.addEventListener('activate', function (event) {
    console.log('Service Worker aktiverad.');
});


  self.addEventListener("push", function(event) {
    if (event.data) {

      var payload = JSON.parse(event.data.text());
      console.log(payload);
      //url.push(payload.url); redirect??
      return self.registration.showNotification(payload.Title, {
        body: payload.Body,
        icon: "images/favicon-192x192.png", 
        tag: payload.Title,
        data: {
            url: payload.url
        }
      });
    } else {
      console.log("Push event but no data");
    }
  });

self.addEventListener("notificationclick", function (event) {
    event.notification.close();

    const urlToOpen = event.notification.data?.url;

    if (urlToOpen) {
        event.waitUntil(
            clients.matchAll({ type: "window", includeUncontrolled: true }).then(function (clientList) {
                // Om fönster redan är öppet, fokusera det
                for (let i = 0; i < clientList.length; i++) {
                    const client = clientList[i];
                    if (client.url === urlToOpen && "focus" in client) {
                        return client.focus();
                    }
                }
                // Annars öppna nytt
                if (clients.openWindow) {
                    return clients.openWindow(urlToOpen);
                }
            })
        );
    }
});
