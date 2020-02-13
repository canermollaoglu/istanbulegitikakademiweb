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

        Cart.prototype.addToCart = function (educationId, hostId) {
            this.ensureStorageCreated();
            var deserialized = JSON.parse(localStorage.getItem("cart"));
            if (deserialized.filter(x => x.educationId == educationId).length === 0) {
                deserialized.push({ educationId: educationId, hostId: hostId });
                localStorage.setItem("cart", JSON.stringify(deserialized));
            }
        }

        Cart.prototype.removeFromCart = function (educationId) {
            this.ensureStorageCreated();
            var deserialized = JSON.parse(localStorage.getItem("cart"));
            deserialized = deserialized.filter(x => x.educationId != educationId);
            localStorage.setItem("cart", JSON.stringify(deserialized));
        }

        Cart.prototype.getItems = function () {
            this.ensureStorageCreated();
            return JSON.parse(localStorage.getItem("cart"));
        }

        Cart.prototype.getItemCount = function () {
            this.ensureStorageCreated();
            var items = JSON.parse(localStorage.getItem("cart"));
            return items.length;
        }

        Cart.prototype.clearCart = function () {
            localStorage.setItem("cart", JSON.stringify([]));
        }

        return Cart;
    }());
    CartSupport.Cart = Cart;
})(CartSupport || (CartSupport = {}));