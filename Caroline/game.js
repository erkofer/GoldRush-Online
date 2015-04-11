var Chat;
(function (Chat) {
    var chatWindow;
    var chatLogContainer;
    var debugLogContainer;
    function initialize() {
        if (!chatWindow) {
            chatWindow = document.createElement('DIV');
            chatWindow.style.transition = 'all 0.2s';
            chatWindow.id = 'chatWindow';
            chatWindow.style.position = 'fixed';
            chatWindow.style.bottom = '0px';
            chatWindow.style.left = '0px';
            chatWindow.style.minWidth = '400px';
            chatWindow.style.width = '40%';
            chatWindow.style.backgroundColor = '#ebebeb';
            chatWindow.style.border = '1px solid #adadad';
            chatWindow.style.height = '280px';
            chatWindow.style.boxShadow = '1px -1px 2px rgb(200,200,200)';

            var chatHeader = document.createElement('DIV');
            chatHeader.style.position = 'relative';
            chatHeader.style.height = '30px';
            chatHeader.style.backgroundColor = 'rgb(160, 160, 160)';
            chatWindow.appendChild(chatHeader);

            var chatCloser = document.createElement('DIV');
            chatCloser.textContent = 'Collapse';
            chatCloser.style.position = 'absolute';
            chatCloser.style.top = '0';
            chatCloser.style.right = '0';
            chatCloser.addEventListener('click', function () {
                var window = document.getElementById('chatWindow');
                window.style.bottom = (window.style.bottom == '0px') ? '-251px' : '0px';
            });
            chatHeader.appendChild(chatCloser);

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
var Utils;
(function (Utils) {
    function addEvent(elem, type, eventHandle) {
        if (elem == null || typeof (elem) == 'undefined')
            return;
        if (elem.addEventListener) {
            elem.addEventListener(type, eventHandle, false);
        } else if (elem.attachEvent) {
            elem.attachEvent("on" + type, eventHandle);
        } else {
            elem["on" + type] = eventHandle;
        }
    }
    Utils.addEvent = addEvent;
    ;

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

    function ifNotDefault(value, callback) {
        if (value != -100)
            callback();
    }
    Utils.ifNotDefault = ifNotDefault;

    function createButton(text, id) {
        var button;
        var textcontent;

        button = document.createElement("div");
        textcontent = document.createElement("div");
        textcontent.textContent = text;

        if (id) {
            textcontent.id = id;
        }

        button.classList.add("button");
        button.appendChild(textcontent);

        return button;
    }
    Utils.createButton = createButton;

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
var Tabs;
(function (Tabs) {
    var lowestTabContainerId = 0;
    var tabContainer = document.getElementById("paneContainer");
    Tabs.bottomPadding = 200;
    var tabContainers = new Array();
    var selectedTab;

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
                tab.activate();
            } else {
                tab.deactivate();
            }

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

    function updateGameTabs() {
        if (selectedTab) {
            var height = selectedTab.scrollHeight;
            if (height > window.innerHeight - Tabs.bottomPadding) {
                height = window.innerHeight - Tabs.bottomPadding;
            }
            tabContainer.style.minHeight = height + 'px';
            tabContainer.style.maxHeight = height + 'px';
            tabContainer.style.overflowY = height >= window.innerHeight - Tabs.bottomPadding ? 'scroll' : 'hidden';
        }
    }
    Tabs.updateGameTabs = updateGameTabs;

    Utils.addEvent(window, 'resize', Tabs.updateGameTabs);
    setInterval(updateGameTabs, 20);

    function activateTab(containerId, tabId) {
        tabContainers[containerId].activate(tabId);
        updateGameTabs();
    }
    Tabs.activateTab = activateTab;

    var Tab = (function () {
        function Tab() {
        }
        Tab.prototype.deactivate = function () {
            Utils.cssSwap(this.button, 'active', 'inactive');
            this.pane.style.display = 'none';
            this.pane.style.overflow = 'hidden';
        };

        Tab.prototype.activate = function () {
            Utils.cssSwap(this.button, 'inactive', 'active');
            this.pane.style.display = 'block';
            this.pane.style.overflow = 'visible';
            selectedTab = this.pane;
        };
        return Tab;
    })();
})(Tabs || (Tabs = {}));
var Objects;
(function (Objects) {
    var gameobjects = new Array();

    var GameObject = (function () {
        function GameObject() {
            this.quantity = 0;
        }
        return GameObject;
    })();

    function register(id, name) {
        if (!gameobjects[id]) {
            var gameobject = new GameObject();
            gameobject.name = name;

            gameobjects[id] = gameobject;
        }
    }
    Objects.register = register;

    function lookupName(id) {
        return gameobjects[id].name;
    }
    Objects.lookupName = lookupName;

    function setQuantity(id, quantity) {
        gameobjects[id].quantity = quantity;
    }
    Objects.setQuantity = setQuantity;

    function getQuantity(id) {
        return gameobjects[id].quantity;
    }
    Objects.getQuantity = getQuantity;

    function setLifeTimeTotal(id, quantity) {
        gameobjects[id].lifeTimeTotal = quantity;
    }
    Objects.setLifeTimeTotal = setLifeTimeTotal;

    function getLifeTimeTotal(id) {
        return gameobjects[id].lifeTimeTotal;
    }
    Objects.getLifeTimeTotal = getLifeTimeTotal;
})(Objects || (Objects = {}));
var Inventory;
(function (Inventory) {
    Inventory.items = new Array();
    Inventory.configClickers = new Array();
    var inventoryPane;
    var inventory;
    var selectedItemPane;
    var selectedItemImage;
    var selectedItem;

    var configTableBody;
    var configTableContainer;
    var drinkButton;

    var configNames = new Array();
    var configImages = new Array();

    console.log("Typescript is still compiling. WTF?");

    var Item = (function () {
        function Item(id, name, worth, category) {
            this.id = id;
            this.name = name;
            this.worth = worth;
            this.category = category;
        }
        return Item;
    })();
    Inventory.Item = Item;

    (function (Category) {
        Category[Category["NFS"] = 0] = "NFS";
        Category[Category["ORE"] = 1] = "ORE";
        Category[Category["GEM"] = 2] = "GEM";
        Category[Category["INGREDIENT"] = 3] = "INGREDIENT";
        Category[Category["CRAFTING"] = 4] = "CRAFTING";
        Category[Category["POTION"] = 5] = "POTION";
    })(Inventory.Category || (Inventory.Category = {}));
    var Category = Inventory.Category;
    ;

    function getSelectedItemQuantity() {
        return selectedItem ? selectedItem.quantity : 0;
    }
    Inventory.getSelectedItemQuantity = getSelectedItemQuantity;

    function selectItem(id) {
        if (id) {
            if (selectedItem != null) {
                selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            }
            selectedItem = Inventory.items[id];
            selectedItemImage.classList.add(Utils.cssifyName(selectedItem.name));
            if (selectedItem.category == 5 /* POTION */) {
                drinkButton.style.display = 'inline-block';
            } else {
                drinkButton.style.display = 'none';
            }
            limitTextQuantity();
        } else {
            selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            selectedItem = null;
        }

        selectedItemPane.style.display = selectedItem == null ? 'none' : 'block';
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
        Inventory.items[item.id] = item;
        Objects.register(item.id, item.name);

        if (!inventoryPane)
            draw();

        if (item.category != 0 /* NFS */ && item.category != null)
            inventory.appendChild(drawItem(item));
        else
            document.getElementById('headerInventory').appendChild(drawItem(item));
    }

    function draw() {
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
        var selectedItemQuantity = document.createElement('INPUT');
        selectedItemQuantity.id = 'selecteditemquantity';
        selectedItemQuantity.type = 'text';
        selectedItemQuantity.style.height = '18px';
        selectedItemQuantity.classList.add('selected-item-quantity');
        selectedItemQuantity.addEventListener('input', function () {
            limitTextQuantity();
        });

        drinkButton = Utils.createButton('Drink', '');
        drinkButton.classList.add('selected-item-quantity');
        drinkButton.addEventListener('click', function () {
            Connection.drink(selectedItem.id);
            limitTextQuantity();
        });

        var sellItems = Utils.createButton('Sell', '');
        sellItems.classList.add('selected-item-quantity');
        sellItems.addEventListener('click', function () {
            var textbox = document.getElementById('selecteditemquantity');
            var quantity = +textbox.value;
            if (Utils.isNumber(quantity)) {
                Inventory.sellSelectedItem(quantity);
            }
            limitTextQuantity();
        });

        var sellAllItems = Utils.createButton('Sell all', '');
        sellAllItems.classList.add('selected-item-quantity');
        sellAllItems.addEventListener('click', function () {
            Inventory.sellAllSelectedItem();
            limitTextQuantity();
        });

        selectedItemPane.appendChild(selectedItemImage);
        selectedItemPane.appendChild(sellAllItems);

        selectedItemPane.appendChild(sellItems);
        selectedItemPane.appendChild(drinkButton);
        selectedItemPane.appendChild(selectedItemQuantity);
        inventoryPane.appendChild(selectedItemPane);

        var configDiv = document.createElement('DIV');
        configDiv.style.textAlign = 'center';
        var configPanel = document.createElement('DIV');
        configPanel.style.display = 'inline-block';
        var sellAll = Utils.createButton('Sell (...)', '');
        sellAll.addEventListener('click', function () {
            Connection.sellAllItems();
        });
        var sellAllConfig = Utils.createButton('...', '');
        sellAllConfig.addEventListener('click', function () {
            Inventory.toggleConfig();
        });
        configPanel.appendChild(sellAll);
        configPanel.appendChild(sellAllConfig);
        configDiv.appendChild(configPanel);
        inventoryPane.appendChild(configDiv);

        configTableContainer = document.createElement('DIV');
        configTableContainer.classList.add('config-container');
        configTableContainer.classList.add('closed');
        var configTable = document.createElement('TABLE');
        configTable.classList.add('config-table');
        configTableContainer.appendChild(configTable);

        var header = configTable.createTHead();
        var titleRow = header.insertRow(0);
        var realTitleRow = header.insertRow(0);
        realTitleRow.classList.add('table-header');
        var titleCell = realTitleRow.insertCell(0);
        titleCell.textContent = 'Inventory Configuration';
        titleCell.setAttribute('colspan', '10');
        titleRow.classList.add('table-subheader');

        for (var enumMember in Category) {
            var isValueProperty = parseInt(enumMember, 10) >= 0;
            if (isValueProperty) {
                if (Category[enumMember] != "NFS") {
                    var configCell = titleRow.insertCell(titleRow.cells.length);
                    configCell.classList.add('config-cell-check');

                    var titleCell = titleRow.insertCell(titleRow.cells.length);
                    titleCell.classList.add('config-cell-name');
                    titleCell.textContent = Category[enumMember];
                }
            }
        }
        configTableBody = configTable.createTBody();

        inventory = document.createElement('DIV');
        inventory.style.position = 'relative';
        inventory.appendChild(configTableContainer);
        inventoryPane.appendChild(inventory);

        Tabs.registerGameTab(inventoryPane, 'Inventory');
    }

    function modifyConfig(id, enabled) {
        if (!Inventory.configClickers[id])
            console.log(id);
        Inventory.configClickers[id].checked = enabled;
    }
    Inventory.modifyConfig = modifyConfig;

    function toggleConfig() {
        if (configTableContainer.classList.contains('closed')) {
            configTableContainer.classList.remove('closed');
        } else
            configTableContainer.classList.add('closed');
    }
    Inventory.toggleConfig = toggleConfig;

    function drawHeaderItem(item) {
        var itemElement = document.createElement('DIV');
        item.container = itemElement;

        itemElement.classList.add('header-item');

        var itemImage = document.createElement('DIV');
        itemImage.style.width = '32px';
        itemImage.style.height = '32px';
        itemImage.style.position = 'relative';
        itemImage.style.margin = '0 auto';
        var image = document.createElement('DIV');
        image.classList.add('Half-' + item.name.replace(' ', '_'));
        itemImage.appendChild(image);
        itemElement.appendChild(itemImage);

        var itemQuantity = document.createElement('DIV');
        item.quantityElm = itemQuantity;
        itemQuantity.classList.add("item-text");
        itemQuantity.textContent = Utils.formatNumber(0);
        itemElement.appendChild(itemQuantity);

        var itemValue = document.createElement('DIV');
        itemValue.style.display = 'none';
        item.worthElm = itemValue;
        itemElement.appendChild(itemValue);

        return itemElement;
    }

    function drawItem(item) {
        if (item.category == 0 /* NFS */ || item.category == null)
            return drawHeaderItem(item);

        var itemElement = document.createElement('DIV');
        item.container = itemElement;
        itemElement.classList.add("item");
        itemElement.addEventListener('click', function () {
            Inventory.selectItem(item.id);
        });
        tooltip.create(itemElement, item.name);

        var itemValueContainer = document.createElement('DIV');
        itemValueContainer.classList.add("item-text");
        var itemValue = document.createElement('DIV');
        itemValue.style.verticalAlign = 'top';
        itemValue.style.display = 'inline-block';
        itemValue.textContent = Utils.formatNumber(item.worth);
        item.worthElm = itemValue;
        var itemCurrency = document.createElement('DIV');
        itemCurrency.classList.add('Quarter-Coins');
        itemCurrency.style.display = 'inline-block';
        itemValueContainer.appendChild(itemCurrency);
        itemValueContainer.appendChild(itemValue);
        itemElement.appendChild(itemValueContainer);

        var itemImage = document.createElement('DIV');
        itemImage.style.width = '64px';
        itemImage.style.height = '64px';
        itemImage.style.position = 'relative';
        itemImage.style.margin = '0 auto';
        var image = document.createElement('DIV');
        image.classList.add(item.name.replace(' ', '_'));
        itemImage.appendChild(image);
        itemElement.appendChild(itemImage);

        var itemQuantity = document.createElement('DIV');
        item.quantityElm = itemQuantity;
        itemQuantity.classList.add("item-text");
        itemQuantity.textContent = Utils.formatNumber(0);
        itemElement.appendChild(itemQuantity);

        if (item.category != null) {
            var selectedItemCell;
            var selectedConfigCell;
            var rows = configTableBody.rows.length;
            var cellIndex = item.category;
            cellIndex *= 2;
            cellIndex--;

            for (var i = 0; i < rows; i++) {
                var testCell = configTableBody.rows[i].cells[cellIndex];
                if (testCell && testCell.childElementCount == 0) {
                    selectedItemCell = configTableBody.rows[i].cells[cellIndex];
                    selectedConfigCell = configTableBody.rows[i].cells[cellIndex - 1];
                    break;
                }
            }
            if (!selectedItemCell) {
                var row = configTableBody.insertRow(configTableBody.rows.length);
                row.classList.add('table-row');
                for (var enumMember in Category) {
                    var isValueProperty = parseInt(enumMember, 10) >= 0;
                    if (isValueProperty) {
                        if (Category[enumMember] != "NFS") {
                            var configCell = row.insertCell(row.cells.length);
                            configCell.classList.add('config-cell-check');

                            var titleCell = row.insertCell(row.cells.length);
                            titleCell.classList.add('config-cell-name');
                        }
                    }
                }
                selectedItemCell = row.cells[cellIndex];
                selectedConfigCell = row.cells[cellIndex - 1];
            }

            var nameAndImage = document.createElement('DIV');
            nameAndImage.classList.add('item-text');
            var nameSpan = document.createElement('SPAN');
            nameSpan.style.verticalAlign = 'top';
            var image = document.createElement('DIV');
            image.classList.add('Third-' + Utils.cssifyName(item.name));
            image.style.display = 'inline-block';
            configImages[item.id] = image;
            nameSpan.textContent = item.name;
            configNames[item.id] = nameSpan;
            nameAndImage.appendChild(image);
            nameAndImage.appendChild(nameSpan);
            selectedItemCell.appendChild(nameAndImage);
            var configChecker = document.createElement('INPUT');
            configChecker.type = 'CHECKBOX';
            var id = item.id;
            Inventory.configClickers[id] = configChecker;
            configChecker.addEventListener('change', function (e) {
                Connection.configureItem(id, Inventory.configClickers[id].checked);
            });
            selectedConfigCell.appendChild(configChecker);
        }

        return itemElement;
    }

    function addItem(id, name, worth, category) {
        if (!Inventory.items[id])
            add(new Item(id, name, worth, category));
    }
    Inventory.addItem = addItem;

    function changePrice(id, price) {
        Utils.ifNotDefault(price, function () {
            var item = Inventory.items[id];
            item.worth = price;
            item.worthElm.textContent = Utils.formatNumber(price);
        });
    }
    Inventory.changePrice = changePrice;

    function changeQuantity(id, quantity) {
        Utils.ifNotDefault(quantity, function () {
            Objects.setQuantity(id, quantity);
            Crafting.update();
            Inventory.items[id].quantityElm.textContent = Utils.formatNumber(quantity);
            Inventory.items[id].quantity = quantity;
            if (Inventory.items[id].category != 0 /* NFS */ && Inventory.items[id].category != null)
                Inventory.items[id].container.style.display = quantity == 0 ? 'none' : 'inline-block';
            else
                Inventory.items[id].container.style.display = Objects.getLifeTimeTotal(id) == 0 ? 'none' : 'inline-block';
            limitTextQuantity();
        });
    }
    Inventory.changeQuantity = changeQuantity;

    function update() {
        if (configNames.length <= 0)
            return;

        Inventory.items.forEach(function (item) {
            var itemQuantity = Objects.getLifeTimeTotal(item.id);
            if (configNames[item.id])
                configNames[item.id].textContent = itemQuantity > 0 ? item.name : '???';

            if (configImages[item.id])
                configImages[item.id].style.display = itemQuantity > 0 ? 'inline-block' : 'none';
        });
    }
    Inventory.update = update;
})(Inventory || (Inventory = {}));
var Connection;
(function (Connection) {
    var conInterval;
    Komodo.connection.received(function (msg) {
        Chat.log("Recieved " + roughSizeOfObject(msg) + " bytes from komodo.");
        Chat.log("Encoded: ");
        Chat.log(msg);
        Chat.log("Decoded: ");
        Chat.log(JSON.stringify(Komodo.decode(msg)));
        Chat.log(roughSizeOfObject(JSON.stringify(Komodo.decode(msg))) - roughSizeOfObject(msg) + " bytes saved.");
        msg = Komodo.decode(msg);

        if (msg.Messages != null) {
            receiveGlobalMessages(msg.Messages);
        }

        if (msg.GameSchema != null) {
            loadSchema(msg.GameSchema);
        }

        if (msg.Items != null) {
            updateInventory(msg.Items);
        }

        if (msg.StoreItemsUpdate != null) {
            updateStore(msg.StoreItemsUpdate);
        }
        if (msg.StatItemsUpdate != null) {
            updateStats(msg.StatItemsUpdate);
        }
        if (msg.ConfigItems != null) {
            updateInventoryConfigurations(msg.ConfigItems);
        }

        if (msg.Processors != null) {
            updateProcessors(msg.Processors);
        }

        if (msg.AntiCheatCoordinates != null) {
            antiCheat(msg.AntiCheatCoordinates);
        }

        if (msg.Buffs != null) {
            console.log(msg.Buffs);
        }
    });

    function restart() {
        Komodo.restart();
    }
    Connection.restart = restart;

    var actions = new Komodo.ClientActions();

    Komodo.connection.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.connected) {
            connected();
        }
        if (change.newState === $.signalR.connectionState.disconnected) {
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

            send(encoded);

            actions = new Komodo.ClientActions();
        }, 1000);
    }

    function loadSchema(schema) {
        if (schema.Items) {
            for (var i = 0; i < schema.Items.length; i++) {
                Inventory.addItem(schema.Items[i].Id, schema.Items[i].Name, schema.Items[i].Worth, schema.Items[i].Category);
                Statistics.addItem(schema.Items[i].Id, schema.Items[i].Name);
            }
        }

        if (schema.StoreItems) {
            for (var i = 0; i < schema.StoreItems.length; i++) {
                var item = schema.StoreItems[i];
                Store.addItem(item.Id, item.Category, item.Price, item.Factor, item.Name, item.MaxQuantity);
            }
        }

        if (schema.Processors) {
            for (var i = 0; i < schema.Processors.length; i++) {
                var processor = schema.Processors[i];
                Crafting.addProcessor(processor.Id, processor.Name);
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
    }

    function receiveGlobalMessages(messages) {
        for (var i = 0; i < messages.length; i++) {
            var msg = messages[i];
            Chat.receiveGlobalMessage(msg.Sender, msg.Text, msg.Time, msg.Permissions);
        }
    }

    function updateProcessors(processors) {
        for (var i = 0; i < processors.length; i++) {
            var processor = processors[i];
            Crafting.updateProcessor(processor.Id, processor.SelectedRecipe, processor.OperationDuration, processor.CompletedOperations, processor.TotalOperations, processor.Capacity);
        }
    }

    function antiCheat(ac) {
        Rock.moveRock(ac.X, ac.Y);
    }

    function updateStats(items) {
        for (var i = 0; i < items.length; i++)
            Statistics.changeStats(items[i].Id, items[i].PrestigeQuantity, items[i].LifeTimeQuantity);
    }

    function updateInventory(items) {
        for (var i = 0; i < items.length; i++) {
            Inventory.changeQuantity(items[i].Id, items[i].Quantity);
            Inventory.changePrice(items[i].Id, items[i].Worth);
        }
    }

    function updateInventoryConfigurations(items) {
        for (var i = 0; i < items.length; i++)
            Inventory.modifyConfig(items[i].Id, items[i].Enabled);
    }

    function updateStore(items) {
        for (var i = 0; i < items.length; i++)
            Store.changeQuantity(items[i].Id, items[i].Quantity, items[i].MaxQuantity, items[i].Price);
    }

    function drink(id) {
        var potionAction = new Komodo.ClientActions.PotionAction();
        potionAction.Id = id;
        actions.PotionActions.push(potionAction);
    }
    Connection.drink = drink;

    function mine(x, y) {
        var miningAction = new Komodo.ClientActions.MiningAction();
        miningAction.X = x;
        miningAction.Y = y;
        actions.MiningActions.push(miningAction);
    }
    Connection.mine = mine;

    function sellItem(id, quantity) {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        var sellAction = new Komodo.ClientActions.InventoryAction.SellAction();
        sellAction.Id = id;
        sellAction.Quantity = quantity;
        inventoryAction.Sell = sellAction;
        actions.InventoryActions.push(inventoryAction);
    }
    Connection.sellItem = sellItem;

    function configureItem(id, enabled) {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        var configAction = new Komodo.ClientActions.InventoryAction.ConfigAction();
        configAction.Id = id;
        configAction.Enabled = enabled;
        inventoryAction.Config = configAction;
        actions.InventoryActions.push(inventoryAction);
    }
    Connection.configureItem = configureItem;

    function purchaseItem(id, quantity) {
        var storeAction = new Komodo.ClientActions.StoreAction();
        var purchaseAction = new Komodo.ClientActions.StoreAction.PurchaseAction();
        purchaseAction.Id = id;
        purchaseAction.Quantity = (quantity ? quantity : 1);
        storeAction.Purchase = purchaseAction;
        actions.StoreActions.push(storeAction);
    }
    Connection.purchaseItem = purchaseItem;

    function sellAllItems() {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        inventoryAction.SellAll = true;
        actions.InventoryActions.push(inventoryAction);
    }
    Connection.sellAllItems = sellAllItems;

    function craftRecipe(id, quantity) {
        var craftingAction = new Komodo.ClientActions.CraftingAction();
        craftingAction.Id = id;
        craftingAction.Quantity = quantity;
        actions.CraftingActions.push(craftingAction);
    }
    Connection.craftRecipe = craftRecipe;

    function processRecipe(id, recipeIndex, iterations) {
        var processingAction = new Komodo.ClientActions.ProcessingAction();
        processingAction.Id = id;
        processingAction.RecipeIndex = recipeIndex;
        processingAction.Iterations = iterations;
        actions.ProcessingActions.push(processingAction);
    }
    Connection.processRecipe = processRecipe;

    function sendGlobalMessage(message) {
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
var Crafting;
(function (Crafting) {
    var storePane;
    var processorSection;
    var craftingSection;
    var craftingTable;
    var cellDescriptions = ['Action', 'Description', 'Input', 'Output', 'Name'];
    var cellWidths = ['10%', '50%', '15%', '15%', '10%'];
    var cellMinWidths = ['170px', '0', '0', '0', '0'];

    var processorCellDescriptions = ['Action', 'Output', 'Input', 'Capacity', 'Image'];
    var processorCellWidths = ['10%', '30%', '20%', '20%', '20%'];
    var processorCellMinWidths = ['170px', '0', '0', '0', '0'];

    var itemsTableOffset = 3;
    var recipes = new Array();
    Crafting.processors = new Array();

    var Processor = (function () {
        function Processor() {
            this._recipes = new Array();
        }
        Processor.prototype.addRecipe = function (recipe) {
            this._recipes.push(recipe);
            if (!this.selectedRecipe)
                this.selectedRecipe = 0;
        };
        return Processor;
    })();
    Crafting.Processor = Processor;

    var Recipe = (function () {
        function Recipe() {
            this.ingredients = new Array();
            this.resultants = new Array();
        }
        Recipe.prototype.addIngredient = function (ingredient) {
            this.ingredients.push(ingredient);
        };

        Recipe.prototype.addResultant = function (ingredient) {
            this.resultants.push(ingredient);
        };
        return Recipe;
    })();
    Crafting.Recipe = Recipe;

    var Ingredient = (function () {
        function Ingredient(id, quantity) {
            this.id = id;
            this.quantity = quantity;
        }
        return Ingredient;
    })();
    Crafting.Ingredient = Ingredient;

    function draw() {
        storePane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(storePane);
        Tabs.registerGameTab(storePane, 'Crafting');

        processorSection = document.createElement('DIV');
        storePane.appendChild(processorSection);

        craftingSection = document.createElement('DIV');
        storePane.appendChild(craftingSection);
        drawCraftingTable();
    }

    function drawCraftingTable() {
        craftingTable = document.createElement('TABLE');
        craftingTable.classList.add('block-table');

        var header = craftingTable.createTHead();
        var titleRow = header.insertRow(0);
        titleRow.classList.add('table-subheader');
        var realTitleRow = header.insertRow(0);
        realTitleRow.classList.add('table-header');

        var titleCell = realTitleRow.insertCell(0);
        titleCell.colSpan = cellDescriptions.length;
        titleCell.textContent = 'Crafting Table';

        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = titleRow.insertCell(0);
            cell.style.width = cellWidths[i];
            cell.textContent = cellDescriptions[i];

            if (cellMinWidths[i] != '0')
                cell.style.minWidth = cellMinWidths[i];
        }

        var itemsSubHeader = craftingTable.insertRow(2);
        itemsSubHeader.classList.add('table-subheader');
        var itemsSubHeaderCell = itemsSubHeader.insertCell(0);
        itemsSubHeaderCell.colSpan = cellDescriptions.length;
        itemsSubHeaderCell.textContent = 'Items';

        var upgradesSubHeader = craftingTable.insertRow(3);
        upgradesSubHeader.classList.add('table-subheader');
        var upgradesSubHeaderCell = upgradesSubHeader.insertCell(0);
        upgradesSubHeaderCell.colSpan = cellDescriptions.length;
        upgradesSubHeaderCell.textContent = 'Upgrades';

        craftingSection.appendChild(craftingTable);
    }

    function addRecipe(id, ingredients, resultants, isItem) {
        if (!storePane)
            draw();

        if (!recipes[id]) {
            var recipe = new Recipe();
            recipe.id = id;
            recipe.isItem = isItem;
            recipes[id] = recipe;

            for (var i = 0; i < ingredients.length; i++)
                recipe.addIngredient(new Ingredient(ingredients[i].Id, ingredients[i].Quantity));

            for (var i = 0; i < resultants.length; i++)
                recipe.addResultant(new Ingredient(resultants[i].Id, resultants[i].Quantity));

            drawRecipe(recipe, isItem);
        }
    }
    Crafting.addRecipe = addRecipe;

    function addProcessor(id, name) {
        if (!storePane)
            draw();

        if (!Crafting.processors[id]) {
            var processor = new Processor();
            processor.id = id;
            processor.name = name;
            Crafting.processors[id] = processor;

            drawProcessor(processor);
        }
    }
    Crafting.addProcessor = addProcessor;

    function hasRecipe(id, resultantId) {
        if (!Crafting.processors[id])
            return;
        var processor = Crafting.processors[id];

        for (var i = 0; i < processor._recipes.length; i++) {
            if (resultantId == processor._recipes[i].resultants[0].id)
                return true;
        }
        return false;
    }

    function addProcessorRecipe(id, ingredients, resultants) {
        if (!Crafting.processors[id])
            return;
        var processor = Crafting.processors[id];

        if (hasRecipe(id, resultants[0].Id))
            return;

        var recipe = new Recipe();

        for (var i = 0; i < ingredients.length; i++)
            recipe.addIngredient(new Ingredient(ingredients[i].Id, ingredients[i].Quantity));

        for (var i = 0; i < resultants.length; i++)
            recipe.addResultant(new Ingredient(resultants[i].Id, resultants[i].Quantity));

        processor.addRecipe(recipe);

        var opt = document.createElement('OPTION');
        opt.textContent = Objects.lookupName(resultants[0].Id);
        processor.recipeSelector.appendChild(opt);

        if (processor._recipes.length == 1) {
            switchProcessorRecipe(id, 0);
        }
    }
    Crafting.addProcessorRecipe = addProcessorRecipe;

    function switchProcessorRecipe(id, recipeIndex) {
        var processor = Crafting.processors[id];
        if (!processor)
            return;

        var recipe = processor._recipes[recipeIndex];
        processor.selectedRecipe = recipeIndex;

        while (processor.recipeList.lastChild) {
            processor.recipeList.removeChild(processor.recipeList.lastChild);
        }

        for (var x = 0; x < recipe.ingredients.length; x++) {
            var ingredientBox = document.createElement('DIV');
            ingredientBox.classList.add('item-text');
            ingredientBox.style.height = '30px';
            var ingredientImage = document.createElement('DIV');
            ingredientImage.style.display = 'inline-block';
            var ingredientQuantity = document.createElement('DIV');
            recipe.ingredients[x].quantityDiv = ingredientQuantity;
            ingredientQuantity.style.display = 'inline-block';
            ingredientQuantity.style.verticalAlign = 'super';
            ingredientQuantity.style.color = (recipe.ingredients[x].quantity <= Objects.getQuantity(recipe.ingredients[x].id)) ? 'darkgreen' : 'darkred';
            ingredientQuantity.textContent = Utils.formatNumber(recipe.ingredients[x].quantity);
            ingredientImage.classList.add("Half-" + Utils.cssifyName(Objects.lookupName(recipe.ingredients[x].id)));

            ingredientBox.appendChild(ingredientImage);
            ingredientBox.appendChild(ingredientQuantity);
            processor.recipeList.appendChild(ingredientBox);
        }
    }

    function drawProcessor(processor) {
        var processorTable = document.createElement('TABLE');
        processorTable.classList.add('block-table');

        var header = processorTable.createTHead();
        var titleRow = header.insertRow(0);
        titleRow.classList.add('table-subheader');
        var realTitleRow = header.insertRow(0);
        realTitleRow.classList.add('table-header');

        var titleCell = realTitleRow.insertCell(0);
        titleCell.colSpan = cellDescriptions.length;
        titleCell.textContent = processor.name;

        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = titleRow.insertCell(0);
            cell.style.width = processorCellWidths[i];
            cell.textContent = processorCellDescriptions[i];

            if (processorCellWidths[i] != '0')
                cell.style.minWidth = processorCellMinWidths[i];
        }

        var progressRow = processorTable.insertRow(2);
        progressRow.classList.add('table-row');
        var progressCell = progressRow.insertCell(0);
        progressCell.colSpan = processorCellDescriptions.length;
        var progressContainer = document.createElement('DIV');
        progressContainer.classList.add('progress-bar-container');
        var progressBar = document.createElement('DIV');
        progressBar.classList.add('progress-bar');
        var progressTextContainer = document.createElement('DIV');
        progressTextContainer.classList.add('progress-bar-text-container');
        var progressText = document.createElement('DIV');
        progressText.classList.add('progress-bar-text');
        progressTextContainer.appendChild(progressText);
        progressContainer.appendChild(progressTextContainer);

        processor.progressBar = progressBar;
        processor.progressText = progressText;
        progressContainer.appendChild(progressBar);
        progressCell.appendChild(progressContainer);

        var contentRow = processorTable.insertRow(3);
        contentRow.classList.add('table-row');

        for (var i = 0; i < processorCellDescriptions.length; i++) {
            var cell = contentRow.insertCell(0);
            cell.style.width = cellWidths[i];
            cell.style.height = '75px';

            if (processorCellMinWidths[i] != '0')
                cell.style.minWidth = processorCellMinWidths[i];

            if (processorCellDescriptions[i] == "Image") {
                var image = document.createElement('DIV');
                image.classList.add(Utils.cssifyName(processor.name));
                image.style.margin = '0 auto';
                cell.appendChild(image);
            }

            if (processorCellDescriptions[i] == "Capacity") {
                cell.textContent = '0';
                processor.capacityElm = cell;
            }

            if (processorCellDescriptions[i] == "Input") {
                processor.recipeList = cell;
            }

            if (processorCellDescriptions[i] == "Output") {
                var maxButton = Utils.createButton('Max', '');
                cell.appendChild(maxButton);

                var quantitySelector = document.createElement('INPUT');
                quantitySelector.type = 'TEXT';
                quantitySelector.style.width = '35px';
                processor.quantityTextbox = quantitySelector;
                cell.appendChild(quantitySelector);
                var id = processor.id;
                var selector = document.createElement('SELECT');
                selector.addEventListener('change', function (e) {
                    switchProcessorRecipe(id, selector.selectedIndex);
                });
                processor.recipeSelector = selector;
                cell.appendChild(selector);
            }

            if (processorCellDescriptions[i] == "Action") {
                var activateBtn = Utils.createButton('Activate', '');
                activateBtn.addEventListener('click', function () {
                    if (Utils.isNumber(processor.quantityTextbox.value))
                        Connection.processRecipe(processor.id, processor.recipeSelector.selectedIndex, +processor.quantityTextbox.value);
                }, false);
                cell.appendChild(activateBtn);
            }
        }

        processorSection.appendChild(processorTable);
    }

    function updateProcessor(id, selectedRecipe, operationDuration, completedOperations, totalOperations, capacity) {
        var processor = Crafting.processors[id];
        if (!processor)
            return;

        var progressChanged = false;

        Utils.ifNotDefault(selectedRecipe, function () {
            processor.selectedRecipe = selectedRecipe;
        });

        Utils.ifNotDefault(totalOperations, function () {
            processor.totalOperations = totalOperations;
            progressChanged = true;
        });

        Utils.ifNotDefault(operationDuration, function () {
            processor.operationDuration = operationDuration;
        });

        Utils.ifNotDefault(capacity, function () {
            processor.capacityElm.textContent = capacity.toString();
        });

        Utils.ifNotDefault(completedOperations, function () {
            processor.completedOperations = completedOperations;
            progressChanged = true;
        });

        if (progressChanged) {
            if (processor.totalOperations <= 0)
                return;
            if (processor.completedOperations == processor.totalOperations)
                return;

            processor.operationStartTime = Date.now();
            processor.operationCompletionTime = processor.operationStartTime + (processor.operationDuration * 1000);
            console.log('Start: ' + processor.operationStartTime + ' End: ' + processor.operationCompletionTime);
        }

        if (processor.selectedRecipe > -1) {
            try  {
                processor.progressText.textContent = Objects.lookupName(processor._recipes[processor.selectedRecipe].resultants[0].id) + ' (' + processor.completedOperations + '/' + processor.totalOperations + ')';
            } catch (err) {
                console.log("invalid processor recipe " + processor.selectedRecipe);
            }
        } else {
            processor.progressText.textContent = '';
        }
    }
    Crafting.updateProcessor = updateProcessor;

    function processorBars() {
        Crafting.processors.forEach(function (processor) {
            if (processor.operationDuration <= 0)
                processor.progressBar.style.width = '0%';
            else if (processor.completedOperations != processor.totalOperations && processor.totalOperations > 0) {
                var timeToFinish = processor.operationCompletionTime - Date.now();
                timeToFinish /= 1000;

                var completionPerc = timeToFinish / processor.operationDuration;
                completionPerc *= 100;
                completionPerc = 100 - completionPerc;

                processor.progressBar.style.width = completionPerc + '%';
            }
        });
    }
    setInterval(processorBars, 10);

    function update() {
        if (!storePane)
            return;

        recipes.forEach(function (recipe) {
            var quantity = Objects.getQuantity(recipe.id);
            recipe.row.style.display = (quantity == -1 || !recipe.isItem && quantity > 0) ? 'none' : '';
            recipe.ingredients.forEach(function (ingredient) {
                var ingQuantity = Objects.getQuantity(ingredient.id);
                ingredient.quantityDiv.style.color = (ingQuantity >= ingredient.quantity) ? 'darkgreen' : 'darkred';
            });
        });

        Crafting.processors.forEach(function (processor) {
            switchProcessorRecipe(processor.id, processor.recipeSelector.selectedIndex);
        });
    }
    Crafting.update = update;

    function drawRecipe(recipe, isItem) {
        var pointOfInsertion = craftingTable.rows.length;

        if (isItem) {
            pointOfInsertion = itemsTableOffset;
            itemsTableOffset++;
        }

        var recipeRow = craftingTable.insertRow(pointOfInsertion);
        recipeRow.classList.add('table-row');
        recipe.row = recipeRow;
        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = recipeRow.insertCell(0);
            cell.style.width = cellWidths[i];

            if (cellMinWidths[i] != '0')
                cell.style.minWidth = cellMinWidths[i];

            if (cellDescriptions[i] == "Name")
                cell.textContent = Objects.lookupName(recipe.id);

            if (cellDescriptions[i] == "Input") {
                for (var x = 0; x < recipe.ingredients.length; x++) {
                    var ingredientBox = document.createElement('DIV');
                    ingredientBox.classList.add('item-text');
                    ingredientBox.style.height = '22px';
                    var ingredientImage = document.createElement('DIV');
                    ingredientImage.style.display = 'inline-block';
                    var ingredientQuantity = document.createElement('DIV');
                    recipe.ingredients[x].quantityDiv = ingredientQuantity;
                    ingredientQuantity.style.display = 'inline-block';
                    ingredientQuantity.style.verticalAlign = 'super';
                    ingredientQuantity.textContent = Utils.formatNumber(recipe.ingredients[x].quantity);
                    ingredientImage.classList.add("Third-" + Utils.cssifyName(Objects.lookupName(recipe.ingredients[x].id)));

                    ingredientBox.appendChild(ingredientImage);
                    ingredientBox.appendChild(ingredientQuantity);
                    cell.appendChild(ingredientBox);
                }
            }

            if (cellDescriptions[i] == "Output") {
                for (var x = 0; x < recipe.resultants.length; x++) {
                    var ingredientBox = document.createElement('DIV');
                    ingredientBox.classList.add('item-text');
                    ingredientBox.style.height = '22px';
                    var ingredientImage = document.createElement('DIV');
                    ingredientImage.style.display = 'inline-block';
                    var ingredientQuantity = document.createElement('DIV');
                    ingredientQuantity.style.display = 'inline-block';
                    ingredientQuantity.style.verticalAlign = 'super';
                    ingredientQuantity.textContent = Utils.formatNumber(recipe.resultants[x].quantity);
                    ingredientImage.classList.add("Third-" + Utils.cssifyName(Objects.lookupName(recipe.resultants[x].id)));

                    ingredientBox.appendChild(ingredientImage);

                    if (recipe.isItem)
                        ingredientBox.appendChild(ingredientQuantity);

                    cell.appendChild(ingredientBox);
                }
            }

            if (cellDescriptions[i] == "Action") {
                var craftBtn = Utils.createButton('Craft', '');
                craftBtn.addEventListener('click', function () {
                    Connection.craftRecipe(recipe.id, 1);
                }, false);
                cell.appendChild(craftBtn);

                if (recipe.isItem) {
                    var quantity = document.createElement('INPUT');
                    quantity.type = 'TEXT';
                    quantity.style.width = '30px';

                    var craftXBtn = Utils.createButton('Craft-x', '');
                    craftXBtn.addEventListener('click', function () {
                        Connection.craftRecipe(recipe.id, +quantity.value);
                    }, false);
                    cell.appendChild(craftXBtn);
                    cell.appendChild(quantity);
                }
            }
        }
    }
})(Crafting || (Crafting = {}));
var modal;
(function (modal) {
    var timeOpened = 0;
    var modalPane;
    modal.activeWindow;
    modal.intervalIdentifier;

    function hide() {
        if (modal.activeWindow) {
            modal.activeWindow.hide();
        }
        modal.activeWindow = null;
    }
    modal.hide = hide;

    function close() {
        if (modal.activeWindow) {
            var a = modal.activeWindow;
            hide();
            a.container.parentNode.removeChild(a.container);
        }
    }
    modal.close = close;

    var Window = (function () {
        function Window() {
            this.container = document.createElement("div");
            this.container.addEventListener("click", function (e) {
                e.stopPropagation();
            }, false);
            this.container.classList.add("modal-window");
            if (!modalPane) {
                var pane = document.createElement("div");
                modalPane = pane;
                pane.classList.add("modal-wrapper");
                pane.addEventListener("click", function (e) {
                    e.stopPropagation();
                    if ((Date.now() - timeOpened) > 3000)
                        modal.close();
                }, false);
                document.body.appendChild(pane);
            }
            modalPane.appendChild(this.container);

            this.titleEl = document.createElement("div");
            this.titleEl.classList.add("modal-header");
            this.bodyEl = document.createElement("div");

            this.container.appendChild(this.titleEl);
            this.container.appendChild(this.bodyEl);
        }
        Object.defineProperty(Window.prototype, "title", {
            get: function () {
                return this._title;
            },
            set: function (s) {
                this._title = s;
                this.titleEl.textContent = this._title;
            },
            enumerable: true,
            configurable: true
        });

        Window.prototype.addElement = function (el) {
            this.bodyEl.appendChild(el);
        };

        Window.prototype.addOption = function (opt) {
            if (!this.options) {
                this.options = document.createElement("div");
                this.options.classList.add("modal-options");
                this.container.appendChild(this.options);
            }
            var optionContainer = document.createElement("span");
            optionContainer.classList.add("modal-option");

            var option = document.createElement("span");
            option.textContent = opt;

            optionContainer.appendChild(option);
            this.options.appendChild(optionContainer);
            return optionContainer;
        };

        Window.prototype.addAffirmativeOption = function (opt) {
            var option = this.addOption(opt);
            option.classList.add("affirmative");
            return option;
        };

        Window.prototype.addNegativeOption = function (opt) {
            var option = this.addOption(opt);
            option.classList.add("negative");
            return option;
        };

        Window.prototype.show = function () {
            if (!this.container.classList.contains("opened"))
                this.container.classList.add("opened");
            if (!modalPane.classList.contains("opened"))
                modalPane.classList.add("opened");
            modal.activeWindow = this;
            updatePosition();
            modal.intervalIdentifier = setInterval(updatePosition, 100);
            timeOpened = Date.now();
        };

        Window.prototype.hide = function () {
            if (this.container.classList.contains("opened"))
                this.container.classList.remove("opened");
            if (modalPane.classList.contains("opened"))
                modalPane.classList.remove("opened");
        };
        return Window;
    })();
    modal.Window = Window;

    function updatePosition() {
        if (!modal.activeWindow) {
            clearInterval(modal.intervalIdentifier);
        } else {
            var containerDimensions = modal.activeWindow.container.getBoundingClientRect();
            modal.activeWindow.container.style.left = (window.innerWidth / 2) - ((containerDimensions.right - containerDimensions.left) / 2) + "px";
            modal.activeWindow.container.style.top = (window.innerHeight / 2) - ((containerDimensions.bottom - containerDimensions.top) / 2) + "px";
        }
    }
})(modal || (modal = {}));
var Account;
(function (Account) {
    var container;
    var userButton;
    var userSpan;
    var contextMenu;

    var mouseTimeout;

    function draw() {
        container = document.createElement('DIV');
        container.classList.add('account-manager');
        container.classList.add('closed');

        container.onmouseenter = function () {
            clearTimeout(mouseTimeout);
        };

        container.onmouseleave = function () {
            mouseTimeout = setTimeout(hideMenu, 250);
        };

        var loginButton = document.createElement('DIV');
        loginButton.textContent = 'Sign in';
        loginButton.classList.add('account-option');
        loginButton.classList.add('anonymous-account-option');
        loginButton.addEventListener('click', function () {
            loginModal();
        });
        container.appendChild(loginButton);

        var registerButton = document.createElement('DIV');
        registerButton.textContent = 'Register';
        registerButton.classList.add('account-option');
        registerButton.classList.add('anonymous-account-option');
        registerButton.addEventListener('click', function () {
            registerModal();
        });
        container.appendChild(registerButton);

        var optionsButton = document.createElement('DIV');
        optionsButton.textContent = 'Options';
        optionsButton.classList.add('account-option');
        optionsButton.classList.add('registered-account-option');
        container.appendChild(optionsButton);

        var logoffButton = document.createElement('DIV');
        logoffButton.textContent = 'Sign out';
        logoffButton.classList.add('account-option');
        logoffButton.classList.add('registered-account-option');
        logoffButton.addEventListener('click', function () {
            logoff();
        });
        container.appendChild(logoffButton);

        userButton = document.createElement('DIV');
        userButton.classList.add('account-user');
        userSpan = document.createElement('SPAN');
        userButton.appendChild(userSpan);
        container.appendChild(userButton);

        document.body.appendChild(container);
        userButton.addEventListener('click', function () {
            toggleMenu();
        });

        info();
    }
    draw();
    function toggleMenu() {
        if (container.classList.contains('closed'))
            container.classList.remove('closed');
        else
            container.classList.add('closed');
    }

    function hideMenu() {
        if (!container.classList.contains('closed'))
            container.classList.add('closed');
    }

    function updateUser(name, isAnon) {
        userSpan.textContent = isAnon ? 'Guest' : name;

        Utils.cssSwap(container, isAnon ? 'registered' : 'anonymous', isAnon ? 'anonymous' : 'registered');
    }

    function loginModal() {
        var loginModal = new modal.Window();
        var formControlsContainer = document.createElement('DIV');
        formControlsContainer.style.width = '400px';

        var usernameContainer = document.createElement('DIV');
        usernameContainer.style.marginBottom = '5px';
        var username = document.createElement("INPUT");
        username.type = 'TEXT';
        username.maxLength = 16;
        username.placeholder = 'Username';
        usernameContainer.appendChild(username);

        var passwordContainer = document.createElement('DIV');
        passwordContainer.style.marginBottom = '5px';
        var password = document.createElement("INPUT");
        password.type = 'PASSWORD';
        password.pattern = ".{6,}";
        password.placeholder = 'Password';
        passwordContainer.appendChild(password);

        var rememberMeContainer = document.createElement('DIV');
        rememberMeContainer.style.marginBottom = '5px';
        var rememberMe = document.createElement('INPUT');
        rememberMe.type = 'CHECKBOX';
        rememberMe.placeholder = 'Stay logged in on this computer?';
        rememberMeContainer.appendChild(rememberMe);

        formControlsContainer.appendChild(usernameContainer);
        formControlsContainer.appendChild(passwordContainer);
        formControlsContainer.appendChild(rememberMeContainer);

        loginModal.title = 'Log in';
        loginModal.addElement(formControlsContainer);

        var no = loginModal.addNegativeOption("Cancel");
        no.addEventListener("click", function () {
            modal.close();
        }, false);
        var yes = loginModal.addAffirmativeOption("Submit");
        yes.addEventListener("click", function () {
            login(username.value, password.value, rememberMe.checked);
            modal.close();
        }, false);
        loginModal.show();
    }

    function registerModal() {
        var registerModal = new modal.Window();
        var formControlsContainer = document.createElement('DIV');
        formControlsContainer.style.width = '400px';

        var usernameContainer = document.createElement('DIV');
        usernameContainer.style.marginBottom = '5px';
        var username = document.createElement("INPUT");
        username.type = 'TEXT';
        username.maxLength = 16;
        username.placeholder = 'Username';
        usernameContainer.appendChild(username);

        var emailContainer = document.createElement('DIV');
        emailContainer.style.marginBottom = '5px';
        var email = document.createElement("INPUT");
        email.type = 'EMAIL';
        email.placeholder = 'Email';
        emailContainer.appendChild(email);

        var passwordContainer = document.createElement('DIV');
        passwordContainer.style.marginBottom = '5px';
        var password = document.createElement("INPUT");
        password.type = 'PASSWORD';
        password.pattern = ".{6,}";
        password.placeholder = 'Password';
        passwordContainer.appendChild(password);

        var confpassContainer = document.createElement('DIV');
        confpassContainer.style.marginBottom = '5px';
        var confirmPassword = document.createElement("INPUT");
        confirmPassword.type = 'PASSWORD';
        confirmPassword.pattern = ".{6,}";
        confirmPassword.placeholder = 'Confirm password';
        confpassContainer.appendChild(confirmPassword);
        confirmPassword.onblur = function () {
            if (password.value != confirmPassword.value)
                confirmPassword.setCustomValidity('Passwords are not the same.');
        };

        confirmPassword.onfocus = function () {
            confirmPassword.setCustomValidity('');
        };
        formControlsContainer.appendChild(usernameContainer);
        formControlsContainer.appendChild(emailContainer);
        formControlsContainer.appendChild(passwordContainer);
        formControlsContainer.appendChild(confpassContainer);

        registerModal.addElement(formControlsContainer);

        registerModal.title = "Register";

        var no = registerModal.addNegativeOption("Cancel");
        no.addEventListener("click", function () {
            modal.close();
        }, false);
        var yes = registerModal.addAffirmativeOption("Submit");
        yes.addEventListener("click", function () {
            create(email.value, username.value, password.value, confirmPassword.value);
            modal.close();
        }, false);
        registerModal.show();
    }

    function create(email, username, password, passwordConfirmation) {
        var request = $.ajax({
            type: 'POST',
            url: '/Api/Account/Register',
            data: $.param({ Email: email, UserName: username, Password: password, ConfirmPassword: passwordConfirmation }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            success: function (request) {
                console.log(request.responseText);
                Connection.restart();
                info();
            }
        });
    }

    function login(email, password, rememberMe) {
        var request = $.ajax({
            type: 'POST',
            url: '/Api/Account/Login',
            data: $.param({ UserName: email, Password: password, RememberMe: rememberMe }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            success: function (request) {
                console.log(request);
                Connection.restart();
                info();
            }
        });
    }

    function logoff() {
        var request = $.ajax({
            type: 'POST',
            url: '/Api/Account/LogOff',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            success: function (request) {
                console.log(request);
                Connection.restart();
                info();
                location.reload();
            }
        });
    }

    function info() {
        var request = $.ajax({
            type: 'POST',
            url: '/Api/Account/Info',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            success: function (request) {
                console.log(request);
                request = JSON.parse(request);
                updateUser(request.UserName, request.Anonymous);
            }
        });
    }
    Account.info = info;
})(Account || (Account = {}));
var Rock;
(function (Rock) {
    var canvas = document.getElementById('rock');
    var context = canvas.getContext('2d');
    var relativeRockURL = '/Content/Rock.png';
    var relativeStoneURL = '/Content/Stone.png';
    var rockImage = new Image();
    var stoneImage = new Image(16, 16);
    var stoneLoaded = false;
    var lastX = 0;
    var lastY = 0;
    var rockSize = 64;
    var rockGrowth = 4;
    var rockIsBig = false;
    var mouseDown = false;

    function initialize() {
        rockImage.onload = function () {
            drawBackground();
            console.log('rock loaded');
        };
        rockImage.src = relativeRockURL;

        stoneImage.onload = function () {
            stoneLoaded = true;
        };
        stoneImage.src = relativeStoneURL;

        canvas.addEventListener('mousemove', function (e) {
            var mousePos = getMousePos(canvas, e);
            isOverRock(mousePos.x, mousePos.y);
        }, false);
        canvas.addEventListener('mousedown', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = true;
            isOverRock(mousePos.x, mousePos.y);
            Connection.mine(mousePos.x, mousePos.y);
        }, false);
        canvas.addEventListener('mouseup', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = false;
            isOverRock(mousePos.x, mousePos.y);
        }, false);
        canvas.addEventListener('mouseleave', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = false;
            isOverRock(mousePos.x, mousePos.y);
        }, false);
    }

    function getMousePos(canvas, evt) {
        var rect = canvas.getBoundingClientRect();
        return {
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        };
    }

    function isOverRock(x, y) {
        if (x > lastX && x < (lastX + rockSize) && y > lastY && y < (lastY + rockSize)) {
            if (!mouseDown)
                drawRock(lastX - (rockGrowth / 2), lastY - (rockGrowth / 2), rockSize + rockGrowth, rockSize + rockGrowth);
            else
                drawRock(lastX + (rockGrowth / 2), lastY + (rockGrowth / 2), rockSize - rockGrowth, rockSize - rockGrowth);

            rockIsBig = true;
        } else if (rockIsBig) {
            drawRock(lastX, lastY, rockSize, rockSize);
            rockIsBig = false;
        }
    }

    function moveRock(x, y) {
        if (x != lastX && y != lastY) {
            lastX = x;
            lastY = y;
            if (stoneLoaded)
                drawRock(x, y, rockSize, rockSize);
            else
                setTimeout(function () {
                    moveRock(x, y);
                }, 10);
        }
    }
    Rock.moveRock = moveRock;

    function clearCanvas() {
        context.clearRect(0, 0, 250, 250);
    }

    function drawBackground() {
        context.drawImage(rockImage, 0, 0);
    }

    function drawRock(x, y, xScale, yScale) {
        clearCanvas();
        drawBackground();
        context.drawImage(stoneImage, x, y, xScale, yScale);
    }

    initialize();
})(Rock || (Rock = {}));
var Statistics;
(function (Statistics) {
    var statsPane;
    var itemsBody;
    var items = new Array();
    var Item = (function () {
        function Item() {
        }
        return Item;
    })();

    function changeStats(id, prestige, lifetime) {
        var item = items[id];

        Utils.ifNotDefault(prestige, function () {
            item.prestigeQuantity = prestige;
            item.prestigeRow.textContent = Utils.formatNumber(prestige);
        });

        Utils.ifNotDefault(lifetime, function () {
            item.lifetimeQuantity = lifetime;
            Objects.setLifeTimeTotal(id, lifetime);
            item.alltimeRow.textContent = Utils.formatNumber(lifetime);
        });
    }
    Statistics.changeStats = changeStats;

    function addItem(id, name) {
        if (!statsPane)
            draw();

        if (!items[id]) {
            var item = new Item();
            items[id] = item;

            var row = itemsBody.insertRow(itemsBody.rows.length);
            row.classList.add('table-row');
            item.alltimeRow = row.insertCell(0);
            item.alltimeRow.style.width = '40%';
            item.prestigeRow = row.insertCell(0);
            item.prestigeRow.style.width = '40%';
            var imageRow = row.insertCell(0);
            imageRow.style.width = '20%';
            var image = document.createElement('DIV');
            image.classList.add('Third-' + Utils.cssifyName(name));
            image.style.display = 'inline-block';
            imageRow.appendChild(image);
        }
    }
    Statistics.addItem = addItem;

    function draw() {
        statsPane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(statsPane);
        Tabs.registerGameTab(statsPane, 'Statistics');

        statsPane.appendChild(drawItemsTable());
    }

    function drawItemsTable() {
        var itemsTable = document.createElement('TABLE');

        var header = itemsTable.createTHead();
        var titleRow = header.insertRow(0);
        titleRow.classList.add('table-header');
        var titleCell = titleRow.insertCell(0);
        titleCell.textContent = 'Item Statistics';
        titleCell.setAttribute('colspan', '3');

        var descriptionsRow = header.insertRow(1);
        descriptionsRow.classList.add('table-subheader');
        var lifetime = descriptionsRow.insertCell(0);
        lifetime.textContent = 'Lifetime Quantity';
        lifetime.style.width = '40%';
        var prestige = descriptionsRow.insertCell(0);
        prestige.textContent = 'Prestige Quantity';
        prestige.style.width = '40%';
        var item = descriptionsRow.insertCell(0);
        item.textContent = 'Item';
        item.style.width = '20%';

        itemsBody = itemsTable.createTBody();

        return itemsTable;
    }
})(Statistics || (Statistics = {}));
var Store;
(function (Store) {
    var storePane;
    var items = new Array();
    var categories = new Array();
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
        categories[name] = categoryContainer;

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

    function addItem(id, category, price, factor, name, maxQuantity) {
        if (!storePane)
            draw();

        if (!items[id]) {
            var item = new StoreItem();
            item.id = id;
            item.category = category;
            item.price = price;
            item.factor = factor;
            item.name = name;
            item.maxQuantity = maxQuantity ? maxQuantity : 0;

            Objects.register(item.id, item.name);

            var categoryContainer = categories[Category[category]];
            if (categoryContainer == null) {
                categoryContainer = categories["MINING"];
            }
            if (item.category != 6 /* CRAFTING */)
                categoryContainer.appendChild(drawItem(item));

            items[id] = item;
        }
    }
    Store.addItem = addItem;

    function changeQuantity(id, quantity, maxQuantity, price) {
        var item = items[id];

        Utils.ifNotDefault(maxQuantity, function () {
            if (maxQuantity != 0) {
                try  {
                    item.maxQuantity = maxQuantity;
                    item.maxQuantityElm.textContent = maxQuantity.toString();
                } catch (err) {
                }
            }
        });

        Utils.ifNotDefault(quantity, function () {
            Objects.setQuantity(id, quantity);
            item.quantity = quantity;
            Crafting.update();

            if (maxQuantity != 0) {
                if (item.category == 6 /* CRAFTING */)
                    return;

                item.container.style.display = (item.quantity <= -1 || item.quantity >= item.maxQuantity && item.maxQuantity > 0) ? 'none' : 'inline-block';
                if (item.maxQuantity && item.maxQuantity > 1) {
                    item.quantityElm.textContent = quantity.toString();
                }
            }
        });

        Utils.ifNotDefault(price, function () {
            if (item.priceElm) {
                item.price = price;
                item.priceElm.textContent = Utils.formatNumber(price);
            }
        });
    }
    Store.changeQuantity = changeQuantity;

    function add() {
    }

    function drawItem(item) {
        var itemContainer = document.createElement('div');
        itemContainer.classList.add('store-item');
        item.container = itemContainer;
        var header = document.createElement('div');
        header.classList.add('store-item-header');
        var headerText = document.createElement('div');
        headerText.classList.add('store-item-header-text');
        headerText.textContent = item.name;
        header.appendChild(headerText);

        var headerQuantityContainer = document.createElement('div');
        headerQuantityContainer.classList.add('store-item-header-quantity-container');

        var headerMaxQuantity = document.createElement('span');
        headerMaxQuantity.classList.add('store-item-header-quantity');
        var headerQuantity = document.createElement('span');
        headerQuantity.classList.add('store-item-header-quantity');
        var divider = document.createElement('span');
        divider.classList.add('store-item-header-quantity');
        divider.textContent = '/';

        if (item.maxQuantity > 1) {
            headerQuantityContainer.appendChild(headerQuantity);
            headerQuantityContainer.appendChild(divider);
            headerQuantityContainer.appendChild(headerMaxQuantity);
        }
        header.appendChild(headerQuantityContainer);

        item.maxQuantityElm = headerMaxQuantity;
        item.quantityElm = headerQuantity;

        item.nameElm = headerText;
        itemContainer.appendChild(header);

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
        coins.classList.add('Quarter-Coins');
        coins.style.display = 'inline-block';
        priceContainer.appendChild(coins);
        priceContainer.appendChild(price);

        var buttonContainer = document.createElement('div');
        var button = Utils.createButton('Buy', '');
        var id = item.id;
        if (item.category != 5 /* ITEMS */) {
            button.addEventListener('click', function () {
                Connection.purchaseItem(id);
            });
        } else {
            var quantityInput = document.createElement('INPUT');
            quantityInput.type = 'TEXT';
            quantityInput.style.width = '40px';
            quantityInput.style.height = '15px';
            buttonContainer.appendChild(quantityInput);
            button.addEventListener('click', function () {
                if (Utils.isNumber(quantityInput.value))
                    Connection.purchaseItem(id, +quantityInput.value);
            });
        }

        buttonContainer.appendChild(button);

        footer.appendChild(buttonContainer);
        footer.appendChild(priceContainer);

        itemContainer.appendChild(footer);

        return itemContainer;
    }
})(Store || (Store = {}));
