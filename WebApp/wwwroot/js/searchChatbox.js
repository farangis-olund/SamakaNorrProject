const searchConnections = {};

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
                const messageBox = document.getElementById(`messagesBox-${requestId}`);
                if (!messageBox) return;

                const msg = document.createElement("div");
                msg.classList.add("message-item");
                msg.innerHTML = `<strong>${sender}:</strong> ${message}`;
                messageBox.appendChild(msg);
                messageBox.scrollTop = messageBox.scrollHeight;
            });

            connection.start().then(() => {
                searchConnections[requestId] = connection;
                console.log("Connected to SearchChatHub for request:", requestId);
            }).catch(err => console.error("SignalR connection error:", err.toString()));
        }
    }
}

async function sendSearchMessage(event, requestId) {
    event.preventDefault();

    const sender = document.getElementById("currentUsername").value; // email
    const input = document.getElementById(`messageText-${requestId}`);
    const message = input.value.trim();

    if (!message) return;

    input.value = "";

    const connection = searchConnections[requestId];
    if (connection) {
        console.log("Invoking SendMessage:", { requestId, sender, message });
        await connection.invoke("SendMessage", requestId.toString(), sender, message); // ✅ pass sender
    } else {
        console.error("No connection found for requestId:", requestId);
    }
}
