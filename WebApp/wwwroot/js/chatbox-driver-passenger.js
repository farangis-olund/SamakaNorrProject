const connections = {};

function toggleChat(button) {
    const rideId = button.getAttribute('data-ride-id');
    const chatBox = document.getElementById(`rideChat-${rideId}`);
    const isVisible = chatBox.style.display === 'block';

    document.querySelectorAll('.ride-messages').forEach(el => {
        el.style.display = 'none';
    });

    if (!isVisible) {
        chatBox.style.display = 'block';

        const badge = button.querySelector('.badge-unread');
        if (badge) {
            badge.remove();
        }

        fetch('/Message/MarkAsRead', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            },
            body: JSON.stringify(rideId)
        }).then(res => {
            if (!res.ok) console.error('Failed to mark messages as read.');
        });

        if (!connections[rideId]) {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(`/chatHub?rideId=${rideId}`)
                .build();

            connection.on("ReceiveMessage", function (sender, message) {
                const messageBox = document.getElementById(`messagesBox-${rideId}`);
                if (!messageBox) return;

                const msg = document.createElement("div");
                msg.classList.add("message-item");
                msg.innerHTML = `<strong>${sender}:</strong> ${message}`;
                messageBox.appendChild(msg);
                messageBox.scrollTop = messageBox.scrollHeight;
            });

            connection.start()
                .then(() => {
                    connections[rideId] = connection;
                })
                .catch(err => console.error(err.toString()));
        }
    } else {
        chatBox.style.display = 'none';
    }
}

async function sendMessage(event, rideId) {
    event.preventDefault();

    const sender = document.getElementById("currentUsername")?.value;
    const receiver = document.querySelector(`#chatButton-${rideId}`).getAttribute("data-receiver");
    const input = document.getElementById(`messageText-${rideId}`);
    const message = input.value;
    input.value = "";

    const connection = connections[rideId];
    if (connection) {
        await connection.invoke("SendMessage", rideId, sender, receiver, message);
    }
}

