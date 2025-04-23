  
/* Notiserna registreras av notification.service.js där vi frågar användfaren om behörighet
samt registrera enheten till backend. Denna service har hand om att ta emot notiserna och presentera dom*/


  self.addEventListener("push", function(event) {
    if (event.data) {

      var payload = JSON.parse(event.data.text());
      console.log(payload);
      //url.push(payload.url); redirect??
      return self.registration.showNotification(payload.Title, {
        body: payload.Body,
        icon: "images/favicon-192x192.png", 
        tag: payload.Title
      });
    } else {
      console.log("Push event but no data");
    }
  });