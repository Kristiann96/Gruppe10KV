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