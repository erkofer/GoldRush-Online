///<reference path="connection.ts"/>
///<reference path="modal.ts"/>
module Tutorial {
    var stages = new Array<Stage>();
    
    export class Stage {
        highlights = new Array<Highlight>();
        title: string;
        modal: Function;

        deactivate() {
            this.toggle(false);
        }

        activate() {
            //this.toggle(true);
            this.modal();
        }

        private toggle(active: boolean) {
            for (var i = 0; i < this.highlights.length; i++) {
                var highlight = this.highlights[i];
                if (active) {
                    highlight.activate();
                } else {
                    highlight.deactivate();
                }
            }
        }
    }

    export class Highlight {
        element: HTMLElement;
        parentElement: HTMLElement;

        follow: boolean=false;
        left: boolean = false;
        bottom: boolean = false;

        deactivate() {
            Utils.cssSwap(this.element, 'open', 'closed');
        }

        activate() {
            Utils.cssSwap(this.element, 'closed', 'open');
            if (this.follow) {
                var width = $(this.parentElement).width();
                var height = $(this.parentElement).height();
                var pos = $(this.parentElement).position();
                this.element.style.position = 'absolute';
                if (!this.bottom)
                    this.element.style.top = (pos.top + height / 2) + 'px';
                else {
                    this.element.style.top = (pos.top + height) + 'px';
                }
                if(!this.left)
                    this.element.style.left = (pos.left + width) + 'px';
                else {
                    this.element.style.right = (pos.left - $(this.element).width())+'px';
                }
            }
        }
    }

    function miningModal() {
        var window = new modal.Window();
        window.title = "Mining Tutorial";
        var instructions = document.createElement('div');
        instructions.textContent = 'To get started click on the rock a couple of times to gather some resources.';
        window.addElement(instructions);
        var imageContainer = document.createElement('div');
        imageContainer.style.textAlign = 'center';
        var image = document.createElement('img');
        image.src = '/content/tutorial/mining.png';
        imageContainer.appendChild(image);
        window.addElement(imageContainer);
        var confirm = window.addAffirmativeOption('Got it!');
        confirm.addEventListener('click', function () {
            modal.close();
        });
        window.show();
    }

    function sellingModal() {
        var window = new modal.Window();
        window.title = "Selling Tutorial";
        var instructions = document.createElement('div');
        instructions.textContent = 'Sell some of your items until you have 1,000 coins.';
        window.addElement(instructions);
        var imageContainer = document.createElement('div');
        imageContainer.style.textAlign = 'center';
        var image = document.createElement('img');
        image.style.width = '395px';
        image.src = '/content/tutorial/selling.png';
        imageContainer.appendChild(image);
        window.addElement(imageContainer);
        var confirm = window.addAffirmativeOption('Got it!');
        confirm.addEventListener('click', function () {
            modal.close();
        });
        window.show();
    }

    function buyingModal() {
        var window = new modal.Window();
        window.title = "Automation Tutorial";
        var instructions = document.createElement('div');
        instructions.textContent = 'To start automating your mining process visit the store and hire a miner.';
        window.addElement(instructions);
        var imageContainer = document.createElement('div');
        imageContainer.style.textAlign = 'center';
        var image = document.createElement('img');
        image.style.width = '395px';
        image.src = '/content/tutorial/buying.png';
        imageContainer.appendChild(image);
        window.addElement(imageContainer);
        var confirm = window.addAffirmativeOption('Got it!');
        confirm.addEventListener('click', function () {
            modal.close();
        });
        window.show();
    }

    var miningStage = new Stage();
    miningStage.title = "MiningTutorial";
    miningStage.modal = miningModal;
    stages.push(miningStage);

    var sellingStage = new Stage();
    sellingStage.title = "SellingTutorial";
    sellingStage.modal = sellingModal;
    stages.push(sellingStage);

    var buyingStage = new Stage();
    buyingStage.title = "BuyingTutorial";
    buyingStage.modal = buyingModal;
    stages.push(buyingStage);

    export function activateStage(title: string) {
        for (var i = 0; i < stages.length; i++) {
            var stage = stages[i];
            if (stage.title == title) stage.activate();
        }
    }

    
} 