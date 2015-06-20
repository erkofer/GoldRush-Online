module Pheidippides {
    var listeners = new Array<Listener>();

    class Listener {
        element: HTMLElement;
        tag: string;
        callback: (elm: HTMLElement,msg: string) => void;
    }

    export function sprint(element: HTMLElement, tag: string, callback: (elm: HTMLElement, msg: string) => void) {
        var listener = new Listener();
        listener.element = element;
        listener.tag = tag;
        listener.callback = callback;

        listeners.push(listener);
    }

    export function deliver(tag: string, message: string) {
        var toRemove = new Array<number>();
        var offset = 0;

        for (var i = 0; i < listeners.length; i++) {
            var listener = listeners[i];
            if (listener.element) {
                if (listener.tag === tag) {
                    listener.callback(listener.element, message); 
                }
            }
            else
                toRemove.push(i);
        }

        for (var i = 0; i < toRemove.length; i++) {
            var offsetted = toRemove[i] - offset;
            listeners.splice(offsetted, 1);

            offset++;
        }
        
    }
    /* Pheidippides.sprint(marketErrorElm,"marketErrors",function(elm,msg) {
            var span = document.createElement("span");
            span.textContent = msg;
            elm.appendChild(span);
        });*/
} 