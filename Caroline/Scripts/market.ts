module Market {
    var marketPane;
    var orders = new Array<Order>();
    import Long = dcodeIO.Long;

    export class Order {
        constructor() {

        }

        buying: boolean;
        closed: boolean = true;

        panel: HTMLElement;
        header: HTMLElement;
        item: HTMLElement;
        quantity: HTMLElement;
        offer: HTMLElement;
        summary: HTMLElement;

        summarize() {
            var item = $(this.item).val();
            var quantity = $(this.quantity).val();
            var offer = $(this.offer).val();

            if (!Inventory.isItem(item)) return; // if there is no items by that name, return.
            if (!Utils.isNumber(quantity)) return;
            if (!Utils.isNumber(offer)) return;

            $(this.summary).text((this.buying?'Buying':'Selling')+' ' + Utils.formatNumber(quantity) + ' ' + item+' @ $'+Utils.formatNumber(offer)+' each for a total of $'+Utils.formatNumber(offer*quantity)+'.');
        }

        submit() {
            var item = $(this.item).val();
            var quantity = $(this.quantity).val();
            var offer = $(this.offer).val();

            if (!Inventory.isItem(item)) return; // if there is no items by that name, return.
            if (!Utils.isNumber(quantity)) return;
            if (!Utils.isNumber(offer)) return;
            
            Connection.submitOrder(Inventory.getId(item), Long.fromNumber(quantity), Long.fromNumber(offer), !this.buying);
            console.log((this.buying ? 'Buying' : 'Selling') + ' ' + quantity + ' ' + Inventory.getId(item) + ' @ $' + offer);
        }
    }

    export function init() {
        if (marketPane) return;

        marketPane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(marketPane);
        Tabs.registerGameTab(marketPane, Connection.Tabs.Market, 'Market');

        for (var i = 0; i < 6; i++)
            drawTradePane(new Order());
    }

    function drawTradePane(order: Order) {
        var panel = document.createElement('div');
        panel.classList.add('market-order');
        panel.classList.add('closed');
        order.panel = panel;

        var header = document.createElement('div');
        header.textContent = 'Empty';
        header.classList.add('market-order-header');
        order.header = header;
        panel.appendChild(header);

        var controls = document.createElement('div');
        controls.classList.add('market-order-controls');
        var buy = document.createElement('div');
        buy.classList.add('market-control');
        buy.classList.add('buy');
        buy.textContent = 'Buy';
        buy.addEventListener('click', function() {
            order.buying = true;
            orderModal(order);
        });
        controls.appendChild(buy);

        var sell = document.createElement('div');
        sell.classList.add('market-control');
        sell.classList.add('sell');
        sell.textContent = 'Sell';
        sell.addEventListener('click', function() {
            order.buying = false;
            orderModal(order);
        });
        controls.appendChild(sell);

        panel.appendChild(controls);

        var display = document.createElement('div');
        display.classList.add('market-order-display');
        panel.appendChild(display);

        orders.push(order);
        marketPane.appendChild(panel);
    }

    $(window).resize(function () {
        $(".ui-autocomplete").css('display', 'none');
    });

    function orderModal(order: Order) {
        var window = new modal.Window();
        window.title = order.buying ? "Buy":"Sell";

        var container = document.createElement('div');
        var itemContainer = document.createElement('div');
        var itemLabel = document.createElement('div');
        itemLabel.textContent = 'Item: ';
        itemLabel.style.width = '70px';
        itemLabel.style.display = 'inline-block';
        var itemName = document.createElement('input');
        order.item = itemName;
        itemName.type = 'TEXT';
        itemName.addEventListener('keyup', function() {
            order.summarize();
        });
        $(itemName).autocomplete({
            source: Inventory.names,
            delay: 0,
            select: function() {
                order.summarize();
            }
        }).data("uiAutocomplete")._renderItem = function(ul, item) {
            return $("<li>").data("item.autocomplete", item)
                .append("<span>").text(item.label).append('<div style="display:inline-block; float:right;" class=Third-'+Utils.cssifyName(item.label)+'></div>')
                .appendTo(ul);
        };
        itemContainer.appendChild(itemLabel);
        itemContainer.appendChild(itemName);
        container.appendChild(itemContainer);

        var quantityContainer = document.createElement('div');
        var quantityLabel = document.createElement('div');
        quantityLabel.style.width = '70px';
        quantityLabel.style.display = 'inline-block';
        var quantityInput = document.createElement('input');
        order.quantity = quantityInput;
        quantityInput.type = 'TEXT';
        quantityInput.addEventListener('keyup', function() {
            order.summarize();
        });
        quantityLabel.textContent = 'Quantity: ';
        quantityContainer.appendChild(quantityLabel);
        quantityContainer.appendChild(quantityInput);
        container.appendChild(quantityContainer);

        var priceContainer = document.createElement('div');
        var priceLabel = document.createElement('div');
        priceLabel.textContent = 'Coins: ';
        priceLabel.style.width = '70px';
        priceLabel.style.display = 'inline-block';
        var priceInput = document.createElement('input');
        priceInput.addEventListener('keyup', function() {
            order.summarize();
        });
        order.offer = priceInput;
        priceContainer.appendChild(priceLabel);
        priceContainer.appendChild(priceInput);
        container.appendChild(priceContainer);

        var summary = document.createElement('div');
        order.summary = summary;
        container.appendChild(summary);

        window.addElement(container);

        var cancel = window.addNegativeOption('Cancel');
        cancel.addEventListener('click', function() {
            modal.close();
        });
        var submit = window.addAffirmativeOption('Submit');
        submit.addEventListener('click', function() {
            order.submit();
        });

        window.show();
    }
} 