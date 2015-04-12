module Buffs {
    var buffs = new Array<Buff>();
    var sidebar = document.getElementById('sidebarInformation');
    var buffContainer;

    class Buff {
        constructor() {

        }

        name: string;
        description: string;
        duration: number;
        timeActive: number;

        container: HTMLElement;
        durationElm: HTMLElement;
    }

    function init() {
        buffContainer = document.createElement('div');
        sidebar.appendChild(buffContainer);
    }

    export function register(id: number, name: string, description: string, duration: number) {
        if (buffs[id]) return;

        var buff = new Buff();
        buff.name = name;
        buff.description = description;
        buff.duration = duration;
        buffs[id] = buff;

        draw(buff);
    }

    function draw(buff: Buff) {
        var container = document.createElement('div');
        container.classList.add('buff-container');
        tooltip.create(container, buff.description);

        var header = document.createElement('div');
        header.textContent = buff.name;
        header.classList.add('buff-header');
        container.appendChild(header);

        var imageContainer = document.createElement('div');
        imageContainer.classList.add('buff-image-container');
        container.appendChild(imageContainer);

        var image = document.createElement('div');
        image.classList.add(Utils.cssifyName(buff.name));
        imageContainer.appendChild(image);

        var duration = document.createElement('div');
        duration.classList.add('buff-header');
        container.appendChild(duration);

        buffContainer.appendChild(container);
        buff.container = container;
        buff.durationElm = duration;
    }

    export function update(id: number, timeActive: number) {
        var buff = buffs[id];
        buff.timeActive = timeActive;
        buff.durationElm.textContent = '('+Utils.formatTime(buff.duration - timeActive)+')';
        buff.container.style.display = timeActive == 0 ? 'none' : 'inline-block';
    }
    init();
} 