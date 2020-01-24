var StorageSupport;
(function (StorageSupport) {

    var Envoy = (function () {
        function Envoy() {

        }

        Envoy.prototype.isStorageExists = function (storageName) {
            return localStorage.getItem(storageName) != null;
        }

        Envoy.prototype.deleteStorage = function (storageName) {
            if (this.isStorageExists(storageName))
                localStorage.removeItem(storageName);
        }

        Envoy.prototype.createRegisterForm = function () {
            localStorage.setItem("register", JSON.stringify({
                name: "",
                surname: "",
                email: "",
                phone: "",
                isNbuy: ""
            }));
        }

        Envoy.prototype.setRegisterForm = function (registerForm) {
            var stored = {};
            if (registerForm.name)
                stored.name = registerForm.name;
            if (registerForm.surname)
                stored.surname = registerForm.surname;
            if (registerForm.email)
                stored.email = registerForm.email;
            if (registerForm.phone)
                stored.phone = registerForm.phone;
            if (registerForm.isNbuy)
                stored.isNbuy = registerForm.isNbuy;
            localStorage.setItem("register", JSON.stringify(stored));
        }

        Envoy.prototype.getRegisterForm = function () {
            var stored = JSON.parse(localStorage.getItem("register"));
            return stored;
        }

        return Envoy;
    }());
    StorageSupport.Envoy = Envoy;
})(StorageSupport || (StorageSupport = {}));