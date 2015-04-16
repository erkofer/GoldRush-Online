/// <reference path="typings/jquery/jquery.d.ts"/>
module Account {
    var container;
    var userButton;
    var userSpan;
    var contextMenu;

    var registrationErrors;
    var loginErrors;

    var mouseTimeout;

    interface ILeaderboardEntry {
        Score: number;
        Rank: number;
        UserId: string;
    }

    class LeaderboardAjaxService {
        constructor() {
            
        }

        private failed(request) {
            this.resultsElement.textContent = 'Loading failed...';
        }

        private succeeded(request) {
            while (this.resultsElement.firstChild)
                this.resultsElement.removeChild(this.resultsElement.firstChild);

            var leaderboardTable: HTMLTableElement = document.createElement('table');
            var thead:HTMLTableElement = <HTMLTableElement>leaderboardTable.createTHead();
            var subheader = <HTMLTableRowElement>thead.insertRow(0);
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
            
            var tbody: HTMLTableElement = <HTMLTableElement>leaderboardTable.createTBody();


            for (var i = 0; i < request.length; i++) {
                var leaderboardEntry: ILeaderboardEntry = request[i];
                console.log(leaderboardEntry);

                var row = <HTMLTableRowElement>tbody.insertRow(tbody.rows.length);
                row.classList.add('table-row');

                var rScore = row.insertCell(0);
                rScore.textContent = Utils.formatNumber(leaderboardEntry.Score);
                rScore.style.width = '65%';
                rScore.addEventListener('click', function(event) {
                    var score;
                    var cell = <HTMLElement>event.target;

                    if (cell.dataset) {
                        score = cell.dataset['tooltip'];
                    } else {
                        score = cell.getAttribute('data-tooltip');
                    }

                    if (cell.textContent.indexOf(',') > 0) { // this number is detailed and filled with commas.
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
        }

        resultsElement: HTMLElement;
        
        sendRequest(lowerbound: number, upperbound: number) {
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
                failure: function(request) {
                    self.failed(request);
                }
            });
        }
    }

    function draw() {
        container = document.createElement('DIV');
        container.classList.add('account-manager');
        container.classList.add('closed');

        container.onmouseenter = function () {
            clearTimeout(mouseTimeout);
        }

        container.onmouseleave = function () {
            mouseTimeout= setTimeout(hideMenu,250);
        }
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

    function updateUser(name: string, isAnon: boolean) {
        userSpan.textContent = isAnon ? 'Guest' : name;
        // styles the container depending on the status of the account.
        Utils.cssSwap(container,
            isAnon ? 'registered' : 'anonymous',
            isAnon ? 'anonymous' : 'registered');
    }

    function leaderboardsModal() {
        var leaderboardModal = new modal.Window();
        leaderboardModal.title = 'Leaderboards';
        var leaderboardList = document.createElement('DIV');
        leaderboardList.style.width = '400px';
        leaderboardList.textContent = 'Loading...';
        leaderboardModal.addElement(leaderboardList);


        var leaderboardService = new LeaderboardAjaxService();
        leaderboardService.resultsElement = leaderboardList;
        leaderboardService.sendRequest(0, 19);

        leaderboardModal.show();
    }

    function loginModal() {
        var loginModal = new modal.Window();
        var formControlsContainer = document.createElement('DIV');
        formControlsContainer.style.width = '400px';

        var usernameContainer = document.createElement('DIV');
        usernameContainer.style.marginBottom = '5px';
        var username = <HTMLInputElement>document.createElement("INPUT");
        username.type = 'TEXT';
        username.maxLength = 16;
        username.placeholder = 'Username';
        usernameContainer.appendChild(username);

        var passwordContainer = document.createElement('DIV');
        passwordContainer.style.marginBottom = '5px';
        var password = <HTMLInputElement>document.createElement("INPUT");
        password.type = 'PASSWORD';
        password.pattern = ".{6,}";
        password.placeholder = 'Password';
        passwordContainer.appendChild(password);

        var rememberMeContainer = document.createElement('DIV');
        rememberMeContainer.style.marginBottom = '5px';
        var rememberMe = <HTMLInputElement>document.createElement('INPUT');
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
        var username = <HTMLInputElement>document.createElement("INPUT");
        username.type = 'TEXT';
        username.maxLength = 16;
        username.placeholder = 'Username';
        usernameContainer.appendChild(username);

        var emailContainer = document.createElement('DIV');
        emailContainer.style.marginBottom = '5px';
        var email = <HTMLInputElement>document.createElement("INPUT");
        email.type = 'EMAIL';
        email.placeholder = 'Email';
        emailContainer.appendChild(email);

        var passwordContainer = document.createElement('DIV');
        passwordContainer.style.marginBottom = '5px';
        var password = <HTMLInputElement>document.createElement("INPUT");
        password.type = 'PASSWORD';
        password.pattern = ".{6,}";
        password.placeholder = 'Password';
        passwordContainer.appendChild(password);

        var confpassContainer = document.createElement('DIV');
        confpassContainer.style.marginBottom = '5px';
        var confirmPassword = <HTMLInputElement>document.createElement("INPUT");
        confirmPassword.type = 'PASSWORD';
        confirmPassword.pattern = ".{6,}";
        confirmPassword.placeholder = 'Confirm password';
        confpassContainer.appendChild(confirmPassword);
        confirmPassword.onblur = function () {
            if (password.value != confirmPassword.value)
                confirmPassword.setCustomValidity('Passwords are not the same.');
        }

        confirmPassword.onfocus = function () {
            confirmPassword.setCustomValidity('');
        }
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


    function create(email: string, username: string, password: string, passwordConfirmation: string) {
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

    function login(email: string, password: string, rememberMe: boolean) {
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
                }
                else {
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

    export function info() {
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
}