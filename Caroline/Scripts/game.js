var Utils;
(function (Utils) {
    function addEvent(elem, type, eventHandle) {
        if (elem == null || typeof (elem) == 'undefined')
            return;
        if (elem.addEventListener) {
            elem.addEventListener(type, eventHandle, false);
        } else if (elem.attachEvent) {
            elem.attachEvent("on" + type, eventHandle);
        } else {
            elem["on" + type] = eventHandle;
        }
    }
    Utils.addEvent = addEvent;
    ;

    function cssSwap(element, initialVal, finalVal) {
        if (element.classList.contains(initialVal))
            element.classList.remove(initialVal);

        element.classList.add(finalVal);
    }
    Utils.cssSwap = cssSwap;

    function cssifyName(name) {
        return name.split(' ').join('_');
    }
    Utils.cssifyName = cssifyName;

    function ifNotDefault(value, callback) {
        if (value != -100)
            callback();
    }
    Utils.ifNotDefault = ifNotDefault;

    function createButton(text, id) {
        var button;
        var textcontent;

        button = document.createElement("div");
        textcontent = document.createElement("div");
        textcontent.textContent = text;

        if (id) {
            textcontent.id = id;
        }

        button.classList.add("button");
        button.appendChild(textcontent);

        return button;
    }
    Utils.createButton = createButton;

    function isNumber(obj) {
        return !isNaN(parseFloat(obj));
    }
    Utils.isNumber = isNumber;

    function formatNumber(n, detailed) {
        if (typeof detailed === "undefined") { detailed = false; }
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
    Utils.formatNumber = formatNumber;

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }
    Utils.getRandomInt = getRandomInt;

    function formatTime(n) {
        var hours = Math.floor(n / 3600);
        var minutes = Math.floor((n % 3600) / 60);
        var seconds = Math.floor(n % 60);

        var hoursString = (hours < 10 ? (hours < 1 ? "" : "0" + hours + ":") : hours + ":");
        var minutesString = (minutes < 10 ? "0" + minutes + ":" : minutes + ":");
        var secondsString = (seconds < 10 ? "0" + seconds : seconds + "");

        return hoursString + minutesString + secondsString;
    }
    Utils.formatTime = formatTime;

    function convertServerTimeToLocal(time) {
        var localDate = new Date();

        var year = localDate.toISOString().split('-')[0];
        var month = localDate.toISOString().split('-')[1];
        var day = localDate.toISOString().split('-')[2].split('T')[0];

        var hours = +time.split(':')[0];
        var minutes = +(time.split(':')[1]).split(' ')[0];
        var amOrPm = time.split(':')[1].split(' ')[1];

        var dateString = month + '/' + day + '/' + year + ' ' + ((hours < 10 ? '0' : '') + hours) + ':' + ((minutes < 10 ? '0' : '') + minutes) + ':' + '00 ' + amOrPm + ' UTC';
        var toConvertDate = new Date(dateString);

        console.log(dateString);
        console.log(toConvertDate);

        hours = toConvertDate.getHours();
        minutes = toConvertDate.getMinutes();

        return (hours < 10 ? '0' : '') + hours + ':' + (minutes < 10 ? '0' : '') + minutes;
    }
    Utils.convertServerTimeToLocal = convertServerTimeToLocal;
})(Utils || (Utils = {}));
var Ajax;
(function (Ajax) {
    var loaders = new Array();

    var Loader = (function () {
        function Loader() {
            this.notDisplayed = 20;
            this.notches = 40;
            this.space = 0.02;
            this.spawned = false;
            this.spinPos = 0;
            this.spinSpeed = 30;
            this.squareSize = 50;
            this.timeSinceLastTick = Date.now();
            this.thickness = 3;
            this.dead = false;
            this.container = document.createElement('div');
            this.container.classList.add('loader');
            this.canvasElement = document.createElement('canvas');
            this.canvasElement.width = this.squareSize;
            this.canvasElement.height = this.squareSize;
            this.container.appendChild(this.canvasElement);
            this.context = this.canvasElement.getContext('2d');
        }
        Loader.prototype.update = function () {
            var time = Date.now();

            if (this.canvasElement.parentElement)
                this.spawned = true;

            var timePassed = time - this.timeSinceLastTick;
            timePassed /= 1000;
            this.spinPos += this.spinSpeed * timePassed;
            this.timeSinceLastTick = time;

            if (this.spawned && !this.canvasElement.parentElement)
                this.dead = true;

            this.draw();
        };

        Loader.prototype.draw = function () {
            var x = this.squareSize / 2;
            var y = x;
            var adjustedSpinPosStart = this.spinPos % 2;
            var adjustedSpinPosEnd = (this.spinPos + 1) % 2;

            var radius = (this.squareSize / 2) - 5;
            var lengthEach = (2 - (this.space * this.notches)) / this.notches;
            var selected = this.spinPos % this.notches;
            var shown = selected - this.notDisplayed;

            this.context.clearRect(0, 0, this.canvasElement.width, this.canvasElement.height);
            for (var i = 0; i < this.notches; i++) {
                var start = 0 + (i * lengthEach) + (this.space * i);
                var end = start + lengthEach;
                var inverse = i - this.notches;

                if (i <= selected && i >= shown) {
                    this.context.beginPath();
                    this.context.arc(x, y, radius, start * Math.PI, end * Math.PI);
                    this.context.lineWidth = this.thickness;
                    this.context.strokeStyle = 'rgba(113,142,164,' + (1 - (selected - i) / (this.notches - this.notDisplayed)) + ')';
                    this.context.stroke();
                } else if (inverse <= selected && inverse >= shown) {
                    this.context.beginPath();
                    this.context.arc(x, y, radius, start * Math.PI, end * Math.PI);
                    this.context.lineWidth = this.thickness;
                    this.context.strokeStyle = 'rgba(113,142,164,' + (1 - (selected - inverse) / (this.notches - this.notDisplayed)) + ')';
                    this.context.stroke();
                }
            }
        };
        return Loader;
    })();

    function createLoader() {
        var loader = new Loader();
        loaders.push(loader);
        return loader.container;
    }
    Ajax.createLoader = createLoader;

    function update() {
        for (var i = 0; i < loaders.length; i++) {
            var loader = loaders[i];
            if (loader.dead)
                loaders.splice(i, 1);
            else
                loader.update();
        }
        setTimeout(update, 10);
    }
    update();
})(Ajax || (Ajax = {}));
var modal;
(function (modal) {
    var timeOpened = 0;
    var modalPane;
    modal.activeWindow;
    modal.intervalIdentifier;

    function hide() {
        if (modal.activeWindow) {
            modal.activeWindow.hide();
        }
        modal.activeWindow = null;
    }
    modal.hide = hide;

    function close() {
        if (modal.activeWindow) {
            var a = modal.activeWindow;
            hide();
            a.container.parentNode.removeChild(a.container);
        }
    }
    modal.close = close;

    var Window = (function () {
        function Window() {
            this.container = document.createElement("div");
            this.container.addEventListener("click", function (e) {
                e.stopPropagation();
            }, false);
            this.container.classList.add("modal-window");
            if (!modalPane) {
                var pane = document.createElement("div");
                modalPane = pane;
                pane.classList.add("modal-wrapper");
                pane.addEventListener("click", function (e) {
                    e.stopPropagation();
                    if ((Date.now() - timeOpened) > 3000)
                        modal.close();
                }, false);
                document.body.appendChild(pane);
            }
            modalPane.appendChild(this.container);

            this.titleEl = document.createElement("div");
            this.titleEl.classList.add("modal-header");
            this.bodyEl = document.createElement("div");

            this.container.appendChild(this.titleEl);
            this.container.appendChild(this.bodyEl);
        }
        Object.defineProperty(Window.prototype, "title", {
            get: function () {
                return this._title;
            },
            set: function (s) {
                this._title = s;
                this.titleEl.textContent = this._title;
            },
            enumerable: true,
            configurable: true
        });

        Window.prototype.addElement = function (el) {
            this.bodyEl.appendChild(el);
        };

        // intended for the bottom bar of controls.
        Window.prototype.addOption = function (opt) {
            if (!this.options) {
                this.options = document.createElement("div");
                this.options.classList.add("modal-options");
                this.container.appendChild(this.options);
            }
            var optionContainer = document.createElement("span");
            optionContainer.classList.add("modal-option");

            var option = document.createElement("span");
            option.textContent = opt;

            optionContainer.appendChild(option);
            this.options.appendChild(optionContainer);
            return optionContainer;
        };

        Window.prototype.addAffirmativeOption = function (opt) {
            var option = this.addOption(opt);
            option.classList.add("affirmative");
            return option;
        };

        Window.prototype.addNegativeOption = function (opt) {
            var option = this.addOption(opt);
            option.classList.add("negative");
            return option;
        };

        Window.prototype.show = function () {
            if (!this.container.classList.contains("opened"))
                this.container.classList.add("opened");
            if (!modalPane.classList.contains("opened"))
                modalPane.classList.add("opened");
            modal.activeWindow = this;
            updatePosition();
            modal.intervalIdentifier = setInterval(updatePosition, 100);
            timeOpened = Date.now();
        };

        Window.prototype.hide = function () {
            if (this.container.classList.contains("opened"))
                this.container.classList.remove("opened");
            if (modalPane.classList.contains("opened"))
                modalPane.classList.remove("opened");
        };
        return Window;
    })();
    modal.Window = Window;

    function updatePosition() {
        if (!modal.activeWindow) {
            clearInterval(modal.intervalIdentifier);
        } else {
            var containerDimensions = modal.activeWindow.container.getBoundingClientRect();
            modal.activeWindow.container.style.left = (window.innerWidth / 2) - ((containerDimensions.right - containerDimensions.left) / 2) + "px";
            modal.activeWindow.container.style.top = (window.innerHeight / 2) - ((containerDimensions.bottom - containerDimensions.top) / 2) + "px";
        }
    }
})(modal || (modal = {}));
var Objects;
(function (Objects) {
    var gameobjects = new Array();

    var GameObject = (function () {
        function GameObject() {
            this.quantity = 0;
        }
        return GameObject;
    })();

    function register(id, name) {
        if (!gameobjects[id]) {
            var gameobject = new GameObject();
            gameobject.name = name;

            gameobjects[id] = gameobject;
        }
    }
    Objects.register = register;

    function lookupName(id) {
        return gameobjects[id].name;
    }
    Objects.lookupName = lookupName;

    function setQuantity(id, quantity) {
        gameobjects[id].quantity = quantity;
    }
    Objects.setQuantity = setQuantity;

    function getQuantity(id) {
        var gameobject = gameobjects[id];
        if (gameobject)
            return gameobject.quantity;

        return 0;
    }
    Objects.getQuantity = getQuantity;

    function setMaxQuantity(id, maxQuantity) {
        gameobjects[id].maxQuantity = maxQuantity;
    }
    Objects.setMaxQuantity = setMaxQuantity;

    function getMaxQuantity(id) {
        return gameobjects[id].maxQuantity;
    }
    Objects.getMaxQuantity = getMaxQuantity;

    function setLifeTimeTotal(id, quantity) {
        gameobjects[id].lifeTimeTotal = quantity;
    }
    Objects.setLifeTimeTotal = setLifeTimeTotal;

    function getLifeTimeTotal(id) {
        return gameobjects[id].lifeTimeTotal;
    }
    Objects.getLifeTimeTotal = getLifeTimeTotal;

    function setTooltip(id, tooltip) {
        gameobjects[id].tooltip = tooltip;
    }
    Objects.setTooltip = setTooltip;
    function getTooltip(id) {
        return gameobjects[id].tooltip;
    }
    Objects.getTooltip = getTooltip;
})(Objects || (Objects = {}));
/// <reference path="typings/jquery/jquery.d.ts"/>
var Account;
(function (Account) {
    var container;
    var userButton;
    var userSpan;
    var contextMenu;

    var registrationErrors;
    var loginErrors;

    var mouseTimeout;

    var LeaderboardAjaxService = (function () {
        function LeaderboardAjaxService() {
        }
        LeaderboardAjaxService.prototype.failed = function (request) {
            this.resultsElement.textContent = 'Loading failed...';
        };

        LeaderboardAjaxService.prototype.succeeded = function (request) {
            while (this.resultsElement.firstChild)
                this.resultsElement.removeChild(this.resultsElement.firstChild);

            var leaderboardTable = document.createElement('table');
            var thead = leaderboardTable.createTHead();
            var subheader = thead.insertRow(0);
            subheader.classList.add('table-subheader');

            var score = subheader.insertCell(0);
            score.textContent = 'Score';
            score.style.width = '65%';
            var player = subheader.insertCell(0);
            player.textContent = 'Name';
            player.style.width = '25%';
            var rank = subheader.insertCell(0);
            rank.textContent = 'Rank';
            rank.style.width = '10%';

            var tbody = leaderboardTable.createTBody();

            for (var i = 0; i < request.length; i++) {
                var leaderboardEntry = request[i];
                console.log(leaderboardEntry);

                var row = tbody.insertRow(tbody.rows.length);
                row.classList.add('table-row');

                var rScore = row.insertCell(0);
                rScore.textContent = Utils.formatNumber(leaderboardEntry.Score);
                rScore.style.width = '65%';
                rScore.addEventListener('click', function (event) {
                    var score;
                    var cell = event.target;

                    if (cell.dataset) {
                        score = cell.dataset['tooltip'];
                    } else {
                        score = cell.getAttribute('data-tooltip');
                    }

                    if (cell.textContent.indexOf(',') > 0) {
                        cell.textContent = Utils.formatNumber(score);
                    } else {
                        cell.textContent = Utils.formatNumber(score, true);
                    }
                });

                if (rScore.dataset) {
                    rScore.dataset['tooltip'] = leaderboardEntry.Score;
                } else {
                    rScore.setAttribute('data-tooltip', leaderboardEntry.Score.toString());
                }

                var rPlayer = row.insertCell(0);
                rPlayer.textContent = leaderboardEntry.UserId;
                player.style.width = '25%';
                var rRank = row.insertCell(0);
                rRank.textContent = Utils.formatNumber(leaderboardEntry.Rank);
                rRank.style.width = '10%';
            }
            this.resultsElement.appendChild(leaderboardTable);
        };

        LeaderboardAjaxService.prototype.sendRequest = function (lowerbound, upperbound) {
            var self = this;

            var request = $.ajax({
                asyn: true,
                type: 'POST',
                url: '/Api/Stats/LeaderBoard/',
                data: $.param({ Lower: lowerbound, Upper: upperbound }),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                success: function (request) {
                    request = JSON.parse(request);
                    self.succeeded(request);
                },
                failure: function (request) {
                    self.failed(request);
                }
            });
        };
        return LeaderboardAjaxService;
    })();

    function draw() {
        container = document.createElement('DIV');
        container.classList.add('account-manager');
        container.classList.add('closed');

        container.onmouseenter = function () {
            clearTimeout(mouseTimeout);
        };

        container.onmouseleave = function () {
            mouseTimeout = setTimeout(hideMenu, 250);
        };

        // Anon stuff.
        var loginButton = document.createElement('DIV');
        loginButton.textContent = 'Sign in';
        loginButton.classList.add('account-option');
        loginButton.classList.add('anonymous-account-option');
        loginButton.addEventListener('click', function () {
            loginModal();
        });
        container.appendChild(loginButton);

        var registerButton = document.createElement('DIV');
        registerButton.textContent = 'Register';
        registerButton.classList.add('account-option');
        registerButton.classList.add('anonymous-account-option');
        registerButton.addEventListener('click', function () {
            registerModal();
        });
        container.appendChild(registerButton);

        // Registered stuff.
        var optionsButton = document.createElement('DIV');
        optionsButton.textContent = 'Options';
        optionsButton.classList.add('account-option');
        optionsButton.classList.add('registered-account-option');
        container.appendChild(optionsButton);

        var logoffButton = document.createElement('DIV');
        logoffButton.textContent = 'Sign out';
        logoffButton.classList.add('account-option');
        logoffButton.classList.add('registered-account-option');
        logoffButton.addEventListener('click', function () {
            logoff();
        });
        container.appendChild(logoffButton);

        userButton = document.createElement('DIV');
        userButton.classList.add('account-user');
        userSpan = document.createElement('SPAN');
        userButton.appendChild(userSpan);
        container.appendChild(userButton);

        document.body.appendChild(container);
        userButton.addEventListener('click', function () {
            toggleMenu();
        });

        var highscoresLink = document.createElement('SPAN');
        highscoresLink.textContent = 'Leaderboards';
        highscoresLink.addEventListener('click', function () {
            leaderboardsModal();
        });
        highscoresLink.style.cursor = 'pointer';
        document.getElementsByClassName('header-links')[0].appendChild(highscoresLink);

        info();
    }
    draw();
    function toggleMenu() {
        if (container.classList.contains('closed'))
            container.classList.remove('closed');
        else
            container.classList.add('closed');
    }

    function hideMenu() {
        if (!container.classList.contains('closed'))
            container.classList.add('closed');
    }

    function updateUser(name, isAnon) {
        userSpan.textContent = isAnon ? 'Guest' : name;

        // styles the container depending on the status of the account.
        Utils.cssSwap(container, isAnon ? 'registered' : 'anonymous', isAnon ? 'anonymous' : 'registered');
    }

    function leaderboardsModal() {
        var leaderboardModal = new modal.Window();
        leaderboardModal.title = 'Leaderboards';
        var leaderboardList = document.createElement('DIV');
        leaderboardList.style.width = '400px';
        leaderboardList.appendChild(Ajax.createLoader());
        leaderboardModal.addElement(leaderboardList);

        var leaderboardService = new LeaderboardAjaxService();
        leaderboardService.resultsElement = leaderboardList;
        leaderboardService.sendRequest(0, 20);

        leaderboardModal.show();
    }

    function loginModal() {
        var loginModal = new modal.Window();
        var formControlsContainer = document.createElement('DIV');
        formControlsContainer.style.width = '400px';

        var usernameContainer = document.createElement('DIV');
        usernameContainer.style.marginBottom = '5px';
        var username = document.createElement("INPUT");
        username.type = 'TEXT';
        username.maxLength = 16;
        username.placeholder = 'Username';
        usernameContainer.appendChild(username);

        var passwordContainer = document.createElement('DIV');
        passwordContainer.style.marginBottom = '5px';
        var password = document.createElement("INPUT");
        password.type = 'PASSWORD';
        password.pattern = ".{6,}";
        password.placeholder = 'Password';
        passwordContainer.appendChild(password);

        var rememberMeContainer = document.createElement('DIV');
        rememberMeContainer.style.marginBottom = '5px';
        var rememberMe = document.createElement('INPUT');
        rememberMe.type = 'CHECKBOX';
        rememberMe.placeholder = 'Stay logged in on this computer?';
        rememberMeContainer.appendChild(rememberMe);

        formControlsContainer.appendChild(usernameContainer);
        formControlsContainer.appendChild(passwordContainer);
        formControlsContainer.appendChild(rememberMeContainer);

        loginErrors = document.createElement('div');
        loginModal.addElement(loginErrors);

        loginModal.title = 'Log in';
        loginModal.addElement(formControlsContainer);

        var no = loginModal.addNegativeOption("Cancel");
        no.addEventListener("click", function () {
            modal.close();
        }, false);
        var yes = loginModal.addAffirmativeOption("Submit");
        yes.addEventListener("click", function () {
            login(username.value, password.value, rememberMe.checked);
            while (loginErrors.firstChild) {
                loginErrors.removeChild(loginErrors.firstChild);
            }
        }, false);
        loginModal.show();
    }

    function registerModal() {
        var registerModal = new modal.Window();
        var formControlsContainer = document.createElement('DIV');
        formControlsContainer.style.width = '400px';

        var usernameContainer = document.createElement('DIV');
        usernameContainer.style.marginBottom = '5px';
        var username = document.createElement("INPUT");
        username.type = 'TEXT';
        username.maxLength = 16;
        username.placeholder = 'Username';
        usernameContainer.appendChild(username);

        var emailContainer = document.createElement('DIV');
        emailContainer.style.marginBottom = '5px';
        var email = document.createElement("INPUT");
        email.type = 'EMAIL';
        email.placeholder = 'Email';
        emailContainer.appendChild(email);

        var passwordContainer = document.createElement('DIV');
        passwordContainer.style.marginBottom = '5px';
        var password = document.createElement("INPUT");
        password.type = 'PASSWORD';
        password.pattern = ".{6,}";
        password.placeholder = 'Password';
        passwordContainer.appendChild(password);

        var confpassContainer = document.createElement('DIV');
        confpassContainer.style.marginBottom = '5px';
        var confirmPassword = document.createElement("INPUT");
        confirmPassword.type = 'PASSWORD';
        confirmPassword.pattern = ".{6,}";
        confirmPassword.placeholder = 'Confirm password';
        confpassContainer.appendChild(confirmPassword);
        confirmPassword.onblur = function () {
            if (password.value != confirmPassword.value)
                confirmPassword.setCustomValidity('Passwords are not the same.');
        };

        confirmPassword.onfocus = function () {
            confirmPassword.setCustomValidity('');
        };
        formControlsContainer.appendChild(usernameContainer);
        formControlsContainer.appendChild(emailContainer);
        formControlsContainer.appendChild(passwordContainer);
        formControlsContainer.appendChild(confpassContainer);

        registerModal.addElement(formControlsContainer);
        registrationErrors = document.createElement('div');
        registerModal.addElement(registrationErrors);

        registerModal.title = "Register";

        var no = registerModal.addNegativeOption("Cancel");
        no.addEventListener("click", function () {
            modal.close();
        }, false);
        var yes = registerModal.addAffirmativeOption("Submit");
        yes.addEventListener("click", function () {
            create(email.value, username.value, password.value, confirmPassword.value);
            while (registrationErrors.firstChild) {
                registrationErrors.removeChild(registrationErrors.firstChild);
            }
        }, false);

        registerModal.show();
    }

    function create(email, username, password, passwordConfirmation) {
        var request = $.ajax({
            type: 'POST',
            url: '/Api/Account/Register',
            data: $.param({ Email: email, UserName: username, Password: password, ConfirmPassword: passwordConfirmation }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            success: function (request) {
                request = JSON.parse(request);
                if (request.Succeeded) {
                    Connection.restart();
                    info();
                    modal.close();
                } else {
                    while (registrationErrors.firstChild) {
                        registrationErrors.removeChild(registrationErrors.firstChild);
                    }
                    for (var i = 0; i < request.Errors.length; i++) {
                        var errorElm = document.createElement('div');
                        errorElm.textContent = request.Errors[i];
                        registrationErrors.appendChild(errorElm);
                    }
                }
            }
        });
    }

    function login(email, password, rememberMe) {
        var request = $.ajax({
            type: 'POST',
            url: '/Api/Account/Login',
            data: $.param({ UserName: email, Password: password, RememberMe: rememberMe }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            success: function (request) {
                request = JSON.parse(request);

                if (request.Succeeded) {
                    Connection.restart();
                    info();
                    modal.close();
                } else {
                    while (loginErrors.firstChild) {
                        loginErrors.removeChild(loginErrors.firstChild);
                    }
                    for (var i = 0; i < request.Errors.length; i++) {
                        var errorElm = document.createElement('div');
                        errorElm.textContent = request.Errors[i];
                        loginErrors.appendChild(errorElm);
                    }
                }
            }
        });
    }

    function logoff() {
        var request = $.ajax({
            type: 'POST',
            url: '/Api/Account/LogOff',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            success: function (request) {
                console.log(request);
                Connection.restart();
                info();
                location.reload();
            }
        });
    }

    function info() {
        var request = $.ajax({
            type: 'POST',
            url: '/Api/Account/Info',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            success: function (request) {
                request = JSON.parse(request);
                updateUser(request.UserName, request.Anonymous);
            }
        });
    }
    Account.info = info;
})(Account || (Account = {}));
var Rock;
(function (Rock) {
    var canvas = document.getElementById('rock');
    var particleCanvas = document.getElementById('particles');
    var context = canvas.getContext('2d');
    var particleContext = particleCanvas.getContext('2d');
    var relativeRockURL = '/Content/Rock.png';
    var relativeStoneURL = '/Content/Stone.png';
    var rockImage = new Image();
    var stoneImage = new Image(16, 16);
    var stoneLoaded = false;
    var lastX = 0;
    var lastY = 0;
    var rockSize = 64;
    var rockGrowth = 4;
    var rockIsBig = false;
    var mouseDown = false;

    var notTouched = true;
    var growing = true;
    var currentNotifier = 0;
    var growRate = 8;
    var notifierGrowth = 4;
    var gsLastTick = Date.now();

    Rock.particles = new Array();

    var Particle = (function () {
        function Particle(x, y) {
            this.width = 5;
            this.height = 5;
            this.dispose = false;
            this.x = x;
            this.y = y;
            this.verticalVelocity = Utils.getRandomInt(-100, -155);
            this.horizonalVelocity = Utils.getRandomInt(-50, 50);
            this.width = Utils.getRandomInt(3, 6);
            this.height = Utils.getRandomInt(3, 6);
            this.rotation = Utils.getRandomInt(0, 180);
            this.rotationalVelocity = Utils.getRandomInt(-75, 75);

            this.lastTick = Date.now();
        }
        Particle.prototype.update = function () {
            var timeSinceLastTick = Date.now() - this.lastTick;
            this.lastTick = Date.now();
            timeSinceLastTick /= 1000;

            this.rotation += (this.rotationalVelocity * timeSinceLastTick);
            this.y += (this.verticalVelocity * timeSinceLastTick);
            this.x += (this.horizonalVelocity * timeSinceLastTick);
            this.verticalVelocity += (200 * timeSinceLastTick);
            if (this.y > 270) {
                this.dispose = true;
            }
        };
        return Particle;
    })();
    Rock.Particle = Particle;

    function initialize() {
        rockImage.onload = function () {
            drawBackground();
            console.log('rock loaded');
        };
        rockImage.src = relativeRockURL;

        stoneImage.onload = function () {
            stoneLoaded = true;
        };
        stoneImage.src = relativeStoneURL;

        canvas.addEventListener('mousemove', function (e) {
            var mousePos = getMousePos(canvas, e);
            isOverRock(mousePos.x, mousePos.y);
            //console.log('x: ' + mousePos.x + ' y: ' + mousePos.y);
        }, false);
        canvas.addEventListener('mousedown', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = true;
            isOverRock(mousePos.x, mousePos.y);
            Connection.mine(mousePos.x, mousePos.y);
        }, false);
        canvas.addEventListener('mouseup', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = false;
            isOverRock(mousePos.x, mousePos.y);
        }, false);
        canvas.addEventListener('mouseleave', function (e) {
            var mousePos = getMousePos(canvas, e);
            mouseDown = false;
            isOverRock(mousePos.x, mousePos.y);
        }, false);
    }

    function growAndShrink() {
        if (!stoneLoaded) {
            setTimeout(growAndShrink, 10);
            return;
        }

        var time = Date.now();
        var timePassed = time - gsLastTick;
        timePassed /= 1000;
        gsLastTick = time;

        if (growing) {
            currentNotifier += growRate * timePassed;
        } else {
            currentNotifier -= growRate * timePassed;
        }

        if (currentNotifier > notifierGrowth || currentNotifier < -notifierGrowth) {
            growing = !growing;
        }

        drawRock(lastX, lastY, rockSize + currentNotifier, rockSize + currentNotifier);

        if (notTouched)
            setTimeout(growAndShrink, 10);
    }
    growAndShrink();

    function getMousePos(canvas, evt) {
        var rect = canvas.getBoundingClientRect();
        return {
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        };
    }

    var released = true;

    function isOverRock(x, y) {
        if (x > lastX && x < (lastX + rockSize) && y > lastY && y < (lastY + rockSize)) {
            if (!mouseDown) {
                drawRock(lastX - (rockGrowth / 2), lastY - (rockGrowth / 2), rockSize + rockGrowth, rockSize + rockGrowth);
                released = true;
                notTouched = false;
            } else {
                drawRock(lastX + (rockGrowth / 2), lastY + (rockGrowth / 2), rockSize - rockGrowth, rockSize - rockGrowth);
                if (released)
                    addParticles(x, y);
                released = false;
                notTouched = false;
            }
            rockIsBig = true;
        } else if (rockIsBig) {
            drawRock(lastX, lastY, rockSize, rockSize);
            rockIsBig = false;
            released = true;
        }
    }

    function moveRock(x, y) {
        if (x != lastX && y != lastY) {
            lastX = x;
            lastY = y;
            if (stoneLoaded)
                drawRock(x, y, rockSize, rockSize);
            else
                setTimeout(function () {
                    moveRock(x, y);
                }, 10);
        }
    }
    Rock.moveRock = moveRock;

    function clearCanvas() {
        context.clearRect(0, 0, 250, 250);
    }

    function drawBackground() {
        context.drawImage(rockImage, 0, 0);
    }

    function addParticles(x, y) {
        var rand = Utils.getRandomInt(1, 3);
        for (var i = 0; i < rand; i++) {
            var xOffset = Utils.getRandomInt(-5, 5);
            var yOffset = Utils.getRandomInt(-2, 2);
            Rock.particles.push(new Particle(x + xOffset, y + yOffset));
        }
    }

    var particleCache = document.createElement('canvas');
    var cacheCtx = particleCache.getContext('2d');

    function drawParticle(particle) {
        particleCache.width = particle.width;
        particleCache.height = particle.height;
        cacheCtx.rect(0, 0, particle.width, particle.height);
        cacheCtx.fillStyle = 'gray';
        cacheCtx.fill();
        cacheCtx.stroke();

        particleContext.beginPath();

        drawImageRot(particleContext, particleCache, particle.x, particle.y, particle.width, particle.height, particle.rotation);
    }

    function drawImageRot(ctx, img, x, y, width, height, deg) {
        //Convert degrees to radian
        var rad = deg * Math.PI / 180;

        //Set the origin to the center of the image
        ctx.translate(x + width / 2, y + height / 2);

        //Rotate the canvas around the origin
        ctx.rotate(rad);

        //draw the image
        ctx.drawImage(img, width / 2 * (-1), height / 2 * (-1), width, height);

        //reset the canvas
        ctx.rotate(rad * (-1));
        ctx.translate((x + width / 2) * (-1), (y + height / 2) * (-1));
    }

    function updateParticles() {
        particleContext.clearRect(0, 0, 250, 250);

        for (var i = 0; i < Rock.particles.length; i++) {
            var particle = Rock.particles[i];
            particle.update();
            drawParticle(particle);

            if (particle.dispose) {
                Rock.particles.splice(i, 1);
            }
        }
    }
    setInterval(updateParticles, 10);

    function drawRock(x, y, xScale, yScale) {
        clearCanvas();
        drawBackground();
        context.drawImage(stoneImage, x, y, xScale, yScale);
        //context.drawImage(stoneImage, x, y);
    }

    initialize();
})(Rock || (Rock = {}));
var Tabs;
(function (Tabs) {
    var lowestTabContainerId = 0;
    var tabContainer = document.getElementById("paneContainer");
    Tabs.bottomPadding = 200;
    var tabContainers = new Array();
    var selectedTab;

    var TabContainer = (function () {
        function TabContainer(container) {
            this.container = container;
            this.tabs = new Array();
            this.lowestId = 0;
            this.id = lowestTabContainerId++;
            tabContainers.push(this);
        }
        TabContainer.prototype.newTab = function (pane) {
            var tab = new Tab();
            tab.pane = pane;
            var button = document.createElement('DIV');
            button.classList.add('tab-button');
            this.container.appendChild(button);
            tab.button = button;

            if (this.lowestId == 0) {
                tab.activate();
            } else {
                tab.deactivate();
            }

            // IDs are incremented here. to get their initial value we must subtract.
            var id = this.lowestId++;
            var contId = this.id;

            button.addEventListener('click', function () {
                Tabs.activateTab(contId, id);
            });

            this.tabs.push(tab);
            this.container.appendChild(button);

            return this.lowestId - 1;
        };

        TabContainer.prototype.activate = function (id) {
            for (var i = 0; i < this.tabs.length; i++) {
                this.tabs[i].deactivate();
            }
            this.tabs[id].activate();
        };

        TabContainer.prototype.css = function (id, className) {
            this.tabs[id].button.classList.add(className);
        };
        return TabContainer;
    })();

    var gameTabs = new TabContainer(document.getElementById('tabContainer'));

    function registerGameTab(pane, css) {
        var id = gameTabs.newTab(pane);
        if (css)
            gameTabs.css(id, css);
    }
    Tabs.registerGameTab = registerGameTab;

    function updateGameTabs() {
        if (selectedTab) {
            var height = selectedTab.scrollHeight;
            if (height > window.innerHeight - Tabs.bottomPadding) {
                height = window.innerHeight - Tabs.bottomPadding;
            }
            tabContainer.style.minHeight = height + 'px';
            tabContainer.style.maxHeight = height + 'px';
            tabContainer.style.overflowY = height >= window.innerHeight - Tabs.bottomPadding ? 'scroll' : 'hidden';
        }
    }
    Tabs.updateGameTabs = updateGameTabs;

    Utils.addEvent(window, 'resize', Tabs.updateGameTabs);
    setInterval(updateGameTabs, 20);

    function activateTab(containerId, tabId) {
        tabContainers[containerId].activate(tabId);
        updateGameTabs();
    }
    Tabs.activateTab = activateTab;

    var Tab = (function () {
        function Tab() {
        }
        Tab.prototype.deactivate = function () {
            Utils.cssSwap(this.button, 'active', 'inactive');
            this.pane.style.display = 'none';
            this.pane.style.overflow = 'hidden';
        };

        Tab.prototype.activate = function () {
            Utils.cssSwap(this.button, 'inactive', 'active');
            this.pane.style.display = 'block';
            this.pane.style.overflow = 'visible';
            selectedTab = this.pane;
        };
        return Tab;
    })();
})(Tabs || (Tabs = {}));
//<reference path="connection.ts"/>
var Chat;
(function (Chat) {
    var chatWindow;
    var chatLogContainer;
    var debugLogContainer;
    function initialize() {
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
                } else {
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
            debugTab.addEventListener('click', function () {
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
    Chat.initialize = initialize;
    initialize();

    function sendGlobalMessagePress() {
        Connection.sendGlobalMessage(document.getElementById('chattext').value);
        document.getElementById('chattext').value = '';
    }

    function log(message) {
        var debugLog = document.getElementById('debuglog');
        var debugItem = document.createElement('DIV');
        debugItem.textContent = message;
        debugLog.appendChild(debugItem);
    }
    Chat.log = log;

    function receiveGlobalMessage(sender, message, time, perms) {
        var chatLog = document.getElementById('chatlog');

        var difference = chatLog.scrollTop - (chatLog.scrollHeight - chatLog.offsetHeight);
        var scrolledDown = Math.abs(difference) < 5;

        var chatItem = document.createElement('DIV');
        chatItem.classList.add('chat-msg');
        if (perms && perms != '')
            chatItem.classList.add('chat-' + perms);
        var timeSpan = document.createElement('SPAN');
        timeSpan.textContent = '[' + Utils.convertServerTimeToLocal(time) + '] ';
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

        if (scrolledDown)
            chatLog.scrollTop = chatLog.scrollHeight;
    }
    Chat.receiveGlobalMessage = receiveGlobalMessage;
})(Chat || (Chat = {}));
var tooltip;
(function (_tooltip) {
    var registeredTooltips = 0;
    var tooltips = new Array();
    var activeTooltipId;
    var activeTooltip;

    var intervalId;
    var appearDelay = 0.25;
    var currentDelay = 0;
    var x;
    var y;

    var focusedElement;

    var Tooltip = (function () {
        function Tooltip() {
            this.html = document.createElement("div");
            this.html.classList.add('tooltip-wrapper');
        }
        Object.defineProperty(Tooltip.prototype, "header", {
            get: function () {
                return this.html.getElementsByClassName('tooltip-header')[0];
            },
            set: function (html) {
                html.classList.add('tooltip-header');
                this.html.appendChild(html);
            },
            enumerable: true,
            configurable: true
        });


        Object.defineProperty(Tooltip.prototype, "content", {
            get: function () {
                return this.html.getElementsByClassName('tooltip-content')[0];
            },
            set: function (html) {
                html.classList.add('tooltip-content');
                this.html.appendChild(html);
            },
            enumerable: true,
            configurable: true
        });

        return Tooltip;
    })();

    function show(id, x, y) {
        var tooltip = tooltips[id];
        if (activeTooltipId !== id) {
            hide();
        }
        activeTooltipId = id;
        activeTooltip = tooltip.html;
        document.body.appendChild(activeTooltip);
        move(x, y);
    }

    function move(x, y) {
        var rect = activeTooltip.getBoundingClientRect();
        var length = rect.right - rect.left;
        var height = rect.bottom - rect.top;
        if (length + (x + 15) > window.innerWidth)
            activeTooltip.style.left = ((x - length) - 15) + "px";
        else
            activeTooltip.style.left = (x + 15) + "px";

        if ((y - height) > 0)
            activeTooltip.style.top = (y - height) + "px";
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

    function complexModify(id, content, title) {
        var tt = tooltips[id];
        if (title) {
            tt.header.parentElement.removeChild(tt.header);
            tt.header = title;
        }
        tt.content.parentElement.removeChild(tt.content);
        tt.content = content;
    }
    _tooltip.complexModify = complexModify;

    function modify(id, content, title) {
        var tt = tooltips[id];
        if (title)
            tt.header.textContent = title;

        tt.content.textContent = content;
    }
    _tooltip.modify = modify;

    function retrieveContent(id) {
        return tooltips[id].content;
    }
    _tooltip.retrieveContent = retrieveContent;

    function complexCreate(element, content, title) {
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
    _tooltip.complexCreate = complexCreate;

    function create(element, content, title) {
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
    _tooltip.create = create;

    function addListeners(element) {
        element.onmouseenter = function (e) {
            var id = +e.target.getAttribute('data-tooltip');
            if (focusedElement !== e.target) {
                intervalId = setInterval(function () {
                    currentDelay += 0.01;
                    if (currentDelay >= appearDelay) {
                        show(id, x, y);
                        currentDelay = 0;
                        clearInterval(intervalId);
                        intervalId = null;
                    }
                }, 10);
            }
            focusedElement = e.target;
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

    function mousePosition(e) {
        if (e.pageX || e.pageY) {
            return { x: e.pageX, y: e.pageY };
        } else if (e.clientX || e.clientY) {
            return {
                x: e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft,
                y: e.clientY + document.body.scrollTop + document.documentElement.scrollTop
            };
        }
    }
})(tooltip || (tooltip = {}));
///<reference path="utils.ts"/>
///<reference path="tooltip.ts"/>
///<reference path="tabs.ts"/>
///<reference path="objects.ts"/>
var Inventory;
(function (Inventory) {
    Inventory.items = new Array();
    Inventory.configClickers = new Array();
    var inventoryPane;
    var inventory;
    var selectedItemPane;
    var selectedItemImage;
    var selectedItem;

    //var configDiv;
    var configTableBody;
    var configTableContainer;
    var drinkButton;

    var configNames = new Array();
    var configImages = new Array();

    var Item = (function () {
        function Item(id, name, worth, category) {
            this.id = id;
            this.name = name;
            this.worth = worth;
            this.category = category;
        }
        return Item;
    })();
    Inventory.Item = Item;

    (function (Category) {
        Category[Category["NFS"] = 0] = "NFS";
        Category[Category["ORE"] = 1] = "ORE";
        Category[Category["GEM"] = 2] = "GEM";
        Category[Category["INGREDIENT"] = 3] = "INGREDIENT";
        Category[Category["CRAFTING"] = 4] = "CRAFTING";
        Category[Category["POTION"] = 5] = "POTION";
    })(Inventory.Category || (Inventory.Category = {}));
    var Category = Inventory.Category;
    ;

    function getSelectedItemQuantity() {
        return selectedItem ? selectedItem.quantity : 0;
    }
    Inventory.getSelectedItemQuantity = getSelectedItemQuantity;

    function selectItem(id) {
        if (id) {
            if (selectedItem != null) {
                selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            }
            selectedItem = Inventory.items[id];
            selectedItemImage.classList.add(Utils.cssifyName(selectedItem.name));
            if (selectedItem.category == 5 /* POTION */) {
                drinkButton.style.display = 'inline-block';
            } else {
                drinkButton.style.display = 'none';
            }
            limitTextQuantity();
        } else {
            selectedItemImage.classList.remove(Utils.cssifyName(selectedItem.name));
            selectedItem = null;
        }

        selectedItemPane.style.display = selectedItem == null ? 'none' : 'block';
    }
    Inventory.selectItem = selectItem;

    function sellSelectedItem(quantity) {
        Connection.sellItem(selectedItem.id, quantity ? quantity : 1);
    }
    Inventory.sellSelectedItem = sellSelectedItem;

    function sellAllSelectedItem() {
        sellSelectedItem(selectedItem.quantity);
    }
    Inventory.sellAllSelectedItem = sellAllSelectedItem;

    function limitTextQuantity() {
        var textbox = document.getElementById('selecteditemquantity');
        var quantity = +textbox.value;
        if (Utils.isNumber(quantity)) {
            if (quantity > Inventory.getSelectedItemQuantity()) {
                textbox.value = Inventory.getSelectedItemQuantity().toString();
            }
        }
    }

    function add(item) {
        Inventory.items[item.id] = item;
        Objects.register(item.id, item.name);

        if (!inventoryPane)
            draw();

        if (item.category != 0 /* NFS */ && item.category != null)
            inventory.appendChild(drawItem(item));
        else
            document.getElementById('headerInventory').appendChild(drawItem(item));
    }

    function draw() {
        // SELECTED ITEM HEADER
        inventoryPane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(inventoryPane);
        selectedItemPane = document.createElement('DIV');
        selectedItemPane.classList.add('selected-item');
        selectedItemPane.style.display = 'none';
        var selectedItemPaneCloser = document.createElement('SPAN');
        selectedItemPaneCloser.textContent = 'x';
        selectedItemPaneCloser.style.top = '0px';
        selectedItemPaneCloser.style.right = '3px';
        selectedItemPaneCloser.style.position = 'absolute';
        selectedItemPaneCloser.addEventListener('click', function () {
            Inventory.selectItem();
        });
        selectedItemPane.appendChild(selectedItemPaneCloser);

        selectedItemImage = document.createElement('DIV');
        selectedItemImage.classList.add('selected-item-image');
        var selectedItemQuantity = document.createElement('INPUT');
        selectedItemQuantity.id = 'selecteditemquantity';
        selectedItemQuantity.type = 'text';
        selectedItemQuantity.style.height = '18px';
        selectedItemQuantity.classList.add('selected-item-quantity');
        selectedItemQuantity.addEventListener('input', function () {
            limitTextQuantity();
        });

        drinkButton = Utils.createButton('Drink', '');
        drinkButton.classList.add('selected-item-quantity');
        drinkButton.addEventListener('click', function () {
            Connection.drink(selectedItem.id);
            limitTextQuantity();
        });

        var sellItems = Utils.createButton('Sell', '');
        sellItems.classList.add('selected-item-quantity');
        sellItems.addEventListener('click', function () {
            var textbox = document.getElementById('selecteditemquantity');
            var quantity = +textbox.value;
            if (Utils.isNumber(quantity)) {
                Inventory.sellSelectedItem(quantity);
            }
            limitTextQuantity();
        });

        var sellAllItems = Utils.createButton('Sell all', '');
        sellAllItems.classList.add('selected-item-quantity');
        sellAllItems.addEventListener('click', function () {
            Inventory.sellAllSelectedItem();
            limitTextQuantity();
        });

        selectedItemPane.appendChild(selectedItemImage);
        selectedItemPane.appendChild(sellAllItems);

        selectedItemPane.appendChild(sellItems);
        selectedItemPane.appendChild(drinkButton);
        selectedItemPane.appendChild(selectedItemQuantity);
        inventoryPane.appendChild(selectedItemPane);

        // CONFIG CONTROLS
        var configDiv = document.createElement('DIV');
        configDiv.style.textAlign = 'center';
        var configPanel = document.createElement('DIV');
        configPanel.style.display = 'inline-block';
        var sellAll = Utils.createButton('Sell (...)', '');
        sellAll.addEventListener('click', function () {
            Connection.sellAllItems();
        });
        var sellAllConfig = Utils.createButton('...', '');
        sellAllConfig.addEventListener('click', function () {
            Inventory.toggleConfig();
        });
        configPanel.appendChild(sellAll);
        configPanel.appendChild(sellAllConfig);
        configDiv.appendChild(configPanel);
        inventoryPane.appendChild(configDiv);

        // CONFIG TABLE
        configTableContainer = document.createElement('DIV');
        configTableContainer.classList.add('config-container');
        configTableContainer.classList.add('closed');
        var configTable = document.createElement('TABLE');
        configTable.classList.add('config-table');
        configTableContainer.appendChild(configTable);

        var header = configTable.createTHead();
        var titleRow = header.insertRow(0);
        var realTitleRow = header.insertRow(0);
        realTitleRow.classList.add('table-header');
        var titleCell = realTitleRow.insertCell(0);
        titleCell.textContent = 'Inventory Configuration';
        titleCell.setAttribute('colspan', '10');
        titleRow.classList.add('table-subheader');

        for (var enumMember in Category) {
            var isValueProperty = parseInt(enumMember, 10) >= 0;
            if (isValueProperty) {
                if (Category[enumMember] != "NFS") {
                    var configCell = titleRow.insertCell(titleRow.cells.length);
                    configCell.classList.add('config-cell-check');

                    var titleCell = titleRow.insertCell(titleRow.cells.length);
                    titleCell.classList.add('config-cell-name');
                    titleCell.textContent = Category[enumMember];
                }
            }
        }
        configTableBody = configTable.createTBody();

        inventory = document.createElement('DIV');
        inventory.style.position = 'relative';
        inventory.appendChild(configTableContainer);
        inventoryPane.appendChild(inventory);

        Tabs.registerGameTab(inventoryPane, 'Inventory');
        Equipment.draw();
    }

    function modifyConfig(id, enabled) {
        if (!Inventory.configClickers[id])
            console.log(id);
        Inventory.configClickers[id].checked = enabled;
    }
    Inventory.modifyConfig = modifyConfig;

    function toggleConfig() {
        if (configTableContainer.classList.contains('closed')) {
            configTableContainer.classList.remove('closed');
        } else
            configTableContainer.classList.add('closed');
    }
    Inventory.toggleConfig = toggleConfig;

    function drawHeaderItem(item) {
        var itemElement = document.createElement('DIV');
        item.container = itemElement;

        itemElement.classList.add('header-item');

        var itemImage = document.createElement('DIV');
        itemImage.style.width = '32px';
        itemImage.style.height = '32px';
        itemImage.style.display = 'inline-block';
        itemImage.style.position = 'relative';
        itemImage.style.margin = '0 auto';
        var image = document.createElement('DIV');
        image.classList.add('Half-' + item.name.replace(' ', '_'));
        itemImage.appendChild(image);
        itemElement.appendChild(itemImage);

        var itemQuantity = document.createElement('DIV');
        item.quantityElm = itemQuantity;
        itemQuantity.classList.add("item-text");
        itemQuantity.textContent = Utils.formatNumber(0);
        itemQuantity.style.verticalAlign = 'top';
        itemQuantity.style.marginTop = '8px';
        itemQuantity.style.display = 'inline-block';
        itemElement.appendChild(itemQuantity);

        var itemValue = document.createElement('DIV');
        itemValue.style.display = 'none';
        item.worthElm = itemValue;
        itemElement.appendChild(itemValue);

        return itemElement;
    }

    function drawItem(item) {
        if (item.category == 0 /* NFS */ || item.category == null)
            return drawHeaderItem(item);

        var itemElement = document.createElement('DIV');
        item.container = itemElement;
        itemElement.classList.add("item");
        itemElement.addEventListener('click', function () {
            Inventory.selectItem(item.id);
        });
        tooltip.create(itemElement, item.name);

        // VALUE
        var itemValueContainer = document.createElement('DIV');
        itemValueContainer.classList.add("item-text");
        var itemValue = document.createElement('DIV');
        itemValue.style.verticalAlign = 'top';
        itemValue.style.display = 'inline-block';
        itemValue.textContent = Utils.formatNumber(item.worth);
        item.worthElm = itemValue;
        var itemCurrency = document.createElement('DIV');
        itemCurrency.classList.add('Quarter-Coins');
        itemCurrency.style.display = 'inline-block';
        itemValueContainer.appendChild(itemCurrency);
        itemValueContainer.appendChild(itemValue);
        itemElement.appendChild(itemValueContainer);

        // IMAGE
        var itemImage = document.createElement('DIV');
        itemImage.style.width = '64px';
        itemImage.style.height = '64px';
        itemImage.style.position = 'relative';
        itemImage.style.margin = '0 auto';
        var image = document.createElement('DIV');
        image.classList.add(item.name.replace(' ', '_'));
        itemImage.appendChild(image);
        itemElement.appendChild(itemImage);

        // QUANTITY
        var itemQuantity = document.createElement('DIV');
        item.quantityElm = itemQuantity;
        itemQuantity.classList.add("item-text");
        itemQuantity.textContent = Utils.formatNumber(0);
        itemElement.appendChild(itemQuantity);

        // CONFIG TABLE
        if (item.category != null) {
            var selectedItemCell;
            var selectedConfigCell;
            var rows = configTableBody.rows.length;
            var cellIndex = item.category;
            cellIndex *= 2;
            cellIndex--;

            for (var i = 0; i < rows; i++) {
                // FIX THIS.
                var testCell = configTableBody.rows[i].cells[cellIndex];
                if (testCell && testCell.childElementCount == 0) {
                    selectedItemCell = configTableBody.rows[i].cells[cellIndex];
                    selectedConfigCell = configTableBody.rows[i].cells[cellIndex - 1];
                    break;
                }
            }
            if (!selectedItemCell) {
                var row = configTableBody.insertRow(configTableBody.rows.length);
                row.classList.add('table-row');
                for (var enumMember in Category) {
                    var isValueProperty = parseInt(enumMember, 10) >= 0;
                    if (isValueProperty) {
                        if (Category[enumMember] != "NFS") {
                            var configCell = row.insertCell(row.cells.length);
                            configCell.classList.add('config-cell-check');

                            var titleCell = row.insertCell(row.cells.length);
                            titleCell.classList.add('config-cell-name');
                        }
                    }
                }
                selectedItemCell = row.cells[cellIndex];
                selectedConfigCell = row.cells[cellIndex - 1];
            }

            /* For doing stuff with empty cells.
            for (var curRow = 0; curRow < rows; curRow++) {
            var inspectedRow = (<HTMLTableElement>configTableBody).rows[i];
            var cells = (<HTMLTableElement>inspectedRow).cells.length;
            for (var curCell = 0; curCell < cells; curCell++) {
            
            }
            }*/
            var nameAndImage = document.createElement('DIV');
            nameAndImage.classList.add('item-text');
            nameAndImage.style.height = 'auto';
            var nameSpan = document.createElement('SPAN');
            nameSpan.style.verticalAlign = 'top';
            var image = document.createElement('DIV');
            image.classList.add('Third-' + Utils.cssifyName(item.name));
            image.style.display = 'inline-block';
            configImages[item.id] = image;
            nameSpan.textContent = item.name;
            configNames[item.id] = nameSpan;
            nameAndImage.appendChild(image);
            nameAndImage.appendChild(nameSpan);
            selectedItemCell.appendChild(nameAndImage);
            var configChecker = document.createElement('INPUT');
            configChecker.type = 'CHECKBOX';
            var id = item.id;
            Inventory.configClickers[id] = configChecker;
            configChecker.addEventListener('change', function (e) {
                Connection.configureItem(id, Inventory.configClickers[id].checked);
            });
            selectedConfigCell.appendChild(configChecker);
        }

        return itemElement;
    }

    function addItem(id, name, worth, category) {
        if (!Inventory.items[id])
            add(new Item(id, name, worth, category));
    }
    Inventory.addItem = addItem;

    function changePrice(id, price) {
        Utils.ifNotDefault(price, function () {
            var item = Inventory.items[id];
            item.worth = price;
            item.worthElm.textContent = Utils.formatNumber(price);
        });
    }
    Inventory.changePrice = changePrice;

    function changeQuantity(id, quantity) {
        Utils.ifNotDefault(quantity, function () {
            Objects.setQuantity(id, quantity);
            Crafting.update();
            Inventory.items[id].quantityElm.textContent = Utils.formatNumber(quantity);
            Inventory.items[id].quantity = quantity;
            if (Inventory.items[id].category != 0 /* NFS */ && Inventory.items[id].category != null)
                Inventory.items[id].container.style.display = quantity == 0 ? 'none' : 'inline-block';
            else
                Inventory.items[id].container.style.display = Objects.getLifeTimeTotal(id) == 0 ? 'none' : 'inline-block';
            limitTextQuantity();
        });
    }
    Inventory.changeQuantity = changeQuantity;

    function update() {
        if (configNames.length <= 0)
            return;

        Inventory.items.forEach(function (item) {
            var itemQuantity = Objects.getLifeTimeTotal(item.id);
            if (configNames[item.id])
                configNames[item.id].textContent = itemQuantity > 0 ? item.name : '???';

            if (configImages[item.id])
                configImages[item.id].style.display = itemQuantity > 0 ? 'inline-block' : 'none';
        });
    }
    Inventory.update = update;
})(Inventory || (Inventory = {}));
var Statistics;
(function (Statistics) {
    var statsPane;
    var itemsBody;
    var items = new Array();
    var Item = (function () {
        function Item() {
        }
        return Item;
    })();

    function changeStats(id, prestige, lifetime) {
        var item = items[id];

        Utils.ifNotDefault(prestige, function () {
            item.prestigeQuantity = prestige;
            item.prestigeRow.textContent = Utils.formatNumber(prestige);
        });

        Utils.ifNotDefault(lifetime, function () {
            item.lifetimeQuantity = lifetime;
            Objects.setLifeTimeTotal(id, lifetime);
            item.alltimeRow.textContent = Utils.formatNumber(lifetime);
        });
    }
    Statistics.changeStats = changeStats;

    function addItem(id, name) {
        if (!statsPane)
            draw();

        if (!items[id]) {
            var item = new Item();
            items[id] = item;

            var row = itemsBody.insertRow(itemsBody.rows.length);
            row.classList.add('table-row');
            item.alltimeRow = row.insertCell(0);
            item.alltimeRow.style.width = '40%';
            item.prestigeRow = row.insertCell(0);
            item.prestigeRow.style.width = '40%';
            var imageRow = row.insertCell(0);
            imageRow.style.width = '20%';
            var image = document.createElement('DIV');
            image.classList.add('Third-' + Utils.cssifyName(name));
            image.style.display = 'inline-block';
            imageRow.appendChild(image);
        }
    }
    Statistics.addItem = addItem;

    function draw() {
        statsPane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(statsPane);
        Tabs.registerGameTab(statsPane, 'Statistics');

        statsPane.appendChild(drawItemsTable());
    }

    function drawItemsTable() {
        var itemsTable = document.createElement('TABLE');

        var header = itemsTable.createTHead();
        var titleRow = header.insertRow(0);
        titleRow.classList.add('table-header');
        var titleCell = titleRow.insertCell(0);
        titleCell.textContent = 'Item Statistics';
        titleCell.setAttribute('colspan', '3');

        var descriptionsRow = header.insertRow(1);
        descriptionsRow.classList.add('table-subheader');
        var lifetime = descriptionsRow.insertCell(0);
        lifetime.textContent = 'Lifetime Quantity';
        lifetime.style.width = '40%';
        var prestige = descriptionsRow.insertCell(0);
        prestige.textContent = 'Prestige Quantity';
        prestige.style.width = '40%';
        var item = descriptionsRow.insertCell(0);
        item.textContent = 'Item';
        item.style.width = '20%';

        itemsBody = itemsTable.createTBody();

        return itemsTable;
    }
})(Statistics || (Statistics = {}));
var Store;
(function (Store) {
    var storePane;
    var items = new Array();
    var categories = new Array();
    (function (Category) {
        Category[Category["MINING"] = 1] = "MINING";
        Category[Category["MACHINES"] = 2] = "MACHINES";
        Category[Category["GATHERING"] = 3] = "GATHERING";
        Category[Category["PROCESSING"] = 4] = "PROCESSING";
        Category[Category["ITEMS"] = 5] = "ITEMS";
        Category[Category["CRAFTING"] = 6] = "CRAFTING";
    })(Store.Category || (Store.Category = {}));
    var Category = Store.Category;
    ;

    var StoreItem = (function () {
        function StoreItem() {
        }
        return StoreItem;
    })();

    function draw() {
        storePane = document.createElement('div');
        document.getElementById('paneContainer').appendChild(storePane);
        Tabs.registerGameTab(storePane, 'Store');

        for (var enumMember in Category) {
            var isValueProperty = parseInt(enumMember, 10) >= 0;
            if (isValueProperty) {
                var name = Category[enumMember];
                if (name != "CRAFTING") {
                    drawCategory(name);
                }
            }
        }
    }

    function drawCategory(name) {
        var categoryContainer = document.createElement('div');
        categoryContainer.classList.add('store-category');
        categories[name] = categoryContainer;

        var categoryHeader = document.createElement('div');
        categoryHeader.textContent = name;
        categoryHeader.classList.add('store-category-header');
        categoryContainer.appendChild(categoryHeader);
        storePane.appendChild(categoryContainer);
    }

    function tempFix() {
        draw();
    }
    Store.tempFix = tempFix;

    function addItem(id, category, price, factor, name, maxQuantity, tooltip) {
        if (!storePane)
            draw();

        if (!items[id]) {
            Objects.register(id, name);

            var item = new StoreItem();
            item.id = id;
            item.category = category;
            item.price = price;
            item.factor = factor;
            item.name = name;
            item.tooltip = tooltip;
            item.maxQuantity = maxQuantity ? maxQuantity : 0;

            Objects.register(item.id, item.name);
            Objects.setTooltip(item.id, item.tooltip);

            if (item.category == 2 /* MACHINES */) {
                Equipment.registerGatherer(item.id);
            }

            var categoryContainer = categories[Category[category]];
            if (categoryContainer == null) {
                categoryContainer = categories["MINING"];
            }
            if (item.category != 6 /* CRAFTING */)
                categoryContainer.appendChild(drawItem(item));

            items[id] = item;
        }
    }
    Store.addItem = addItem;

    function changeQuantity(id, quantity, maxQuantity, price) {
        var item = items[id];

        Utils.ifNotDefault(maxQuantity, function () {
            if (maxQuantity != 0) {
                try  {
                    Equipment.changeMaxQuantity(id, maxQuantity);
                    item.maxQuantity = maxQuantity;
                    item.maxQuantityElm.textContent = maxQuantity.toString();
                } catch (err) {
                }
            }
        });

        Utils.ifNotDefault(quantity, function () {
            Objects.setQuantity(id, quantity);
            item.quantity = quantity;
            Crafting.update();
            Equipment.changeQuantity(id, quantity);

            if (maxQuantity != 0) {
                if (item.category == 6 /* CRAFTING */)
                    return;

                item.container.style.display = (item.quantity <= -1 || item.quantity >= item.maxQuantity && item.maxQuantity > 0) ? 'none' : 'inline-block';
                if (item.maxQuantity && item.maxQuantity > 1) {
                    item.quantityElm.textContent = quantity.toString();
                }
            }
        });

        Utils.ifNotDefault(price, function () {
            if (item.priceElm) {
                item.price = price;
                item.priceElm.textContent = Utils.formatNumber(price);
            }
        });
    }
    Store.changeQuantity = changeQuantity;

    function add() {
    }

    function drawItem(item) {
        var itemContainer = document.createElement('div');
        if (item.tooltip != "undefined") {
            tooltip.create(itemContainer, item.tooltip);
        }
        itemContainer.classList.add('store-item');
        item.container = itemContainer;
        var header = document.createElement('div');
        header.classList.add('store-item-header');
        var headerText = document.createElement('div');
        headerText.classList.add('store-item-header-text');
        headerText.textContent = item.name;
        header.appendChild(headerText);

        var headerQuantityContainer = document.createElement('div');
        headerQuantityContainer.classList.add('store-item-header-quantity-container');

        var headerMaxQuantity = document.createElement('span');
        headerMaxQuantity.classList.add('store-item-header-quantity');
        var headerQuantity = document.createElement('span');
        headerQuantity.classList.add('store-item-header-quantity');
        var divider = document.createElement('span');
        divider.classList.add('store-item-header-quantity');
        divider.textContent = '/';

        if (item.maxQuantity > 1) {
            headerQuantityContainer.appendChild(headerQuantity);
            headerQuantityContainer.appendChild(divider);
            headerQuantityContainer.appendChild(headerMaxQuantity);
        }
        header.appendChild(headerQuantityContainer);

        item.maxQuantityElm = headerMaxQuantity;
        item.quantityElm = headerQuantity;

        item.nameElm = headerText;
        itemContainer.appendChild(header);

        // IMAGE
        var itemImage = document.createElement('DIV');
        itemImage.style.width = '64px';
        itemImage.style.height = '64px';
        itemImage.style.position = 'relative';
        itemImage.style.margin = '0 auto';
        var image = document.createElement('DIV');
        image.classList.add(Utils.cssifyName(item.name));
        itemImage.appendChild(image);
        itemContainer.appendChild(itemImage);

        var footer = document.createElement('div');
        footer.classList.add('store-item-footer');

        var priceContainer = document.createElement('div');
        priceContainer.classList.add('item-text');
        var price = document.createElement('div');
        price.textContent = Utils.formatNumber(item.price);
        price.style.display = 'inline-block';
        price.style.verticalAlign = 'top';
        item.priceElm = price;
        var coins = document.createElement('div');
        coins.classList.add('Quarter-Coins');
        coins.style.display = 'inline-block';
        priceContainer.appendChild(coins);
        priceContainer.appendChild(price);

        var buttonContainer = document.createElement('div');
        var button = Utils.createButton('Buy', '');
        var id = item.id;
        if (item.category != 5 /* ITEMS */) {
            button.addEventListener('click', function () {
                Connection.purchaseItem(id);
            });
        } else {
            var quantityInput = document.createElement('INPUT');
            quantityInput.type = 'TEXT';
            quantityInput.style.width = '40px';
            quantityInput.style.height = '15px';
            buttonContainer.appendChild(quantityInput);
            button.addEventListener('click', function () {
                if (Utils.isNumber(quantityInput.value))
                    Connection.purchaseItem(id, +quantityInput.value);
            });
        }

        buttonContainer.appendChild(button);

        footer.appendChild(buttonContainer);
        footer.appendChild(priceContainer);

        itemContainer.appendChild(footer);

        return itemContainer;
    }
})(Store || (Store = {}));
var Equipment;
(function (Equipment) {
    var gatherers = new Array();
    var equipmentPane;
    var gathererCategory;

    var Gatherer = (function () {
        function Gatherer() {
        }
        return Gatherer;
    })();

    function draw() {
        if (equipmentPane)
            return;

        equipmentPane = document.createElement('div');
        document.getElementById('paneContainer').appendChild(equipmentPane);
        Tabs.registerGameTab(equipmentPane, 'Equipment');
    }
    Equipment.draw = draw;

    function registerGatherer(id) {
        if (gatherers[id])
            return;

        var gatherer = new Gatherer();
        gatherers[id] = gatherer;
        drawGatherer(id);
    }
    Equipment.registerGatherer = registerGatherer;

    function drawGathererCategory() {
        gathererCategory = document.createElement('div');
        gathererCategory.classList.add('store-category');
        var header = document.createElement('div');
        header.classList.add('store-category-header');
        header.textContent = 'Gatherers';
        gathererCategory.appendChild(header);
        equipmentPane.appendChild(gathererCategory);
    }

    function drawGatherer(id) {
        if (!gathererCategory)
            drawGathererCategory();

        var gatherer = gatherers[id];
        gatherer.container = document.createElement('div');
        gatherer.container.classList.add('equipment-gatherer-container');
        var header = document.createElement('div');
        header.classList.add('store-item-header');
        var headerText = document.createElement('div');
        headerText.classList.add('store-item-header-text');
        headerText.textContent = Objects.lookupName(id);
        header.appendChild(headerText);
        var toggle = Utils.createButton('Disable', '');
        toggle.addEventListener('click', function () {
            Connection.toggleGatherer(id, !gatherer.enabled);
        });
        var quantityContainer = document.createElement('div');
        quantityContainer.classList.add('store-item-header-quantity-container');

        gatherer.quantityElm = document.createElement('span');
        var dividerSpan = document.createElement('span');
        dividerSpan.textContent = '/';
        gatherer.maxQuantityElm = document.createElement('span');

        quantityContainer.appendChild(gatherer.quantityElm);
        quantityContainer.appendChild(dividerSpan);
        quantityContainer.appendChild(gatherer.maxQuantityElm);

        header.appendChild(quantityContainer);

        gatherer.container.appendChild(header);
        gatherer.efficiencyElm = document.createElement('div');
        gatherer.container.appendChild(gatherer.efficiencyElm);
        gatherer.fuelConsumptionElm = document.createElement('div');
        gatherer.container.appendChild(gatherer.fuelConsumptionElm);
        gatherer.rarityBonusElm = document.createElement('div');
        gatherer.container.appendChild(gatherer.rarityBonusElm);
        gatherer.container.appendChild(toggle);
        gatherer.toggleElm = toggle.firstChild;

        gathererCategory.appendChild(gatherer.container);
    }

    function toggleGatherer(id, enabled) {
        var gatherer = gatherers[id];
        if (!gatherer)
            return;
        gatherer.enabled = enabled;
        gatherer.toggleElm.textContent = enabled ? 'Disable' : 'Enable';
    }
    Equipment.toggleGatherer = toggleGatherer;

    function changeQuantity(id, quantity) {
        Utils.ifNotDefault(quantity, function () {
            var gatherer = gatherers[id];
            if (!gatherer)
                return;

            gatherer.quantity = quantity;
            gatherer.container.style.display = (quantity > -1) ? 'inline-block' : 'none';
            gatherer.quantityElm.textContent = Utils.formatNumber(quantity);
        });
    }
    Equipment.changeQuantity = changeQuantity;

    function changeMaxQuantity(id, maxQuantity) {
        Utils.ifNotDefault(maxQuantity, function () {
            var gatherer = gatherers[id];
            if (!gatherer)
                return;

            gatherer.maxQuantity = maxQuantity;
            gatherer.maxQuantityElm.textContent = Utils.formatNumber(maxQuantity);
        });
    }
    Equipment.changeMaxQuantity = changeMaxQuantity;

    function changeFuelConsumption(id, fuelConsumed) {
        Utils.ifNotDefault(fuelConsumed, function () {
            var gatherer = gatherers[id];
            if (!gatherer)
                return;

            gatherer.fuelConsumption = fuelConsumed;
            gatherer.fuelConsumptionElm.textContent = 'Fuel consumed: ' + Utils.formatNumber(fuelConsumed);
        });
    }
    Equipment.changeFuelConsumption = changeFuelConsumption;

    function changeEfficiency(id, efficiency) {
        Utils.ifNotDefault(efficiency, function () {
            var gatherer = gatherers[id];
            if (!gatherer)
                return;

            gatherer.efficiency = efficiency;
            gatherer.efficiencyElm.textContent = 'Resources/s: ' + Utils.formatNumber(efficiency);
        });
    }
    Equipment.changeEfficiency = changeEfficiency;

    function changeRarityBonus(id, rarityBonus) {
        Utils.ifNotDefault(rarityBonus, function () {
            var gatherer = gatherers[id];
            if (!gatherer)
                return;

            gatherer.rarityBonus = rarityBonus;
            gatherer.rarityBonusElm.textContent = 'Rarity multi: x' + Utils.formatNumber(rarityBonus);
        });
    }
    Equipment.changeRarityBonus = changeRarityBonus;
})(Equipment || (Equipment = {}));
var Crafting;
(function (Crafting) {
    var storePane;
    var processorSection;
    var craftingSection;
    var craftingTable;
    var cellDescriptions = ['Action', 'Description', 'Input', 'Output', 'Name'];
    var cellWidths = ['10%', '50%', '15%', '15%', '10%'];
    var cellMinWidths = ['170px', '0', '0', '0', '0'];

    var processorCellDescriptions = ['Action', 'Output', 'Input', 'Capacity', 'Image'];
    var processorCellWidths = ['10%', '30%', '20%', '20%', '20%'];
    var processorCellMinWidths = ['170px', '0', '0', '0', '0'];

    var itemsTableOffset = 3;
    var recipes = new Array();
    Crafting.processors = new Array();

    var sidebar = document.getElementById('sidebarInformation');
    var processorSidebar;

    var Processor = (function () {
        function Processor() {
            this._recipes = new Array();
        }
        Processor.prototype.addRecipe = function (recipe) {
            this._recipes.push(recipe);
            if (!this.selectedRecipe)
                this.selectedRecipe = 0;
        };
        return Processor;
    })();
    Crafting.Processor = Processor;

    var Recipe = (function () {
        function Recipe() {
            this.ingredients = new Array();
            this.resultants = new Array();
        }
        Recipe.prototype.addIngredient = function (ingredient) {
            this.ingredients.push(ingredient);
        };

        Recipe.prototype.addResultant = function (ingredient) {
            this.resultants.push(ingredient);
        };
        return Recipe;
    })();
    Crafting.Recipe = Recipe;

    var Ingredient = (function () {
        function Ingredient(id, quantity) {
            this.id = id;
            this.quantity = quantity;
        }
        return Ingredient;
    })();
    Crafting.Ingredient = Ingredient;

    function draw() {
        storePane = document.createElement('DIV');
        document.getElementById('paneContainer').appendChild(storePane);
        Tabs.registerGameTab(storePane, 'Crafting');

        processorSection = document.createElement('DIV');
        storePane.appendChild(processorSection);

        craftingSection = document.createElement('DIV');
        storePane.appendChild(craftingSection);
        drawCraftingTable();
    }

<<<<<<< HEAD
    Account.signedIn = false;

    var registrationErrors;
    var loginErrors;
=======
    function drawCraftingTable() {
        craftingTable = document.createElement('TABLE');
        craftingTable.classList.add('block-table');

        var header = craftingTable.createTHead();
        var titleRow = header.insertRow(0);
        titleRow.classList.add('table-subheader');
        var realTitleRow = header.insertRow(0);
        realTitleRow.classList.add('table-header');

        var titleCell = realTitleRow.insertCell(0);
        titleCell.colSpan = cellDescriptions.length;
        titleCell.textContent = 'Crafting Table';
>>>>>>> 48573a74dbde3044acffad41d7f4206ab30b244e

        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = titleRow.insertCell(0);
            cell.style.width = cellWidths[i];
            cell.textContent = cellDescriptions[i];

<<<<<<< HEAD
    var Leaderboard;
    (function (Leaderboard) {
        Leaderboard.currentlyInspecting = 0;

        function load(lower, upper, element) {
            Leaderboard.currentlyInspecting = lower;

            var leaderboardService = new Ajax.LeaderboardAjaxService();
            leaderboardService.resultsElement = element;
            leaderboardService.sendRequest(lower, upper);
        }

        function open() {
            var leaderboardModal = new modal.Window();
            leaderboardModal.title = 'Leaderboards';
            if (!Account.signedIn) {
                var signInNotification = document.createElement('div');
                signInNotification.style.fontSize = '18px';
                signInNotification.style.background = 'darkred';
                signInNotification.style.color = 'white';
                signInNotification.style.fontWeight = 'bold';
                signInNotification.style.textAlign = 'center';
                signInNotification.style.border = '1px solid white';
                signInNotification.textContent = "Sign in to see where you rank.";
                leaderboardModal.addElement(signInNotification);
            }
            var leaderboardList = document.createElement('DIV');
            leaderboardList.style.width = '400px';
            leaderboardList.appendChild(Ajax.createLoader());
            leaderboardModal.addElement(leaderboardList);

            // Navigation controls.
            var controlContainer = document.createElement('div');
            controlContainer.style.textAlign = 'center';
            controlContainer.style.marginTop = '5px';
            var navigateUp = Utils.createButton('<<', '');
            var navigateSpecific = document.createElement('input');
            navigateSpecific.type = 'TEXT';
            var navigateDown = Utils.createButton('>>', '');

            navigateUp.addEventListener('click', function () {
                Leaderboard.currentlyInspecting -= 20;
                if (Leaderboard.currentlyInspecting < 0)
                    Leaderboard.currentlyInspecting = 0;
                Leaderboard.currentlyInspecting = Leaderboard.currentlyInspecting - (Leaderboard.currentlyInspecting % 20);

                load(Leaderboard.currentlyInspecting, Leaderboard.currentlyInspecting + 20, leaderboardList);
            });

            navigateSpecific.addEventListener('keyup', function (e) {
                if (e.keyCode == 13) {
                    var text = e.currentTarget.value;
                    if (!Utils.isNumber(text))
                        return;

                    Leaderboard.currentlyInspecting = +text;
                    Leaderboard.currentlyInspecting = Leaderboard.currentlyInspecting - (Leaderboard.currentlyInspecting % 20);
                    load(Leaderboard.currentlyInspecting, Leaderboard.currentlyInspecting + 20, leaderboardList);
                }
            });

            navigateDown.addEventListener('click', function () {
                Leaderboard.currentlyInspecting += 20;
                Leaderboard.currentlyInspecting = Leaderboard.currentlyInspecting - (Leaderboard.currentlyInspecting % 20);

                load(Leaderboard.currentlyInspecting, Leaderboard.currentlyInspecting + 20, leaderboardList);
            });

            controlContainer.appendChild(navigateUp);
            controlContainer.appendChild(navigateSpecific);
            controlContainer.appendChild(navigateDown);
            leaderboardModal.addElement(controlContainer);

            // options
            var close = leaderboardModal.addOption("Close");
            close.addEventListener('click', function () {
                modal.close();
            });

            leaderboardModal.show();
            load(0, 20, leaderboardList);
        }
        Leaderboard.open = open;
    })(Leaderboard || (Leaderboard = {}));
=======
            if (cellMinWidths[i] != '0')
                cell.style.minWidth = cellMinWidths[i];
        }

        var itemsSubHeader = craftingTable.insertRow(2);
        itemsSubHeader.classList.add('table-subheader');
        var itemsSubHeaderCell = itemsSubHeader.insertCell(0);
        itemsSubHeaderCell.colSpan = cellDescriptions.length;
        itemsSubHeaderCell.textContent = 'Items';

        var upgradesSubHeader = craftingTable.insertRow(3);
        upgradesSubHeader.classList.add('table-subheader');
        var upgradesSubHeaderCell = upgradesSubHeader.insertCell(0);
        upgradesSubHeaderCell.colSpan = cellDescriptions.length;
        upgradesSubHeaderCell.textContent = 'Upgrades';

        craftingSection.appendChild(craftingTable);
    }

    function addRecipe(id, ingredients, resultants, isItem) {
        if (!storePane)
            draw();

        if (!recipes[id]) {
            var recipe = new Recipe();
            recipe.id = id;
            recipe.isItem = isItem;
            recipes[id] = recipe;

            for (var i = 0; i < ingredients.length; i++)
                recipe.addIngredient(new Ingredient(ingredients[i].Id, ingredients[i].Quantity));

            for (var i = 0; i < resultants.length; i++)
                recipe.addResultant(new Ingredient(resultants[i].Id, resultants[i].Quantity));

            drawRecipe(recipe, isItem);
        }
    }
    Crafting.addRecipe = addRecipe;

    function addProcessor(id, name, required) {
        if (!storePane)
            draw();

        if (!Crafting.processors[id]) {
            var processor = new Processor();
            processor.id = id;
            processor.name = name;
            processor.requiredId = required;
            Crafting.processors[id] = processor;

            drawProcessor(processor);
        }
    }
    Crafting.addProcessor = addProcessor;

    function hasRecipe(id, resultantId) {
        if (!Crafting.processors[id])
            return;
        var processor = Crafting.processors[id];

        for (var i = 0; i < processor._recipes.length; i++) {
            if (resultantId == processor._recipes[i].resultants[0].id)
                return true;
        }
        return false;
    }
>>>>>>> 48573a74dbde3044acffad41d7f4206ab30b244e

    function addProcessorRecipe(id, ingredients, resultants) {
        if (!Crafting.processors[id])
            return;
        var processor = Crafting.processors[id];

        // if we've already added this recipe ignore this.
        if (hasRecipe(id, resultants[0].Id))
            return;

        var recipe = new Recipe();

        for (var i = 0; i < ingredients.length; i++)
            recipe.addIngredient(new Ingredient(ingredients[i].Id, ingredients[i].Quantity));

        for (var i = 0; i < resultants.length; i++)
            recipe.addResultant(new Ingredient(resultants[i].Id, resultants[i].Quantity));

        processor.addRecipe(recipe);

        var opt = document.createElement('OPTION');
        opt.textContent = Objects.lookupName(resultants[0].Id);
        processor.recipeSelector.appendChild(opt);

        if (processor._recipes.length == 1) {
            switchProcessorRecipe(id, 0);
        }
    }
    Crafting.addProcessorRecipe = addProcessorRecipe;

    function switchProcessorRecipe(id, recipeIndex) {
        var processor = Crafting.processors[id];
        if (!processor)
            return;

<<<<<<< HEAD
        var highscoresLink = document.createElement('SPAN');
        highscoresLink.textContent = 'Leaderboards';
        highscoresLink.addEventListener('click', function () {
            Leaderboard.open();
        });
        highscoresLink.style.cursor = 'pointer';
        document.getElementsByClassName('header-links')[0].appendChild(highscoresLink);
=======
        var recipe = processor._recipes[recipeIndex];
        processor.selectedRecipe = recipeIndex;
>>>>>>> 48573a74dbde3044acffad41d7f4206ab30b244e

        while (processor.recipeList.lastChild) {
            processor.recipeList.removeChild(processor.recipeList.lastChild);
        }

        for (var x = 0; x < recipe.ingredients.length; x++) {
            var ingredientBox = document.createElement('DIV');
            ingredientBox.classList.add('item-text');
            ingredientBox.style.height = '30px';
            var ingredientImage = document.createElement('DIV');
            ingredientImage.style.display = 'inline-block';
            var ingredientQuantity = document.createElement('DIV');
            recipe.ingredients[x].quantityDiv = ingredientQuantity;
            ingredientQuantity.style.display = 'inline-block';
            ingredientQuantity.style.verticalAlign = 'super';
            ingredientQuantity.style.color = (recipe.ingredients[x].quantity <= Objects.getQuantity(recipe.ingredients[x].id)) ? 'darkgreen' : 'darkred';
            ingredientQuantity.textContent = Utils.formatNumber(recipe.ingredients[x].quantity);
            ingredientImage.classList.add("Half-" + Utils.cssifyName(Objects.lookupName(recipe.ingredients[x].id)));

            ingredientBox.appendChild(ingredientImage);
            ingredientBox.appendChild(ingredientQuantity);
            processor.recipeList.appendChild(ingredientBox);
        }
    }

    function drawProcessor(processor) {
        var processorTable = document.createElement('TABLE');
        processorTable.classList.add('block-table');
        processor.container = processorTable;

        var header = processorTable.createTHead();
        var titleRow = header.insertRow(0);
        titleRow.classList.add('table-subheader');
        var realTitleRow = header.insertRow(0);
        realTitleRow.classList.add('table-header');

<<<<<<< HEAD
        // styles the container depending on the status of the account.
        Account.signedIn = !isAnon;
        Utils.cssSwap(container, isAnon ? 'registered' : 'anonymous', isAnon ? 'anonymous' : 'registered');
    }

    function loginModal() {
        var loginModal = new modal.Window();
        var formControlsContainer = document.createElement('DIV');
        formControlsContainer.style.width = '400px';
=======
        var titleCell = realTitleRow.insertCell(0);
        titleCell.colSpan = cellDescriptions.length;
        titleCell.textContent = processor.name;

        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = titleRow.insertCell(0);
            cell.style.width = processorCellWidths[i];
            cell.textContent = processorCellDescriptions[i];

            if (processorCellWidths[i] != '0')
                cell.style.minWidth = processorCellMinWidths[i];
        }

        // progress
        var progressRow = processorTable.insertRow(2);
        progressRow.classList.add('table-row');
        var progressCell = progressRow.insertCell(0);
        progressCell.colSpan = processorCellDescriptions.length;
        var progressContainer = document.createElement('DIV');
        progressContainer.classList.add('progress-bar-container');
        var progressBar = document.createElement('DIV');
        progressBar.classList.add('progress-bar');
        var progressTextContainer = document.createElement('DIV');
        progressTextContainer.classList.add('progress-bar-text-container');
        var progressText = document.createElement('DIV');
        progressText.classList.add('progress-bar-text');
        progressTextContainer.appendChild(progressText);
        progressContainer.appendChild(progressTextContainer);

        processor.progressBar = progressBar;
        processor.progressText = progressText;
        progressContainer.appendChild(progressBar);
        progressCell.appendChild(progressContainer);
>>>>>>> 48573a74dbde3044acffad41d7f4206ab30b244e

        var contentRow = processorTable.insertRow(3);
        contentRow.classList.add('table-row');

        for (var i = 0; i < processorCellDescriptions.length; i++) {
            var cell = contentRow.insertCell(0);
            cell.style.width = cellWidths[i];
            cell.style.height = '75px';

            if (processorCellMinWidths[i] != '0')
                cell.style.minWidth = processorCellMinWidths[i];

            if (processorCellDescriptions[i] == "Image") {
                var image = document.createElement('DIV');
                image.classList.add(Utils.cssifyName(processor.name));
                image.style.margin = '0 auto';
                cell.appendChild(image);
            }

            if (processorCellDescriptions[i] == "Capacity") {
                cell.textContent = '0';
                processor.capacityElm = cell;
                /*
                for (var x = 0; x < recipe.ingredients.length; x++) {
                var ingredientBox = document.createElement('DIV');
                ingredientBox.classList.add('item-text');
                ingredientBox.style.height = '22px';
                var ingredientImage = document.createElement('DIV');
                ingredientImage.style.display = 'inline-block';
                var ingredientQuantity = document.createElement('DIV');
                recipe.ingredients[x].quantityDiv = ingredientQuantity;
                ingredientQuantity.style.display = 'inline-block';
                ingredientQuantity.style.verticalAlign = 'super';
                ingredientQuantity.textContent = Utils.formatNumber(recipe.ingredients[x].quantity);
                ingredientImage.classList.add("Third-" + Utils.cssifyName(Objects.lookupName(recipe.ingredients[x].id)));
                
                ingredientBox.appendChild(ingredientImage);
                ingredientBox.appendChild(ingredientQuantity);
                cell.appendChild(ingredientBox);
                }*/
            }

            if (processorCellDescriptions[i] == "Input") {
                processor.recipeList = cell;
            }

            if (processorCellDescriptions[i] == "Output") {
                var maxButton = Utils.createButton('Max', '');
                cell.appendChild(maxButton);

                var quantitySelector = document.createElement('INPUT');
                quantitySelector.type = 'TEXT';
                quantitySelector.style.width = '35px';
                processor.quantityTextbox = quantitySelector;
                cell.appendChild(quantitySelector);
                var id = processor.id;
                var selector = document.createElement('SELECT');
                selector.addEventListener('change', function (e) {
                    switchProcessorRecipe(id, selector.selectedIndex);
                });
                processor.recipeSelector = selector;
                cell.appendChild(selector);
            }

            if (processorCellDescriptions[i] == "Action") {
                var activateBtn = Utils.createButton('Activate', '');
                activateBtn.addEventListener('click', function () {
                    if (Utils.isNumber(processor.quantityTextbox.value))
                        Connection.processRecipe(processor.id, processor.recipeSelector.selectedIndex, +processor.quantityTextbox.value);
                }, false);
                cell.appendChild(activateBtn);
            }
        }
        drawProcessorSidebar(processor);
        processorSection.appendChild(processorTable);
    }

    function updateProcessor(id, selectedRecipe, operationDuration, completedOperations, totalOperations, capacity) {
        var processor = Crafting.processors[id];
        if (!processor)
            return;

        var progressChanged = false;

        Utils.ifNotDefault(selectedRecipe, function () {
            processor.selectedRecipe = selectedRecipe;
        });

        Utils.ifNotDefault(totalOperations, function () {
            processor.totalOperations = totalOperations;
            progressChanged = true;
        });

        Utils.ifNotDefault(operationDuration, function () {
            processor.operationDuration = operationDuration;
        });

        Utils.ifNotDefault(capacity, function () {
            processor.capacityElm.textContent = capacity.toString();
        });

        Utils.ifNotDefault(completedOperations, function () {
            processor.completedOperations = completedOperations;
            progressChanged = true;
        });

        if (progressChanged) {
            if (processor.totalOperations <= 0 || processor.completedOperations == processor.totalOperations) {
                processor.progressBar.style.width = '0%';
                processor.progressText.textContent = '';
                processor.sidebarProgressText.textContent = '';
                processor.sidebarContainer.style.display = 'none';
                processor.totalOperationCompletionTime = 0;
                return;
            }
            processor.sidebarContainer.style.display = 'inline-block';
            processor.operationStartTime = Date.now();
            processor.operationCompletionTime = processor.operationStartTime + (processor.operationDuration * 1000);

            if (processor.totalOperationCompletionTime == 0)
                processor.totalOperationCompletionTime = processor.operationStartTime + ((processor.operationDuration * 1000) * (processor.totalOperations - processor.completedOperations));
        }

        /*
        Utils.ifNotDefault(completedOperations, function () {
        if (processor.completedOperations != completedOperations) {
        console.log('New ' + processor.name + ' operation');
        processor.completedOperations = completedOperations;
        if (processor.totalOperations > 0) {
        processor.operationStartTime = Date.now();
        processor.operationCompletionTime = processor.operationStartTime + (processor.operationDuration * 1000);
        console.log('Start: ' + processor.operationStartTime + ' End: ' + processor.operationCompletionTime);
        } else {
        console.log('Operation finished.');
        }
        }
        });*/
        if (processor.selectedRecipe > -1) {
            try  {
                processor.progressText.textContent = Objects.lookupName(processor._recipes[processor.selectedRecipe].resultants[0].id) + ' (' + processor.completedOperations + '/' + processor.totalOperations + ')';
                processor.sidebarJobText.textContent = Objects.lookupName(processor._recipes[processor.selectedRecipe].resultants[0].id) + ' (' + processor.completedOperations + '/' + processor.totalOperations + ')';
            } catch (err) {
                console.log("invalid processor recipe " + processor.selectedRecipe);
            }
        } else {
            processor.progressText.textContent = '';
            processor.sidebarProgressText.textContent = '';
        }
        /* if (processor.operationCompletionTime != operationCompletionTime) {
        processor.operationStartTime = Date.now() / 1000;
        processor.operationCompletionTime = operationCompletionTime;
        }*/
    }
    Crafting.updateProcessor = updateProcessor;

    function processorBars() {
        Crafting.processors.forEach(function (processor) {
            if (processor.operationDuration <= 0)
                processor.progressBar.style.width = '0%';
            else if (processor.completedOperations != processor.totalOperations && processor.totalOperations > 0) {
                /*console.log(processor.operationCompletionTime);
                console.log(processor.operationStartTime);
                console.log(((((processor.operationCompletionTime - processor.operationStartTime) / processor.operationDuration))/10) + '%');*/
                var timeToFinish = processor.operationCompletionTime - Date.now();
                timeToFinish /= 1000;

                var totalTimeToFinish = processor.totalOperationCompletionTime - Date.now();
                var totalCompletionPerc = totalTimeToFinish / (processor.operationDuration * processor.totalOperations);
                totalCompletionPerc = (100 - totalCompletionPerc / 10);

                var completionPerc = timeToFinish / processor.operationDuration;
                completionPerc *= 100;
                completionPerc = 100 - completionPerc;

                // processor.progressBar.style.width = ((((processor.operationCompletionTime - processor.operationStartTime) / processor.operationDuration)) / 10) + '%';
                processor.progressBar.style.width = completionPerc + '%';
                processor.sidebarJobBar.style.width = completionPerc + '%';

                processor.sidebarProgressBar.style.width = totalCompletionPerc + '%';
                processor.sidebarProgressText.textContent = Utils.formatTime(totalTimeToFinish / 1000);
            }

            if (processor.requiredId != 0)
                processor.container.style.display = Objects.getQuantity(processor.requiredId) > 0 ? 'block' : 'none';
        });
    }
    setInterval(processorBars, 10);

    function update() {
        if (!storePane)
            return;

        recipes.forEach(function (recipe) {
            var quantity = Objects.getQuantity(recipe.id);
            recipe.row.style.display = (quantity == -1 || !recipe.isItem && quantity > 0) ? 'none' : '';
            recipe.ingredients.forEach(function (ingredient) {
                var ingQuantity = Objects.getQuantity(ingredient.id);
                ingredient.quantityDiv.style.color = (ingQuantity >= ingredient.quantity) ? 'darkgreen' : 'darkred';
            });
        });

        Crafting.processors.forEach(function (processor) {
            switchProcessorRecipe(processor.id, processor.recipeSelector.selectedIndex);
        });
        /*
        for (var i = 0; i < recipes.length; i++) {
        var recipe = recipes[i];
        var quantity = Objects.getQuantity(recipe.id);
        recipe.row.style.display = (quantity == -1) ? 'none' : 'inline-block';
        }*/
    }
    Crafting.update = update;

    function drawProcessorSidebar(processor) {
        if (!processorSidebar) {
            processorSidebar = document.createElement('div');
            sidebar.appendChild(processorSidebar);
        }

        var container = document.createElement('div');
        container.classList.add('processor-sidebar');
        processor.sidebarContainer = container;

        var header = document.createElement('div');
        header.classList.add('buff-header');
        header.textContent = processor.name;
        container.appendChild(header);

        var totalTime = document.createElement('div');
        totalTime.classList.add('processor-sidebar-progress-bar-container');
        var totalTimeBar = document.createElement('div');
        totalTimeBar.classList.add('processor-sidebar-progress-bar');
        processor.sidebarProgressBar = totalTimeBar;
        var totalTimeText = document.createElement('div');
        totalTimeText.classList.add('processor-sidebar-progress-text');
        processor.sidebarProgressText = totalTimeText;
        totalTime.appendChild(totalTimeText);
        totalTime.appendChild(totalTimeBar);
        container.appendChild(totalTime);

        var jobTime = document.createElement('div');
        jobTime.classList.add('processor-sidebar-progress-bar-container');
        var jobTimeBar = document.createElement('div');
        jobTimeBar.classList.add('processor-sidebar-progress-bar');
        processor.sidebarJobBar = jobTimeBar;
        var jobTimeText = document.createElement('div');
        jobTimeText.classList.add('processor-sidebar-progress-text');
        processor.sidebarJobText = jobTimeText;
        jobTime.appendChild(jobTimeText);
        jobTime.appendChild(jobTimeBar);
        container.appendChild(jobTime);
        container.style.display = 'none';

        processorSidebar.appendChild(container);
    }

    function drawRecipe(recipe, isItem) {
        var pointOfInsertion = craftingTable.rows.length;

        if (isItem) {
            pointOfInsertion = itemsTableOffset;
            itemsTableOffset++;
        }

        var recipeRow = craftingTable.insertRow(pointOfInsertion);
        recipeRow.classList.add('table-row');
        recipe.row = recipeRow;
        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = recipeRow.insertCell(0);
            cell.style.width = cellWidths[i];

            if (cellMinWidths[i] != '0')
                cell.style.minWidth = cellMinWidths[i];

            if (cellDescriptions[i] == "Name")
                cell.textContent = Objects.lookupName(recipe.id);

            if (cellDescriptions[i] == "Input") {
                for (var x = 0; x < recipe.ingredients.length; x++) {
                    var ingredientBox = document.createElement('DIV');
                    ingredientBox.classList.add('item-text');
                    ingredientBox.style.height = 'auto';
                    var ingredientImage = document.createElement('DIV');
                    ingredientImage.style.display = 'inline-block';
                    var ingredientQuantity = document.createElement('DIV');
                    recipe.ingredients[x].quantityDiv = ingredientQuantity;
                    ingredientQuantity.style.display = 'inline-block';
                    ingredientQuantity.style.verticalAlign = 'super';
                    ingredientQuantity.textContent = Utils.formatNumber(recipe.ingredients[x].quantity);
                    ingredientImage.classList.add("Third-" + Utils.cssifyName(Objects.lookupName(recipe.ingredients[x].id)));

                    ingredientBox.appendChild(ingredientImage);
                    ingredientBox.appendChild(ingredientQuantity);
                    cell.appendChild(ingredientBox);
                }
            }

            if (cellDescriptions[i] == "Description") {
                cell.textContent = Objects.getTooltip(recipe.resultants[0].id);
            }

            if (cellDescriptions[i] == "Output") {
                for (var x = 0; x < recipe.resultants.length; x++) {
                    var ingredientBox = document.createElement('DIV');
                    ingredientBox.classList.add('item-text');
                    ingredientBox.style.height = 'auto';
                    var ingredientImage = document.createElement('DIV');
                    ingredientImage.style.display = 'inline-block';
                    var ingredientQuantity = document.createElement('DIV');
                    ingredientQuantity.style.display = 'inline-block';
                    ingredientQuantity.style.verticalAlign = 'super';
                    ingredientQuantity.textContent = Utils.formatNumber(recipe.resultants[x].quantity);
                    ingredientImage.classList.add("Third-" + Utils.cssifyName(Objects.lookupName(recipe.resultants[x].id)));

                    ingredientBox.appendChild(ingredientImage);

                    if (recipe.isItem)
                        ingredientBox.appendChild(ingredientQuantity);

                    cell.appendChild(ingredientBox);
                }
            }

            if (cellDescriptions[i] == "Action") {
                var craftBtn = Utils.createButton('Craft', '');
                craftBtn.addEventListener('click', function () {
                    Connection.craftRecipe(recipe.id, 1);
                }, false);
                cell.appendChild(craftBtn);

                if (recipe.isItem) {
                    var quantity = document.createElement('INPUT');
                    quantity.type = 'TEXT';
                    quantity.style.width = '30px';

                    var craftXBtn = Utils.createButton('Craft-x', '');
                    craftXBtn.addEventListener('click', function () {
                        Connection.craftRecipe(recipe.id, +quantity.value);
                    }, false);
                    cell.appendChild(craftXBtn);
                    cell.appendChild(quantity);
                }
            }
        }
    }
})(Crafting || (Crafting = {}));
var Buffs;
(function (Buffs) {
    var buffs = new Array();
    var sidebar = document.getElementById('sidebarInformation');
    var buffContainer;

<<<<<<< HEAD
    var LeaderboardAjaxService = (function () {
        function LeaderboardAjaxService() {
        }
        LeaderboardAjaxService.prototype.failed = function (request) {
            this.resultsElement.textContent = 'Loading failed...';
        };

        LeaderboardAjaxService.prototype.succeeded = function (request) {
            while (this.resultsElement.firstChild)
                this.resultsElement.removeChild(this.resultsElement.firstChild);

            var leaderboardTable = document.createElement('table');
            var thead = leaderboardTable.createTHead();
            var subheader = thead.insertRow(0);
            subheader.classList.add('table-subheader');

            var score = subheader.insertCell(0);
            score.textContent = 'Score';
            score.style.width = '65%';
            var player = subheader.insertCell(0);
            player.textContent = 'Name';
            player.style.width = '25%';
            var rank = subheader.insertCell(0);
            rank.textContent = 'Rank';
            rank.style.width = '10%';

            var tbody = leaderboardTable.createTBody();

            for (var i = 0; i < request.length; i++) {
                var leaderboardEntry = request[i];

                var row = tbody.insertRow(tbody.rows.length);
                row.classList.add('table-row');

                var rScore = row.insertCell(0);
                rScore.textContent = Utils.formatNumber(leaderboardEntry.Score);
                rScore.style.width = '65%';
                rScore.addEventListener('click', function (event) {
                    var score;
                    var cell = event.target;

                    if (cell.dataset) {
                        score = cell.dataset['tooltip'];
                    } else {
                        score = cell.getAttribute('data-tooltip');
                    }

                    if (cell.textContent.indexOf(',') > 0) {
                        cell.textContent = Utils.formatNumber(score);
                    } else {
                        cell.textContent = Utils.formatNumber(score, true);
                    }
                });

                if (rScore.dataset) {
                    rScore.dataset['tooltip'] = leaderboardEntry.Score;
                } else {
                    rScore.setAttribute('data-tooltip', leaderboardEntry.Score.toString());
                }

                var rPlayer = row.insertCell(0);
                rPlayer.textContent = leaderboardEntry.UserId;
                player.style.width = '25%';
                var rRank = row.insertCell(0);
                rRank.textContent = Utils.formatNumber(leaderboardEntry.Rank);
                rRank.style.width = '10%';
            }
            this.resultsElement.appendChild(leaderboardTable);
        };

        LeaderboardAjaxService.prototype.sendRequest = function (lowerbound, upperbound) {
            var self = this;

            var request = $.ajax({
                asyn: true,
                type: 'POST',
                url: '/Api/Stats/LeaderBoard/',
                data: $.param({ Lower: lowerbound, Upper: upperbound }),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                success: function (request) {
                    request = JSON.parse(request);
                    self.succeeded(request);
                },
                failure: function (request) {
                    self.failed(request);
                }
            });
        };
        return LeaderboardAjaxService;
    })();
    Ajax.LeaderboardAjaxService = LeaderboardAjaxService;

    var Loader = (function () {
        function Loader() {
            this.notDisplayed = 20;
            this.notches = 40;
            this.space = 0.02;
            this.spawned = false;
            this.spinPos = 0;
            this.spinSpeed = 30;
            this.squareSize = 50;
            this.timeSinceLastTick = Date.now();
            this.thickness = 3;
            this.dead = false;
            this.container = document.createElement('div');
            this.container.classList.add('loader');
            this.canvasElement = document.createElement('canvas');
            this.canvasElement.width = this.squareSize;
            this.canvasElement.height = this.squareSize;
            this.container.appendChild(this.canvasElement);
            this.context = this.canvasElement.getContext('2d');
=======
    var Buff = (function () {
        function Buff() {
>>>>>>> 48573a74dbde3044acffad41d7f4206ab30b244e
        }
        return Buff;
    })();

    function init() {
        buffContainer = document.createElement('div');
        sidebar.appendChild(buffContainer);
    }

    function register(id, name, description, duration) {
        if (buffs[id])
            return;

        var buff = new Buff();
        buff.name = name;
        buff.description = description;
        buff.duration = duration;
        buffs[id] = buff;

        draw(buff);
    }
    Buffs.register = register;

    function draw(buff) {
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

    function update(id, timeActive) {
        var buff = buffs[id];
        buff.timeActive = timeActive;
        buff.durationElm.textContent = '(' + Utils.formatTime(buff.duration - timeActive) + ')';
        buff.container.style.display = timeActive == 0 ? 'none' : 'inline-block';
    }
    Buffs.update = update;
    init();
})(Buffs || (Buffs = {}));
///<reference path="chat.ts"/>
///<reference path="inventory.ts"/>
///<reference path="stats.ts"/>
///<reference path="store.ts"/>
///<reference path="rock.ts"/>
///<reference path="equipment.ts"/>
///<reference path="crafting.ts"/>
///<reference path="typings/jquery/jquery.d.ts"/>
///<reference path="buffs.ts"/>
///<reference path="register.ts"/>
///<reference path="modal.ts"/>
///<reference path="tooltip.ts"/>
///<reference path="ajax.ts"/>
var Connection;
(function (Connection) {
    var conInterval;
    var disconInterval;
    var notificationElm;
    var networkErrorElm;
    var rateLimitedElm;
    var playerCounter;

    function init() {
        var headerLinks = document.getElementsByClassName('header-links')[0];
        var versionHistory = document.createElement('div');
        versionHistory.style.display = 'inline-block';
        versionHistory.textContent = 'Version History';
        versionHistory.addEventListener('click', function () {
            window.open('/version');
        });
        versionHistory.style.cursor = 'pointer';
        headerLinks.appendChild(versionHistory);

        playerCounter = document.createElement('div');
        playerCounter.style.display = 'inline-block';
        playerCounter.textContent = 'There are 0 players mining.';
        headerLinks.appendChild(playerCounter);

        notificationElm = document.createElement('div');
        notificationElm.classList.add('error-notification-tray');

        networkErrorElm = document.createElement('div');
        networkErrorElm.classList.add('network-error');
        var networkErrorText = document.createElement('div');
        networkErrorText.classList.add('network-error-text');
        networkErrorText.textContent = 'No connection';
        networkErrorElm.appendChild(networkErrorText);

        rateLimitedElm = document.createElement('div');
        rateLimitedElm.classList.add('rate-limited');
        var rateLimitText = document.createElement('div');
        rateLimitText.classList.add('network-error-text');
        rateLimitedElm.style.display = 'none';
        ;
        rateLimitText.textContent = 'You have exceeded your allotted requests';
        rateLimitedElm.appendChild(rateLimitText);

        var game = document.getElementById('game');
        notificationElm.appendChild(networkErrorElm);
        notificationElm.appendChild(rateLimitedElm);
        game.insertBefore(notificationElm, game.childNodes[0]);
    }
    init();

    Komodo.connection.received(function (msg) {
        Chat.log("Recieved " + roughSizeOfObject(msg) + " bytes from komodo.");
        Chat.log("Encoded: ");
        Chat.log(msg);
        Chat.log("Decoded: ");
        Chat.log(JSON.stringify(Komodo.decode(msg)));
        Chat.log(roughSizeOfObject(JSON.stringify(Komodo.decode(msg))) - roughSizeOfObject(msg) + " bytes saved.");
        msg = Komodo.decode(msg);

        // CHAT MESSAGES
        if (msg.Messages != null) {
            receiveGlobalMessages(msg.Messages);
        }

        // GAME SCHEMA
        if (msg.GameSchema != null) {
            loadSchema(msg.GameSchema);
        }

        // INVENTORY UPDATES
        if (msg.Items != null) {
            updateInventory(msg.Items);
        }

        // STORE UPDATES
        if (msg.StoreItemsUpdate != null) {
            updateStore(msg.StoreItemsUpdate);
        }
        if (msg.StatItemsUpdate != null) {
            updateStats(msg.StatItemsUpdate);
        }
        if (msg.ConfigItems != null) {
            updateInventoryConfigurations(msg.ConfigItems);
        }

        // PROCESSOR UPDATES
        if (msg.Processors != null) {
            updateProcessors(msg.Processors);
        }

        // Anti cheat
        if (msg.AntiCheatCoordinates != null) {
            antiCheat(msg.AntiCheatCoordinates);
        }

        // Buffs
        if (msg.Buffs != null) {
            updateBuffs(msg.Buffs);
        }
        if (msg.IsRateLimited != null) {
            rateLimit(msg.IsRateLimited);
        }
        if (msg.Gatherers != null) {
            updateGatherers(msg.Gatherers);
        }
        if (msg.ConnectedUsers) {
            playerCounter.textContent = 'There are ' + msg.ConnectedUsers + ' players mining.';
        }
    });

    function restart() {
        Komodo.restart();
    }
    Connection.restart = restart;

    var actions = new Komodo.ClientActions();

    Komodo.connection.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.connected) {
            connected();
            networkErrorElm.style.display = 'none';
        }
        if (change.newState === $.signalR.connectionState.disconnected) {
            clearInterval(conInterval);
            networkErrorElm.style.display = 'block';
        }
        if (change.newState === $.signalR.connectionState.reconnecting) {
            networkErrorElm.style.display = 'block';
        }
    });

    function connected() {
        console.log('Connection opened');
        var encoded = actions.encode64();
        send(encoded);
        actions = new Komodo.ClientActions();

        conInterval = setInterval(function () {
            var encoded = actions.encode64();

            // if (encoded!='') {
            send(encoded);

            //}
            actions = new Komodo.ClientActions();
        }, 1000);
    }

    function disconnected() {
        console.log('Connection lost');
        networkErrorElm.style.top = '0px';
        disconInterval = setTimeout(function () {
            Komodo.restart();
        }, 5000);
    }

    function loadSchema(schema) {
        if (schema.Items) {
            for (var i = 0; i < schema.Items.length; i++) {
                Inventory.addItem(schema.Items[i].Id, schema.Items[i].Name, schema.Items[i].Worth, schema.Items[i].Category);
                Statistics.addItem(schema.Items[i].Id, schema.Items[i].Name);
            }
        }

        if (schema.StoreItems) {
            for (var i = 0; i < schema.StoreItems.length; i++) {
                var item = schema.StoreItems[i];
                Store.addItem(item.Id, item.Category, item.Price, item.Factor, item.Name, item.MaxQuantity, item.Tooltip);
            }
        }

        if (schema.Processors) {
            for (var i = 0; i < schema.Processors.length; i++) {
                var processor = schema.Processors[i];
                Crafting.addProcessor(processor.Id, processor.Name, processor.RequiredId);
                for (var r = 0; r < processor.Recipes.length; r++) {
                    Crafting.addProcessorRecipe(processor.Id, processor.Recipes[r].Ingredients, processor.Recipes[r].Resultants);
                }
            }
        }
        if (schema.CraftingItems) {
            for (var i = 0; i < schema.CraftingItems.length; i++) {
                var item = schema.CraftingItems[i];
                Crafting.addRecipe(item.Id, item.Ingredients, item.Resultants, item.IsItem);
            }
        }
        if (schema.Buffs) {
            for (var i = 0; i < schema.Buffs.length; i++) {
                var buff = schema.Buffs[i];
                Buffs.register(buff.Id, buff.Name, buff.Description, buff.Duration);
            }
        }
    }

    function rateLimit(limited) {
        rateLimitedElm.style.display = limited ? 'block' : 'none';
    }

    function toggleGatherer(id, enabled) {
        var gathererAction = new Komodo.ClientActions.GathererAction();
        gathererAction.Id = id;
        gathererAction.Enabled = enabled;
        actions.GathererActions.push(gathererAction);
    }
    Connection.toggleGatherer = toggleGatherer;

    function updateGatherers(gatherers) {
        for (var i = 0; i < gatherers.length; i++) {
            var gatherer = gatherers[i];
            Equipment.toggleGatherer(gatherer.Id, gatherer.Enabled);
            Equipment.changeEfficiency(gatherer.Id, gatherer.Efficiency);
            Equipment.changeFuelConsumption(gatherer.Id, gatherer.FuelConsumed);
            Equipment.changeRarityBonus(gatherer.Id, gatherer.RarityBonus);
        }
    }

    function updateBuffs(buffs) {
        for (var i = 0; i < buffs.length; i++) {
            var buff = buffs[i];
            Buffs.update(buff.Id, buff.TimeActive);
        }
    }

    function receiveGlobalMessages(messages) {
        for (var i = 0; i < messages.length; i++) {
            var msg = messages[i];
            console.log(msg);
            Chat.receiveGlobalMessage(msg.Sender, msg.Text, msg.Time, msg.Permissions);
        }
    }

    function updateProcessors(processors) {
        for (var i = 0; i < processors.length; i++) {
            var processor = processors[i];
            Crafting.updateProcessor(processor.Id, processor.SelectedRecipe, processor.OperationDuration, processor.CompletedOperations, processor.TotalOperations, processor.Capacity);
        }
    }

    function antiCheat(ac) {
        Rock.moveRock(ac.X, ac.Y);
    }

    function updateStats(items) {
        for (var i = 0; i < items.length; i++)
            Statistics.changeStats(items[i].Id, items[i].PrestigeQuantity, items[i].LifeTimeQuantity);
    }

    function updateInventory(items) {
        for (var i = 0; i < items.length; i++) {
            Inventory.changeQuantity(items[i].Id, items[i].Quantity);
            Inventory.changePrice(items[i].Id, items[i].Worth);
        }
    }

    function updateInventoryConfigurations(items) {
        for (var i = 0; i < items.length; i++)
            Inventory.modifyConfig(items[i].Id, items[i].Enabled);
    }

    function updateStore(items) {
        for (var i = 0; i < items.length; i++)
            Store.changeQuantity(items[i].Id, items[i].Quantity, items[i].MaxQuantity, items[i].Price);
    }

    function drink(id) {
        var potionAction = new Komodo.ClientActions.PotionAction();
        potionAction.Id = id;
        actions.PotionActions.push(potionAction);
    }
    Connection.drink = drink;

    function mine(x, y) {
        var miningAction = new Komodo.ClientActions.MiningAction();
        miningAction.X = x;
        miningAction.Y = y;
        actions.MiningActions.push(miningAction);
    }
    Connection.mine = mine;

    function sellItem(id, quantity) {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        var sellAction = new Komodo.ClientActions.InventoryAction.SellAction();
        sellAction.Id = id;
        sellAction.Quantity = quantity;
        inventoryAction.Sell = sellAction;
        actions.InventoryActions.push(inventoryAction);
    }
    Connection.sellItem = sellItem;

    function configureItem(id, enabled) {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        var configAction = new Komodo.ClientActions.InventoryAction.ConfigAction();
        configAction.Id = id;
        configAction.Enabled = enabled;
        inventoryAction.Config = configAction;
        actions.InventoryActions.push(inventoryAction);
        //ConfigAction
    }
    Connection.configureItem = configureItem;

    function purchaseItem(id, quantity) {
        var storeAction = new Komodo.ClientActions.StoreAction();
        var purchaseAction = new Komodo.ClientActions.StoreAction.PurchaseAction();
        purchaseAction.Id = id;
        purchaseAction.Quantity = (quantity ? quantity : 1);
        storeAction.Purchase = purchaseAction;
        actions.StoreActions.push(storeAction);
    }
    Connection.purchaseItem = purchaseItem;

    function sellAllItems() {
        var inventoryAction = new Komodo.ClientActions.InventoryAction();
        inventoryAction.SellAll = true;
        actions.InventoryActions.push(inventoryAction);
    }
    Connection.sellAllItems = sellAllItems;

    function craftRecipe(id, quantity) {
        var craftingAction = new Komodo.ClientActions.CraftingAction();
        craftingAction.Id = id;
        craftingAction.Quantity = quantity;
        actions.CraftingActions.push(craftingAction);
    }
    Connection.craftRecipe = craftRecipe;

    function processRecipe(id, recipeIndex, iterations) {
        var processingAction = new Komodo.ClientActions.ProcessingAction();
        processingAction.Id = id;
        processingAction.RecipeIndex = recipeIndex;
        processingAction.Iterations = iterations;
        actions.ProcessingActions.push(processingAction);
    }
    Connection.processRecipe = processRecipe;

    function sendGlobalMessage(message) {
        /*var clientActions = new Komodo.ClientActions();
        var socialAction = new Komodo.ClientActions.SocialAction();
        var chatAction = new Komodo.ClientActions.SocialAction.ChatAction();
        chatAction.GlobalMessage = message;
        socialAction.Chat = chatAction;
        clientActions.SocialActions.push(socialAction);
        
        Connection.send(clientActions);*/
        var socialAction = new Komodo.ClientActions.SocialAction();
        var chatAction = new Komodo.ClientActions.SocialAction.ChatAction();
        chatAction.GlobalMessage = message;
        socialAction.Chat = chatAction;
        actions.SocialActions.push(socialAction);
    }
    Connection.sendGlobalMessage = sendGlobalMessage;

    function send(message) {
        if (message.encode64) {
            var encoded = message.encode64();
            Chat.log("Sent " + roughSizeOfObject(encoded) + " bytes to komodo.");
            Chat.log("Decoded: ");
            Chat.log(JSON.stringify(message));
            Chat.log("Encoded: ");
            Chat.log(message.encode64());
            Komodo.send(message.encode64());
        } else {
            Chat.log("Sent " + roughSizeOfObject(message) + " bytes to komodo.");
            Komodo.send(message);
        }
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
