module modal {
    var timeOpened = 0;
    var modalPane;
    export var activeWindow;
    export var intervalIdentifier;

    export function hide() {
        if (activeWindow) {
            activeWindow.hide();
        }
        activeWindow = null;
    }

    export function close() {
        if (activeWindow) {
            var a = activeWindow;
            hide();
            a.container.parentNode.removeChild(a.container);
        }
    }

    export class Window {
        container: HTMLElement;
        private _title: string;
        private titleEl: HTMLElement;
        private bodyEl: HTMLElement;
        options: HTMLElement;

        get title(): string {
            return this._title;
        }
        set title(s: string) {
            this._title = s;
            this.titleEl.textContent = this._title;
        }

        constructor() {
            this.container = document.createElement("div");
            this.container.addEventListener("click", function (e) {
                e.stopPropagation();
            }, false);
            this.container.classList.add("modal-window");
            if (!modalPane) {
                var pane = document.createElement("div");
                modalPane = pane;
                pane.classList.add("modal-wrapper");
                pane.addEventListener("click", function (e) {
                    e.stopPropagation();
                    if ((Date.now() - timeOpened) > 3000)
                        modal.close();
                }, false);
                document.body.appendChild(pane);
            }
            modalPane.appendChild(this.container);

            this.titleEl = document.createElement("div");
            this.titleEl.classList.add("modal-header");
            this.bodyEl = document.createElement("div");

            this.container.appendChild(this.titleEl);
            this.container.appendChild(this.bodyEl);
        }

        addElement(el: HTMLElement) {
            this.bodyEl.appendChild(el);
        }

        // intended for the bottom bar of controls.
        addOption(opt: string) {
            if (!this.options) {
                this.options = document.createElement("div");
                this.options.classList.add("modal-options");
                this.container.appendChild(this.options);
            }
            var optionContainer = document.createElement("span");
            optionContainer.classList.add("modal-option");

            var option = document.createElement("span");
            option.textContent = opt;

            optionContainer.appendChild(option);
            this.options.appendChild(optionContainer);
            return optionContainer;
        }

        addAffirmativeOption(opt: string) {
            var option = this.addOption(opt);
            option.classList.add("affirmative");
            return option;
        }

        addNegativeOption(opt: string) {
            var option = this.addOption(opt);
            option.classList.add("negative");
            return option;
        }

        show() {
            if (!this.container.classList.contains("opened"))
                this.container.classList.add("opened");
            if (!modalPane.classList.contains("opened"))
                modalPane.classList.add("opened");
            activeWindow = this;
            updatePosition();
            intervalIdentifier = setInterval(updatePosition, 100);
            timeOpened = Date.now();
        }

        hide() {
            if (this.container.classList.contains("opened"))
                this.container.classList.remove("opened");
            if (modalPane.classList.contains("opened"))
                modalPane.classList.remove("opened");
        }
    }

    function updatePosition() {
        if (!modal.activeWindow) {
            clearInterval(modal.intervalIdentifier);
        } else {
            var containerDimensions = modal.activeWindow.container.getBoundingClientRect();
            modal.activeWindow.container.style.left = (window.innerWidth / 2) - ((containerDimensions.right - containerDimensions.left) / 2) + "px";
            modal.activeWindow.container.style.top = (window.innerHeight / 2) - ((containerDimensions.bottom - containerDimensions.top) / 2) + "px";
        }
    }
}  