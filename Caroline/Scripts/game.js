var Utils;
(function (Utils) {
    function cssSwap(element, initialVal, finalVal) {
        if (element.classList.contains(initialVal))
            element.classList.remove(initialVal);

        element.classList.add(finalVal);
    }
    Utils.cssSwap = cssSwap;

    function cssifyName(name) {
        return name.split(' ').join('_');
    }
    Utils.cssifyName = cssifyName;

    function isNumber(obj) {
        return !isNaN(parseFloat(obj));
    }
    Utils.isNumber = isNumber;

    function formatNumber(n) {
        if (!n)
            return '0';

        if (n > 999999999999999) {
            return (n / 1000000000000000).toFixed(3) + "Qa";
        } else if (n > 999999999999) {
            return (n / 1000000000000).toFixed(3) + "T";
        } else if (n > 999999999) {
            return (n / 1000000000).toFixed(3) + "B";
        } else if (n > 999999) {
            return (n / 1000000).toFixed(3) + "M";
        } else {
            return n.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    }
    Utils.formatNumber = formatNumber;

    function convertServerTimeToLocal(time) {
        var hours = +time.split(':')[0];
        var minutes = +(time.split(':')[1]).split(' ')[0];
        var amOrPm = (time.split(':')[1]).split(' ')[1];

        if (amOrPm == 'PM')
            hours += 12;

        var offset = new Date().getTimezoneOffset();
        hours -= ((offset / 60) - offset % 60);
        minutes -= offset % 60;

        if (hours < 0)
            hours = 24 - hours;

        if (minutes < 0) {
            minutes = 60 - minutes;
            hours--;
        }

        if (hours > 23) {
            hours = hours - 24;
        }

        if (minutes > 59) {
            minutes = minutes - 60;
            hours++;
        }

        return (hours < 10 ? '0' : '') + hours + ':' + (minutes < 10 ? '0' : '') + minutes;
    }
    Utils.convertServerTimeToLocal = convertServerTimeToLocal;
})(Utils || (Utils = {}));
var Tabs;
(function (Tabs) {
    var lowestTabContainerId = 0;
    var tabContainers = new Array();
    var TabContainer = (function () {
        function TabContainer(container) {
            this.container = container;
            this.tabs = new Array();
            this.lowestId = 0;
            this.id = lowestTabContainerId++;
            tabContainers.push(this);
        }
        TabContainer.prototype.newTab = function (pane) {
            var tab = new Tab();
            tab.pane = pane;
            var button = document.createElement('DIV');
            button.classList.add('tab-button');
            this.container.appendChild(button);
            tab.button = button;

            if (this.lowestId == 0) {
                tab.button.classList.add('active');
            } else {
                tab.button.classList.add('inactive');
                tab.pane.style.display = 'none';
            }

            // IDs are incremented here. to get their initial value we must subtract.
            var id = this.lowestId++;
            var contId = this.id;

            button.addEventListener('click', function () {
                Tabs.activateTab(contId, id);
            });

            this.tabs.push(tab);
            this.container.appendChild(button);

            return this.lowestId - 1;
        };

        TabContainer.prototype.activate = function (id) {
            for (var i = 0; i < this.tabs.length; i++) {
                this.tabs[i].deactivate();
            }
            this.tabs[id].activate();
        };

        TabContainer.prototype.css = function (id, className) {
            this.tabs[id].button.classList.add(className);
        };
        return TabContainer;
    })();

    var gameTabs = new TabContainer(document.getElementById('tabContainer'));

    function registerGameTab(pane, css) {
        var id = gameTabs.newTab(pane);
        if (css)
            gameTabs.css(id, css);
    }
    Tabs.registerGameTab = registerGameTab;

    function activateTab(containerId, tabId) {
        tabContainers[containerId].activate(tabId);
    }
    Tabs.activateTab = activateTab;

    var Tab = (function () {
        function Tab() {
        }
        Tab.prototype.deactivate = function () {
            Utils.cssSwap(this.button, 'active', 'inactive');
            this.pane.style.display = 'none';
        };

        Tab.prototype.activate = function () {
            Utils.cssSwap(this.button, 'inactive', 'active');
            this.pane.style.display = 'block';
        };
        return Tab;
    })();
})(Tabs || (Tabs = {}));
//<reference path="connection.ts"/>
var Chat;
(function (Chat) {
    var chatWindow;
    var chatLogContainer;
    var debugLogContainer;
    function initialize() {
        if (!chatWindow) {
            chatWindow = document.createElement('DIV');
            chatWindow.style.position = 'fixed';
            chatWindow.style.bottom = '0';
            chatWindow.style.left = '0';
            chatWindow.style.minWidth = '400px';
            chatWindow.style.width = '40%';
            chatWindow.style.backgroundColor = '#ebebeb';
            chatWindow.style.border = '1px solid #adadad';
            chatWindow.style.height = '280px';
            chatWindow.style.boxShadow = '1px -1px 2px rgb(200,200,200)';

            var chatHeader = document.createElement('DIV');
            chatHeader.style.height = '30px';
            chatHeader.style.backgroundColor = 'rgb(160, 160, 160)';
            chatWindow.appendChild(chatHeader);

            var chatRoomTab = document.createElement('DIV');
            chatRoomTab.style.color = 'white';
            chatRoomTab.style.fontSize = '18px';
            chatRoomTab.style.height = '30px';
            chatRoomTab.style.display = 'inline-block';
            chatRoomTab.textContent = 'General';
            chatRoomTab.style.padding = '3px 10px';
            chatRoomTab.addEventListener('click', function () {
                document.getElementById('debugpane').style.display = 'none';
                document.getElementById('chatpane').style.display = 'block';
            });
            chatHeader.appendChild(chatRoomTab);

            var debugTab = document.createElement('DIV');
            debugTab.style.color = 'white';
            debugTab.id = 'debugtab';
            debugTab.style.fontSize = '18px';
            debugTab.style.height = '30px';
            debugTab.style.display = 'inline-block';
            debugTab.textContent = '>Dev';
            debugTab.style.padding = '3px 10px';
            debugTab.addEventListener('click', function () {
                document.getElementById('debugpane').style.display = 'block';
                document.getElementById('chatpane').style.display = 'none';
            });
            debugTab.style.display = 'none';
            chatHeader.appendChild(debugTab);

            debugLogContainer = document.createElement('DIV');
            debugLogContainer.id = 'debugpane';
            debugLogContainer.style.width = '100%';
            debugLogContainer.style.height = '230px';
            debugLogContainer.style.display = 'none';
            debugLogContainer.style.position = 'relative';
            chatWindow.appendChild(debugLogContainer);

            chatLogContainer = document.createElement('DIV');
            chatLogContainer.id = 'chatpane';
            chatLogContainer.style.width = '100%';
            chatLogContainer.style.height = '230px';
            chatLogContainer.style.position = 'relative';
            chatWindow.appendChild(chatLogContainer);

            var debugLog = document.createElement('DIV');
            debugLog.id = 'debuglog';
            debugLog.style.position = 'absolute';
            debugLog.style.bottom = '0px';
            debugLog.style.maxHeight = '230px';
            debugLog.style.width = '100%';
            debugLog.style.overflow = 'auto';
            debugLogContainer.appendChild(debugLog);

            var chatLog = document.createElement('DIV');
            chatLog.id = 'chatlog';
            chatLog.style.position = 'absolute';
            chatLog.style.bottom = '0px';
            chatLog.style.maxHeight = '230px';
            chatLog.style.width = '100%';
            chatLog.style.overflow = 'auto';
            chatLogContainer.appendChild(chatLog);

            var chatInput = document.createElement('INPUT');
            chatInput.setAttribute('TYPE', 'TEXT');
            chatInput.style.width = '100%';
            chatInput.style.maxWidth = 'none';
            chatInput.style.height = '20px';
            chatInput.style.border = 'none';
            chatInput.style.borderTop = '1px solid #adadad';
            chatInput.style.width = '85%';
            chatInput.setAttribute('maxlength', '220');
            chatInput.addEventListener('keydown', function (e) {
                if (e.keyCode == 13)
                    sendGlobalMessagePress();

                if (e.keyCode == 68 && e.altKey)
                    document.getElementById('debugtab').style.display = 'inline-block';
            });
            chatInput.id = 'chattext';
            chatWindow.appendChild(chatInput);

            var chatSend = document.createElement('INPUT');
            chatSend.setAttribute('TYPE', 'BUTTON');
            chatSend.setAttribute('VALUE', 'SEND');
            chatSend.style.height = '20px';
            chatSend.style.width = '15%';
            chatSend.style.fontSize = '13px';
            chatSend.style.border = 'none';
            chatSend.style.borderTop = '1px solid #adadad';
            chatSend.style.borderLeft = '1px solid #adadad';
            chatSend.addEventListener('click', function () {
                sendGlobalMessagePress();
            });
            chatWindow.appendChild(chatSend);

            document.body.appendChild(chatWindow);
        }
    }
    Chat.initialize = initialize;
    initialize();

    function sendGlobalMessagePress() {
        Connection.sendGlobalMessage(document.getElementById('chattext').value);
        document.getElementById('chattext').value = '';
    }

    function log(message) {
        var debugLog = document.getElementById('debuglog');
        var debugItem = document.createElement('DIV');
        debugItem.textContent = message;
        debugLog.appendChild(debugItem);
    }
    Chat.log = log;

    function receiveGlobalMessage(sender, message, time, perms) {
        var chatLog = document.getElementById('chatlog');
        var chatItem = document.createElement('DIV');
        chatItem.classList.add('chat-msg');
        if (perms && perms != '')
            chatItem.classList.add('chat-' + perms);
        var timeSpan = document.createElement('SPAN');
        timeSpan.textContent = '[' + Utils.convertServerTimeToLocal(time) + '] ';
        var nameSpan = document.createElement('SPAN');
        nameSpan.textContent = sender;
        nameSpan.classList.add('chat-sender');
        var messageSpan = document.createElement('SPAN');
        messageSpan.textContent = message;
        messageSpan.classList.add('chat-text');
        var dividerSpan = document.createElement('SPAN');
        dividerSpan.textContent = ': ';
        chatItem.appendChild(timeSpan);
        chatItem.appendChild(nameSpan);
        chatItem.appendChild(dividerSpan);
        chatItem.appendChild(messageSpan);

        chatLog.appendChild(chatItem);
    }
    Chat.receiveGlobalMessage = receiveGlobalMessage;
})(Chat || (Chat = {}));
var tooltip;
(function (_tooltip) {
    var registeredTooltips = 0;
    var tooltips = new Array();
    var activeTooltipId;
    var activeTooltip;

    var intervalId;
    var appearDelay = 0.25;
    var currentDelay = 0;
    var x;
    var y;

    var focusedElement;

    var Tooltip = (function () {
        function Tooltip() {
            this.html = document.createElement("div");
            this.html.classList.add('tooltip-wrapper');
        }
        Object.defineProperty(Tooltip.prototype, "header", {
            get: function () {
                return this.html.getElementsByClassName('tooltip-header')[0];
            },
            set: function (html) {
                html.classList.add('tooltip-header');
                this.html.appendChild(html);
            },
            enumerable: true,
            configurable: true
        });


        Object.defineProperty(Tooltip.prototype, "content", {
            get: function () {
                return this.html.getElementsByClassName('tooltip-content')[0];
            },
            set: function (html) {
                html.classList.add('tooltip-content');
                this.html.appendChild(html);
            },
            enumerable: true,
            configurable: true
        });

        return Tooltip;
    })();

    function show(id, x, y) {
        var tooltip = tooltips[id];
        if (activeTooltipId !== id) {
            hide();
        }
        activeTooltipId = id;
        activeTooltip = tooltip.html;
        document.body.appendChild(activeTooltip);
        move(x, y);
    }

    function move(x, y) {
        var rect = activeTooltip.getBoundingClientRect();
        var length = rect.right - rect.left;
        var height = rect.bottom - rect.top;
        if (length + (x + 15) > window.innerWidth)
            activeTooltip.style.left = ((x - length) - 15) + "px";
        else
            activeTooltip.style.left = (x + 15) + "px";

        if ((y - height) > 0)
            activeTooltip.style.top = (y - height) + "px";
        else {
            activeTooltip.style.top = (y + 5) + "px";
        }
    }

    function hide() {
        if (activeTooltip) {
            activeTooltip.parentElement.removeChild(activeTooltip);
        }
        activeTooltip = null;
        activeTooltipId = null;
    }

    function complexModify(id, content, title) {
        var tt = tooltips[id];
        if (title) {
            tt.header.parentElement.removeChild(tt.header);
            tt.header = title;
        }
        tt.content.parentElement.removeChild(tt.content);
        tt.content = content;
    }
    _tooltip.complexModify = complexModify;

    function modify(id, content, title) {
        var tt = tooltips[id];
        if (title)
            tt.header.textContent = title;

        tt.content.textContent = content;
    }
    _tooltip.modify = modify;

    function retrieveContent(id) {
        return tooltips[id].content;
    }
    _tooltip.retrieveContent = retrieveContent;

    function complexCreate(element, content, title) {
        var tt = new Tooltip();
        addListeners(element);

        if (title)
            tt.header = title;

        tt.content = content;

        tooltips.push(tt);
        if (element.dataset) {
            element.dataset['tooltip'] = registeredTooltips;
        } else {
            element.setAttribute('data-tooltip', registeredTooltips.toString());
        }
        registeredTooltips++;
    }
    _tooltip.complexCreate = complexCreate;

    function create(element, content, title) {
        var text = document.createElement('div');
        text.textContent = content;

        if (title) {
            var header = document.createElement('div');
            header.textContent = title;
            complexCreate(element, text, header);
        } else {
            complexCreate(element, text);
        }
    }
    _tooltip.create = create;

    function addListeners(element) {
        element.onmouseenter = function (e) {
            var id = +e.target.getAttribute('data-tooltip');
            if (focusedElement !== e.target) {
                intervalId = setInterval(function () {
                    currentDelay += 0.01;
                    if (currentDelay >= appearDelay) {
                        show(id, x, y);
                        currentDelay = 0;
                        clearInterval(intervalId);
                        intervalId = null;
                    }
                }, 10);
            }
            focusedElement = e.target;
        };

        element.onmousemove = function (e) {
            var pos = mousePosition(e);
            if (!intervalId) {
                move(pos.x, pos.y);
            }
            x = pos.x;
            y = pos.y;
        };

        element.onmouseleave = function (e) {
            focusedElement = null;
            clearInterval(intervalId);
            currentDelay = 0;
            hide();
        };
    }

    function mousePosition(e) {
        if (e.pageX || e.pageY) {
            return { x: e.pageX, y: e.pageY };
        } else if (e.clientX || e.clientY) {
            return {
                x: e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft,
                y: e.clientY + document.body.scrollTop + document.documentElement.scrollTop
            };
        }
    }
})(tooltip || (tooltip = {}));
///<reference path="utils.ts"/>
///<reference path="tooltip.ts"/>
///<reference path="tabs.ts"/>
var Inventory;
(function (Inventory) {
    var items = new Array();
    var inventoryPane;
    var selectedItemPane;
    var selectedItemImage;
    var selectedItem;
    var Item = (function () {
        function Item(id, name, worth) {
            this.id = id;
            this.name = name;
            this.worth = worth;
        }
        return Item;
    })();

    function getSelectedItemQuantity() {
        return selectedItem ? selectedItem.quantity : 0;
    }
    Inventory.getSelectedItemQuantity = getSelectedItemQuantity;

    function selectItem(id) {
        if (id) {
            if (selectedItem != null) {
                selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            }
            selectedItem = items[id];
            selectedItemImage.classList.add(Utils.cssifyName(selectedItem.name));
            selectedItemPane.style.display = 'block';
            limitTextQuantity();
        } else {
            selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            selectedItem = null;
            selectedItemPane.style.display = 'none';
        }
    }
    Inventory.selectItem = selectItem;

    function sellSelectedItem(quantity) {
        Connection.sellItem(selectedItem.id, quantity ? quantity : 1);
    }
    Inventory.sellSelectedItem = sellSelectedItem;

    function sellAllSelectedItem() {
        sellSelectedItem(selectedItem.quantity);
    }
    Inventory.sellAllSelectedItem = sellAllSelectedItem;

    function limitTextQuantity() {
        var textbox = document.getElementById('selecteditemquantity');
        var quantity = +textbox.value;
        if (Utils.isNumber(quantity)) {
            if (quantity > Inventory.getSelectedItemQuantity()) {
                textbox.value = Inventory.getSelectedItemQuantity().toString();
            }
        }
    }

    function add(item) {
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
        var selectedItemQuantity = document.createElement('INPUT');
        selectedItemQuantity.id = 'selecteditemquantity';
        selectedItemQuantity.type = 'text';
        selectedItemQuantity.classList.add('selected-item-quantity');
        selectedItemQuantity.addEventListener('input', function () {
            limitTextQuantity();
        });

        var sellItems = document.createElement('INPUT');
        sellItems.type = 'button';
        sellItems.classList.add('selected-item-quantity');
        sellItems.value = 'Sell';
        sellItems.addEventListener('click', function () {
            var textbox = document.getElementById('selecteditemquantity');
            var quantity = +textbox.value;
            if (Utils.isNumber(quantity)) {
                Inventory.sellSelectedItem(quantity);
            }
            limitTextQuantity();
        });

        var sellAllItems = document.createElement('INPUT');
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

        Tabs.registerGameTab(inventoryPane, 'Inventory');
    }

    function drawItem(item) {
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

    function addItem(id, name, worth) {
        if (!items[id])
            add(new Item(id, name, worth));
    }
    Inventory.addItem = addItem;

    function changeQuantity(id, quantity) {
        items[id].quantityElm.textContent = Utils.formatNumber(quantity);
        items[id].quantity = quantity;
        items[id].container.style.display = quantity == 0 ? 'none' : 'inline-block';
        limitTextQuantity();
    }
    Inventory.changeQuantity = changeQuantity;
})(Inventory || (Inventory = {}));
///<reference path="chat.ts"/>
///<reference path="inventory.ts"/>
var Connection;
(function (Connection) {
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
    });
    var actions = new Komodo.ClientActions();
    setInterval(function () {
        var encoded = actions.encode64();
        if (encoded != '') {
            send(encoded);
        }

        actions = new Komodo.ClientActions();
    }, 1000);

    function loadSchema(schema) {
        for (var i = 0; i < schema.Items.length; i++)
            Inventory.addItem(schema.Items[i].Id, schema.Items[i].Name, schema.Items[i].Worth);

        for (var i = 0; i < schema.StoreItems.length; i++) {
            var item = schema.StoreItems[i];
            console.log(item);
            Store.addItem(item.Id, item.Category, item.BasePrice, item.Factor, item.Name);
        }
    }

    function updateInventory(items) {
        for (var i = 0; i < items.length; i++)
            Inventory.changeQuantity(items[i].Id, items[i].Quantity);
    }

    function sellItem(id, quantity) {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        var sellAction = new Komodo.ClientActions.InventoryAction.SellAction();
        sellAction.Id = id;
        sellAction.Quantity = quantity;
        inventoryAction.Sell = sellAction;
        actions.InventoryActions.push(inventoryAction);
    }
    Connection.sellItem = sellItem;

    function sellAllItems() {
    }
    Connection.sellAllItems = sellAllItems;

    function sendGlobalMessage(message) {
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
    Connection.sendGlobalMessage = sendGlobalMessage;

    function send(message) {
        if (message.encode64) {
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
    Connection.send = send;

    function roughSizeOfObject(object) {
        var objectList = [];
        var stack = [object];
        var bytes = 0;

        while (stack.length) {
            var value = stack.pop();

            if (typeof value === 'boolean') {
                bytes += 4;
            } else if (typeof value === 'string') {
                bytes += value.length * 2;
            } else if (typeof value === 'number') {
                bytes += 8;
            } else if (typeof value === 'object' && objectList.indexOf(value) === -1) {
                objectList.push(value);

                for (var i in value) {
                    stack.push(value[i]);
                }
            }
        }
        return bytes;
    }
})(Connection || (Connection = {}));
var Store;
(function (Store) {
    var storePane;
    var items = new Array();
    Store.categories = new Array();
    (function (Category) {
        Category[Category["MINING"] = 1] = "MINING";
        Category[Category["MACHINES"] = 2] = "MACHINES";
        Category[Category["GATHERING"] = 3] = "GATHERING";
        Category[Category["PROCESSING"] = 4] = "PROCESSING";
        Category[Category["ITEMS"] = 5] = "ITEMS";
        Category[Category["CRAFTING"] = 6] = "CRAFTING";
    })(Store.Category || (Store.Category = {}));
    var Category = Store.Category;
    ;

    var StoreItem = (function () {
        function StoreItem() {
        }
        return StoreItem;
    })();

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

    function drawCategory(name) {
        var categoryContainer = document.createElement('div');
        categoryContainer.classList.add('store-category');
        Store.categories[name] = categoryContainer;

        var categoryHeader = document.createElement('div');
        categoryHeader.textContent = name;
        categoryHeader.classList.add('store-category-header');
        categoryContainer.appendChild(categoryHeader);
        storePane.appendChild(categoryContainer);
    }

    function tempFix() {
        draw();
    }
    Store.tempFix = tempFix;

    function addItem(id, category, price, factor, name) {
        if (!storePane)
            draw();

        var item = new StoreItem();
        item.id = id;
        item.category = category;
        item.price = price;
        item.factor = factor;
        item.name = name;

        var categoryContainer = Store.categories[Category[category]];
        categoryContainer.appendChild(drawItem(item));
    }
    Store.addItem = addItem;

    function add() {
    }

    function drawItem(item) {
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
})(Store || (Store = {}));
