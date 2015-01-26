///<reference path="utils.ts"/>
///<reference path="tooltip.ts"/>
///<reference path="tabs.ts"/>
module Inventory {
    export var items = new Array<Item>();
    export var configClickers = new Array<HTMLElement>();
    var inventoryPane: HTMLElement;
    var inventory: HTMLElement;
    var selectedItemPane:HTMLElement;
    var selectedItemImage: HTMLElement;
    var selectedItem: Item;
    //var configDiv;
    var configTableBody: HTMLElement;
    var configTableContainer:HTMLElement;
    export class Item {
        id: number;
        name: string;
        quantity: number;
        worth: number;
        category: Category;

        container: HTMLElement;
        worthElm: HTMLElement;
        quantityElm: HTMLElement;

        constructor(id: number, name: string, worth: number, category: Category) {
            this.id = id;
            this.name = name;
            this.worth = worth;
            this.category = category;
        }
    }

    export enum Category {
        NFS = 0,
        ORE = 1,
        GEM = 2,
        INGREDIENT = 3,
        CRAFTING = 4,
        POTION = 5
    };

    export function getSelectedItemQuantity(): number {
        return selectedItem ? selectedItem.quantity: 0;
    }

    export function selectItem(id?: number) {
        if (id) {
            if (selectedItem != null) {
                selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            }
            selectedItem = items[id];
            selectedItemImage.classList.add(Utils.cssifyName(selectedItem.name));
            limitTextQuantity();
        }
        else {
            selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            selectedItem = null;
        }

        selectedItemPane.style.display = selectedItem == null ? 'none' : 'block';
    }

    export function sellSelectedItem(quantity?: number) {
        Connection.sellItem(selectedItem.id, quantity ? quantity : 1);
    }

    export function sellAllSelectedItem() {
        sellSelectedItem(selectedItem.quantity);
    }

    function limitTextQuantity() {
        var textbox = <HTMLInputElement>document.getElementById('selecteditemquantity');
        var quantity = +textbox.value;
        if (Utils.isNumber(quantity)) {
            if (quantity > Inventory.getSelectedItemQuantity()) {
                textbox.value = Inventory.getSelectedItemQuantity().toString();
            }
        }
    }

    function add(item: Item) {
        items[item.id] = item;

        if (!inventoryPane)
            draw();

        inventory.appendChild(drawItem(item));
    }

    function draw() {
        // SELECTED ITEM HEADER
        inventoryPane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(inventoryPane);
        selectedItemPane = document.createElement('DIV');
        selectedItemPane.classList.add('selected-item');
        selectedItemPane.style.display = 'none';
        var selectedItemPaneCloser = document.createElement('SPAN');
        selectedItemPaneCloser.textContent = 'x';
        selectedItemPaneCloser.style.top = '0px';
        selectedItemPaneCloser.style.right = '3px';
        selectedItemPaneCloser.style.position = 'absolute';
        selectedItemPaneCloser.addEventListener('click', function () {
            Inventory.selectItem();
        });
        selectedItemPane.appendChild(selectedItemPaneCloser);

        selectedItemImage = document.createElement('DIV');
        selectedItemImage.classList.add('selected-item-image');
        var selectedItemQuantity = <HTMLInputElement>document.createElement('INPUT');
        selectedItemQuantity.id = 'selecteditemquantity';
        selectedItemQuantity.type = 'text';
        selectedItemQuantity.style.height = '18px';
        selectedItemQuantity.classList.add('selected-item-quantity');
        selectedItemQuantity.addEventListener('input', function () {
            limitTextQuantity();
        });

        var sellItems = <HTMLInputElement>Utils.createButton('Sell', '');
        sellItems.classList.add('selected-item-quantity');
        sellItems.addEventListener('click', function () {
            var textbox = <HTMLInputElement>document.getElementById('selecteditemquantity');
            var quantity = +textbox.value;
            if (Utils.isNumber(quantity)) {
                Inventory.sellSelectedItem(quantity);
            }
            limitTextQuantity();
        });
        
        var sellAllItems = <HTMLInputElement>Utils.createButton('Sell all', '');
        sellAllItems.classList.add('selected-item-quantity');
        sellAllItems.addEventListener('click', function () {
            Inventory.sellAllSelectedItem();
            limitTextQuantity();
        });
        

        selectedItemPane.appendChild(selectedItemImage);
        selectedItemPane.appendChild(sellAllItems);

        selectedItemPane.appendChild(sellItems);
        selectedItemPane.appendChild(selectedItemQuantity);
        inventoryPane.appendChild(selectedItemPane);
        // CONFIG CONTROLS
        var configDiv = document.createElement('DIV');
        configDiv.style.textAlign = 'center';
        var configPanel = document.createElement('DIV');
        configPanel.style.display = 'inline-block';
        var sellAll = Utils.createButton('Sell (...)', '');
        sellAll.addEventListener('click', function () {
            Connection.sellAllItems();
        });
        var sellAllConfig = Utils.createButton('...','');
        sellAllConfig.addEventListener('click', function () {
            Inventory.toggleConfig();
        });
        configPanel.appendChild(sellAll);
        configPanel.appendChild(sellAllConfig);
        configDiv.appendChild(configPanel);
        inventoryPane.appendChild(configDiv);
        // CONFIG TABLE
        configTableContainer = document.createElement('DIV');
        configTableContainer.classList.add('config-container');
        configTableContainer.classList.add('closed');
        var configTable = document.createElement('TABLE');
        configTable.classList.add('config-table');
        configTableContainer.appendChild(configTable);
        
        var header = (<HTMLTableElement>configTable).createTHead();
        var titleRow = (<HTMLTableElement>header).insertRow(0);
        var realTitleRow = (<HTMLTableElement>header).insertRow(0);
        realTitleRow.classList.add('table-header');
        var titleCell = (<HTMLTableRowElement>realTitleRow).insertCell(0);
        titleCell.textContent = 'Inventory Configuration';
        titleCell.setAttribute('colspan', '10');
        titleRow.classList.add('table-subheader');
        // TODO: finish config table
        for (var enumMember in Category) {
            var isValueProperty = parseInt(enumMember, 10) >= 0
            if (isValueProperty) {
                if (Category[enumMember] != "NFS") { // if the item is in fact for sale.
                    var configCell = (<HTMLTableRowElement>titleRow).insertCell((<HTMLTableRowElement>titleRow).cells.length);
                    configCell.classList.add('config-cell-check');
                    

                    var titleCell = (<HTMLTableRowElement>titleRow).insertCell((<HTMLTableRowElement>titleRow).cells.length);
                    titleCell.classList.add('config-cell-name');
                    titleCell.textContent = Category[enumMember];
                }
            }
        }
        configTableBody = (<HTMLTableElement>configTable).createTBody();

        inventory = document.createElement('DIV');
        inventory.style.position = 'relative';
        inventory.appendChild(configTableContainer);
        inventoryPane.appendChild(inventory);


        Tabs.registerGameTab(inventoryPane, 'Inventory');
    }

    export function modifyConfig(id: number, enabled: boolean) {
        if (!configClickers[id])
            console.log(id);
        (<HTMLInputElement>configClickers[id]).checked = enabled;
    }

    export function toggleConfig() {
        if (configTableContainer.classList.contains('closed')) {
            configTableContainer.classList.remove('closed')
        }
        else 
            configTableContainer.classList.add('closed')
    }

    function drawItem(item: Item): HTMLElement {
        var itemElement = document.createElement('DIV');
        item.container = itemElement;
        itemElement.classList.add("item");
        itemElement.addEventListener('click', function () {
            Inventory.selectItem(item.id);
        });
        tooltip.create(itemElement, item.name);
        // VALUE
        var itemValueContainer = document.createElement('DIV');
        itemValueContainer.classList.add("item-text");
        var itemValue = document.createElement('DIV');
        itemValue.style.verticalAlign = 'top';
        itemValue.style.display = 'inline-block';
        itemValue.textContent = Utils.formatNumber(item.worth);
        item.worthElm = itemValue;
        var itemCurrency = document.createElement('DIV');
        itemCurrency.classList.add('Third-Coins');
        itemCurrency.style.display = 'inline-block';
        itemValueContainer.appendChild(itemCurrency);
        itemValueContainer.appendChild(itemValue);
        itemElement.appendChild(itemValueContainer);
        // IMAGE
        var itemImage = document.createElement('DIV');
        itemImage.style.width = '64px';
        itemImage.style.height = '64px';
        itemImage.style.position = 'relative';
        itemImage.style.margin = '0 auto';
        var image = document.createElement('DIV');
        image.classList.add(item.name.replace(' ', '_'));
        itemImage.appendChild(image);
        itemElement.appendChild(itemImage);
        // QUANTITY 
        var itemQuantity = document.createElement('DIV');
        item.quantityElm = itemQuantity;
        itemQuantity.classList.add("item-text");
        itemQuantity.textContent = Utils.formatNumber(0);
        itemElement.appendChild(itemQuantity);
        // CONFIG TABLE
        if (item.category != null) {
            var selectedItemCell;
            var selectedConfigCell;
            var rows = (<HTMLTableElement>configTableBody).rows.length;
            var cellIndex = item.category;
            cellIndex *= 2;
            cellIndex--;

            for (var i = 0; i < rows; i++) {
                // FIX THIS.
                var testCell = (<HTMLTableElement>(<HTMLTableElement>configTableBody).rows[i]).cells[cellIndex];
                if (testCell && testCell.childElementCount == 0) { // if we have an empty place to write.
                    selectedItemCell = (<HTMLTableElement>(<HTMLTableElement>configTableBody).rows[i]).cells[cellIndex];
                    selectedConfigCell = (<HTMLTableElement>(<HTMLTableElement>configTableBody).rows[i]).cells[cellIndex - 1];
                    break;
                }
            }
            if (!selectedItemCell) { // we need a new row.
                var row = (<HTMLTableElement>configTableBody).insertRow((<HTMLTableElement>configTableBody).rows.length);
                row.classList.add('table-row');
                for (var enumMember in Category) {
                    var isValueProperty = parseInt(enumMember, 10) >= 0
                    if (isValueProperty) {
                        if (Category[enumMember] != "NFS") { // if the item is in fact for sale.
                            var configCell = (<HTMLTableRowElement>row).insertCell((<HTMLTableRowElement>row).cells.length);
                            configCell.classList.add('config-cell-check');

                            var titleCell = (<HTMLTableRowElement>row).insertCell((<HTMLTableRowElement>row).cells.length);
                            titleCell.classList.add('config-cell-name');
                        }
                    }
                }
                selectedItemCell = (<HTMLTableElement>row).cells[cellIndex];
                selectedConfigCell = (<HTMLTableElement>row).cells[cellIndex-1];
            }
            /* For doing stuff with empty cells.
            for (var curRow = 0; curRow < rows; curRow++) {
                var inspectedRow = (<HTMLTableElement>configTableBody).rows[i];
                var cells = (<HTMLTableElement>inspectedRow).cells.length;
                for (var curCell = 0; curCell < cells; curCell++) {

                }
            }*/

            var nameAndImage = document.createElement('DIV');
            nameAndImage.classList.add('item-text');
            var nameSpan = document.createElement('SPAN');
            nameSpan.style.verticalAlign = 'top';
            var image = document.createElement('DIV');
            image.classList.add('Third-' + Utils.cssifyName(item.name));
            image.style.display = 'inline-block';
            nameSpan.textContent = item.name;
            nameAndImage.appendChild(image);
            nameAndImage.appendChild(nameSpan);
            (<HTMLElement>selectedItemCell).appendChild(nameAndImage);
            var configChecker = <HTMLInputElement>document.createElement('INPUT');
            configChecker.type = 'CHECKBOX';
            var id = item.id;
            configClickers[id] = configChecker;
            configChecker.addEventListener('change', function (e) {
                Connection.configureItem(id, (<HTMLInputElement>configClickers[id]).checked);
            });
            (<HTMLElement>selectedConfigCell).appendChild(configChecker);

        }

        return itemElement;
    }

    export function addItem(id: number, name: string, worth: number, category: number) {
        if(!items[id]) // If we haven't already added this item.
            add(new Item(id, name, worth,category));
    }

    export function changeQuantity(id: number, quantity: number) {
        items[id].quantityElm.textContent = Utils.formatNumber(quantity);
        items[id].quantity = quantity;
        items[id].container.style.display = quantity == 0 ? 'none' : 'inline-block';
        limitTextQuantity();
    }
}