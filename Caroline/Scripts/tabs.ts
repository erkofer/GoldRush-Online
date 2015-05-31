///<reference path="connection.ts"/>

module Tabs {
    var lowestTabContainerId: number = 0;
    var buttonContainer = document.getElementById('tabContainer');
    var tabContainer = document.getElementById("paneContainer");
    export var bottomPadding = 170;
    var tabContainers = new Array<TabContainer>();
    var selectedTab:HTMLElement; 
   
    class TabContainer {
        tabs: Array<Tab>;
        container: HTMLElement;
        lowestId: number;
        id: number;

        constructor(container: HTMLElement) {
            this.container = container;
            this.tabs = new Array<Tab>();
            this.lowestId = 0;
            this.id = lowestTabContainerId++;
            tabContainers.push(this);
        }

        newTab(pane: HTMLElement) {
            var tab = new Tab();
            tab.pane = pane;
            var button = document.createElement('DIV');
            button.classList.add('tab-button');
            this.container.appendChild(button);
            tab.button = button;

            if (this.lowestId == 0) {
                tab.activate();
            }
            else {
                tab.deactivate();
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
        }

        activate(id: number) {
            for (var i = 0; i < this.tabs.length; i++) {
                this.tabs[i].deactivate();
            }
            this.tabs[id].activate();
        }

        css(id: number, className: string) {
            this.tabs[id].button.classList.add(className);
        }

        connectionLink(id: number, connectionTab: Connection.Tabs) {
            this.tabs[id].connectionTab = connectionTab;
        }
    }

    var gameTabs = new TabContainer(document.getElementById('tabContainer'));

    export function registerGameTab(pane: HTMLElement, connectionTab: Connection.Tabs, css?: string) {
        var id = gameTabs.newTab(pane);
        gameTabs.connectionLink(id, connectionTab);

        if (css)
            gameTabs.css(id, css);
    }

    export function updateGameTabs() {
        if (selectedTab) {
            var height = selectedTab.scrollHeight;
            var extra = bottomPadding + buttonContainer.clientHeight;
            if (height > window.innerHeight - extra) {
                height = window.innerHeight - extra;
            }
            tabContainer.style.minHeight = height + 'px';
            tabContainer.style.maxHeight = height + 'px';
            tabContainer.style.overflowY = height >= window.innerHeight - extra ? 'scroll' : 'hidden';
        }
    }

    Utils.addEvent(window, 'resize', Tabs.updateGameTabs);
    setInterval(updateGameTabs, 20);

    export function activateTab(containerId: number, tabId: number) {
        tabContainers[containerId].activate(tabId);
        updateGameTabs();
    }

    class Tab {
        pane: HTMLElement;
        button: HTMLElement;
        connectionTab: Connection.Tabs;

        deactivate() {
            Utils.cssSwap(this.button, 'active', 'inactive');
            this.pane.style.display = 'none';
            this.pane.style.overflow = 'hidden';
        }

        activate() {
            Utils.cssSwap(this.button, 'inactive', 'active');
            this.pane.style.display = 'block';
            this.pane.style.overflow = 'visible';
            selectedTab = this.pane;
            Connection.changeSelectedTab(this.connectionTab);
        }
    }
}