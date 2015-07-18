module Objects {
    var gameobjects = new Array<GameObject>();
    import Long = dcodeIO.Long;
    class GameObject {
        constructor() {
            this.quantity = Long.fromNumber(0);
        }

        name: string;
        quantity: Long;
        maxQuantity: number;
        lifeTimeTotal: number;
        tooltip: string;
    }

    export function register(id: number, name: string) {
        if (!gameobjects[id]) {
            var gameobject = new GameObject();
            gameobject.name = name;

            gameobjects[id] = gameobject;
        }
    }

    export function lookupName(id: number) {
        return gameobjects[id].name;
    }

    export function setQuantity(id: number, quantity: Long) {
        gameobjects[id].quantity = quantity;
    }

    export function getQuantity(id: number): Long {
        var gameobject = gameobjects[id];
        if (gameobject) return gameobject.quantity;

        return Long.fromNumber(0);
    }

    export function setMaxQuantity(id: number, maxQuantity: number) {
        gameobjects[id].maxQuantity = maxQuantity;
    }

    export function getMaxQuantity(id: number) {
        return gameobjects[id].maxQuantity;
    }

    export function setLifeTimeTotal(id: number, quantity: number) {
        gameobjects[id].lifeTimeTotal = quantity;
    }

    export function getLifeTimeTotal(id: number) {
        return gameobjects[id].lifeTimeTotal;
    }

    export function setTooltip(id: number, tooltip:string) {
        gameobjects[id].tooltip = tooltip;
    }
    export function getTooltip(id: number) {
        return gameobjects[id].tooltip;
    }
}