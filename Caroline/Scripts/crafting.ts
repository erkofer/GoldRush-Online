module Crafting {
    var storePane: HTMLElement;
    var processorSection: HTMLElement;
    var craftingSection: HTMLElement;
    var cellDescriptions = ['Action', 'Description', 'Input', 'Output', 'Name'];
    var cellWidths = ['10%', '30%', '15%', '15%', '10%'];

    function draw() {
        storePane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(storePane);
        Tabs.registerGameTab(storePane, 'Crafting');

        processorSection = document.createElement('DIV');
        storePane.appendChild(processorSection);

        craftingSection = document.createElement('DIV');
        storePane.appendChild(craftingSection);
        drawCraftingTable();
    }

    function drawCraftingTable() {
        var craftingTable = document.createElement('TABLE');
        craftingTable.classList.add('block-table');

        var header = (<HTMLTableElement>craftingTable).createTHead();
        var titleRow = (<HTMLTableElement>header).insertRow(0);
        titleRow.classList.add('table-subheader');
        var realTitleRow = (<HTMLTableElement>header).insertRow(0);
        realTitleRow.classList.add('table-header');

        var titleCell = (<HTMLTableRowElement>realTitleRow).insertCell(0);
        (<HTMLTableCellElement>titleCell).colSpan = cellDescriptions.length;
        titleCell.textContent = 'Crafting Table';

        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = (<HTMLTableRowElement>titleRow).insertCell(0);
            cell.style.width = cellWidths[i];
            cell.textContent = cellDescriptions[i];
        }

        craftingSection.appendChild(craftingTable);
    }

    export function addCraftingItem() {
        if (!storePane) draw();
    }

    export function addCraftingUpgrade() {
        if (!storePane) draw();
    }

}