module Store {
    var storePane:HTMLElement;
    var items = new Array<StoreItem>();
    export var categories = new Array<HTMLElement>();
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
        name: string;

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

    export function addItem(id: number, category: Category, price: number, factor: number, name: string) {
        if (!storePane)
            draw();

        var item = new StoreItem();
        item.id = id;
        item.category = category;
        item.price = price;
        item.factor = factor;
        item.name = name;

        var categoryContainer = categories[Category[category]];
        categoryContainer.appendChild(drawItem(item));
    }

    

    function add() {

    }

    function drawItem(item: StoreItem):HTMLElement {
        var itemContainer = document.createElement('div');
        itemContainer.classList.add('store-item');
        var header = document.createElement('div');
        header.classList.add('store-item-header');
        header.textContent = item.name;
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
        footer.textContent = Utils.formatNumber(item.price);
        itemContainer.appendChild(footer);

        return itemContainer;
    }
}