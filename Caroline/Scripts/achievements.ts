module Achievements {
    var achievements = new Array<Achievement>();
    var achievementPane;
    var reverseRequired = false;

    export enum Category {
        Undefined= 0,
        Money= 1,
        TimePlayed= 2,
        RockClicks= 3,
        Oil= 4
    };

    function init() {
        achievementPane = document.createElement('div');
        document.getElementById('paneContainer').appendChild(achievementPane);
        Tabs.registerGameTab(achievementPane, 'Achievements');
    }

    class Achievement {

        name: string;
        requires: Achievement;
        requiredBy: Achievement;
        progress: number;
        goal: number;
        category: Category;
        progressBar: HTMLElement;
        container: HTMLElement;
        completed: boolean;

        getTooltip(): string {
            if (this.category == Category.Money) {
                return "Have a total of " + Utils.formatNumber(this.goal) + " accumulated coins.";
            } else if (this.category == Category.TimePlayed) {
                return "Play for " + secondsToTimePeriod(this.goal) + ".";
            } else if (this.category == Category.RockClicks) {
                return "Click the rock " + Utils.formatNumber(this.goal) + " times.";
            } else if (this.category == Category.Oil) {
                if (this.goal == 1) return "Get some oil!";
                else return "Have a total of " + Utils.formatNumber(this.goal) + " accumulated oil.";
            }
        }


        trickleDown(progress?: number) {
            if (!progress) progress = this.progress;
            this.progress = progress;
            this.progressBar.style.width = ((progress / this.goal) * 100) + "%";
            if (this.requiredBy != null) this.requiredBy.trickleDown(progress);
        }
    }

    export function register(id: number, name: string, requires: number, goal: number, category: Category) {
        if (achievements[id]) return;


        var achievement = new Achievement();
        achievement.name = name;
        if (requires != 0)
            achievement.requires = achievements[requires];
        achievement.goal = goal;
        achievement.category = category;
        drawAchievement(achievement);
        
        achievements[id] = achievement;
    }

    function drawAchievement(achievement: Achievement) {
        if (!achievementPane) init();

        var achievementContainer = document.createElement('div');
        achievementContainer.classList.add('achievement');
        achievement.container = achievementContainer;

        var achievementImage = document.createElement('div');
        achievementImage.classList.add('achievement-image');
        achievementContainer.appendChild(achievementImage);

        var achievementBody = document.createElement('div');
        achievementBody.classList.add('achievement-body');
        achievementContainer.appendChild(achievementBody);

        var achievementTitle = document.createElement('div');
        achievementTitle.classList.add('achievement-title');
        achievementTitle.textContent = achievement.name;
        achievementBody.appendChild(achievementTitle);

        var achievementDescription = document.createElement('div');
        achievementDescription.classList.add('achievement-description');
        achievementDescription.textContent = achievement.getTooltip();
        achievementBody.appendChild(achievementDescription);

        var achievementProgressContainer = document.createElement('div');
        achievementProgressContainer.classList.add('achievement-progress-container');
        achievementBody.appendChild(achievementProgressContainer);

        var achievementProgress = document.createElement('div');
        achievementProgress.classList.add('achievement-progress');
        achievementProgress.style.width = '0%';
        achievementProgressContainer.appendChild(achievementProgress);
        achievement.progressBar = achievementProgress;
        achievementPane.appendChild(achievementContainer);
    }

    function reverseRequire() {
        achievements.forEach(achievement => {
            if (achievement.requires != null) {
                achievement.requires.requiredBy = achievement;
            }
        });
    }


    export function updateAchievement(id: number, progress: number) {
        if (!reverseRequired) {
            reverseRequire();
            reverseRequired = true;
        }

        var achievement = achievements[id];
        if (!achievement) return;
        if (!progress) progress = 0;

        achievement.progress = progress;
        achievement.trickleDown(progress);
    }


    function secondsToTimePeriod(seconds: number): string {
        var minutes: number = seconds / 60;
        if (minutes < 60) {
            return Math.max(Math.ceil(minutes), 1) + " minutes";
        }
        var hours: number = minutes / 60;
        if (hours < 24) {
            return Math.max(Math.ceil(hours), 1) + ((hours<=1) ? " hour":" hours");
        }
        var days: number = hours / 24;
        if (days < 30) {
            return Math.max(Math.ceil(days), 1) + ((days<=1)?" day":" days");
        }
        var months: number = days / 30;
        if (months < 12) {
            return Math.max(Math.ceil(months), 1) + ((months<=1)?" month":" months");
        }
    }
} 