
﻿module Ajax {
    var loaders = new Array<Loader>();



    interface ILeaderboardEntry {
        Score: number;
        Rank: number;
        UserId: string;
    }

    export class LeaderboardAjaxService {
        constructor() {

        }

        private failed(request) {
            this.resultsElement.textContent = 'Loading failed...';
        }

        private succeeded(request) {

            while (this.resultsElement.firstChild)
                this.resultsElement.removeChild(this.resultsElement.firstChild);

            var leaderboardTable: HTMLTableElement = document.createElement('table');
            var thead: HTMLTableElement = <HTMLTableElement>leaderboardTable.createTHead();
            var subheader = <HTMLTableRowElement>thead.insertRow(0);
            subheader.classList.add('table-subheader');

            var score = subheader.insertCell(0);
            score.textContent = 'Score';
            score.style.width = '65%';
            var player = subheader.insertCell(0);
            player.textContent = 'Name';
            player.style.width = '25%';
            var rank = subheader.insertCell(0);
            rank.textContent = 'Rank';
            rank.style.width = '10%';

            var tbody: HTMLTableElement = <HTMLTableElement>leaderboardTable.createTBody();


            for (var i = 0; i < request.length; i++) {
                var leaderboardEntry: ILeaderboardEntry = request[i];

                var row = <HTMLTableRowElement>tbody.insertRow(tbody.rows.length);
                row.classList.add('table-row');

                var rScore = row.insertCell(0);
                rScore.textContent = Utils.formatNumber(leaderboardEntry.Score);
                rScore.style.width = '65%';
                rScore.addEventListener('click', function (event) {
                    var score;
                    var cell = <HTMLElement>event.target;

                    if (cell.dataset) {
                        score = cell.dataset['tooltip'];
                    } else {
                        score = cell.getAttribute('data-tooltip');
                    }

                    if (cell.textContent.indexOf(',') > 0) { // this number is detailed and filled with commas.
                        cell.textContent = Utils.formatNumber(score);
                    } else {
                        cell.textContent = Utils.formatNumber(score, true);
                    }
                });

                if (rScore.dataset) {
                    rScore.dataset['tooltip'] = leaderboardEntry.Score;
                } else {
                    rScore.setAttribute('data-tooltip', leaderboardEntry.Score.toString());
                }

                var rPlayer = row.insertCell(0);
                rPlayer.textContent = leaderboardEntry.UserId;
                player.style.width = '25%';
                var rRank = row.insertCell(0);
                rRank.textContent = Utils.formatNumber(leaderboardEntry.Rank);
                rRank.style.width = '10%';
            }
            this.resultsElement.appendChild(leaderboardTable);
        }

        resultsElement: HTMLElement;

        sendRequest(lowerbound: number, upperbound: number) {
            var self = this;

            var request = $.ajax({
                asyn: true,
                type: 'POST',
                url: '/Api/Stats/LeaderBoard/',
                data: $.param({ Lower: lowerbound, Upper: upperbound }),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                success: function (request) {
                    request = JSON.parse(request);
                    self.succeeded(request);
                },
                failure: function (request) {
                    self.failed(request);
                }
            });
        }
    }

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