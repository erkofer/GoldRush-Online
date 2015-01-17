var Komodo;
(function (Komodo) {
    Komodo.connection = $.connection("/neon/komodo/dragons");
   /* Komodo.connection.received(function (msg) {
        console.log("Recieved " + roughSizeOfObject(msg) + " bytes from server.");
        console.log("Encoded: ");
        console.log(msg);
        console.log("Decoded: ")
        console.log(decodeMsg(msg));
    });*/
    Komodo.connection.start();

    function restart() {
        Komodo.connection.stop();
        Komodo.connection.start();
    }
    Komodo.restart = restart;
    
    var ProtoBuf = dcodeIO.ProtoBuf,
        ByteBuffer = ProtoBuf.ByteBuffer;

    var builder = ProtoBuf.loadProtoFile("../Content/GameState.proto"),
        GameStateDecoder = builder.build("Caroline.App.Models");

    var outputBuilder = ProtoBuf.loadProtoFile("../Content/ClientActions.proto"),
        ClientActionsEncoder = outputBuilder.build("Caroline.App.Models");
    Komodo.ClientActions = ClientActionsEncoder.ClientActions;


    function sendMsg(msg) {
        Komodo.connection.send(msg);
    }
    Komodo.send = sendMsg;

    function buyTest() {
        var clientActions = new Komodo.ClientActions({
            "SellOrders": [
                { "Id": 2, "Quantity": 5 },
                { "Id": 6, "Quantity": 15 }
            ]
        });
        var message = clientActions.encode();
        Komodo.connection.send(message);
    }
    Komodo.test = buyTest;

    function decodeMsg(msg) {
        return GameStateDecoder.GameState.decode(msg);
    }
    Komodo.decode = decodeMsg;
    
})(Komodo || (Komodo = {}));