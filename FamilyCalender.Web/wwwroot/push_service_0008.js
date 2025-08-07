
/* Notiserna registreras av notification.service.js där vi frågar användfaren om behörighet
samt registrera enheten till backend. Denna service har hand om att ta emot notiserna och presentera dom*/


self.addEventListener('install', function (event) {
    console.log('Service Worker installerad.');
    self.skipWaiting();
});

self.addEventListener('activate', function (event) {
    console.log('Service Worker aktiverad.');
    event.waitUntil(self.clients.claim());
});

self.addEventListener('push', function (event) {
    event.waitUntil(
        (async () => {
            if (event.data) {
                const text = await event.data.text();
                const payload = JSON.parse(text);
                console.log('Push payload:', payload);

                return self.registration.showNotification(payload.Title, {
                    body: payload.Body,
                    icon: "images/favicon-192x192.png",
                    tag: payload.Title,
                    data: {
                        url: payload.url || '/'
                    }
                });
            } else {
                console.log("Push event men utan data");
            }
        })()
    );
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();

    const urlToOpen = event.notification.data?.url || '/';

    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true }).then(function (clientList) {
            for (const client of clientList) {
                // Om fönster redan är öppet och URL innehåller urlToOpen, fokusera det
                if (client.url.includes(urlToOpen) && 'focus' in client) {
                    return client.focus();
                }
            }
            // Om inget fönster öppet, öppna nytt med urlToOpen
            if (clients.openWindow) {
                return clients.openWindow(urlToOpen);
            }
        })
    );
});
