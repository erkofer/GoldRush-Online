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
var Inventory;
(function (Inventory) {
    Inventory.items = new Array();
    var inventoryPane;
    var inventory;
    var selectedItemPane;
    var selectedItemImage;
    var selectedItem;

    var configTableBody;
    var configTableContainer;
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

        if (!inventoryPane)
            draw();

        inventory.appendChild(drawItem(item));
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
            console.log('why..');
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

    function toggleConfig() {
        if (configTableContainer.classList.contains('closed')) {
            configTableContainer.classList.remove('closed');
        } else
            configTableContainer.classList.add('closed');
    }
    Inventory.toggleConfig = toggleConfig;

    function drawItem(item) {
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
        itemCurrency.classList.add('Third-Coins');
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
            for (var curRow = 0; curRow < rows; curRow++) {
                var inspectedRow = configTableBody.rows[i];
                var cells = inspectedRow.cells.length;
                for (var curCell = 0; curCell < cells; curCell++) {
                }
            }

            var nameAndImage = document.createElement('DIV');
            nameAndImage.classList.add('item-text');
            var nameSpan = document.createElement('SPAN');
            nameSpan.style.verticalAlign = 'top';
            var image = document.createElement('DIV');
            image.classList.add('Third-' + Utils.cssifyName(item.name));
            image.style.display = 'inline-block';
            nameSpan.textContent = item.name;
            nameAndImage.appendChild(image);
            nameAndImage.appendChild(nameSpan);
            selectedItemCell.appendChild(nameAndImage);
            var configChecker = document.createElement('INPUT');
            configChecker.type = 'CHECKBOX';
            selectedConfigCell.appendChild(configChecker);
        }

        return itemElement;
    }

    function addItem(id, name, worth, category) {
        if (!Inventory.items[id])
            add(new Item(id, name, worth, category));
    }
    Inventory.addItem = addItem;

    function changeQuantity(id, quantity) {
        Inventory.items[id].quantityElm.textContent = Utils.formatNumber(quantity);
        Inventory.items[id].quantity = quantity;
        Inventory.items[id].container.style.display = quantity == 0 ? 'none' : 'inline-block';
        limitTextQuantity();
    }
    Inventory.changeQuantity = changeQuantity;
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

        if (msg.Message != null) {
            Chat.receiveGlobalMessage(msg.Message.Sender, msg.Message.Text, msg.Message.Time, msg.Message.Permissions);
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
        conInterval = setInterval(function () {
            var encoded = actions.encode64();

            send(encoded);

            actions = new Komodo.ClientActions();
        }, 1000);
    }

    function loadSchema(schema) {
        for (var i = 0; i < schema.Items.length; i++) {
            Inventory.addItem(schema.Items[i].Id, schema.Items[i].Name, schema.Items[i].Worth, schema.Items[i].Category);
            Statistics.addItem(schema.Items[i].Id, schema.Items[i].Name);
        }

        for (var i = 0; i < schema.StoreItems.length; i++) {
            var item = schema.StoreItems[i];
            Store.addItem(item.Id, item.Category, item.Price, item.Factor, item.Name, item.MaxQuantity);
        }
    }

    function updateStats(items) {
        for (var i = 0; i < items.length; i++)
            Statistics.changeStats(items[i].Id, items[i].PrestigeQuantity, items[i].LifeTimeQuantity);
    }

    function updateInventory(items) {
        for (var i = 0; i < items.length; i++)
            Inventory.changeQuantity(items[i].Id, items[i].Quantity);
    }

    function updateStore(items) {
        for (var i = 0; i < items.length; i++)
            Store.changeQuantity(items[i].Id, items[i].Quantity);
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

        item.prestigeRow.textContent = prestige ? Utils.formatNumber(prestige) : '0';
        item.alltimeRow.textContent = lifetime ? Utils.formatNumber(lifetime) : '0';
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

            var categoryContainer = categories[Category[category]];
            if (categoryContainer == null) {
                categoryContainer = categories["MINING"];
            }
            categoryContainer.appendChild(drawItem(item));
            items[id] = item;
        }
    }
    Store.addItem = addItem;

    function changeQuantity(id, quantity) {
        var item = items[id];
        item.quantity = quantity;
        item.container.style.display = (item.quantity == -1 || item.quantity >= item.maxQuantity && item.maxQuantity > 0) ? 'none' : 'inline-block';
        if (item.maxQuantity && item.maxQuantity > 1)
            item.nameElm.textContent = item.name + ' (' + ((item.quantity) ? item.quantity : 0) + '/' + item.maxQuantity + ')';
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
        if (item.maxQuantity <= 1) {
            header.textContent = item.name;
        } else {
            header.textContent = item.name + ' (' + item.quantity + '/' + item.maxQuantity + ')';
        }
        item.nameElm = header;
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
        coins.classList.add('Third-Coins');
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
