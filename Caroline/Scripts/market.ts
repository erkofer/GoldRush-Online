module Market {
    var marketPane;
    var orders = new Array<Order>();
    import Long = dcodeIO.Long;

    export class Order {
        constructor() {

        }

        slot: number;
        buying: boolean;
        closed: boolean = true;

        panel: HTMLElement;
        header: HTMLElement;
        item: HTMLElement;
        quantityElm: HTMLElement;
        offer: HTMLElement;
        summary: HTMLElement;
        nameElm: HTMLElement;
        unitValueElm: HTMLElement;

        progress: HTMLElement;
        image: HTMLElement;
        imageQuantityElm: HTMLElement;
        quantity: number;
        unfulfilledQuantity: number;
        lastUpdate: IOrderUpdate;


        update(update: IOrderUpdate) {
            if (!update.Quantity.subtract) {
                update.Quantity = Long.fromNumber(<any>update.Quantity);
                update.UnfulfilledQuantity = Long.fromNumber(<any>update.UnfulfilledQuantity);
                update.ItemId = Long.fromNumber(<any>update.ItemId);
                update.UnitValue = Long.fromNumber(<any>update.UnitValue);
                update.UnclaimedItems = Long.fromNumber(<any>update.UnclaimedItems);
                update.UnclaimedCoins = Long.fromNumber(<any>update.UnclaimedCoins);
            }
            this.lastUpdate = update;
            Utils.cssSwap(this.panel, 'closed', 'open');
            var quantityLeft = update.Quantity.subtract(update.UnfulfilledQuantity);
            this.progress.style.width = (quantityLeft.multiply(Long.fromNumber(100)).div(update.Quantity)).toNumber() + '%';
            this.progress.style.background = update.UnfulfilledQuantity.equals(Long.fromNumber(0)) ? (update.IsCanceled?'red':'green') : 'yellow';
            this.header.textContent = update.IsSelling ? 'Sell' : 'Buy';
            this.imageQuantityElm.textContent = update.Quantity.toString();
            var name = Inventory.getName(update.ItemId.toNumber());
            this.image.className = Utils.cssifyName(name);
            this.nameElm.textContent = name;
            this.unitValueElm.textContent = '$' + this.commaSeparateNumber(update.UnitValue.toString());
        }

        details() {
            var window = new modal.Window();
            window.title = 'Details';
            var slot = this.slot;
            var container = document.createElement('div');
            var imageSection = document.createElement('div');
            imageSection.style.height = '74px';
            var imageContainer = document.createElement('div');
            imageContainer.classList.add('market-image-container');
            var imageQuantity = document.createElement('div');
            imageQuantity.classList.add('market-image-quantity');
            imageQuantity.textContent = this.lastUpdate.Quantity.toString();
            imageContainer.appendChild(imageQuantity);
            var image = document.createElement('div');
            var name = Inventory.getName(this.lastUpdate.ItemId.toNumber());
            image.classList.add(Utils.cssifyName(name));
            imageContainer.appendChild(image);
            imageSection.appendChild(imageContainer);
            container.appendChild(imageSection);

            var bottomContainer = document.createElement('div');
            bottomContainer.classList.add('market-info-bottom');

            var progressSection = document.createElement('div');
            var progressText = document.createElement('div');
            progressText.textContent = 'You have ' + (this.lastUpdate.IsSelling ? 'sold' : 'bought') + ' ' + this.lastUpdate.Quantity.subtract(this.lastUpdate.UnfulfilledQuantity).toString() + ' out of ' + this.lastUpdate.Quantity;
            progressText.style.fontSize = '11px';
            progressText.style.marginRight = '80px';
            progressSection.appendChild(progressText);
            var progressContainer = document.createElement('div');
            progressContainer.classList.add('market-progress-container');
            progressContainer.style.marginRight = '80px';
            progressSection.appendChild(progressContainer);

            var progress = document.createElement('div');
            progress.classList.add('market-progress');
            var quantityLeft = this.lastUpdate.Quantity.subtract(this.lastUpdate.UnfulfilledQuantity);
            progress.style.background = this.lastUpdate.UnfulfilledQuantity.equals(Long.fromNumber(0)) ? (this.lastUpdate.IsCanceled?'red':'green') : 'yellow';
            progress.style.width = (quantityLeft.multiply(Long.fromNumber(100)).div(this.lastUpdate.Quantity)).toNumber() + '%';
            progressContainer.appendChild(progress);
            bottomContainer.appendChild(progressSection);

            var claimsContainer = document.createElement('div');
            claimsContainer.classList.add('market-claims-container');

            var itemsClaim = document.createElement('div');
            itemsClaim.classList.add('market-claims');
            var itemImage = document.createElement('div');
            itemImage.classList.add("Half-"+Utils.cssifyName(name));
            itemsClaim.appendChild(itemImage);
            
            var itemQuantity = document.createElement('div');
            itemQuantity.classList.add('market-claims-text');
            itemQuantity.textContent = this.commaSeparateNumber(this.lastUpdate.UnclaimedItems);
            itemQuantity.style.display = itemQuantity.textContent == '0' ? 'none' : 'block';
            itemImage.style.display = itemQuantity.style.display;
            itemsClaim.appendChild(itemQuantity);
            tooltip.create(itemsClaim, "Claim items.");
            itemsClaim.addEventListener('click', function () {
                Connection.claim(slot, false);
                itemImage.style.display = 'none';
                itemQuantity.style.display = 'none';
            });

            var coinsClaim = document.createElement('div');
            coinsClaim.classList.add('market-claims');
            var coinImage = document.createElement('div');
            coinImage.classList.add('Half-Coins');
            coinsClaim.appendChild(coinImage);
            var coinQuantity = document.createElement('div');
            coinQuantity.classList.add('market-claims-text');
            coinQuantity.textContent = this.commaSeparateNumber(this.lastUpdate.UnclaimedCoins);
            coinQuantity.style.display = coinQuantity.textContent == '0' ? 'none' : 'block';
            coinImage.style.display = coinQuantity.style.display;
            coinsClaim.appendChild(coinQuantity);
            tooltip.create(coinsClaim, "Claim coins.");
            coinsClaim.addEventListener('click', function() {
                Connection.claim(slot, true);
                coinImage.style.display = 'none';
                coinQuantity.style.display = 'none';
            });

            claimsContainer.appendChild(itemsClaim);
            claimsContainer.appendChild(coinsClaim);

            bottomContainer.appendChild(claimsContainer);
            container.appendChild(bottomContainer);

            window.addElement(container);
            window.addOption('Close');
            window.addNegativeOption('Cancel Order').addEventListener('click',function() {
                Connection.cancelOrder(slot);
            });
            window.show();
        }

        private validate(): boolean {
            var item = $(this.item).val();
            var quantity = $(this.quantityElm).val();
            var offer = $(this.offer).val();

            if (!Inventory.isItem(item)) return false; // if there is no items by that name, return.
            if (!Utils.isNumber(quantity)) return false;
            if (!Utils.isNumber(offer)) return false;

            return true;
        }

        private commaSeparateNumber(val) {
            while (/(\d+)(\d{3})/.test(val.toString())) {
                val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
            }
            return val;
        }

        summarize() {
            var item = $(this.item).val();
            var quantity = $(this.quantityElm).val();
            var offer = $(this.offer).val();

            if (this.validate())
                $(this.summary).text((this.buying ? 'Buying' : 'Selling') + ' ' + Utils.formatNumber(quantity) + ' ' + item + ' @ $' + Utils.formatNumber(offer) + ' each for a total of $' + Utils.formatNumber(offer * quantity) + '.');
        }

        submit() {
            var item = $(this.item).val();
            var quantity = $(this.quantityElm).val();
            var offer = $(this.offer).val();

            if (this.validate())
                Connection.submitOrder(this.slot, Inventory.getId(item), Long.fromNumber(quantity), Long.fromNumber(offer), !this.buying);
        }
    }

    export interface IOrderUpdate {
        Slot: number;
        Quantity: Long;
        UnfulfilledQuantity: Long;
        IsSelling: boolean;
        ItemId: Long;
        UnitValue: Long;
        UnclaimedItems: Long;
        UnclaimedCoins: Long;
        IsCanceled: boolean;
    }

    export function updateOrders(updates: Array<IOrderUpdate>) {
        for (var i = 0; i < orders.length; i++) {
            Utils.cssSwap(orders[i].panel, 'open', 'closed');
        }

        for (var i = 0; i < updates.length; i++) {
            var update = updates[i];
            var orderToUpdate = orders[update.Slot];

            orderToUpdate.update(update);
        }
    }

    export function init() {
        if (marketPane) return;

        marketPane = document.createElement('DIV');
        var refresh = Utils.createButton('Refresh', '');
        refresh.style.cssFloat = 'right';
        refresh.addEventListener('click', function() {
            Connection.requestOrders();
        });
        marketPane.appendChild(refresh);
        document.getElementById('paneContainer').appendChild(marketPane);
        Tabs.registerGameTab(marketPane, Connection.Tabs.Market, 'Market');

        for (var i = 0; i < 6; i++) {
            var order = new Order();
            order.slot = i;
            drawTradePane(order);
        }
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
        buy.addEventListener('click', function () {
            order.buying = true;
            orderModal(order);
        });
        controls.appendChild(buy);

        var sell = document.createElement('div');
        sell.classList.add('market-control');
        sell.classList.add('sell');
        sell.textContent = 'Sell';
        sell.addEventListener('click', function () {
            order.buying = false;
            orderModal(order);
        });
        controls.appendChild(sell);

        panel.appendChild(controls);

        var display = document.createElement('div');
        display.classList.add('market-order-display');
        var infoContainer = document.createElement('div');
        infoContainer.style.marginTop = "10px";
        var edit = document.createElement('div');
        edit.textContent = '...';
        edit.addEventListener('click',function() {
            order.details();
        })
        edit.classList.add('market-order-edit');
        infoContainer.appendChild(edit);
        var imageContainer = document.createElement('div');
        imageContainer.classList.add('market-image-container');
        var imageQuantity = document.createElement('div');
        imageQuantity.classList.add('market-image-quantity');
        order.imageQuantityElm = imageQuantity;
        imageContainer.appendChild(imageQuantity);
        var image = document.createElement('div');
        imageContainer.appendChild(image);
        infoContainer.appendChild(imageContainer);
        var info = document.createElement('div');
        info.classList.add('market-order-info');
        var name = document.createElement('div');
        order.nameElm = name;
        info.appendChild(name);
        var value = document.createElement('div');
        order.unitValueElm = value;
        info.appendChild(value);
        infoContainer.appendChild(info);
        display.appendChild(infoContainer);
        order.image = image;

        var progressContainer = document.createElement('div');
        progressContainer.classList.add('market-progress-container');
        var progress = document.createElement('div');
        progress.classList.add('market-progress');
        progressContainer.appendChild(progress);
        order.progress = progress;
        display.appendChild(progressContainer);
        panel.appendChild(display);

        orders.push(order);
        marketPane.appendChild(panel);
    }

    $(window).resize(function () {
        $(".ui-autocomplete").css('display', 'none');
    });

    function orderModal(order: Order) {
        var window = new modal.Window();
        window.title = order.buying ? "Buy" : "Sell";

        var container = document.createElement('div');
        var itemContainer = document.createElement('div');
        var itemLabel = document.createElement('div');
        itemLabel.textContent = 'Item: ';
        itemLabel.style.width = '70px';
        itemLabel.style.display = 'inline-block';
        var itemName = document.createElement('input');
        order.item = itemName;
        itemName.type = 'TEXT';
        itemName.addEventListener('keyup', function () {
            order.summarize();
        });
        $(itemName).autocomplete({
            source: Inventory.names,
            delay: 0,
            select: function () {
                order.summarize();
            }
        }).data("uiAutocomplete")._renderItem = function (ul, item) {
            return $("<li>").data("item.autocomplete", item)
                .append("<span>").text(item.label).append('<div style="display:inline-block; float:right;" class=Third-' + Utils.cssifyName(item.label) + '></div>')
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
        order.quantityElm = quantityInput;
        quantityInput.type = 'TEXT';
        quantityInput.addEventListener('keyup', function () {
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
        priceInput.addEventListener('keyup', function () {
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
        cancel.addEventListener('click', function () {
            modal.close();
        });
        var submit = window.addAffirmativeOption('Submit');
        submit.addEventListener('click', function () {
            order.submit();
        });

        window.show();
    }
} 