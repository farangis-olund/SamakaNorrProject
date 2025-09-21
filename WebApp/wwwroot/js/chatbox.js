
//const connection = new signalR.HubConnectionBuilder()
//	.withUrl("/chatHub?rideId=" + rideId)
//	.build();

//connection.on("ReceiveMessage", function (sender, message) {
//	const messageBox = document.getElementById("messagesBox");
//	const msg = document.createElement("div");
//	msg.classList.add("message-item");
//	msg.innerHTML = `<strong>${sender}:</strong> ${message}`;
//	messageBox.appendChild(msg);
//	messageBox.scrollTop = messageBox.scrollHeight;
//});

//connection.start().catch(err => console.error(err.toString()));

//async function sendMessage(event) {
//	event.preventDefault();
//	const input = document.getElementById("messageText");
//	const message = input.value;
//	input.value = "";

//	await connection.invoke("SendMessage", rideId, sender, receiver, message);


//}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub?rideId=" + rideId)
    .build();

connection.on("ReceiveMessage", function (sender, message) {
    const currentUser = document.getElementById("currentUsername").value;

    // 🔥 Skip if the sender is me (prevents duplicates)
    if (sender === currentUser) return;

    appendRideMessage(sender, message, currentUser);
});

connection.start().then(() => {
    console.log("✅ Connected to ride ChatHub:", rideId);
}).catch(err => console.error("❌ Connection error:", err.toString()));

async function sendMessage(event) {
    event.preventDefault();
    const input = document.getElementById("messageText");
    const message = input.value.trim();
    input.value = "";

    if (!message) return;

    const currentUser = document.getElementById("currentUsername").value;

    // 🚀 Send to server
    await connection.invoke("SendMessage", rideId, currentUser, receiver, message);

    // ✅ Show immediately in UI
    appendRideMessage(currentUser, message, currentUser);
}

// Reusable function to append message with style
function appendRideMessage(sender, message, currentUser, timestamp = null) {
    const messageBox = document.getElementById("messagesBox");
    if (!messageBox) return;

    const isMe = sender === currentUser;
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

    messageBox.appendChild(msg);
    messageBox.scrollTop = messageBox.scrollHeight;
}
