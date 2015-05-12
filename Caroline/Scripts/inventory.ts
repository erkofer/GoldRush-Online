///<reference path="utils.ts"/>
///<reference path="tooltip.ts"/>
///<reference path="tabs.ts"/>
///<reference path="objects.ts"/>
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
    var configTableContainer: HTMLElement;
    var drinkButton;

    var configNames = new Array<HTMLElement>();
    var configImages = new Array<HTMLElement>();

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
            if (selectedItem.category == Category.POTION) {
                drinkButton.style.display = 'inline-block';
            } else {
                drinkButton.style.display = 'none';
            }
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
        Objects.register(item.id, item.name);

        if (!inventoryPane)
            draw();

        if (item.category != Category.NFS && item.category != null)
            inventory.appendChild(drawItem(item));
        else
            document.getElementById('headerInventory').appendChild(drawItem(item));
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

        drinkButton = <HTMLInputElement>Utils.createButton('Drink', ''); 
        drinkButton.classList.add('selected-item-quantity');
        drinkButton.addEventListener('click', function () {
            Connection.drink(selectedItem.id);
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
        selectedItemPane.appendChild(drinkButton);
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
            var isValueProperty = parseInt(enumMember, 10) >= 0;
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
        Equipment.draw();
    }

    export function modifyConfig(id: number, enabled: boolean) {
        (<HTMLInputElement>configClickers[id]).checked = enabled;
    }

    export function toggleConfig() {
        if (configTableContainer.classList.contains('closed')) {
            configTableContainer.classList.remove('closed');
        }
        else
            configTableContainer.classList.add('closed');
    }

    function drawHeaderItem(item: Item): HTMLElement {
        var itemElement = document.createElement('DIV');
        item.container = itemElement;

        itemElement.classList.add('header-item');

        var itemImage = document.createElement('DIV');
        itemImage.style.width = '32px';
        itemImage.style.height = '32px';
        itemImage.style.display = 'inline-block';
        itemImage.style.position = 'relative';
        itemImage.style.margin = '0 auto';
        var image = document.createElement('DIV');
        image.classList.add('Half-'+item.name.replace(' ', '_'));
        itemImage.appendChild(image);
        itemElement.appendChild(itemImage);

        var itemQuantity = document.createElement('DIV');
        item.quantityElm = itemQuantity;
        itemQuantity.classList.add("item-text");
        itemQuantity.textContent = Utils.formatNumber(0);
        itemQuantity.style.verticalAlign = 'top';
        itemQuantity.style.marginTop = '8px';
        itemQuantity.style.display = 'inline-block'
        itemElement.appendChild(itemQuantity);

        var itemValue = document.createElement('DIV');
        itemValue.style.display = 'none';
        item.worthElm = itemValue;
        itemElement.appendChild(itemValue);

        return itemElement;
    }

    function drawItem(item: Item): HTMLElement {
        if (item.category == Category.NFS || item.category == null) 
            return drawHeaderItem(item);

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
        itemCurrency.classList.add('Quarter-Coins');
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
                    var isValueProperty = parseInt(enumMember, 10) >= 0;
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
                selectedConfigCell = (<HTMLTableElement>row).cells[cellIndex - 1];

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
            nameAndImage.style.height = 'auto';
            var nameSpan = document.createElement('SPAN');
            nameSpan.style.verticalAlign = 'top';
            var image = document.createElement('DIV');
            image.classList.add('Third-' + Utils.cssifyName(item.name));
            image.style.display = 'inline-block';
            configImages[item.id] = image;
            nameSpan.textContent = item.name;
            configNames[item.id] = nameSpan;
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

    export function changePrice(id: number, price: number) {
        Utils.ifNotDefault(price, function () {
            var item = items[id];
            item.worth = price;
            item.worthElm.textContent = Utils.formatNumber(price);
        });
    }

    export function changeQuantity(id: number, quantity: number) {
        Utils.ifNotDefault(quantity, function () {
            Objects.setQuantity(id, quantity);
            Crafting.update();
            items[id].quantityElm.textContent = Utils.formatNumber(quantity);
            items[id].quantity = quantity;
            if (items[id].category != Category.NFS && items[id].category != null)
                items[id].container.style.display = quantity == 0 ? 'none' : 'inline-block';
            else
                items[id].container.style.display = Objects.getLifeTimeTotal(id) == 0 ? 'none' : 'inline-block';
            limitTextQuantity();
        });
    }

    function update() {
        if (configNames.length <= 0) return;

        items.forEach(item => {
            var itemQuantity = Objects.getLifeTimeTotal(item.id);
            if (configNames[item.id])
                configNames[item.id].textContent = itemQuantity > 0 ? item.name : '???';

            if (configImages[item.id])
                configImages[item.id].style.display = itemQuantity > 0 ? 'inline-block' : 'none';
        });
    }
    setInterval(update, 1000);
}