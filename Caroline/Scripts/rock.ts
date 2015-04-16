module Rock {
    var canvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById('rock');
    var particleCanvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById('particles');
    var context = canvas.getContext('2d');
    var particleContext = particleCanvas.getContext('2d');
    var relativeRockURL: string = '/Content/Rock.png';
    var relativeStoneURL: string = '/Content/Stone.png';
    var rockImage = new Image();
    var stoneImage = new Image(16,16);
    var stoneLoaded = false;
    var lastX = 0;
    var lastY = 0;
    var rockSize = 64;
    var rockGrowth = 4;
    var rockIsBig = false;
    var mouseDown = false;
    export var particles = new Array<Particle>();

    export class Particle {
        constructor(x: number, y: number) {
            this.x = x;
            this.y = y;
            this.verticalVelocity = Utils.getRandomInt(-100,-155);
            this.horizonalVelocity = Utils.getRandomInt(-50, 50);
            this.width = Utils.getRandomInt(3, 6);
            this.height = Utils.getRandomInt(3, 6);
            //this.rotation = Utils.getRandomInt(0, 180);
            //this.rotationalVelocity = Utils.getRandomInt(-20, 20);
            
            this.lastTick = Date.now();
        }

        x: number;
        y: number;
        width: number = 5;
        height: number =5;
        rotation: number;
        rotationalVelocity:number;
        verticalVelocity: number;
        horizonalVelocity: number;
        lastTick: number;
        dispose: boolean=false;

        update() {
            var timeSinceLastTick = Date.now() - this.lastTick;
            this.lastTick = Date.now();
            timeSinceLastTick /= 1000;

            //this.rotation += (this.rotationalVelocity * timeSinceLastTick);
            this.y += (this.verticalVelocity * timeSinceLastTick);
            this.x += (this.horizonalVelocity * timeSinceLastTick);
            this.verticalVelocity += (200 * timeSinceLastTick);
            if (this.y > 270) {
                this.dispose = true;
            }
        }
    }

    function initialize() {
        rockImage.onload = function () {
            drawBackground();
            console.log('rock loaded');
        };
        rockImage.src = relativeRockURL;

        stoneImage.onload = function () {
            stoneLoaded = true;
        };
        stoneImage.src = relativeStoneURL;

        canvas.addEventListener('mousemove', function (e) {
            var mousePos = getMousePos(canvas, e);
            isOverRock(mousePos.x, mousePos.y);
            //console.log('x: ' + mousePos.x + ' y: ' + mousePos.y);
        }, false);
        canvas.addEventListener('mousedown', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = true;
            isOverRock(mousePos.x, mousePos.y);
            Connection.mine(mousePos.x, mousePos.y);
        }, false);
        canvas.addEventListener('mouseup', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = false;
            isOverRock(mousePos.x, mousePos.y);
        }, false);
        canvas.addEventListener('mouseleave', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = false;
            isOverRock(mousePos.x, mousePos.y);
        }, false);
    }

    function getMousePos(canvas, evt) {
        var rect = canvas.getBoundingClientRect();
        return {
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        };
    }

    function isOverRock(x: number, y: number) {
        if (x > lastX && x < (lastX + rockSize) && y > lastY && y < (lastY + rockSize)) {
            if (!mouseDown)
                drawRock(lastX - (rockGrowth / 2), lastY - (rockGrowth / 2), rockSize + rockGrowth, rockSize + rockGrowth);
            else {
                drawRock(lastX + (rockGrowth / 2), lastY + (rockGrowth / 2), rockSize - rockGrowth, rockSize - rockGrowth);
                addParticles(x, y);
            }

            rockIsBig = true;
        }
        else if (rockIsBig) {
            drawRock(lastX, lastY, rockSize, rockSize);
            rockIsBig = false;
        }
    }

    export function moveRock(x: number, y: number) {
        if (x != lastX && y != lastY) {
        lastX = x;
        lastY = y;
        if (stoneLoaded)
            drawRock(x, y, rockSize, rockSize);
        else 
                setTimeout(function () { moveRock(x, y); }, 10);
        }
    }

    function clearCanvas() {
        context.clearRect(0, 0, 250, 250);
    }

    function drawBackground() {
        context.drawImage(rockImage, 0, 0);
    }

    function addParticles(x: number, y: number) {
        var rand = Utils.getRandomInt(1, 3);
        for (var i = 0; i < rand; i++) {
            var xOffset = Utils.getRandomInt(-5, 5);
            var yOffset = Utils.getRandomInt(-2, 2);
            particles.push(new Particle(x+xOffset, y+yOffset));
        }
    }

    function drawParticle(particle: Particle) {
        particleContext.beginPath();
        particleContext.fillStyle = 'gray';
        particleContext.rect(particle.x, particle.y, particle.width, particle.height);
        particleContext.fill();
        particleContext.stroke();
    }

    function updateParticles() {
        particleContext.clearRect(0, 0, 250, 250);

        for (var i = 0; i < particles.length; i++) {
            var particle: Particle=particles[i];
            particle.update();
            drawParticle(particle);

            if (particle.dispose) {
                particles.splice(i, 1);
            }
        }   
    }
    setInterval(updateParticles, 10);


    function drawRock(x: number, y: number, xScale: number, yScale: number) {
        clearCanvas();
        drawBackground();
        context.drawImage(stoneImage, x, y, xScale, yScale);
        //context.drawImage(stoneImage, x, y);
    
    }

    initialize();
}