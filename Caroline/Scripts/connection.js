///<reference path="chat.ts"/>
var Connection;
(function (Connection) {
    Komodo.connection.received(function (msg) {
        console.log("Recieved " + roughSizeOfObject(msg) + " bytes from komodo.");
        console.log("Encoded: ");
        console.log(msg);
        console.log("Decoded: ");
        console.log(Komodo.decode(msg));
    });

    function send(message) {
        console.log("Sent " + roughSizeOfObject(message) + " bytes to komodo.");
        console.log("Decoded: ");
        console.log(JSON.stringify(message));
        console.log("Encoded: ");
        console.log(message.encode64());
        Komodo.send(message.encode64());
    }
    Connection.send = send;

    function roughSizeOfObject(object) {
        var objectList = [];
        var stack = [object];
        var bytes = 0;

        while (stack.length) {
            var value = stack.pop();

            if (typeof value === 'boolean') {
                bytes += 4;
            } else if (typeof value === 'string') {
                bytes += value.length * 2;
            } else if (typeof value === 'number') {
                bytes += 8;
            } else if (typeof value === 'object' && objectList.indexOf(value) === -1) {
                objectList.push(value);

                for (var i in value) {
                    stack.push(value[i]);
                }
            }
        }
        return bytes;
    }
})(Connection || (Connection = {}));
