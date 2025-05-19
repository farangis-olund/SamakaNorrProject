
    document.addEventListener("DOMContentLoaded", function () {
    const villages = [
    "Arvidsjaur", "Abborrträsk", "Bastuträsk", "Glommersträsk", "Järvträsk",
    "Kalixträsk", "Kittelfjäll", "Moskosel", "Myrheden", "Nyborg", "Slagnäs",
    "Sorsele", "Valfors", "Överkalix"
    ];

    const fuse = new Fuse(villages, {
        includeScore: true,
    threshold: 0.4
    });

    function setupAutocomplete(inputId) {
        const inputField = document.getElementById(inputId);
    const suggestionBox = document.createElement('div');
    suggestionBox.className = 'suggestion-box';
    let currentFocus = -1; // Tracks the currently focused suggestion

    // Ensure the parent has `position: relative`
    const parent = inputField.closest('.input-group');
    parent.style.position = 'relative';
    parent.appendChild(suggestionBox);

    inputField.addEventListener('input', function (e) {
            const query = e.target.value;
    suggestionBox.innerHTML = ''; // Clear previous suggestions
    currentFocus = -1; // Reset the focus index
            if (query.length > 2) {
                const results = fuse.search(query);
                results.forEach((result, index) => {
                    const suggestionItem = document.createElement('div');
    suggestionItem.textContent = result.item;
    suggestionItem.className = 'suggestion-item';
    suggestionItem.setAttribute('data-index', index); // Add index for keyboard navigation
                    suggestionItem.addEventListener('click', () => {
        inputField.value = result.item; // Set the selected value
    suggestionBox.innerHTML = ''; // Clear suggestions
                    });
    suggestionBox.appendChild(suggestionItem);
                });
            }
        });

    inputField.addEventListener('keydown', function (e) {
            const items = suggestionBox.querySelectorAll('.suggestion-item');
    if (e.key === 'ArrowDown') {
        // Move down in the suggestion list
        currentFocus++;
                if (currentFocus >= items.length) currentFocus = 0; // Wrap around
    updateFocus(items);
            } else if (e.key === 'ArrowUp') {
        // Move up in the suggestion list
        currentFocus--;
    if (currentFocus < 0) currentFocus = items.length - 1; // Wrap around
    updateFocus(items);
            } else if (e.key === 'Enter') {
        // Select the currently focused item
        e.preventDefault(); // Prevent form submission
                if (currentFocus > -1 && items[currentFocus]) {
        inputField.value = items[currentFocus].textContent;
    suggestionBox.innerHTML = ''; // Clear suggestions
                }
            } else if (e.key === 'Escape') {
        // Clear the suggestions box on Escape key
        suggestionBox.innerHTML = '';
    currentFocus = -1;
            }
        });

    function updateFocus(items) {
        // Remove "active" class from all items
        items.forEach(item => item.classList.remove('active'));
            if (currentFocus > -1) {
        // Add "active" class to the focused item
        items[currentFocus].classList.add('active');
    items[currentFocus].scrollIntoView({block: 'nearest' }); // Ensure visibility
            }
        }

        inputField.addEventListener('blur', () => {
        setTimeout(() => {
            suggestionBox.innerHTML = '';
            currentFocus = -1;
        }, 200);
        });
    }

    setupAutocomplete('origin');
    setupAutocomplete('destination');
});
