var PagedTable;
(function (PagedTable) {
    /* class */
    var Creator = (function () {
        function Creator(tbodyId, paginationId) {
            this.tbody = $(`#${tbodyId}`);
            this.pagination = $(`#${paginationId}`);
            this.paginationId = paginationId;
            this.page = 0;
            this.cols = [{ prop: "", append: function () { } }];
        }
        Creator.prototype.setColumns = function (cols) {
            this.cols = cols;
        }
        Creator.prototype.createTable = function (data, count, shownRecords = 15) {
            this.tbody.text("");
            this.tBodyAppendText = appendText;
            data.forEach((element) => {
                var tds = "";
                Object.keys(element).forEach((val) => {
                    tds += `<td>${element[val]}</td>`;
                });
                this.tbody.prepend(
                    `<tr>` +
                    `<td></td>`
                );
            });
            this.tbody.append(appendText);

            this.pagination.html("");
            if (count > 0) {
                var pageCount = Math.ceil(count / shownRecords);
                for (var i = 0; i < pageCount; i++) {
                    this.pagination.append(`<li><a id="page-link-${i + 1}" data-value="${i}">${i + 1}</a></li>`);
                }
                var pageLinks = document.getElementById(`${this.paginationId}`).getElementsByTagName("a");
                for (var i = 0; i < pageLinks.length; i++) {
                    var element = pageLinks[i];
                    element.onclick = () => {
                        var val = element.getAttribute("data-value");
                        this.changePage(val);
                    }
                }
            }
        }

        Creator.prototype.changePage = function (i) {
            if (i < 0)
                return;
            this.page = i;

        }
        Creator.prototype.getData = function (ajaxOptions) {
            $.ajax({
                url: ajaxOptions.url,
                method: ajaxOptions.method,
                success: (res) => {
                    Creator.prototype.createTable(this.tBodyAppendText)
                }
            });
        }
        return Creator;
    }());
    PagedTable.Creator = Creator;
})(PagedTable || (PagedTable = {}));