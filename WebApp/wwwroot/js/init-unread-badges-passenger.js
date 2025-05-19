function initializeUnreadBadges() {
    document.querySelectorAll("[data-ride-id]").forEach(button => {
        const rideId = button.getAttribute("data-ride-id");
        fetch(`/Message/GetUnreadCountForPassenger?rideId=${rideId}`)
            .then(res => res.json())
            .then(count => {
                const badge = document.getElementById(`unreadBadge-${rideId}`);
                if (badge) {
                    if (count > 0) {
                        badge.innerText = count;
                        badge.style.display = "inline-block";
                    } else {
                        badge.style.display = "none";
                    }
                }
            });
    });
}

document.addEventListener('DOMContentLoaded', initializeUnreadBadges);
