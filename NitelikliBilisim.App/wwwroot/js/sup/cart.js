var CartSupport;
(function (CartSupport) {

/* class */
    var Cart = (function () {
        function Cart() {

        }

        Cart.prototype.ensureStorageCreated = function () {
            if (!localStorage.getItem("cart"))
                localStorage.setItem("cart", JSON.stringify([]));
        }

        Cart.prototype.addToCart = function (educationId) {
            this.ensureStorageCreated();
            var deserialized = JSON.parse(localStorage.getItem("cart"));
            if (deserialized.indexOf(educationId) === -1) {
                deserialized.push(educationId);
                localStorage.setItem("cart", JSON.stringify(deserialized));
            }
        }

        Cart.prototype.getItems = function () {
            this.ensureStorageCreated();
            return JSON.parse(localStorage.getItem("cart"));
        }

        return Cart;
    }());
    CartSupport.Cart = Cart;
})(CartSupport || (CartSupport = {}));