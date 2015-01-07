var Inventory;
(function (Inventory) {
    var items = new Array();

    var Item = (function () {
        function Item() {
            add(this);
        }
        return Item;
    })();

    function add(item) {
        items.push(item);
        // TODO: add dom logic and st00f here.
    }
})(Inventory || (Inventory = {}));
