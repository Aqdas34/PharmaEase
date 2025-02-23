//"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

////Disable the send button until connection is established.
////document.getElementById("SendMessage").disabled = true;

//connection.on("ReceiveMessage", function () {
//    //var li = document.createElement("li");
//    //document.getElementById("messagesList").appendChild(li);
//    // We can assign user-supplied strings to an element's textContent because it
//    // is not interpreted as markup. If you're assigning in any other way, you
//    // should be aware of possible script injection concerns.
//    //li.textContent = message;
//    alert(message);
//});

////connection.start().then(function () {
////    document.getElementById("sendButton").disabled = false;
////}).catch(function (err) {
////    return console.error(err.toString());
////});

//document.getElementById("SendMessage").addEventListener("click", function (event) {
//    // var message = document.getElementById("messageInput").value;
//    alert("Hello");
//    connection.invoke("SendMessage").catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});



