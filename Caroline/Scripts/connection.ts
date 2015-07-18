///<reference path="pheidippides.ts"/>
///<reference path="chat.ts"/>
///<reference path="inventory.ts"/>
///<reference path="stats.ts"/>
///<reference path="store.ts"/>
///<reference path="rock.ts"/>
///<reference path="equipment.ts"/>
///<reference path="crafting.ts"/>
///<reference path="typings/jquery/jquery.d.ts"/>
///<reference path="typings/jqueryui/jqueryui.d.ts"/>
///<reference path="typings/dcode/long.d.ts"/>
///<reference path="buffs.ts"/>
///<reference path="register.ts"/>
///<reference path="modal.ts"/>
///<reference path="tooltip.ts"/>
///<reference path="ajax.ts"/>
///<reference path="tutorial.ts"/>
///<reference path="achievements.ts"/>
///<reference path="market.ts"/>

module Connection {
    declare var Komodo: { connection: any; ClientActions: any; decode: any; send: any; restart: any };
    var conInterval;
    var disconInterval;
    var notificationElm;
    var networkErrorElm;
    var rateLimitedElm;
    var playerCounter;
    var currentTab: Tabs;
    import Long = dcodeIO.Long;
    var sessionId = null;

    export enum Tabs {
        Inventory= 1,
        Statistics= 2,
        Equipment= 3,
        Store= 4,
        Crafting= 5,
        Achievements= 6,
        Market= 7
    };

    function init() {
        var headerLinks = document.getElementsByClassName('header-links')[0];
        var versionContainer = document.createElement('div');
        versionContainer.classList.add('header-button');
        var versionHistory = document.createElement('div');
        versionHistory.style.display = 'inline-block';
        versionHistory.classList.add('History');
        versionHistory.style.position = 'absolute';
        versionHistory.style.top = '2px';
        versionHistory.style.left = '2px';
        versionContainer.addEventListener('click', function () {
            window.open('/version');
        });
        tooltip.create(versionContainer, "Version history");
        versionHistory.style.cursor = 'pointer';
        versionContainer.appendChild(versionHistory);
        headerLinks.appendChild(versionContainer);

        var playerCounterContainer = document.createElement('div');
        playerCounterContainer.style.position = 'relative';
        playerCounterContainer.style.display = 'inline-block';
        playerCounterContainer.style.width = '36px';
        var playerCounterImage = document.createElement('div');
        playerCounterImage.classList.add('Players');
        playerCounterContainer.appendChild(playerCounterImage);
        tooltip.create(playerCounterContainer, "Active players");

        playerCounter = document.createElement('div');
        playerCounter.style.display = 'inline-block';
        playerCounter.style.textAlign = 'center';
        playerCounter.style.width = '36px';
        playerCounter.textContent = '0';
        playerCounter.style.position = 'absolute';
        playerCounter.style.bottom = '1px';
        playerCounterContainer.appendChild(playerCounter);
        headerLinks.appendChild(playerCounterContainer);

        notificationElm = document.createElement('div');
        notificationElm.classList.add('error-notification-tray');

        networkErrorElm = document.createElement('div');
        networkErrorElm.classList.add('network-error');
        var networkErrorText = document.createElement('div');
        networkErrorText.classList.add('network-error-text');
        networkErrorText.textContent = 'No connection';
        networkErrorElm.appendChild(networkErrorText);

        rateLimitedElm = document.createElement('div');
        rateLimitedElm.classList.add('rate-limited');
        var rateLimitText = document.createElement('div');
        rateLimitText.classList.add('network-error-text');
        rateLimitedElm.style.display = 'none';;
        rateLimitText.textContent = 'You have exceeded your allotted requests';
        rateLimitedElm.appendChild(rateLimitText);

        var game = document.getElementById('game');
        notificationElm.appendChild(networkErrorElm);
        notificationElm.appendChild(rateLimitedElm);
        game.insertBefore(notificationElm, game.childNodes[0]);
    }
    init();

    Komodo.connection.received(function (msg) {
        Chat.log("Recieved " + roughSizeOfObject(msg) + " bytes from komodo.");
        Chat.log("Encoded: ");
        Chat.log(msg);
        Chat.log("Decoded: ");
        Chat.log(JSON.stringify(Komodo.decode(msg)));
        Chat.log(roughSizeOfObject(JSON.stringify(Komodo.decode(msg))) - roughSizeOfObject(msg) + " bytes saved.");
        msg = Komodo.decode(msg);
        // CHAT MESSAGES
        if (msg.Messages != null) {
            receiveGlobalMessages(msg.Messages);

        }
        // GAME SCHEMA
        if (msg.GameSchema != null) {
            loadSchema(msg.GameSchema);
        }
        // INVENTORY UPDATES
        if (msg.Items != null) {
            updateInventory(msg.Items);
        }
        // STORE UPDATES
        if (msg.StoreItemsUpdate != null) {
            updateStore(msg.StoreItemsUpdate);
        }
        if (msg.StatItemsUpdate != null) {
            updateStats(msg.StatItemsUpdate);
        }
        if (msg.ConfigItems != null) {
            updateInventoryConfigurations(msg.ConfigItems);
        }
        // PROCESSOR UPDATES
        if (msg.Processors != null) {
            updateProcessors(msg.Processors);
        }
        // Anti cheat
        if (msg.AntiCheatCoordinates != null) {
            antiCheat(msg.AntiCheatCoordinates);
        }
        // Buffs
        if (msg.Buffs != null) {
            updateBuffs(msg.Buffs);
        }
        if (msg.IsRateLimited != null) {
            rateLimit(msg.IsRateLimited);
        }
        if (msg.Gatherers != null) {
            updateGatherers(msg.Gatherers);
        }
        if (msg.ConnectedUsers) {
            playerCounter.textContent = msg.ConnectedUsers;
        }
        if (msg.Achievements) {
            updateAchievements(msg.Achievements);
        }
        if (msg.Orders) {
            if (msg.OrdersSent == true)
                Market.updateOrders(msg.Orders);
        }
        if (msg.Notifications) {
            for (var i = 0; i < msg.Notifications.length; i++) {
                var notification = msg.Notifications[i];
                Pheidippides.deliver(notification.Tag, notification.Message);
            }
        }
        if (msg.CurrentTutorial) {
            Tutorial.activateStage(msg.CurrentTutorial);
        }
        if (msg.SessionId) {
            sessionId = msg.SessionId;
        }
        if (msg.OfflineRecord) {
            console.log(msg.OfflineRecord);
            Offline.display(msg.OfflineRecord);
        }
        Store.update();
    });

    export function restart() {
        Komodo.restart();
    }

    var actions = new Komodo.ClientActions();

    Komodo.connection.stateChanged(function (change) {
        if (change.newState === (<any>$).signalR.connectionState.connected) {
            connected();
            networkErrorElm.style.display = 'none';
        }
        if (change.newState === (<any>$).signalR.connectionState.disconnected) {
            clearInterval(conInterval);
            networkErrorElm.style.display = 'block';
        }
        if (change.newState === (<any>$).signalR.connectionState.reconnecting) {
            networkErrorElm.style.display = 'block';
        }
    });

    export function changeSelectedTab(selected: Tabs) {
        currentTab = selected;
    }

    function connected() {
        console.log('Connection opened');
        var encoded = actions.encode64();
        send(encoded);
        actions = new Komodo.ClientActions();

        conInterval = setInterval(function () {
            var encoded = actions.encode64();
            // if (encoded!='') {
            send(encoded);
            //}

            actions = new Komodo.ClientActions();
        }, 1000);
    }

    function disconnected() {
        console.log('Connection lost');
        networkErrorElm.style.top = '0px';
        disconInterval = setTimeout(function () {
            Komodo.restart();
        }, 5000);
    }

    function loadSchema(schema: any) {
        if (schema.Items) {
            for (var i = 0; i < schema.Items.length; i++) {
                Inventory.addItem(schema.Items[i].Id, schema.Items[i].Name, schema.Items[i].Worth, schema.Items[i].Category);
                Statistics.addItem(schema.Items[i].Id, schema.Items[i].Name);
            }

        }

        if (schema.StoreItems) {
            for (var i = 0; i < schema.StoreItems.length; i++) {
                var item = schema.StoreItems[i];
                Store.addItem(item.Id, item.Category, item.Price, item.Factor, item.Name, item.MaxQuantity, item.Tooltip);
            }
        }

        if (schema.Processors) {
            for (var i = 0; i < schema.Processors.length; i++) {
                var processor = schema.Processors[i];
                Crafting.addProcessor(processor.Id, processor.Name, processor.RequiredId);
                for (var r = 0; r < processor.Recipes.length; r++) {
                    Crafting.addProcessorRecipe(processor.Id, processor.Recipes[r].Ingredients, processor.Recipes[r].Resultants);
                }
            }
        }
        if (schema.CraftingItems) {
            for (var i = 0; i < schema.CraftingItems.length; i++) {
                var item = schema.CraftingItems[i];
                Crafting.addRecipe(item.Id, item.Ingredients, item.Resultants, item.IsItem);
            }
        }
        if (schema.Buffs) {
            for (var i = 0; i < schema.Buffs.length; i++) {
                var buff = schema.Buffs[i];
                Buffs.register(buff.Id, buff.Name, buff.Description, buff.Duration);
            }
        }
        if (schema.Achievements) {
            for (var i = 0; i < schema.Achievements.length; i++) {
                var achievement = schema.Achievements[i];
                Achievements.register(achievement.Id, achievement.Name, achievement.RequiredId, achievement.Goal, achievement.Category);
            }

            Market.init();
        }
    }

    export function placeOrder(id: number, quantity: number, value: number, isSelling: boolean) {
        var order = new Komodo.ClientActions.Order();
        order.ItemId = id;
        order.ItemQuantity = quantity;
        order.ItemValue = value;
        order.IsSelling = isSelling;
        actions.Orders.push(order);
    }

    function rateLimit(limited: any) {
        rateLimitedElm.style.display = limited ? 'block' : 'none';
    }

    export function cancelOrder(position: number) {
        var cancel = new Komodo.ClientActions.OrderCancel();
        cancel.Slot = position;
        actions.Cancels.push(cancel);
    }

    export function submitOrder(position: number, itemId: number, itemQuantity: Long, itemValue: Long, isSelling: boolean) {
        var order = new Komodo.ClientActions.Order();
        order.ItemId = itemId;
        order.ItemQuantity = itemQuantity;
        order.ItemValue = itemValue;
        order.IsSelling = isSelling;
        order.Position = position;
        console.log(order);
        actions.Orders.push(order);
    }

    export function toggleGatherer(id: number, enabled: boolean) {
        var gathererAction = new Komodo.ClientActions.GathererAction();
        gathererAction.Id = id;
        gathererAction.Enabled = enabled;
        actions.GathererActions.push(gathererAction);
    }

    function updateGatherers(gatherers: any) {
        for (var i = 0; i < gatherers.length; i++) {
            var gatherer = gatherers[i];
            Equipment.toggleGatherer(gatherer.Id, gatherer.Enabled);
            Equipment.changeEfficiency(gatherer.Id, gatherer.Efficiency);
            Equipment.changeFuelConsumption(gatherer.Id, gatherer.FuelConsumed);
            Equipment.changeRarityBonus(gatherer.Id, gatherer.RarityBonus);
        }
    }

    function updateAchievements(achievements: any) {
        for (var i = 0; i < achievements.length; i++) {
            var achievement = achievements[i];
            Achievements.updateAchievement(achievement.Id, achievement.Progress);
        }
    }

    function updateBuffs(buffs: any) {
        for (var i = 0; i < buffs.length; i++) {
            var buff = buffs[i];
            Buffs.update(buff.Id, buff.TimeActive);
        }
    }

    function receiveGlobalMessages(messages: any) {
        for (var i = 0; i < messages.length; i++) {

            var msg = messages[i];
            Chat.receiveGlobalMessage(msg.Sender, msg.Text, msg.Time, msg.Permissions);
        }
    }

    function updateProcessors(processors: any) {
        for (var i = 0; i < processors.length; i++) {
            var processor = processors[i];
            Crafting.updateProcessor(processor.Id, processor.SelectedRecipe, processor.OperationDuration, processor.CompletedOperations, processor.TotalOperations, processor.Capacity);
        }
    }

    function antiCheat(ac: any) {
        Rock.moveRock(ac.X, ac.Y);
    }


    function updateStats(items: any) {
        for (var i = 0; i < items.length; i++)
            Statistics.changeStats(items[i].Id, items[i].PrestigeQuantity, items[i].LifeTimeQuantity);
    }

    function updateInventory(items: any) {
        for (var i = 0; i < items.length; i++) {
            Inventory.changeQuantity(items[i].Id, items[i].Quantity);
            Inventory.changePrice(items[i].Id, items[i].Worth);
        }
    }

    function updateInventoryConfigurations(items: any) {
        for (var i = 0; i < items.length; i++)
            Inventory.modifyConfig(items[i].Id, items[i].Enabled);
    }

    function updateStore(items: any) {
        for (var i = 0; i < items.length; i++)
            Store.changeQuantity(items[i].Id, items[i].Quantity, items[i].MaxQuantity, items[i].Price);
    }

    export function drink(id: number) {
        var potionAction = new Komodo.ClientActions.PotionAction();
        potionAction.Id = id;
        actions.PotionActions.push(potionAction);
    }

    export function claim(slot: number, coins: boolean) {
        var claimAction = new Komodo.ClientActions.OrderClaim();
        claimAction.Slot = slot;
        claimAction.Coins = coins;
        actions.Claims.push(claimAction);
    }

    export function mine(x: number, y: number) {
        var miningAction = new Komodo.ClientActions.MiningAction();
        miningAction.X = x;
        miningAction.Y = y;
        actions.MiningActions.push(miningAction);
    }

    export function sellItem(id: number, quantity: Long) {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        var sellAction = new Komodo.ClientActions.InventoryAction.SellAction();
        sellAction.Id = id;
        sellAction.Quantity = quantity;
        console.log(quantity);
        inventoryAction.Sell = sellAction;
        actions.InventoryActions.push(inventoryAction);
    }

    export function configureItem(id: number, enabled: boolean) {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        var configAction = new Komodo.ClientActions.InventoryAction.ConfigAction();
        configAction.Id = id;
        configAction.Enabled = enabled;
        inventoryAction.Config = configAction;
        actions.InventoryActions.push(inventoryAction);
        //ConfigAction
    }

    export function requestOrders() {
        actions.RequestOrders = true;
    }

    export function purchaseItem(id: number, quantity?: number) {
        var storeAction = new Komodo.ClientActions.StoreAction();
        var purchaseAction = new Komodo.ClientActions.StoreAction.PurchaseAction();
        purchaseAction.Id = id;
        purchaseAction.Quantity = (quantity ? quantity : 1);
        storeAction.Purchase = purchaseAction;
        actions.StoreActions.push(storeAction);
    }

    export function sellAllItems() {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        inventoryAction.SellAll = true;
        actions.InventoryActions.push(inventoryAction);
    }

    export function craftRecipe(id: number, quantity: number) {
        var craftingAction = new Komodo.ClientActions.CraftingAction();
        craftingAction.Id = id;
        craftingAction.Quantity = quantity;
        actions.CraftingActions.push(craftingAction);
    }

    export function processRecipe(id: number, recipeIndex: number, iterations: number) {
        var processingAction = new Komodo.ClientActions.ProcessingAction();
        processingAction.Id = id;
        processingAction.RecipeIndex = recipeIndex;
        processingAction.Iterations = iterations;
        actions.ProcessingActions.push(processingAction);
    }

    export function sendGlobalMessage(message: string) {
        /*var clientActions = new Komodo.ClientActions();
        var socialAction = new Komodo.ClientActions.SocialAction();
        var chatAction = new Komodo.ClientActions.SocialAction.ChatAction();
        chatAction.GlobalMessage = message;
        socialAction.Chat = chatAction;
        clientActions.SocialActions.push(socialAction);

        Connection.send(clientActions);*/

        var socialAction = new Komodo.ClientActions.SocialAction();
        var chatAction = new Komodo.ClientActions.SocialAction.ChatAction();
        chatAction.GlobalMessage = message;
        socialAction.Chat = chatAction;
        actions.SocialActions.push(socialAction);

    }

    export function send(message: any) {
        if (message.encode64) { // if this message is a protobuf object.
            if (message.SelectedTab) { // if this is a clientactions message.
                message.SelectedTab = currentTab;

                if (sessionId != null)
                    message.SessionId = sessionId;
            }
            var encoded = message.encode64();
            Chat.log("Sent " + roughSizeOfObject(encoded) + " bytes to komodo.");
            Chat.log("Decoded: ");
            Chat.log(JSON.stringify(message));
            Chat.log("Encoded: ");
            Chat.log(message.encode64());
            Komodo.send(message.encode64());
        } else {
            Chat.log("Sent " + roughSizeOfObject(message) + " bytes to komodo.");
            Komodo.send(message);
        }
    }

    function roughSizeOfObject(object: any) {

        var objectList = [];
        var stack = [object];
        var bytes = 0;

        while (stack.length) {
            var value = stack.pop();

            if (typeof value === 'boolean') {
                bytes += 4;
            }
            else if (typeof value === 'string') {
                bytes += value.length * 2;
            }
            else if (typeof value === 'number') {
                bytes += 8;
            }
            else if
            (
                typeof value === 'object'
                && objectList.indexOf(value) === -1
                ) {
                objectList.push(value);

                for (var i in value) {
                    stack.push(value[i]);
                }
            }
        }
        return bytes;
    }
}

module Offline {
    import Long = dcodeIO.Long;

    export function display(message: any) {
        var window = new modal.Window();
        window.title = "Offline Progression";
        var content = document.createElement("div");
        var timeGone = document.createElement("div");
        timeGone.textContent = message.SecondsGone;
        content.appendChild(timeGone);
        content.style.maxHeight = "500px";
        content.style.overflowY = "auto";
        var resourcesCollected = document.createElement("div");
        content.appendChild(resourcesCollected);
        var table = document.createElement("table");
        table.style.color = "black";
        table.style.margin = "0 auto";
        table.style.boxShadow = "none";
        var totalResources: Long = Long.fromNumber(0);
        for (var i = 0; i < message.Items.length; i++) {
            var item = message.Items[i];
            if (item.Change == null) continue;

            totalResources = totalResources.add(item.Change);
            var name = Objects.lookupName(item.Id);

            var tableRow = (<HTMLTableElement>table).insertRow(table.rows.length);

            var tableCellImage = (<HTMLTableRowElement>tableRow).insertCell(0);
            var tableCellChange = (<HTMLTableRowElement>tableRow).insertCell(0);

            var imageCell = document.createElement("div");
            imageCell.style.display = "inline-block";
            imageCell.style.verticalAlign = "bottom";
            imageCell.classList.add("Third-" + Utils.cssifyName(name));
            tableCellImage.appendChild(imageCell);

            var nameCell = document.createElement("div");
            nameCell.textContent = name;
            nameCell.style.display = "inline-block";
            nameCell.style.paddingLeft = "5px";
            tableCellImage.appendChild(nameCell);

            tableCellChange.textContent = Utils.formatNumber(item.Change);
        }

        content.appendChild(table);
        resourcesCollected.textContent = Utils.formatNumber((<any>totalResources)) + " resources were collected while you were away!";

        window.addElement(content);
        window.addOption("Awesome!").addEventListener("click", function () {
            modal.close();
        }, false);
        window.show();
    }
}