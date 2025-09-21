window.appendMessage = function (requestId, sender, message,  currentUser) {
    console.log("appendMessage called with:", { requestId, sender, message, currentUser });

    const messagesBox = document.getElementById(`messagesBox-${requestId}`);
    if (!messagesBox) return;

    const cssClass = sender === currentUser ? "sent" : "received";

    const messageDiv = document.createElement("div");
    messageDiv.className = `message-item ${cssClass}`;
    messageDiv.innerHTML = `
        <div class="bubble">
            ${sender !== currentUser ? `<strong>${sender}:</strong>` : ""} 
            ${message}
            
        </div>
    `;

    messagesBox.appendChild(messageDiv);
    messagesBox.scrollTop = messagesBox.scrollHeight;
};
