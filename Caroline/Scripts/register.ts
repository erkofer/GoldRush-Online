/// <reference path="typings/jquery/jquery.d.ts"/>
module Account {
    var container;
    var userButton;
    var userSpan;
    var contextMenu;

    export var signedIn = false;

    var registrationErrors;
    var loginErrors;

    var mouseTimeout;

    module Leaderboard {
        export var currentlyInspecting = 0;

        function load(lower: number, upper: number, element: HTMLElement) {
            currentlyInspecting = lower;

            var leaderboardService = new Ajax.LeaderboardAjaxService();
            leaderboardService.resultsElement = element;
            leaderboardService.sendRequest(lower, upper);
        }

        export function open() {
            var leaderboardModal = new modal.Window();
            leaderboardModal.title = 'Leaderboards';
            if (!signedIn) {
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
                currentlyInspecting -= 20;
                if (currentlyInspecting < 0) currentlyInspecting = 0;
                currentlyInspecting = currentlyInspecting - (currentlyInspecting % 20);

                load(currentlyInspecting, currentlyInspecting + 20, leaderboardList);
            });

            navigateSpecific.addEventListener('keyup', function(e) {
                if (e.keyCode == 13) { // if they pressed enter.
                    var text = (<HTMLInputElement>e.currentTarget).value;
                    if (!Utils.isNumber(text)) return;

                    currentlyInspecting = +text;
                    currentlyInspecting = currentlyInspecting - (currentlyInspecting % 20);
                    load(currentlyInspecting, currentlyInspecting + 20, leaderboardList);
                }
            });

            navigateDown.addEventListener('click', function () {
                currentlyInspecting += 20;
                currentlyInspecting = currentlyInspecting - (currentlyInspecting % 20);

                load(currentlyInspecting, currentlyInspecting + 20, leaderboardList);
            });

            controlContainer.appendChild(navigateUp);
            controlContainer.appendChild(navigateSpecific);
            controlContainer.appendChild(navigateDown);
            leaderboardModal.addElement(controlContainer);
            // options
            var close = leaderboardModal.addOption("Close");
            close.addEventListener('click', function () { modal.close() });

            leaderboardModal.show();
            load(0, 20, leaderboardList);
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
            mouseTimeout = setTimeout(hideMenu, 250);
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
            Leaderboard.open();
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

    function updateUser(name: string, isAnon: boolean) {
        userSpan.textContent = isAnon ? 'Guest' : name;
        // styles the container depending on the status of the account.
        signedIn = !isAnon;
        Utils.cssSwap(container,
            isAnon ? 'registered' : 'anonymous',
            isAnon ? 'anonymous' : 'registered');
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