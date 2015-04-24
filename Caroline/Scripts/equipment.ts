
module Equipment {
    var gatherers = new Array<Gatherer>();
    var equipmentPane: HTMLElement;
    var gathererCategory: HTMLElement;

    class Gatherer {
        constructor() {

        }
        container: HTMLElement;

        enabled: boolean;
        toggleElm: HTMLElement;

        quantity: number;
        quantityElm: HTMLElement;

        maxQuantity: number;
        maxQuantityElm: HTMLElement;

        efficiency: number;
        efficiencyElm: HTMLElement;

        fuelConsumption: number;
        fuelConsumptionElm: HTMLElement;

        rarityBonus: number;
        rarityBonusElm: HTMLElement;
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


    function drawGathererCategory() {
        gathererCategory = document.createElement('div');
        gathererCategory.classList.add('store-category');
        var header = document.createElement('div');
        header.classList.add('store-category-header');
        header.textContent = 'Gatherers';
        gathererCategory.appendChild(header);
        equipmentPane.appendChild(gathererCategory);
    }

    function drawGatherer(id: number) {
        if (!gathererCategory) drawGathererCategory();

        var gatherer = gatherers[id];
        gatherer.container = document.createElement('div');
        gatherer.container.classList.add('equipment-gatherer-container');
        var header = document.createElement('div');
        header.classList.add('store-item-header');
        var headerText = document.createElement('div');
        headerText.classList.add('store-item-header-text');
        headerText.textContent = Objects.lookupName(id);
        header.appendChild(headerText);
        var toggle = Utils.createButton('Disable', '');
        toggle.addEventListener('click', function () {
            Connection.toggleGatherer(id, !gatherer.enabled);
        });
        var quantityContainer = document.createElement('div');
        quantityContainer.classList.add('store-item-header-quantity-container');

        gatherer.quantityElm = document.createElement('span');
        var dividerSpan = document.createElement('span');
        dividerSpan.textContent = '/';
        gatherer.maxQuantityElm = document.createElement('span');

        quantityContainer.appendChild(gatherer.quantityElm);
        quantityContainer.appendChild(dividerSpan);
        quantityContainer.appendChild(gatherer.maxQuantityElm);
        
        header.appendChild(quantityContainer);

        gatherer.container.appendChild(header);
        gatherer.efficiencyElm = document.createElement('div');
        gatherer.container.appendChild(gatherer.efficiencyElm);
        gatherer.fuelConsumptionElm = document.createElement('div');
        gatherer.container.appendChild(gatherer.fuelConsumptionElm);
        gatherer.rarityBonusElm = document.createElement('div');
        gatherer.container.appendChild(gatherer.rarityBonusElm);
        gatherer.container.appendChild(toggle);
        gatherer.toggleElm = <HTMLElement>toggle.firstChild;

        gathererCategory.appendChild(gatherer.container);
    }

    export function toggleGatherer(id: number, enabled: boolean) {
        var gatherer = gatherers[id];
        if (!gatherer) return;
        gatherer.enabled = enabled;
        gatherer.toggleElm.textContent = enabled ? 'Disable' : 'Enable';
    }

    export function changeQuantity(id: number, quantity: number) {
        Utils.ifNotDefault(quantity, function() {
            var gatherer = gatherers[id];
            if (!gatherer) return;

            gatherer.quantity = quantity;
            gatherer.container.style.display = (quantity > -1) ? 'inline-block' : 'none';
            gatherer.quantityElm.textContent = Utils.formatNumber(quantity);
        });
    }

    export function changeMaxQuantity(id: number, maxQuantity: number) {
        Utils.ifNotDefault(maxQuantity, function () {
            var gatherer = gatherers[id];
            if (!gatherer) return;

            gatherer.maxQuantity = maxQuantity;
            gatherer.maxQuantityElm.textContent = Utils.formatNumber(maxQuantity);
        });
    }

    export function changeFuelConsumption(id: number, fuelConsumed: number) {
        Utils.ifNotDefault(fuelConsumed, function () {
            var gatherer = gatherers[id];
            if (!gatherer) return;

            gatherer.fuelConsumption = fuelConsumed;
            gatherer.fuelConsumptionElm.textContent = 'Fuel consumed: '+Utils.formatNumber(fuelConsumed);
        });
    }

    export function changeEfficiency(id: number, efficiency: number) {
        Utils.ifNotDefault(efficiency, function () {
            var gatherer = gatherers[id];
            if (!gatherer) return;

            gatherer.efficiency = efficiency;
            gatherer.efficiencyElm.textContent = 'Resources/s: ' + Utils.formatNumber(efficiency);
        });
    }

    export function changeRarityBonus(id: number, rarityBonus: number) {
        Utils.ifNotDefault(rarityBonus, function () {
            var gatherer = gatherers[id];
            if (!gatherer) return;

            gatherer.rarityBonus = rarityBonus;
            gatherer.rarityBonusElm.textContent = 'Rarity multiplier: x' + Utils.formatNumber(rarityBonus);
        });
    }
}