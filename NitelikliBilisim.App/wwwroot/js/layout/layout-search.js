var AppWide;
(function (AppWide) {
    var Searcher = (function () {
        function Searcher() {

        }

        Searcher.prototype.initialize = function () {
            var btnSearch = $("#btn-hidden-search");

            btnSearch.on("click", () => {
                var inputSearchBox = $("#input-hidden-search");
                
                var tagFormatter = new AppWide.TagFormatter();
                var searchText = tagFormatter.formatForTag(inputSearchBox.val());

                if (!searchText || searchText === "")
                    return;

                location.href = `/egitimler?s=${searchText}`;
            });
        }

        return Searcher;
    }());
    AppWide.Searcher = Searcher;

    var TagFormatter = (function () {
        function TagFormatter() {

        }

        TagFormatter.prototype.formatForTag = function (text) {
            text = this.characterConverter(text);

            var splitted = text.split(" ");

            splitted = splitted.filter((e) => {
                return e;
            });

            //for (var i = 0; i < splitted.length; i++) {
            //    if (splitted[i] === "") {
            //        splitted.splice(i, 1);
            //        i--;
            //    }
            //}

            var result = "";
            for (var i = 0; i < splitted.length; i++) {
                result += i === splitted.length - 1 ? splitted[i] : `${splitted[i]}-`;
            }

            return result;
        }

        TagFormatter.prototype.characterConverter = function (text) {
            return text.trim().toLowerCase()
                .replace("-", " ")
                .replace("\"", "")
                .replace("!", "")
                .replace("'", "")
                .replace("^", "")
                .replace("#", "sharp")
                .replace("+", "")
                .replace("$", "")
                .replace("%", "")
                .replace("&", "")
                .replace("/", "")
                .replace("{", "")
                .replace("(", "")
                .replace("[", "")
                .replace(")", "")
                .replace("]", "")
                .replace("=", "")
                .replace("}", "")
                .replace("?", "")
                .replace("*", "")
                .replace("\\", "")
                .replace("_", "")
                .replace("@", "")
                .replace("€", "")
                .replace("~", "")
                .replace("¨", "")
                .replace("´", "")
                .replace(";", "")
                .replace(",", "")
                .replace(":", "")
                .replace(".", "")
                .replace("<", "")
                .replace(">", "")
                .replace("|", "")
                .replace("ı", "i")
                .replace("ö", "o")
                .replace("ü", "u")
                .replace("ş", "s")
                .replace("ç", "c")
                .replace("ğ", "g");
        }

        return TagFormatter;
    }());
    AppWide.TagFormatter = TagFormatter;
})(AppWide || (AppWide = {}));