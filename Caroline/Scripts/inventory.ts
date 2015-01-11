///<reference path="utils.ts"/>
///<reference path="tooltip.ts"/>
///<reference path="tabs.ts"/>
module Inventory {
    export var items = new Array<Item>();
    var inventoryPane: HTMLElement;
    var inventory: HTMLElement;
    var selectedItemPane:HTMLElement;
    var selectedItemImage: HTMLElement;
    var selectedItem:Item;
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
        var sellAll = <HTMLInputElement>Utils.createButton('Sell (...)', '');
        sellAll.addEventListener('click', function () {
            Connection.sellAllItems();
        });
        var sellAllConfig = <HTMLInputElement>Utils.createButton('...','');
        sellAllConfig.addEventListener('click', function () {
            toggleConfig();
        });
        configPanel.appendChild(sellAll);
        configPanel.appendChild(sellAllConfig);
        configDiv.appendChild(configPanel);
        inventoryPane.appendChild(configDiv);
        // CONFIG TABLE

        inventory = document.createElement('DIV');
        inventoryPane.appendChild(inventory);

        Tabs.registerGameTab(inventoryPane,'Inventory');
    }

    function toggleConfig() {

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