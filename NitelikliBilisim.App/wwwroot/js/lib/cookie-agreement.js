var StorageSupport;
(function (StorageSupport) {

    var Envoy = (function () {
        function Envoy() {
            this.registerForm = {
                name: "",
                surname: "",
                email: "",
                phone: "",
                address: "",
                city: "",
                country: ""
            };
        }

        Envoy.prototype.isStorageExists = function (storageName) {
            return localStorage.getItem(storageName) != null;
        }

        Envoy.prototype.deleteStorage = function (storageName) {
            if (this.isStorageExists(storageName))
                localStorage.removeItem(storageName);
        }

        Envoy.prototype.createRegisterForm = function () {
            localStorage.setItem("register", JSON.stringify(this.registerForm));
        }

        Envoy.prototype.setRegisterForm = function (registerForm) {
            if (registerForm.name)
                this.registerForm.name = registerForm.name;
            if (registerForm.surname)
                this.registerForm.surname = registerForm.surname;
            if (registerForm.email)
                this.registerForm.email = registerForm.email;
            if (registerForm.phone)
                this.registerForm.phone = registerForm.phone;
            if (registerForm.address)
                this.registerForm.address = registerForm.address;
            if (registerForm.city)
                this.registerForm.city = registerForm.city;
            if (registerForm.country)
                this.registerForm.country = registerForm.country;

            localStorage.setItem("register", JSON.stringify(this.registerForm));
        }

        Envoy.prototype.getRegisterForm = function () {
            this.registerForm = JSON.parse(localStorage.getItem("register"));
            return this.registerForm;
        }

        return Envoy;
    }());
    StorageSupport.Envoy = Envoy;
})(StorageSupport || (StorageSupport = {}));