///<reference path="chat.ts"/>
///<reference path="inventory.ts"/>
///<reference path="stats.ts"/>
///<reference path="store.ts"/>
///<reference path="rock.ts"/>
///<reference path="equipment.ts"/>
///<reference path="crafting.ts"/>
///<reference path="typings/jquery/jquery.d.ts"/>
///<reference path="typings/dcode/long.d.ts"/>
///<reference path="buffs.ts"/>
///<reference path="register.ts"/>
///<reference path="modal.ts"/>
///<reference path="tooltip.ts"/>
///<reference path="ajax.ts"/>
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
    
    
    function init() {
        var headerLinks = document.getElementsByClassName('header-links')[0];
        var versionHistory = document.createElement('div');
        versionHistory.style.display = 'inline-block';
        versionHistory.textContent = 'Version History';
        versionHistory.addEventListener('click', function () {
            window.open('/version');
        });
        versionHistory.style.cursor = 'pointer';
        headerLinks.appendChild(versionHistory);

        playerCounter = document.createElement('div');
        playerCounter.style.display = 'inline-block';
        playerCounter.textContent = 'There are 0 players mining.';
        headerLinks.appendChild(playerCounter);

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
        if(msg.Buffs != null){
            updateBuffs(msg.Buffs);
        }
        if (msg.IsRateLimited != null) {
            rateLimit(msg.IsRateLimited);
        }
        if (msg.Gatherers != null) {
            updateGatherers(msg.Gatherers);
        }
        if (msg.ConnectedUsers) {
            playerCounter.textContent = 'There are ' + msg.ConnectedUsers + ' players mining.';
        }
        if (msg.Achievements) {
            updateAchievements(msg.Achievements);
        }
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
                Store.addItem(item.Id, item.Category, item.Price, item.Factor, item.Name, item.MaxQuantity,item.Tooltip);
            }
        }

        if (schema.Processors) {
            for (var i = 0; i < schema.Processors.length; i++) {
                var processor = schema.Processors[i];
                Crafting.addProcessor(processor.Id, processor.Name,processor.RequiredId);
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
                console.log(achievement);
                Achievements.register(achievement.Id, achievement.Name, achievement.RequiredId, achievement.Goal, achievement.Category);
            }
        }
    }

    function rateLimit(limited: any) {
        rateLimitedElm.style.display = limited ? 'block' : 'none';
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
        for (var i = 0; i < buffs.length; i++){
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

    export function mine(x: number, y: number) {
        var miningAction = new Komodo.ClientActions.MiningAction();
        miningAction.X = x;
        miningAction.Y = y;
        actions.MiningActions.push(miningAction);
    }

    export function sellItem(id: number, quantity: number) {
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
    
    function roughSizeOfObject(object:any) {

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