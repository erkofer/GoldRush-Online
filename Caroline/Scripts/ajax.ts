module Ajax {
    class Loader {
        constructor() {
            this.canvasElement = document.createElement('canvas');
            this.canvasElement.width = this.squareSize;
            this.canvasElement.height = this.squareSize;
            this.context = this.canvasElement.getContext('2d');
        }

        canvasElement: HTMLCanvasElement;
        context: CanvasRenderingContext2D;
        spinSpeed: number = 5;
        squareSize: number = 50;
        timeSinceLastTick: number = Date.now();
        thickness: number = 3;

        update() {
            var time = Date.now();
            var timePassed = time - this.timeSinceLastTick;
            timePassed /= 1000;
            this.timeSinceLastTick = time;
            this.draw(timePassed);

            setTimeout(this.update, 10);
        }

        draw(timePassed: number) {
            var x = this.squareSize / 2;
            var y = x;
            var radius = (this.squareSize / 2) - 3; // ensure there is a buffer area around the outside edge.
            this.context.clearRect(0, 0, this.canvasElement.width, this.canvasElement.height);
            this.context.arc(x, y, radius, 0, 10);

        }
    }

    export function createLoader() {
        var loader = new Loader();
        loader.update();
        return loader.canvasElement;
    }
} 