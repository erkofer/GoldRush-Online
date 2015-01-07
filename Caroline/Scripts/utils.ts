module Utils {
    export function cssSwap(element: HTMLElement, initialVal: string, finalVal: string) {
        if (element.classList.contains(initialVal))
            element.classList.remove(initialVal);

        element.classList.add(finalVal);
    }

    export function cssifyName(name: string): string {
        return name.split(' ').join('_');
    }

    export function isNumber(obj) { return !isNaN(parseFloat(obj)) }

    export function formatNumber(n: number): string {
        if (!n)
            return '0';

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

    export function convertServerTimeToLocal(time: string): string {
        var hours = +time.split(':')[0];
        var minutes = +(time.split(':')[1]).split(' ')[0];
        var amOrPm = (time.split(':')[1]).split(' ')[1];

        if (amOrPm == 'PM')
            hours += 12;

        var offset = new Date().getTimezoneOffset();
        hours -= ((offset / 60) - offset % 60);
        minutes -= offset % 60;

        if (hours < 0)
            hours = 24 - hours;
      
        if (minutes < 0) {
            minutes = 60 - minutes;
            hours--;
        }

        if (hours > 23) {
            hours = hours - 24;
        }

        if (minutes > 59) {
            minutes = minutes - 60;
            hours++;
        }

        return (hours < 10 ? '0' : '') + hours + ':' + (minutes < 10 ? '0' : '') + minutes;
    }
} 