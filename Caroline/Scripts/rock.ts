///<reference path="tutorial.ts"/>

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

    var notTouched = true;
    var growing = true;
    var currentNotifier = 0;
    var growRate = 8;
    var notifierGrowth = 4;
    var gsLastTick = Date.now();

    export var particles = new Array<Particle>();

    export class Particle {
        constructor(x: number, y: number) {
            this.x = x;
            this.y = y;
            this.verticalVelocity = Utils.getRandomInt(-100,-155);
            this.horizonalVelocity = Utils.getRandomInt(-50, 50);
            this.width = Utils.getRandomInt(3, 6);
            this.height = Utils.getRandomInt(3, 6);
            this.rotation = Utils.getRandomInt(0, 180);
            this.rotationalVelocity = Utils.getRandomInt(-75, 75);
            
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

            this.rotation += (this.rotationalVelocity * timeSinceLastTick);
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

    function growAndShrink() {
        if (!stoneLoaded) {
            setTimeout(growAndShrink, 10);
            return;
        }

        var time = Date.now();
        var timePassed = time - gsLastTick;
        timePassed /= 1000;
        gsLastTick = time;

        if (growing) { currentNotifier += growRate * timePassed; }
        else {currentNotifier -= growRate * timePassed;}

        if (currentNotifier > notifierGrowth || currentNotifier < -notifierGrowth) {
            growing = !growing;
        }

        drawRock(lastX, lastY, rockSize + currentNotifier, rockSize + currentNotifier);

        if (notTouched)
            setTimeout(growAndShrink, 10);
    }
    growAndShrink();

    function getMousePos(canvas, evt) {
        var rect = canvas.getBoundingClientRect();
        return {
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        };
    }

    var released = true;

    function isOverRock(x: number, y: number) {
        if (x > lastX && x < (lastX + rockSize) && y > lastY && y < (lastY + rockSize)) {
            if (!mouseDown) { // mouse is hovering but not clicking
                drawRock(lastX - (rockGrowth / 2), lastY - (rockGrowth / 2), rockSize + rockGrowth, rockSize + rockGrowth);
                released = true;
                notTouched = false;
            } else { // mouse is clicking
                drawRock(lastX + (rockGrowth / 2), lastY + (rockGrowth / 2), rockSize - rockGrowth, rockSize - rockGrowth);
                if (released)
                    addParticles(x, y);
                released = false;
                notTouched = false;
            }
            rockIsBig = true;
        } else if (rockIsBig) {
            drawRock(lastX, lastY, rockSize, rockSize);
            rockIsBig = false;
            released = true;
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

    var particleCache = document.createElement('canvas');
    var cacheCtx = particleCache.getContext('2d');

    function drawParticle(particle: Particle) {
        particleCache.width = particle.width;
        particleCache.height = particle.height;
        cacheCtx.rect(0, 0, particle.width, particle.height);
        cacheCtx.fillStyle = 'gray';
        cacheCtx.fill();
        cacheCtx.stroke();


        particleContext.beginPath();

        drawImageRot(particleContext,particleCache, particle.x, particle.y, particle.width, particle.height, particle.rotation);
       
    }

    function drawImageRot(ctx, img, x, y, width, height, deg) {

        //Convert degrees to radian 
        var rad = deg * Math.PI / 180;

        //Set the origin to the center of the image
        ctx.translate(x + width / 2, y + height / 2);

        //Rotate the canvas around the origin
        ctx.rotate(rad);

        //draw the image    
        ctx.drawImage(img, width / 2 * (-1), height / 2 * (-1), width, height);

        //reset the canvas  
        ctx.rotate(rad * (-1));
        ctx.translate((x + width / 2) * (-1), (y + height / 2) * (-1));
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