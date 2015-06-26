module Tutorial {
    var stages = new Array<Stage>();
    
    export class Stage {
        highlights = new Array<Highlight>();
        title: string;

        deactivate() {
            this.toggle(false);
        }

        activate() {
            this.toggle(true);
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
                this.element.style.top = (pos.top+height) + 'px';
                this.element.style.left = (pos.left + width) + 'px';
            }
        }
    }

    var miningStage = new Stage();
    miningStage.title = "MiningTutorial";
    stages.push(miningStage);

    var miningStageHighlight = new Highlight();
    miningStageHighlight.follow = true;
    var miningStageElement = document.createElement('div');
    miningStageElement.textContent = 'Click the rock a couple of times to gather some resources.';
    miningStageElement.classList.add('tutorial');
    miningStageElement.classList.add('closed');
    miningStageElement.style.position = 'absolute';
    miningStageElement.style.top = '0';
    miningStageElement.style.left = '0';
    miningStageHighlight.parentElement = $(".rock-container")[0];
    document.body.appendChild(miningStageElement);
    miningStageHighlight.element = miningStageElement;
    miningStage.highlights.push(miningStageHighlight);

    export function activateStage(title: string) {
        for (var i = 0; i < stages.length; i++) {
            var stage = stages[i];
            if (stage.title != title) stage.deactivate();
            else stage.activate();
        }
    }
} 