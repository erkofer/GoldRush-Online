///<reference path="chat.ts"/>
///<reference path="inventory.ts"/>

module Connection {
    declare var Komodo: { connection: any; ClientActions: any; decode: any; send: any; restart: any };
    var conInterval;
    Komodo.connection.received(function (msg) {
        Chat.log("Recieved " + roughSizeOfObject(msg) + " bytes from komodo.");
        Chat.log("Encoded: ");
        Chat.log(msg);
        Chat.log("Decoded: ");
        Chat.log(JSON.stringify(Komodo.decode(msg)));
        Chat.log(roughSizeOfObject(JSON.stringify(Komodo.decode(msg))) - roughSizeOfObject(msg) + " bytes saved.");
        msg = Komodo.decode(msg);
        // CHAT MESSAGES
        if (msg.Message != null) {
            Chat.receiveGlobalMessage(msg.Message.Sender, msg.Message.Text, msg.Message.Time, msg.Message.Permissions);
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
        // Crafting
        //temp
        Crafting.addCraftingItem();
    });

    export function restart() {
        Komodo.restart();
    }


    var actions = new Komodo.ClientActions();

    Komodo.connection.stateChanged(function (change) {
        if (change.newState === (<any>$).signalR.connectionState.connected) {
            connected();
        }
        if (change.newState === (<any>$).signalR.connectionState.disconnected) {
            clearInterval(conInterval);
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

    

    function loadSchema(schema: any) {
        for (var i = 0; i < schema.Items.length; i++) {
            Inventory.addItem(schema.Items[i].Id, schema.Items[i].Name, schema.Items[i].Worth, schema.Items[i].Category);
            Statistics.addItem(schema.Items[i].Id, schema.Items[i].Name);
        }

        for (var i = 0; i < schema.StoreItems.length; i++) {
            var item = schema.StoreItems[i];
            Store.addItem(item.Id, item.Category, item.Price, item.Factor, item.Name, item.MaxQuantity);
        }
    }

    function updateStats(items: any) {
        for (var i = 0; i < items.length; i++)
            Statistics.changeStats(items[i].Id, items[i].PrestigeQuantity, items[i].LifeTimeQuantity);
    }

    function updateInventory(items: any) {
        for (var i = 0; i < items.length; i++)
            Inventory.changeQuantity(items[i].Id, items[i].Quantity);
    }

    function updateInventoryConfigurations(items: any) {
        for (var i = 0; i < items.length; i++)
            Inventory.modifyConfig(items[i].Id, items[i].Enabled);
    }

    function updateStore(items: any) {
        for (var i = 0; i < items.length; i++)
            Store.changeQuantity(items[i].Id, items[i].Quantity);
    }

    export function sellItem(id: number, quantity: number) {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        var sellAction = new Komodo.ClientActions.InventoryAction.SellAction();
        sellAction.Id = id;
        sellAction.Quantity = quantity;
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