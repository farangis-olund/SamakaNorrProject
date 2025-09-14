
    document.addEventListener("DOMContentLoaded", async function () {
    try {
        // 🔹 Fetch villages dynamically from backend
        const response = await fetch('/api/locations');
    if (!response.ok) throw new Error("Failed to fetch villages");

    const villages = await response.json();

    // Initialize Fuse.js for fuzzy search
    const fuse = new Fuse(villages, {
        includeScore: true,
    threshold: 0.4
        });

    function setupAutocomplete(inputId) {
            const inputField = document.getElementById(inputId);
    const suggestionBox = document.createElement('div');
    suggestionBox.className = 'suggestion-box';
    let currentFocus = -1;

    const parent = inputField.closest('.input-group');
    parent.style.position = 'relative';
    parent.appendChild(suggestionBox);

    inputField.addEventListener('input', function (e) {
                const query = e.target.value;
    suggestionBox.innerHTML = '';
    currentFocus = -1;

                if (query.length > 2) {
                    const results = fuse.search(query);
                    results.forEach((result, index) => {
                        const suggestionItem = document.createElement('div');
    suggestionItem.textContent = result.item;
    suggestionItem.className = 'suggestion-item';
    suggestionItem.setAttribute('data-index', index);

                        suggestionItem.addEventListener('click', () => {
        inputField.value = result.item;
    suggestionBox.innerHTML = '';
                        });

    suggestionBox.appendChild(suggestionItem);
                    });
                }
            });

    inputField.addEventListener('keydown', function (e) {
                const items = suggestionBox.querySelectorAll('.suggestion-item');
    if (e.key === 'ArrowDown') {
        currentFocus++;
                    if (currentFocus >= items.length) currentFocus = 0;
    updateFocus(items);
                } else if (e.key === 'ArrowUp') {
        currentFocus--;
    if (currentFocus < 0) currentFocus = items.length - 1;
    updateFocus(items);
                } else if (e.key === 'Enter') {
        e.preventDefault();
                    if (currentFocus > -1 && items[currentFocus]) {
        inputField.value = items[currentFocus].textContent;
    suggestionBox.innerHTML = '';
                    }
                } else if (e.key === 'Escape') {
        suggestionBox.innerHTML = '';
    currentFocus = -1;
                }
            });

    function updateFocus(items) {
        items.forEach(item => item.classList.remove('active'));
                if (currentFocus > -1) {
        items[currentFocus].classList.add('active');
    items[currentFocus].scrollIntoView({block: 'nearest' });
                }
            }

            inputField.addEventListener('blur', () => {
        setTimeout(() => {
            suggestionBox.innerHTML = '';
            currentFocus = -1;
        }, 200);
            });
        }

    // Setup autocomplete for both fields
    setupAutocomplete('origin');
    setupAutocomplete('destination');

    } catch (err) {
        console.error("❌ Error loading villages:", err);
    }
});

