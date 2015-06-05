module Utils {
    export function addEvent(elem, type, eventHandle) {
        if (elem == null || typeof (elem) == 'undefined') return;
        if (elem.addEventListener) {
            elem.addEventListener(type, eventHandle, false);
        } else if (elem.attachEvent) {
            elem.attachEvent("on" + type, eventHandle);
        } else {
            elem["on" + type] = eventHandle;
        }
    };

    export function cssSwap(element: HTMLElement, initialVal: string, finalVal: string) {
        if (element.classList.contains(initialVal))
            element.classList.remove(initialVal);

        element.classList.add(finalVal);
    }

    export function cssifyName(name: string): string {
        return name.split(' ').join('_');
    }

    export function ifNotDefault(value, callback) {
        if (value != -100) callback();
    }

    export function createButton(text, id): HTMLElement {
        var button;
        var textcontent;

        button = <HTMLElement>document.createElement("div");
        textcontent = document.createElement("div");
        textcontent.textContent = text;

        if (id) {
            textcontent.id = id;
        }

        button.classList.add("button");
        button.appendChild(textcontent);

        return button;
    }


    export function isNumber(obj) { return !isNaN(parseFloat(obj)) }

    export function formatNumber(n: number, detailed: boolean = false): string {
        if (!n)
            return '0';

        if (detailed)
            return n.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

        if (n > 999999999999999) {
            return (n / 1000000000000000).toFixed(3) + "Qa";
        } else if (n > 999999999999) {
            return (n / 1000000000000).toFixed(3) + "T";
        } else if (n > 999999999) {
            return (n / 1000000000).toFixed(3) + "B";
        } else if (n > 999999) {
            return (n / 1000000).toFixed(3) + "M";
        } else {
            return n.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    }

    export function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    export function formatTime(n: number) {
        var hours = Math.floor(n / 3600);
        var minutes = Math.floor((n % 3600) / 60);
        var seconds = Math.floor(n % 60);

        var hoursString = (hours < 10 ? (hours < 1 ? "" : "0" + hours + ":") : hours + ":");
        var minutesString = (minutes < 10 ? "0" + minutes + ":" : minutes + ":");
        var secondsString = (seconds < 10 ? "0" + seconds : seconds + "");

        return hoursString + minutesString + secondsString;
    }

    export function convertServerTimeToLocal(time: string): string {
        var localDate = new Date();

        var year = localDate.toISOString().split('-')[0];
        var month = localDate.toISOString().split('-')[1];
        var day = localDate.toISOString().split('-')[2].split('T')[0];

        var hours = +time.split(':')[0];
        var minutes = +(time.split(':')[1]).substring(0, 2);
        var amOrPm = time.split(' ')[1];


        var dateString = month + '/'
            + day + '/'
            + year+ ' '
            + ((hours < 10 ? '0' : '') + hours) + ':'
            + ((minutes < 10 ? '0' : '') + minutes) + ':'
            + '00 ' 
            + amOrPm + ' UTC';
        var toConvertDate = new Date(dateString);

        console.log(dateString);
        console.log(toConvertDate);

        hours = toConvertDate.getHours();
        minutes = toConvertDate.getMinutes();
        
        return (hours < 10 ? '0' : '') + hours + ':' + (minutes < 10 ? '0' : '') + minutes;
    }
} 