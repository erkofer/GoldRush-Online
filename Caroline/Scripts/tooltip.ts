module tooltip {
    var registeredTooltips: number = 0;
    var tooltips = new Array<Tooltip>();
    var activeTooltipId: number;
    var activeTooltip: HTMLElement;

    var intervalId;
    var appearDelay: number = 0.25;
    var currentDelay: number = 0;
    var x: number;
    var y: number;

    var focusedElement: HTMLElement;

    class Tooltip {
        html: HTMLElement;
        constructor() {
            this.html = document.createElement("div");
            this.html.classList.add('tooltip-wrapper');
        }

        get header() {
            return <HTMLElement>this.html.getElementsByClassName('tooltip-header')[0];
        }

        set header(html: HTMLElement) {
            html.classList.add('tooltip-header');
            this.html.appendChild(html);
        }

        get content() {
            return <HTMLElement>this.html.getElementsByClassName('tooltip-content')[0];
        }

        set content(html: HTMLElement) {
            html.classList.add('tooltip-content');
            this.html.appendChild(html);
        }
    }

    function show(id: number,x:number,y:number) {
        var tooltip = tooltips[id];
        if (activeTooltipId !== id) {
            hide();
        }
        activeTooltipId = id;
        activeTooltip = tooltip.html;
        document.body.appendChild(activeTooltip);
        move(x, y);
    }

    function move(x: number, y: number) {
        var rect = activeTooltip.getBoundingClientRect();
        var length = rect.right - rect.left;
        var height = rect.bottom - rect.top;
        if (length + (x + 15) > window.innerWidth)  // flip horizontal position.
            activeTooltip.style.left = ((x -length) - 15) + "px";
        else 
            activeTooltip.style.left = (x + 15) + "px";
        
        if((y-height) > 0)
            activeTooltip.style.top = (y-height) + "px";
        else {
            activeTooltip.style.top = (y + 5) + "px";
        }
    }

    function hide() {
        if (activeTooltip) {
            activeTooltip.parentElement.removeChild(activeTooltip);
        }
        activeTooltip = null;
        activeTooltipId = null;
    }

    export function complexModify(id: number, content: HTMLElement, title?: HTMLElement) {
        var tt = tooltips[id];
        if (title) {
            tt.header.parentElement.removeChild(tt.header);
            tt.header = title;
        }
        tt.content.parentElement.removeChild(tt.content);
        tt.content = content;
    }

    export function modify(id: number, content: string, title?: string) {
        var tt = tooltips[id];
        if (title)
            tt.header.textContent = title;

        tt.content.textContent = content;
    }

    export function retrieveContent(id: number) {
        return tooltips[id].content;
    }

    export function complexCreate(element: HTMLElement, content: HTMLElement, title?: HTMLElement) {
        var tt = new Tooltip();
        addListeners(element);

        if (title) 
            tt.header = title;
        
        tt.content = content;

        tooltips.push(tt);
        if (element.dataset) {
            element.dataset['tooltip'] = registeredTooltips;
        } else {
            element.setAttribute('data-tooltip', registeredTooltips.toString());
        }
        registeredTooltips++;
    }

    export function create(element: HTMLElement, content: string, title?:string) {
        var text = document.createElement('div');
        text.textContent = content;

        if (title) {
            var header = document.createElement('div');
            header.textContent = title;
            complexCreate(element, text, header);
        } else {
            complexCreate(element, text);
        }
    }

    function addListeners(element: HTMLElement) {
        element.onmouseenter = function (e) {
            var id = +(<HTMLElement>e.target).getAttribute('data-tooltip');
            if (focusedElement !== <HTMLElement>e.target) {
                intervalId = setInterval(function() {
                    currentDelay += 0.01;
                    if (currentDelay >= appearDelay) {
                        show(id, x, y);
                        currentDelay = 0;
                        clearInterval(intervalId);
                        intervalId = null;
                    }
                }, 10);
            }
            focusedElement = <HTMLElement>e.target;
        };

        element.onmousemove = function (e) {
            var pos = mousePosition(e);
            if (!intervalId) {
                move(pos.x, pos.y);
            }
            x = pos.x;
            y = pos.y;
        };

        element.onmouseleave = function (e) {
            focusedElement = null;
            clearInterval(intervalId);
            currentDelay = 0;
            hide();
        };
    }


    function mousePosition(e: any) {
        if (e.pageX || e.pageY) {
            return { x: e.pageX, y: e.pageY };
        }
        else if (e.clientX || e.clientY) {
            return {
                x: e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft,
                y: e.clientY + document.body.scrollTop + document.documentElement.scrollTop
            };
        }
    }
} 