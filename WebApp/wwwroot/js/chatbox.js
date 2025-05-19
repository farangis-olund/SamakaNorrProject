
const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub?rideId=" + rideId)
	.build();

connection.on("ReceiveMessage", function (sender, message) {
	const messageBox = document.getElementById("messagesBox");
	const msg = document.createElement("div");
	msg.classList.add("message-item");
	msg.innerHTML = `<strong>${sender}:</strong> ${message}`;
	messageBox.appendChild(msg);
	messageBox.scrollTop = messageBox.scrollHeight;
});

connection.start().catch(err => console.error(err.toString()));

async function sendMessage(event) {
	event.preventDefault();
	const input = document.getElementById("messageText");
	const message = input.value;
	input.value = "";

	await connection.invoke("SendMessage", rideId, sender, receiver, message);
}
