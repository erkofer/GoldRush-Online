module Objects {
    var gameobjects = new Array<GameObject>();

    class GameObject {
        constructor() {
            this.quantity = 0;
        }

        name: string;
        quantity: number;
        lifeTimeTotal: number;
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

    export function setQuantity(id: number, quantity: number) {
        gameobjects[id].quantity = quantity;
    }

    export function getQuantity(id: number) {
        return gameobjects[id].quantity;
    }

    export function setLifeTimeTotal(id: number, quantity: number) {
        gameobjects[id].lifeTimeTotal = quantity;
    }

    export function getLifeTimeTotal(id: number) {
        return gameobjects[id].lifeTimeTotal;
    }
}