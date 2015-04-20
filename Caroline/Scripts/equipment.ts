<<<<<<< HEAD
﻿module Equipment {
    var gatherers = new Array<Gatherer>();
    var equipmentPane: HTMLElement;
   
    class Gatherer {
        constructor() {
            
        }

        toggleElm: HTMLElement;

        quantity: number;
        maxQuantity: number;
        enabled: boolean;
        fuelConsumption: number;
    }

    export function draw() {
        if (equipmentPane) return;

        equipmentPane = document.createElement('div');
        document.getElementById('paneContainer').appendChild(equipmentPane);
        Tabs.registerGameTab(equipmentPane, 'Equipment');
    }

    export function registerGatherer(id: number) {
        if (gatherers[id]) return;

        var gatherer = new Gatherer();
        gatherers[id] = gatherer;
        drawGatherer(id);
    }

    function drawGatherer(id: number) {
        var gatherer = gatherers[id];
        var container = document.createElement('div');
        container.classList.add('equipment-gatherer-container');
        var header = document.createElement('div');
        header.textContent = Objects.lookupName(id);
        var toggle = Utils.createButton('Disable', '');
        toggle.addEventListener('click', function() {
            Connection.toggleGatherer(id, !gatherer.enabled);
        });
        container.appendChild(header);
        container.appendChild(toggle);
        gatherer.toggleElm = <HTMLElement>toggle.firstChild;

        equipmentPane.appendChild(container);
    }

    export function toggleGatherer(id: number, enabled: boolean) {
        var gatherer = gatherers[id];
        if (!gatherer) return;
        gatherer.enabled = enabled;
        gatherer.toggleElm.textContent = enabled ? 'Disable' : 'Enable';
    }
=======
﻿module Equipment {
    var gatherers = new Array<Gatherer>();
    var equipmentPane: HTMLElement;
   
    class Gatherer {
        constructor() {
            
        }

        toggleElm: HTMLElement;

        quantity: number;
        maxQuantity: number;
        enabled: boolean;
        fuelConsumption: number;
    }

    export function draw() {
        if (equipmentPane) return;

        equipmentPane = document.createElement('div');
        document.getElementById('paneContainer').appendChild(equipmentPane);
        Tabs.registerGameTab(equipmentPane, 'Equipment');
    }

    export function registerGatherer(id: number) {
        if (gatherers[id]) return;

        var gatherer = new Gatherer();
        gatherers[id] = gatherer;
        drawGatherer(id);
    }

    function drawGatherer(id: number) {
        var gatherer = gatherers[id];
        var container = document.createElement('div');
        container.classList.add('equipment-gatherer-container');
        var header = document.createElement('div');
        header.textContent = Objects.lookupName(id);
        var toggle = Utils.createButton('Disable', '');
        toggle.addEventListener('click', function() {
            Connection.toggleGatherer(id, !gatherer.enabled);
        });
        container.appendChild(header);
        container.appendChild(toggle);
        gatherer.toggleElm = <HTMLElement>toggle.firstChild;

        equipmentPane.appendChild(container);
    }

    export function toggleGatherer(id: number, enabled: boolean) {
        var gatherer = gatherers[id];
        if (!gatherer) return;
        console.log(enabled);
        gatherer.enabled = enabled;
        gatherer.toggleElm.textContent = enabled ? 'Disable' : 'Enable';
    }
>>>>>>> 3e6ec0c0bafb03877038ad7258953bc6fd752454
} 