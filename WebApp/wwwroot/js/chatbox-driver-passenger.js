
//const connections = {};

//function toggleChat(button) {
//    const rideId = button.getAttribute('data-ride-id');
//    const bookingId = button.getAttribute('data-booking-id');

//    const chatBox = document.getElementById(`rideChat-${rideId}-booking-${bookingId}`);
//    if (!chatBox) {
//        console.warn(`Chat box not found: rideChat-${rideId}-booking-${bookingId}`);
//        return;
//    }

//    const isVisible = chatBox.style.display === 'block';

//    document.querySelectorAll('.ride-messages').forEach(el => {
//        el.style.display = 'none';
//    });

//    if (!isVisible) {
//        chatBox.style.display = 'block';

//        const badge = button.querySelector('.badge-unread');
//        if (badge) badge.remove();

//        // Mark messages as read
//        fetch('/Message/MarkAsRead', {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json',
//                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
//            },
//            body: JSON.stringify(rideId)
//        });

//        // SignalR connection using only rideId
//        if (!connections[rideId]) {
//            const connection = new signalR.HubConnectionBuilder()
//                .withUrl(`/chatHub?rideId=${rideId}`)
//                .build();

//            connection.on("ReceiveMessage", function (sender, message) {
//                const messageBox = document.getElementById(`messagesBox-${rideId}-booking-${bookingId}`);
//                if (!messageBox) return;

//                const msg = document.createElement("div");
//                msg.classList.add("message-item");
//                msg.innerHTML = `<strong>${sender}:</strong> ${message}`;
//                messageBox.appendChild(msg);
//                messageBox.scrollTop = messageBox.scrollHeight;
//            });

//            connection.start().then(() => {
//                connections[rideId] = connection;
//            }).catch(err => console.error(err.toString()));
//        }
//    } else {
//        chatBox.style.display = 'none';
//    }
//}


//async function sendMessage(event, rideId) {
//    event.preventDefault();

//    const sender = document.getElementById("currentUsername")?.value;
//    const receiver = document.querySelector(`#chatButton-${rideId}`)?.getAttribute("data-receiver");
//    const input = document.getElementById(`messageText-${rideId}`);
//    const message = input.value;
//    console.log("Sending message with the following data:");
//    console.log("rideId:", rideId);
//    console.log("sender:", sender);
//    console.log("receiver:", receiver);
//    console.log("message:", message);

//    input.value = "";

//    const connection = connections[rideId];
//    if (connection) {
//        await connection.invoke("SendMessage", rideId, sender, receiver, message);
//    } else {
//        console.error("No connection found for rideId:", rideId);
//    }
//}

const connections = {};

function toggleChat(button) {
    const rideId = button.getAttribute('data-ride-id');
    const bookingId = button.getAttribute('data-booking-id');

    const chatBox = document.getElementById(`rideChat-${rideId}-booking-${bookingId}`);
    if (!chatBox) return;

    const isVisible = chatBox.style.display === 'block';

    // Hide all open chats
    document.querySelectorAll('.ride-messages').forEach(el => {
        el.style.display = 'none';
    });

    if (!isVisible) {
        chatBox.style.display = 'block';

        const badge = button.querySelector('.badge-unread');
        if (badge) badge.remove();

        // Mark as read
        fetch('/Message/MarkAsRead', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            },
            body: JSON.stringify(rideId)
        });

        // SignalR connection
        if (!connections[rideId]) {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(`/chatHub?rideId=${rideId}`)
                .build();

            connection.on("ReceiveMessage", function (sender, message) {
                const currentUser = document.getElementById("currentUsername").value;
                const isMe = sender === currentUser;

                appendRideMessage(rideId, bookingId, sender, message, isMe);
            });

            connection.start().then(() => {
                connections[rideId] = connection;
                console.log(`✅ Connected to ChatHub for ride ${rideId}`);
            }).catch(err => console.error("❌ SignalR error:", err.toString()));
        }
    } else {
        chatBox.style.display = 'none';
    }
}

async function sendMessage(event, rideId) {
    event.preventDefault();

    const sender = document.getElementById("currentUsername")?.value;
    const receiver = document.querySelector(`#chatButton-${rideId}`)?.getAttribute("data-receiver");
    const input = document.getElementById(`messageText-${rideId}`);
    const message = input.value.trim();
    const bookingId = event.target.closest(".ride-messages").getAttribute("id").split("-").pop();

    if (!message) return;
    input.value = "";

    const connection = connections[rideId];
    if (connection) {
        await connection.invoke("SendMessage", rideId, sender, receiver, message);
    } else {
        console.error("❌ No connection found for rideId:", rideId);
    }

    // ✅ Show instantly in UI
    appendRideMessage(rideId, bookingId, sender, message, true);
}

function appendRideMessage(rideId, bookingId, sender, message, isMe, timestamp = null) {
    const messagesBox = document.getElementById(`messagesBox-${rideId}-booking-${bookingId}`);
    if (!messagesBox) return;

    const cssClass = isMe ? "sent" : "received";

    const msg = document.createElement("div");
    msg.className = `message-item ${cssClass}`;
    msg.innerHTML = `
        <div class="bubble">
            ${!isMe ? `<strong>${sender}:</strong>` : ""}
            ${message}
            <div class="timestamp">${timestamp || new Date().toLocaleString()}</div>
        </div>
    `;

    messagesBox.appendChild(msg);
    messagesBox.scrollTop = messagesBox.scrollHeight;
}

