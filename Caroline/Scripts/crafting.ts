module Crafting {
    var storePane: HTMLElement;
    var processorSection: HTMLElement;
    var craftingSection: HTMLElement;
    var craftingTable: HTMLElement;
    var cellDescriptions = ['Action', 'Description', 'Input', 'Output', 'Name'];
    var cellWidths = ['10%', '50%', '15%', '15%', '10%'];
    var cellMinWidths = ['170px', '0', '0', '0', '0'];

    var processorCellDescriptions = ['Action', 'Output', 'Input', 'Capacity', 'Image'];
    var processorCellWidths = ['10%', '30%', '20%', '20%', '20%'];
    var processorCellMinWidths = ['170px', '0', '0', '0', '0'];

    var itemsTableOffset = 3;
    var recipes = new Array<Recipe>();
    export var processors = new Array<Processor>();

    export class Processor {
        constructor() {

        }
        id: number;
        selectedRecipe: number;
        name: string;
        progressBar: HTMLElement;
        progressText: HTMLElement;
        recipeSelector: HTMLElement;
        capacityElm: HTMLElement;
        recipeList: HTMLElement;

        completedOperations: number;
        totalOperations: number;
        operationDuration: number;
        operationStartTime: number;
        operationCompletionTime: number;

        _recipes = new Array<Recipe>();

        addRecipe(recipe: Recipe) {
            this._recipes.push(recipe);
            if (!this.selectedRecipe)
                this.selectedRecipe = recipe.resultants[0].id;
        }
    }

    export class Recipe {
        constructor() {

        }
        id: number;
        name: string;
        isItem: boolean;
        row: HTMLElement;

        ingredients = new Array<Ingredient>();
        resultants = new Array<Ingredient>();

        addIngredient(ingredient: Ingredient) {
            this.ingredients.push(ingredient);
        }

        addResultant(ingredient: Ingredient) {
            this.resultants.push(ingredient);
        }
    }

    export class Ingredient {
        constructor(id: number, quantity: number) {
            this.id = id;
            this.quantity = quantity;
        }

        id: number;
        quantity: number;
        quantityDiv: HTMLElement;
    }

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

    function drawCraftingTable() {
        craftingTable = document.createElement('TABLE');
        craftingTable.classList.add('block-table');

        var header = (<HTMLTableElement>craftingTable).createTHead();
        var titleRow = (<HTMLTableElement>header).insertRow(0);
        titleRow.classList.add('table-subheader');
        var realTitleRow = (<HTMLTableElement>header).insertRow(0);
        realTitleRow.classList.add('table-header');

        var titleCell = (<HTMLTableRowElement>realTitleRow).insertCell(0);
        (<HTMLTableCellElement>titleCell).colSpan = cellDescriptions.length;
        titleCell.textContent = 'Crafting Table';

        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = (<HTMLTableRowElement>titleRow).insertCell(0);
            cell.style.width = cellWidths[i];
            cell.textContent = cellDescriptions[i];

            if (cellMinWidths[i] != '0')
                cell.style.minWidth = cellMinWidths[i];

        }

        var itemsSubHeader = (<HTMLTableElement>craftingTable).insertRow(2);
        itemsSubHeader.classList.add('table-subheader');
        var itemsSubHeaderCell = (<HTMLTableRowElement>itemsSubHeader).insertCell(0);
        (<HTMLTableCellElement>itemsSubHeaderCell).colSpan = cellDescriptions.length;
        itemsSubHeaderCell.textContent = 'Items';

        var upgradesSubHeader = (<HTMLTableElement>craftingTable).insertRow(3);
        upgradesSubHeader.classList.add('table-subheader');
        var upgradesSubHeaderCell = (<HTMLTableRowElement>upgradesSubHeader).insertCell(0);
        (<HTMLTableCellElement>upgradesSubHeaderCell).colSpan = cellDescriptions.length;
        upgradesSubHeaderCell.textContent = 'Upgrades';

        craftingSection.appendChild(craftingTable);
    }

    export function addRecipe(id: number, ingredients: any, resultants: any, isItem: boolean) {
        if (!storePane) draw();

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

    export function addProcessor(id: number, name: string) {
        if (!storePane) draw();

        if (!processors[id]) {
            var processor = new Processor();
            processor.id = id;
            processor.name = name;
            processors[id] = processor;

            drawProcessor(processor);
        }
    }

    export function addProcessorRecipe(id: number, ingredients: any, resultants: any) {
        if (!processors[id]) return;

        var processor = processors[id];
        var recipe = new Recipe();

        for (var i = 0; i < ingredients.length; i++)
            recipe.addIngredient(new Ingredient(ingredients[i].Id, ingredients[i].Quantity));

        for (var i = 0; i < resultants.length; i++)
            recipe.addResultant(new Ingredient(resultants[i].Id, resultants[i].Quantity));

        processor.addRecipe(recipe);

        var opt = document.createElement('OPTION');
        opt.textContent = Objects.lookupName(resultants[0].Id);
        processor.recipeSelector.appendChild(opt);

        if (processor._recipes.length == 1) { // if this was the first recipe we added.
            switchProcessorRecipe(id, 0);
        }
    }

    function switchProcessorRecipe(id: number, recipeIndex: number) {
        var processor = processors[id];
        if (!processor) return;

        var recipe = processor._recipes[recipeIndex];
        processor.selectedRecipe = recipe.resultants[0].id;

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
            ingredientQuantity.style.color = (recipe.ingredients[x].quantity <=Objects.getQuantity(recipe.ingredients[x].id)) ? 'darkgreen' : 'darkred';
            ingredientQuantity.textContent = Utils.formatNumber(recipe.ingredients[x].quantity);
            ingredientImage.classList.add("Half-" + Utils.cssifyName(Objects.lookupName(recipe.ingredients[x].id)));

            ingredientBox.appendChild(ingredientImage);
            ingredientBox.appendChild(ingredientQuantity);
            processor.recipeList.appendChild(ingredientBox);
         }
    }

    function drawProcessor(processor: Processor) {
        var processorTable = document.createElement('TABLE');
        processorTable.classList.add('block-table');

        var header = (<HTMLTableElement>processorTable).createTHead();
        var titleRow = (<HTMLTableElement>header).insertRow(0);
        titleRow.classList.add('table-subheader');
        var realTitleRow = (<HTMLTableElement>header).insertRow(0);
        realTitleRow.classList.add('table-header');

        var titleCell = (<HTMLTableRowElement>realTitleRow).insertCell(0);
        (<HTMLTableCellElement>titleCell).colSpan = cellDescriptions.length;
        titleCell.textContent = processor.name;

        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = (<HTMLTableRowElement>titleRow).insertCell(0);
            cell.style.width = processorCellWidths[i];
            cell.textContent = processorCellDescriptions[i];

            if (processorCellWidths[i] != '0')
                cell.style.minWidth = processorCellMinWidths[i];
        }
        // progress
        var progressRow = (<HTMLTableElement>processorTable).insertRow(2);
        progressRow.classList.add('table-row');
        var progressCell = (<HTMLTableRowElement>progressRow).insertCell(0);
        (<HTMLTableCellElement>progressCell).colSpan = processorCellDescriptions.length;
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

        var contentRow = (<HTMLTableElement>processorTable).insertRow(3);
        contentRow.classList.add('table-row');

        for (var i = 0; i < processorCellDescriptions.length; i++) {
            var cell = (<HTMLTableRowElement>contentRow).insertCell(0);
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
                (<HTMLInputElement>quantitySelector).type = 'TEXT';
                quantitySelector.style.width = '35px';
                cell.appendChild(quantitySelector);
                var id = processor.id;
                var selector = document.createElement('SELECT');
                selector.addEventListener('change', function (e) {
                    switchProcessorRecipe(id, (<HTMLSelectElement>selector).selectedIndex);
                });
                processor.recipeSelector = selector;
                cell.appendChild(selector);
            }
            
            if (processorCellDescriptions[i] == "Action") {
                var activateBtn = Utils.createButton('Activate', '');
                activateBtn.addEventListener('click', function () {//fix.
                    Connection.processRecipe(processor.id, (<HTMLSelectElement>processor.recipeSelector).selectedIndex, 1);
                }, false);
                cell.appendChild(activateBtn);
            }
        }

        processorSection.appendChild(processorTable);
    }

    export function updateProcessor(id: number, selectedRecipe: number, operationDuration: number, completedOperations: number, totalOperations: number, capacity: number) {
        var processor = processors[id];
        if (!processor) return;
        console.log('updating ' + processor.name);

        processor.selectedRecipe = selectedRecipe ? selectedRecipe : processor.selectedRecipe;
        

        if(totalOperations)
            processor.totalOperations = totalOperations;

        if(operationDuration)
            processor.operationDuration = operationDuration;

        if(capacity)
            processor.capacityElm.textContent = capacity.toString();

        if (processor.completedOperations != completedOperations) {
            console.log('New '+processor.name+' operation');
            processor.completedOperations = completedOperations;
            processor.operationStartTime = Date.now();
            processor.operationCompletionTime = processor.operationStartTime + (processor.operationDuration * 1000);
        } 

        if (processor.operationDuration > -1) {
            processor.progressText.textContent = Objects.lookupName(processor._recipes[processor.selectedRecipe].resultants[0].id);
        }
       /* if (processor.operationCompletionTime != operationCompletionTime) {
            processor.operationStartTime = Date.now() / 1000;
            processor.operationCompletionTime = operationCompletionTime;
        }*/
    }

    function processorBars() {
        processors.forEach(processor => {
            if (processor.operationDuration < 0)
                processor.progressBar.style.width = '0%';
            else if (processor.completedOperations != processor.totalOperations && processor.totalOperations > 0) {
                /*console.log(processor.operationCompletionTime);
                console.log(processor.operationStartTime);
                console.log(((((processor.operationCompletionTime - processor.operationStartTime) / processor.operationDuration))/10) + '%');*/
                processor.progressBar.style.width = ((((processor.operationCompletionTime - processor.operationStartTime) / processor.operationDuration))/10) + '%';
            }
        });
    }
    setInterval(processorBars, 10);

    export function update() {
        if (!storePane) return;

        recipes.forEach(recipe => {
            var quantity = Objects.getQuantity(recipe.id);
            recipe.row.style.display = (quantity == -1 || !recipe.isItem && quantity > 0) ? 'none' : '';
            recipe.ingredients.forEach(ingredient => {
                var ingQuantity = Objects.getQuantity(ingredient.id);
                ingredient.quantityDiv.style.color = (ingQuantity >= ingredient.quantity) ? 'darkgreen' : 'darkred';
            });
        });

        processors.forEach(processor => {
            switchProcessorRecipe(processor.id, (<HTMLSelectElement>processor.recipeSelector).selectedIndex);
        });
        /*
        for (var i = 0; i < recipes.length; i++) {
            var recipe = recipes[i];
            var quantity = Objects.getQuantity(recipe.id);
            recipe.row.style.display = (quantity == -1) ? 'none' : 'inline-block';
        }*/
    }

    function drawRecipe(recipe: Recipe, isItem: boolean) {
        var pointOfInsertion = (<HTMLTableElement>craftingTable).rows.length;

        if (isItem) {
            pointOfInsertion = itemsTableOffset;
            itemsTableOffset++;
        }

        var recipeRow = (<HTMLTableElement>craftingTable).insertRow(pointOfInsertion);
        recipeRow.classList.add('table-row');
        recipe.row = recipeRow;
        for (var i = 0; i < cellDescriptions.length; i++) {
            var cell = (<HTMLTableRowElement>recipeRow).insertCell(0);
            cell.style.width = cellWidths[i];

            if (cellMinWidths[i] != '0')
                cell.style.minWidth = cellMinWidths[i];

            if (cellDescriptions[i] == "Name")
                cell.textContent = Objects.lookupName(recipe.id);

            if (cellDescriptions[i] == "Input") {
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
                }
            }

            if (cellDescriptions[i] == "Output") {
                for (var x = 0; x < recipe.resultants.length; x++) {
                    var ingredientBox = document.createElement('DIV');
                    ingredientBox.classList.add('item-text');
                    ingredientBox.style.height = '22px';
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
                    (<HTMLInputElement>quantity).type = 'TEXT';
                    quantity.style.width = '30px';

                    var craftXBtn = Utils.createButton('Craft-x', '');
                    craftXBtn.addEventListener('click', function () {
                        Connection.craftRecipe(recipe.id, +(<HTMLInputElement>quantity).value);
                    }, false);
                    cell.appendChild(craftXBtn);
                    cell.appendChild(quantity);
                }
            }
        }
    }
}