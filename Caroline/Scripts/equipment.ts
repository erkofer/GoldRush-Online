module Equipment {
    var gatherers = new Array<Gatherer>();

    class Gatherer {
        constructor() {
            
        }

        quantity: number;
        maxQuantity: number;
        enabled: number;
        fuelConsumption: number;
    }

    function init() {
        
    }
    init();

    export function registerGatherer(id: number) {
        if (gatherers[id]) return;

        var gatherer = new Gatherer();
        gatherers[id] = gatherer;
    }

    export function toggleGatherer(id: number, enabled: boolean) {
        
    }
} 