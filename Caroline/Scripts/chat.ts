//<reference path="connection.ts"/>

module Chat {
    declare var Komodo: { ClientActions: any; }
    var chatWindow;
    var chatLogContainer;
    var debugLogContainer;
    export function initialize() {
        if (!chatWindow) {
            chatWindow = document.createElement('DIV');
            chatWindow.style.transition = 'all 0.2s';
            chatWindow.id = 'chatWindow';
            chatWindow.style.position = 'fixed';
            chatWindow.style.bottom = '0px';
            chatWindow.style.left = '0px';
            chatWindow.style.minWidth = '400px';
            chatWindow.style.width = '40%';
            chatWindow.style.backgroundColor = '#ebebeb';
            chatWindow.style.border = '1px solid #adadad';
            chatWindow.style.height = '280px';
            chatWindow.style.boxShadow = '1px -1px 2px rgb(200,200,200)';

            var chatHeader = document.createElement('DIV');
            chatHeader.style.position = 'relative';
            chatHeader.style.height = '30px';
            chatHeader.style.backgroundColor = 'rgb(160, 160, 160)';
            chatWindow.appendChild(chatHeader);

            var chatCloser = document.createElement('DIV');
            chatCloser.textContent = 'Collapse';
            chatCloser.style.position = 'absolute';
            chatCloser.style.top = '0';
            chatCloser.style.right = '0';
            chatCloser.addEventListener('click', function () {
                var window = document.getElementById('chatWindow');
                window.style.bottom = (window.style.bottom == '0px') ? '-251px' : '0px';
            });
            chatHeader.appendChild(chatCloser);

            var chatRoomTab = document.createElement('DIV');
            chatRoomTab.style.color = 'white';
            chatRoomTab.style.fontSize = '18px';
            chatRoomTab.style.height = '30px';
            chatRoomTab.style.display = 'inline-block';
            chatRoomTab.textContent = 'General';
            chatRoomTab.style.padding = '3px 10px';
            chatRoomTab.addEventListener('click', function () {
                document.getElementById('debugpane').style.display = 'none';
                document.getElementById('chatpane').style.display = 'block';
            });
            chatHeader.appendChild(chatRoomTab);

            var debugTab = document.createElement('DIV');
            debugTab.style.color = 'white';
            debugTab.id = 'debugtab';
            debugTab.style.fontSize = '18px';
            debugTab.style.height = '30px';
            debugTab.style.display = 'inline-block';
            debugTab.textContent = '>Dev';
            debugTab.style.padding = '3px 10px';
            debugTab.addEventListener('click', function () {
                document.getElementById('debugpane').style.display = 'block';
                document.getElementById('chatpane').style.display = 'none';
            });
            debugTab.style.display = 'none';
            chatHeader.appendChild(debugTab);

            debugLogContainer = document.createElement('DIV');
            debugLogContainer.id = 'debugpane';
            debugLogContainer.style.width = '100%';
            debugLogContainer.style.height = '230px';
            debugLogContainer.style.display = 'none';
            debugLogContainer.style.position = 'relative';
            chatWindow.appendChild(debugLogContainer);

            chatLogContainer = document.createElement('DIV');
            chatLogContainer.id = 'chatpane';
            chatLogContainer.style.width = '100%';
            chatLogContainer.style.height = '230px';
            chatLogContainer.style.position = 'relative';
            chatWindow.appendChild(chatLogContainer);

            var debugLog = document.createElement('DIV');
            debugLog.id = 'debuglog';
            debugLog.style.position = 'absolute';
            debugLog.style.bottom = '0px';
            debugLog.style.maxHeight = '230px';
            debugLog.style.width = '100%';
            debugLog.style.overflow = 'auto';
            debugLogContainer.appendChild(debugLog);

            var chatLog = document.createElement('DIV');
            chatLog.id = 'chatlog';
            chatLog.style.position = 'absolute';
            chatLog.style.bottom = '0px';
            chatLog.style.maxHeight = '230px';
            chatLog.style.width = '100%';
            chatLog.style.overflow = 'auto';
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
            chatInput.addEventListener('keydown', function (e) {
                if (e.keyCode == 13)
                    sendGlobalMessagePress();

                if (e.keyCode == 68 && e.altKey)
                    document.getElementById('debugtab').style.display = 'inline-block';
            });
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
                sendGlobalMessagePress();
            });
            chatWindow.appendChild(chatSend);

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
    }
} 