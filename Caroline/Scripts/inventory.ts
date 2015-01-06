///<reference path="utils.ts"/>
///<reference path="tooltip.ts"/>

module Inventory {
    var items = new Array<Item>();
    var inventoryPane: HTMLElement;
    var selectedItemPane:HTMLElement;
    var selectedItemImage: HTMLElement;
    var selectedItem:Item;
    class Item {
        id: number;
        name: string;
        quantity: number;
        worth: number;

        container: HTMLElement;
        worthElm: HTMLElement;
        quantityElm: HTMLElement;

        constructor(id: number, name: string, worth: number) {
            this.id = id;
            this.name = name;
            this.worth = worth;
        }
    }

    export function getSelectedItemQuantity(): number {
        return selectedItem ? selectedItem.quantity: 0;
    }

    export function selectItem(id: number) {
        if (id) {
            if (selectedItem != null) {
                selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            }
            selectedItem = items[id];
            selectedItemImage.classList.add(Utils.cssifyName(selectedItem.name));
            selectedItemPane.style.display = 'block';
            limitTextQuantity();
        }
        else {
            selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            selectedItem = null;
            selectedItemPane.style.display = 'none';
        }
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

        inventoryPane.appendChild(drawItem(item));
    }

    function draw() {
        inventoryPane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(inventoryPane);
        selectedItemPane = document.createElement('DIV');
        selectedItemPane.classList.add('selected-item');
        selectedItemImage = document.createElement('DIV');
        selectedItemImage.classList.add('selected-item-image');
        var selectedItemQuantity = <HTMLInputElement>document.createElement('INPUT');
        selectedItemQuantity.id = 'selecteditemquantity';
        selectedItemQuantity.type = 'text';
        selectedItemQuantity.classList.add('selected-item-quantity');
        selectedItemQuantity.addEventListener('input', function () {
            limitTextQuantity();
        });

        var sellItems = <HTMLInputElement>document.createElement('INPUT');
        sellItems.type = 'button';
        sellItems.classList.add('selected-item-quantity');
        sellItems.value = 'Sell';
        sellItems.addEventListener('click', function () {
            var textbox = <HTMLInputElement>document.getElementById('selecteditemquantity');
            var quantity = +textbox.value;
            if (Utils.isNumber(quantity)) {
                Inventory.sellSelectedItem(quantity);
            }
            limitTextQuantity();
        });
        
        var sellAllItems = <HTMLInputElement>document.createElement('INPUT');
        sellAllItems.type = 'button';
        sellAllItems.classList.add('selected-item-quantity');
        sellAllItems.value = 'Sell all';
        sellAllItems.addEventListener('click', function () {
            Inventory.sellAllSelectedItem();
            limitTextQuantity();
        });
        

        selectedItemPane.appendChild(selectedItemImage);
        selectedItemPane.appendChild(sellAllItems);

        selectedItemPane.appendChild(sellItems);
        selectedItemPane.appendChild(selectedItemQuantity);
        inventoryPane.appendChild(selectedItemPane);
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

    export function addItem(id: number, name: string, worth: number) {
        if(!items[id]) // If we haven't already added this item.
            add(new Item(id, name, worth));
    }

    export function changeQuantity(id: number, quantity: number) {
        items[id].quantityElm.textContent = Utils.formatNumber(quantity);
        items[id].quantity = quantity;
        items[id].container.style.display = quantity == 0 ? 'none' : 'inline-block';
        limitTextQuantity();
    }
}