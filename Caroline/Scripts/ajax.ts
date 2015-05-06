module Ajax {
    var loaders = new Array<Loader>();

    class Loader {
        constructor() {
            this.container = document.createElement('div');
            this.container.classList.add('loader');
            this.canvasElement = document.createElement('canvas');
            this.canvasElement.width = this.squareSize;
            this.canvasElement.height = this.squareSize;
            this.container.appendChild(this.canvasElement);
            this.context = this.canvasElement.getContext('2d');

        }
        container: HTMLElement;
        notDisplayed: number = 20;
        notches: number = 40;
        space: number = 0.02;
        canvasElement: HTMLCanvasElement;
        spawned: boolean = false;
        context: CanvasRenderingContext2D;
        spinPos: number = 0;
        spinSpeed: number = 30;
        squareSize: number = 50;
        timeSinceLastTick: number = Date.now();
        thickness: number = 3;
        dead: boolean = false;


        update() {
            var time = Date.now();

            if (this.canvasElement.parentElement) this.spawned = true;

            var timePassed = time - this.timeSinceLastTick;
            timePassed /= 1000;
            this.spinPos += this.spinSpeed * timePassed;
            this.timeSinceLastTick = time;

            if (this.spawned && !this.canvasElement.parentElement) this.dead = true;

            this.draw();
        }

        draw() {
            var x = this.squareSize / 2;
            var y = x;
            var adjustedSpinPosStart = this.spinPos % 2;
            var adjustedSpinPosEnd = (this.spinPos + 1) % 2;

            var radius = (this.squareSize / 2) - 5; // ensure there is a buffer area around the outside edge.
            var lengthEach = (2 - (this.space * this.notches)) / this.notches;
            var selected = this.spinPos % this.notches;
            var shown = selected - this.notDisplayed;

            this.context.clearRect(0, 0, this.canvasElement.width, this.canvasElement.height);
            for (var i = 0; i < this.notches; i++) {
                var start = 0 + (i * lengthEach) + (this.space * i);
                var end = start + lengthEach;
                var inverse = i - this.notches;

                if (i <= selected && i >= shown) {
                    this.context.beginPath();
                    this.context.arc(x, y, radius, start * Math.PI, end * Math.PI);
                    this.context.lineWidth = this.thickness;
                    this.context.strokeStyle = 'rgba(113,142,164,'+(1-(selected-i)/(this.notches-this.notDisplayed))+')';
                    this.context.stroke();
                } else if (inverse <= selected && inverse >= shown) {
                    this.context.beginPath();
                    this.context.arc(x, y, radius, start * Math.PI, end * Math.PI);
                    this.context.lineWidth = this.thickness;
                    this.context.strokeStyle = 'rgba(113,142,164,' + (1 - (selected - inverse) / (this.notches - this.notDisplayed)) + ')';
                    this.context.stroke();
                }
            }
        }
       // this.context.strokeStyle = 'rgba()';
    }

    export function createLoader() {
        var loader = new Loader();
        loaders.push(loader);
        return loader.container;
    }

    function update() {
        for (var i = 0; i < loaders.length; i++) {
            var loader = loaders[i];
            if (loader.dead) loaders.splice(i, 1);
            else loader.update();
        }
        setTimeout(update, 10);
    }
    update();
} 