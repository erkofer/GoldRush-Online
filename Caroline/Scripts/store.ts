module Store {
    var storePane:HTMLElement;
    var items = new Array<StoreItem>();
    var categories = new Array<HTMLElement>();
    export enum Category {
        MINING=1,
		MACHINES=2,
		GATHERING=3,
		PROCESSING=4,
		ITEMS=5,
		CRAFTING=6
    };

    class StoreItem {
        id: number;
        category: Category;
        price: number;
        factor: number;
        quantity: number;
        maxQuantity: number;
        name: string;
        priceElm: HTMLElement;
        nameElm: HTMLElement;
        container: HTMLElement;

        constructor() {

        }
    }

    function draw() {
        storePane = document.createElement('div');
        document.getElementById('paneContainer').appendChild(storePane);
        Tabs.registerGameTab(storePane, 'Store');

        for (var enumMember in Category) {
            var isValueProperty = parseInt(enumMember, 10) >= 0;
            if (isValueProperty) {
                var name = Category[enumMember];
                if (name != "CRAFTING") {
                    drawCategory(name);
                }
            }
        }
    }

    function drawCategory(name: string) {
        var categoryContainer = document.createElement('div');
        categoryContainer.classList.add('store-category');
        categories[name] = categoryContainer;

        var categoryHeader = document.createElement('div');
        categoryHeader.textContent = name;
        categoryHeader.classList.add('store-category-header');
        categoryContainer.appendChild(categoryHeader);
        storePane.appendChild(categoryContainer);
    }

    export function tempFix() {
        draw();
    }

    export function addItem(id: number, category: Category, price: number, factor: number, name: string, maxQuantity:number) {
        if (!storePane)
            draw();

        if (!items[id]) { // if we haven't already drawn this item.
            var item = new StoreItem();
            item.id = id;
            item.category = category;
            item.price = price;
            item.factor = factor;
            item.name = name;
            item.maxQuantity = maxQuantity ? maxQuantity : 0;

            var categoryContainer = categories[Category[category]];
            if (categoryContainer == null) {
                categoryContainer = categories["MINING"];
            }
            categoryContainer.appendChild(drawItem(item));
            items[id] = item;
        }
    }

    export function changeQuantity(id: number, quantity: number) {
        var item = items[id];
        item.quantity = quantity;
        item.container.style.display = (item.quantity == -1 || item.quantity >= item.maxQuantity && item.maxQuantity > 0) ? 'none' : 'inline-block';
        if (item.maxQuantity && item.maxQuantity > 1)
            item.nameElm.textContent = item.name + ' (' + ((item.quantity) ? item.quantity : 0) + '/' + item.maxQuantity + ')';
        
    }

    function add() {

    }

    function drawItem(item: StoreItem):HTMLElement {
        var itemContainer = document.createElement('div');
        itemContainer.classList.add('store-item');
        item.container = itemContainer;
        var header = document.createElement('div');
        header.classList.add('store-item-header');
        if (item.maxQuantity <= 1) {
            header.textContent = item.name;
        } else {
            header.textContent = item.name+' (' + item.quantity + '/' + item.maxQuantity + ')';
        }
        item.nameElm = header;
        itemContainer.appendChild(header);
        // IMAGE
        var itemImage = document.createElement('DIV');
        itemImage.style.width = '64px';
        itemImage.style.height = '64px';
        itemImage.style.position = 'relative';
        itemImage.style.margin = '0 auto';
        var image = document.createElement('DIV');
        image.classList.add(Utils.cssifyName(item.name));
        itemImage.appendChild(image);
        itemContainer.appendChild(itemImage);

        var footer = document.createElement('div');
        footer.classList.add('store-item-footer');

        var priceContainer = document.createElement('div');
        priceContainer.classList.add('item-text');
        var price = document.createElement('div');
        price.textContent = Utils.formatNumber(item.price);
        price.style.display = 'inline-block';
        price.style.verticalAlign = 'top';
        item.priceElm = price;
        var coins = document.createElement('div');
        coins.classList.add('Third-Coins');
        coins.style.display = 'inline-block';
        priceContainer.appendChild(coins);
        priceContainer.appendChild(price);

        var buttonContainer = document.createElement('div');
        var button = Utils.createButton('Buy', '');
        var id = item.id;
        if (item.category != Category.ITEMS) {
            button.addEventListener('click', function () {
                Connection.purchaseItem(id);
            });
        } else {
            var quantityInput = <HTMLInputElement>document.createElement('INPUT');
            quantityInput.type = 'TEXT';
            quantityInput.style.width = '40px';
            quantityInput.style.height = '15px';
            buttonContainer.appendChild(quantityInput);
            button.addEventListener('click', function () {
                if(Utils.isNumber(quantityInput.value))
                    Connection.purchaseItem(id,+quantityInput.value);
            });
        }

        buttonContainer.appendChild(button);

        footer.appendChild(buttonContainer);
        footer.appendChild(priceContainer);

        

        itemContainer.appendChild(footer);

        return itemContainer;
    }
}