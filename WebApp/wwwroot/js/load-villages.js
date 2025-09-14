//async function loadVillages() {
//    try {
//        const response = await fetch('/api/locations');
//        if (!response.ok) throw new Error("Failed to fetch villages");

//        const villages = await response.json();

//        // Get selects
//        const originSelect = document.getElementById('origin');
//        const destinationSelect = document.getElementById('destination');

//        // Populate options
//        villages.forEach(village => {
//            originSelect.add(new Option(village, village));
//            destinationSelect.add(new Option(village, village));
//        });

//        // Turn selects into searchable comboboxes
//        new Choices(originSelect, {
//            searchEnabled: true,
//            itemSelectText: '',
//            placeholderValue: 'Välj ort',
//        });

//        new Choices(destinationSelect, {
//            searchEnabled: true,
//            itemSelectText: '',
//            placeholderValue: 'Välj ort',
//        });

//    } catch (err) {
//        console.error("Error loading villages:", err);
//    }
//}

//document.addEventListener("DOMContentLoaded", loadVillages);

async function loadVillages() {
    try {
        const response = await fetch('/api/locations');
        if (!response.ok) throw new Error("Failed to fetch villages");

        const villages = await response.json();

        // Get all selects with .village-select
        const selects = document.querySelectorAll('.village-select');

        // Populate both origin & destination selects
        selects.forEach(select => {
            select.innerHTML = ""; // clear old options

            // Add placeholder option
            select.add(new Option("Välj ort", ""));

            // Add villages
            villages.forEach(village => {
                select.add(new Option(village, village));
            });
        });

        // Now initialize Choices.js for both selects
        selects.forEach(select => {
            new Choices(select, {
                searchEnabled: true,
                placeholderValue: 'Välj ort',
                itemSelectText: '',
            });
        });

    } catch (err) {
        console.error("Error loading villages:", err);
    }
}

document.addEventListener("DOMContentLoaded", loadVillages);
