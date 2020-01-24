var DateTimeSupport;
(function (DateTimeSupport) {
    var Manager = (function () {
        function Manager() {

        }
        Manager.prototype.getDateAsArray = function (date) {
            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();

            return [day, month, year];
        }
        Manager.prototype.getToday = function () {
            var date = this.getDateAsArray(new Date());
            return this.formatDate(date);
        }
        Manager.prototype.formatDate = function (dateArray) {
            var day = "", month = "";
            day = dateArray[0] < 10 ? "0" + dateArray[0] : dateArray[0];
            month = dateArray[1] < 10 ? "0" + dateArray[1] : dateArray[1];

            return `${dateArray[2]}-${month}-${day}`;
        }
        return Manager;
    }());

    DateTimeSupport.Manager = Manager;
})(DateTimeSupport || (DateTimeSupport = {}));