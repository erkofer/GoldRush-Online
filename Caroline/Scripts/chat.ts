//<reference path="connection.ts"/>

module Chat {
    declare var Komodo: { ClientActions: any; }
    var chatWindow;
    var chatLogContainer;
    var debugLogContainer;
    export function initialize() {
        if (!chatWindow) {
            chatWindow = document.createElement('DIV');
            chatWindow.classList.add('chat-window');
            chatWindow.classList.add('social');
            chatWindow.id = 'chatWindow';

            chatWindow.style.backgroundColor = '#ebebeb';
            chatWindow.style.border = '1px solid #adadad';

            var chatHeader = document.createElement('DIV');
            chatHeader.classList.add('chat-header');

            chatHeader.style.backgroundColor = 'rgb(160, 160, 160)';
            chatWindow.appendChild(chatHeader);

            var chatCloser = document.createElement('DIV');
            chatCloser.textContent = '_';
            chatCloser.style.position = 'absolute';
            chatCloser.style.top = '0';
            chatCloser.style.right = '0';
            chatCloser.addEventListener('click', function () {
                if (chatWindow.classList.contains('closed')) {
                    chatWindow.classList.remove('closed');
                    chatCloser.textContent = '_';
                }
                else {
                    chatWindow.classList.add('closed');
                    chatCloser.textContent = '+';
                }
            });
            chatHeader.appendChild(chatCloser);

            var chatRoomTab = document.createElement('DIV');
            chatRoomTab.classList.add('chat-room-tab');
            chatRoomTab.textContent = 'General';
            chatRoomTab.addEventListener('click', function () {
                document.getElementById('debugpane').style.display = 'none';
                document.getElementById('chatpane').style.display = 'block';
            });
            chatHeader.appendChild(chatRoomTab);

            var debugTab = document.createElement('DIV');
            debugTab.classList.add('chat-room-tab');
            debugTab.textContent = '>Dev';
            debugTab.addEventListener('click', function () { // refactor this later with objects and shit.
                document.getElementById('debugpane').style.display = 'block';
                document.getElementById('chatpane').style.display = 'none';
            });
            debugTab.style.display = 'none';
            chatHeader.appendChild(debugTab);

            debugLogContainer = document.createElement('DIV');
            debugLogContainer.id = 'debugpane';
            debugLogContainer.classList.add('chat-room');
            debugLogContainer.style.display = 'none';
            chatWindow.appendChild(debugLogContainer);

            chatLogContainer = document.createElement('DIV');
            chatLogContainer.id = 'chatpane';
            chatLogContainer.classList.add('chat-room');
            chatWindow.appendChild(chatLogContainer);

            var debugLog = document.createElement('DIV');
            debugLog.id = 'debuglog';
            debugLog.classList.add('chat-room-content');
            debugLogContainer.appendChild(debugLog);

            var chatLog = document.createElement('DIV');
            chatLog.id = 'chatlog';
            chatLog.classList.add('chat-room-content');
            chatLogContainer.appendChild(chatLog);

            var chatSendingContainer = document.createElement('DIV');

            var chatInputContainer = document.createElement('DIV');
            chatInputContainer.classList.add('chat-textbox-container');

            var chatInput = document.createElement('INPUT');
            chatInput.setAttribute('TYPE', 'TEXT');
            chatInput.classList.add('chat-textbox');
            chatInput.style.borderTop = '1px solid #adadad';
            chatInput.setAttribute('maxlength', '220');
            chatInput.addEventListener('keydown', function (e) {
                if (e.keyCode == 13)
                    sendGlobalMessagePress();

                if (e.keyCode == 68 && e.altKey)
                    document.getElementById('debugtab').style.display = 'inline-block';
            });
            chatInput.id = 'chattext';
            chatInputContainer.appendChild(chatInput);
            chatSendingContainer.appendChild(chatInputContainer);

            var chatSendContainer = document.createElement('DIV');
            chatSendContainer.classList.add('chat-submit-container');

            var chatSend = document.createElement('INPUT');
            chatSend.setAttribute('TYPE', 'BUTTON');
            chatSend.setAttribute('VALUE', 'SEND');
            chatSend.classList.add('chat-submit');
            chatSend.style.borderTop = '1px solid #adadad';
            chatSend.style.borderLeft = '1px solid #adadad';
            chatSend.addEventListener('click', function () {
                sendGlobalMessagePress();
            });
            chatSendContainer.appendChild(chatSend);
            chatSendingContainer.appendChild(chatSendContainer);
            chatWindow.appendChild(chatSendingContainer);

            document.body.appendChild(chatWindow);
        }
    }
    initialize();

    function sendGlobalMessagePress() {
        Connection.sendGlobalMessage((<HTMLInputElement>document.getElementById('chattext')).value);
        (<HTMLInputElement>document.getElementById('chattext')).value = '';
    }

    export function log(message: string) {
        var debugLog = document.getElementById('debuglog');
        var debugItem = document.createElement('DIV');
        debugItem.textContent = message;
        debugLog.appendChild(debugItem);
    }


    export function receiveGlobalMessage(sender: string, message: string, time:string, perms?: string) {
        var chatLog = document.getElementById('chatlog');

        var difference = chatLog.scrollTop - (chatLog.scrollHeight - chatLog.offsetHeight);
        var scrolledDown = Math.abs(difference) < 5;

        var chatItem = document.createElement('DIV');
        chatItem.classList.add('chat-msg');
        if (perms && perms != '')
            chatItem.classList.add('chat-'+perms);
        var timeSpan = document.createElement('SPAN');
        timeSpan.textContent = '['+Utils.convertServerTimeToLocal(time)+'] ';
        var nameSpan = document.createElement('SPAN');
        nameSpan.textContent = sender;
        nameSpan.classList.add('chat-sender');
        var messageSpan = document.createElement('SPAN');
        messageSpan.textContent = message;
        messageSpan.classList.add('chat-text');
        var dividerSpan = document.createElement('SPAN');
        dividerSpan.textContent = ': ';
        chatItem.appendChild(timeSpan);
        chatItem.appendChild(nameSpan);
        chatItem.appendChild(dividerSpan);
        chatItem.appendChild(messageSpan);

        chatLog.appendChild(chatItem);

        if (scrolledDown) chatLog.scrollTop = chatLog.scrollHeight;
    }
} 