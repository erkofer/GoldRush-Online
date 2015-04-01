module Rock {
    var canvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById('rock');
    var context = canvas.getContext('2d');
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
            if(!mouseDown)
                drawRock(lastX - (rockGrowth / 2), lastY - (rockGrowth / 2), rockSize + rockGrowth, rockSize + rockGrowth);
            else
                drawRock(lastX + (rockGrowth / 2), lastY + (rockGrowth / 2), rockSize - rockGrowth, rockSize - rockGrowth);

            rockIsBig = true;
        }
        else if (rockIsBig) {
            drawRock(lastX, lastY, rockSize, rockSize);
            rockIsBig = false;
        }
    }

    export function moveRock(x:number,y:number) {
        lastX = x;
        lastY = y;
        if (stoneLoaded)
            drawRock(x, y, rockSize, rockSize);
        else 
            setTimeout(function () {moveRock(x, y); }, 10);
    }

    function clearCanvas() {
        context.clearRect(0, 0, 250, 250);
    }

    function drawBackground() {
        context.drawImage(rockImage, 0, 0);
    }

    function drawRock(x: number, y: number, xScale: number, yScale: number) {
        clearCanvas();
        drawBackground();
        context.drawImage(stoneImage, x, y, xScale, yScale);
        //context.drawImage(stoneImage, x, y);
    
    }

    initialize();
}