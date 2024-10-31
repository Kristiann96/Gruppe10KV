/* function sortTableByDate() {
    const table = document.querySelector("table tbody");
    const rows = Array.from(table.querySelectorAll("tr"));

    // Toggle sort direction between ascending and descending
    let sortDirection = table.getAttribute("data-sort-direction") === "asc" ? "desc" : "asc";
    table.setAttribute("data-sort-direction", sortDirection);

    // Find the header element and set arrows
    const header = document.getElementById("sortSisteEndring");
    header.textContent = `SISTE ENDRING ${sortDirection === "asc" ? "▲" : "▼"}`;

    rows.sort((a, b) => {
        const dateA = parseDate(a.cells[5].textContent);
        const dateB = parseDate(b.cells[5].textContent);

        return sortDirection === "asc" ? dateA - dateB : dateB - dateA;
    });

    // Re-append rows in sorted order
    rows.forEach(row => table.appendChild(row));
}

// Helper function to parse date strings in "dd.MM.yyyy HH:mm" format
function parseDate(dateString) {
    const [datePart, timePart] = dateString.split(" ");
    const [day, month, year] = datePart.split(".").map(Number);
    const [hours, minutes] = timePart.split(":").map(Number);
    return new Date(year, month - 1, day, hours, minutes);
}
*/





/* 
// Function to sort the table by the specified column and direction
function sortTable(column, direction) {
    const table = document.querySelector("table tbody");
    const rows = Array.from(table.querySelectorAll("tr"));

    // Determine if sorting by date or by ID
    const isDateColumn = column === "date";

    // Set sort direction based on dropdown selection
    let sortDirection = direction === "asc" || direction === "latest" ? "asc" : "desc";

    // Sort rows based on column type and chosen direction
    rows.sort((a, b) => {
        const valueA = isDateColumn ? parseDate(a.cells[5].textContent) : parseInt(a.cells[0].textContent, 10);
        const valueB = isDateColumn ? parseDate(b.cells[5].textContent) : parseInt(b.cells[0].textContent, 10);

        if (sortDirection === "asc") {
            return valueA - valueB;
        } else {
            return valueB - valueA;
        }
    });

    // Re-append sorted rows to the table body
    rows.forEach(row => table.appendChild(row));
}

// Helper function to parse date strings in "dd.MM.yyyy HH:mm" format
function parseDate(dateString) {
    const [datePart, timePart] = dateString.split(" ");
    const [day, month, year] = datePart.split(".").map(Number);
    const [hours, minutes] = timePart.split(":").map(Number);
    return new Date(year, month - 1, day, hours, minutes);
}
*/





/*
let originalRows = null;

document.addEventListener('DOMContentLoaded', function() {
    // Store original table state when page loads
    const tbody = document.querySelector('table tbody');
    originalRows = Array.from(tbody.children);

    // Add click outside listener
    document.addEventListener('click', function(event) {
        if (!event.target.closest('.header-container')) {
            closeAllDropdowns();
        }
    });
});

function closeAllDropdowns() {
    const dropdowns = document.querySelectorAll('.dropdown-content');
    dropdowns.forEach(dropdown => {
        dropdown.classList.remove('active');
    });
}

function toggleDropdown(dropdownId, event) {
    event.stopPropagation();

    // Close all other dropdowns
    const allDropdowns = document.querySelectorAll('.dropdown-content');
    allDropdowns.forEach(dropdown => {
        if (dropdown.id !== dropdownId) {
            dropdown.classList.remove('active');
        }
    });

    // Toggle the clicked dropdown
    const dropdown = document.getElementById(dropdownId);
    dropdown.classList.toggle('active');
}

function sortTable(column, direction, event) {
    event.stopPropagation();

    const table = document.querySelector("table tbody");
    const rows = Array.from(table.querySelectorAll("tr"));

    // Show the clear button for this column
    const headerContainer = event.target.closest('.header-container');
    headerContainer.querySelector('.clear-button').classList.add('active');

    // Add active state to header
    headerContainer.classList.add('active');

    // Determine if sorting by date or by ID
    const isDateColumn = column === "date";

    // Set sort direction
    const sortDirection = direction === "asc" || direction === "latest" ? 1 : -1;

    // Sort rows
    rows.sort((a, b) => {
        let valueA, valueB;

        if (isDateColumn) {
            valueA = parseDate(a.cells[5].textContent);
            valueB = parseDate(b.cells[5].textContent);
        } else {
            valueA = parseInt(a.cells[0].textContent, 10);
            valueB = parseInt(b.cells[0].textContent, 10);
        }

        return (valueA - valueB) * sortDirection;
    });

    // Clear the table
    while (table.firstChild) {
        table.removeChild(table.firstChild);
    }

    // Add sorted rows
    rows.forEach(row => table.appendChild(row));

    // Close the dropdown
    closeAllDropdowns();
}

function clearSort(column, event) {
    event.stopPropagation();

    const tbody = document.querySelector('table tbody');

    // Clear the table
    while (tbody.firstChild) {
        tbody.removeChild(tbody.firstChild);
    }

    // Restore original rows
    originalRows.forEach(row => {
        tbody.appendChild(row.cloneNode(true));
    });

    // Hide clear button and remove active state
    const headerContainer = event.target.closest('.header-container');
    headerContainer.querySelector('.clear-button').classList.remove('active');
    headerContainer.classList.remove('active');
}

function parseDate(dateString) {
    const [datePart, timePart] = dateString.split(" ");
    const [day, month, year] = datePart.split(".").map(Number);
    const [hours, minutes] = timePart.split(":").map(Number);
    return new Date(year, month - 1, day, hours, minutes);
}
*/
