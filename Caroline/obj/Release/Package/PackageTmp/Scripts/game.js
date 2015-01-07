var Chat;
(function (Chat) {
    var chatWindow;
    function initialize() {
        if (!chatWindow) {
            chatWindow = document.createElement('DIV');
            chatWindow.style.position = 'fixed';
            chatWindow.style.bottom = '0';
            chatWindow.style.left = '0';
            chatWindow.style.minWidth = '400px';
            chatWindow.style.width = '40%';
            chatWindow.style.backgroundColor = '#ebebeb';
            chatWindow.style.border = '1px solid #adadad';
            chatWindow.style.height = '250px';
            chatWindow.style.boxShadow = '1px -1px 2px rgb(200,200,200)';

            var chatLogContainer = document.createElement('DIV');
            chatLogContainer.style.width = '100%';
            chatLogContainer.style.height = '230px';
            chatLogContainer.style.position = 'relative';
            chatWindow.appendChild(chatLogContainer);

            var chatLog = document.createElement('DIV');
            chatLog.id = 'chatlog';
            chatLog.style.position = 'absolute';
            chatLog.style.bottom = '0px';
            chatLog.style.maxHeight = '230px';
            chatLog.style.width = '100%';
            chatLogContainer.appendChild(chatLog);

            var chatInput = document.createElement('INPUT');
            chatInput.setAttribute('TYPE', 'TEXT');
            chatInput.style.width = '100%';
            chatInput.style.maxWidth = 'none';
            chatInput.style.height = '20px';
            chatInput.style.border = 'none';
            chatInput.style.borderTop = '1px solid #adadad';
            chatInput.style.width = '85%';
            chatInput.setAttribute('maxlength', '220');
            chatInput.id = 'chattext';
            chatWindow.appendChild(chatInput);

            var chatSend = document.createElement('INPUT');
            chatSend.setAttribute('TYPE', 'BUTTON');
            chatSend.setAttribute('VALUE', 'SEND');
            chatSend.style.height = '20px';
            chatSend.style.width = '15%';
            chatSend.style.fontSize = '13px';
            chatSend.style.border = 'none';
            chatSend.style.borderTop = '1px solid #adadad';
            chatSend.style.borderLeft = '1px solid #adadad';
            chatSend.addEventListener('click', function () {
                sendGlobalMessage(document.getElementById('chattext').value);
                document.getElementById('chattext').value = '';
            });
            chatWindow.appendChild(chatSend);

            document.body.appendChild(chatWindow);
        }
    }
    Chat.initialize = initialize;
    initialize();

    function sendGlobalMessage(message) {
        var clientActions = new Komodo.ClientActions();
        var socialAction = new Komodo.ClientActions.SocialAction();
        var chatAction = new Komodo.ClientActions.SocialAction.ChatAction();
        chatAction.GlobalMessage = message;
        socialAction.Chat = chatAction;
        clientActions.SocialActions.push(socialAction);

        Connection.send(clientActions);
    }
    Chat.sendGlobalMessage = sendGlobalMessage;

    function receiveGlobalMessage(sender, message) {
        var chatLog = document.getElementById('chatlog');
        var chatItem = document.createElement('DIV');
        chatItem.style.minHeight = '25px';
        chatItem.style.width = '100%';
        var nameSpan = document.createElement('SPAN');
        nameSpan.textContent = sender;
        nameSpan.style.verticalAlign = 'top';
        nameSpan.style.display = 'inline-block';
        nameSpan.style.width = '15%';
        var messageSpan = document.createElement('SPAN');
        messageSpan.textContent = message;
        messageSpan.style.display = 'inline-block';
        messageSpan.style.width = '85%';
        chatItem.appendChild(nameSpan);
        chatItem.appendChild(messageSpan);

        chatLog.appendChild(chatItem);
    }
    Chat.receiveGlobalMessage = receiveGlobalMessage;
})(Chat || (Chat = {}));
///<reference path="chat.ts"/>
var Connection;
(function (Connection) {
    Komodo.connection.received(function (msg) {
        console.log("Recieved " + roughSizeOfObject(msg) + " bytes from komodo.");
        console.log("Encoded: ");
        console.log(msg);
        console.log("Decoded: ");
        console.log(Komodo.decode(msg));
        msg = Komodo.decode(msg);
        if (msg.Message != null) {
            Chat.receiveGlobalMessage(msg.Message.Sender, msg.Message.Text);
        }
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
