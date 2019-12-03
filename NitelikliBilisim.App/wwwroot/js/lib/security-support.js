var SecuritySupport;
(function (SecuritySupport) {

    /*class*/
    var TokenVerifier = (function () {
        function TokenVerifier() {

        }

        TokenVerifier.prototype.addToken = function (form, data) {
            data["__RequestVerificationToken"] = $('input[name="__RequestVerificationToken"]', $(`#form`)).val();
            return data;
        }

        return TokenVerifier;
    }());
    SecuritySupport.TokenVerifier = TokenVerifier;
})(SecuritySupport || (SecuritySupport = {}));