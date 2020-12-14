$(document).ready(function () {
    var password1 = $('#password1'); //id of first password field
    var password2 = $('#password2'); //id of second password field
    var passwordsInfo = $('#pass-info'); //id of indicator element

    passwordStrengthCheck(password1, password2, passwordsInfo); //call password check function

});

function passwordStrengthCheck(password1, password2, passwordsInfo) {
    //Must contain 5 characters or more
    var WeakPass = /(?=.{5,}).*/;
    //Must contain lower case letters and at least one digit.
    var MediumPass = /^(?=\S*?[a-z])(?=\S*?[0-9])\S{5,}$/;
    //Must contain at least one upper case letter, one lower case letter and one digit.
    var StrongPass = /^(?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])\S{5,}$/;
    //Must contain at least one upper case letter, one lower case letter and one digit.
    var VryStrongPass = /^(?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[^\w\*])\S{5,}$/;

    $(password1).on('keyup', function (e) {
        if (VryStrongPass.test(password1.val())) {
            passwordsInfo.removeClass().addClass('vrystrongpass').html("Çok Güçlü! (Harika, Lütfen şifrenizi unutmayınız!)");
        }
        else if (StrongPass.test(password1.val())) {
            passwordsInfo.removeClass().addClass('strongpass').html("Güçlü! (Özel karakter girerek şifrenizi güçlendirin");
        }
        else if (MediumPass.test(password1.val())) {
            passwordsInfo.removeClass().addClass('goodpass').html("İyi! (Büyük harf girerek şifrenizi güçlendirin)");
        }
        else if (WeakPass.test(password1.val())) {
            passwordsInfo.removeClass().addClass('stillweakpass').html("Hala zayıf! (Sayı girerek iyi bir şifre oluşturun)");
        }
        else {
            passwordsInfo.removeClass().addClass('weakpass').html("Çok zayıf! (5 ya da daha fazla karakter içermeli)");
        }
    });

    $(password2).on('keyup', function (e) {

        if (password1.val() !== password2.val()) {
            passwordsInfo.removeClass().addClass('weakpass').html("Şifreler uyuşmuyor!");
        } else {
            passwordsInfo.removeClass().addClass('goodpass').html("Şifreler uyuşuyor!");
        }

    });
}