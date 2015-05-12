module Market {
    var marketPane;

    export class Order {
        constructor() {
            
        }

        buying: boolean;

        header: HTMLElement;
    }

    function init() {
        marketPane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(marketPane);
        Tabs.registerGameTab(marketPane, 'Market');
    }

    export function drawTradePane(order: Order) {
        if (!marketPane) init();

        var panel = document.createElement('div');
        panel.classList.add('market-order');
        panel.classList.add('closed');

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
        controls.appendChild(buy);

        var sell = document.createElement('div');
        sell.classList.add('market-control');
        sell.classList.add('sell');
        sell.textContent = 'Sell';
        controls.appendChild(sell);

        panel.appendChild(controls);

        var display = document.createElement('div');
        display.classList.add('market-order-display');
        panel.appendChild(display);

        marketPane.appendChild(panel);
    }
} 