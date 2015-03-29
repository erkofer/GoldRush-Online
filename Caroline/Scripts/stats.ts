module Statistics {
    var statsPane;
    var itemsBody;
    var items = new Array<Item>();
    class Item {
        id: number;
        prestigeQuantity: number;
        lifetimeQuantity: number;
        alltimeRow: HTMLElement;
        prestigeRow: HTMLElement;

        constructor() {

        }
    }

    export function changeStats(id: number, prestige: number, lifetime: number) {
        var item = items[id];
        item.prestigeQuantity = prestige?prestige:item.prestigeQuantity;
        item.lifetimeQuantity = lifetime?lifetime:item.lifetimeQuantity;

        if(lifetime)
            Objects.setLifeTimeTotal(id, lifetime);

        item.prestigeRow.textContent = prestige ? Utils.formatNumber(prestige):'0';
        item.alltimeRow.textContent = lifetime ? Utils.formatNumber(lifetime):'0';
    }

    export function addItem(id:number,name:string) {
        if (!statsPane)
            draw();


        if (!items[id]) {
            var item = new Item();
            items[id] = item;

            var row = <HTMLTableRowElement>itemsBody.insertRow(itemsBody.rows.length);
            row.classList.add('table-row');
            item.alltimeRow = row.insertCell(0);
            item.alltimeRow.style.width = '40%';
            item.prestigeRow = row.insertCell(0);
            item.prestigeRow.style.width = '40%';
            var imageRow = row.insertCell(0);
            imageRow.style.width = '20%';
            var image = document.createElement('DIV');
            image.classList.add('Third-' + Utils.cssifyName(name));
            image.style.display = 'inline-block';
            imageRow.appendChild(image);
        }
    }

    function draw() {
        statsPane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(statsPane);
        Tabs.registerGameTab(statsPane, 'Statistics');

        statsPane.appendChild(drawItemsTable());
    }

    function drawItemsTable() {
        var itemsTable = document.createElement('TABLE');

        var header = (<HTMLTableElement>itemsTable).createTHead();
        var titleRow = (<HTMLTableElement>header).insertRow(0);
        titleRow.classList.add('table-header');
        var titleCell = (<HTMLTableRowElement>titleRow).insertCell(0);
        titleCell.textContent = 'Item Statistics';
        titleCell.setAttribute('colspan', '3');

        var descriptionsRow = (<HTMLTableElement>header).insertRow(1);
        descriptionsRow.classList.add('table-subheader');
        var lifetime = (<HTMLTableRowElement>descriptionsRow).insertCell(0);
        lifetime.textContent = 'Lifetime Quantity';
        lifetime.style.width = '40%';
        var prestige = (<HTMLTableRowElement>descriptionsRow).insertCell(0);
        prestige.textContent = 'Prestige Quantity';
        prestige.style.width = '40%';
        var item = (<HTMLTableRowElement>descriptionsRow).insertCell(0);
        item.textContent = 'Item';
        item.style.width = '20%';

        itemsBody = (<HTMLTableElement>itemsTable).createTBody();

        return itemsTable;
    }
} 