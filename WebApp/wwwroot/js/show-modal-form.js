document.getElementById("addRideButton").addEventListener("click", function () {
    document.getElementById("addRideModal").style.display = "block";
});

document.getElementById("closeModal").addEventListener("click", function () {
    document.getElementById("addRideModal").style.display = "none";
});

window.addEventListener("click", function (event) {
    if (event.target == document.getElementById("addRideModal")) {
        document.getElementById("addRideModal").style.display = "none";
    }
});

document.getElementById("free").addEventListener("change", function () {
    const priceField = document.getElementById("price");
    if (this.checked) {
        priceField.value = "";
        priceField.setAttribute("disabled", "disabled");
    } else {
        priceField.removeAttribute("disabled");
    }
});