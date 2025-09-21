//const searchConnections = {};

//function toggleSearchChat(button) {
//    const requestId = button.getAttribute('data-request-id');
//    console.log("appendMessage is", typeof appendMessage);
//    const chatBox = document.getElementById(`searchChat-${requestId}`);
//    if (!chatBox) return;

//    const isVisible = chatBox.style.display === 'block';

//    // Hide all chats
//    document.querySelectorAll('.search-messages').forEach(el => {
//        el.style.display = 'none';
//    });

//    if (!isVisible) {
//        chatBox.style.display = 'block';

//        // Create SignalR connection if not exists
//        if (!searchConnections[requestId]) {
//            const connection = new signalR.HubConnectionBuilder()
//                .withUrl(`/searchChatHub?requestId=${requestId}`)
//                .build();

//            connection.on("ReceiveMessage", function (sender, message) {
//                const messageBox = document.getElementById(`messagesBox-${requestId}`);
//                if (!messageBox) return;

//                const msg = document.createElement("div");
//                msg.classList.add("message-item");
//                msg.innerHTML = `<strong>${sender}:</strong> ${message}`;
//                messageBox.appendChild(msg);
//                messageBox.scrollTop = messageBox.scrollHeight;

//                const currentUser = document.getElementById("currentUsername").value;
//                // ✅ Use helper only (no duplicate raw append)
//                appendMessage(requestId, sender, message, currentUser);
//            });

//            connection.start().then(() => {
//                searchConnections[requestId] = connection;
//                console.log("Connected to SearchChatHub for request:", requestId);
//            }).catch(err => console.error("SignalR connection error:", err.toString()));
//        }
//    }
//}

//async function sendSearchMessage(event, requestId) {
//    event.preventDefault();
//    const currentUser = document.getElementById("currentUsername").value;
//    const sender = document.getElementById("currentUsername").value; // email
//    const input = document.getElementById(`messageText-${requestId}`);
//    const message = input.value.trim();
//    console.log("appendMessage is", typeof appendMessage);
//    if (!message) return;

//    input.value = "";

//    const connection = searchConnections[requestId];
//    if (connection) {
//        console.log("Invoking SendMessage:", { requestId, sender, message });
//        await connection.invoke("SendMessage", requestId.toString(), sender, message); // ✅ pass sender
//    } else {
//        console.error("No connection found for requestId:", requestId);
//    }

//    // 👇 Reuse helper
//    // ✅ Use helper only (no duplicate raw append)

//    appendMessage(requestId, sender, message, currentUser);
//}

const searchConnections = {};

// Toggle chat box
function toggleSearchChat(button) {
    const requestId = button.getAttribute('data-request-id');
    const chatBox = document.getElementById(`searchChat-${requestId}`);
    if (!chatBox) return;

    const isVisible = chatBox.style.display === 'block';

    // Hide all chats
    document.querySelectorAll('.search-messages').forEach(el => {
        el.style.display = 'none';
    });

    if (!isVisible) {
        chatBox.style.display = 'block';

        // Create SignalR connection if not exists
        if (!searchConnections[requestId]) {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(`/searchChatHub?requestId=${requestId}`)
                .build();

            connection.on("ReceiveMessage", function (sender, message) {
                const currentUser = document.getElementById("currentUsername").value;
                console.log("sender and currentUser:", sender + " " + currentUser);
                // 🔥 Ignore server echo if it's me (prevents duplicate messages)
                if (sender === currentUser) return;

                appendMessage(requestId, sender, message, currentUser);
            });

            connection.start().then(() => {
                searchConnections[requestId] = connection;
                console.log("Connected to SearchChatHub for request:", requestId);
            }).catch(err => console.error("SignalR connection error:", err.toString()));
        }
    }
}

// Send a message
async function sendSearchMessage(event, requestId) {
    event.preventDefault();

    const currentUser = document.getElementById("currentUsername").value;
    const sender = currentUser;
    const input = document.getElementById(`messageText-${requestId}`);
    const message = input.value.trim();

    if (!message) return;

    input.value = "";

    const connection = searchConnections[requestId];
    if (connection) {
        console.log("Invoking SendMessage:", { requestId, sender, message });
        await connection.invoke("SendMessage", requestId.toString(), sender, message);
    } else {
        console.error("No connection found for requestId:", requestId);
    }

    // ✅ Show instantly in UI
    appendMessage(requestId, sender, message, currentUser);
}

// Append message with style
function appendMessage(requestId, sender, message, currentUser, timestamp = null) {
    const messagesBox = document.getElementById(`messagesBox-${requestId}`);
    if (!messagesBox) return;

    const isMe = sender === currentUser;
    const cssClass = isMe ? "sent" : "received";

    const messageDiv = document.createElement("div");
    messageDiv.className = `message-item ${cssClass}`;
    console.log("sender and currentUser:", sender + " " + currentUser);
   
    messageDiv.innerHTML = `
        <div class="bubble">
            ${!isMe ? `<strong>${sender}:</strong>` : ""}
            ${message}
            <div class="timestamp">${timestamp || new Date().toLocaleString()}</div>
        </div>
    `;

    messagesBox.appendChild(messageDiv);
    messagesBox.scrollTop = messagesBox.scrollHeight;
}
